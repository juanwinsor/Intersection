using UnityEngine;
using System.Collections;

public class DebugScreenShot : MonoBehaviour {

	int ssCount = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.P))
		{
			Application.CaptureScreenshot(Application.dataPath + "/../screenshots/" + ssCount.ToString() + ".png");
			ssCount++;
		}
	}
}
