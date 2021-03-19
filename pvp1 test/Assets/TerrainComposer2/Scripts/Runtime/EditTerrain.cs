using UnityEngine;
using System.Collections;
using TerrainComposer2;

public class EditTerrain
{

	static public float GetHeight(Vector3 worldPos)
	{
		Terrain t = GetTerrain(worldPos);

		if (t == null) return -1;

		Vector2 startPos = new Vector2(worldPos.x - t.transform.position.x, worldPos.z - t.transform.position.z);

		Vector3 size = t.terrainData.size;
		int resolution = t.terrainData.heightmapResolution;
		
		return t.terrainData.GetHeight(Mathf.RoundToInt((startPos.x / size.x) * resolution), Mathf.RoundToInt(startPos.y / size.x) * resolution);
	}

	static public void SetHeight(Vector3 worldPos, float height)
	{
		Terrain t = GetTerrain(worldPos);

		if (t == null) return;

		Vector2 startPos = new Vector2(worldPos.x - t.transform.position.x, worldPos.z - t.transform.position.z);

		Vector3 size = t.terrainData.size;
		int resolution = t.terrainData.heightmapResolution;

		float[,] heights = new float[1, 1];
		heights[0, 0] = height - t.transform.position.y;

		t.terrainData.SetHeights(Mathf.RoundToInt((startPos.x / size.x) * resolution), Mathf.RoundToInt(startPos.y / size.x) * resolution, heights);
	}

	static public Terrain GetTerrain(Vector3 worldPos)
	{
		TC_Area2D area2D = TC_Area2D.instance;

		for (int i = 0; i < area2D.terrainAreas[0].terrains.Count; i++)
		{
			TCUnityTerrain t = area2D.terrainAreas[0].terrains[i];

			if (t.terrain == null) continue;
			if (t.terrain.terrainData == null) continue;

			Rect rect = new Rect(t.terrain.transform.position.x, t.terrain.transform.position.z, t.terrain.terrainData.size.x, t.terrain.terrainData.size.z);

			if (rect.Contains(worldPos)) return t.terrain;
		}

		return null;
	}
}
