using IGJ.SIMP.Runtime.Actors;
using NPP.TaskTimers;
using UnityEngine;
using UnityEngine.AI;

namespace IGJ.SIMP.Runtime.Managers
{

    public class MobSpawner : MonoBehaviour
    {
        [SerializeField] ActorAiBehaviour[] mobPrefabs;
        [SerializeField] float spawnDelay = 3f;
        [SerializeField] int maxMobActive = 125;
        [SerializeField] float radiusToSpawnFromPlayer = 4f;

        int currentMobSpawnedCount;
        NavMeshHit navHit;

        private void Start()
        {
            TaskTimer.CreateConditionalTask(spawnDelay, () => { return GameRuntimeManager.CurrentState != GameState.Game; }, (int i) =>
            {
                if (currentMobSpawnedCount < maxMobActive)
                {
                    SpawnMob();
                }
            });
        }

        public ActorAiBehaviour SpawnMob()
        {
            Vector3 position = (GameRuntimeManager.PlayerTransform.position + Random.insideUnitSphere * radiusToSpawnFromPlayer);
            position.y = GameRuntimeManager.PlayerTransform.localPosition.y;

            NavMesh.SamplePosition(position, out navHit, radiusToSpawnFromPlayer, 1);

            if (navHit.hit)
            {
                var spawned = Instantiate(mobPrefabs[UnityEngine.Random.Range(0, mobPrefabs.Length)], position, Quaternion.identity);
                return spawned;
            }

            return null;
        }
    }
}
