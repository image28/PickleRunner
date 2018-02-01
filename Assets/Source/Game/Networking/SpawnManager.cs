using UnityEngine;
using System;
using System.Collections;

public class SpawnManager : Photon.MonoBehaviour {
	
	enum server
	{
		wait,
		finalize,
		done,
		restart
	};

	private bool[] randomSpawnArray;
	private int instantiated=0;
	private server connectionProgress=server.wait;
	[HideInInspector]
	public connectBuffer buffer;
	private playerManager manager;
	public int maxPlayers=4;
	public int[] forceSpawn =  new int[8];
	
	//public GameObject coinPrefab;
	public Transform TopLeft;
	public Transform BottomRight;

	[HideInInspector]
	public Transform[] playerPrefabArray;
	[HideInInspector]
	public bool gameStarted; // FIXME
	[HideInInspector]
	public int maxSpawns;
	[HideInInspector]
	public float startTime=0.0f;
	[HideInInspector]
	public int[] currentSpawn ;
	public GameObject[] spawnPoints;
	private float waiting=0.0f;
	public static int localPlayer=-1;

	void Start()
	{
		// Screen.lockCursor = true; // UNCOMMENT

		gameStarted=false;
		int count=0;
		instantiated=0;

		manager=GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<playerManager>();
		
		// SPAWN POINTS/POSITION CODE
		maxSpawns=GameObject.FindGameObjectsWithTag("SpawnPoint").Length;
		spawnPoints=new GameObject[maxSpawns];
		for(int d=1; d <= maxSpawns;d++)
		{
			spawnPoints[d-1]=GameObject.Find("Spawn" + d);
		}
		
		randomSpawnArray = new bool[maxSpawns];
		for(int d=0;d<maxSpawns;d++)
			randomSpawnArray[d]=false;
		
		// LOAD ALL THE PLAYER MODELS
		playerPrefabArray = new Transform[Resources.LoadAll("PlayerPrefabs", typeof(Transform)).Length];

		count=0;
		foreach(Transform d in Resources.LoadAll("PlayerPrefabs", typeof(Transform)))
		{
			playerPrefabArray[count] = d;
			count++;
		}
		
		// CHECK IF SINGLE PLAYER OR MULTIPLAYER
		if ( MainMenu.gameMode == 0 )
		{
			
			// SINGLE PLAYER
			SinglePlayerSpawn();
			localPlayer=0;
			manager.players = new GameObject[1];
			manager.players[0] = GameObject.Find("Player0");
			gameStarted=true;
			
			//randomCoins();
		}else{
			// MULTIPLAYER
			buffer = GameObject.Find("preSpawn").GetComponent<connectBuffer>();
			PhotonNetwork.isMessageQueueRunning=true;
		}
		
	}
	
	// MAIN UPDATE FUNCTION RUN EVERY FRAME : SERVER ONLY CODE
	void Update()
	{	
		// Check if this is a single player or multiplayer game
		if ( ( MainMenu.gameMode != 0 ) && ( PhotonNetwork.isMasterClient ) ) 
		{
			switch(connectionProgress)
			{
			case server.wait:

				if  ( buffer.playerCount >= maxPlayers-1 ) 
				{

					connectionProgress=server.finalize;	

					// SPAWN THE MASTER CLIENT
					SpawnPlayer (PhotonNetwork.player);
					#if DEBUG_NETWORK
					Debug.LogError(stringManager.Instance.SpawnManager0 + PhotonNetwork.player.ID.ToString());
					#endif
					// Spawn the random coins
//					randomCoins();
					
					// SPAWN ALL THE OTHER CLIENTS
					for(int d=0; d< maxPlayers-1; d++)
					{
						#if DEBUG_NETWORK
						Debug.LogError(stringManager.Instance.SpawnManager1 + buffer.remotePlayerArray[d].ID.ToString());
						#endif
						SpawnPlayer(buffer.remotePlayerArray[d]);
					}
					
					// CHANGE CONNECTION STATE TO FINALIZE
					
				}
				break;
				
			case server.finalize:
				// IF ALL THE PLAYERS ARE DONE SPAWNING
				//	Debug.LogError(buffer.playerCount + stringManager.Instance.SpawnManager2 + instantiated + stringManager.Instance.SpawnManager3 + (playerCount+1).ToString());
				if ( instantiated >= maxPlayers )
				{
					startTime=Time.time;
					
					// FINIALIZE STARTUP ON SERVER AND CLIENTS
					gameHasStarted(buffer.playerCount, PhotonNetwork.player);
					
					for(int i=0; i < maxPlayers-1; i++)
					{
						photonView.RPC(stringManager.Instance.SpawnManager4, buffer.remotePlayerArray[i], buffer.playerCount, buffer.remotePlayerArray[i]);
					}
					
					// CHANGE CONNECTION STATE TO DONE
					connectionProgress=server.done;
				}	
				
				break;
				
			case server.restart:
				restart();
				break;
			}
		}
	}

