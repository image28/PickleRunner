using UnityEngine;
using System.Collections;

[System.Serializable]
public class pulse
{
	public bool enabled=false;
	[HideInInspector]
	public bool fade=false;
	public Color startColor=Color.white;
	public Color endColor=Color.white;
	[HideInInspector]
	public Color currentColor=Color.white;
	[HideInInspector]
	public Color fadeIn=Color.white;
	[HideInInspector]
	public Color fadeOut=Color.white;
	[HideInInspector]
	public float progress=0;
}

[System.Serializable]
public class zoom
{
	public bool enabled=false;
	[HideInInspector]
	public bool zooming=false;
	public Vector3 startScale=new Vector3(1.0f,1.0f,1.0f);
	public Vector3 endScale=new Vector3(1.0f,1.0f,1.0f);
	[HideInInspector]
	public Vector3 currentScale=new Vector3(1.0f,1.0f,1.0f);
	[HideInInspector]
	public Vector3 zoomIn=new Vector3(1.0f,1.0f,1.0f);
	[HideInInspector]
	public Vector3 zoomOut=new Vector3(1.0f,1.0f,1.0f);
	[HideInInspector]
	public float progress=0;
}

[System.Serializable]
public class rot
{
	public bool enabled=false;
	[HideInInspector]
	public bool roting=false;
	public Vector3 startRot=new Vector3(1.0f,1.0f,1.0f);
	public Vector3 endRot=new Vector3(1.0f,1.0f,1.0f);
	[HideInInspector]
	public Vector3 currentRot=new Vector3(1.0f,1.0f,1.0f);
	[HideInInspector]
	public Vector3 rotIn=new Vector3(1.0f,1.0f,1.0f);
	[HideInInspector]
	public Vector3 rotOut=new Vector3(1.0f,1.0f,1.0f);
	[HideInInspector]
	public float progress=0;
}



