using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

// TODO: Fix method in select and mask node group, make distinct between layer and inside

namespace TerrainComposer2
{
    [ExecuteInEditMode]
    public class TC_ItemBehaviour : MonoBehaviour
    {
        [NonSerialized] public TC_ItemBehaviour parentItem;
        static public event RepaintAction DoRepaint;
        public delegate void RepaintAction();
        
        public float versionNumber = 0;
        public bool defaultPreset;
        public bool isLocked;
        public bool autoGenerate = true;
        public bool visible = true;
        public bool active = true;
        public int foldout = 2;
        public bool nodeFoldout = true;
        public int outputId;
        public int terrainLevel;
        public int level;
        public string notes;

        public int listIndex;
        public bool firstLoad; 

        public TexturePreview preview = new TexturePreview();
        public RenderTexture rtDisplay, rtPreview;
        public RenderTexture rtPortal;
        public int isPortalCount = 0;
        public TC_ItemBehaviour portalNode;
        public List<TC_ItemBehaviour> usedAsPortalList;
        
        public Method method;

        public float opacity = 1;
        public bool abs = false;

        public Curve localCurve = new Curve();
        public Curve worldCurve = new Curve();

        public Transform t;
        public Transform parentOld;
        public int siblingIndexOld = -1;

        public CachedTransform ct = new CachedTransform();
        public CachedTransform ctOld = new CachedTransform();
        public Bounds bounds;

        public bool lockTransform, lockPosParent, lockPosChildren;
        public bool lockPosX = true, lockPosY = true, lockPosZ = true, lockRotY = true, lockScaleX = true, lockScaleY = true, lockScaleZ = true;
        public PositionMode positionMode;
        public float posY;
        public Vector3 posOffset;
        
        [NonSerialized] public DropPosition dropPosition;
        public bool controlDown;

        [SerializeField]int instanceID = 0;


        public void SetVersionNumber()
        {
            versionNumber = TC.GetVersionNumber();
        }

        public void Repaint()
        {
            if (DoRepaint != null) DoRepaint();
        }

        public void InitPreviewRenderTexture(bool assignRtDisplay = true, string name = "Preview")
        {
            TC_Compute.InitPreviewRenderTexture(ref rtPreview, name);
            if (assignRtDisplay) rtDisplay = rtPreview;
        }
        
        public virtual void Awake()
        {
            // Debug.Log("Awake");
            if (!firstLoad)
            {

                firstLoad = true;
                #if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(this);
                #endif
            }

            rtDisplay = null;
            rtPreview = null;

            t = transform;
            t.hasChanged = false;
            
            // Debug.Log("Awake " + name);
            // TCGenerate.singleton.AutoGenerate();
            // RemoveCloneText();
            DetectClone();
        }

        public void Lock(bool active)
        {
            if (active)
            {
                t.hideFlags = HideFlags.NotEditable;
                hideFlags = HideFlags.NotEditable;
            }
            else
            {
                t.hideFlags = HideFlags.None;
                hideFlags = HideFlags.None;
            }
        }

        void DetectClone()
        {
            // Detect duplicate
            if (instanceID == 0) { instanceID = GetInstanceID(); return; }

            if (instanceID != GetInstanceID() && GetInstanceID() < 0)
            {
                // Debug.Log("Detected Duplicate!");
                instanceID = GetInstanceID();

                CloneSetup();
            }
        }

        public virtual void CloneSetup()
        {
            RemoveCloneText();
        }
       
        public virtual void OnEnable()
        {
            // preview.Create(128);
            if (TC_Settings.instance == null) TC_Settings.GetInstance();
        }

        public void OnDisable()
        {
            // if (autoGenerate) TC_Generate.AutoGenerate();
        }

        public virtual void OnDestroy()
        {
            // RemovePortalNodes();
            RemoveFromPortalNode();
            DisposeTextures();
            // Debug.Log("Destroy");  
            // TCGenerate.singleton.AutoGenerate();   
        }

        public virtual void DisposeTextures()
        {
            rtDisplay = null;
            TC_Compute.DisposeRenderTexture(ref rtPreview);
            TC_Compute.DisposeRenderTexture(ref rtPortal);
        }

