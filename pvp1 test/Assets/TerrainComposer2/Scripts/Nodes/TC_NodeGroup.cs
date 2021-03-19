using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TerrainComposer2
{
    public class TC_NodeGroup : TC_GroupBehaviour
    {
        [NonSerialized] public List<TC_ItemBehaviour> itemList = new List<TC_ItemBehaviour>();
        public int nodeGroupLevel;
        public NodeGroupType type;
        public RenderTexture rtColorPreview;
        public bool useConstant;

        public float seed = 0;

        public override void Awake()
        {
            rtColorPreview = null;
            base.Awake();
        }

        public override void OnDestroy()
        {
            DisposeTextures();
            
        }

        public override void DisposeTextures()
        {
            base.DisposeTextures();
            TC_Compute.DisposeRenderTexture(ref rtColorPreview);
        }

        public void LinkClone(TC_NodeGroup nodeGroupS)
        {
            preview = nodeGroupS.preview;

            for (int i = 0; i < itemList.Count; i++)
            {
                TC_Node node = itemList[i] as TC_Node;
                if (node != null)
                {
                    TC_Node sNode = nodeGroupS.itemList[i] as TC_Node;
                    node.preview = sNode.preview;
                    node.Init();
                }
            }
        }

        public override void SetLockChildrenPosition(bool lockPos)
        {
            lockPosParent = lockPos;
            
            for (int i = 0; i < itemList.Count; i++)
            {
                TC_Node node = itemList[i] as TC_Node;
                if (node != null) node.lockPosParent = lockPosParent || lockPosChildren;
            }
        }

        public override void UpdateTransforms()
        {
            // ct.CopySpecial(this);

            for (int i = 0; i < itemList.Count; i++)
            {
                TC_Node node = itemList[i] as TC_Node;
                if (node != null) node.ct.CopySpecial(node);
            }
        }

        public ComputeBuffer ComputeValue(float seedParent)
        {
            TC_Compute compute = TC_Compute.instance;
            if (compute == null) Debug.Log("Compute is null");

            ComputeBuffer nodeBuffer = null;
            ComputeBuffer totalBuffer = null;
            
            bool inputCurrent;

            if (totalActive > 1) InitPreviewRenderTexture(true, name);

            int length = useConstant ? 1 : itemList.Count;

            float seedTotal = seed + seedParent;

            for (int i = 0; i < length; i++)
            {
                TC_Node node = itemList[i] as TC_Node;

                if (node != null)
                {
                    node.Init();
                    
                    if (!node.active) continue;
                    
                    if (node.clamp)
                    {
                        // if (node.OutOfBounds()) continue;
                    }

                    inputCurrent = (node.inputKind == InputKind.Current);
                    node.InitPreviewRenderTexture(true, node.name);

                    if (totalBuffer == null && !inputCurrent)
                    {
                        totalBuffer = compute.RunNodeCompute(this, node, seedTotal);
                    }
                    else
                    {
                        if (!inputCurrent)
                        {
                            // Debug.Log(totalBuffer == null);
                            nodeBuffer = compute.RunNodeCompute(this, node, seedTotal, totalBuffer, false);
                        }
                        else
                        {
                            for (int j = 0; j < node.iterations; j++) totalBuffer = compute.RunNodeCompute(this, node, seedTotal, totalBuffer, true);
                            // if (preview && totalBuffer != null) { compute.DisposeBuffer(ref totalBuffer); }
                        }

                        // if (preview && nodeBuffer != null) { compute.DisposeBuffer(ref nodeBuffer); }
                    }
                    if (totalBuffer != null && nodeBuffer != null && !inputCurrent) compute.RunComputeMethod(this, node, totalBuffer, ref nodeBuffer, itemList.Count, i == lastActive ? rtPreview : null);
                }
                else
                {
                    TC_NodeGroup nodeGroup = itemList[i] as TC_NodeGroup;

                    if (nodeGroup != null)
                    {
                        if (!nodeGroup.active) continue;
                        if (totalBuffer == null) totalBuffer = nodeGroup.ComputeValue(seedTotal);
                        else nodeBuffer = nodeGroup.ComputeValue(seedTotal);

                        if (totalBuffer != null && nodeBuffer != null) compute.RunComputeMethod(this, nodeGroup, totalBuffer, ref nodeBuffer, itemList.Count, i == lastActive ? rtPreview : null);
                    }
                }
            }

            if (totalActive == 1)
            {
                TC_Compute.DisposeRenderTexture(ref rtPreview);
                rtDisplay = itemList[firstActive].rtDisplay;
            }

            if (isPortalCount > 0 && totalBuffer != null) TC_Compute.instance.MakePortalBuffer(this, totalBuffer);

            return totalBuffer;
        }

        public override void ChangeYPosition(float y) { for (int i = 0; i < itemList.Count; i++) itemList[i].ChangeYPosition(y); }

        public override void SetFirstLoad(bool active)
        {
            base.SetFirstLoad(active);
            for (int i = 0; i < itemList.Count; i++) itemList[i].SetFirstLoad(active);
        }

        public override bool ContainsCollisionNode()
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                TC_Node node = itemList[i] as TC_Node;

                if (node != null)
                {
                    if (node.active && node.visible)
                    {
                        if (node.inputKind == InputKind.Terrain && node.inputTerrain == InputTerrain.Collision) return true;
                    }
                }
            }

            return false;
        }

        public override void GetItems(bool refresh, bool rebuildGlobalLists, bool resetTextures)
        {
            if (resetTextures) DisposeTextures();

            int childCount = transform.childCount;
            // Init();
            itemList.Clear();

            active = visible;
            
            firstActive = lastActive = -1;
            totalActive = 0;

            bool newBounds = true;
            int listIndex = 0;
            
            for (int i = childCount - 1; i >= 0; i--)
            {
                Transform child = t.GetChild(i);
                
                TC_Node node = child.GetComponent<TC_Node>();
                
                if (node != null)
                {
                    if (resetTextures) node.DisposeTextures();
                    node.active = true;
                    node.Init();
                    if (node.inputKind == InputKind.Current && totalActive == 0)
                    {
                        TC.AddMessage("'Current' can only be used if there is active node/s before it.");
                        node.active = false;
                    }
                    if (!node.visible)
                    {
                        node.active = false;
                        // Debug.Log(node.name);
                    }
                    node.SetParameters(this, listIndex);
                    node.nodeGroupLevel = nodeGroupLevel + 1;
                    node.nodeType = type;

                    node.UpdateVersion();

                    if (node.active)
                    {
                        if (node.clamp) node.CalcBounds();
                        if (newBounds) { bounds = node.bounds; newBounds = false; }
                        else bounds.Encapsulate(node.bounds);

                        lastActive = listIndex;
                        if (firstActive == -1) firstActive = lastActive;
                        ++totalActive;
                    }

                    if (i == childCount - 1) // TODO: Consider hide and do in calculation
                    {
                        if (node.method != Method.Add && node.method != Method.Subtract) node.method = Method.Add;
                    }

                    itemList.Add(node);
                    ++listIndex;
                }
                else
                {
                    TC_NodeGroup nodeGroup = child.GetComponent<TC_NodeGroup>();

                    if (nodeGroup != null)
                    {
                        nodeGroup.SetParameters(this, listIndex);
                        nodeGroup.nodeGroupLevel = nodeGroupLevel + 1;
                        itemList.Add(nodeGroup);
                        ++listIndex;
                        nodeGroup.GetItems(refresh, rebuildGlobalLists, resetTextures);

                        if (nodeGroup.active)
                        {
                            lastActive = listIndex;
                            if (firstActive == -1) firstActive = lastActive;
                            ++totalActive;
                        }
                    }
                    //else
                    //{
                    //    TC_NodeClone nodeClone = child.GetComponent<TC_NodeClone>();
                    //}
                        
                }
            }

            if (itemList.Count == 1)
            {
                if (itemList[0].active) active = visible = true;
            }

            if (!active) totalActive = 0; 
            if (totalActive == 0) active = false;
        }
    }
}
