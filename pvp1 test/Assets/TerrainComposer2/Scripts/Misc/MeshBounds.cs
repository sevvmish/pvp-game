using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MeshBounds : MonoBehaviour {

	void Start ()
    {
        MeshFilter mf = GetComponent<MeshFilter>();

        if (mf == null) return;

        Mesh m = mf.sharedMesh;
        if (m == null) return;

        Vector3 size = transform.lossyScale * 10;
        size.y = 4800;
        m.bounds = new Bounds(Vector3.zero, size);
        Debug.Log(name + " new bounds " + size);
	}
}
