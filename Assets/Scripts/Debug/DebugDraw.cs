using UnityEngine;
using System.Collections;

public class DebugDraw : MonoBehaviour {
	
	public bool debuggingEnabled = false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		if(debuggingEnabled)
		{
			GUI.color = Color.black;
			GUI.Label(new Rect(0, 0, Screen.width, Screen.height), DebugOut.Instance.getOutputString());
		}
	}
}
