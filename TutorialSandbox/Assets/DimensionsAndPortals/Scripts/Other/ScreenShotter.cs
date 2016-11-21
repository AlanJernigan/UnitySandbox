using UnityEngine;
using System.Collections;

public class ScreenShotter : MonoBehaviour {
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.F12)) {
	        Application.CaptureScreenshot(System.DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")  + ".png");
	    }
	}
}
