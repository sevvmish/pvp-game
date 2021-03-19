using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TC_PreviewArea : MonoBehaviour {

    [HideInInspector] public Transform t;
    public bool manual;
    public Rect area; 
    public float positionY;
    

    void Awake()
    {
        t = transform;
    }

    void Update()
    {
        area.center = new Vector2(t.position.x, t.position.z);
    }


    void OnDrawGizmos()
    {
        if (!manual) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(area.center.x, positionY, area.center.y), new Vector3(area.width, 500, area.height));
        Gizmos.color = Color.white;
    }
	
	
}
