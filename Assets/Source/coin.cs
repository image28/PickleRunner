using UnityEngine;
using System.Collections;

public class coin : Photon.MonoBehaviour {

	[Range(0.1f,500.0f)]
	public float height=10.0f;
	[Range(1.0f,50.0f)]
	public float spinSpeed=10.0f;
	public SpawnManager spawnScript;
	public PhotonView masterclient;
	[Range(1.0f,60.0f)]
	public float respawnTime=10;
	public float respawn=0;
	public AudioClip coinSound;

	// Use this for initialization
	void Start() {
		RaycastHit hit;
		if(Physics.Raycast(transform.position, -Vector3.up,out hit, Mathf.Infinity))
		{
			transform.position=new Vector3(transform.position.x, (transform.position.y-hit.distance)+height, transform.position.z);
		}

		spawnScript=GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
		masterclient=GameObject.Find("SpawnManager").GetComponent<PhotonView>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(0,spinSpeed*Time.deltaTime,0));

		if ( ! gameObject.GetComponent<Renderer>().enabled ) 
		{
			if ( Time.time > respawn+respawnTime )
			{
				respawn=0;
				gameObject.GetComponent<Renderer>().enabled=true;
			}
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if ( ( ( MainMenu.gameMode == 0 ) || ( PhotonNetwork.isMasterClient ) ) && ( col.gameObject.name.Contains("Player") ) && ( gameObject.GetComponent<Renderer>().enabled ) )
		{
			//Debug.LogError("Picked up a coin");
			if ( MainMenu.gameMode != 0 )
			{
				masterclient.RPC("addCoin",PhotonTargets.OthersBuffered,col.gameObject.name,gameObject.name);
			}

			if ( spawnScript != null )
				spawnScript.addCoin(col.gameObject.name,gameObject.name);
		}
	}
}
