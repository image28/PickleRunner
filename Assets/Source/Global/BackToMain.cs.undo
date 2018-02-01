using UnityEngine;
using System.Collections;

public class BackToMain : Photon.MonoBehaviour {

	void OnClick()
	{
		if ( PhotonNetwork.isMasterClient )
		{
			PhotonNetwork.Disconnect();	
		}
		
		Application.LoadLevel("MainMenu");
	}
}
