using UnityEngine;
using System.Collections;

public class bulletPool : Photon.MonoBehaviour {
	
	[HideInInspector]
	public GameObject[] bullets;
	[HideInInspector]
	public bulletCollision[] bulletCollisionScript;
	[HideInInspector]
	public float[] bulletsTtl;
	[HideInInspector]
	public string[] bulletID;
	[HideInInspector]
	public int currentBullet;
	public int maxBulletPool=10;
	public GameObject bullet ;
	public float duration;
	smoothFollow followScript;
	[HideInInspector]
	public int playerID=-1;
	public explosionPool explodePool;
	
	// Use this for initialization
	void Start () 
	{
		currentBullet=0;
		
		bullets = new GameObject[maxBulletPool];
		bulletCollisionScript = new bulletCollision[maxBulletPool];
		bulletsTtl = new float[maxBulletPool];
		bulletID = new string[maxBulletPool];
		
		
        //if (PhotonNetwork.isNonMasterClientInRoom)
        //   enabled = false;
		
		// Generate Bullet and Explosion Object Pool For Each player
		for(int d=0; d < maxBulletPool; d++)
		{
			
			bullets[d] = (GameObject)Instantiate(bullet);
			bullets[d].transform.parent = transform;
			bullets[d].name="bullet" + d;
			bullets[d].SetActive(false);
			bulletsTtl[d]=0.0f;
			bulletCollisionScript[d] = bullets[d].GetComponent<bulletCollision>();
		
		}

		explodePool.init();
		
	}
	
	void Update()
	{
		playerID = PhotonNetwork.player.ID-1;
		
		for (int d=0;d< maxBulletPool;d++)
		{
			if ( bullets[d].GetActive() )
			{
				bulletsTtl[d] += Time.deltaTime;	
				//Debug.LogError(bulletsTtl[d] + stringManager.Instance.bulletPool0 + duration);
				if ( ( ( bulletCollisionScript[d].playerID == playerID.ToString() ) && ( bulletsTtl[d] > 0.2f ) ) || ( ( bulletsTtl[d] > 0.2f ) && ( MainMenu.gameMode == 0 ) && ( bulletCollisionScript[d].playerID == stringManager.Instance.bulletPool1) ) )
				{

					if ( bulletsTtl[d] >= duration )
					{
#if DEBUG_POOLS
						Debug.LogError(stringManager.Instance.bulletPool2);
#endif
						bullets[d].transform.position=new Vector3(0,-1000,0);
						bullets[d].SetActive(false);
						bulletsTtl[d]=0.0f;

						bulletCollisionScript[d].playerID=string.Empty;
					}
				}
			}else{
				bulletsTtl[d]=0.0f;
			}
		}	
	}
}