	[PunRPC]
	public void offMap(string playerName)
	{
		if ( ( PhotonNetwork.isMasterClient ) || ( MainMenu.gameMode == 0 ) )
		{
			int playerID=Convert.ToInt32(playerName.Substring(6));
			// Pick a random starting point for the playe
			//newPoint=pickSpawn(spawnPoints.Length);
			clientPlayer playerScript=GameObject.Find(playerName).GetComponent<clientPlayer>();
			/*int waypoint=(playerScript.nearestWaypoint-1);
			if ( waypoint < 0 )
			{
				waypoint=0;
			}
			//Debug.LogError("Finding waypoint " + waypoint);
			GameObject point =(GameObject) GameObject.Find("Waypoint" + waypoint.ToString());*/
			Vector3 at=playerScript.gameObject.transform.position;
			at.y=at.y+5.0f;

			RaycastHit hit;
			Vector3 newpos=at;
			
			// Reposition player on the ground
			if(Physics.Raycast(at, -Vector3.up,out hit, Mathf.Infinity))
			{
				newpos.y=(newpos.y-hit.distance)+0.5f;
			}
			
			at=newpos;

			respawnPlayer(playerID, playerName, playerID, at, Quaternion.identity);
			
			if ( MainMenu.gameMode != 0 )
			{
				photonView.RPC("respawnPlayer", PhotonTargets.OthersBuffered,playerID ,playerName, playerID, at, Quaternion.identity);
			}
		}
	}
	
	[PunRPC] 
	public void addCoin(string player, string coinName)
	{

		clientPlayer thePlayer=GameObject.Find(player).GetComponent<clientPlayer>();
		//score scoreBoard = GameObject.Find("ScoreBoard").GetComponent<score>();
		//scoreBoard.coins[Convert.ToInt32(player.Substring(6))]=thePlayer.coins;

		GameObject pickup=GameObject.Find(coinName);
		pickup.GetComponent<Renderer>().enabled=false;
		coin coinScript=pickup.GetComponent<coin>();
		coinScript.respawn=Time.time;
		coinScript.GetComponent<AudioSource>().PlayOneShot(coinScript.coinSound,0.2f);

		if ( coinName.Contains("Landmine") )
		{
			thePlayer.landmineCount++;
		}
		else if ( coinName.Contains("PickleJar") )
		{
			thePlayer.pickleJarCount++;
		}
	}

