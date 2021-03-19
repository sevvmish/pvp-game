using UnityEngine;
using System.Collections;

namespace TerrainComposer2
{
    public class TC_RandomSpawner : MonoBehaviour
    {
        public GameObject spawnObject;
        public float posOffsetY = 1;
        public Vector2 posRangeX = new Vector2(-1000, 1000);
        public Vector2 posRangeZ = new Vector2(-1000, 1000);
        public Vector2 rotRangeY = new Vector2(-180, 180);
        public bool spawnOnStart;

        // Use this for initialization
        void Start()
        {
            if (spawnOnStart) Spawn();
        }

        public GameObject Spawn()
        {
            if (spawnObject == null) return null;

            Vector3 pos = transform.position;
            pos.x += Random.Range(posRangeX.x, posRangeX.y) * transform.localScale.x;
            pos.z += Random.Range(posRangeZ.x, posRangeZ.y) * transform.localScale.z;
            pos.y = SampleTerrainHeight(pos) + posOffsetY;

            Vector3 rot = new Vector3(0, Random.Range(rotRangeY.x, rotRangeY.y), 0);

            GameObject go = (GameObject)Instantiate(spawnObject, pos, Quaternion.Euler(rot));
            return go;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3((posRangeX.y - posRangeX.x) * transform.localScale.x * 2, 100, (posRangeZ.y - posRangeZ.x)) * transform.localScale.z * 2);
        }
        
        float SampleTerrainHeight(Vector3 pos)
        {
            TC_TerrainArea terrainArea = TC_Generate.instance.area2D.terrainAreas[0];

            for (int i = 0; i < terrainArea.terrains.Count; i++)
            {
                TCUnityTerrain tcTerrain = terrainArea.terrains[i];
                if (tcTerrain.terrain == null) continue;
                if (tcTerrain.terrain.terrainData == null) continue;

                Vector3 posTerrain = tcTerrain.terrain.transform.position;
                Vector3 sizeTerrain = tcTerrain.terrain.terrainData.size;

                Rect rect = new Rect(posTerrain.x, posTerrain.z, sizeTerrain.x, sizeTerrain.z);

                if (rect.Contains(pos))
                {
                    return tcTerrain.terrain.SampleHeight(pos);
                }
            }

            return -1;
        }
    }
}
