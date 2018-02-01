using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	public static int gameMode;
		
	void Start()
	{
		Screen.lockCursor = false;
		gameMode=1;	
	}
}
