using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TerrainComposer2
{
    static public class TC_SelectItemGroupGUI
    {
        static public int Draw(TC_SelectItemGroup selectItemGroup, ref Vector2 pos, Color color, Color colSelectItem, Color colBracket, float activeMulti)
        {
            bool nodeFoldout = selectItemGroup.parentItem.nodeFoldout;
            // bool select = false;
            // Draw total node
            // DropDownMenu();
            //if (selectItemGroup.itemList.Count > 1)
            //{
            // RenderTexture previewTex;
            // if (selectItemGroup.itemList.Count == 1) previewTex = selectItemGroup.itemList[0].displayRenderTex; else previewTex = selectItemGroup.displayRenderTex;
            // startOffset.x -= TD.texCardBody.width;

            TD.DrawBracket(ref pos, nodeFoldout, true, colBracket * activeMulti, ref selectItemGroup.foldout, true, selectItemGroup.itemList.Count > 0);

            //if (TC_Settings.instance.hasMasterTerrain && selectItemGroup.outputId != TC.treeOutput && selectItemGroup.outputId != TC.objectOutput)
            //{
            //    Vector2 sliderPos = TD.GetPosition(new Vector2(startOffset.x + 6, (startOffset.y + 84)));
            //    GUIUtility.ScaleAroundPivot(new Vector2(TD.scale / 1.5f, TD.scale / 1.5f), new Vector2(sliderPos.x, sliderPos.y));

            //    if (selectItemGroup.itemValues.values.Count > 0)
            //    {
            //        GUI.changed = false;

            //        selectItemGroup.itemValues.values[0].mix = GUI.HorizontalSlider(new Rect(sliderPos.x, sliderPos.y, 93, 10), selectItemGroup.itemValues.values[0].mix, 0, 1.5f);
            //        if (GUI.changed) selectItemGroup.CalcSelectItemGroupPreview();
            //    }
            //// selectItemGroup.itemValues.values[index].value = GUIW.MinMaxSlider(new Rect(sliderPos.x, sliderPos.y + 20, 78, 10), selectItemGroup.itemValues.values[index].value, 0.0f, 100.0f, new Vector2(200.0f, 25.0f));

            //    GUI.matrix = Matrix4x4.Scale(new Vector3(1, 1, 1));
            //}

            //if (selectItemGroup.itemList.Count != 0)
            //{
            //    TD.DrawRect(new Rect(startOffset.x - 9, (startOffset.y + TD.cardHeight / 2) - 5, 18, 10), TC_Settings.instance.global.colTextBackGround);
            //    TD.DrawText(new Vector2(startOffset.x + 2, startOffset.y + TD.cardHeight / 2), selectItemGroup.itemList.Count.ToString(), 8, FontStyle.Bold, Color.white, HorTextAlign.Center, VerTextAlign.Center);
            //}

            if (selectItemGroup.foldout > 0)
            {
                if (selectItemGroup.itemList.Count > 1)
                {
                    bool isCulled = false;
                    pos.x -= TD.texCardBody.width;
                    TD.DrawNode(selectItemGroup, pos, color, Color.white, ref isCulled, activeMulti, nodeFoldout, false, false);
                }

                if (selectItemGroup.foldout == 2)
                {

                    TC_SelectItem selectItem;
                    for (int i = selectItemGroup.itemList.Count - 1; i >= 0; --i)
                    {
                        selectItem = selectItemGroup.itemList[i];
                        //if (index < 0)
                        //{
                        //    startOffset.x -= 5;
                        //    if (TC.filterGroups[-(index + 1)].Draw(ref startOffset, color, colorFilter, -(index + 1)) == 1) LeftClickMenu("Erase FilterGroup");
                        //}
                        if (selectItem != null)
                        {
                            pos.x -= TD.nodeWidthHSpace;
                            TC_SelectItemGUI.Draw(selectItemGroup, activeMulti, i, nodeFoldout, ref pos, colSelectItem);
                        }
                    }
                }
            }

            if (nodeFoldout)
            {
                int mouseClick = TD.DrawNodeCount(selectItemGroup, GroupType.SelectItemGroup, ref pos, selectItemGroup.itemList.Count, nodeFoldout, ref selectItemGroup.foldout, (selectItemGroup.foldout == 0 ? colBracket : (color * 0.75f)) * activeMulti);
                if (mouseClick == 0 && selectItemGroup.itemList.Count == 0) selectItemGroup.Add<TC_SelectItem>("", false, false, true);
            }

            TD.DrawBracket(ref pos, nodeFoldout, false, colBracket * activeMulti, ref selectItemGroup.foldout, true, selectItemGroup.itemList.Count > 0);

            return 0;
        }
    }
}