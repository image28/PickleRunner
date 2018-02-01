using UnityEngine;
using System.Collections;

public class score : MonoBehaviour {
	
/*	//Public
	public GameObject spawn;
	public bool hidden=true;
	public bool over=false;
	public GameObject playerScore;
	
	//Public Hidden
	[HideInInspector]
	public int[] frags;
	[HideInInspector]
	public int[] deaths;
	[HideInInspector]
	public int players=0;
	[HideInInspector]
	public int[] coins;
	[HideInInspector]
	public int fraglimit=99;

	
	//Private
	private GameObject[] info;
	private GameObject header;
	private float startTime;
	private int maxPlayers;
	private SpawnManager spawnScript;
	
	
	// Use this for initialization
	void Start () {
		spawnScript =  GameObject.Find("PlayerSpawn").GetComponent<Spawn>();
		Vector3 startPos = new Vector3(-0.28f,0.95f,0.05881374f);
		over=false;
		maxPlayers=ServerCreateScript.maxPlayers+1;
		header=GameObject.Find("header");
		header.SetActive(false);
		
		info = new GameObject[maxPlayers];
		frags = new int[maxPlayers];
		deaths = new int[maxPlayers];
		coins = new int[maxPlayers];
		
		startPos.x = startPos.x + gameObject.transform.position.x;
		startPos.y = startPos.y + gameObject.transform.position.y;
		startPos.z = startPos.z + gameObject.transform.position.z;
		
		for(int d=0;d<maxPlayers;d++)
		{
			info[d] = (GameObject)Instantiate(playerScore, startPos,gameObject.transform.rotation);
			info[d].name=spawnScript.playerNamesArray[d] + "score";
			info[d].transform.parent=gameObject.transform;
			startPos.y=startPos.y-0.05f;
			info[d].SetActive(false);
			frags[d]=0;
			deaths[d]=0;
		}
	}
	
	void Update()
	{
		
		
		if ( PhotonNetwork.isMasterClient )
		{
			if ( spawnScript.gameStarted )
			{
				if ( startTime == 0 )
				{
					startTime=spawn.GetComponent<Spawn>().startTime;
				}
				
				int fallen=0;
				
				if ( ServerCreateScript.timelimit != 0 )
				{
					float currentTime=Time.time;
					//Debug.LogError(currentTime + stringManager.Instance.score0 + startTime + stringManager.Instance.score0 + (float)ServerCreateScript.timelimit*60.0f);
					if ( ( currentTime-startTime >= (float)ServerCreateScript.timelimit*60.0f ) && ( ! over ) )
					{
						Debug.LogError(stringManager.Instance.score2);
						spawn.GetComponent<PhotonView>().RPC(stringManager.Instance.score3,PhotonTargets.OthersBuffered);
						spawn.GetComponent<Spawn>().endgameServer();
						over=true;
					}
				}
				
				for (int d=0; d < players; d++ )
				{
					if ( ServerCreateScript.gametype == 0 )
					{
						// Deathmatch
						if ( ( coins[d] >= fraglimit ) && ( fraglimit > 0 ) && ( ! over ) )
						{
							//Debug.LogError(stringManager.Instance.score4);
							spawn.GetComponent<PhotonView>().RPC(stringManager.Instance.score5,PhotonTargets.OthersBuffered);
							spawn.GetComponent<Spawn>().endgameServer();
							over=true;
							d=maxPlayers;
						}
					}else{
						// Last Man Standing
						if ( ( deaths[d] >= fraglimit ) && ( fraglimit >0 ) && ( ! over ) )
						{
							fallen++;
						}
					}
				}
				
				
				if ( ( ServerCreateScript.gametype == 1 ) && ( fallen >= players-1 ) && ( ! over ) )
				{
					spawn.GetComponent<PhotonView>().RPC(stringManager.Instance.score6,PhotonTargets.OthersBuffered);
					spawn.GetComponent<Spawn>().endgameServer();
					over=true;
				}
			}
		}
		

		if(Input.GetKeyDown(stringManager.Instance.score7))
		{
			displayScore ();
		
		}
		
		
		if ( ( Input.GetKeyUp(stringManager.Instance.score8) ) && ( ! hidden ) )
		{
			hideScore();
		}
	}
	
	public void displayScore()
	{
		//Debug.LogError(players);
		if ( players > 0 )
		{
			hidden=false;
			header.SetActive(true);
			for(int d=0; d<players;d++)
			{
				info[d].SetActive(true);
				info[d].guiText.text=spawnScript.playerNamesArray[d] + stringManager.Instance.score9 + frags[d] + stringManager.Instance.score10 + deaths[d] + stringManager.Instance.score11 + coins[d];
			}
		}
	}
	
	void hideScore()
	{
		if ( players > 0 )
		{
			hidden=true;
			header.SetActive(false);
			
			for(int d=0;d < players;d++)
			{
				info[d].SetActive(false);
			}
		}
	}*/
}
