using System;
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace TerrainComposer2
{
    static public class TC_NodeGUI
    {
        static public float input = 0;
        static public float position = 0;
        
        static public void Draw(TC_Node node, bool nodeFoldout, bool drawMethod, Vector2 pos, Color color, float activeMulti)
        {
            // Draw total node
            // startOffset.x -= TD.nodeSpaceWidth;
            // parent.DropDownMenu();
            
            bool isCulled = false;
            Rect rect = TD.DrawNode(node, pos, color, Color.white, ref isCulled, activeMulti, nodeFoldout, drawMethod, false);

            if (nodeFoldout)
            {
                Rect rectButton = new Rect(pos.x + 7.24f, pos.y + 306.09f, 253.92f, 30);

                if (TD.GetRectScaled(rectButton).y > 0)
                {
                    string text = node.GetInputPopup().ToString();

                    TD.DrawTextureScaled(rectButton, TD.texButton, string.Empty, color * activeMulti);
                    TD.DrawText(new Vector2(rectButton.x + 5.02f, rectButton.y + 14.54f), text, string.Empty, 21, Color.white, FontStyle.Normal, HorTextAlign.Left, VerTextAlign.Center);

                    //if (node.inputKind == InputKind.Terrain && node.inputTerrain == InputTerrain.Splatmap)
                    //{
                    //    TD.DrawTextureScaled(new Rect(pos.x + g.rect3.x, pos.y + g.rect3.y, 75, 75), TC_Settings.instance.GetPreviewSplatTexture(node.splatSelectIndex), Color.white * activeMulti * 0.8f);
                    //}
                    DrawAddItem(rect, pos, node);
                    DropDownMenuInput(TD.GetRectScaled(rectButton), node);
                }
                LeftClickMenu(rect, node);
            }
        }

        static public void DropDownMenuInput(Rect rect, TC_Node node)
        {
            if (TD.ClickRect(rect) != 0) return;
            
            GenericMenu menu = new GenericMenu();

            string instanceID = node.GetInstanceID().ToString();

            if (node.outputId == TC.heightOutput)
                AddMenuItem(node, menu, instanceID, node.inputTerrain, 5, true);
            else
                AddMenuItem(node, menu, instanceID, node.inputTerrain, 0);

            AddMenuItem(node, menu, instanceID, node.inputNoise);
            AddMenuItem(node, menu, instanceID, node.inputShape);
            AddMenuItem(node, menu, instanceID, node.inputFile);
            AddMenuItem(node, menu, instanceID, node.inputCurrent);
            menu.AddItem(new GUIContent("Portal"), false, ClickMenuInput, instanceID + ":Portal/Portal");
            
            menu.ShowAsContext();
            Event.current.Use();
        }

        static public void AddMenuItem(TC_Node node, GenericMenu menu, string instanceID, Enum e, int startIndex = 0, bool showOne = false)
        {
            int length;
            if (showOne) length = startIndex + 1; else length = Enum.GetNames(e.GetType()).Length;
            string inputName = e.GetType().ToString().Replace("TerrainComposer2.Input", "");

            for (int i = startIndex; i < length; i++)
            {
                string name = Enum.GetName(e.GetType(), i);
                if ((name == "Convexity" || name == "Collision") && node.outputId != TC.heightOutput) menu.AddSeparator("Terrain/");
                if (name == "IQ" || name == "Random") menu.AddSeparator("Noise/");
                menu.AddItem(new GUIContent(inputName + "/" + name), false, ClickMenuInput, instanceID + ":" + inputName+ "/" + name);
            }
        }

        static public void ClickMenuInput(object obj)
        {
            int instanceID;
            string command = TD.ObjectToCommandAndInstanceID(obj, out instanceID);

            TC_Node node = EditorUtility.InstanceIDToObject(instanceID) as TC_Node;

            if (node != null)
            {
                int index = command.IndexOf("/");
                string inputKind = command.Substring(0, index);
                string input = command.Substring(index + 1);
                bool changed = false;

                InputKind oldInputKind = node.inputKind;
                node.inputKind = (InputKind)Enum.Parse(typeof(InputKind), inputKind);

                if (node.inputKind != oldInputKind) changed = true;
                
                if (inputKind == "Terrain")
                {
                    InputTerrain oldInputTerrain = node.inputTerrain;
                    node.inputTerrain = (InputTerrain)Enum.Parse(typeof(InputTerrain), input);
                    if (node.inputTerrain != oldInputTerrain) changed = true;
                }
                else if (inputKind == "Noise")
                {
                    InputNoise oldInputNoise = node.inputNoise;
                    node.inputNoise = (InputNoise)Enum.Parse(typeof(InputNoise), input);
                    if (node.inputNoise != oldInputNoise) changed = true;
                }
                else if (inputKind == "Shape")
                {
                    InputShape oldInputShape = node.inputShape;
                    node.inputShape = (InputShape)Enum.Parse(typeof(InputShape), input);
                    if (node.inputShape != oldInputShape) changed = true;
                }
                else if (inputKind == "File")
                {
                    InputFile oldInputFile = node.inputFile;
                    node.inputFile = (InputFile)Enum.Parse(typeof(InputFile), input);
                    if (node.inputFile != oldInputFile) changed = true;
                }
                else if (inputKind == "Current")
                {
                    InputCurrent oldInputCurrent = node.inputCurrent;
                    node.inputCurrent = (InputCurrent)Enum.Parse(typeof(InputCurrent), input);
                    if (node.inputCurrent != oldInputCurrent) changed = true;
                }

                if (changed)
                {
                    node.Init();
                    EditorUtility.SetDirty(node);
                    TC.RefreshOutputReferences(node.outputId, true);
                }
            }
        }

        static void LeftClickMethodMenu(object obj)
        {
            //int instanceID;
            //string command = ObjectToCommandAndInstanceID(obj, out instanceID);

            //TC_ItemBehaviour item = EditorUtility.InstanceIDToObject(instanceID) as TC_ItemBehaviour;
            //Method oldMethod = item.method;
            //item.method = (Method)System.Enum.Parse(typeof(Method), command);
            //if (item.method != oldMethod) TC_Generate.instance.AutoGenerate();
        }

        static public void DrawAddItem(Rect rect, Vector2 pos, TC_ItemBehaviour item)
        {
            Event eventCurrent = Event.current;
            if (DragAndDrop.objectReferences.Length > 0) return;

            Vector2 posMouse = eventCurrent.mousePosition;
            
            if (rect.Contains(posMouse))
            {
                if (eventCurrent.alt && !eventCurrent.control)
                {
                    if (posMouse.x < rect.x + ((TD.texCardHeader.width / 2) * TD.scale)) item.dropPosition = DropPosition.Left;
                    else item.dropPosition = DropPosition.Right;

                    if (eventCurrent.type == EventType.MouseDown && eventCurrent.button == 0 && eventCurrent.clickCount == 2)
                    {
                        AddItem(item, eventCurrent.shift);
                        eventCurrent.Use();
                    }

                    TC.repaintNodeWindow = true;
                }
                else item.dropPosition = DropPosition.None;
            }
        }

        static public void LeftClickMenu(Rect rect, TC_ItemBehaviour item)
        {
            if (TD.ClickRect(rect) != 1) return;

            string itemText = "";

            if (item.GetType() == typeof(TC_SelectItem)) itemText = "Item"; else itemText = "Node";

            GenericMenu menu = new GenericMenu();
            
            string instanceID = item.GetInstanceID().ToString();

            if (Event.current.mousePosition.x < rect.x + ((TD.texCardHeader.width / 2) * TD.scale))
            {
                menu.AddItem(new GUIContent("<-- Add " + itemText), false, LeftClickMenu, instanceID + ":Add Left");
                //if (itemText == "Node")
                //{
                //    menu.AddItem(new GUIContent("<-- Add Node Group"), false, LeftClickMenu, instanceID + ":Add NodeGroup Left");
                //    menu.AddSeparator("");
                //}
                menu.AddItem(new GUIContent("<-- Duplicate " + itemText), false, LeftClickMenu, instanceID + ":Duplicate Left");
            }
            else
            {
                menu.AddItem(new GUIContent("--> Add " + itemText), false, LeftClickMenu, instanceID + ":Add Right");
                //if (itemText == "Node")
                //{
                //    menu.AddItem(new GUIContent("--> Add Node Group"), false, LeftClickMenu, instanceID + ":Add NodeGroup Right");
                //    menu.AddSeparator("");
                //}
                menu.AddItem(new GUIContent("--> Duplicate " + itemText), false, LeftClickMenu, instanceID + ":Duplicate Right");
            }

            // bool eraseNodeMenu = true;

            //if (node.nodeType == NodeType.Select)
            //{
            //    TC_NodeGroup nodeGroup = node.t.parent.GetComponent<TC_NodeGroup>();
            //    if (nodeGroup != null)
            //    {
            //        if (nodeGroup.itemList.Count == 1) eraseNodeMenu = false;
            //    }
            //}

            //if (eraseNodeMenu) {
            //    menu.AddSeparator("");
            //    menu.AddItem(new GUIContent("Erase Node"), false, LeftClickMenu, instanceID + ":Erase Node");
            //}

            menu.ShowAsContext();
        }

        static public void LeftClickMenu(object obj)
        {
            int instanceID;
            string command = TD.ObjectToCommandAndInstanceID(obj, out instanceID);

            TC_ItemBehaviour item = EditorUtility.InstanceIDToObject(instanceID) as TC_ItemBehaviour;

            if (item != null)
            {
                // if (command == "Add Node ") node.Add<TC_Node>("", true, false, true);
                // else if (command == "Duplicate Node") node.Duplicate(node.t.parent);
                if (command.Contains("Right")) item.dropPosition = DropPosition.Right;
                else if (command.Contains("Left")) item.dropPosition = DropPosition.Left;

                bool addNodeGroup = command.Contains("NodeGroup");

                bool clone;
                if (command.Contains("Duplicate")) clone = true; else clone = false;

                AddItem(item, clone, addNodeGroup);

                item.dropPosition = DropPosition.None;
                // else if (command == "Erase Node") node.Destroy(true);
            }
        }

        static public TC_ItemBehaviour AddItem(TC_ItemBehaviour item, bool clone, bool addNodeGroup = false)
        {
            TC_ItemBehaviour itemToDrop = null;

            if (!clone)
            {
                if (item.GetType() == typeof(TC_Node))
                {
                    if (addNodeGroup) itemToDrop = (TC_ItemBehaviour)item.Add<TC_NodeGroup>("Node Group", true); else itemToDrop = (TC_ItemBehaviour)item.Add<TC_Node>("", true);
                }
                else if (item.GetType() == typeof(TC_SelectItem))
                {
                    itemToDrop = (TC_ItemBehaviour)item.Add<TC_SelectItem>("", true);

                    TC_SelectItem newSelectItem = (TC_SelectItem)itemToDrop;
                    TC_SelectItem selectItem = (TC_SelectItem)item;

                    if (selectItem.dropPosition == DropPosition.Right) newSelectItem.selectIndex = Mathf.Min(selectItem.selectIndex + 1, Mathf.Max(0, newSelectItem.GetItemTotalFromTerrain() - 1));
                    else newSelectItem.selectIndex = Mathf.Max(selectItem.selectIndex - 1, 0);
                }
                Undo.RegisterCreatedObjectUndo(item.gameObject, "Created " + item.name);
            }
            else itemToDrop = item;

            itemToDrop = TD.DropItemSameLevel(item, itemToDrop, clone, false);

            Selection.activeTransform = itemToDrop.t;

            return itemToDrop;
        }
    }
}