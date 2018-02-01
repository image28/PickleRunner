using UnityEngine;
using System;
using System.Collections;

public class bulletCollision : Photon.MonoBehaviour 
{

//	private GameObject bulletCamera;
//	private smoothFollow followScript;
	private explosionPool explosionScript;
	public bool terrainDestruction=false;
	[HideInInspector]
	public float tempHeight;
	[HideInInspector]
	public float tempDist;
	[HideInInspector]
	public float rotDamp;
	[HideInInspector]
	public float heightDamp;
	[HideInInspector]
	public string playerID;
	public AudioClip pickleClip;
	public AudioClip landMineClip;
	private AudioSource source;

	public void Start()
	{
		source=gameObject.GetComponent<AudioSource>();
		explosionScript = GameObject.Find("ExplosionPool").GetComponent<explosionPool>();
	}

	// PLACES AND GENORATES THE EXPLOSION EFFECT FROM THE EXPLSION POOL
	void genorate_explosion()
	{
#if DEBUG_POOLS
		Debug.LogError("Generating explosion #" + explosionScript.currentExplosion + " at " + bullet.position);
#endif
		explosionScript.explosions[explosionScript.currentExplosion].transform.position=transform.position;
		explosionScript.explosions[explosionScript.currentExplosion].transform.rotation=transform.rotation;
		explosionScript.explosions[explosionScript.currentExplosion].SetActive(true);
		explosionScript.explosions[explosionScript.currentExplosion].GetComponent<Detonator>().Explode();
		explosionScript.explosions[explosionScript.currentExplosion].GetComponent<Detonator>().Reset();
		explosionScript.explosions[explosionScript.currentExplosion].GetComponent<explosionCollision>().playerID=playerID;
		explosionScript.currentExplosion++;
		if ( explosionScript.currentExplosion > explosionScript.maxExplosionPool-1 )
		{
			explosionScript.currentExplosion=0;
		}
		
	}
	
	// BULLET COLLISION FUNCTION / REPLACE WITH EXPLOSION COLLISION
	void OnCollisionEnter(Collision collision)
    {
#if DEBUG_COLLISIONS
		Debug.LogError("Bullet Colliding with " + collision.gameObject.name + " at (" + transform.position.x + "," + transform.position.z + ")");
#endif

		if ( gameObject.transform.parent.name == "LandMinePool" )
		{
			if ( landMineClip != null )
				playClip(landMineClip,true);
		}else{
			
			if ( pickleClip != null )
				playClip(pickleClip,true);
		}

		// Create an explosion on the client
		genorate_explosion();
	
		playerID="";
		gameObject.SetActive(false);
		
	}

	public void playClip(AudioClip animationName, bool local)
	{
		if ( animationName != null )
		{
			source.PlayOneShot(animationName);
			
			/*if ( ( MainMenu.gameMode != 0 ) && ( local ) )
			{

					photonView.RPC("playAudio",PhotonTargets.OthersBuffered, animationName);
			}*/
		}
	}
	

}
