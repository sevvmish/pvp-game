using UnityEngine;
using System;


public class TC_TerrainNeighbors: MonoBehaviour {
    
    public Terrain left;
    public Terrain top;
    public Terrain right;
    public Terrain bottom;
    
    public void Start() 
    {
#if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2
    	Terrain terrain = null;
    	terrain = GetComponent<Terrain>();
    	terrain.SetNeighbors(left,top,right,bottom);
#endif
    }
}