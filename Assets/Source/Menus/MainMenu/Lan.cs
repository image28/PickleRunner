using UnityEngine;
using System.Collections;

public class Lan : MonoBehaviour {

	void OnClick()
	{
		if ( Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork )
		{
				// LAN NOT AVAILABLE
		}else{
			MainMenu.gameMode=1;
			Application.LoadLevel("10 - Lobby");
		}
	}
}
