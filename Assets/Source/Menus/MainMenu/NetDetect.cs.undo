using UnityEngine;
using System.Collections;

public class NetDetect : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if ( Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork )
		{
				gameObject.SetActive(false);
		}
		
	}
}
