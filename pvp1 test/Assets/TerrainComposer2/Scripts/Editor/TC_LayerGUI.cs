using UnityEngine;
using UnityEditor;
using System.Collections;

namespace TerrainComposer2
{
    // Layer
    static public class TC_LayerGUI
    {
        // public float barX;
        static public void Draw(TC_Layer layer, ref Vector2 startOffset, float activeMulti, bool drawMethod, bool isFirst, bool isLast)
        {
            TC_GlobalSettings g = TC_Settings.instance.global;
            
            bool isCulled = false;
            
            TC_LayerGroupGUI.DrawLayerOrLayerGroup(layer, ref startOffset, g.colLayer, ref isCulled, activeMulti, drawMethod, isFirst, isLast);
            if (!layer.active) activeMulti *= 0.75f;

            // DropDownMenu(rect, layer);

            bool hideSelectNodes = false;

            if (layer.outputId != TC.heightOutput)
            {
                if (layer.selectItemGroup.totalActive <= 1) hideSelectNodes = true;
            }

            TC_NodeGroupGUI.Draw(layer.maskNodeGroup, ref startOffset, g.colMaskNodeGroup, g.colMaskNode, g.colLayer, activeMulti, layer.nodeFoldout, false, false, false);
            if (!hideSelectNodes) TC_NodeGroupGUI.Draw(layer.selectNodeGroup, ref startOffset, g.colSelectNodeGroup, g.colSelectNode, g.colLayer, activeMulti, layer.nodeFoldout, false, layer.outputId != TC.heightOutput, hideSelectNodes);
            if (layer.selectItemGroup != null && layer.outputId != TC.heightOutput)
                TC_SelectItemGroupGUI.Draw(layer.selectItemGroup, ref startOffset, TC_Settings.instance.global.colSelectItemGroup, TC_Settings.instance.global.colSelectItem, g.colLayer, activeMulti);
        }

        static public void DropDownMenu(Rect rect, TC_Layer layer)
        {
            if (TD.ClickRect(rect) != 1) return;

            GenericMenu menu = new GenericMenu();

            // menu.AddItem(new GUIContent("Add Layer"), false, LeftClickMenu, "Add Layer");
            string instanceID = layer.GetInstanceID().ToString();

            if (layer.maskNodeGroup.itemList.Count == 0)
            {
                menu.AddItem(new GUIContent("Add Mask"), false, LeftClickMenu, instanceID + ":Add Mask");
                menu.AddSeparator("");
            }
            
            menu.AddItem(new GUIContent("Add Layer"), false, LeftClickMenu, instanceID + ":Add Layer");
            menu.AddItem(new GUIContent("Duplicate Layer"), false, LeftClickMenu, instanceID + ":Duplicate Layer");
            if (layer.level > 1)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Add Layer Group"), false, LeftClickMenu, instanceID + ":Add LayerGroup");
            }
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Erase Layer"), false, LeftClickMenu, instanceID + ":Erase Layer");
            menu.ShowAsContext();
        }

        static public void LeftClickMenu(object obj)
        {
            int instanceID;
            string command = TD.ObjectToCommandAndInstanceID(obj, out instanceID);

            TC_Layer layer = EditorUtility.InstanceIDToObject(instanceID) as TC_Layer;

            if (layer != null)
            {
                if (command == "Add Mask") layer.maskNodeGroup.Add<TC_Node>("", false, false, true);
                else if (command == "Add Layer") layer.Add<TC_Layer>("", true, false, true);
                else if (command == "Duplicate Layer") layer.Duplicate(layer.t.parent);
                else if (command == "Add LayerGroup") layer.Add<TC_LayerGroup>("", true, false, true);
                else if (command == "Erase Layer")
                {
                    layer.DestroyMe(true);
                }

            }
        }
    }
}