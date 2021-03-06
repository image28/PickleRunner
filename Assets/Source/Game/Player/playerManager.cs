using UnityEngine;
using System.Collections;

public class playerManager : MonoBehaviour {
	
	[HideInInspector]
	public SpawnManager spawnScript;
	
	[HideInInspector]
	public bool playersFound=false;
	
	[HideInInspector]
	public GameObject[] players;

	[HideInInspector]
	public int[] waypoint;

	[HideInInspector]
	public int[] nextWaypoint;

	[HideInInspector]
	public float[] distToWaypoint;

	public float[] waypointCount;

	void Start()
	{
		spawnScript=GameObject.Find("SpawnManager").GetComponent<SpawnManager>();	
	}
	
	// BUILD AN ARRAY OF ALL PLAYERS
	public void buildArray(int playerCount)
	{
		// if the playersCount is not -1, and this function hasn't been run already for this player
		if ( ( playerCount != -1 ) && ( ! playersFound ) )
		{
			// Create the players array
			players = new GameObject[playerCount+1];
			waypoint = new int[playerCount+1];
			nextWaypoint = new int[playerCount+1];
			distToWaypoint = new float[playerCount+1];
			waypointCount = new float[playerCount+1];

			// For all the players in the game
			for(int d=0;d<playerCount+1;d++)
			{
		
				players[d]=GameObject.Find("Player" + d);
				players[d].GetComponent<PhotonView>().ObservedComponents.Add(players[d].GetComponent<CubeLerp>()); // players[d].transform
				waypoint[d]=-1;
				nextWaypoint[d]=-1;
				distToWaypoint[d]=-1;
			}
			
			// set playerFound for extra checking to make sure this function doesn't get run more than once per player
			playersFound=true;
		}
				
	}
}
