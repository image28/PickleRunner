using UnityEngine;
using UnityEngine;
using System.Collections;

public class FindGame : Photon.MonoBehaviour {

	public static string lobbyType;
	private RoomInfo[] roomsList;
	RoomOptions roomOptions;
	private bool ready=false;
	
	void Start()
	{
		Debug.LogError ("Find Game Script Running");
		Screen.lockCursor = false;
		PhotonNetwork.ConnectUsingSettings("v1.0");
		lobbyType=Application.loadedLevelName;
		roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 4 };
		PhotonNetwork.JoinLobby();
	}

	void OnPhotonRandomJoinFailed()
	{
		// create a room
		Debug.LogError("Creating a room");
		PhotonNetwork.CreateRoom(null,roomOptions,TypedLobby.Default);
		Application.LoadLevel("Loading");
	}

	void OnJoinedRoom()
	{
		Debug.LogError("Joining a room");
		Application.LoadLevel("Loading");
	}

	void OnJoinedLobby()
	{
		Debug.LogError("In the lobby");
		PhotonNetwork.JoinRandomRoom();
	}
}
