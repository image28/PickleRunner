using UnityEngine;
using System.Collections;



public class playMusic : MonoBehaviour {

	public AudioClip[] clips =new AudioClip[4];
	public AudioSource oneShot;

	// Use this for initialization
	void Start () {
		oneShot.clip=clips[System.Convert.ToInt32(transform.parent.gameObject.name.Replace("Player",""))];
		oneShot.Play();
	}
	
	// Update is called once per frame
	void Update () {

	
	}
}
