using IGJ.SIMP.Runtime.Managers;
using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IGJ.SIMP.Runtime.Actors
{
    public class ActorMovement : MonoBehaviour
    {
        private CharacterController controller;
        private Vector3 playerVelocity;
        private bool groundedPlayer;

        [SerializeField] private float playerSpeed = 2.0f;
        private float jumpHeight = 1.0f;
        private float gravityValue = -9.81f;
        public bool Enable { get; protected set; }

        [Header("Feedbacks")]
        [SerializeField] private MMF_Player movementFeedback;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        void Update()
        {
            if (!Enable && GameRuntimeManager.CurrentPlayerState != PlayerState.Collide) return;

            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            controller.Move(move * Time.deltaTime * playerSpeed);

            if (!movementFeedback.IsPlaying && (move.x != 0 || move.z != 0))
                movementFeedback.PlayFeedbacks();

            if (move != Vector3.zero)
            {
                gameObject.transform.forward = move;
            }

            //// Changes the height position of the player..
            //if (Input.GetButtonDown("Jump") && groundedPlayer)
            //{
            //    playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            //}

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }

        public void EnableBehaviour(bool v)
        {
            Enable = v;
        }
    }

}