using UnityEngine;
using System.Collections;

public class controls : MonoBehaviour {
//	private bool wasLocked = false;
	
	// Use this for initialization
	void Start () {
		Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit(); 

	}
}
