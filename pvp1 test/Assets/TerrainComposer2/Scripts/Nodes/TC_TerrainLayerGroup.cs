using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace TerrainComposer2
{
    public class TC_TerrainLayerGroup : TC_GroupBehaviour
    {
        List<TerrainLayerGroupItem> itemList = new List<TerrainLayerGroupItem>();

        //public void ComputeValue(ref ComputeBuffer totalBuffer)
        //{
        //    TCCompute compute = TCCompute.singleton;
        //    LayerGroupItem item;
        //    ComputeBuffer layerBuffer = null;
        //    ComputeBuffer layerMaskBuffer = null;
        //    // ComputeBuffer layerGroupBuffer = null;
        //    TCLayerGroup layerLevel;

        //    for (int i = 0; i < itemList.Count; i++)
        //    {
        //        item = itemList[i];
        //        layerLevel = item.layerGroup;

        //        if (i == 0)
        //        {
        //            layerMaskBuffer = layerLevel.ComputeValue(ref totalBuffer, TC.heightOutput);
        //        }
        //        else
        //        {
        //            layerMaskBuffer = layerLevel.ComputeValue(ref layerBuffer, TC.heightOutput);
        //            if (layerBuffer != null) compute.RunComputeMethod(this, layerLevel, totalBuffer, ref layerBuffer, itemList.Count, i < itemList.Count - 1 ? null : m_previewRenderTex, true, layerMaskBuffer);
        //        }
        //    }

        //    if (layerMaskBuffer != null) { compute.DisposeBuffer(ref layerMaskBuffer); }


        //    if (totalBuffer == null) Debug.Log("Layer buffer null");
        //}

        //public override void GetItems()
        //{
        //    Transform child;
        //    TCLayerGroup layerLevel;
        //    int childCount = t.childCount;

        //    Init();
        //    itemList.Clear();

        //    // if (preview.tex == null || preview.bytes == null) preview.Create();

        //    for (int i = 0; i < childCount; i++)
        //    {
        //        child = t.GetChild(i);
        //        layerLevel = child.GetComponent<TCLayerLevel>();
        //        if (layerLevel != null)
        //        {
        //            layerLevel.outputId = i;
        //            layerLevel.active = layerLevel.go.activeSelf;
        //            itemList.Add(new LayerGroupMultiItem(layerLevel));
        //            layerLevel.GetItems();
        //        }
        //    }
        //}

        public override void GetItems(bool refresh, bool rebuildGlobalLists, bool resetTextures)
        {
            Transform child;
            TC_TerrainLayerGroup terrainLayerGroup;
            TC_TerrainLayer terrainLayer;
            int childCount = transform.childCount;

            // Init();
            itemList.Clear();

            lastActive = -1;
            totalActive = 0;

            // if (preview.tex == null || preview.bytes == null) preview.Create();
            bool newBounds = true;
            active = visible;

            int listIndex = 0;

            for (int i = childCount - 1; i >= 0; i--)
            {
                child = transform.GetChild(i);
                terrainLayer = child.GetComponent<TC_TerrainLayer>();
                if (terrainLayer != null)
                {
                    terrainLayer.SetParameters(this, listIndex++);
                    terrainLayer.terrainLevel = terrainLevel + 1;

                        terrainLayer.GetItems(refresh, rebuildGlobalLists, resetTextures);
                        terrainLayer.GetItem(outputId, rebuildGlobalLists, resetTextures);
                    //{
                        ++totalActive;
                        itemList.Add(new TerrainLayerGroupItem(null, terrainLayer));
                        lastActive = itemList.Count - 1;
                        if (newBounds) { bounds = terrainLayer.bounds; newBounds = false; }
                        else bounds.Encapsulate(terrainLayer.bounds);
                    //}
                    // else layer.displayRenderTex = null;
                }
                else
                {
                    terrainLayerGroup = child.GetComponent<TC_TerrainLayerGroup>();
                    if (terrainLayerGroup != null)
                    {
                        terrainLayerGroup.SetParameters(this, listIndex++);
                        terrainLayerGroup.terrainLevel = terrainLevel + 1;

                        terrainLayerGroup.GetItems(refresh, rebuildGlobalLists, resetTextures);
                        
                            ++totalActive;
                            itemList.Add(new TerrainLayerGroupItem(terrainLayerGroup, null));
                            lastActive = itemList.Count - 1;
                            if (newBounds) { bounds = terrainLayerGroup.bounds; newBounds = false; }
                            else bounds.Encapsulate(terrainLayerGroup.bounds);
                        
                    }
                }
            }

            TC_Reporter.Log(TC.outputNames[outputId] + " Level " + level + " activeTotal " + totalActive);

            if (!active) totalActive = 0;
            else if (totalActive == 0) active = false; else active = visible;
        }
    }

    [Serializable]
    public class TerrainLayerGroupItem
    {
        public TC_TerrainLayerGroup terrainLayerGroup;
        public TC_TerrainLayer terrainLayer;

        public TerrainLayerGroupItem(TC_TerrainLayerGroup terrainLayerGroup, TC_TerrainLayer terrainLayer)
        {
            this.terrainLayer = terrainLayer;
            this.terrainLayerGroup = terrainLayerGroup;
        }
    }

}