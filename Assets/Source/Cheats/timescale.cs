using UnityEngine;
using System.Collections;

public class timescale : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		Application.targetFrameRate = 60;
		Time.timeScale=3.0f;
	}
	
	// Update is called once per frame
	void Update () {
		/*if ( Input.GetKeyUp(KeyCode.T) )
		{
			if ( Time.timeScale == 3.0f )
			{
				Time.timeScale=1.0f;
			}else{
				Time.timeScale=3.0f;
			}
		}*/
	}
}
