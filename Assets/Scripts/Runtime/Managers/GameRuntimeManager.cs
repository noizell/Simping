using IGJ.SIMP.Runtime.Actors;
using UnityEngine;

namespace IGJ.SIMP.Runtime.Managers
{
    public enum GameState { Menu = 0, Game = 1, Cutscene = 2 };
    public enum PlayerState { Collide = 1, Normal = 2 };

    public static class GameRuntimeManager
    {
        public static GameState CurrentState;

        public const float GlobalAttackTime = 2.2f;

        public static int CurrentTime { get; private set; }
        public static PlayerState CurrentPlayerState { get; private set; }
        public static Transform PlayerTransform { get; private set; }
        public static ActorActionHandler PlayerActor { get; private set; }
        public static bool PlayerIsEnable { get; private set; }

        static ActorMovement actorMovement;
        static ActorActionHandler actionHandler;
        static MobSpawner mobSpawner;

        static Cinemachine.CinemachineVirtualCameraBase gameCamera, titleCamera;


        public static MobSpawner Spawner
        {
            get
            {
                if (mobSpawner == null)
                    mobSpawner = GameObject.FindObjectOfType<MobSpawner>();

                return mobSpawner;
            }
        }

        public static int DynamicFollowerAdded
        {
            get
            {
                if (PlayerActor.FollowList < 10)
                    return 0;

                return Mathf.CeilToInt(PlayerActor.FollowList / 2);
            }
        }

        public static void UpdateTime(int newTime)
        {
            CurrentTime = newTime;
        }

        public static float CountAdditionalTime(int followCount, float multiplier = 1f)
        {
            return followCount > 0 ? followCount * multiplier : 1f;
        }

        public static void EnablePlayer(bool activate)
        {
            if (PlayerTransform == null)
            {
                PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
                PlayerActor = PlayerTransform.GetComponent<ActorActionHandler>();
                actorMovement = PlayerTransform.GetComponent<ActorMovement>();
                actionHandler = PlayerTransform.GetComponent<ActorActionHandler>();
            }

            actorMovement.EnableBehaviour(activate);
            actionHandler.EnableBehaviour(activate);

            PlayerIsEnable = activate;
        }

        public static void SetGameCameraPriority(int p = 10)
        {
            if (gameCamera == null || titleCamera == null)
            {
                gameCamera = GameObject.FindGameObjectWithTag("Game Camera").GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
                titleCamera = GameObject.FindGameObjectWithTag("Title Camera").GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
            }

            gameCamera.Priority = p;
        }

        public static void SetTitleCameraPriority(int p = 10)
        {
            if (gameCamera == null || titleCamera == null)
            {
                gameCamera = GameObject.FindGameObjectWithTag("Game Camera").GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
                titleCamera = GameObject.FindGameObjectWithTag("Title Camera").GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
            }

            titleCamera.Priority = p;
        }

        public static void SetPlayerState(PlayerState ps)
        {
            CurrentPlayerState = ps;
        }
    }
}
