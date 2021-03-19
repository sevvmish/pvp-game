using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TerrainComposer2
{
    public class TC_TerrainLayer : TC_ItemBehaviour
    {
        // [NonSerialized] static public TC_TerrainLayer current;
        [NonSerialized] public TC_LayerGroup[] layerGroups = new TC_LayerGroup[6];
        // [NonSerialized]
        public List<TC_SelectItem> objectSelectItems;
        // [NonSerialized]
        public List<TC_SelectItem> treeSelectItems;
        
        public float treeResolutionPM = 128;
        public float objectResolutionPM = 128;
        public Vector2 objectAreaSize;
        public Transform objectTransform; // stream around camera

        public int colormapResolution = 128;
        public int meshResolution = 2048;

        public float seedChild;

        public TC_LayerGroup Clone()
        {
            GameObject obj = (GameObject)Instantiate(gameObject, t.position, t.rotation);
            obj.transform.parent = TC_Area2D.instance.transform;
            return obj.GetComponent<TC_LayerGroup>();
        }

        public void LinkClone(TC_TerrainLayer terrainLayerS)
        {
            TC_LayerGroup layerGroup;

            for (int i = 0; i < layerGroups.Length; i++)
            {
                layerGroup = layerGroups[i];

                if (layerGroup != null)
                {
                    layerGroup.LinkClone(terrainLayerS.layerGroups[i]);
                }
            }
        }

        public void New(bool undo)
        {
            //#if UNITY_EDITOR
            //if (undo) UnityEditor.Undo.RecordObject(gameObject, "Undo New TerrainComposer Project");
            //#endif
            bool autoGenerateOld = TC_Generate.instance.autoGenerate;
            TC_Generate.instance.autoGenerate = false;

            Reset();
            CreateLayerGroups();

            TC_Generate.instance.autoGenerate = autoGenerateOld;
        }

        public void CreateLayerGroups()
        {
            TC_ItemBehaviour item;

            for (int i = 0; i < 6; i++)
            {
                item = (TC_ItemBehaviour)Add<TC_LayerGroup>(TC.outputNames[i] + " Output", false, false, false, i);
                item.visible = false;
            }
        }


        // (arg0 : NodeTYpe) (arg1 : OutputId) (arg2 : Command)
        public int ExecuteCommand(string[] arg)
        {
            if (arg == null) return -1;
            if (arg.Length == 0) return -1;
            int returnValue = -1;

            if (arg[0] == "TerrainLayer" || arg[0] == "All")
            {

            }
            else
            {
                if (arg.Length <= 1) return -1;

                int outputId = TC.OutputNameToOutputID(arg[1]);
                if (outputId == -1)
                {
                    for (int i = 0; i < 6; i++) returnValue = layerGroups[i].ExecuteCommand(arg);
                }
                else returnValue = layerGroups[outputId].ExecuteCommand(arg);
            }

            return returnValue;
        }

        public void ResetPlaced()
        {
            layerGroups[TC.treeOutput].ResetPlaced();
            layerGroups[TC.objectOutput].ResetPlaced();
        }

        public void CalcTreePlaced()
        {
            layerGroups[TC.treeOutput].CalcPlaced();
        }

        public void CalcObjectPlaced()
        {
            layerGroups[TC.objectOutput].CalcPlaced();
        }

        public void ResetObjects()
        {
            layerGroups[TC.objectOutput].ResetObjects();
        }
        
        public void Reset()
        {
            TC.DestroyChildrenTransform(t);
        }

        public void GetItems(bool refresh)
        {
            GetItems(refresh, true, false);
        }
        
        public override void GetItems(bool refresh, bool rebuildGlobalLists, bool resetTextures)
        {
            // current = this;
            if (TC_Settings.instance == null) return;
            TC_Settings.instance.HasMasterTerrain();
            if (resetTextures)
            {
                TC_Compute.instance.DisposeTextures();
                TC_Area2D.instance.DisposeTextures();
                TC_Settings.instance.DisposeTextures();
            }
            
            for (int i = 0; i < layerGroups.Length; i++) GetItem(i, rebuildGlobalLists, resetTextures);
            TC.MoveToDustbinChildren(t, 6);
        }

        public void GetItem(int outputId, bool rebuildGlobalLists, bool resetTextures) 
        {
            // Debug.Log("Terrain Layer GetItem " + TC.outputNames[outputId]);
            active = visible;

            if (t.childCount < 6)
            {
                active = false;
                return;
            }

            if (outputId == TC.objectOutput)
            {
                if (objectSelectItems == null) objectSelectItems = new List<TC_SelectItem>();
                else objectSelectItems.Clear();
            }
            else if (outputId == TC.treeOutput)
            {
                if (treeSelectItems == null) treeSelectItems = new List<TC_SelectItem>();
                else treeSelectItems.Clear();
            }
            
            Transform child = t.GetChild(outputId); 
            TC_LayerGroup layerGroup = child.GetComponent<TC_LayerGroup>();
            if (layerGroup != null)
            {
                layerGroup.level = 0;
                layerGroup.outputId = outputId;
                layerGroup.listIndex = outputId;
                layerGroup.parentItem = this;
                
                layerGroup.GetItems(true, rebuildGlobalLists, resetTextures);
                layerGroups[outputId] = layerGroup;
            }
        }
    }
}