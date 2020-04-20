using UnityEngine;
using System.Collections;

public class ScreenshotsMaker : MonoBehaviour
{
	private static int ind = 0;

	private bool disableGui = false;

	void Update ()
	{
		if(Input.GetKeyUp(KeyCode.Space))
		{
			ScreenCapture.CaptureScreenshot(string.Format("Screenshot{0}_{1}x{2}.png", ind, Screen.width, Screen.height));
			ind++;
		}

	}

}
