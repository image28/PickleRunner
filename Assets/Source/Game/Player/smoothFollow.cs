using UnityEngine;
using System.Collections;

public class smoothFollow : Photon.MonoBehaviour {

/*
This camera smoothes out rotation around the y-axis and height.
Horizontal Distance to the target is always fixed.

There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

For every of those smoothed values we calculate the wanted value and the current value.
Then we smooth it using the Lerp function.
Then we apply the smoothed values to the transform's position.
*/
	
	/*public enum camCurrentState
	{
		shipFlying,
		shipLanding,
		playerUnloading,
		followPlayer
	};*/

	public float sensitivityX = 5f;
	public float sensitivityY = 5f;

	public float minimumX = -360f;
	public float maximumX = 360f;

	public float minimumY = -75f;
	public float maximumY = -10f;

	private float rotationY = 0f;
	private float rotationX = 0f;

	/*
	public camCurrentState camState=camCurrentState.shipFlying ;
	camCurrentState currentFollow;
	*/

	// The target we are following
	Transform target ;
	// The distance in the x-z plane to the target
	public float distance = 10.0f;
	// the height we want the camera to be above the target
	public float height = 5.0f;
	// How much we 
	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;
	public GameObject follow;
	public string playerID;
	
	[HideInInspector]
	public float startHeightDamping;
	[HideInInspector]
	public float startRotationDamping;
	[HideInInspector]
	public float startDistance;
	[HideInInspector]
	public float startHeight;
	private SpawnManager spawnScript;
	public bool cameraCollisions=true;

	/*
	private float lerpProgress=0.0f;
	private float lerpSpeed=0.0f;
	private bool lerpFirst=true;
	public bool skipStart=false;
	*/

	void Start()
	{
		startHeight=height;
		startDistance=distance;
		startHeightDamping=heightDamping;
		startRotationDamping=rotationDamping;
		spawnScript =  GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
	}


	void LateUpdate () {
	// Early out if we don't have a target
	
		if ( spawnScript.gameStarted )
		{
			
			/*switch(camState)
			{
				case camCurrentState.shipFlying:
					if ( currentFollow != camCurrentState.shipFlying )
					{
						if ( ! follow )
							follow=null;
					}
				
					if ( ! follow )
					{
						if ( MainMenu.gameMode != 0 )
						{
							int ID = PhotonNetwork.player.ID - 1;
							follow = GameObject.Find(stringManager.Instance.smoothFollow0 + ID.ToString());
						}else{
							follow = GameObject.Find(stringManager.Instance.smoothFollow1);
						}
						if ( ! follow )
							return;
					
						currentFollow=camCurrentState.shipFlying;
						target=follow.transform;
					}
						
					// if ship is above landing point.... switch camState to ship landing
					
				break;
				
				case camCurrentState.shipLanding:
					if ( currentFollow != camCurrentState.shipLanding )
					{
				
						if ( ! follow )
						{
							if ( MainMenu.gameMode != 0 )
							{
								int ID = PhotonNetwork.player.ID - 1;
								follow = GameObject.Find(stringManager.Instance.smoothFollow2 + ID.ToString());
							}else{
								follow = GameObject.Find(stringManager.Instance.smoothFollow3);
							}
							if ( ! follow )
								return;

						}
					
						currentFollow=camCurrentState.shipLanding;
					}
				
					// if factory dropped and landed switch camState to playerUnloading
				break;
				
				case camCurrentState.playerUnloading:
					if ( currentFollow != camCurrentState.playerUnloading )
					{
				
						if ( ! follow )
						{
							if ( MainMenu.gameMode != 0 )
							{
								int ID = PhotonNetwork.player.ID - 1;
								follow = GameObject.Find(stringManager.Instance.smoothFollow4 + ID.ToString());
							}else{
								follow = GameObject.Find(stringManager.Instance.smoothFollow5);
							}
							if ( ! follow )
								return;

						}
					
						currentFollow=camCurrentState.playerUnloading;
					}
				
					// if player fully unloaded switch camState to followPlayer
				break;
				
				case camCurrentState.followPlayer:*/
					if ( ! follow )
					{
						if ( MainMenu.gameMode != 0 )
						{
							int ID = PhotonNetwork.player.ID - 1;
							follow = GameObject.Find(stringManager.Instance.smoothFollow6 + ID.ToString());
						}else{
							follow = GameObject.Find(stringManager.Instance.smoothFollow7);
						}
						if ( ! follow )
							return;
					}
						
					target=follow.transform;
			/*	break;
				
				
			}*/
			
			// Calculate the current rotation angles
			var wantedRotationAngle = target.eulerAngles.y;
			var wantedHeight = target.position.y + height;
				
			var currentRotationAngle = transform.eulerAngles.y;
			var currentHeight = transform.position.y;
			
			// Damp the rotation around the y-axis
			currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		
			// Damp the height
			currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		
#if UNITY_STANDALONE

			float rotationX = transform.localEulerAngles.y + Input.GetAxis(stringManager.Instance.smoothFollow8) * sensitivityX;
			
			rotationX += Input.GetAxis(stringManager.Instance.smoothFollow9) * sensitivityX; //transform.localEulerAngles.y + 
			rotationX = Mathf.Clamp ( rotationX,minimumX, maximumX);
			rotationY += Input.GetAxis(stringManager.Instance.smoothFollow10) * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

			/*Vector3 pos = transform.position;
			Ray ray;
			RaycastHit hit;

			ray = new Ray(transform.position, -Vector3.up); // cast ray downwards
			if (Physics.Raycast(ray, out hit)){
				
				if (( hit.distance > 0.0f ) && ( hit.distance < 3.0f ))
				{
					rotationY = pos.y+hit.distance+2.0f;
				}else{
					ray = new Ray(transform.position, Vector3.up); // cast ray downwards
					if (Physics.Raycast(ray, out hit)){
						
						if ( hit.distance > 0.0f )
						{
							pos.y=pos.y+hit.distance+3.0f;
							rotationY = pos.y;
						}
					}
				}
			}*/





			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);

			currentRotationAngle=transform.eulerAngles.y;
			
			// Convert the angle into a rotation
			var currentRotation = Quaternion.Euler (-rotationY, currentRotationAngle, 0);
			
			// Set the position of the camera on the x-z plane to:
			// distance meters behind the target
			Vector3 tempPos = target.position;
			tempPos.y = tempPos.y + height;
			tempPos = tempPos - (currentRotation * Vector3.forward * distance);
			transform.position = tempPos;
#else
			var currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);

