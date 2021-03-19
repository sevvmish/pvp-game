using UnityEngine;
using System.Collections;

namespace TerrainComposer2 {
    [ExecuteInEditMode]
    public class TC_AnimateNode : MonoBehaviour {

        public Vector3 moveSpeed;
        public float rotSpeed;
        public float scaleSpeed;
        public float opacitySpeed;

        TC_ItemBehaviour item;
        bool refresh;

        void Start()
        {
            item = GetComponent<TC_ItemBehaviour>();
        }

        #if UNITY_EDITOR
        void OnEnable()
        {
            UnityEditor.EditorApplication.update += MyUpdate;
        }

        void OnDisable()
        {
            UnityEditor.EditorApplication.update -= MyUpdate;
        }
        #endif


        void MyUpdate() {
            transform.Rotate(0, rotSpeed, 0);
            transform.Translate(moveSpeed * 90);
            transform.localScale += new Vector3(scaleSpeed, scaleSpeed, scaleSpeed);

            if (rotSpeed != 0 || moveSpeed.x != 0 || moveSpeed.y != 0 || moveSpeed.z != 0 || scaleSpeed != 0) refresh = true;

            if (opacitySpeed != 0)
            {
                if (item != null)
                {
                    item.opacity = Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup * opacitySpeed));
                    refresh = true;
                }
            }

            if (refresh)
            {
                TC.repaintNodeWindow = true;
                TC.AutoGenerate();
            }
        }
    }
}
