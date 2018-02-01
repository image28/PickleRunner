using UnityEngine;
using System.Collections;

public class playerMinimapIcon : MonoBehaviour {

	private Transform thisPlayer;
	private clientPlayer script;
	private bool once=true;

	// Use this for initialization
	void Start () {
		thisPlayer=transform.parent;
		transform.parent=null;
		script=thisPlayer.GetComponent<clientPlayer>();
	}
	
	// Update is called once per frame
	void Update () {

		if ( script.scripts.spawnScript.gameStarted )
		{
			if ( once )
			{
				if ( thisPlayer.name.Contains((PhotonNetwork.player.ID-1).ToString()) )
				{
					gameObject.GetComponent<Renderer>().material.color=Color.blue;
				}else{
					gameObject.GetComponent<Renderer>().material.color=Color.green;
				}

				once=false;
			}
	
			transform.position=new Vector3(thisPlayer.position.x,600,thisPlayer.position.z);
		}
	}
}
