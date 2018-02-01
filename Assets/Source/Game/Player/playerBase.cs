using UnityEngine;
//using UnityEditor;
using System;
using System.Collections;



[Serializable]
public class playerSpeeds
{
	// SPEED SETTINGS
	[Range(20.0f,200.0f)][Tooltip("Walking speed of the player")]
	public float walkSpeed;
	[Range(30.0f,200.0f)][Tooltip("Running speed of the player")]
	public float runSpeed;
	[Range(20.0f,200.0f)][Tooltip("Rotation speed of the player")]
	public float rotateSpeed;
	public bool boostEnabled=false;
	[Range(30.0f,200.0f)]
	public float boostSpeed;
	[HideInInspector]
	public float boostTimer=0.0f;
	[Range(1.0f,20.0f)]
	public float boostDurration=5.0f;
	public AudioClip boostClip;

	// Wheeled Vehicles
	[Tooltip("Testing, use proper physics for wheeled vehicles movements")]
	public bool wheeledVehicle;
	[HideInInspector]
	public bool running=false;

}

[Serializable]
public class playerMovement
{
	public playerSpeeds speed;



	// MOVEMENT TEMP VARIABLES
	[HideInInspector]
	public float serverHInput = 0f;
	[HideInInspector]
	public float serverVInput = 0f;
	[HideInInspector]
	public float HInput=0;
	[HideInInspector]
	public float VInput=0;

	// MOVEMENT MAIN VARIABLES
	[Range(1.0f,100.0f)][Tooltip("Downward force to apply to player")]
	public float gravity = 10.0f; // gravity acceleration
	[HideInInspector]
	public float lerpSpeed = 10.0f; // smoothing speed
	[HideInInspector]
	public Vector3 curNormal = Vector3.up; // smoothed terrain normal
	[HideInInspector]
	public float rotation=0.0f;
	[HideInInspector]
	public Vector3 myNormal; // character normal
	[HideInInspector]
	public Vector3 surfaceNormal;
	[HideInInspector]
	public bool climbing;
	[HideInInspector]
	public float distGround;
	[HideInInspector]
	public float deltaGround=0.2f;
	[HideInInspector]
	public bool isGrounded;
	[HideInInspector]
	public Transform myTransform;
	[Range(1.0f,100.0f)][Tooltip("Range from player to a wall he can jump to UNUSED")]
	public float jumpRange = 10; 
	[HideInInspector]
	public bool jumping =false;
	[Range(10.0f,100.0f)][Tooltip("Height the player can jump")]
	public float jumpHeight;

	[HideInInspector]
	public bool landing=false;
	[HideInInspector]
	public float prevHeight=0.0f;
	[HideInInspector]
	public bool maxHeightReached=false;
	[Range(1.0f,100.0f)][Tooltip("Distance from ground before landing animation is played")]
	public float landDistance=10.0f;
	//[HideInInspector]
	public float lockMovement=0.0f;
	public float lockTime=1.0f;
}

[Serializable]
public class playerAttack
{
	// BULLET TIMINGS
	public bool canShoot=true;
	[HideInInspector]
	public float OneTime=0.0f;
	[HideInInspector]
	public bool canceled = false;
	[Range(1.0f,200.0f)][Tooltip("Speed of the bullet")]
	public float bulletSpeed; 
	public Vector3 bulletOffset;
	[Range(1.0f,100.0f)][Tooltip("How far to spawn the bullet infront of the player")]
	public int bulletForward;
	public AudioClip pickleClip;
	public AudioClip landMine;
	public AudioClip explosion;
}

[Serializable]
public class strafeAnimation
{

}

[Serializable]
public class movementAnimation
{
	public AnimationClip WalkingAnim;
	public AudioClip WalkingClip;
	[Range(0.01f,5.0f)]
	public float WalkAnimSpeed=1.0f;
	public AnimationClip RunningAnim;
	public AudioClip RunningClip;
	[Range(0.01f,5.0f)]
	public float RunAnimSpeed=1.0f;
	public AnimationClip RunLookBack;
	public AudioClip RunLookBackClip;
	[Range(0.01f,5.0f)]
	public float RunLookBackSpeed=1.0f;
	public AnimationClip Winner;
	public AudioClip WinnerClip;
	[Range(0.01f,5.0f)]
	public float WinnerSpeed=1.0f;
	public AnimationClip Loser;
	public AudioClip LoserClip;
	[Range(0.01f,5.0f)]
	public float LoserSpeed=1.0f;
}

[Serializable]
public class jumpAnimation
{
	public AnimationClip JumpingAnim;
	[Range(0.01f,5.0f)]
	public float JumpAnimSpeed=1.0f;
	public AnimationClip LandingAnim;
	[Range(0.01f,5.0f)]
	public float LandAnimSpeed=1.0f;
	public AnimationClip FallingAnim;
	[Range(0.01f,5.0f)]
	public float FallAnimSpeed=1.0f;
}

