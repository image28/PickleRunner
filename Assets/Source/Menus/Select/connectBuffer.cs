using UnityEngine;
using System.Collections;

public class connectBuffer : Photon.MonoBehaviour {
	[HideInInspector]
	public PhotonPlayer[] remotePlayerArray = new PhotonPlayer[30];
	[HideInInspector]
	public int playerCount=0;
	[HideInInspector]
	public bool gameLoading=false;
	[HideInInspector]
	public bool done=false;
	[HideInInspector]
	public bool ready=false;
	[HideInInspector]
	private bool gotServerName;
	
	
	// Use this for initialization
	void Start () {
		playerCount=0;
		DontDestroyOnLoad(this);
		PhotonNetwork.isMessageQueueRunning=true;
		//PhotonNetwork.SetLevelPrefix(10);
	}

	// Update is called once per frame
	void Update () {
		if ( ( gameLoading ) && ( ! done ) && ( ! PhotonNetwork.isMasterClient ) && ( PhotonNetwork.room != null ) && ( ready ) )
		{
			//Debug.LogError(stringManager.Instance.connectBuffer0);
			photonView.RPC(stringManager.Instance.connectBuffer1, PhotonTargets.MasterClient, PhotonNetwork.player);	
			PhotonNetwork.isMessageQueueRunning=false;
			done=true;
		}
		
		if ( ( ! done ) && ( PhotonNetwork.isMasterClient ) )
		{

			//Debug.LogError(stringManager.Instance.connectBuffer2);
			//Debug.LogError(PhotonNetwork.inRoom + stringManager.Instance.connectBuffer3 + PhotonNetwork.insideLobby);
			photonView.RPC(stringManager.Instance.connectBuffer4,PhotonTargets.OthersBuffered);
			done=true;
		}

	}
	
	[PunRPC]
	void announce()
	{
		Debug.LogError("I'm ready");
		ready=true;
	}
	
	[PunRPC]
	void clientJoin(PhotonPlayer netPlayer) 
	{
		//Debug.LogError("A client has connected to us");
		remotePlayerArray[(netPlayer.ID-2)]=netPlayer;
		playerCount++;
    }

}
