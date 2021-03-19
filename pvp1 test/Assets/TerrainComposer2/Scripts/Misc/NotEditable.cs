using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class NotEditable : MonoBehaviour {

    void OnEnable()
    {
        transform.hideFlags = HideFlags.HideInInspector;
        hideFlags = HideFlags.HideInInspector;
    }

    void OnDrawGizmosSelected()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
}