[Serializable]
public class playerAnimation
{
	// PLAYER ANIMATIONS
	public AnimationClip IdleAnim;
	[Range(0.01f,5.0f)]
	public float IdleAnimSpeed=1.0f;
	public movementAnimation movementAnim;
	public jumpAnimation jumpAnim;
	public AnimationClip ShootingAnim;
	[Range(0.01f,5.0f)]
	public float ShootAnimSpeed=1.0f;
	public AnimationClip StunnedAnim;
	[Range(0.01f,5.0f)]
	public float StunnedAnimSpeed=1.0f;
	public AnimationClip[] Specials=new AnimationClip[4];
	public float[] SpecialsSpeed=new float[4];
}

[Serializable]
public class playerScripts
{
	// SCRIPTS
	[HideInInspector]
	public bulletPool bulletsScript;
	[HideInInspector]
	public smoothFollow followScript;
	[HideInInspector]
	public SpawnManager spawnScript;
	[HideInInspector]
	public playerManager manager;
}

[Serializable]
public class playerStatistics
{
	// PLAYER STATS VARIABLES
	[Range(1,10)][Tooltip("Starting Health of the Player")]
	public int maxhealth=3;
	[HideInInspector]
	public int health=3;
	[HideInInspector]
	public int killedBy=-1;
}


public abstract class playerBase : Photon.MonoBehaviour {

	public enum attackType
	{
		pickleJar,
		landMine,
		boost,
		stomp
	}

	// THE LOCAL OWNER OF THIS SCRIPT/PLAYER
	[HideInInspector]
	public PhotonPlayer theOwner;
	[HideInInspector]
	public int playerID=-1;
	public AnimationClip stomping;
	public playerMovement movement;
	public playerAttack attack;
	public playerAnimation animations;

	[HideInInspector]
	public playerScripts scripts;
	
	// GAMEOBJECTS
	[HideInInspector]
	public GameObject mainCamera;
	
	// COMPONENTS
	[HideInInspector]
	public CharacterController controller;

	// Terrain Collider
	[HideInInspector]
	public Collider terrainCollider;

	public playerStatistics stats;
	public GameObject model;

	[Tooltip("Mouse Sensitivity on the X Axis")]
	public float sensitivityX = 15F;

	[HideInInspector]
	public float currentRot;
	[HideInInspector]
	public float prevRot;
	[HideInInspector]
	public float updateRot;
	[HideInInspector]
	public float xRot;
	[HideInInspector]
	public bool first=true;

	[Tooltip("Enable/Disable Crossfading Animations")]
	public bool crossfadeAnims=false;
	[HideInInspector]
	public float curDir=90.0f;
	public float stompDistance=0;

	[HideInInspector]
	public int pickleJarCount=0;
	[HideInInspector]
	public int landmineCount=0;
	[HideInInspector]
	public string currentAnimation;
	public AudioSource source;


