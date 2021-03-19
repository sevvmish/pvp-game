using UnityEngine;
using System.Collections;

namespace TerrainComposer2
{
    [ExecuteInEditMode]
    public class TC_CamCapture : MonoBehaviour
    {
        public Camera cam;
        public int collisionMask;
        [System.NonSerialized] public Terrain terrain;
        Transform t;
        public CollisionDirection collisionDirection;

        RenderTexture rtCapture;

        void Start()
        {
            t = transform;
            cam = GetComponent<Camera>();
            cam.aspect = 1;
        }

        private void OnDestroy()
        {
            DisposeRTCapture();
        }

        public void Capture(int collisionMask, CollisionDirection collisionDirection, int outputId, Vector2 resolution)
        {
            if (TC_Area2D.instance.currentTerrainArea == null) return;

            bool create = false;
            if (rtCapture == null) create = true;
            else if (rtCapture.width != resolution.x || rtCapture.height != resolution.y)
            {
                TC_Compute.DisposeRenderTexture(ref rtCapture);
                create = true;
            }

            if (create)
            {
                rtCapture = new RenderTexture((int)resolution.x, (int)resolution.y, 16, RenderTextureFormat.Depth, RenderTextureReadWrite.Linear);
                cam.targetTexture = rtCapture;
            }
            
            // Debug.Log("Capture");
            this.collisionMask = collisionMask;
            terrain = TC_Area2D.instance.currentTerrain;
            // this.collisionDirection = collisionDirection;
            cam.cullingMask = collisionMask;
             
            SetCamera(collisionDirection, outputId);

            float oldLodBias = QualitySettings.lodBias;
            QualitySettings.lodBias = Mathf.Infinity;
            
            cam.Render();

            QualitySettings.lodBias = oldLodBias;
        }

        public void DisposeRTCapture()
        {
            cam.targetTexture = null;
            TC_Compute.DisposeRenderTexture(ref rtCapture);
        }

        public void SetCamera(CollisionDirection collisionDirection, int outputId)
        {
            if (t == null) Start();

            if (collisionDirection == CollisionDirection.Up)
            {
                t.position = new Vector3(TC_Area2D.instance.bounds.center.x, -1 + TC_Settings.instance.generateOffset.y, TC_Area2D.instance.bounds.center.z);
                t.rotation = Quaternion.Euler(-90, 0, 0);
            }
            else
            {
                t.position = new Vector3(TC_Area2D.instance.bounds.center.x, TC_Area2D.instance.bounds.center.y + 1 + TC_Settings.instance.generateOffset.y, TC_Area2D.instance.bounds.center.z);
                t.rotation = Quaternion.Euler(90, 0, 0);
            } 
                
            float orthographicSize = TC_Area2D.instance.bounds.extents.x;

            if (outputId == TC.heightOutput) orthographicSize += TC_Area2D.instance.resExpandBorderSize;

            cam.orthographicSize = orthographicSize;

            cam.nearClipPlane = 0;
            cam.farClipPlane = TC_Area2D.instance.currentTerrainArea.terrainSize.y + 1;

            // Debug.Log(t.position);

            // Vector3 size = area.currentTerrain.terrainData.size;
            // t.position = new Vector3(area.area.center.x, -1, area.area.center.y);
        }
    }
}