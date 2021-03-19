using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TerrainComposer2
{
    public class TC_Layer : TC_ItemBehaviour
    {
        [NonSerialized] public TC_SelectItemGroup selectItemGroup;
        [NonSerialized] public TC_NodeGroup maskNodeGroup;
        [NonSerialized] public TC_NodeGroup selectNodeGroup;
        // public new TC_LayerGroupResult parentItem;

        public List<TC_SelectItem.DistanceRule> distanceRules;
        
        public bool doNormalize;
        public float placeLimit = 0.5f;
        public float selectValue;
        public float maskValue;

        public float seed = 0;
        public int placed;

        float splatTotal;
        float x, y;

        // Compute Heightm
        public void ComputeHeight(ref ComputeBuffer layerBuffer, ref ComputeBuffer maskBuffer, float seedParent, bool first = false)
        {
            TC_Compute compute = TC_Compute.instance;

            float seedTotal = seed + seedParent;

            layerBuffer = selectNodeGroup.ComputeValue(seedTotal);

            if (layerBuffer != null)
            {
                if (maskNodeGroup.active) maskBuffer = maskNodeGroup.ComputeValue(seedTotal);

                if (maskBuffer != null)
                {
                    if (method != Method.Lerp || first)
                    {
                        InitPreviewRenderTexture(true, "rtPreview_Layer_" + TC.outputNames[outputId]);
                        compute.RunComputeMethod(null, null, layerBuffer, ref maskBuffer, 0, rtPreview);
                    }
                }
                else rtDisplay = selectNodeGroup.rtDisplay;

                if (isPortalCount > 0) TC_Compute.instance.MakePortalBuffer(this, layerBuffer, method == Method.Lerp ? maskBuffer : null);
            }
            else TC_Reporter.Log("Layerbuffer " + listIndex + " = null, reporting from layer");
        }

        // Compute color, splat and grass
        public bool ComputeMulti(ref RenderTexture[] renderTextures, ref ComputeBuffer maskBuffer, float seedParent, bool first = false)
        {
            TC_Compute compute = TC_Compute.instance;
            bool didCompute = false;

            float seedTotal = seed + seedParent;

            ComputeBuffer layerBuffer = selectNodeGroup.ComputeValue(seedTotal);

            if (layerBuffer != null)
            {
                didCompute = true;

                TC_Compute.InitPreviewRenderTexture(ref rtPreview, "rtPreview_Layer");
                
                if (maskNodeGroup.active) maskBuffer = maskNodeGroup.ComputeValue(seedTotal);

                TC_Compute.InitPreviewRenderTexture(ref selectNodeGroup.rtColorPreview, "rtNodeGroupPreview_" + TC.outputNames[outputId]);

                if (outputId == TC.colorOutput)
                {
                    if (selectItemGroup.itemList.Count == 1 && selectItemGroup.itemList[0].texColor != null) compute.RunColorTexCompute(selectNodeGroup, selectItemGroup.itemList[0], ref renderTextures[0], ref layerBuffer);
                    else compute.RunColorCompute(selectNodeGroup, selectItemGroup, ref renderTextures[0], ref layerBuffer);
                }
                else compute.RunSplatCompute(selectNodeGroup, selectItemGroup, ref renderTextures, ref layerBuffer); 

                compute.DisposeBuffer(ref layerBuffer);

                if (maskBuffer != null)
                {
                    if (method != Method.Lerp || first)
                    {
                        TC_Reporter.Log("Run layer select * mask");
                        if (outputId == TC.colorOutput) compute.RunComputeColorMethod(this, ref renderTextures[0], maskBuffer, rtPreview);
                        else compute.RunComputeMultiMethod(this, doNormalize, ref renderTextures, maskBuffer, rtPreview);
                    }
                    rtDisplay = rtPreview;
                }
                else
                {
                    TC_Reporter.Log("No mask buffer assign colorPreviewTex to layer");
                    rtDisplay = selectNodeGroup.rtColorPreview;
                }
            }

            return didCompute;
        }

        // Compute trees and objects
        public bool ComputeItem(ref ComputeBuffer itemMapBuffer, ref ComputeBuffer maskBuffer, float seedParent, bool first = false)
        {
            TC_Compute compute = TC_Compute.instance;
            bool didCompute = false;

            float seedTotal = seed + seedParent;

            ComputeBuffer selectBuffer = selectNodeGroup.ComputeValue(seedTotal);

            if (selectBuffer != null)
            {
                didCompute = true;

                TC_Compute.InitPreviewRenderTexture(ref rtPreview, "rtPreview_Layer_" + TC.outputNames[outputId]);
                rtDisplay = rtPreview;
                TC_Compute.InitPreviewRenderTexture(ref selectNodeGroup.rtColorPreview, "rtColorPreview");
                compute.RunItemCompute(this, ref itemMapBuffer, ref selectBuffer);
                compute.DisposeBuffer(ref selectBuffer);

                // compute.shader.SetBuffer(compute.terrainSplatmap0Kernel, "itemMapBuffer", itemMapBuffer);
                // compute.RunItemPositionCompute(itemMapBuffer, TC.treeOutput);

                if (maskNodeGroup.active) maskBuffer = maskNodeGroup.ComputeValue(seedTotal);

                if (maskBuffer != null)
                {
                    TC_Reporter.Log("Run layer select * mask");
                    if (method != Method.Lerp || first)
                    {
                        compute.RunItemComputeMask(this, ref rtPreview, selectNodeGroup.rtColorPreview, ref itemMapBuffer, ref maskBuffer);
                    }
                }
            }

            return didCompute;
        }

        public void LinkClone(TC_Layer layerS)
        {
            preview = layerS.preview;
            maskNodeGroup.LinkClone(layerS.maskNodeGroup);
            selectNodeGroup.LinkClone(layerS.selectNodeGroup);
        }

        public void ResetPlaced()
        {
            selectItemGroup.ResetPlaced();
        }

        public int CalcPlaced()
        {
            placed = selectItemGroup.CalcPlaced();
            return placed; 
        }

        public void ResetObjects()
        {
            selectItemGroup.ResetObjects();
        }

        public override void GetItems(bool refresh, bool rebuildGlobalLists, bool resetTextures)
        {
            if (resetTextures) DisposeTextures();

            active = visible;
            // Init();
            // InitPreview(ref rtPreview);
            bool newBounds = true;

            maskNodeGroup = GetGroup<TC_NodeGroup>(0, refresh, resetTextures);
            if (maskNodeGroup != null) 
            {
                maskNodeGroup.type = NodeGroupType.Mask;
                if (maskNodeGroup.totalActive > 0)
                {
                    bounds = maskNodeGroup.bounds;
                    newBounds = false;
                }
            }
            
            selectNodeGroup = GetGroup<TC_NodeGroup>(1, refresh, resetTextures);
            if (selectNodeGroup != null)
            {
                selectNodeGroup.type = NodeGroupType.Select;
                if (selectNodeGroup.totalActive == 0) { TC_Reporter.Log("SelectNodeGroup 0 active"); active = false; }
                else
                {
                    if (newBounds) bounds = selectNodeGroup.bounds;
                    else bounds.Encapsulate(selectNodeGroup.bounds);
                }
            }
            else active = false;
            
            if (outputId != TC.heightOutput)
            {
                selectItemGroup = GetGroup<TC_SelectItemGroup>(2, refresh, resetTextures);
                if (selectItemGroup != null)
                {
                    if (selectItemGroup.totalActive == 0) { TC_Reporter.Log("itemGroup 0 active"); active = false; }
                    else if (selectItemGroup.itemList.Count <= 1)
                    {
                        // TODO: Make better solution for this
                        selectNodeGroup.useConstant = true;
                        if (selectNodeGroup.itemList.Count > 0)
                        {
                            selectNodeGroup.itemList[0].visible = true;
                            active = visible;
                            GetGroup<TC_NodeGroup>(1, true, resetTextures);
                        }
                    }
                    else selectNodeGroup.useConstant = false;
                }
                else active = false;
            }
        }

        public override void SetLockChildrenPosition(bool lockPos)
        {
            // Debug.Log("lockPos " + lockPos);
            lockPosParent = lockPos;
            // Debug.Log("lockPosParent " + lockPosParent);
            if (maskNodeGroup != null) maskNodeGroup.SetLockChildrenPosition(lockPosParent || lockPosChildren);
            if (selectNodeGroup != null) selectNodeGroup.SetLockChildrenPosition(lockPosParent || lockPosChildren);
        }

        public override void UpdateTransforms()
        {
            // ct.CopySpecial(this);

            maskNodeGroup.UpdateTransforms();
            selectNodeGroup.UpdateTransforms();
        }

        public override void ChangeYPosition(float y) { selectNodeGroup.ChangeYPosition(y); }

        public override void SetFirstLoad(bool active)
        {
            base.SetFirstLoad(active);
            maskNodeGroup.SetFirstLoad(active);
            selectNodeGroup.SetFirstLoad(active);
            selectItemGroup.SetFirstLoad(active);
        }

        public override bool ContainsCollisionNode()
        {
            if (selectNodeGroup.ContainsCollisionNode()) return true;
            if (maskNodeGroup.ContainsCollisionNode()) return true;

            return false;
        }
    }
}