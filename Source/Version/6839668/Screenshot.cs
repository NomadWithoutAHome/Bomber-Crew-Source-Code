using System;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F9))
		{
			string filename = "BomberCrew " + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss-fff") + ".png";
			Application.CaptureScreenshot(filename);
		}
	}
}
