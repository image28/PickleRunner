using UnityEngine;
using System;
using System.Collections;

public class clientPlayer : playerBase {
	public bool disableTerrainRotation=false;
	private string prevTerrain="";
	public int coins=0;
	public int fallDistance=-500;
	public int flyDistance=400;
	public float heightClamp=4.0f;
	public GUITexture portrait;
	public GameObject Track;
	public int nearestWaypoint=-1;
	private float waypointDistance=-1;
	public Transform currentPlacing;

	public GUIText landmineText;
	public GUIText pickleJarText;
	public GUITexture winner;
	public GUITexture loser;
	
	public void Start()
	{
		curDir=90.0f;
		if ( MainMenu.gameMode == 0 )
		{
			playerID=0;
		}

		landmineText=GameObject.Find("Counter2").GetComponent<GUIText>();
		pickleJarText=GameObject.Find("Counter1").GetComponent<GUIText>();

		// Set maxhealth
		stats.health=stats.maxhealth;
		scripts.manager=GameObject.Find("Players").GetComponent<playerManager>();

		currentPlacing=(Transform)GameObject.Find("CurrentPlacing").transform;

		// Get Gameobjects
		mainCamera = GameObject.Find("Main Camera");
		
		// Get all Scipts
		scripts.spawnScript=GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
		scripts.followScript = mainCamera.GetComponent<smoothFollow>();
		scripts.manager = GameObject.Find("Players").GetComponent<playerManager>();
		mainCamera.transform.parent=transform;
		/*mainCamera.transform.position=Vector3.zero;
		mainCamera.transform.position=new Vector3(0,0.2f,-0.5f);
		mainCamera.transform.LookAt(mainCamera.transform.parent);*/

		if ( ! scripts.spawnScript )
		{
			Debug.LogError("Spawn script not found");
		}
		transform.eulerAngles=new Vector3(0,90.0f,0);
		// normal starts as character up direction
		movement.myNormal = transform.up;
		movement.myTransform = transform;

		GetComponent<Rigidbody>().freezeRotation = true; // disable physics rotation



		if ( animations.IdleAnim != null )
		{
			model.GetComponent<Animation>()[animations.IdleAnim.name].layer = 0;
			model.GetComponent<Animation>()[animations.IdleAnim.name].speed=animations.IdleAnimSpeed;
		}

		if ( animations.movementAnim.WalkingAnim != null )
		{
			model.GetComponent<Animation>()[animations.movementAnim.WalkingAnim.name].layer = 0;
			model.GetComponent<Animation>()[animations.movementAnim.WalkingAnim.name].speed=animations.movementAnim.WalkAnimSpeed;
		}

		if ( animations.movementAnim.RunningAnim != null )
			model.GetComponent<Animation>()[animations.movementAnim.RunningAnim.name].speed=animations.movementAnim.RunAnimSpeed;
		if ( animations.jumpAnim.JumpingAnim != null )
			model.GetComponent<Animation>()[animations.jumpAnim.JumpingAnim.name].speed=animations.jumpAnim.JumpAnimSpeed;
		if ( animations.jumpAnim.FallingAnim != null )
			model.GetComponent<Animation>()[animations.jumpAnim.FallingAnim.name].speed=animations.jumpAnim.FallAnimSpeed;
		if ( animations.jumpAnim.LandingAnim != null  )
			model.GetComponent<Animation>()[animations.jumpAnim.LandingAnim.name].speed=animations.jumpAnim.LandAnimSpeed;
		if ( animations.ShootingAnim != null )
		{
			model.GetComponent<Animation>()[animations.ShootingAnim.name].speed=animations.ShootAnimSpeed;
		}
	}

	/*void Awake()
	{
		Vector3 pos = transform.position;
		Ray ray;
		RaycastHit hit;
		ray = new Ray(transform.position, -Vector3.up); // cast ray downwards
		if (Physics.Raycast(ray, out hit, Mathf.Infinity)){
			pos.y=pos.y-hit.distance+0.5f;
		}
		transform.position=pos;
	}*/
		
