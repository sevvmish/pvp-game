using UnityEngine;
using System.Collections;
using TerrainComposer2;


public class RemoveTree : MonoBehaviour {

    public bool removeTree;
    public int index = 0;

    public GameObject prefab;
	
    
	void Update () {
	    if (removeTree)
        {
            removeTree = false;
            RemoveTreeAtIndex(index);
        } 
	}

    void RemoveTreeAtIndex(int index)
    {
        TerrainData terrainData = TC_Area2D.instance.terrainAreas[0].terrains[0].terrain.terrainData;
        prefab = terrainData.treePrototypes[0].prefab;

        TreeInstance tree = terrainData.GetTreeInstance(index);

        float height = tree.heightScale;
        float width = tree.widthScale;

        Vector3 pos = tree.position;
        pos.Scale(terrainData.size);
        pos -= new Vector3(1024, 0, 1024);
        
        float rotation = tree.rotation * Mathf.Rad2Deg;
        
        tree.heightScale = 0;
        tree.widthScale = 0;

        terrainData.SetTreeInstance(index, tree);

        GameObject go = (GameObject)Instantiate(prefab, pos, Quaternion.Euler(0, rotation, 0));

        go.transform.localScale = new Vector3(width, height, width);


        this.index++;
    }
}
