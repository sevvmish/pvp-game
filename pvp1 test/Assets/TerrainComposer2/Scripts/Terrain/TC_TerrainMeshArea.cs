using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TerrainComposer2
{
    [ExecuteInEditMode]
    public class TC_TerrainMeshArea : MonoBehaviour
    {
        public List<MeshTerrain> terrains = new List<MeshTerrain>();
        public bool refresh;

        void OnEnable()
        {
            // GetTerrains();
        }

        void Update()
        {
            if (refresh)
            {
                refresh = false;
                GetTerrains();
            }
        }

        void GetTerrains()
        {
            terrains.Clear();

            Transform[] transforms = GetComponentsInChildren<Transform>();

            for (int i = 1; i < transforms.Length; i++)
            {
                Debug.Log(transforms[i].name);
                terrains.Add(new MeshTerrain(transforms[i]));
            }
        }
    }
}