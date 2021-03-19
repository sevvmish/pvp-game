using UnityEngine;
using System.Collections;
using TerrainComposer2;

[ExecuteInEditMode]
public class CreateTerrain : MonoBehaviour {

	public bool createTerrain;

	void Update()
	{
		if (createTerrain)
		{
			createTerrain = false;
			CreateTerrains();
		}
	}
	
	void CreateTerrains()
	{
		TC_Area2D area2D = TC_Area2D.instance;
		if (area2D == null)
		{
			return;
		}
        
		area2D.terrainAreas[0].CreateTerrains();
	}
}
