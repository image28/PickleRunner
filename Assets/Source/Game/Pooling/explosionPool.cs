using UnityEngine;
using System.Collections;

public class explosionPool : MonoBehaviour {
	
	public GameObject[] explosions;

	public explosionCollision[] explosionScript;
	public GameObject explosion;
	public int currentExplosion;
	public int maxExplosionPool=10;
	public float duration=5.0f;
	private float[] explosionsTtl;
	[HideInInspector]
	public string playerID;
	[HideInInspector]
	public bool ready=false;
	
	// Use this for initialization
	public void init() 
	{
		explosions = new GameObject[maxExplosionPool];
		explosionsTtl = new float[maxExplosionPool];
		explosionScript = new explosionCollision[maxExplosionPool];
		currentExplosion=0;
		
		for(int e=0; e < maxExplosionPool; e++)
		{
			explosions[e]=(GameObject)Instantiate(explosion);
			explosions[e].transform.parent = GameObject.Find("ExplosionPool").transform;
			explosions[e].name="explosion" + e;
		#if DEBUG_POOLS
			Debug.LogError(explosions[e] + " Created");
		#endif
			explosionsTtl[e]=0.0f;
			explosionScript[e]=explosions[e].GetComponent<explosionCollision>();
			explosionScript[e].ttl=explosionsTtl[e];
			explosions[e].SetActive(false);
			Debug.LogError(explosions[e].name);
		}
	
		ready=true;
	}
	
	void Update()
	{
		if ( ready )
		{
			for (int d=0;d< maxExplosionPool;d++)
			{
				if ( explosions[d].GetActive() )
				{

					explosionsTtl[d] += Time.deltaTime;	
					explosionScript[d].ttl=explosionsTtl[d];
					if ( explosionsTtl[d] >= duration )
					{
	#if DEBUG_POOLS
						Debug.LogError(stringManager.Instance.explosionPool0);
	#endif
						explosions[d].transform.position=new Vector3(0,-1000,0);
						explosions[d].transform.rotation=new Quaternion(0,0,0,0);
						explosions[d].GetComponent<Detonator>().Reset();
						explosions[d].SetActive(false);
						explosionsTtl[d]=0.0f;
						explosionScript[d].hit=false;
					}
				}else{
					//explosions[d].transform.position=new Vector3(0,-1000,0);
					//explosions[d].GetComponent<Detonator>().Reset();
					explosionsTtl[d]=0.0f;
					explosionScript[d].ttl=explosionsTtl[d];
					explosionScript[d].hit=false;
				}
			}
		}
	}

	
}
