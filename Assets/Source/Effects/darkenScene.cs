using UnityEngine;
using System.Collections;

public class darkenScene : MonoBehaviour {

	public GameObject sceneRoot;
	public Color darkenColor;
	private Color fadeOut;
	private Color currentColor;
	private Color startColor;
	public float smoothness = 0.02f;
	public float duration=5.0f;
	private bool fadeIn=false;
	public GameObject clouds;
	public bool addClouds;

	public void Start()
	{
	}

	public void runEffect()
	{
		if ( fadeIn )
		{
			fadeIn=false;
			
			startColor=darkenColor;
			currentColor=darkenColor;
			fadeOut=Color.white;

			if ( ( addClouds ) && ( clouds != null ) )
			{
				clouds.SetActive(false);
			}
		}else{
			fadeIn=true;
			
			startColor=Color.white;
			currentColor=Color.white;
			fadeOut=darkenColor;

			if ( ( addClouds ) && ( clouds != null ) )
			{
				clouds.SetActive(true);
			}
		}

		if ( gameObject.activeSelf )
			StartCoroutine("fader");
	}

	IEnumerator fader()
	{
		float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
		float increment = smoothness/duration; //The amount of change to apply.

		
		while(progress < 1)
		{
			currentColor = Color.Lerp(startColor, fadeOut, progress);
			darkenChildren(sceneRoot,fadeIn);
			progress += increment;
			yield return new WaitForSeconds(smoothness);
		}
	}

	void darkenChildren(GameObject current, bool dark)
	{
		if ( current.GetComponent<SpriteRenderer>() != null )
		{
			SpriteRenderer rend=(SpriteRenderer)current.GetComponent<SpriteRenderer>();
			if ( dark )
			{
				current.GetComponent<SpriteRenderer>().material.color=currentColor;
			}else{
				current.GetComponent<SpriteRenderer>().material.color=currentColor;
			}
		}

		if ( current.transform.childCount > 0 )
		{
			foreach( Transform child in current.transform)
			{
				darkenChildren(child.gameObject,dark);
			}
		}
	}
}
