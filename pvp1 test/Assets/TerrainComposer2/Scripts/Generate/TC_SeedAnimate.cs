using UnityEngine;
using System.Collections;

namespace TerrainComposer2
{
    [ExecuteInEditMode]
    public class TC_SeedAnimate : MonoBehaviour
    {
        public float animateSpeed;
        float time;
        
        void Update()
        {
            MyUpdate();
        }

        void MyUpdate()
        {
            if (TC_Settings.instance == null) return;
            TC_Settings.instance.seed += (Time.realtimeSinceStartup - time) * animateSpeed;
            time = Time.realtimeSinceStartup;
            TC.AutoGenerate();
        }

#if UNITY_EDITOR
        void OnEnable()
        {
            TC.AutoGenerate();
            UnityEditor.EditorApplication.update += MyUpdate;
        }
        
         
        void OnDisable()
        {
            UnityEditor.EditorApplication.update -= MyUpdate;
        }

        void OnDestroy()
        {
            UnityEditor.EditorApplication.update -= MyUpdate;
        }
#endif
    }
}