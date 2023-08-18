using IGJ.SIMP.Runtime.Managers;
using MoreMountains.Feedbacks;
using NPP.TaskTimers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IGJ.SIMP.Runtime.Effects
{
    [RequireComponent(typeof(SphereCollider))]
    public class DestructableObject : MonoBehaviour
    {
        [SerializeField] int requiredFollowCount = 10;
        [SerializeField] int spawnBoonCount = 3;
        [SerializeField] int addedBonusTime = 10;
        [SerializeField] float particleSizeMult = 10;
        [SerializeField] ParticleSystem explosionParticlePrefab;
        [SerializeField] ParticleSystem sparklingParticlePrefab;

        TaskDelay anarkisCounter, anarkisDelay;

        MMF_Player destructionFeedbacks;
        MMF_Flash destructionFlash;
        MMF_ParticlesInstantiation destructionParticleSpawner;
        MMF_SetActive destructionSetActive;

        ParticleSystem sparklingParticle;

        bool stopThrowing = true;

        private void Start()
        {
            anarkisCounter = TaskTimer.CreateTask(.0001f);
            anarkisDelay = TaskTimer.CreateTask(.0001f);
            destructionFeedbacks = new GameObject("Destruction Feedback").AddComponent<MMF_Player>();
            destructionFeedbacks.transform.SetParent(transform);

            sparklingParticle = Instantiate(sparklingParticlePrefab, transform);
            sparklingParticle.transform.localPosition = new Vector3(0, 1.5f, 0);
            sparklingParticle.loop = true;
            sparklingParticle.Simulate(0f, true, true);
            sparklingParticle.Play(true);
            FeedbackSetup();
        }

        private void FeedbackSetup()
        {
            if (destructionFeedbacks != null)
            {
                destructionFeedbacks.Initialization();

                destructionFlash = destructionFeedbacks.AddFeedback(typeof(MMF_Flash)) as MMF_Flash;
                destructionParticleSpawner = destructionFeedbacks.AddFeedback(typeof(MMF_ParticlesInstantiation)) as MMF_ParticlesInstantiation;
                destructionSetActive = destructionFeedbacks.AddFeedback(typeof(MMF_SetActive)) as MMF_SetActive;

                destructionFlash.FlashDuration = .25f;
                destructionFlash.FlashAlpha = 1.2f;
                destructionFlash.Initialization(destructionFeedbacks);

                destructionParticleSpawner.Mode = MMF_ParticlesInstantiation.Modes.OnDemand;
                destructionParticleSpawner.ParticlesPrefab = explosionParticlePrefab;
                destructionParticleSpawner.PositionMode = MMF_ParticlesInstantiation.PositionModes.Transform;
                destructionParticleSpawner.InstantiateParticlesPosition = transform;
                destructionParticleSpawner.NestParticles = false;
                destructionParticleSpawner.RandomParticlePrefabs = new List<ParticleSystem>() { explosionParticlePrefab };
                destructionParticleSpawner.ForceSetActiveOnPlay = true;
                destructionParticleSpawner.Initialization(destructionFeedbacks);

                destructionSetActive.TargetGameObject = gameObject;
                destructionSetActive.SetStateOnPlay = true;
                destructionSetActive.StateOnPlay = MMF_SetActive.PossibleStates.Inactive;
                destructionSetActive.Initialization(destructionFeedbacks);
                destructionSetActive.SetInitialDelay(.21f);

                destructionFeedbacks.RefreshCache();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform == GameRuntimeManager.PlayerTransform)
            {
                GameRuntimeManager.PlayerActor.TriggerOpenBoon(requiredFollowCount + GameRuntimeManager.DynamicFollowerAdded);

                if (GameRuntimeManager.PlayerActor.FollowList >= requiredFollowCount + GameRuntimeManager.DynamicFollowerAdded)
                {
                    stopThrowing = false;

                    TaskTimer.CreateConditionalTask(.3f, () => { return stopThrowing; }, (int i) =>
                    {
                        TaskTimer.CreateTask(Random.Range(.1f, .5f), () =>
                        {
                            GameRuntimeManager.PlayerActor.FollowersAttackTarget(other.transform);
                        });
                    });

                    if (anarkisCounter.Completed())
                    {
                        anarkisCounter = TaskTimer.CreateConditionalTask(.3f, () => { return GameRuntimeManager.PlayerActor.IsFollowerGatherAroundPlayer(); }, null, () =>
                        {
                            if (anarkisDelay.Completed())
                            {

                                anarkisDelay = TaskTimer.CreateTask(GameRuntimeManager.GlobalAttackTime + GameRuntimeManager.CountAdditionalTime(GameRuntimeManager.PlayerActor.FollowList, .2f), () =>
                                  {
                                      TaskTimer.CreateTask(.3f, () =>
                                      {
                                          if (spawnBoonCount > 0)
                                          {
                                              for (int i = 0; i < spawnBoonCount; i++)
                                              {
                                                  var mob = GameRuntimeManager.Spawner.SpawnMob();
                                                  mob.SetCharmed(true);
                                                  GameRuntimeManager.PlayerActor.AddToList(mob);
                                              }
                                          }

                                      });

                                      stopThrowing = true;
                                      destructionFeedbacks.PlayFeedbacks();
                                      GameRuntimeManager.UpdateTime(GameRuntimeManager.CurrentTime + addedBonusTime);
                                  });
                            }
                        });
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            stopThrowing = true;
            anarkisDelay.Stop();
            anarkisCounter.Stop();
        }

    }

}