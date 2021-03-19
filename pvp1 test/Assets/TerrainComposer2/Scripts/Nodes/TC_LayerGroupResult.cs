using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace TerrainComposer2
{
    public class TC_LayerGroupResult : TC_GroupBehaviour
    {
        // [NonSerialized]
        public List<TC_ItemBehaviour> itemList = new List<TC_ItemBehaviour>();

        public float seed = 0;
        
        // Compute height, trees and objects
        public ComputeBuffer ComputeSingle(float seedParent, bool first = false)
        {
            TC_Compute compute = TC_Compute.instance;

            ComputeBuffer totalBuffer = null;
            ComputeBuffer layerBuffer = null;
            ComputeBuffer layerMaskBuffer = null;

            RenderTexture[] rtsPreview = null;
            RenderTexture rtRightPreview = null;
            RenderTexture rtLeftPreview = null;

            if (outputId != TC.heightOutput) rtsPreview = new RenderTexture[2];

            SetPreviewTextureBefore();

            int even = 0;

            float seedTotal = seed + seedParent;
            
            for (int i = 0; i < itemList.Count; i++)
            {
                TC_Layer layer = itemList[i] as TC_Layer;
                
                if (layer != null)
                {
                    if (!layer.active) { TC_Reporter.Log("Inactive layer " + i); continue; }
                    
                    if (totalBuffer == null)
                    {
                        if (outputId == TC.heightOutput) layer.ComputeHeight(ref totalBuffer, ref layerMaskBuffer, seedTotal, i == firstActive);
                        else
                        {
                            layer.ComputeItem(ref totalBuffer, ref layerMaskBuffer, seedTotal, i == firstActive);
                            if (totalBuffer != null) rtLeftPreview = layer.rtDisplay;
                        }
                        
                        compute.DisposeBuffer(ref layerMaskBuffer);
                    }
                    else
                    {
                        if (outputId == TC.heightOutput) layer.ComputeHeight(ref layerBuffer, ref layerMaskBuffer, seedTotal);
                        else layer.ComputeItem(ref layerBuffer, ref layerMaskBuffer, seedTotal);

                        if (layerBuffer != null)
                        {
                            if (outputId == TC.heightOutput) compute.RunComputeMethod(this, layer, totalBuffer, ref layerBuffer, totalActive, i == lastActive ? rtPreview : null, layerMaskBuffer);
                            else
                            {
                                rtRightPreview = layer.rtDisplay;
                                compute.RunComputeObjectMethod(this, layer, totalBuffer, ref layerBuffer, layerMaskBuffer, rtPreview, ref rtsPreview[even++ % 2], ref rtLeftPreview, rtRightPreview);
                            }
                        }
                        compute.DisposeBuffer(ref layerMaskBuffer);
                    }
                }
                else
                {
                    TC_LayerGroup layerGroup = itemList[i] as TC_LayerGroup;
                    if (layerGroup == null) continue;
                    if (!layerGroup.active) continue;

                    if (totalBuffer == null)
                    {
                        layerMaskBuffer = layerGroup.ComputeSingle(ref totalBuffer, seedTotal, i == firstActive);
                        if (totalBuffer != null) rtLeftPreview = layerGroup.rtDisplay;

                        compute.DisposeBuffer(ref layerMaskBuffer);
                    }
                    else
                    {
                        layerMaskBuffer = layerGroup.ComputeSingle(ref layerBuffer, seedTotal);

                        if (layerBuffer != null)
                        {
                            if (outputId == TC.heightOutput) compute.RunComputeMethod(this, layerGroup, totalBuffer, ref layerBuffer, totalActive, i == lastActive ? rtPreview : null, layerMaskBuffer);
                            else
                            {
                                rtRightPreview = layerGroup.rtDisplay;
                                compute.RunComputeObjectMethod(this, layerGroup, totalBuffer, ref layerBuffer, layerMaskBuffer, rtPreview, ref rtsPreview[even++ % 2], ref rtLeftPreview, rtRightPreview);
                            }
                        }
                        compute.DisposeBuffer(ref layerMaskBuffer);
                    }
                }
            }

            SetPreviewTextureAfter();
            
            if (outputId != TC.heightOutput) TC_Compute.DisposeRenderTextures(ref rtsPreview);
            compute.DisposeBuffer(ref layerMaskBuffer);
            
            if (totalBuffer == null) TC_Reporter.Log("Layer buffer null");
            return totalBuffer;
        }

        public bool ComputeMulti(ref RenderTexture[] renderTextures, float seedParent, bool doNormalize, bool first = false)
        {
            TC_Compute compute = TC_Compute.instance;
            RenderTexture[] rtsLayer = null;
            RenderTexture rtRightPreview = null;
            RenderTexture rtLeftPreview = null;

            RenderTexture[] rtsPreview = null;
            // RenderTexture rtPreview2 = null;
            ComputeBuffer layerMaskBuffer = null;
            TC_LayerGroup layerGroup;
            TC_Layer layer;
            bool firstCompute = false;
            bool lastCompute = false;

            int even = 0;
            
            rtsPreview = new RenderTexture[2];
            
            SetPreviewTextureBefore();

            float seedTotal = seed + seedParent;

            for (int i = 0; i < itemList.Count; i++)
            {
                layer = itemList[i] as TC_Layer;

                if (layer != null)
                {
                    if (!layer.active) continue;
                    // InitPreview(ref layer.previewRenderTex);

                    if (!firstCompute)
                    {
                        firstCompute = layer.ComputeMulti(ref renderTextures, ref layerMaskBuffer, seedTotal, i == firstActive);
                        
                        if (firstCompute)
                        {
                            rtLeftPreview = layer.rtDisplay;
                            TC_Reporter.Log("firt compute " + layer.maskNodeGroup.totalActive);
                            compute.DisposeBuffer(ref layerMaskBuffer);
                        }
                    }
                    else
                    {
                        TC_Compute.InitRenderTextures(ref rtsLayer, "rtsLayer", renderTextures.Length);
                        lastCompute = layer.ComputeMulti(ref rtsLayer, ref layerMaskBuffer, seedTotal);

                        if (lastCompute)
                        {
                            TC_Reporter.Log("Run layer method multi");
                            rtRightPreview = (layer.method == Method.Lerp) ? layer.selectNodeGroup.rtColorPreview : layer.rtDisplay;
                            // Debug.Log(rtRight.name+ " "+ (layer.maskNodeGroup.activeTotal == 0 || layer.method == Method.Lerp));
                            
                            if (outputId == TC.colorOutput) compute.RunComputeColorMethod(layer, layer.method, ref renderTextures[0], ref rtsLayer[0], layerMaskBuffer, rtPreview, ref rtsPreview[even++ % 2], ref rtLeftPreview, rtRightPreview);
                            else compute.RunComputeMultiMethod(layer, layer.method, i == lastActive && doNormalize, ref renderTextures, ref rtsLayer, layerMaskBuffer, rtPreview, ref rtsPreview[even++ % 2], ref rtLeftPreview, rtRightPreview);
                            
                            compute.DisposeBuffer(ref layerMaskBuffer);
                        }
                    }
                }
                else 
                {
                    layerGroup = itemList[i] as TC_LayerGroup;
                    if (layerGroup == null) continue;
                    if (!layerGroup.active) continue;

                    if (!firstCompute)
                    {
                        firstCompute = layerGroup.ComputeMulti(ref renderTextures, ref layerMaskBuffer, seedTotal, i == firstActive);
                        if (firstCompute)
                        {
                            rtLeftPreview = layerGroup.rtDisplay;
                            compute.DisposeBuffer(ref layerMaskBuffer);
                            TC_Reporter.Log("LayerGroup did first compute");
                        }
                    }
                    else
                    {
                        TC_Compute.InitRenderTextures(ref rtsLayer, "rtsLayer", renderTextures.Length);
                        lastCompute = layerGroup.ComputeMulti(ref rtsLayer, ref layerMaskBuffer, seedTotal);
                        if (lastCompute)
                        {
                            // if (layerGroup.groupResult.activeTotal == 1) rtRight = layerGroup.rtDisplay; else rtRight = layerGroup.rtPreview;
                            rtRightPreview = (layerGroup.method == Method.Lerp) ? layerGroup.groupResult.rtDisplay : layerGroup.rtDisplay;

                            if (outputId == TC.colorOutput) compute.RunComputeColorMethod(layerGroup, layerGroup.method, ref renderTextures[0], ref rtsLayer[0], layerMaskBuffer, rtPreview, ref rtsPreview[even++ % 2], ref rtLeftPreview, rtRightPreview);
                            else compute.RunComputeMultiMethod(layerGroup, layerGroup.method, i == lastActive && doNormalize, ref renderTextures, ref rtsLayer, layerMaskBuffer, rtPreview, ref rtsPreview[even++ % 2], ref rtLeftPreview, rtRightPreview);
                            
                            compute.DisposeBuffer(ref layerMaskBuffer);
                        }
                    }
                }
            }

            SetPreviewTextureAfter();
            
            if (layerMaskBuffer != null) { compute.DisposeBuffer(ref layerMaskBuffer); TC_Reporter.Log("Dispose layerMaskBuffer"); }
            
            TC_Compute.DisposeRenderTextures(ref rtsPreview);
            TC_Compute.DisposeRenderTextures(ref rtsLayer);

            return firstCompute;
        }
        
        public void SetPreviewTextureBefore()
        {

            // Debug.Log("no " + maskNodeGroup.itemList.Count + " " + itemList.Count);
            if (totalActive == 0)
            {
                active = false;
                rtDisplay = null;

                TC_Compute.DisposeRenderTexture(ref rtPreview);
            }
            else if (totalActive != 1)
            { 
                TC_Compute.InitPreviewRenderTexture(ref rtPreview, "rtGroupResult");
                rtDisplay = rtPreview;
            }
        }

        public void SetPreviewTextureAfter()
        {
            if (totalActive == 1)
            {
                TC_Compute.DisposeRenderTexture(ref rtPreview);

                rtDisplay = itemList[firstActive].rtDisplay;
            }
        }

        public void LinkClone(TC_LayerGroupResult resultLayerGroupS)
        {
            preview = resultLayerGroupS.preview;
         
            for (int i = 0; i < itemList.Count; i++)
            {
                TC_Layer layer = itemList[i] as TC_Layer;
                if (layer != null)
                {
                    TC_Layer layerS = resultLayerGroupS.itemList[i] as TC_Layer;
                    layer.LinkClone(layerS);
                }
                else
                {
                    TC_LayerGroup layerGroup = itemList[i] as TC_LayerGroup;
                    if (layerGroup != null)
                    {
                        TC_LayerGroup layerGroupS = resultLayerGroupS.itemList[i] as TC_LayerGroup;
                        layerGroup.LinkClone(layerGroupS);
                    }
                }
            }
        }

        public void ResetPlaced()
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                TC_Layer layer = itemList[i] as TC_Layer;
                if (layer != null) layer.ResetPlaced();
                else
                {
                    TC_LayerGroup layerGroup = itemList[i] as TC_LayerGroup;
                    if (layerGroup != null) layerGroup.ResetPlaced();
                }
            }
        }

        public int CalcPlaced()
        {
            int placed = 0;

            for (int i = 0; i < itemList.Count; i++)
            {
                TC_Layer layer = itemList[i] as TC_Layer;
                if (layer != null) placed += layer.CalcPlaced();
                else
                {
                    TC_LayerGroup layerGroup = itemList[i] as TC_LayerGroup;
                    if (layerGroup != null) placed += layerGroup.CalcPlaced();
                }
            }

            return placed;
        }

        public override void SetLockChildrenPosition(bool lockPos)
        {
            lockPosParent = lockPos;
            for (int i = 0; i < itemList.Count; i++) itemList[i].SetLockChildrenPosition(lockPosParent || lockPosChildren);
        }

        public override void UpdateTransforms()
        {
            // ct.CopySpecial(this);

            for (int i = 0; i < itemList.Count; i++) itemList[i].UpdateTransforms();
        }
        
        public override void ChangeYPosition(float y)
        {
            for (int i = 0; i < itemList.Count; i++) itemList[i].ChangeYPosition(y);
        }

        public override void SetFirstLoad(bool active)
        {
            base.SetFirstLoad(active);

            for (int i = 0; i < itemList.Count; i++) itemList[i].SetFirstLoad(active);
        }

        public override bool ContainsCollisionNode()
        {
            bool returnValue = false;

            for (int i = 0; i < itemList.Count; i++)
            {
                returnValue = itemList[i].ContainsCollisionNode();
                
                if (returnValue) return true;
            }

            return false;
        }

        public void ResetObjects()
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                TC_Layer layer = itemList[i] as TC_Layer;
                if (layer != null) layer.ResetObjects(); 
                else
                {
                    TC_LayerGroup layerGroup = itemList[i] as TC_LayerGroup;
                    if (layerGroup != null) layerGroup.ResetObjects();
                }
            }
        }

        public override void GetItems(bool refresh, bool rebuildGlobalLists, bool resetTextures)
        {
            if (resetTextures) DisposeTextures();

            active = visible;

            itemList.Clear();

            firstActive = lastActive = -1;
            totalActive = 0; 

            bool newBounds = true;
            int listIndex = 0;

            // Debug.Log(name + " GetItems");

            for (int i = t.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                TC_Layer layer = child.GetComponent<TC_Layer>();

                if (layer != null)
                {
                    layer.SetParameters(this, listIndex);

                    layer.GetItems(refresh, rebuildGlobalLists, resetTextures);
                    if (layer.active)
                    {
                        ++totalActive;
                        lastActive = listIndex;
                        if (firstActive == -1) firstActive = lastActive;
                    }
                    itemList.Add(layer);
                    ++listIndex;

                    if (newBounds) { bounds = layer.bounds; newBounds = false; }
                    else bounds.Encapsulate(layer.bounds);
                }
                else
                {
                    TC_LayerGroup layerGroup = child.GetComponent<TC_LayerGroup>();

                    if (layerGroup == null) TC.MoveToDustbin(child);
                    else
                    {
                        layerGroup.SetParameters(this, listIndex);
                        layerGroup.GetItems(refresh, rebuildGlobalLists, resetTextures);

                        if (layerGroup.active)
                        {
                            ++totalActive;
                            lastActive = listIndex;
                            if (firstActive == -1) firstActive = lastActive;
                        }

                        if (layerGroup.groupResult == null) TC.MoveToDustbin(child);
                        else
                        {
                            itemList.Add(layerGroup);
                            listIndex++;
                        }
                        if (newBounds) { bounds = layerGroup.bounds; newBounds = false; }
                        else bounds.Encapsulate(layerGroup.bounds);
                    }
                }
            }

            TC_Reporter.Log(TC.outputNames[outputId] + " Level " + level + " activeTotal " + totalActive);

            if (!active) totalActive = 0;
            else if (totalActive == 0) active = false; 
        }

        public int ExecuteCommand(string[] arg)
        {
            if (arg == null) return -1;
            if (arg.Length == 0) return -1;
            int returnValue = -1;

            if (arg[0] == "ResultGroup" || arg[0] == "All")
            {

            }
            
            if (arg[0] != "ResultGroup")
            {
                if (arg.Length <= 1) return -1;

                for (int i = 0; i < itemList.Count; i++)
                {
                    // if (itemList[i].layer != null) returnValue = Mathf.Max(returnValue, itemList[i].layer.ExecuteCommand(arg));
                    // else if (itemList[i].layerGroup != null) returnValue = Mathf.Max(returnValue, itemList[i].layerGroup.ExecuteCommand(arg));
                }
            }

            return returnValue;
        }
    }
}