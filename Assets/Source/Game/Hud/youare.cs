using UnityEngine;
using System.Collections;

public class youare : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		int ID=PhotonNetwork.player.ID-1;
		this.GetComponent<GUIText>().text=stringManager.Instance.youare0+ID.ToString();
	}
}