	// CHOOSE A UNIQUE RANDOM SPAWN POINT
	public int pickSpawn(int spawnCount)
	{
		// Select a previously unselected spawn point at random
		int spawnsUsed=0;
		
		for(int d=0;d<spawnCount;d++)
		{
			if ( randomSpawnArray[d] )
			{
				spawnsUsed++;
			}
		}
		
		// If all spawn points have been used, clear all used spawn points
		if ( spawnsUsed == spawnCount )
		{
			for(int d=0;d<spawnCount;d++)
			{
				randomSpawnArray[d]=false;
			}
		}
		
		int random=UnityEngine.Random.Range(0,spawnCount);
		
		while ( randomSpawnArray[random] )
		{
			random=UnityEngine.Random.Range(0,spawnCount);
		}
		
		randomSpawnArray[random]=true;
		
		//Debug.LogError ("Random point " + random);
		return(random);
	}
/*
	void randomCoins()
	{
		Vector2[] spawnPositions = new Vector2[200];
		
		for(int i=0; i<200; i++)
		{
			spawnPositions[i].x=UnityEngine.Random.Range(TopLeft.position.x,BottomRight.position.x);
			spawnPositions[i].y=UnityEngine.Random.Range(BottomRight.position.z,TopLeft.position.z);
		}
		
		if ( MainMenu.gameMode == 0 )
		{
			instantiateCoins(spawnPositions);
		}else{
			//Debug.LogError(DateTime.Now);
			photonView.RPC("instantiateCoins",PhotonTargets.AllBuffered,spawnPositions);
		}
	}*/

/*	[PunRPC]
	void instantiateCoins(Vector2[] points)
	{
		GameObject coinParent = new GameObject();
		coinParent.name="CoinRoot";
		coinParent.transform.position=Vector3.zero;
		
		for(int i=0; i<200; i++)
		{
			GameObject coin =(GameObject)Instantiate(coinPrefab, new Vector3(points[i].x,1000,points[i].y), coinPrefab.transform.rotation);
			coin.transform.eulerAngles= new Vector3(0,UnityEngine.Random.Range(0.0f,120.0f),0);
			coin.name="Coin" + i.ToString();
			coin.transform.parent=coinParent.transform;
			coin.GetComponent<coin>().Ground();
		}
	}*/

	// SINGLE PLAYER SPAWN FUNCTION
	void SinglePlayerSpawn()
	{
		// Pick a random starting point for the player
		
		
		int random=0;
		
		if ( forceSpawn[0] != 0 )
		{
			random=forceSpawn[0]-1;
		}else{
			random=pickSpawn(maxSpawns);
		}
		#if DEBUG_SPAWN
		Debug.LogError(maxSpawns + " Spawn points Found, selected " + spawnPoints[random].name);
		#endif
		
		// get the players network id number
		string tempPlayerString = "0";
		
		currentSpawn = new int[1];
		currentSpawn[0]=random;

		int playerType=0;

		// Instantiate the player
		Transform newPlayerTransform = (Transform)Instantiate(playerPrefabArray[playerType], spawnPoints[random].transform.position+(spawnPoints[random].transform.forward*15), Quaternion.identity);
		newPlayerTransform.name="Player" + tempPlayerString;
		newPlayerTransform.parent = GameObject.Find("Players").transform;
		//newPlayerTransform.eulerAngles=new Vector3(0,0,180);
		//Color rex=new Color(UnityEngine.Random.Range(0.0f,1.0f),UnityEngine.Random.Range(0.0f,1.0f),UnityEngine.Random.Range(0.0f,1.0f));
		//newPlayerTransform.FindChild("T-Rex_DINO").FindChild("roxy2").renderer.material.color=rex;
		newPlayerTransform.GetComponent<clientPlayer>().enabled=true;
		GameObject.Find("PlayerIcons").transform.Find("player" + (0).ToString()).GetComponent<GUITexture>().enabled=true;
		RaycastHit hit;
		Vector3 newpos=spawnPoints[random].transform.position;

		manager.buildArray(0);

		// Reposition player on the ground
		if(Physics.Raycast(spawnPoints[random].transform.position, -Vector3.up,out hit, Mathf.Infinity))
		{
			newpos.y=(newpos.y-hit.distance)+0.5f;
		}

		newPlayerTransform.position=newpos;
	}

	// NETWORK PLAYER SPAWN FUNCTION
	void SpawnPlayer(PhotonPlayer player)
	{
		int random;
		// Pick a random starting point for the player
		if ( forceSpawn[(player.ID-1)] != 0 )
		{
			random=forceSpawn[(player.ID-1)]-1;
		}else{
			random=pickSpawn(spawnPoints.Length);
		}
		


		// get the players network id number
		int playerID =player.ID-1;
		int playerType=playerID;
		#if DEBUG_SPAWN
		Debug.LogError("Spawing player with ID of " + playerID + " at spawnpoint " + random);
		#endif	



		Color rex=Color.white;//new Color(UnityEngine.Random.Range(0.0f,1.0f),UnityEngine.Random.Range(0.0f,1.0f),UnityEngine.Random.Range(0.0f,1.0f));
		
		photonView.RPC("instantiatePlayer", PhotonTargets.AllBuffered,  player, random, playerID,playerType,spawnPoints[random].transform.position,spawnPoints[random].transform.forward, spawnPoints[random].transform.rotation, rex.r, rex.g,rex.b);
		
		
	}