        void RemoveCloneText()
        {
            string name = t.name;
            int index = name.IndexOf("(");
            if (index != -1)
            {
                t.name = name.Remove(index);
                if (TC_Settings.instance.selectionOld != null)
                {
                    index = TC_Settings.instance.selectionOld.GetSiblingIndex();
                    t.SetSiblingIndex(index);
                }
            }
        }

        public virtual void OnTransformChildrenChanged()
        { 
            // Debug.Log("Children " + name + " has changed "+TC.outputNames[outputId]);
            TC.RefreshOutputReferences(outputId, true);
            // GetItems(this);
            // TC_Generate.AutoGenerate();
        }

        public void SetLockPositionXZ(bool active)
        {
            if (active) ctOld.Copy(this);
        }

        //#if UNITY_EDITOR
        //public void OnDrawGizmosSelected()
        //{
        //    if (GetType() == typeof(TC_SelectItemGroup) || GetType() == typeof(TC_SelectItem)) return;

        //    if (UnityEditor.Selection.activeTransform == t)
        //    {
        //        Gizmos.DrawWireCube(bounds.center, bounds.size);
        //        // if (GetType() == typeof(TCNode)) Debug.Log(bounds.size);
        //    }
        //}
        //#endif

        public virtual void ChangeYPosition(float y) { }
        public virtual void SetLockChildrenPosition(bool lockPos) { }
        public virtual void UpdateTransforms() { }

        public void ResetPosition()
        {
            t.localPosition = Vector3.zero;
            // t.localRotation = Quaternion.identity;
            // t.localScale = Vector3.one;
        }

        public void ResetPositionCompensateChildren()
        {
            GameObject go = new GameObject();
            Transform child;

            int childCount = t.childCount;

            for (int i = 0; i < childCount; i++)
            {
                child = t.GetChild(0);
                child.parent = go.transform;
            }

            t.localPosition = Vector3.zero;

            for (int i = 0; i < childCount; i++)
            {
                child = go.transform.GetChild(0);
                child.parent = transform;
            }

            DestroyImmediate(go);
        }

        public virtual void DestroyMe(bool undo)
        {
            // Debug.Log("Destroy");

            #if UNITY_EDITOR
            if (undo) UnityEditor.Undo.DestroyObjectImmediate(gameObject); else DestroyImmediate(gameObject);
            #else
                Destroy(gameObject);
            #endif
        }

        public virtual MonoBehaviour Add<T>(string label, bool addSameLevel, bool addBefore = false, bool makeSelection = false, int startIndex = 1) where T : TC_ItemBehaviour
        {
            GameObject newItemGo = new GameObject();
            Transform newItemT = newItemGo.transform;
            
            Type type = typeof(T);

            if (label == "")
            {
                if (type == typeof(TC_LayerGroup)) label = "Layer Group";
                else if (type == typeof(TC_Layer)) label = "Layer";
                else if (type == typeof(TC_Node)) label = "Node";
                else if (type == typeof(TC_SelectItem)) label = "Item";
                else if (type == typeof(TC_SelectItemGroup)) label = "Item Group";
            }

            // Debug.Log("Add " + label);
            newItemT.name = label;

            #if UNITY_EDITOR
                if (makeSelection) UnityEditor.Selection.activeTransform = newItemT;
            #endif

            int index;
            TC_ItemBehaviour item = newItemGo.AddComponent<T>();
            // Version number
            item.SetVersionNumber();

            item.outputId = outputId;
            
            if (addSameLevel)
            {
                newItemT.parent = t.parent;
                index = t.GetSiblingIndex() + (addBefore ? 1 : 0);
                newItemT.SetSiblingIndex(index);
            }
            else
            {
                if (type == typeof(TC_SelectItemGroup)) { startIndex = 2; }

                newItemT.parent = t;
                newItemT.SetSiblingIndex(index = startIndex);
            }

            if (newItemT.parent != null) newItemT.localPosition = Vector3.zero;

            if (type == typeof(TC_Node)) ((TC_Node)item).SetDefaultSettings();

            else if (type == typeof(TC_LayerGroup))
            {
                if (outputId != TC.heightOutput && outputId != TC.grassOutput) item.method = Method.Lerp;
                item.Add<TC_NodeGroup>("Mask Group", false);
                item.Add<TC_LayerGroupResult>("Result", false);
            }
            else if (type == typeof(TC_Layer))
            {
                if (outputId != TC.heightOutput && outputId != TC.grassOutput) item.method = Method.Lerp;
                AddLayerNodeGroups((TC_Layer)item);
            }

            return item;
        }
        