	void FindNearestWaypoint()
	{


		if ( nearestWaypoint == -1 )
		{
			float distance=1000;
			int nearest=-1;

			if ( Track == null )
				Track=(GameObject)GameObject.FindWithTag("Path");

			foreach(Transform waypoint in Track.transform)
			{
				float tempDist=Vector3.Distance(transform.position,waypoint.position);
				if ( tempDist < distance )
				{
					waypointDistance=tempDist;
					nearestWaypoint=Convert.ToInt32(waypoint.name.Substring(8));
		
				}
			}
		}else{
			int currentRank=nearestWaypoint;

			Transform next;
			Transform prev;
			//Debug.LogError("Waypoint gfsgfds " + Track.transform.childCount);
			Transform current=Track.transform.Find("Waypoint"+nearestWaypoint).transform;

			if ( nearestWaypoint <= 0 )
				prev=Track.transform.Find("Waypoint"+(Track.transform.childCount-1).ToString()).transform;
			else
				prev=Track.transform.Find("Waypoint"+(nearestWaypoint-1).ToString()).transform;

			if ( nearestWaypoint >= Track.transform.childCount-1 )
				next=Track.transform.Find("Waypoint"+0.ToString()).transform;
			else
				next=Track.transform.Find("Waypoint"+(nearestWaypoint+1).ToString()).transform;

			float prevDist=Vector3.Distance(transform.position,prev.position);
			float currentDist=Vector3.Distance(transform.position,current.position);
			float nextDist=Vector3.Distance(transform.position,next.position);

			if ( currentDist < prevDist )
			{
				if ( nextDist < currentDist )
				{
					nearestWaypoint=Convert.ToInt32(next.name.Substring(8));
					waypointDistance=nextDist;
				}
			}else{
				nearestWaypoint=Convert.ToInt32(prev.name.Substring(8));
				waypointDistance=prevDist;
			}

		
			/*if ( scripts.manager.nextWaypoint[playerID] == 60 )
			{
				crossfade(animations.movementAnim.Winner.name,true);
				if ( winner != null )
					playClip(animations.movementAnim.WinnerClip,true);

				int pc=scripts.spawnScript.maxPlayers;

				if ( MainMenu.gameMode == 0 )
					pc=1;

				for(int i=0; i < pc; i++)
				{
					if ( i != playerID )
					{
						scripts.manager.players[i].GetComponent<clientPlayer>().crossfade(animations.movementAnim.Loser.name,true);
						//if ( scripts.manager.players[i].GetComponent<clientPlayer>().loser != null )
						//	scripts.manager.players[i].GetComponent<clientPlayer>().playClip(loser);
					}
				}
			}else{*/

			//if ( currentRank != nearestWaypoint )
			//{
				updateRank(playerID,nearestWaypoint,nextDist);

				if ( MainMenu.gameMode != 0 )
					photonView.RPC("updateRank",PhotonTargets.OthersBuffered,playerID,nearestWaypoint,nextDist);
			//}
			//}
		}


		//Debug.LogError("Waypoint " + nearestWaypoint);
	}

	void calculateRank()
	{
		int playerCount=1;

		if ( MainMenu.gameMode != 0 )
		{
			playerCount=scripts.spawnScript.maxPlayers;
		}

		int[] ranking = new int[playerCount];

		for(int i=0; i< playerCount; i++)
		{
			ranking[i]=scripts.manager.waypoint[i];
		}

		Array.Sort(ranking);
		int rank=0;
		for(int i=0; i<playerCount; i++)
		{
			rank=((playerCount-1)-i);
			if ( ranking[i] == scripts.manager.waypoint[playerID] )
			{
				currentPlacing.Find("number" + (rank.ToString())).GetComponent<GUITexture>().enabled=true;
			}else{
				currentPlacing.Find("number" + (rank.ToString())).GetComponent<GUITexture>().enabled=false;
			}
		}

		/*float checkDist=scripts.manager.distToWaypoint[playerID];

		for(int i=0; i < playerCount; i++)
		{
			if ( ( scripts.manager.waypoint[playerID] == scripts.manager.waypoint[i] ) && ( i != playerID ) )
			{
				if ( checkDist > scripts.manager.distToWaypoint[i] )
				{
					currentPlacing.FindChild("number" + (rank.ToString())).guiTexture.enabled=false;
					rank++;
					currentPlacing.FindChild("number" + (rank.ToString())).guiTexture.enabled=true;
				}
			}
		}*/


	}

