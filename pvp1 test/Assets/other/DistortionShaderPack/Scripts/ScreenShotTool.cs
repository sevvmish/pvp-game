using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class ScreenShotTool : MonoBehaviour
	{

		// Mono
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.S))
			{
				ScreenCapture.CaptureScreenshot("4KScreenshot.png");
			}
		}

		void OnGUI()
		{
			//if (GUI.Button(new Rect(10, 10, 150, 40), "4k"))
			//{
			//	Screen.SetResolution(4096, 2160, false);
			//}
			//if (GUI.Button(new Rect(10, 60, 150, 40), "Screenshot"))
			//{
			//	Application.CaptureScreenshot("4KScreenshot.png");
			//}
		}
	}
}