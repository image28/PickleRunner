using UnityEngine;
using System.Collections;

public class DayNightCycle : MonoBehaviour {

	public skyBoxManager manager;
	private float lastTime=0;
	[Range(60.0f,1000.0f)]
	public float dayDurration=240.0f;
	// Use this for initialization
	void Start () {
		lastTime=Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if ( Time.time > lastTime+dayDurration )
		{
			manager.night=!manager.night;
			lastTime=Time.time;
		}
	}
}
