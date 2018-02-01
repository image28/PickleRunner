using UnityEngine;
using System.Collections;

public class explosionCollision : Photon.MonoBehaviour {

	[HideInInspector]
	public string playerID;
	[HideInInspector]
	public float ttl;
	[HideInInspector]
	public bool hit=false;
	private SpawnManager spawnScript;
	[HideInInspector]
	private playerBase playerScript;

	void OnEnable()
	{
		hit=false;
		ttl=0.0f;
	}

	void OnTriggerEnter(Collider collision)
    {

		if ( ( collision.gameObject.name.Contains("Player") ) && ( ! playerID.Contains("-1") ) && ( playerID != string.Empty ) )
		{
			if ( "Player" + playerID != collision.gameObject.name ) 
			{
				Debug.LogError("Player" + playerID + " has hit " + collision.gameObject.name);
				if ( ( ttl < 0.5f ) && ( ! hit ) )
				{
					Debug.LogError("Player" + playerID + " has hit " + collision.gameObject.name);
					if ( PhotonNetwork.isMasterClient )
					{	
						playerScript=collision.gameObject.GetComponent<playerBase>();

						if ( ! spawnScript )
							spawnScript=GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
						//if ( playerScript.attack.explosion != null )
						//	playerScript.playClip(playerScript.attack.explosion,true);

						spawnScript.playerHit(playerID, collision.gameObject.name);

						if ( MainMenu.gameMode != 0 )
						{
							spawnScript.GetComponent<PhotonView>().RPC("playerHit", PhotonTargets.OthersBuffered, playerID, collision.gameObject.name);
						}
					}
					hit=true;
				}
			}

			playerID="-1";
		}
		

	}
}
