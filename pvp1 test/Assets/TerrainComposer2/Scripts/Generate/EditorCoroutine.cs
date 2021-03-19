using System;
using System.Collections;
using System.Collections.Generic;
// using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TerrainComposer2
{
    public class EditorCoroutine
    {
        public bool pause;

        public static EditorCoroutine Start(IEnumerator _routine)
        {
            EditorCoroutine coroutine = new EditorCoroutine(_routine);
            coroutine.Start();
            return coroutine;
        }

        readonly IEnumerator routine;
        EditorCoroutine(IEnumerator _routine)
        {
            routine = _routine;
        }

        void Start()
        {
            //Debug.Log("start");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update += Update;
#endif
        }
        public void Stop()
        {
            //Debug.Log("stop");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= Update;
#endif
        }

        void Update()
        {
            /* NOTE: no need to try/catch MoveNext,
			 * if an IEnumerator throws its next iteration returns false.
			 * Also, Unity probably catches when calling EditorApplication.update.
			 */

            //Debug.Log("update");
            if (pause) return;

            if (!routine.MoveNext()) Stop();
        }
    }
}