	// RESPAWN A PLAYER ON ALL CLIENTS
	[PunRPC]
	void respawnPlayer(int playerID, string name, int frag,  Vector3 position, Quaternion rotation)
	{
		#if DEBUG_SCORE
		Debug.LogError("Player" + frag + " killed " + name);
		#endif
		
		GameObject localPlayer = GameObject.Find(name);
		/*score scoreBoard = GameObject.Find("ScoreBoard").GetComponent<score>();
		
		scoreBoard.frags[frag]++;
		scoreBoard.deaths[playerID]++;
		
		#if DEBUG_SCORE
		Debug.LogError (scoreBoard.frags[frag] + "/" + scoreBoard.fraglimit + " frags");
		#endif*/
		
		
		if ( localPlayer == null )
			return;
		
		if ( ( playerID != PhotonNetwork.player.ID-1 ) && ( MainMenu.gameMode != 0 ) )
			localPlayer.GetComponent<CubeLerp>().enabled=false;
		
		#if DEBUG_SPAWN
		Debug.LogError("Respawning player");
		#endif
		
		RaycastHit hit;
		Vector3 newpos=position;
		newpos.y=50;
		
		if(Physics.Raycast(position, -Vector3.up,out hit, Mathf.Infinity))
		{
			newpos.y=(newpos.y-hit.distance)+0.5f;
		}
		
		position=newpos;
		
		/*if ( ServerCreateScript.gametype == 0 )
		{
			// if deathmatch
			if ( scoreBoard.frags[frag] < scoreBoard.fraglimit )
			{*/
				localPlayer.transform.position=position;
				localPlayer.transform.rotation=rotation;
			/*}
		}else{
			// if last man standing
			if ( scoreBoard.deaths[frag] < scoreBoard.fraglimit )
			{
				localPlayer.transform.position=position;
				localPlayer.transform.rotation=rotation;
			}
			
		}*/
		
		if ( ( playerID != PhotonNetwork.player.ID-1 ) && ( MainMenu.gameMode != 0 ) )
			localPlayer.GetComponent<CubeLerp>().enabled=true;
	}

	// ALLOCATE THE NETWORK VIEW ID ON CLIENTS
	[PunRPC]
	void allocateID(string name, int viewID)
	{
		
		//Debug.LogError ("Assigning networkview ID " + viewID);
		GameObject player = GameObject.Find(name);
		PhotonView view= player.GetComponent<PhotonView>();
		view.viewID = viewID;	
	}

	// RUN ON ALL CLIENTS : CREATE A PLAYER
	[PunRPC]
	void instantiatePlayer( PhotonPlayer player, int spawnPoint, int playerID, int playerType, Vector3 position, Vector3 forward, Quaternion rotation, float r, float g, float b)
	{
		currentSpawn = new int[maxPlayers];
		currentSpawn[playerID]=spawnPoint;
		string name="Player" + playerID.ToString();
		Transform thePlayer = (Transform)Instantiate(playerPrefabArray[playerType], position+(forward*15), rotation);
		thePlayer.parent = GameObject.Find("Players").transform;
		thePlayer.name=name;
		thePlayer.GetComponent<clientPlayer>().enabled=true;

		GameObject playerObj = GameObject.Find(name);
		playerObj.AddComponent<PhotonView>();
		PhotonView view;
		view = playerObj.GetComponent<PhotonView>();
		view.synchronization=ViewSynchronization.UnreliableOnChange;
		view.ObservedComponents.Add(thePlayer.GetComponent<CubeLerp>());

		RaycastHit hit;
		Vector3 newpos=position;
		//position.y=50;
		
		if(Physics.Raycast(newpos, -Vector3.up,out hit, Mathf.Infinity))
		{
			newpos.y=(newpos.y-hit.distance)+0.5f;
		}
		
		thePlayer.position=newpos;

		clientPlayer script = thePlayer.GetComponent<clientPlayer>();
		CubeLerp netRigid = thePlayer.GetComponent<CubeLerp>();
		script.playerID=playerID;

		if ( script != null )
		{
			#if DEBUG_NETWORK
			Debug.LogError("current player = " + player.ID + " and local player is = " + PhotonNetwork.player.ID );
			#endif
			
			script.Start();
			
			if ( player.ID == PhotonNetwork.player.ID )
			{
				localPlayer=player.ID-1;
				#if DEBUG_NETWORK
				Debug.LogError("Client is owner of Player" + playerID.ToString());
				#endif
				script.theOwner = player;
				script.enabled = true;
				netRigid.enabled = false;
//				thePlayer.GetComponent<netPlayer>().enabled=false;
				view.enabled=true;
				if ( PhotonNetwork.isMasterClient )
				{
					playerInstantiated();
				}else{
					photonView.RPC("playerInstantiated", PhotonTargets.MasterClient);
				}

				GameObject.Find("PlayerIcons").transform.Find("player" + playerID.ToString()).GetComponent<GUITexture>().enabled=true;
			}else{
				#if DEBUG_NETWORK
				Debug.LogError("Client is not owner of player" + playerID.ToString());
				#endif
				script.enabled = false;
				netRigid.enabled = true;
//				thePlayer.GetComponent<netPlayer>().enabled=true;
				view.enabled=true;
				
			}
			
			
			
		}else{
			#if DEBUG_NETWORK
			Debug.LogError("Network script is null");
			#endif
		}
	}

