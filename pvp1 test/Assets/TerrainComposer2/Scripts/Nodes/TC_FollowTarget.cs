using UnityEngine;
using System.Collections;

namespace TerrainComposer2
{
    [ExecuteInEditMode]
    public class TC_FollowTarget : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset;
        public bool refresh = false;
        public bool followPosition = true;
        public bool followRotation = true;
        public bool followScale = true;
        public bool followScaleY = true;

        Transform t;
        TC_ItemBehaviour targetItem;
        TC_ItemBehaviour item;

        void Start()
        {
            t = transform;
            if (target != null) targetItem = target.GetComponent<TC_ItemBehaviour>();
            item = GetComponent<TC_ItemBehaviour>();
        }

        #if UNITY_EDITOR
        void OnEnable()
        {
            t = transform;
            if (target != null) targetItem = target.GetComponent<TC_ItemBehaviour>();
            item = GetComponent<TC_ItemBehaviour>();
            UnityEditor.EditorApplication.update += Update;
        }

        void OnDisable()
        {
            UnityEditor.EditorApplication.update -= Update;
        }
        #endif


        void Update()
        {
            if (target == null) return;

            if (followPosition) t.position = target.position + offset;
            if (followRotation) t.rotation = target.rotation;
            if (followScale)
            {
                float scaleY;
                if (followScaleY) scaleY = t.localScale.y; else scaleY = target.lossyScale.y;
                t.localScale = new Vector3(target.lossyScale.x, scaleY, target.lossyScale.z);

            }

            if (targetItem != null && item != null)
            {
                if (item.visible != targetItem.visible)
                {
                    item.visible = targetItem.visible;
                    TC.RefreshOutputReferences(item.outputId);
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
