using UnityEngine;
using System.Collections;
using TerrainComposer2;

public class LoadTerrainLayer : MonoBehaviour {

    public GameObject terrainLayerPrefab;
    public bool generateOnStart;
    public bool instantGenerate;

	void Start ()
    {
        InstantiateTerrainLayer();
        if (generateOnStart) TC_Generate.instance.Generate(instantGenerate);
	}

    public void InstantiateTerrainLayer()
    {
        if (terrainLayerPrefab == null) return;
        if (terrainLayerPrefab.GetComponent<TC_TerrainLayer>() == null) return;
        
        TC_Area2D area2D = TC_Area2D.instance;
        if (area2D == null) return;

        // Destroy the Terrain Layer that is currently assigned
        if (area2D.terrainLayer != null) Destroy(area2D.terrainLayer.gameObject);

        // Instantiate the Terrain Layer Prefab
        GameObject terrainLayerGO = Instantiate(terrainLayerPrefab);
        area2D.terrainLayer = terrainLayerGO.GetComponent<TC_TerrainLayer>();

        // Assign all nodes in Terrain Layer
        area2D.terrainLayer.GetItems(false);
    }
}
