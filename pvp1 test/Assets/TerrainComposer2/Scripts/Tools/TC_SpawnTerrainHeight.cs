using UnityEngine;
using System.Collections;

namespace TerrainComposer2
{
    [ExecuteInEditMode]
    public class TC_SpawnTerrainHeight : MonoBehaviour
    {
        public float heightOffset = 0;
        Transform t;

        void Start()
        {
            t = transform;

            SetSpawnHeight();
        }

        #if UNITY_EDITOR
        void Update()
        {
            if (Application.isPlaying) return;
            SetSpawnHeight();
        }
        #endif

        void SetSpawnHeight()
        {
            Ray ray = new Ray(t.position + new Vector3(0, 10000, 0), Vector3.down);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                t.position = new Vector3(t.position.x, hit.point.y + heightOffset, t.position.z);
            }
        }
    }
}
