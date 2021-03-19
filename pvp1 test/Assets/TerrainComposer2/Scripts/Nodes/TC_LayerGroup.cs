using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TerrainComposer2
{
    public class TC_LayerGroup : TC_ItemBehaviour
    {
        [NonSerialized] public TC_NodeGroup maskNodeGroup;
        [NonSerialized] public TC_LayerGroupResult groupResult;
        
        public bool doNormalize;
        public float placeLimit = 0.5f;
        public Vector2 nodePos;

        public float seed = 0;
        public int placed;

        // Compute height, trees and objects
        public ComputeBuffer ComputeSingle(ref ComputeBuffer totalBuffer, float seedParent, bool first = false)
        {
            if (!groupResult.active) return null;

            TC_Compute compute = TC_Compute.instance;

            float seedTotal = seed + seedParent;
            
            totalBuffer = groupResult.ComputeSingle(seedTotal, first);

            // Debug.Log("layerMaskBuffer " + layerMaskBuffer == null);
            ComputeBuffer maskBuffer = null;
            if (maskNodeGroup.active) maskBuffer = maskNodeGroup.ComputeValue(seedTotal);

            if (maskBuffer != null)
            {
                TC_Compute.InitPreviewRenderTexture(ref rtPreview, "rtPreview_LayerGroup");

                if (method != Method.Lerp || first)
                {
                    if (outputId == TC.heightOutput) compute.RunComputeMethod(null, null, totalBuffer, ref maskBuffer, 0, rtPreview);
                    else compute.RunItemComputeMask(this, ref rtPreview, groupResult.rtDisplay, ref totalBuffer, ref maskBuffer);
                }

                rtDisplay = rtPreview;
            }
            else
            {
                if (outputId == TC.heightOutput || level == 0 || groupResult.totalActive == 1) rtDisplay = groupResult.rtDisplay;
                else rtDisplay = rtPreview;
            }

            if (totalBuffer == null) TC_Reporter.Log("Layer buffer null");
            else
            {
                if (isPortalCount > 0 && outputId == TC.heightOutput) TC_Compute.instance.MakePortalBuffer(this, totalBuffer, method == Method.Lerp ? maskBuffer : null);
            }

            return maskBuffer;
        }

        // Compute Color, splat and grass
        public bool ComputeMulti(ref RenderTexture[] renderTextures, ref ComputeBuffer maskBuffer, float seedParent, bool first = false)
        {
            TC_Compute compute = TC_Compute.instance;

            float totalSeed = seed + seedParent;

            bool computed = groupResult.ComputeMulti(ref renderTextures, totalSeed, doNormalize, first);

            if (maskNodeGroup.active) maskBuffer = maskNodeGroup.ComputeValue(totalSeed);

            if (maskBuffer != null)
            {
                TC_Compute.InitPreviewRenderTexture(ref rtPreview, "rtPreview_LayerGroup_" + TC.outputNames[outputId]);
                if (method != Method.Lerp || first)
                {
                    if (outputId == TC.colorOutput) compute.RunComputeColorMethod(this, ref renderTextures[0], maskBuffer, groupResult.rtDisplay);
                    else compute.RunComputeMultiMethod(this, doNormalize, ref renderTextures, maskBuffer, groupResult.rtDisplay);
                }
                rtDisplay = rtPreview;
            }
            else rtDisplay = groupResult.rtDisplay;
            
            return computed;
        }

        public void ResetPlaced()
        {
            groupResult.ResetPlaced();
        }

        public int CalcPlaced()
        {
            placed = groupResult.CalcPlaced();
            return placed;
        }
        
        public void LinkClone(TC_LayerGroup layerGroupS)
        {
            preview = layerGroupS.preview;
            maskNodeGroup.LinkClone(layerGroupS.maskNodeGroup);
            groupResult.LinkClone(layerGroupS.groupResult);
        }

        public override void SetLockChildrenPosition(bool lockPos)
        {
            lockPosParent = lockPos;
            if (groupResult != null) groupResult.SetLockChildrenPosition(lockPosParent || lockPosChildren);
            if (maskNodeGroup != null) maskNodeGroup.SetLockChildrenPosition(lockPosParent || lockPosChildren);
        }

        public override void UpdateTransforms()
        {
            ct.Copy(this);

            groupResult.UpdateTransforms();
        }

        public override void SetFirstLoad(bool active)
        {
            base.SetFirstLoad(active);
            maskNodeGroup.SetFirstLoad(active);
            groupResult.SetFirstLoad(active);
        }

        public void ResetObjects()
        {
            groupResult.ResetObjects();
        }

        public override void GetItems(bool refresh, bool rebuildGlobalLists, bool resetTextures)
        {
            bool newBounds = true;

            active = visible;
            if (resetTextures) DisposeTextures();
            
            maskNodeGroup = GetGroup<TC_NodeGroup>(0, refresh, resetTextures);

            if (maskNodeGroup == null) active = false;
            else 
            {
                maskNodeGroup.type = NodeGroupType.Mask;
                if (maskNodeGroup.active)
                {
                    if (newBounds) bounds = maskNodeGroup.bounds;
                    else bounds.Encapsulate(maskNodeGroup.bounds);
                }
            }

            if (t.childCount <= 1) active = false;
            else
            {
                Transform child = t.GetChild(1);
                groupResult = child.GetComponent<TC_LayerGroupResult>();

                if (groupResult == null)
                {
                    TC.MoveToDustbin(child);
                    active = false;
                }
                else
                {
                    groupResult.SetParameters(this, 1);
                    groupResult.GetItems(refresh, rebuildGlobalLists, resetTextures);
                    if (!groupResult.active) active = false;
                }
            }
        }

        public override void ChangeYPosition(float y)
        {
            if (groupResult != null) groupResult.ChangeYPosition(y);
        }

        public override bool ContainsCollisionNode()
        {
            if (groupResult != null) return groupResult.ContainsCollisionNode();
            return false;
        }

        public int ExecuteCommand(string[] arg)
        {
            if (arg == null) return -1;
            if (arg.Length == 0) return -1;

            int returnValue = -1;
            
            if (arg[0] == "LayerGroup" || arg[0] == "All")
            {

            }
            
            if (arg[0] != "LayerGroup")
            {
                if (arg.Length <= 1) return -1;

                if (groupResult != null) returnValue = groupResult.ExecuteCommand(arg);
            }

            return returnValue;
        }
    }
}