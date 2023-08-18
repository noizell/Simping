using UnityEngine;
using UnityEngine.AI;

namespace IGJ.SIMP.Runtime.AI
{

    public class NavMeshBaker : MonoBehaviour
    {
        [SerializeField] bool bakeAtAwake = true;
        NavMeshSurface[] surfaces;

        private void Awake()
        {
            if (bakeAtAwake)
                BakeCapturedNavmesh();
        }

        public void BakeCapturedNavmesh()
        {
            FindNavmeshSurface();

            for (int i = 0; i < surfaces.Length; i++)
            {
                surfaces[i].BuildNavMesh();
            }
        }

        private void FindNavmeshSurface()
        {
            surfaces = GameObject.FindObjectsOfType<NavMeshSurface>();
        }
    }
}