	[PunRPC] 
	void playAnimation(string name)
	{
		crossfade(name,false);
		if ( name != currentAnimation )
			currentAnimation=name;
	}

	/*[PunRPC] 
	void playAudio(AudioClip name)
	{
		playClip(name,false);
	}*/


	[PunRPC]
	void updateRank(int ID, int waypoint, float distance)
	{
		if ( Track == null )
			Track=(GameObject)GameObject.FindWithTag("Path");

		if ( scripts.manager.waypoint[ID] != -1 )
		{

			if ( ( waypoint >= scripts.manager.nextWaypoint[ID] ) && ( waypoint < scripts.manager.nextWaypoint[ID]+20 ) )
			{
				scripts.manager.nextWaypoint[ID]=waypoint+1;
			}
			scripts.manager.waypoint[ID]=waypoint;
		}else{
			scripts.manager.waypoint[ID]=waypoint;



			if ( waypoint >= Track.transform.childCount-1 )
			{
				// Win Condition // End the Game
				// Say Doctor/Cowboy/Pirate/Spaceman is the winner on all computers...
				// Player winner loser animation, spin the camera
				// Disconnect the network after a couple of seconds
				scripts.manager.nextWaypoint[ID]=0;
			}

			if ( ( waypoint >= scripts.manager.nextWaypoint[ID] ) && ( waypoint < scripts.manager.nextWaypoint[ID]+20 ) )
			{
				scripts.manager.nextWaypoint[ID]=waypoint+1;
			}
		}

		scripts.manager.distToWaypoint[ID]=distance;
		//Debug.LogError(scripts.manager.nextWaypoint[ID]);
	}

	void updateText()
	{
		pickleJarText.text="x" + pickleJarCount.ToString();
		landmineText.text="x" + landmineCount.ToString();
	}

