using UnityEngine;
using System.Collections;

public class tempNet : Photon.MonoBehaviour {
	
	public bool viewFound=false;
	GameObject preNet;
	PhotonView preNetView;
	bool sent=false;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
		if ( ! preNet )
		{
			preNet = GameObject.Find(stringManager.Instance.tempNet0);
			
			if ( ! preNet )
				return;
			
			Debug.LogError(stringManager.Instance.tempNet1);
			
			preNet.AddComponent<PhotonView>();
			preNetView = preNet.GetComponent<PhotonView>();
			preNetView.enabled=true;
		}
		
		if ( ( PhotonNetwork.isMasterClient ) && ( ! sent ) )
		{
			Debug.LogError(stringManager.Instance.tempNet2);
			int viewID;
			viewID = PhotonNetwork.AllocateViewID();
			photonView.RPC(stringManager.Instance.tempNet3, PhotonTargets.AllBuffered , viewID);
			sent=true;
		}
	}
		
	[PunRPC]
	void allocateID(int viewID)
	{
		while ( ! preNetView )
		{
			if ( preNetView )
			{
				Debug.LogError("allocated the network view");
				preNetView.viewID = viewID;	
				viewFound=true;
			}	
		}
	}
}
