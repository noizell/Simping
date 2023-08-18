using NPP.TaskTimers;
using UnityEngine;
using UnityEngine.AI;

namespace IGJ.SIMP.Runtime.Actors
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ActorAiMovement : MonoBehaviour
    {
        NavMeshAgent agent;
        [SerializeField] float lookDistance = 4f;
        [SerializeField] float minDurationExpired = 5f;
        [SerializeField] float maxDurationExpired = 15f;
        [SerializeField] float idleDuration = 4f;
        [SerializeField] float wanderDuration = 4f;
        [SerializeField] float followCheckDuration = .8f;

        [SerializeField] Animator animator;

        NavMeshHit navHit;
        Vector3 randDirection;
        AiState currentState;

        bool delayed = false;

        public AiState CurrentState => currentState;

        ActorAiBehaviour aiBehaviour;
        Transform playerActor;

        TaskDelay decay;

        private void Awake()
        {
            playerActor = GameObject.FindGameObjectWithTag("Player").transform;
            decay = TaskTimer.CreateTask(.001f);
            agent = GetComponent<NavMeshAgent>();
            aiBehaviour = GetComponent<ActorAiBehaviour>();
            currentState = AiState.Idle;
        }

        private void Start()
        {
            TaskTimer.CreateConditionalTask(.1f, () => { return Managers.GameRuntimeManager.CurrentState != Managers.GameState.Game; }, (int i) =>
            {
                if (aiBehaviour.Charmed)
                {
                    //AnyState -> Follow
                    if (delayed) return;
                    delayed = true;

                    animator.Play("Walking");

                    currentState = AiState.Follow;
                    TaskTimer.CreateTask(followCheckDuration, () =>
                    {
                        delayed = false;
                    });

                    if (agent != null)
                        if (agent.isOnNavMesh)
                            agent.destination = playerActor.position;
                }

                if (delayed) return;
                delayed = true;

                //idle -> Wander
                if (currentState == AiState.Idle)
                {
                    TaskTimer.CreateTask(wanderDuration, () =>
                    {
                        delayed = false;
                    });


                    animator.Play("Walking");

                    randDirection = Vector3.zero;
                    currentState = AiState.Wander;
                    randDirection = Random.insideUnitSphere * lookDistance;
                    randDirection += transform.position;
                    NavMesh.SamplePosition(randDirection, out navHit, lookDistance, 1);

                    if (navHit.hit)
                        if (agent.isOnNavMesh)
                            agent.destination = navHit.position;

                }

                //Wander -> Idle
                if (currentState == AiState.Wander)
                {
                    TaskTimer.CreateTask(idleDuration, () =>
                    {
                        delayed = false;
                    });

                    //animator.Play("Idle");

                    currentState = AiState.Idle;
                }

                //Follow -> Idle
                if (decay.Completed())
                {
                    if (currentState != AiState.Follow) return;
                    decay = TaskTimer.CreateTask(Random.Range(minDurationExpired, maxDurationExpired), () =>
                    {
                        TaskTimer.CreateTask(idleDuration, () =>
                        {
                            delayed = false;
                        });

                        currentState = AiState.Idle;
                        aiBehaviour.SetCharmed(false);
                    });
                }
            });
        }

        public bool OnMove()
        {
            if (!agent.isOnNavMesh) return false;
            return agent.velocity.sqrMagnitude > .1f;
        }

        public enum AiState { Wander, Follow, Dead, Idle };

    }

}