using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour 
{

	//public float speed is set to 5. Can be changed in the inspector
	public float speed = 5f;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Moves the laser in the z direction times the speed times Time.deltaTime
		transform.Translate(0,0, speed * Time.deltaTime);
	}
	
	//When the laser leaves the view of any camera on the screen.
	void OnBecameInvisible()
	{
		//Debug.Log ("Laser Disappear");
		//Destory this laser gameobject.
		Destroy(gameObject);
	}
}
