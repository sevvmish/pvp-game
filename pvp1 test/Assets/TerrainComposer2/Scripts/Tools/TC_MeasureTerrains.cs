using UnityEngine;
using System.Collections;

namespace TerrainComposer2
{
    public class TC_MeasureTerrains : MonoBehaviour
    {
        public bool locked;

        public RaycastHit hit;
        public Terrain terrain;
        public MeshRenderer mr;
        public float normalizedHeight, height, angle;

        public int textureSize = 50;

        public float[,,] splat;
        public Vector3 size;
        public int splatResolution;
        public Vector2 splatConversion, localPos;

        public GrassLayer[] grassLayer;
        public int grassResolution;
        public Vector2 grassConversion, grassLocalPos;

        public bool drawSplat, drawGrass;

        public void ReadTerrain()
        {
            size = terrain.terrainData.size;
            height = hit.point.y - terrain.transform.position.y;
            normalizedHeight = height / size.y;

            localPos = new Vector2(hit.point.x - terrain.transform.position.x, hit.point.z - terrain.transform.position.z);

            if (drawSplat)
            {
                splatResolution = terrain.terrainData.alphamapResolution;
                splatConversion = new Vector2((splatResolution - 1) / size.x, (splatResolution - 1) / size.z);
                splat = terrain.terrainData.GetAlphamaps(Mathf.RoundToInt(localPos.x * splatConversion.x), Mathf.RoundToInt(localPos.y * splatConversion.y), 1, 1);
            }
            if (drawGrass)
            {
                grassResolution = terrain.terrainData.detailResolution;
                grassConversion = new Vector2(grassResolution / size.x, grassResolution / size.z);

                int length = terrain.terrainData.detailPrototypes.Length;
                if (grassLayer == null) grassLayer = new GrassLayer[length];
                else if (grassLayer.Length != length) grassLayer = new GrassLayer[length];

                for (int i = 0; i < length; i++)
                {
                    if (grassLayer[i] == null) grassLayer[i] = new GrassLayer();
                    grassLayer[i].grass = terrain.terrainData.GetDetailLayer(Mathf.RoundToInt(localPos.x * grassConversion.x), Mathf.RoundToInt(localPos.y * grassConversion.y), 1, 1, i);
                }
            }
        }
        
        public class GrassLayer
        {
            public int[,] grass;
        }
    }
}