	// MAIN UPDATE FUNCTION, PERFORM LOCAL PLAYER MOVEMENT AND INFORM ALL OTHER PLAYERS OF THE MOVEMENT
	void Update()
	{
		if ( ( MainMenu.gameMode == 0 ) || ( ( scripts.spawnScript.gameStarted ) && (theOwner != null ) && ( PhotonNetwork.player == theOwner) && ( gameObject.name == stringManager.Instance.clientPlayer0 + playerID.ToString() ) ) && ( scripts.manager.playersFound ) )  
        {

			if ( movement.lockMovement <= 0 )
			{
				FindNearestWaypoint();
				calculateRank();
				updateText();
			
				movement.HInput = 0;
				movement.VInput = 0;

				if ( ( movement.jumping ) && ( ! movement.speed.wheeledVehicle ) )
				{
					movement.landing=false;
					Ray ray2;
					RaycastHit hit2;
					ray2 = new Ray(transform.position, -movement.myNormal); // cast ray downwards
					if (Physics.Raycast(ray2, out hit2))
					{

						if ( movement.prevHeight > hit2.distance )
						{
							movement.maxHeightReached=true;
						}
						else if ( ( movement.maxHeightReached ) && ( hit2.distance < movement.landDistance ) )
						{
							movement.maxHeightReached=false;
							movement.jumping=false;
							Debug.LogError(stringManager.Instance.clientPlayer1);
							movement.landing=true;
						}
							
						movement.prevHeight=hit2.distance;


					}
				}else{
					movement.maxHeightReached=false;
					movement.prevHeight=0.0f;
				}

				#if UNITY_STANDALONE
				// move local player and send new position to server
				movement.HInput = Input.GetAxis(stringManager.Instance.clientPlayer2);
	            movement.VInput = Input.GetAxis(stringManager.Instance.clientPlayer3);
				#endif

			//	rigidbody.isKinematic = false;

				// WALL WALKING CODE BLOCK
				if ( ( ! movement.jumping ) && ( ! movement.speed.wheeledVehicle ) ) 
				{
					#if UNITY_STANDALONE
					if ( ( Input.GetButtonDown(stringManager.Instance.clientPlayer4) ) && ( animations.jumpAnim.JumpingAnim != null ) )
					{ // jump pressed:
						GetComponent<Rigidbody>().velocity += movement.jumpHeight * movement.myNormal;
						movement.jumping=true;
					}
					#endif
				}

				if ( ( movement.maxHeightReached ) && ( animations.jumpAnim.FallingAnim != null )) {

						crossfade(animations.jumpAnim.FallingAnim.name,true);

				}else if ( ( movement.landing ) ){

					if ( animations.jumpAnim.LandingAnim != null )
					{
						crossfade(animations.jumpAnim.LandingAnim.name,true);

						if( model.GetComponent<Animation>()[animations.jumpAnim.LandingAnim.name].normalizedTime >= 0.8)
						{
							movement.landing=false;
							Debug.LogError(stringManager.Instance.clientPlayer5);
						}
					}else{
						movement.landing=false;
					}
				}else if  ( ( movement.jumping ) && ( animations.jumpAnim.JumpingAnim != null ) ){

						crossfade(animations.jumpAnim.JumpingAnim.name,true);
				}else if ( ( attack.OneTime > 0.0f ) && ( stomping != null ) ){

					crossfade(stomping.name,true);
				}else if ( ( movement.VInput > 0.0f ) || ( movement.VInput < 0.0f ) )
				{

					if ( ! movement.jumping )
					{
						if ( movement.speed.running )
						{
							if ( animations.movementAnim.RunningAnim != null )
							{
								crossfade(animations.movementAnim.RunningAnim.name,true);
							}
						}else{
							if ( animations.movementAnim.WalkingAnim != null )
							{

								crossfade(animations.movementAnim.WalkingAnim.name,true);
							}
						}
					}

				}else if ( ( movement.HInput > 0.0f ) || ( movement.HInput < 0.0f ) ) 
				{
					//this.animation.CrossFade(stringManager.Instance.clientPlayer6,true);
				}else{

					if ( animations.IdleAnim != null )
					{
						crossfade(animations.IdleAnim.name,true);
					}
				}
	
				turnPlayer();

				RaycastHit wall;
				bool forward=true;

				if ( movement.VInput != 0 )
				{
					if ( Physics.Raycast(new Vector3(transform.position.x, transform.position.y+0.5f, transform.position.z),transform.forward*movement.VInput,out wall,1.5f) )
					{
						if ( wall.collider.gameObject.tag == stringManager.Instance.clientPlayer7 )
						{
							forward=false;
						}
					}
				}

				Vector3 target=transform.position;

				if ( forward )
				{
					GetComponent<Rigidbody>().isKinematic=false;
					if ( movement.speed.boostEnabled )
					{
						target=target+(transform.forward*movement.VInput);
						transform.position=Vector3.MoveTowards(transform.position,target,movement.speed.boostSpeed*Time.deltaTime);
						if ( Time.time > movement.speed.boostTimer+movement.speed.boostDurration )
						{
							movement.speed.boostEnabled=false;
						}

					}else{

						target=target+(transform.forward*movement.VInput);
						transform.position=Vector3.MoveTowards(transform.position,target,movement.speed.walkSpeed*Time.deltaTime);
						movement.speed.running=false;

					}
				}else{
					if ( animations.jumpAnim.FallingAnim != null )
					{
						GetComponent<Rigidbody>().isKinematic=true;
						crossfade(animations.jumpAnim.FallingAnim.name,true);
						// network play
						movement.lockMovement=Time.time;
					}

				}

				if ( attack.canShoot )
				{
					#if UNITY_STANDALONE
					if( Input.GetAxis(stringManager.Instance.clientPlayer8) > 0)
					{
						if( (attack.OneTime == 0.0f) && ( ! attack.canceled ) && ( pickleJarCount > 0 ) )
					 	{ 
						//	Debug.LogError(playerID);

							if ( MainMenu.gameMode != 0 )
								photonView.RPC(stringManager.Instance.clientPlayer9, PhotonTargets.OthersBuffered,playerID, (int)attackType.pickleJar);
							
							Shoot(playerID,(int)attackType.pickleJar);


							attack.OneTime=0.1f;
						}else{
							attack.canceled=false;
						}
					}

					if ( Input.GetAxis(stringManager.Instance.clientPlayer10) > 0 )
					{
						if( (attack.OneTime == 0.0f) && ( ! attack.canceled ) && ( landmineCount > 0 ) )
						{ 
							if ( MainMenu.gameMode != 0 )
								photonView.RPC(stringManager.Instance.clientPlayer11, PhotonTargets.OthersBuffered,playerID, (int)attackType.landMine);
							
							Shoot(playerID,(int)attackType.landMine);
							
							attack.OneTime=0.1f;
						}else{
							attack.canceled=false;
						}
					}
					#endif
								
					// Check if bullet has been fired in the last 1.5 seconds
					if ( attack.OneTime > 0.0f )
						attack.OneTime=attack.OneTime + ( 1.0f * Time.deltaTime );
				
					if ( attack.OneTime >= 0.6f )
						attack.OneTime=0.0f;
				}

				offMap();
	        
			}else{
				if ( Time.time > movement.lockMovement+movement.lockTime )
				{
					movement.lockMovement=0;
				}
			}

		}
		
	}