        public void AddLayerNodeGroups(TC_Layer layer)
        {
            layer.Add<TC_NodeGroup>("Mask Group", false);
            TC_NodeGroup selectNodeGroup = (TC_NodeGroup)layer.Add<TC_NodeGroup>("Select Group", false);
            selectNodeGroup.Add<TC_Node>("", false, false, false);
            
            layer.Add<TC_SelectItemGroup>("", false);
        }

        public T GetGroup<T>(int index, bool refresh, bool resetTextures) where T : TC_GroupBehaviour
        {
            if (resetTextures) DisposeTextures();

            if (index >= t.childCount)
            {
                TC.MoveToDustbin(t);
                return null;
            }

            Transform child = t.GetChild(index);

            T group = child.GetComponent<T>();

            if (group == null)
            {
                TC.MoveToDustbin(t);
            }
            else
            {
                group.SetParameters(this, index);
                group.GetItems(refresh, resetTextures, true);
            }
            
            return group;
        }

        public void SetParameters(TC_ItemBehaviour parentItem, int index)
        {
            this.parentItem = parentItem;
            level = parentItem.level + 1;
            outputId = parentItem.outputId;
            listIndex = index;
        }

        public TC_ItemBehaviour Duplicate(Transform parent)
        {
            // Debug.Log("Duplicate");
            GameObject newGo = (GameObject)Instantiate(gameObject, t.position, t.rotation);
            Transform newT = newGo.transform;
            newT.parent = parent;
            if (dropPosition == DropPosition.Left) newT.SetSiblingIndex(t.GetSiblingIndex() - 1);
            else newT.SetSiblingIndex(t.GetSiblingIndex());
            newT.localScale = t.localScale;
            TC_ItemBehaviour item = newGo.GetComponent<TC_ItemBehaviour>();

            item.usedAsPortalList = null;
            item.isPortalCount = 0;

            if (item.portalNode != null)
            {
                item.portalNode.usedAsPortalList.Add(item);
                item.portalNode.isPortalCount++;
            }
            
            #if UNITY_EDITOR
                UnityEditor.Selection.activeObject = newGo;
                UnityEditor.Undo.RegisterCreatedObjectUndo(newGo, "Duplicate " + newGo.name);
            #endif
            
            return item;
        }

        public void RemovePortalNodes()
        {
            if (usedAsPortalList == null || usedAsPortalList.Count == 0) return;

            for (int i = 0; i < usedAsPortalList.Count; i++)
            {
                usedAsPortalList[i].portalNode = null;
            }
            usedAsPortalList.Clear();
        }

        public void RemoveFromPortalNode()
        {
            if (portalNode == null) return;
            if (portalNode.isPortalCount > 0)
            {
                portalNode.isPortalCount--;
                portalNode.usedAsPortalList.Remove(this);
                if (portalNode.isPortalCount == 0) TC_Compute.DisposeRenderTexture(ref portalNode.rtPortal);
            }
            else portalNode.isPortalCount = 0;
        }

        public void CopyTransform(TC_ItemBehaviour item)
        {
            t.position = item.t.position;
            t.rotation = item.t.rotation;
            t.localScale = item.t.localScale;
        }

        public virtual void GetItems(bool refresh, bool rebuildGlobalLists, bool resetTextures) { }
        public virtual void SetFirstLoad(bool active)
        {
            firstLoad = active;
        }
        public virtual bool ContainsCollisionNode() { return false; }
    }

