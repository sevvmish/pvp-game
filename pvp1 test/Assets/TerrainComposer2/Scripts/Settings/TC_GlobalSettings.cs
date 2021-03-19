using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace TerrainComposer2
{
    [CreateAssetMenu(fileName = "TC_GlobalSettings", menuName = "TerrainComposer2/GlobalSettings")]
    public class TC_GlobalSettings : ScriptableObject
    {
        public bool tooltip;
        public Vector3 defaultTerrainSize = new Vector3(2048, 1000, 2048);

        public bool SavePreviewTextures = true;

        public Color[] previewColors = { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan, Color.white, Color.grey };
        
        public Color colLayerGroup;
        public Color colLayer;
        public Color colMaskNodeGroup;
        public Color colMaskNode;
        public Color colSelectNodeGroup;
        public Color colSelectNode;
        public Color colSelectItemGroup;
        public Color colSelectItem;

        public float shelveHeight = 428;
        public float shelveRightWidth = 18;

        public float outputVSpace;
        public float groupVSpace = 25;

        public float layerVSpace = 50;
        public float layerHSpace = 180;

        public float nodeHSpace = 5;
        public float bracketHSpace = 10;

        public bool showResolutionWarnings = true;
        public bool linkScaleToMaskDefault = true;
        public bool documentationClicked;
        public bool discordClicked;

        public Rect rect, rect2, rect3, rect4, rect5, rect6, rect7, rect8;

        public KeyCode keyZoomIn = KeyCode.Plus;
        public KeyCode keyZoomOut = KeyCode.Minus;
        
        public Color GetVisualizeColor(int index)
        {
            return previewColors[(int)Mathf.Repeat(index, previewColors.Length)];
        }
    }
}


