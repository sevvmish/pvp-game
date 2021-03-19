using UnityEngine;
using UnityEditor;
using System.Collections;

namespace TerrainComposer2
{
    static public class TC_TerrainLayerGUI
    {
        static public void Draw(TC_TerrainLayer terrainLayer)
        {
            Vector2 pos = new Vector2(TC_NodeWindow.window.position.width / 2, TC_NodeWindow.window.position.height / 2);

            TD.DrawTextureScaled(pos.x, pos.y, TD.texShelfStart, Color.white, false, StretchMode.None, StretchMode.Screen);
            TD.DrawTextureScaled(pos.x + TD.texShelfStart.width, pos.y, TD.texShelfBackGround2, Color.white, false, StretchMode.Right, StretchMode.Screen);
            
            float scrollY = 0;

            for (int i = 0; i < 6; i++) DrawLayerGroup(terrainLayer, i, ref pos, ref scrollY);

        }
        
        static public void DrawLayerGroup(TC_TerrainLayer terrainLayer, int index, ref Vector2 pos, ref float scrollY)
        {
            TC_GlobalSettings g = TC_Settings.instance.global;
            Event eventCurrent = Event.current;

            TC_LayerGroup layerGroup = terrainLayer.layerGroups[index];
            if (layerGroup == null) return; 

            Vector2 posOld = pos;
            
            float t = 0;
            TC_LayerGroupGUI.Draw(layerGroup, false, ref pos, 1, false, false, ref t);

            Rect rect;
            rect = new Rect(posOld.x + 50, posOld.y - 18, 400, 100);
            TD.DrawTextureScaled(posOld.x - 44, posOld.y, 100, TD.texLineHorizontal, g.colLayerGroup);

            Color buttonColor;
            if (layerGroup.visible)
            {
                if (layerGroup.active) buttonColor = Color.green; else buttonColor = Color.red;
            }
            else buttonColor = Color.white;

            if (TD.DrawButton(rect, TC.outputNames[layerGroup.outputId], 64, true, Color.white, TD.editorSkinMulti == 1 ? buttonColor : new Color(buttonColor.r * 0.4f, buttonColor.g * 0.4f, buttonColor.b * 0.4f, 1)))
            {
                // Debug.Log("Clicked Button");
                TD.ClickOutputButton(layerGroup);
                eventCurrent.Use();
            }

            for (int i = 0; i < 3; i++)
            {
                rect = TD.GetRectScaled(pos.x - 32 + 25, pos.y + 33 + (i * 32), 32, 32); rect.xMin = 0;
                TD.DrawTexture(rect, TD.texSeparatorCenter, string.Empty, new Color(1, 1, 1, 0.5f));
                rect = TD.GetRectScaled(pos.x + 25, pos.y + 33 + (i * 32), 32, 32);
                TD.DrawTexture(rect, TD.texSeparatorRight, string.Empty, new Color(1, 1, 1, 0.5f));
            }

            pos.y += TD.cardHeight;
            pos.y += g.outputVSpace;

            // scrollY += startY - startOffset.y;
            // else GlobalManager.singleton.scrollAdd.y = 0; 
            pos.x = posOld.x;
        }
    }
}
