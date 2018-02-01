using UnityEngine;
using System.Collections;

public class Wan : MonoBehaviour {

	void OnClick()
	{
		if ( Application.internetReachability == NetworkReachability.NotReachable )
		{
			// Internet is not reachable at all on mobiles, 3g or wifi
		}else{
		
			if ( Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork )
			{
				// Call function that displays warning about slower connection and inablity to create servers over 3g	
			}
			
		}
		
		MainMenu.gameMode=2;
		Application.LoadLevel("Lobby");
	}
}
