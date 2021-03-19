using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace TerrainComposer2
{
    static class DrawCommand
    {
        static List<TextureCommand> drawTextureList0 = new List<TextureCommand>();
        static List<TextureCommand> drawTextureList1 = new List<TextureCommand>();
        static List<TextureCommand> drawTextureList2 = new List<TextureCommand>();
        static List<TextCommand> drawTextList = new List<TextCommand>();

        static public void Add(Rect rect, Texture tex, string tooltip, Color color, int queue = 0)
        {
            var textureCommand = new TextureCommand(rect, tex, tooltip, color);
            
            if (queue == 0) drawTextureList0.Add(textureCommand);
            else if (queue == 1) drawTextureList1.Add(textureCommand);
            else if (queue == 2) drawTextureList2.Add(textureCommand);
        }
        static public void Add(Vector2 pos, Texture tex, string tooltip, Color color, int queue = 0)
        {
            var textureCommand = new TextureCommand(pos, tex, tooltip, color);

            if (queue == 0) drawTextureList0.Add(textureCommand);
            else if (queue == 1) drawTextureList1.Add(textureCommand);
            else if (queue == 2) drawTextureList2.Add(textureCommand);
        }

        static public void Add(Vector2 pos, string text, string tooltip, int fontSize, Color color, FontStyle fontStyle = FontStyle.Normal, HorTextAlign horTextAlign = HorTextAlign.Left, VerTextAlign vertTextAlign = VerTextAlign.Top)
        {
            drawTextList.Add(new TextCommand(pos, text, tooltip, fontSize, color, fontStyle, horTextAlign, vertTextAlign));
        }
        static public void Add(Vector2 pos, int space, string prefixText, string text, string tooltip, int fontSize, Color color, FontStyle fontStyle = FontStyle.Normal, HorTextAlign horTextAlign = HorTextAlign.Left, VerTextAlign vertTextAlign = VerTextAlign.Top)
        {
            drawTextList.Add(new TextCommand(pos, prefixText, tooltip, fontSize, color, fontStyle, horTextAlign, vertTextAlign));
            drawTextList.Add(new TextCommand(pos + new Vector2(space, 0), text, tooltip, fontSize, color, fontStyle, horTextAlign, vertTextAlign));
        }

        static public void DrawTextureList(List<TextureCommand> drawTextureList)
        {
            for (int i = 0; i < drawTextureList.Count; i++)
            {
                TextureCommand textureCommand = drawTextureList[i];
                TD.DrawTextureScaled(textureCommand.rect, textureCommand.tex, textureCommand.tooltip, textureCommand.color);
            }

            drawTextureList.Clear();
        }

        static public void DrawCommandLists()
        {
            DrawTextureList(drawTextureList0);
            DrawTextureList(drawTextureList1);
            DrawTextureList(drawTextureList2);

            for (int i = 0; i < drawTextList.Count; i++) drawTextList[i].Draw();
            GUI.color = Color.white;

            drawTextList.Clear();
        }

        public class TextureCommand
        {
            public Rect rect;
            public Texture tex;
            public string tooltip;
            public Color color;

            public TextureCommand(Vector2 pos, Texture tex, string tooltip, Color color)
            {
                rect = new Rect(pos.x, pos.y, tex.width, tex.height);
                this.tex = tex; this.tooltip = tooltip; this.color = color;
            }

            public TextureCommand(Rect rect, Texture tex, string tooltip, Color color)
            {
                this.rect = rect;
                this.tex = tex; this.tooltip = tooltip; this.color = color;
            }
        }

        public class TextCommand
        {
            public Vector2 pos;
            public string text;
            public string tooltip;
            public int fontSize;
            public Color color;
            public FontStyle fontStyle;
            public HorTextAlign horTextAlign;
            public VerTextAlign vertTextAlign;

            public TextCommand(Vector2 pos, string text, string tooltip, int fontSize, Color color, FontStyle fontStyle = FontStyle.Normal, HorTextAlign horTextAlign = HorTextAlign.Left, VerTextAlign vertTextAlign = VerTextAlign.Top)
            {
                this.pos = pos; this.text = text; this.tooltip = tooltip; this.color = color; this.fontSize = fontSize; this.fontStyle = fontStyle; this.horTextAlign = horTextAlign; this.vertTextAlign = vertTextAlign;
            }

            public void Draw()
            {
                TD.DrawText(pos, text, tooltip, fontSize, color, fontStyle, horTextAlign, vertTextAlign);
            }
        }
    }

    static public class TC_LayerGroupGUI
    {
        static public Rect DrawLayerOrLayerGroup(TC_ItemBehaviour item, ref Vector2 pos, Color color, ref bool isCulled, float activeMulti, bool drawMethod, bool isFirst, bool isLast) 
        {
            TC_GlobalSettings g = TC_Settings.instance.global; // TODO: Make global static and init
            TC_LayerGroup layerGroup = item as TC_LayerGroup;
            TC_Layer layer = item as TC_Layer;
            bool nodeFoldout = (layerGroup != null) ? layerGroup.nodeFoldout : layer.nodeFoldout;

            if (isFirst)
            {
                pos.y += TD.shelfOffsetY;
                TD.DrawTextureScaled(pos.x, pos.y, TD.texShelfLinesConnectDown, Color.white);
                TD.DrawTextureScaled(pos.x, pos.y, TD.texLineConnectDown, g.colLayer * activeMulti);
                pos.y += TD.texShelfLinesConnectDown.height;
            }
            else if (item.level == 0)
            {
                TD.DrawTextureScaled(pos.x + 64, pos.y, TD.texShelfStartConnect, Color.white);
            }
            else pos.y += 15;

            Texture texShelf;
            Texture texLine = null;
            
            if (item.level == 0)
            {
                if (nodeFoldout) texShelf = TD.texShelfStartOutput; else texShelf = TD.texShelfStartOutputCollapsed;
            }
            else if (isLast)
            {
                if (nodeFoldout) { texShelf = TD.texShelfLayerStart2; texLine = TD.texLineLayerStart2; }
                else { texShelf = TD.texShelfLayerCollapsedStart2; texLine = TD.texLineLayerStart2; }
            }
            else
            {
                if (nodeFoldout) { texShelf = TD.texShelfLayerStart1; texLine = TD.texLineLayerStart1; }
                else { texShelf = TD.texShelfLayerCollapsedStart1; texLine = TD.texLineLayerCollapsedStart1; }
            }

            TD.DrawTextureScaled(pos.x, pos.y, texShelf, Color.white);
            if (item.level > 0) TD.DrawTextureScaled(pos.x, pos.y, texLine, g.colLayer * activeMulti);
            
            TD.DrawTextureScaled(pos.x - TD.texShelfStartOutput.width, pos.y, nodeFoldout ? TD.texShelfLayer : TD.texShelfLayerCollapsed, Color.white, false, StretchMode.Left);
            
            pos.y += TD.shelfOffsetY;
            pos.x -= TD.texCardBody.width - (item.level == 0 ? 20 : 34);

            isCulled = false;
            
            Rect nodeRect = TD.DrawNode(item, pos, color, Color.white, ref isCulled, activeMulti, nodeFoldout, drawMethod);
            if (isCulled) return nodeRect;

            int mouseButton = TD.Button(new Rect(pos.x + 245.1f, pos.y + 6.5f, 20, 20), TD.texFoldout, string.Empty, true, new Color(1, 1, 1, 0.25f), Color.white, Color.white, true);
            if (mouseButton >= 0)
            {
                if (layerGroup != null)
                {
                    layerGroup.nodeFoldout = !layerGroup.nodeFoldout;
                    if (mouseButton == 0) layerGroup.foldout = layerGroup.nodeFoldout ? 2 : 0;
                }
                else layer.nodeFoldout = !layer.nodeFoldout;

                Event.current.Use();
            }
            
            //if (GUI.Button(TD.GetRectScaled(pos.x + 225, pos.y + 2, 40, 25), ""))
            //{
            //    if (layerGroup != null) layerGroup.nodeFoldout = !layerGroup.nodeFoldout;
            //    else layer.nodeFoldout = !layer.nodeFoldout;
            //}
            
            if (item.method == Method.Lerp && drawMethod)
            {
                // TD.DrawSlider(startOffset, ref item.overlay, 0, 1);
                // if (GUI.changed) TC_Generate.singleton.AutoGenerate();
            }

            if (item.level > 0)
            {
                DrawConnectionIndicator(item, new Vector2(pos.x + 289, pos.y - 27), false, true);
                if (isLast)
                {
                    DrawConnectionIndicator(item, new Vector2(pos.x + 289, pos.y + TD.cardHeight + 4), true, nodeFoldout);
                }
            }
           
            return nodeRect;
        }

        static void DrawConnectionIndicator(TC_ItemBehaviour item, Vector2 pos, bool addBefore, bool nodeFoldout)
        {
            if (!nodeFoldout) pos.y -= 351;

            //if (TD.startDrag)
            //{
            //    Rect dropRect = TD.GetRectScaled(new Rect(pos.x, pos.y, TD.texConnectionIndicator.width, TD.texConnectionIndicator.height));
            //    // TD.DragDropNode(item, dropRect);
            //    DrawTextureCommand.Add(pos, TD.texConnectionIndicator, Color.white);
            //}
            //else
            //{
            TC_GlobalSettings g = TC_Settings.instance.global;

            pos -= new Vector2(2.5f, 2.5f);
            Rect rect = new Rect(pos.x + g.rect4.x, pos.y + g.rect4.y, g.rect4.width, g.rect4.width);
            // DrawTextureCommand.Add(rect, TD.texCardCounter, TD.g.colLayer * 0.25f);
            int clickedButton = TD.Button(rect, TD.texAddFirstCard, "Left click to add a new Layer\n\nRight click to add a new Layer Group", true, new Color(1, 1, 1, 0.25f), Color.white, Color.white, true, true);

            // tooltip
            //Vector2 mousePosition = Event.current.mousePosition;

            //if (rectScaled.Contains(mousePosition))
            //{
            //    DrawCommand.Add(mousePosition + new Vector2(9, 19), 65, "Left Click", "-> Add a new Layer", Color.white);
            //    DrawCommand.Add(mousePosition + new Vector2(9, 19 + TD.labelHeight), 65, "Right Click","-> Add a new Layer Group", Color.white);
            //    TD.repaintNodeWindow = true;
            //}

            if (clickedButton == 0) Undo.RegisterCreatedObjectUndo(item.Add<TC_Layer>("", true, addBefore, true).gameObject, "Created Layer"); 
            else if (clickedButton == 1) Undo.RegisterCreatedObjectUndo(item.Add<TC_LayerGroup>("", true, addBefore, true).gameObject, "Created GameObject");
            //}
        }

        static public void Draw(TC_LayerGroup layerGroup, bool drawMethod, ref Vector2 pos, float activeMulti, bool isFirst, bool isLast, ref float shelfLineVerticalStartY)
        {
            TC_GlobalSettings g = TC_Settings.instance.global;

            if (layerGroup.level == 0) pos.x -= TD.texShelfStartOutput.width;

            float posOldX = pos.x;
            
            bool isCulled = false;
            layerGroup.nodePos = pos - new Vector2(0, TD.cardHeight);
            Rect rect = DrawLayerOrLayerGroup(layerGroup, ref pos, g.colLayerGroup, ref isCulled, activeMulti, drawMethod, isFirst, isLast);
            activeMulti = layerGroup.active ? activeMulti : activeMulti * 0.75f;

            // Rect rectFoldout = TD.GetRectScaled(pos.x + 240, pos.y + 43 + (layerGroup.nodeFoldout ? TD.cardHeight : 32), 20, 20);
            // if (GUI.Button(rectFoldout, "")) layerGroup.foldout = !layerGroup.foldout;

            shelfLineVerticalStartY = pos.y + (layerGroup.nodeFoldout ? TD.texShelfLayerStart1.height : TD.texShelfLayerCollapsed.height) - TD.shelfOffsetY;
            
            DropDownMenu(rect, layerGroup);

            Vector2 bar2 = pos;
            bar2.x -= TD.texCardBody.width * 1.5f;
            bar2.y += TD.cardHeight;

            TC_NodeGroupGUI.Draw(layerGroup.maskNodeGroup, ref pos, g.colMaskNodeGroup, g.colMaskNode, g.colLayerGroup, activeMulti, layerGroup.nodeFoldout, false, false, false);
            // if (startOffsetXMax > startOffset.x) startOffsetXMax = startOffset.x;

            // Draw Result Node
            TC_LayerGroupResultGUI.Draw(layerGroup, ref pos, posOldX, activeMulti, layerGroup.nodeFoldout);
                 
            int layerGroupCount = 0;
            int layerCount = 0;

            float lineOffsetX;
            if (layerGroup.maskNodeGroup.itemList.Count > 0) { lineOffsetX = 2.5f; } else lineOffsetX = 0;

            bool m_isFirst, m_isLast;
            float m_shelfLineVerticalStartY = 0;

            pos.y += layerGroup.nodeFoldout ? (TD.cardHeight) : 32;

            if (layerGroup.foldout == 2)
            {
                for (int i = layerGroup.groupResult.itemList.Count - 1; i >= 0; --i)
                {
                    pos.x = posOldX;// - lineOffsetX ;
                    pos.x -= g.layerHSpace;

                    TC_LayerGroup layerGroupChild = layerGroup.groupResult.itemList[i] as TC_LayerGroup;

                    m_isLast = (i == 0);
                    m_isFirst = (i == layerGroup.groupResult.itemList.Count - 1);

                    if (layerGroupChild != null)
                    {
                        Draw(layerGroupChild, i != 0, ref pos, activeMulti, m_isFirst, m_isLast, ref m_shelfLineVerticalStartY);
                        if (!m_isLast)
                        {
                            pos.y += layerGroupChild.nodeFoldout ? 0 : 32;
                            
                            TD.DrawTextureScaledV(pos.x + 64, m_shelfLineVerticalStartY, (pos.y - m_shelfLineVerticalStartY) + 16, TD.texShelfLinesVertical, Color.white);
                            TD.DrawTextureScaledV(pos.x + 64, m_shelfLineVerticalStartY, (pos.y - m_shelfLineVerticalStartY) + 16, TD.texLineVertical, g.colLayer * activeMulti);
                        }

                        ++layerGroupCount;
                    }
                    else
                    {
                        TC_Layer layer = layerGroup.groupResult.itemList[i] as TC_Layer;
                        if (layer == null) continue;

                        TC_LayerGUI.Draw(layer, ref pos, activeMulti, i != 0, m_isFirst, m_isLast);
                        pos.y += layer.nodeFoldout ? TD.cardHeight : 32;
                        ++layerCount;
                    }
                }
            }
            
            pos.y += 64;
            
            Rect clickRect = TD.GetRectScaled(new Rect(bar2.x + (TD.texCardBody.width * 1.5f) - (2.5f + lineOffsetX), bar2.y - 5f, 10, 10));
            if (TD.ClickRect(clickRect, 0))
            {
                if (layerGroup.foldout == 0) layerGroup.foldout = 2; else layerGroup.foldout = 0;
            }
            
            pos.x = posOldX;
        }
        
        static void DropDownMenu(Rect rect, TC_LayerGroup layerGroup)
        {
            if (TD.ClickRect(rect) != 1) return;

            GenericMenu menu = new GenericMenu();

            // menu.AddItem(new GUIContent("Add Layer"), false, LeftClickMenu, "Add Layer");
            string instanceID = layerGroup.GetInstanceID().ToString();

            //if (layerGroup.level > 1)
            //{
            //    menu.AddSeparator("");
            //    menu.AddItem(new GUIContent("Add Layer"), false, LeftClickMenu, instanceID + ":Add Layer");
            //    menu.AddItem(new GUIContent("Add Layer Group"), false, LeftClickMenu, instanceID + ":Add LayerGroup");
            //    menu.AddItem(new GUIContent("Duplicate Layer Group"), false, LeftClickMenu, instanceID + ":Duplicate LayerGroup");
            //    menu.AddSeparator("");
            //}
            if (layerGroup.groupResult.itemList.Count > 0)
            {
                //if (layerGroup.level == 0 && layerGroup.outputId == TC.heightOutput)
                //{
                //    menu.AddItem(new GUIContent("Export Heightmap"), false, LeftClickMenu, instanceID + ":Export Heightmap");
                //    menu.AddSeparator("");
                //}
                //else if (layerGroup.level == 0 && layerGroup.outputId == TC.colorOutput)
                //{
                //    menu.AddItem(new GUIContent("Export Colormap"), false, LeftClickMenu, instanceID + ":Export Colormap");
                //    menu.AddSeparator("");
                //}
                
                menu.AddItem(new GUIContent("Clear Layer Group"), false, LeftClickMenu, instanceID + ":Clear LayerGroup");
            }
            // if (layerGroup.level > 1) menu.AddItem(new GUIContent("Erase Layer Group"), false, LeftClickMenu, instanceID + ":Erase LayerGroup");

            menu.ShowAsContext();
        }

        static public void LeftClickMenu(object obj)
        {
            int instanceID;
            string command = TD.ObjectToCommandAndInstanceID(obj, out instanceID);
            
            TC_LayerGroup layerGroup = EditorUtility.InstanceIDToObject(instanceID) as TC_LayerGroup;

            if (layerGroup != null)
            {
                if (command == "Add Mask")
                {
                    layerGroup.maskNodeGroup.Add<TC_Node>("", false, false, true, 0);
                }
                else if (command == "Add Layer Inside") layerGroup.groupResult.Add<TC_Layer>("", false, true);
                else if (command == "Add LayerGroup Inside") layerGroup.groupResult.Add<TC_LayerGroup>("", false, false, true);
                else if (command == "Add Layer") layerGroup.groupResult.Add<TC_Layer>("", true, true);
                else if (command == "Add LayerGroup") layerGroup.groupResult.Add<TC_LayerGroup>("", true, false, true);
                else if (command == "Duplicate LayerGroup") layerGroup.Duplicate(layerGroup.t.parent);
                else if (command == "Clear LayerGroup") layerGroup.groupResult.Clear(true);
                else if (command == "Erase LayerGroup") layerGroup.DestroyMe(true);
            }
        }
    }
}