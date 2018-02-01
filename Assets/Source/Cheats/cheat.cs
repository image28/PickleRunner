using UnityEngine;
using System.Collections;

public class cheat : MonoBehaviour {
	
	private GameObject map;
	
	
#if UNITY_STANDALONE
	// Use this for initialization
	void Start () {
		map=GameObject.Find("Minimap");
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(stringManager.Instance.cheat0))
		{
			if(Input.GetKey(stringManager.Instance.cheat1))
			{
				map.SetActive(true);
				//Debug.LogError(stringManager.Instance.cheat2);
			}else{
				map.SetActive(false);
			}
		}else{
			map.SetActive(false);
		}
			
			
	}
#endif
}