	[PunRPC] 
	public void playerHit(string killer, string player)
	{
		int killerID=System.Convert.ToInt32(killer);
		int loserID=System.Convert.ToInt32(player.Replace("Player",""));
		clientPlayer thePlayer=GameObject.Find(player).GetComponent<clientPlayer>();

		if ( thePlayer.animations.jumpAnim.FallingAnim != null )
			thePlayer.crossfade(thePlayer.animations.jumpAnim.FallingAnim.name,false);

		if ( player == "Player" + (PhotonNetwork.player.ID-1).ToString() )
			thePlayer.movement.lockMovement=Time.time;

	}

	// RUN ON SERVER : INCREMENT THE COUNT OF PLAYERS CREATED
	[PunRPC]
	void playerInstantiated()
	{
		instantiated++;	
		#if DEBUG_NETWORK
		Debug.LogError(instantiated + " players added");
		#endif
	}

	
	// RUN ON CLIENT : RPC Call to inform server that the current client is ready
	[PunRPC]
	void gameHasStarted(int players, PhotonPlayer player)
	{
		gameStarted=true;
		/*score scoreBoard = GameObject.Find("ScoreBoard").GetComponent<score>();
		scoreBoard.fraglimit = limit;
		scoreBoard.players=players+1;*/
		
		if ( PhotonNetwork.player == player )
		{
			#if DEBUG_NETWORK
			Debug.LogError("We are the local player");
			#endif
			int ID = player.ID - 1 ;
			int viewID;
			viewID = PhotonNetwork.AllocateViewID();
			photonView.RPC("allocateID", PhotonTargets.AllBuffered ,"Player" + ID.ToString(), viewID);
		}
		
		// Build the array of players for this player
		manager.buildArray(players);
		
		#if DEBUG_NETWORK
		Debug.LogError("Game has started");
		#endif
	}

	// INFORM THE CLIENTS THE GAME IS OVER AND SHOW FINIAL SCORE
	[PunRPC]
	void endgameClient()
	{
		//GameObject.Find("ScoreBoard").GetComponent<score>().displayScore();
		int ID=PhotonNetwork.player.ID-1;
		clientPlayer localplayer = GameObject.Find("Player" + ID.ToString()).GetComponent<clientPlayer>();
		localplayer.enabled=false;
		
		Invoke("restart",4f);
	}
	
	// TELL THE SERVER THE GAME IS OVER
	public void endgameServer()
	{
		
		//GameObject.Find("ScoreBoard").GetComponent<score>().displayScore();
		GameObject.Find ("Player0").GetComponent<clientPlayer>().enabled=false;
		//PhotonNetwork.SendOutgoingCommands();
		Invoke ("endgameFinalize",6f);
	}
	
	// SET THE SERVER RESTART FLAG
	void endgameFinalize()
	{
		connectionProgress=server.restart;
	}
	
	// PERFORM A GAME RESTART
	[PunRPC]
	void restart()
	{
		Destroy(buffer);
		//PhotonNetwork.LoadLevel("Menu");
	}
}
