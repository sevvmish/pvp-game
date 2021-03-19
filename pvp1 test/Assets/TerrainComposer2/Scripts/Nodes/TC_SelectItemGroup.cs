using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace TerrainComposer2
{
    public class TC_SelectItemGroup : TC_GroupBehaviour
    {
        public List<TC_SelectItem> itemList = new List<TC_SelectItem>();
        [NonSerialized] public TC_SelectItem refreshRangeItem;
        public bool refreshRanges;

        public SplatCustom[] splatMixBuffer;
        public ColorItem[] colorMixBuffer;
        public ItemSettings[] indices; // layerLevel item list
        public Transform endT;
        public Vector2 scaleMinMaxMulti = Vector2.one;
        public float scaleMulti = 1;
        
        public float mix;

        public float scale = 1;
        public bool linkScaleToMask = true;
        public float linkScaleToMaskAmount = 1;

        public bool untouched = true;
        public int placed;

        public override void Awake()
        {
            if (!firstLoad)
            {
                t = transform;
                GetItems(true, true, false);

                if (TC_Settings.instance) linkScaleToMask = TC_Settings.instance.global.linkScaleToMaskDefault;
            }

            base.Awake();
            
            t.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            t.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector; 
        }

        public override void OnDestroy()
        {
            if (preview.tex != null)
            {
                #if UNITY_EDITOR
                    DestroyImmediate(preview.tex);
                #else
                    Destroy(preview.tex);
                #endif
            }
            base.OnDestroy();
        }

        public override void CloneSetup()
        {
            base.CloneSetup();
            if (TC_Settings.instance == null) return;
            TC_Settings.instance.HasMasterTerrain(); // TODO make solution for this
            
            preview.tex = null;
            GetItems(true, true, false);
        }

        public override void OnTransformChildrenChanged()
        {
            refreshRanges = true;
            base.OnTransformChildrenChanged();
        }

        public void ResetPlaced()
        {
            for (int i = 0; i < itemList.Count; i++) itemList[i].placed = 0;
        }

        public int CalcPlaced()
        {
            placed = 0;
            for (int i = 0; i < itemList.Count; i++) placed += itemList[i].placed;
            return placed;
        }

        public void SetReadWriteTextureItems()
        {
            for (int i = 0; i < itemList.Count; i++) TC.SetTextureReadWrite(itemList[i].preview.tex);
        }

        public void ResetObjects()
        {
            for (int i = 0; i < itemList.Count; i++) itemList[i].ResetObjects();
        }

        public void CalcPreview(bool calcValues = true)
        {
            if (!TC_Settings.instance.hasMasterTerrain) return;

            if (itemList.Count > 1)
            {
                int length = TC.GetTerrainSplatTextureLength(TC_Settings.instance.masterTerrain);

                if (outputId == TC.splatOutput)
                {
                    Texture2D[] texArray = TC.GetTerrainSplatTextures(TC_Settings.instance.masterTerrain);

                    for (int i = 0; i < length; i++)
                    {
                        TC.SetTextureReadWrite(texArray[i]);
                    }
                    CalcPreview(texArray);
                }
                else
                {
                    if (outputId == TC.grassOutput)
                    {
                        for (int i = 0; i < itemList.Count; i++) TC.SetTextureReadWrite(itemList[i].preview.tex);
                    }
                    CalcPreview(null);
                }
            }

            CreateMixBuffer();

            // Debug.Log("CalcPreview");
            TC.AutoGenerate();
        }

        void CalcPreview(Texture2D[] texArray)
        {
            preview.Init(128); 

            float resolution = preview.tex.width;
            
            // TC_Reporter.BenchmarkStart();

            Color[] splatColors = TC_Settings.instance.global.previewColors;

            for (float y = 0; y < resolution; y++)
            {
                float normY = y / resolution;

                for (float x = 0; x < resolution; x++)
                {
                    Color color = Color.black;
                    float total = 0;

                    float normX = x / resolution;

                    if (normX > 0.90f) color = Color.white * normY;

                    for (int i = 0; i < itemList.Count; i++)
                    {
                        TC_SelectItem item = itemList[i];
                        if (!item.active) continue;

                        float v = EvaluateItem(item, normY);

                        if ((normY) < item.range.y + 0.004f && (normY) > item.range.y - 0.004f)
                        {
                            color = Color.red;
                            total = 1;
                            break;
                        }
                        else if (normX > 0.80f && normX <= 0.90f && outputId != TC.colorOutput)
                        {
                            float g = (normX - 0.80f) * 50;
                            if (item.splatCustom)
                            {
                                for (int j = 0; j < texArray.Length; j++) color += (item.splatCustomValues[j] / item.splatCustomTotal) * splatColors[j] * v * g;
                            }
                            else
                            {
                                color += item.color * v * g;
                            }
                            total += v * g;
                        }
                        if ((v > 0 && normX <= 0.9f) || outputId == TC.colorOutput)
                        {
                            if (item.splatCustom)
                            {
                                for (int j = 0; j < texArray.Length; j++) color += (item.splatCustomValues[j] / item.splatCustomTotal) * texArray[j].GetPixel(Mathf.RoundToInt(normX * item.preview.tex.width), Mathf.RoundToInt(normY * item.preview.tex.height)) * v;
                            }
                            else
                            {
                                if (outputId != TC.colorOutput && item.preview.tex != null) color += item.preview.tex.GetPixel(Mathf.RoundToInt(normX * item.preview.tex.width), Mathf.RoundToInt(normY * item.preview.tex.height)) * v * Mathf.Lerp(1, 0, (normX - 0.8f) * 10);
                                else color += item.color * v * Mathf.Lerp(1, 0, (normX - 0.8f) * 10);
                            }
                            total += v * Mathf.Lerp(1, 0, (normX - 0.8f) * 10);
                        }
                    }

                    if (normX <= 0.9f) color /= total;

                    preview.SetPixelColor(Mathf.RoundToInt(normX * resolution), Mathf.RoundToInt(normY * resolution), color);
                }
            }

            // TC_Reporter.BenchmarkStop();
            preview.UploadTexture();
        }

        public override void GetItems(bool refresh, bool rebuildGlobalLists, bool resetTextures)
        {
            if (resetTextures) DisposeTextures();

            itemList.Clear();
            
            totalActive = 0;

            int listIndex = 0;

            for (int i = t.childCount - 1; i >= 0; i--)
            {
                Transform child = t.GetChild(i);

                TC_SelectItem selectItem = child.GetComponent<TC_SelectItem>();
                if (selectItem == null) TC.MoveToDustbin(child);
                else
                {
                    selectItem.SetParameters(this, listIndex);
                    selectItem.parentItem = this;

                    selectItem.active = selectItem.visible;

                    if (outputId == TC.splatOutput && selectItem.splatCustom)
                    {
                        if (TC_Settings.instance.hasMasterTerrain)
                        {
                            int length = TC.GetTerrainSplatTextureLength(TC_Settings.instance.masterTerrain);
                            if (selectItem.splatCustomValues == null) selectItem.splatCustomValues = new float[length];
                            else if (selectItem.splatCustomValues.Length != length) selectItem.splatCustomValues = Mathw.ResizeArray(selectItem.splatCustomValues, length);
                        }
                        selectItem.CalcSplatCustomTotal();
                    }

                    selectItem.SetPreviewItemTexture(); // Put deactive if listIndex is out of bounds

                    if (selectItem.active) ++totalActive;

                    if (outputId == TC.treeOutput)
                    {
                        if (selectItem.tree == null) selectItem.tree = new TC_SelectItem.Tree();

                        if (selectItem.active)
                        {
                            bool addToList = true;

                            List<TC_SelectItem> treeSelectItems = TC_Area2D.instance.terrainLayer.treeSelectItems;

                            if (!rebuildGlobalLists)
                            {
                                int index = treeSelectItems.IndexOf(selectItem);
                                if (index != -1) { addToList = false; selectItem.globalListIndex = index; }
                            }

                            if (addToList)
                            {
                                treeSelectItems.Add(selectItem);
                                selectItem.globalListIndex = treeSelectItems.Count - 1;
                            }
                        }
                    }
                    else if (outputId == TC.objectOutput)
                    {
                        if (selectItem.spawnObject == null) selectItem.spawnObject = new TC_SelectItem.SpawnObject();
                        if (selectItem.spawnObject.go == null 
                            || (selectItem.spawnObject.parentMode == TC_SelectItem.SpawnObject.ParentMode.Existing && selectItem.spawnObject.parentT == null) 
                            || (selectItem.spawnObject.parentMode == TC_SelectItem.SpawnObject.ParentMode.Create && selectItem.spawnObject.parentName == ""))
                        {
                            selectItem.active = false;
                            TC_Area2D.instance.terrainLayer.objectSelectItems.Remove(selectItem);
                            // Debug.Log("Remove from list");
                        }

                        if (selectItem.active)
                        {
                            bool addToList = true;

                            TC_Area2D area2D;

                            if (TC_Area2D.instance == null)
                            {
                                area2D = FindObjectOfType<TC_Area2D>();
                            }
                            else area2D = TC_Area2D.instance;

                            List<TC_SelectItem> objectSelectItems = area2D.terrainLayer.objectSelectItems;

                            if (!rebuildGlobalLists)
                            {
                                int index = objectSelectItems.IndexOf(selectItem);
                                if (index != -1) { addToList = false; selectItem.globalListIndex = index; }
                            }

                            if (addToList)
                            {
                                objectSelectItems.Add(selectItem);
                                selectItem.globalListIndex = objectSelectItems.Count - 1;
                            }
                            selectItem.selectIndex = listIndex;
                        }
                    }
                    
                    selectItem.SetPreviewItemTexture();
                    if (selectItem.outputId != TC.colorOutput) selectItem.SetPreviewColor();

                    itemList.Add(selectItem);
                    listIndex++;
                }
            }

            if (refreshRangeItem != null || refreshRanges) {
                refreshRanges = false;
                RefreshRanges();
                refreshRangeItem = null;
            }
            else if (refresh || TC.refreshPreviewImages) CalcPreview();
        }

        public void RefreshRanges()
        {
            if (itemList.Count <= 1) untouched = true;
            if (untouched) ResetRanges(); else SetRanges(refreshRangeItem);
        }

        public void CreateMixBuffer()
        {
            if (outputId == TC.splatOutput || outputId == TC.grassOutput) CreateSplatMixBuffer();
            else if (outputId == TC.colorOutput) CreateColorMixBuffer();
            else CreateItemMixBuffer();
        }
        
        public void CreateItemMixBuffer()
        {
            // Debug.Log("Create Item Mix buffer");

            if (indices == null) indices = new ItemSettings[totalActive];
            else if (indices.Length != itemList.Count) indices = new ItemSettings[totalActive];

            int index = 0;
            
            for (int i = 0; i < itemList.Count; i++)
            {
                TC_SelectItem item = itemList[i];

                if (item.active)
                {
                    indices[index++] = new ItemSettings(item.globalListIndex, outputId == TC.treeOutput ? item.tree.randomPosition : item.spawnObject.randomPosition, item.range, item.opacity * opacity);
                    // Debug.Log(indices[index - 1].randomPosition);
                }
            }
        }

        public void CreateColorMixBuffer()
        {
            if (colorMixBuffer == null) colorMixBuffer = new ColorItem[totalActive];
            if (colorMixBuffer.Length != totalActive) colorMixBuffer = new ColorItem[totalActive];

            float blend = (mix + 0.001f) / totalActive;
            float range = 1 / blend;
            
            int index = 0;

            for (int i = 0; i < itemList.Count; i++)
            {
                TC_SelectItem item = itemList[i];

                if (item.active) colorMixBuffer[index++] = new ColorItem(new Vector3(item.range.x - blend, item.range.y, range), item.color);
                // Debug.Log(colorMixBuffer[i].select);
            }
        }

        public void CreateSplatMixBuffer()
        {
            // Debug.Log("Create splat mix buffer");

            if (splatMixBuffer == null) splatMixBuffer = new SplatCustom[totalActive];
            if (splatMixBuffer.Length != totalActive) splatMixBuffer = new SplatCustom[totalActive];

            float splatCustomTotal;
            float[] splatCustomValues;
            Vector4 custom0, custom1;

            float blend = (mix + 0.001f) / totalActive;
            float range = 1 / blend;
            
            int index = 0;

            for (int i = 0; i < itemList.Count; ++i)
            {
                TC_SelectItem selectItem = itemList[i];

                if (selectItem.active)
                {
                    if (selectItem.splatCustom)
                    {
                        splatCustomTotal = selectItem.splatCustomTotal;
                        splatCustomValues = selectItem.splatCustomValues;

                        custom0 = new Vector4(splatCustomValues[0] / splatCustomTotal, splatCustomValues[1] / splatCustomTotal, splatCustomValues[2] / splatCustomTotal, splatCustomValues[3] / splatCustomTotal);
                        custom1 = new Vector4(splatCustomValues[4] / splatCustomTotal, splatCustomValues[5] / splatCustomTotal, splatCustomValues[6] / splatCustomTotal, splatCustomValues[7] / splatCustomTotal);

                        // Debug.Log(custom0 + " - " + custom1);
                    }
                    else
                    {
                        custom0 = Vector4.zero;
                        custom1 = Vector4.zero;
                    }
                    
                    splatMixBuffer[index++] = new SplatCustom(new Vector4(selectItem.range.x - blend, selectItem.range.y, range, selectItem.splatCustom ? -1 : selectItem.selectIndex), custom0, custom1, custom0, custom1);
                }
            }
        }

        public float EvaluateItem(TC_SelectItem selectItem, float time)
        {
            float r1, r2, r3;
            
            float blend = (mix + 0.001f) / totalActive;

            r2 = selectItem.range.x;
            r3 = selectItem.range.y;

            r2 += blend / 2;
            r3 -= blend / 2;

            r1 = r2 - blend;
            
            float range = 1 / blend;

            // Debug.Log(r1 + ", " + r2 + ", " + r3);
            // Debug.Log(range);

            if (time < r3) return Mathf.Lerp(0, 1, Mathf.Clamp01(time - r1) * range);
            else return Mathf.Lerp(1, 0, Mathf.Clamp01(time - r3) * range);
        }

        public void SetRanges(TC_SelectItem changedSelectItem = null, bool resetInActive = false)
        {
            if (itemList.Count == 0) { untouched = true; return; }
            if (changedSelectItem == null) changedSelectItem = itemList[0];
            
            TC_SelectItem selectItem;

            int index = changedSelectItem.active ? changedSelectItem.listIndex : 0;

            float x = itemList[index].range.x;
            float y = itemList[index].range.y;
            
            if (!resetInActive) untouched = false;

            for (int i = index + 1; i < itemList.Count; ++i)
            {
                selectItem = itemList[i];

                if (selectItem.active)
                {
                    selectItem.range.x = y;
                    selectItem.range.y = y = Mathf.Max(selectItem.range.x, selectItem.range.y);
                }
            }

            for (int i = index - 1; i >= 0; --i)
            {
                selectItem = itemList[i];

                if (selectItem.active)
                {
                    selectItem.range.y = x;
                    selectItem.range.x = x = Mathf.Min(selectItem.range.x, selectItem.range.y);
                }
            }

            selectItem = GetActiveItemUp(0);
            if (selectItem != null) selectItem.range.x = 0;

            selectItem = GetActiveItemDown(itemList.Count - 1);
            if (selectItem != null) selectItem.range.y = 1;

            if (resetInActive)
            {
                TC_SelectItem neighborItem;

                for (int i = 0; i < itemList.Count; i++)
                {
                    selectItem = itemList[i];

                    if (!selectItem.active)
                    {
                        neighborItem = GetActiveItemDown(i - 1);
                        if (neighborItem != null) selectItem.range.y = selectItem.range.x = neighborItem.range.y;
                        else selectItem.range.y = 0;

                        neighborItem = GetActiveItemUp(i + 1);
                        if (neighborItem != null) selectItem.range.x = selectItem.range.y = neighborItem.range.x;
                        else selectItem.range.x = 1;

                    }
                }
            }
            
            CalcPreview();
        }

        TC_SelectItem GetActiveItemUp(int index)
        {
            for (int i = index; i < itemList.Count; i++)
            {
                TC_SelectItem selectItem = itemList[i];
                if (selectItem.active) return selectItem;
            }
            return null;
        }

        TC_SelectItem GetActiveItemDown(int index)
        {
            // Debug.Log(index + " " + itemList.Count);
            for (int i = index; i >= 0; i--)
            {
                TC_SelectItem selectItem = itemList[i];
                if (selectItem.active) return selectItem;
            }

            return null;
        }

        public Vector2 GetInbetweenRange(int index)
        {
            Vector2 range;
            TC_SelectItem item;
            item = GetActiveItemUp(index);
            if (item != null) range.x = item.range.y; else range.x = 0;
            item = GetActiveItemDown(index);
            if (item != null) range.y = item.range.x; else range.y = 1;

            return range;
        }

        public void ResetRanges()
        {
            if (itemList.Count == 0) return;

            float average = 1.0f / totalActive;
            float x = 0;

            for (int i = 0; i < itemList.Count; i++)
            {
                TC_SelectItem selectItem = itemList[i];

                if (selectItem.active)
                {
                    selectItem.range = new Vector2(x, x + average);
                    x += average;
                }
            }

            SetRanges(itemList[0], true); 
        }

        public void CenterRange(TC_SelectItem changedSelectItem)
        {
            if (!changedSelectItem.active) return;

            float average = 1.0f / totalActive;
            float x = 0;
            
            int index = 0;

            for (int i = 0; i < itemList.Count; i++)
            {
                TC_SelectItem selectItem = itemList[i];

                if (selectItem == changedSelectItem) { selectItem.range = new Vector2(x, x + average); index = i; break; }
                if (selectItem.active) x += average;
            }

            SetRanges(itemList[index], true);
        }

        public Vector2[] GetRanges()
        {
            Vector2[] ranges = new Vector2[itemList.Count];
            for (int i = 0; i < itemList.Count; i++) ranges[i] = itemList[i].range;
            return ranges;
        }

        public void SetRanges(Vector2[] ranges)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                if (i < ranges.Length) itemList[i].range = ranges[i];
            }
        }

        public override void SetFirstLoad(bool active)
        {
            base.SetFirstLoad(active);
            for (int i = 0; i < itemList.Count; i++) itemList[i].SetFirstLoad(active);
        }
    }
    
    //[Serializable]
    //public class SelectGroupItem
    //{
    //    public itemGroup itemGroup;
    //    public item item;

    //    public SelectGroupItem(itemGroup layerGroup)
    //    {
    //        this.itemGroup = layerGroup;
    //    }

    //    public SelectGroupItem(item select)
    //    {
    //        this.item = select;
    //    }
    //}
    
    [Serializable]
    public struct SplatCustom
    {
        public Vector4 select;
        public Vector4 map0;
        public Vector4 map1;
        public Vector4 map2;
        public Vector4 map3;

        public SplatCustom(Vector4 select, Vector4 map0, Vector4 map1, Vector4 map2, Vector4 map3)
        {
            this.select = select;
            this.map0 = map0;
            this.map1 = map1;
            this.map2 = map2;
            this.map3 = map3;
        }
    }

    [Serializable]
    public struct ColorItem
    {
        public Vector3 select;
        public Vector4 color;

        public ColorItem(Vector3 select, Vector4 color)
        {
            this.select = select;
            this.color = color;
        }
    }

    public struct ItemSettings
    {
        public int index;
        public float randomPosition;
        public Vector2 range;
        public float opacity;

        public ItemSettings(int index, float randomPosition, Vector2 range, float opacity)
        {
            this.index = index;
            this.randomPosition = randomPosition;
            this.range = range;
            this.opacity = opacity;
        }
    }
}