// Class to handle text notifications and Animation effects for the text
public class textNotifications : MonoBehaviour {
	public bool showEffects=true;
	[Range(0.1f,10.0f)]
	public float displayTime=2.0f;
	[Range(0.0f,2.0f)]
	public float effectSpeed=0.2f;
	public float smoothness=0.1f;
	private float currentTime=0.0f;
	private bool finalEffect=false;
	private bool progress=false;
	public pulse pulseEffect;
	public zoom zoomEffect;
	public rot rotEffect;

	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().enabled=false;
	}

	public void Update()
	{
		if ( finalEffect )
		{
			if ( ( zoomEffect.progress == 0 ) && ( pulseEffect.progress == 0 ) && ( rotEffect.progress == 0 ) )
			{
				progress=false;
			}

			if ( ! progress ) 
			{
				showEffects=false;
				GetComponent<Renderer>().enabled=false;
				finalEffect=false;
				currentTime=0.0f;
				progress=false;
				transform.eulerAngles=Vector3.zero;
			}
		}
		else if ( showEffects )
		{
			if ( currentTime == 0.0f )
			{
				currentTime=Time.time;
			}

			GetComponent<Renderer>().enabled=true;
			if ( ( zoomEffect.progress == 0 ) && ( pulseEffect.progress == 0 ) && ( rotEffect.progress == 0 ) )
			{
				progress=true;

				runEffect();

				if ( Time.time > currentTime+displayTime )
				{
					finalEffect=true;
				}
			}


		}else{
			GetComponent<Renderer>().enabled=false;
		}
	}

	public void runEffect()
	{
	
		// pulse in/out effect
		if ( pulseEffect.enabled )
		{
			if ( pulseEffect.fade )
			{
				pulseEffect.fade=false;
				
				pulseEffect.fadeIn=pulseEffect.endColor;
				pulseEffect.currentColor=pulseEffect.endColor;
				pulseEffect.fadeOut=pulseEffect.startColor;
			}else{
				pulseEffect.fade=true;
				
				pulseEffect.fadeIn=pulseEffect.startColor;
				pulseEffect.currentColor=pulseEffect.startColor;
				pulseEffect.fadeOut=pulseEffect.endColor;
			}
			
			StartCoroutine(stringManager.Instance.textNotifications0);
		}

		// Zoom in/out effect
		if ( zoomEffect.enabled )
		{
			if ( zoomEffect.zooming )
			{
				zoomEffect.zooming=false;
				
				zoomEffect.zoomIn=zoomEffect.endScale;
				zoomEffect.currentScale=zoomEffect.endScale;
				zoomEffect.zoomOut=zoomEffect.startScale;
			}else{
				zoomEffect.zooming=true;
				
				zoomEffect.zoomIn=zoomEffect.startScale;
				zoomEffect.currentScale=zoomEffect.startScale;
				zoomEffect.zoomOut=zoomEffect.endScale;
			}
			
			StartCoroutine(stringManager.Instance.textNotifications1);
		}

		// Rotation in/out effect
		if ( rotEffect.enabled )
		{
			if ( rotEffect.roting )
			{
				rotEffect.roting=false;
				
				rotEffect.rotIn=rotEffect.endRot;
				rotEffect.currentRot=rotEffect.endRot;
				rotEffect.rotOut=rotEffect.startRot;
			}else{
				rotEffect.roting=true;
				
				rotEffect.rotIn=rotEffect.startRot;
				rotEffect.currentRot=rotEffect.startRot;
				rotEffect.rotOut=rotEffect.endRot;
			}
			
			StartCoroutine(stringManager.Instance.textNotifications2);
		}
		
		

	}
	
	IEnumerator fader()
	{
		float increment = smoothness/effectSpeed; //The amount of change to apply.
		
		
		while(pulseEffect.progress < 1)
		{
			pulseEffect.currentColor = Color.Lerp(pulseEffect.fadeIn, pulseEffect.fadeOut, pulseEffect.progress);
			
			if ( ( pulseEffect.fadeOut.a == 0.0f ) && ( pulseEffect.currentColor.a < pulseEffect.fadeOut.a+0.1f ) )
			{
				pulseEffect.currentColor.a=0.0f;
			}
			
			if ( gameObject.GetComponent<SpriteRenderer>() != null )
			{
				SpriteRenderer rend=(SpriteRenderer)gameObject.GetComponent<SpriteRenderer>();
				rend.material.SetFloat(stringManager.Instance.textNotifications3,pulseEffect.currentColor.a);		
				rend.color=pulseEffect.currentColor;
			}

			pulseEffect.progress += increment;
			yield return new WaitForSeconds(smoothness);
		}

		pulseEffect.progress=0;
		
	}

	IEnumerator zoomer()
	{
		float increment = smoothness/effectSpeed; //The amount of change to apply.
		
		
		while(zoomEffect.progress < 1)
		{
			zoomEffect.currentScale = Vector3.Lerp(zoomEffect.zoomIn, zoomEffect.zoomOut, zoomEffect.progress);
			
			if ( ( zoomEffect.zoomOut == Vector3.zero) && ( Vector3.Distance(zoomEffect.zoomOut,zoomEffect.currentScale) < 0.1f ) )
			{
				zoomEffect.currentScale=Vector3.zero;
			}
			
			gameObject.transform.localScale=zoomEffect.currentScale;
			zoomEffect.progress += increment;
			yield return new WaitForSeconds(smoothness);
		}

		zoomEffect.progress=0;
		
	}

	IEnumerator spin()
	{
		float increment = smoothness/effectSpeed; //The amount of change to apply.
		
		
		while(rotEffect.progress < 1)
		{
			rotEffect.currentRot = Vector3.Lerp(rotEffect.rotIn, rotEffect.rotOut, rotEffect.progress);
			
			if ( ( rotEffect.rotOut == Vector3.zero) && ( Vector3.Distance(rotEffect.rotOut,rotEffect.currentRot) < 0.1f ) )
			{
				rotEffect.currentRot=Vector3.zero;
			}
			
			gameObject.transform.Rotate(rotEffect.currentRot);
			rotEffect.progress += increment;
			yield return new WaitForSeconds(smoothness);
		}
		
		rotEffect.progress=0;
		
	}


}
