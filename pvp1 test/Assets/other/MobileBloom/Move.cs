
using UnityEngine;

public class Move : MonoBehaviour {
    public Vector3[] points;
    public Vector3[] orients;
	public float accuracy;
    Vector3 currentPosiition;
    Vector3 targetPos;
    Vector3 targetOr;
    int indexp, indexo;
    private void Start()
    {
        targetPos = points[0];
        targetOr = orients[0];
        indexp = 0;
        indexo = 0;
    }
    void Update () {
        currentPosiition = transform.position;

        if (V3Equal(currentPosiition, targetPos))
        {
            targetPos = points[indexp+1 == points.Length ? 0 : ++indexp];
            targetOr = orients[indexo + 1 == points.Length ? 0 : ++indexo];
        }
        gameObject.transform.position = Vector3.Lerp(currentPosiition,targetPos,0.01f);
        gameObject.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetOr), 0.01f);
    }
    public bool V3Equal(Vector3 a, Vector3 b)
    {
		return Vector3.SqrMagnitude(a - b) < accuracy;
    }
}
