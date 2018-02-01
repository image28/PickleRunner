using UnityEngine;
using System.Collections;

public class boost : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col)
	{
		if ( col.gameObject.name.Contains("Player") )
		{
			clientPlayer script = col.gameObject.GetComponent<clientPlayer>();
			script.Shoot(script.playerID,(int)playerBase.attackType.boost);

			if ( MainMenu.gameMode != 0 )
			{
				col.gameObject.GetComponent<PhotonView>().RPC("Shoot",PhotonTargets.OthersBuffered,script.playerID,playerBase.attackType.boost);
			}
		}
	}
}
