using System;
using UnityEngine;
using UnityEditor;
using System.Collections;


namespace TerrainComposer2
{
    static public class TC_SelectItemGUI
    {
        // static public FilterGroupNode parent;
        static public void DrawSplatCustomPreview(TC_SelectItem selectItem, Rect rect)
        {
            // selectItem.CalcSplatCustomTotal();

            GUI.color = Color.black;
            EditorGUI.DrawPreviewTexture(rect, Texture2D.whiteTexture);
            Texture2D splatTexture;

            for (int i = 0; i < selectItem.splatCustomValues.Length; i++)
            {
                GUI.color = new Color(1, 1, 1, (selectItem.splatCustomValues[i] / selectItem.splatCustomTotal) * 1.5f);
                TC.GetTerrainSplatTexture(TC_Settings.instance.masterTerrain, i, out splatTexture);
                EditorGUI.DrawPreviewTexture(rect, splatTexture);
            }

            GUI.color = Color.white;
        }

        static public void Draw(TC_SelectItemGroup selectItemGroup, float activeMulti, int index, bool nodeFoldout, ref Vector2 pos, Color color)
        {
            TC_SelectItem selectItem = selectItemGroup.itemList[index];
            Undo.RecordObject(selectItem, selectItem.name);

            Rect rectPreview;
            bool isCulled = false;
            if (selectItem.outputId == TC.colorOutput)
            {
                if (selectItem.texColor != null && selectItem.parentItem.itemList.Count == 1) rectPreview = TD.DrawNode(selectItem, pos, color, Color.white, ref isCulled, activeMulti, nodeFoldout, false, false);
                else rectPreview = TD.DrawNode(selectItem, pos, color, selectItem.color, ref isCulled, activeMulti, nodeFoldout, false, false);
            }
            else rectPreview = TD.DrawNode(selectItem, pos, color, Color.white, ref isCulled, activeMulti, nodeFoldout, false, !selectItem.splatCustom);
            if (isCulled || !nodeFoldout) return;

            if (TC_Settings.instance.hasMasterTerrain)
            {
                Vector2 sliderPos = TD.GetPositionScaled(new Vector2(pos.x + 10.5f, pos.y + 297.5f));
                GUIUtility.ScaleAroundPivot(new Vector2(TD.scale * 2.25f, TD.scale * 2.25f), new Vector2(sliderPos.x, sliderPos.y));

                if (selectItem.outputId != TC.objectOutput)
                {
                    if (selectItem.outputId == TC.colorOutput)
                    {
                        Color colOld = selectItem.color;
                        if (Event.current.button != 2) selectItem.color = EditorGUI.ColorField(new Rect(sliderPos.x, sliderPos.y + 4, 93, 10), selectItem.color);
                        else EditorGUI.ColorField(new Rect(sliderPos.x, sliderPos.y + 4, 93, 10), selectItem.color);
                        selectItem.color.a = 1;
                        if (selectItem.color != colOld) selectItem.Refresh();
                    }
                    else
                    {
                        int selectIndexOld = selectItem.selectIndex;
                        int total = selectItem.GetItemTotalFromTerrain();
                        if (total > 1)
                        {
                            if (selectItem.outputId == TC.treeOutput) sliderPos.y -= 17;
                            if (Event.current.button != 2) selectItem.selectIndex = (int)GUI.HorizontalSlider(new Rect(sliderPos.x, sliderPos.y, 110, 16), selectItem.selectIndex, 0, total - 1);
                            else GUI.HorizontalSlider(new Rect(sliderPos.x, sliderPos.y, 110, 16), selectItem.selectIndex, 0, total - 1);
                        }

                        if (selectItem.selectIndex != selectIndexOld) selectItem.Refresh();
                    }
                }

                if (selectItem.outputId == TC.splatOutput)
                {
                    // if (selectItem.splatCustom) DrawSplatCustomPreview(selectItem, rectPreview);
                }
                
                GUI.matrix = Matrix4x4.Scale(new Vector3(1, 1, 1));

                TC_NodeGUI.DrawAddItem(rectPreview, pos, selectItem);
                TC_NodeGUI.LeftClickMenu(rectPreview, selectItem);
            }
            
            if (selectItem.outputId != TC.colorOutput)
            {
                Rect colorRect = new Rect(rectPreview.x + 0 * TD.scale, rectPreview.y +  0 * TD.scale, 60 * TD.scale, 16f * TD.scale);
                GUI.color = new Color(selectItem.color.r, selectItem.color.g, selectItem.color.b, 0.75f);
                GUI.DrawTexture(colorRect, Texture2D.whiteTexture);
                GUI.color = Color.white;
            }
        }
    }
}