using UnityEngine;
using System.Collections;

public class fog : MonoBehaviour {

	bool done=false;

	void Start()
	{
		RenderSettings.fog = !RenderSettings.fog;
	}

	// Use this for initialization
	void Update () 
	{
		if (( GameObject.Find(stringManager.Instance.fog0).GetComponent<SpawnManager>().gameStarted ) && ( ! done ) )
		{
			RenderSettings.fog = !RenderSettings.fog;

			if ( RenderSettings.fog == true )
			{
			    if( RenderSettings.fogMode == FogMode.Linear ) {
			    	GetComponent<Camera>().farClipPlane = RenderSettings.fogEndDistance;
			    } else if( RenderSettings.fogMode == FogMode.Exponential ) {
			    	GetComponent<Camera>().farClipPlane = Mathf.Log( 1f / 0.0019f ) / RenderSettings.fogDensity;
			    } else if( RenderSettings.fogMode == FogMode.ExponentialSquared ) {
			    	GetComponent<Camera>().farClipPlane = Mathf.Sqrt( Mathf.Log( 1f / 0.0019f ) ) / RenderSettings.fogDensity;
			    }
				
			#if DEBUG_FOG
				Debug.LogError(stringManager.Instance.fog1 + camera.farClipPlane);
			#endif
			}
			done=true;
		}
	}

}
