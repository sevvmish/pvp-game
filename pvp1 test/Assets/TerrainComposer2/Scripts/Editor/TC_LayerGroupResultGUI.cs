using UnityEngine;
using System.Collections;

namespace TerrainComposer2
{
    public class TC_LayerGroupResultGUI
    {
        static public void Draw(TC_LayerGroup layerGroup, ref Vector2 pos, float posOldX, float activeMulti, bool nodeFoldout)
        {
            TC_GlobalSettings g = TC_Settings.instance.global;

            TC_LayerGroupResult groupResult = layerGroup.groupResult;
            
            float x1 = pos.x - 78;
            if (groupResult.foldout < 2) x1 += nodeFoldout ? 126 : 135;
            x1 -= TD.texCardBody.width / 2;
            float y1 = pos.y + (layerGroup.nodeFoldout ? TD.cardHeight : 32);
            TD.DrawTextureScaled(x1 - (TD.texShelfLinesConnectUp.width / 2), y1 - 16, TD.texShelfLinesConnectUp, Color.white);
            TD.DrawTextureScaled(x1 - (TD.texShelfLinesConnectUp.width / 2), y1 - 16, TD.texLineConnectUp, g.colLayer * activeMulti);
            x1 += 30;

            if (layerGroup.foldout == 2 && groupResult.itemList.Count > 0)
            {
                TD.DrawTextureScaled(x1, y1 + 16, posOldX - x1 - g.layerHSpace, TD.texShelfLinesHorizontal, Color.white);
                TD.DrawTextureScaled(x1, y1 + 16, posOldX - x1 - g.layerHSpace, TD.texLineHorizontal, g.colLayer * activeMulti);
            }
            else
            {
                Vector2 posOld = pos;
                pos.x = x1 + 52;
                pos.y += layerGroup.nodeFoldout ? 258 : -94;
                int mouseClick = TD.DrawNodeCount(groupResult, GroupType.LayerGroupResult, ref pos, groupResult.itemList.Count, true, ref layerGroup.foldout, g.colLayer * activeMulti, g.rect.width);
                if (groupResult.itemList.Count == 0)
                {
                    if (mouseClick == 0) groupResult.Add<TC_Layer>("", false);
                    else if (mouseClick == 1) groupResult.Add<TC_LayerGroup>("", false);
                }
                else
                {
                    if (mouseClick == 0) layerGroup.nodeFoldout = true;
                }
                
                pos = posOld;
            }
            
            TD.DrawBracket(ref pos, nodeFoldout, true, g.colLayerGroup * activeMulti, ref groupResult.foldout, true, true);
            if (groupResult.foldout == 2)
            {
                bool isCulled = false;
                pos.x -= TD.texCardBody.width;
                TD.DrawNode(groupResult, pos, g.colLayer, Color.white, ref isCulled, activeMulti, nodeFoldout, false, true);

                int mouseButton = TD.Button(new Rect(pos.x + 245.1f, pos.y + 6.5f, 20, 20), TD.texFoldout, string.Empty, true, new Color(1, 1, 1, 0.25f), Color.white, Color.white, true);
                if (mouseButton == 0)
                {
                    if (layerGroup.foldout == 0) layerGroup.foldout = 2; else layerGroup.foldout = 0;
                }
            }
            else
            {
                if (nodeFoldout) TD.DrawNodeCount(groupResult, GroupType.LayerGroupResult, ref pos, 1, true, ref groupResult.foldout, g.colLayerGroup * activeMulti, 1);
            }
            TD.DrawBracket(ref pos, nodeFoldout, false, g.colLayerGroup * activeMulti, ref groupResult.foldout, true, true);
        }
    }
}
