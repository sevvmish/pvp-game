using UnityEngine;
using System.Collections;
using System;

namespace TerrainComposer2
{
    [Serializable]
    public class TexturePreview
    {

        public bool edit;
        public Texture2D tex;
        [NonSerialized]
        public byte[] bytes;
        float x, y;

        public void Init()
        {
            // Debug.Log("Create");
            int resolution = TC_Area2D.instance.previewResolution;

            if (bytes == null) bytes = new byte[resolution * resolution * 4];
            if (tex == null)
            {
                tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
                tex.hideFlags = HideFlags.DontSave;
                tex.name = "texPreview";
            }
        }

        public void Init(int resolution)
        {
            bool create = false;

            if (bytes == null) bytes = new byte[resolution * resolution * 4];

            if (tex == null) create = true;
            else if (tex.width != resolution)
            {
                tex.Resize(resolution, resolution);
                return;
            }

            if (create)
            {
                tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
                tex.hideFlags = HideFlags.DontSave;
                tex.name = "texPreview";
            }
        }

        public void ReCreate(int resolution)
        {
            // Debug.Log("recreate preview tex");
            bytes = new byte[resolution * resolution * 4];
            tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
            tex.hideFlags = HideFlags.DontSave;
            tex.name = "texPreview";
        }

        public void UploadTexture()
        {
            tex.LoadRawTextureData(bytes);
            tex.Apply(false);
        }

        public void SetPixel(float v)
        {
            x = TC_Area2D.instance.previewPos.x;
            y = TC_Area2D.instance.previewPos.y;

            int px = (int)x;
            int py = (int)y;

            if (px > 127 || px < 0) return;
            if (py > 127 || py < 0) return;

            py *= 512;
            px *= 4;

            Color32 color = Color.white * v;
            if (v > 1) color = Color.Lerp(Color.red, new Color(1, 0, 1), Mathw.Clamp01(v - 1));
            else if (v < 0) color = Color.Lerp(Color.cyan, Color.blue, Mathw.Clamp01(v * -1));

            bytes[px + py] = (byte)(color.r);
            bytes[px + py + 1] = (byte)(color.g);
            bytes[px + py + 2] = (byte)(color.b);
            bytes[px + py + 3] = 1;
        }

        public void SetPixelColor(Color color)
        {
            // if (tex == null) Create(128);

            x = TC_Area2D.instance.previewPos.x;
            y = TC_Area2D.instance.previewPos.y;

            int px = (int)x;
            int py = (int)y;

            if (px > 127 || px < 0) return;
            if (py > 127 || py < 0) return;

            py *= 512;
            px *= 4;

            bytes[px + py] = (byte)(color.r * 255);
            bytes[px + py + 1] = (byte)(color.g * 255);
            bytes[px + py + 2] = (byte)(color.b * 255);
            bytes[px + py + 3] = 1;
        }

        public void SetPixelColor(int px, int py, Color color)
        {
            py *= tex.width * 4;
            px *= 4;

            bytes[px + py] = (byte)(color.r * 255);
            bytes[px + py + 1] = (byte)(color.g * 255);
            bytes[px + py + 2] = (byte)(color.b * 255);
            bytes[px + py + 3] = 1;
        }
    }
}