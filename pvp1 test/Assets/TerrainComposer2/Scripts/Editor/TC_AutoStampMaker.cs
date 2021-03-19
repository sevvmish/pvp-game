using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace TerrainComposer2
{
    class TC_AutoStampMaker : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            string path;

            for (int i = 0; i < importedAssets.Length; i++)
            {
                path = importedAssets[i];
                
                if (path.Contains("/RawFiles/") )
                {
                    string extension = path.Substring(path.Length - 3);
                    if (extension.Contains("raw") || extension.Contains("Raw") || extension.Contains("r16") || extension.Contains("R16")) ConvertToImage(path);
                }
            }
        }

        static void ConvertToImage(string path)
        {
            byte[] newBytes;

            string newPath = TC.GetApplicationPath() + path;
            byte[] bytes = File.ReadAllBytes(newPath);

            if (path.Contains("/Resources/") && (path.Contains(".raw") || path.Contains(".Raw") || path.Contains(".r16") || path.Contains("R16")))
            {
                File.Move(newPath, newPath.Remove(newPath.Length - 3) + "bytes");
                AssetDatabase.Refresh();
            }

            // Debug.Log(bytes.Length);

            int resolution = (int)Mathf.Sqrt(bytes.Length / 2);

            int newResolution = resolution;
            if (newResolution > 512) newResolution = 512;
            
            Texture2D tex = new Texture2D(newResolution, newResolution, TextureFormat.RGB24, false);
            newBytes = new byte[newResolution * newResolution * 3];

            int index;

            float resConversion = (float)resolution / (float)newResolution;
            
            for (int y = 0; y < newResolution; y++)
            {
                for (int x = 0; x < newResolution; x++)
                {
                    int i = (Mathf.RoundToInt(x * resConversion)) + (Mathf.RoundToInt(y * resConversion) * resolution);

                    float v = Mathf.Round(((bytes[i * 2] + (bytes[(i * 2) + 1] * 255)) / 65535f) * 255f);
                    index = (x + (newResolution - y - 1) * newResolution) * 3;
                    newBytes[index] = (byte)v;
                    newBytes[index + 1] = newBytes[index];
                    newBytes[index + 2] = newBytes[index];
                }
            }
            
            tex.LoadRawTextureData(newBytes);
            
            index = newPath.LastIndexOf("/");

            string file = newPath.Substring(index + 1);
            file = file.Remove(file.Length - 3);
            file += "Jpg";

            newPath = newPath.Substring(0, index + 1);
            
            newPath = newPath.Replace("RawFiles/", "") + file;
            File.WriteAllBytes(newPath, tex.EncodeToJPG());
            
            newPath = newPath.Replace(Application.dataPath, "Assets");
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(newPath);
        }
    }
}