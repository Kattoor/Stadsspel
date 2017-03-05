using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Transform CameraBoundsTransform;
  public Transform CameraM;
	public Vector2 Margin, Smoothing;
	public BoxCollider2D cameraBounds;
	public BoxCollider2D cameraCollider;
	private Vector3 _min, _max;
	public bool Isfollowing {get; set;}
	
	public void Start () 
	{
    // If map is fixed
		_min = cameraBounds.bounds.min;
		_max = cameraBounds.bounds.max;
		Isfollowing = true;
	}

	public void Update () 
	{
    //_min = cameraBounds.bounds.min;
    //_max = cameraBounds.bounds.max;


    
		var x = transform.position.x;
		var y = transform.position.y;

		if (Isfollowing) 
		{
			if(Mathf.Abs(x - CameraM.position.x) > Margin.x)
				x = Mathf.Lerp(x, CameraM.position.x, Smoothing.x * Time.deltaTime);
			if(Mathf.Abs(y - CameraM.position.y) > Margin.y)
				y = Mathf.Lerp(y, CameraM.position.y, Smoothing.y * Time.deltaTime);
		}

		var cameraHalfWidth = GetComponent<Camera>().orthographicSize * ((float)Screen.width / Screen.height);
		x = Mathf.Clamp (x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);
		y = Mathf.Clamp (y, _min.y + GetComponent<Camera>().orthographicSize, _max.y - GetComponent<Camera>().orthographicSize);

		transform.position = new Vector3 (x, y, transform.position.z);
    

		// If endless
		//Vector2 boundsPosition = new Vector2(Character.position.x, CameraBoundsTransform.transform.position.y);
		//CameraBoundsTransform.position = boundsPosition;
	}
}
