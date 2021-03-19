using UnityEngine;
using System.Collections;

namespace TerrainComposer2
{
    public class TC_Image : MonoBehaviour
    {
        public RenderTexture rt;
        public int referenceCount;

        public bool isDestroyed, callDestroy;

        void Awake()
        {
            if (isDestroyed)
            {
                // LoadRawImage(fullPath);
                TC_Settings.instance.imageList.Add(this);
            }
            if (!callDestroy) { TC.RefreshOutputReferences(TC.allOutput); referenceCount = 0; }
            else callDestroy = false;
        }

        void OnDestroy()
        {
            if (!callDestroy) TC.RefreshOutputReferences(TC.allOutput);
        }

        void DestroyMe()
        {
            TC_Settings settings = TC_Settings.instance;
            if (settings == null) return;
            if (settings.imageList == null) return;

            int index = settings.imageList.IndexOf(this);
            if (index != -1) settings.imageList.RemoveAt(index);

            TC_Compute.DisposeRenderTexture(ref rt);

            #if UNITY_EDITOR
            UnityEditor.Undo.DestroyObjectImmediate(gameObject);
            #else
            Destroy(gameObject);
            #endif
        }

        public void UnregisterReference()
        {
            --referenceCount;
            if (referenceCount > 0) return;
            isDestroyed = true;
            callDestroy = true;

            DestroyMe();
        }
    }
}