			Vector3 tempPos = target.position - (currentRotation * Vector3.forward * distance);
			// Set the height of the camera
			tempPos.y=currentHeight;
#endif
			
		/*	if ( ( camState == camCurrentState.followPlayer ) && ( lerpProgress <= 0.8f ) )
			{
				if ( lerpFirst )
				{
					lerpSpeed=10.0f/Vector3.Distance(transform.position,tempPos);
					lerpFirst=false;
				}
				if ( skipStart )
					lerpProgress=1.0f;
				else
					lerpProgress=lerpProgress+lerpSpeed*Time.deltaTime;
				//Debug.LogError(lerpProgress);
				transform.position = Vector3.Lerp(transform.position,tempPos,lerpProgress);
			}else{*/
				transform.position = tempPos;
			//}


			if ( cameraCollisions )
			{
				Vector3 position = transform.position;
				RaycastHit hit;
				int layermask;

				layermask=1<<11;
				layermask = ~layermask;


				//now we trace a ray downward from camera
				if(Physics.Raycast(position,transform.TransformDirection(-Vector3.up),out hit))
				{ // if camera position is very near to a collider like terrain or i.e a building we reposition camera to afew top of it
					//Debug.Log(stringManager.Instance.smoothFollow11+transform.TransformDirection(-Vector3.up)+stringManager.Instance.smoothFollow12+hit.point+stringManager.Instance.smoothFollow13+hit.point.y+0.2f);
					if(Vector3.Distance(position,hit.point)<=2.0f)
					{
						position=new Vector3(hit.point.x ,hit.point.y+2.0f,hit.point.z);
					}
				}
				else if(Physics.Raycast(position,transform.TransformDirection(Vector3.up),out hit))
				{
					position=new Vector3(hit.point.x,hit.point.y+2.0f, hit.point.z);
					//Debug.Log(stringManager.Instance.smoothFollow14+transform.TransformDirection(Vector3.up)+stringManager.Instance.smoothFollow15);
				}

				if(Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward),out hit,0.5f))
				{
					position=new Vector3(position.x ,hit.point.y+2.0f,position.z);
				}

				if(Physics.Linecast(target.position,position,out hit,layermask))
				{
					if ( hit.collider.gameObject.tag != stringManager.Instance.smoothFollow16 )
					{
						position=new Vector3(hit.point.x,hit.point.y+2.0f,hit.point.z); //-cameraavoidcollider
					//	Debug.Log(hit.collider.gameObject.name);
					}
				}
				
				Vector3 reference = Vector3.zero;
				transform.position = position; //Vector3.SmoothDamp( transform.position, position,ref reference , 0.5f);
			}
	
			// Always look at the target
			transform.LookAt (target);
		}
	}
}
