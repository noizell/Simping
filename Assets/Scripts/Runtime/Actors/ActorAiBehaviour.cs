using IGJ.SIMP.Runtime.AI;
using MoreMountains.Feedbacks;
using ToolBox.Pools;
using UnityEngine;

namespace IGJ.SIMP.Runtime.Actors
{
    public enum WeaponType { Keyboard, Pot, Galon }

    public class ActorAiBehaviour : MonoBehaviour
    {
        [SerializeField] GameObject weaponPrefabs;
        [SerializeField] WeaponType weaponType;
        [SerializeField] MMF_Player charmedFeedback;
        [SerializeField] MMF_Player brokenHeartFeedback;
        [SerializeField] MMF_Player simpingFeedback;
        [SerializeField] MMF_Player MovementFeedback;

        ActorAiMovement actorAiMovement;

        private void Awake()
        {
            actorAiMovement = GetComponent<ActorAiMovement>();
        }

        public bool Charmed { get; protected set; }
        bool spawnedOnce = false;

        public void SetCharmed(bool charmStatus)
        {
            Charmed = charmStatus;

            if (Charmed)
            {
                if (spawnedOnce) return;
                spawnedOnce = true;
                charmedFeedback.PlayFeedbacks();
            }
            else if (!Charmed)
            {
                brokenHeartFeedback.PlayFeedbacks();
            }
        }

        private void Update()
        {
            if (Charmed)
            {
                if (!simpingFeedback.IsPlaying)
                    simpingFeedback.PlayFeedbacks();
            }
            else
            {
                if (simpingFeedback.IsPlaying)
                    simpingFeedback.StopFeedbacks();
            }

            if (actorAiMovement.OnMove() && actorAiMovement.CurrentState != ActorAiMovement.AiState.Follow)
            {
                if (!MovementFeedback.IsPlaying)
                    MovementFeedback.PlayFeedbacks();
            }
        }

        public void SetAttackToTarget(Transform transform)
        {
            switch (weaponType)
            {
                case WeaponType.Pot:
                    var p = weaponPrefabs.Reuse<ThrowablePotBehaviour>();
                    p.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y + 1.2f, this.transform.localPosition.z);
                    p.LaunchToTarget(transform, p.gameObject.Release);
                    break;

                case WeaponType.Keyboard:
                    var k = weaponPrefabs.Reuse<ThrowableKeyboardBehaviour>();
                    k.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y + 1.2f, this.transform.localPosition.z);
                    k.LaunchToTarget(transform, k.gameObject.Release);
                    break;

                case WeaponType.Galon:
                    var g = weaponPrefabs.Reuse<ThrowableGalonBehaviour>();
                    g.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y + 1.2f, this.transform.localPosition.z);
                    g.LaunchToTarget(transform, g.gameObject.Release);
                    break;
            }
        }
    }

}