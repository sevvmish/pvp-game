using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class TC_AnimateTransform : MonoBehaviour {

    public bool animate = true;
    public float rotSpeed;
    public Vector3 moveSpeed;

    public float scaleSpeed, scaleAmplitude, scaleOffset;
    
    Vector3 posOld;
    float time;

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
     
    void Update()
    {
        MyUpdate();
    }

	void MyUpdate ()
    {
        // if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.A)) animate = !animate;
       
        if (!animate) return;

        float deltaTime = Time.realtimeSinceStartup - time;
        transform.Rotate(0, rotSpeed * deltaTime, 0);
        transform.Translate(moveSpeed * deltaTime * 90);

        float sp = (Mathf.Sin(Time.realtimeSinceStartup * scaleSpeed) * scaleAmplitude) + scaleOffset;
        transform.localScale = new Vector3(sp, sp, sp);
        
        time = Time.realtimeSinceStartup;
	}

    void OnDrawGizmos()
    {
        // Gizmos.Lab
        Event eventCurrent = Event.current;
        // if (eventCurrent.keyCode == KeyCode.Space) Debug.Log(eventCurrent);
        if (eventCurrent.shift && eventCurrent.type == EventType.KeyUp) animate = !animate;
    }
}