	public void crossfade(string animationName, bool local)
	{
		//Debug.LogError(animationName);
		if ( crossfadeAnims )
		{
			model.GetComponent<Animation>().CrossFade(animationName,0.2f,PlayMode.StopSameLayer);
		}else{
			model.GetComponent<Animation>().Play(animationName);
		}

		if ( ( MainMenu.gameMode != 0 ) && ( local ) )
		{
			if ( currentAnimation != animationName )
			{
				photonView.RPC("playAnimation",PhotonTargets.OthersBuffered, animationName);
			}
		}

		if ( animationName.Contains("inner") ) 
		{
			scripts.spawnScript.gameStarted=false;

			Invoke("Quit",5f);
		}

		currentAnimation=animationName;
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

	void Quit()
	{
		Application.Quit(); 
	}

			// LOCAL AND NETWORK PLAYER FIRE FUNCTION/RPC
	[PunRPC] 
	public void Shoot(int ent, int type)
	{

		switch(type)
		{

		case (int)attackType.stomp:

			for(int i=0; i < scripts.manager.players.Length; i++)
			{
				//Debug.LogError(gameObject.name + " " + scripts.manager.players[i].name );
				if ( scripts.manager.players[i].name != scripts.manager.players[ent].name )
				{
					if ( Vector3.Distance(scripts.manager.players[i].transform.position, scripts.manager.players[ent].transform.position) < stompDistance )
					{

						Debug.LogError("hit player " + scripts.manager.players[i].name);
						clientPlayer tempScript=scripts.manager.players[i].GetComponent<clientPlayer>();

						tempScript.movement.lockMovement=Time.time;
						// this should be called hit animation
					//	if ( tempScript.animations.ShootingAnim != null )
					//		tempScript.crossfade(tempScript.animations.ShootingAnim.name);

					}
				}else{
					clientPlayer tempScript=scripts.manager.players[ent].GetComponent<clientPlayer>();

					// add this to the animation class
					if ( tempScript.stomping != null )
							tempScript.crossfade(tempScript.stomping.name,true);

				}
			}
		break;

		case (int)attackType.boost:
			movement.speed.boostEnabled=true;
			movement.speed.boostTimer=Time.time;
		break;

		case (int)attackType.pickleJar:


		//if ( ! scripts.bulletsScript )
			scripts.bulletsScript=GameObject.FindGameObjectWithTag("BulletPool").GetComponent<bulletPool>();

		if ( scripts.bulletsScript )
		{
		#if DEBUG_WEAPONS
			Debug.LogError("Creating a bullet");
		#endif
			Vector3 start = Vector3.zero;
			start = scripts.manager.players[ent].transform.position + attack.bulletOffset + (scripts.manager.players[ent].transform.forward*attack.bulletForward) ;

			// set location and velocity of bullet
			scripts.bulletsScript.bullets[scripts.bulletsScript.currentBullet].transform.position=start;

			scripts.bulletsScript.bullets[scripts.bulletsScript.currentBullet].transform.rotation=scripts.manager.players[ent].transform.rotation; // this need to be changed to the guns rotation
			scripts.bulletsScript.bullets[scripts.bulletsScript.currentBullet].GetComponent<Rigidbody>().velocity = scripts.manager.players[ent].transform.forward * attack.bulletSpeed; // adjust bullet speed by player power setting

			//To avoid collision against the shooter itself
			//scripts.bulletsScript.bullets[scripts.bulletsScript.currentBullet].renderer.enabled=false;
			scripts.bulletsScript.bullets[scripts.bulletsScript.currentBullet].GetComponent<Rigidbody>().detectCollisions = false;
			//scripts.bulletsScript.bullets[scripts.bulletsScript.currentBullet].renderer.enabled=true;
			scripts.bulletsScript.bullets[scripts.bulletsScript.currentBullet].GetComponent<Rigidbody>().detectCollisions = true; 
			scripts.bulletsScript.bulletID[scripts.bulletsScript.currentBullet]=ent.ToString();
			scripts.bulletsScript.bullets[scripts.bulletsScript.currentBullet].SetActive(true);
			scripts.bulletsScript.bulletCollisionScript[scripts.bulletsScript.currentBullet].playerID=ent.ToString();
#if DEBUG_WEAPONS
			Debug.LogError("player " + ent.ToString() + " is firing");
#endif
			scripts.bulletsScript.currentBullet++;
			
			if ( scripts.bulletsScript.currentBullet >= scripts.bulletsScript.maxBulletPool )
				scripts.bulletsScript.currentBullet=0;

				pickleJarCount--;

		}
			break;

		case (int)attackType.landMine:
			//if ( ! scripts.bulletsScript )
				scripts.bulletsScript=GameObject.FindGameObjectWithTag("LandMinePool").GetComponent<bulletPool>();
			
			if ( scripts.bulletsScript )
			{
				#if DEBUG_WEAPONS
				Debug.LogError("Creating a bullet");
				#endif
				Vector3 start = Vector3.zero;
				start = scripts.manager.players[ent].transform.TransformPoint( -Vector3.forward * ( scripts.manager.players[ent].GetComponent<Collider>().bounds.extents.z*3.5f )); 
				start.y=0;

				// set location and velocity of bullet
				scripts.bulletsScript.bullets[scripts.bulletsScript.currentBullet].transform.position=start;
				
				//scripts.bulletsScript.bullets[scripts.bulletsScript.currentBullet].transform.rotation=scripts.manager.players[ent].transform.rotation; // this need to be changed to the guns rotation
				//scripts.bulletsScript.bullets[scripts.bulletsScript.currentBullet].rigidbody.velocity = scripts.manager.players[ent].transform.forward * attack.bulletSpeed; // adjust bullet speed by player power setting
				
				//To avoid collision against the shooter itself
				//scripts.bulletsScript.bullets[scripts.bulletsScript.currentBullet].renderer.enabled=false;
				//scripts.bulletsScript.bullets[scripts.bulletsScript.currentBullet].rigidbody.detectCollisions = false;
				//scripts.bulletsScript.bullets[scripts.bulletsScript.currentBullet].renderer.enabled=true;
				//scripts.bulletsScript.bullets[scripts.bulletsScript.currentBullet].rigidbody.detectCollisions = true; 
				scripts.bulletsScript.bulletID[scripts.bulletsScript.currentBullet]=ent.ToString();
				scripts.bulletsScript.bullets[scripts.bulletsScript.currentBullet].SetActive(true);
				scripts.bulletsScript.bulletCollisionScript[scripts.bulletsScript.currentBullet].playerID=ent.ToString();
				#if DEBUG_WEAPONS
				Debug.LogError("player " + ent.ToString() + " is firing");
				#endif
				scripts.bulletsScript.currentBullet++;
				
				if ( scripts.bulletsScript.currentBullet >= scripts.bulletsScript.maxBulletPool )
					scripts.bulletsScript.currentBullet=0;

				landmineCount--;
				
			}
			break;

	}
	}
	


}
