using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class trackReader : MonoBehaviour {

	private List<Vector3> points;
	private LineRenderer lineRenderer;
	public GameObject[] pickups;
	public float[] frequency;
	private float prevDist=0;

	// Use this for initialization
	void Start () 
	{
		lineRenderer = GetComponent<LineRenderer>();
		points = new List<Vector3>();

		loadPath();
		drawPath();
	}

	void loadPath()
	{
		GameObject path = (GameObject)GameObject.FindGameObjectWithTag("Path");

		if ( path != null )
		{
			int total=path.transform.childCount;

			for(int d=0; d< total; d++)
			{
				points.Add(path.transform.Find("Waypoint" + d).transform.position);
			}
		}
	
	}

	void drawPath()
	{
		BezierPath bezierPath = new BezierPath();
		bezierPath.Interpolate(points, .25f);
		
		List<Vector3> drawingPoints = bezierPath.GetDrawingPoints2();
	//	placeObjects(drawingPoints);
	//	gizmos = bezierPath.GetControlPoints(); 
		
		SetLinePoints(drawingPoints);
	}

	void placeObjects(List<Vector3> drawingPoints)
	{
		int current=0;
		foreach(GameObject pickup in pickups)
		{

			GameObject pickupParent = new GameObject();
			pickupParent.name=pickup.name+"Root";
			pickupParent.transform.position=Vector3.zero;
			int count=0;


			for(int d=0; d<drawingPoints.Count; d++)
			{
				if ( d+1 < drawingPoints.Count )
				{
					if ( Vector3.Distance(drawingPoints[d],drawingPoints[d+1]) < frequency[current] )
					{
						prevDist=prevDist+Vector3.Distance(drawingPoints[d],drawingPoints[d+1]);

						int tempPos=distanceJoin(drawingPoints,d+1, drawingPoints.Count,current );

						if ( tempPos != 0 )
						{
							for(int e=1; e < (prevDist/frequency[current]);e++)
							{
								Vector3 temp=Vector3.Lerp(drawingPoints[d],drawingPoints[tempPos],(1.0f/(prevDist/frequency[current])*e));
								GameObject place =(GameObject)Instantiate(pickup, new Vector3(temp.x,1000,temp.z), pickup.transform.rotation);
								place.transform.eulerAngles= new Vector3(0,UnityEngine.Random.Range(0.0f,120.0f),0);
								place.name=pickup.name + count.ToString();
								place.transform.parent=pickupParent.transform;
								//place.GetComponent<coin>().Ground();
								count++;
							}

							prevDist=0;
							d=tempPos;
						}else{
							prevDist=0;
							d=drawingPoints.Count;
						}
					}else{
						prevDist=0;
						for(int e=1; e < (Vector3.Distance(drawingPoints[d],drawingPoints[d+1])/frequency[current]);e++)
						{
							Vector3 temp=Vector3.Lerp(drawingPoints[d],drawingPoints[d+1],(1.0f/(Vector3.Distance(drawingPoints[d],drawingPoints[d+1])/frequency[current])*e));
							GameObject place =(GameObject)Instantiate(pickup, new Vector3(temp.x,1000,temp.z), pickup.transform.rotation);
							place.transform.eulerAngles= new Vector3(0,UnityEngine.Random.Range(0.0f,120.0f),0);
							place.name=pickup + count.ToString();
							place.transform.parent=pickupParent.transform;
							//place.GetComponent<coin>().Ground();
							count++;
						}
					}
				}
			}
			current++;
		}
	}

	int distanceJoin(List<Vector3> drawingPoints, int arrayPos, int total, int current)
	{
		if ( arrayPos < total-1 )
		{
			prevDist=prevDist+Vector3.Distance(drawingPoints[arrayPos],drawingPoints[arrayPos+1]);
			if ( prevDist < frequency[current] )
			{
				arrayPos=distanceJoin(drawingPoints,arrayPos+1,total,current);
			}else{
				arrayPos++;
			}

			return(arrayPos);
		}else{
			return(0);
		}
	}

	private void SetLinePoints(List<Vector3> drawingPoints)
	{
		lineRenderer.SetVertexCount(drawingPoints.Count);
		
		for (int i = 0; i < drawingPoints.Count; i++)
		{
			lineRenderer.SetPosition(i, drawingPoints[i]);
		}
	}

	/*public void OnDrawGizmos()
	{
		if (gizmos == null)
		{
			return;
		}        
		
		for (int i = 0; i < gizmos.Count; i++)
		{
			Gizmos.DrawWireSphere(gizmos[i], 1f);            
		}
	}*/
}
