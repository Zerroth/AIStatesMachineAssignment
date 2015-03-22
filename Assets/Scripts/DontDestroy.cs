using UnityEngine;
using System.Collections;

public class DontDestroy : MonoBehaviour 
{
	//Declares a static variable instanceREf of type DontDestroy
	private static DontDestroy instanceRef;
	
	void Awake()
	{
		//if instanceRef does not exist
		if(instanceRef == null)
		{
			instanceRef = this; //Make this gameObject instanceRef
			DontDestroyOnLoad(gameObject); //Dont destroy this object when a new scene loads
		}
		else
		{
			//This gameObject is destroyed if one already exists
			//Prevents another one from appearing when the level is reloaded.
			DestroyImmediate(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
