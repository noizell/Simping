using IGJ.SIMP.Runtime.Managers;
using MoreMountains.Feedbacks;
using NPP.TaskTimers;
using System.Collections.Generic;
using UnityEngine;

namespace IGJ.SIMP.Runtime.Actors
{

    public class ActorActionHandler : MonoBehaviour
    {
        [SerializeField] private float detectRadius = 3f;
        [SerializeField] private float initialPickDelay = .4f;
        [SerializeField] private float maxPickDelay = 3f;
        [SerializeField] private float incrementPickDelay = .2f;
        Collider[] cols;

        [Header("Feedbacks")]
        [SerializeField] MMF_Player fillerFeedback;
        [SerializeField] MMF_Player requiredNumFeedback;

        List<ActorAiBehaviour> SimpList;
        TaskDelay pickingDelay;
        bool completePickState = false;
        float calculatedPickDelay = 0;

        public bool Enable { get; protected set; }

        public int FollowList { get => SimpList.Count; }

        private void Start()
        {
            SimpList = new List<ActorAiBehaviour>();
            pickingDelay = TaskTimer.CreateTask(.0001f);

            TaskTimer.CreateConditionalTask(.2f, () => { return GameRuntimeManager.CurrentState != GameState.Game; }, (int i) =>
            {
                if (!Enable) return;

                cols = Physics.OverlapSphere(transform.position, detectRadius, LayerMask.NameToLayer("Simp"));

                if (cols.Length > 1)
                {
                    if (pickingDelay.Completed())
                    {
                        completePickState = false;
                        calculatedPickDelay = SimpList.Count > 0 ? Mathf.Clamp(SimpList.Count * incrementPickDelay, initialPickDelay, maxPickDelay) : 0;

                        pickingDelay = TaskTimer.CreateTask(initialPickDelay + calculatedPickDelay, () =>
                          {
                              for (int i = 0; i < cols.Length; i++)
                              {
                                  if (cols[i] != null)
                                      if (cols[i].gameObject.GetComponentInParent<ActorAiBehaviour>() != null)
                                      {
                                          cols[i].gameObject.GetComponentInParent<ActorAiBehaviour>().SetCharmed(true);

                                          if (!SimpList.Contains(cols[i].gameObject.GetComponentInParent<ActorAiBehaviour>()))
                                          {
                                              SimpList.Add(cols[i].gameObject.GetComponentInParent<ActorAiBehaviour>());
                                              fillerFeedback.GetFeedbackOfType<MMF_TMPText>().NewText = FollowList.ToString();
                                              fillerFeedback.PlayFeedbacks();
                                          }
                                      }
                              }
                              completePickState = true;
                          });
                    }
                }
            });
        }

        public void AddToList(ActorAiBehaviour simp)
        {
            SimpList.Add(simp);
            TaskTimer.CreateTask(.1f, () =>
            {
                fillerFeedback.GetFeedbackOfType<MMF_TMPText>().NewText = FollowList.ToString();
                fillerFeedback.PlayFeedbacks();
            });
        }

        public bool IsFollowerGatherAroundPlayer()
        {
            for (int i = 0; i < SimpList.Count; i++)
            {
                if (Vector3.Distance(transform.localPosition, SimpList[i].transform.localPosition) > 4f)
                {
                    return false;
                }
            }
            return true;
        }

        public void TriggerOpenBoon(int reqFollower)
        {
            requiredNumFeedback.GetFeedbackOfType<MMF_TMPText>().NewText = reqFollower.ToString();
            if (!requiredNumFeedback.IsPlaying)
                requiredNumFeedback.PlayFeedbacks();
        }

        public void EnableBehaviour(bool v)
        {
            Enable = v;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, detectRadius);
        }

        public void FollowersAttackTarget(Transform transform)
        {
            for (int i = 0; i < SimpList.Count; i++)
            {
                SimpList[i].GetComponent<ActorAiBehaviour>().SetAttackToTarget(transform);
            }
        }
    }

}