    [Serializable]
    public class Curve
    {
        public bool active;
        public Vector2 range = new Vector2(0, 1);
        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        
        public Vector4[] c;
        public float[] curveKeys;
        public int length;

        public float Calc(float v)
        {
            return curve.Evaluate(v) * v;
        }

        public void ConvertCurve(float scale = 1)
        {
            if (curve.keys.Length < 2 || !active) { length = 0; return; }

            length = curve.keys.Length;
            c = new Vector4[curve.length - 1];
            curveKeys = new float[curve.length];

            // Reporter.Log("Convert curve "+c.Length+", "+curveKeys.Length);

            float p1x, p1y, tp1, p2x, p2y, tp2;

            for (int i = 0; i < c.Length; i++)
            {
                p1x = curve.keys[i].time;
                p1y = curve.keys[i].value * scale;
                tp1 = curve.keys[i].outTangent;
                p2x = curve.keys[i + 1].time;
                p2y = curve.keys[i + 1].value * scale;
                tp2 = curve.keys[i + 1].inTangent;

                c[i].x = (p1x * tp1 + p1x * tp2 - p2x * tp1 - p2x * tp2 - 2 * p1y + 2 * p2y) / (p1x * p1x * p1x - p2x * p2x * p2x + 3 * p1x * p2x * p2x - 3 * p1x * p1x * p2x);
                c[i].y = ((-p1x * p1x * tp1 - 2 * p1x * p1x * tp2 + 2 * p2x * p2x * tp1 + p2x * p2x * tp2 - p1x * p2x * tp1 + p1x * p2x * tp2 + 3 * p1x * p1y - 3 * p1x * p2y + 3 * p1y * p2x - 3 * p2x * p2y) / (p1x * p1x * p1x - p2x * p2x * p2x + 3 * p1x * p2x * p2x - 3 * p1x * p1x * p2x));
                c[i].z = ((p1x * p1x * p1x * tp2 - p2x * p2x * p2x * tp1 - p1x * p2x * p2x * tp1 - 2 * p1x * p2x * p2x * tp2 + 2 * p1x * p1x * p2x * tp1 + p1x * p1x * p2x * tp2 - 6 * p1x * p1y * p2x + 6 * p1x * p2x * p2y) / (p1x * p1x * p1x - p2x * p2x * p2x + 3 * p1x * p2x * p2x - 3 * p1x * p1x * p2x));
                c[i].w = ((p1x * p2x * p2x * p2x * tp1 - p1x * p1x * p2x * p2x * tp1 + p1x * p1x * p2x * p2x * tp2 - p1x * p1x * p1x * p2x * tp2 - p1y * p2x * p2x * p2x + p1x * p1x * p1x * p2y + 3 * p1x * p1y * p2x * p2x - 3 * p1x * p1x * p2x * p2y) / (p1x * p1x * p1x - p2x * p2x * p2x + 3 * p1x * p2x * p2x - 3 * p1x * p1x * p2x));
            }

            for (int i = 0; i < curveKeys.Length; i++) curveKeys[i] = curve.keys[i].time;

            // Reporter.Log(p1x + ", " + p1y + ", " + tp1 + ", " + p2x + ", " + p2y + ", " + tp2);
            // Reporter.Log("Evaluate Unity: " + anim.Evaluate(0.1f) + ", " + anim.Evaluate(0.2f) + ", " + anim.Evaluate(0.3f) + ", " + anim.Evaluate(0.4f) + ", " + anim.Evaluate(0.5f) + ", " + anim.Evaluate(0.6f) + ", " + anim.Evaluate(0.76f) + ", " + anim.Evaluate(0.88f) + ", " + anim.Evaluate(0.98f));
            // Reporter.Log("Evaluate Cubic: " + Evaluate(0.1f) + ", " + Evaluate(0.2f) + ", " + Evaluate(0.3f) + ", " + Evaluate(0.4f) + ", " + Evaluate(0.5f) + ", " + Evaluate(0.6f) + ", " + Evaluate(0.76f) + ", " + Evaluate(0.88f) + ", " + anim.Evaluate(0.98f));
        }
    }
}