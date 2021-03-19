using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TC_LevelWithTerrain : MonoBehaviour {

    public bool levelChildren;

    void Update()
    {
        if (levelChildren)
        {
            levelChildren = false;
            LevelChildren();
        }
    }


	void LevelChildren()
    {
        Transform child;
        RaycastHit hit;
        Ray ray = new Ray();
        ray.direction = new Vector3(0, -1, 0);
        int layer = LayerMask.NameToLayer("Terrain");
        layer = ~layer;

        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            child = transform.GetChild(i);

            ray.origin = child.position;

            if (Physics.Raycast(ray, out hit))
            {
                child.position = new Vector3(child.position.x, hit.point.y, child.position.z);
            }
        }
	}
	
	
}
