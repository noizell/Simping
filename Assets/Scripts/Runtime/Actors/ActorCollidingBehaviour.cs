using UnityEngine;
using DG.Tweening;
using NPP.TaskTimers;

namespace IGJ.SIMP.Runtime.Actors
{
    public class ActorCollidingBehaviour : MonoBehaviour
    {
        [SerializeField] MoreMountains.Feedbacks.MMF_Player collidingFeedbacks;

        private CharacterController controller;

        TaskDelay collidingDelay;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            collidingDelay = TaskTimer.CreateTask(.0001f);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Interactable") && Managers.GameRuntimeManager.CurrentPlayerState != Managers.PlayerState.Collide)
            {
                if (collidingDelay.Completed())
                {
                    Managers.GameRuntimeManager.SetPlayerState(Managers.PlayerState.Collide);
                    collidingFeedbacks.PlayFeedbacks();

                    collidingDelay = TaskTimer.CreateTask(.2f, () =>
                    {
                        Managers.GameRuntimeManager.SetPlayerState(Managers.PlayerState.Normal);
                    });

                }
            }
        }
    }

}