	void turnPlayer()
	{
		float turn = 0.0f;
		
		if ( movement.VInput >= 0 )
			turn = movement.HInput*movement.speed.rotateSpeed*Time.deltaTime;
		else
			turn = (-movement.HInput)*movement.speed.rotateSpeed*Time.deltaTime;
		
		curDir = (curDir + turn) % 360;

		
		Vector3  myTarget = new Vector3(0,curDir,0);
		if ( disableTerrainRotation )
		{
			transform.rotation=Quaternion.Euler(0,curDir, 0);
		}else{
			transform.rotation=getTerrainRot( myTarget, transform.position)*Quaternion.Euler(0,curDir, 0);
		}
	}
	
	void offMap()
	{
		if ( ( transform.position.y < (float)fallDistance ) )
		{
			if ( MainMenu.gameMode == 0 )
			{
				scripts.spawnScript.offMap(gameObject.name);
			}else{
				scripts.spawnScript.photonView.RPC(stringManager.Instance.clientPlayer12,PhotonTargets.MasterClient,gameObject.name);
			}
		}
	}
	
	Quaternion getTerrainRot(Vector3 myTarget, Vector3 terrPos)
	{
		RaycastHit terrHit ;
		Vector3 newPos = Vector3.zero;
		Vector3 normal = Vector3.up; // default normal = up

		if (Physics.Raycast (terrPos, -movement.curNormal, out terrHit, 100.0f))
		{
			normal = terrHit.normal; 
		}

		movement.curNormal = Vector3.Lerp(movement.curNormal, normal, 2*Time.deltaTime);   
		newPos = myTarget - terrPos; // get the direction to look at
		newPos.y = 0; // zero y to keep only the horizontal direction
		// calculate rotation to normal
		Quaternion rot = Quaternion.FromToRotation(Vector3.up, movement.curNormal);
		// combine with the horizontal rotation
		rot = rot * Quaternion.FromToRotation(Vector3.forward, newPos);

		return rot;
	}

	void OnTriggerEnter(Collider col)
	{
		if ( col.gameObject.name.Contains(stringManager.Instance.clientPlayer13) )
		{
			crossfade(animations.movementAnim.Winner.name,true);

			if ( winner != null )
				playClip(animations.movementAnim.WinnerClip,true);
			
			int pc=scripts.spawnScript.maxPlayers;
			
			if ( MainMenu.gameMode == 0 )
				pc=1;
			
			for(int i=0; i < pc; i++)
			{
				if ( i != playerID )
				{
					clientPlayer currentPlayer=scripts.manager.players[i].GetComponent<clientPlayer>();
					currentPlayer.crossfade(currentPlayer.animations.movementAnim.Loser.name,true);
					//if ( scripts.manager.players[i].GetComponent<clientPlayer>().loser != null )
					//	scripts.manager.players[i].GetComponent<clientPlayer>().playClip(loser);
				}
			}
		}
	}
}
					
