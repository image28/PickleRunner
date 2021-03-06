using UnityEngine;
using System.Collections;

public class ProgressBar : MonoBehaviour {
	
	private Texture2D map;
	private Color[] array;
	private Color[] filler;

	public int maxHeight;
	public int maxWidth;
//	int progress=0;
//	int lastprogress=0;
	
	// Use this for initialization
    void Start() {
		/*if ( MainMenu.gameMode != 0 )
		{*/
			//PhotonNetwork.SetLevelPrefix(10);
			//PhotonNetwork.isMessageQueueRunning=false;
			GameObject.Find("preSpawn").GetComponent<connectBuffer>().gameLoading=true;

		//}
		
		map = new Texture2D(maxWidth,maxHeight);
		array = new Color[maxWidth*maxHeight];
		filler = new Color[maxWidth*maxHeight];
		
		for (int i = 0; i < array.Length; i++) 
		{
			array[i] = Color.black;
			filler[i] = Color.green;
		}
		
		map.SetPixels(0, 0, maxWidth, maxHeight, array);
		map.Apply();
		
		gameObject.GetComponent<GUITexture>().texture=map;
		StartCoroutine(loadLevel());
	}
	
	IEnumerator loadLevel()
	{
		AsyncOperation async = Application.LoadLevelAsync("Game");
        while (!async.isDone) 
		{
			int progress=(int)((float)((float)maxWidth/100)*((int)(async.progress*100)));
			map.SetPixels(0,0, progress, maxHeight,filler);

			map.Apply();
	    	yield return(0);
	    }
        Debug.Log("Loading complete");
		PhotonNetwork.isMessageQueueRunning=false;
	}
}
