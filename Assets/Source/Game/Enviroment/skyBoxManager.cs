using UnityEngine;
using System.Collections;

public class skyBoxManager : MonoBehaviour {
	
	// Skybox rotation variables
	public Transform gameCamera;
	[Range(0.1f,100.0f)]
	public float rotateSpeed=0.1f;
	private float currentY=0.0f;
	
	// Day/Night Switch
	public bool night=false;
	private bool currentlyDay=true;
	
	// Light Fading Variables
	public Light LightObject;
	public float dayLight;
	public float nightLight;
	private float currentLight;

	// Skybox Fading Variables
	public Material daySkybox;
	public Material nightSkybox;
	public Color startColor;
	public Color darkenColor;
	private Color currentColor;
	private Skybox script;

	// Fading Speed Variables
	public float smoothness = 0.02f;
	public float duration=5.0f;

	// Use this for initialization
	void Start () {
		script=gameObject.GetComponent<Skybox>();
		daySkybox.SetColor("_Color",startColor);
		nightSkybox.SetColor("_Color",darkenColor);
		LightObject.intensity=dayLight;
	}
	
	// Update is called once per frame
	void Update () {
		/*Vector3 temp = new Vector3(0,0,0);
		if ( gameCamera.eulerAngles.x >= 0 )
			temp.x=temp.x-gameCamera.eulerAngles.x;
		else
			temp.x=temp.x+Mathf.Abs(gameCamera.eulerAngles.x);

		if ( gameCamera.eulerAngles.y >= 0 )
			temp.y=currentY-gameCamera.eulerAngles.y;
		else
			temp.y=currentY+Mathf.Abs(gameCamera.eulerAngles.y);

		if ( gameCamera.eulerAngles.z >= 0 )
			temp.z=temp.z-gameCamera.eulerAngles.z;
		else
			temp.z=temp.z+Mathf.Abs(gameCamera.eulerAngles.z);

		transform.eulerAngles=temp;*/

		if ( currentY > 360 )
			currentY=0;
		else
			currentY=currentY+rotateSpeed*Time.deltaTime;

		transform.eulerAngles=new Vector3(gameCamera.eulerAngles.x,gameCamera.eulerAngles.y+(currentY),gameCamera.eulerAngles.z);


		// if it is daytime, and night has been triggered
		if ( ( currentlyDay ) && ( night ) )
		{
			// fade out the day
			currentlyDay=false;
			StartCoroutine(stringManager.Instance.skyBoxManager0);
			StartCoroutine(stringManager.Instance.skyBoxManager1);
		}

		// if it is nighttime, and day has been triggered
		if ( ( ! currentlyDay ) && ( ! night ) )
		{
			// fade out the night
			currentlyDay=true;
			StartCoroutine(stringManager.Instance.skyBoxManager2);
			StartCoroutine(stringManager.Instance.skyBoxManager3);
		}


	}

	IEnumerator fadeLight()
	{
		float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
		float increment = (smoothness/duration)/2; //The amount of change to apply.

		while(progress < 1)
		{
			if ( ( currentlyDay ) && ( ! night ) )
			{
				currentLight = Mathf.Lerp(nightLight, dayLight, progress);
			}else{
				currentLight = Mathf.Lerp (dayLight, nightLight, progress);
			}

			LightObject.intensity=currentLight;

			progress += increment;
			yield return new WaitForSeconds(smoothness/2);
		}



	}

	IEnumerator fadeSkybox()
	{
		float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
		float increment = smoothness/duration; //The amount of change to apply.

		script.material.SetColor("_Color",startColor);
		currentColor=startColor;

		while(progress < 1)
		{
			currentColor = Color.Lerp(startColor, darkenColor, progress);
			
			if ( ( currentColor.a < darkenColor.a+0.1f ) )
			{
				currentColor=darkenColor;
			}
			
			script.material.SetColor("_Color",currentColor);
			progress += increment;
			yield return new WaitForSeconds(smoothness);
		}

		progress=0;

		currentColor=darkenColor;

		if ( ( currentlyDay ) && ( ! night ) )
		{
			daySkybox.SetColor("_Color",darkenColor);
			script.material=daySkybox;
		}else{
			nightSkybox.SetColor("_Color",darkenColor);
			script.material=nightSkybox;
		}
		script.material.SetColor("_Color",darkenColor);

		while(progress < 1)
		{
			currentColor = Color.Lerp(darkenColor, startColor, progress);
			
			if ( ( currentColor.a > startColor.a-0.1f ) )
			{
				currentColor=startColor;
			}
			
			script.material.SetColor("_Color",currentColor);
			progress += increment;
			yield return new WaitForSeconds(smoothness);
		}

	}

}
