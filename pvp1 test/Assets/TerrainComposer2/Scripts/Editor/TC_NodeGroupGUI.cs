using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TerrainComposer2
{
    static public class TC_NodeGroupGUI
    {
        static public int Draw(TC_NodeGroup nodeGroup, ref Vector2 pos, Color colGroupNode, Color colNode, Color colBracket, float activeMulti, bool nodeFoldout, bool drawMethod, bool colorPreviewTex, bool hideSelectNodes)
        {
            if (nodeGroup == null) return 0;
            // bool select = false;
            // Draw total node

            Rect dropDownRect;
            float activeMultiOld = activeMulti;
            activeMulti *= nodeGroup.active ? 1 : 0.75f;
            
            bool isCulled = false;
            TC_GlobalSettings g = TC_Settings.instance.global;

            // if ((nodeGroup.foldout == 1 && nodeGroup.itemList.Count == 1) || nodeGroup.itemList.Count == 0) nodeGroup.foldout = 0;
            
            // Closing Bracket
            TD.DrawBracket(ref pos, nodeFoldout, true, colBracket * activeMultiOld, ref nodeGroup.foldout, true, nodeGroup.itemList.Count > 0);
            
            if (nodeGroup.foldout > 0)
            {
                if ((nodeGroup.itemList.Count != 1 || nodeGroup.foldout != 1) && nodeGroup.itemList.Count != 0 && !hideSelectNodes) pos.x -= TD.texCardBody.width;
                
                if (nodeGroup.itemList.Count > 1 && !hideSelectNodes)
                {
                    dropDownRect = TD.DrawNode(nodeGroup, pos, colGroupNode, Color.white, ref isCulled, activeMulti, nodeFoldout, drawMethod, false);
                    if (nodeGroup.foldout == 2) pos.x -= TD.texCardBody.width + g.nodeHSpace;
                    DropDownMenu(dropDownRect, nodeGroup);
                }

                //if (colorPreviewTex)
                //{
                //    startOffset.x -= TD.nodeWidth;
                //    TD.DrawNode(nodeGroup, drawMethod, ref startOffset, true, color, click, Color.white, ref isCulled);
                //    startOffset.x -= TD.nodeWidth;
                //}

                // Draw ()
                //if (nodeGroup.itemList.Count > 1)
                //{
                //    startOffset.x += 10;

                //    Draw ItemList Count
                //    if (!nodeGroup.foldout)
                //    {
                //        TD.DrawRect(new Rect(startOffset.x - 9, (startOffset.y + TD.nodeHeight / 2) - 5, 18, 10), TC_Settings.instance.global.colTextBackGround);
                //        TD.DrawText(new Vector2(startOffset.x + 2, startOffset.y + TD.nodeHeight / 2), nodeGroup.itemList.Count.ToString(), 8, FontStyle.Bold, Color.white, HorTextAlign.Center, VerTextAlign.Center);
                //    }
                //    startOffset.x -= TD.nodeWidthSpace;
                //}
                
                if (nodeGroup.foldout == 2 && !hideSelectNodes)
                {
                    if (nodeGroup.itemList.Count > 1 && nodeFoldout) TD.DrawMethod(nodeGroup, pos + new Vector2(TD.texCardBody.width - 18, 172), true, colNode, activeMulti);

                    for (int i = nodeGroup.itemList.Count - 1; i >= 0; --i)
                    {
                        TC_Node node = nodeGroup.itemList[i] as TC_Node;

                        if (node != null)
                        {
                            TC_NodeGUI.Draw(node, nodeFoldout, i == 0 ? false : true, pos, colNode, activeMulti);
                            if (i != 0) pos.x -= TD.nodeWidthHSpace;
                            if (node.inputKind != InputKind.Current && node.inputKind != InputKind.Portal && i != 0 && nodeFoldout)
                            {
                                TD.DrawMethod(node, pos + new Vector2(TD.texCardBody.width - 18, 172), false, colNode, (node.active ? 1 : 0.5f) * activeMulti);
                            }
                                
                        }
                        else
                        {
                            TC_NodeGroup nodeGroupChild = nodeGroup.itemList[i] as TC_NodeGroup;
                            
                            if (nodeGroupChild != null)
                            {
                                pos.x += TD.nodeWidthHSpace;
                                Draw(nodeGroupChild, ref pos, colGroupNode, colNode, colBracket, activeMulti, nodeFoldout, drawMethod, false, false);
                                if (i != 0)
                                {
                                    pos.x -= TD.nodeWidthHSpace;
                                    TD.DrawMethod(nodeGroupChild, pos + new Vector2(TD.texCardBody.width - 18, 172), false, colNode, (nodeGroupChild.active ? 1 : 0.5f) * activeMulti);
                                }
                            }
                        }
                    }
                }
            }

            if (nodeFoldout)
            {
                int mouseClick = TD.DrawNodeCount(nodeGroup, GroupType.NodeGroup, ref pos, nodeGroup.itemList.Count, nodeFoldout, ref nodeGroup.foldout, (nodeGroup.foldout == 1 && nodeGroup.itemList.Count != 1 ? colGroupNode * 0.75f : colBracket) * activeMulti);
                if (mouseClick == 0 && nodeGroup.itemList.Count == 0) nodeGroup.Add<TC_Node>("", false, false, true);
            }

            // Opening Bracket
            TD.DrawBracket(ref pos, nodeFoldout, false, colBracket * activeMultiOld, ref nodeGroup.foldout, true, nodeGroup.itemList.Count > 0);

            return 0;
        }
        
        static public void DropDownMenu(Rect rect, TC_NodeGroup nodeGroup)
        {
            if (TD.ClickRect(rect) != 1) return;

            GenericMenu menu = new GenericMenu();

            // menu.AddItem(new GUIContent("Add Layer"), false, LeftClickMenu, "Add Layer");
            string instanceID = nodeGroup.GetInstanceID().ToString();

            menu.AddItem(new GUIContent("Clear Nodes"), false, LeftClickMenu, instanceID + ":Clear Nodes");
            
            menu.ShowAsContext();
        }

        static public void LeftClickMenu(object obj)
        {
            int instanceID;
            string command = TD.ObjectToCommandAndInstanceID(obj, out instanceID);

            TC_NodeGroup nodeGroup = EditorUtility.InstanceIDToObject(instanceID) as TC_NodeGroup;

            if (nodeGroup != null)
            {
                if (command == "Clear Nodes")
                {
                    nodeGroup.Clear(true);
                }
            }
        }

    }
}