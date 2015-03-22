using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour 
{
	public GameObject deathParticle; // deathParticle of type GameObject. Where the particle system will be dragged to in the inspector
	public Text mineText; //Text to show how many mines the player has. 
	public GameObject trap; // Creates a slot for the trap prefab in the inspector
	
	int mine; //integer for the amount of mines
	float speed; // a float for the speed of the player
	float rotateSpeed; // A float for the rotation speed of the player.
	Vector3 rotateDir; // A Vector3 for rotating the player 
	bool move; //boolean for move. If true, the player can move
	
	AudioSource audio;
	
	
	void Start()
	{
		mine = 0; //Player has no mines at the start
		mineText.text = "Mine:" + mine; //Updates the tex on the UI
		speed = 10f; // Player speed is 10f
		rotateSpeed = 250f; //Rotate speed for player is 250f
		rotateDir = new Vector3(rotateSpeed,0,0); // Rotate direction is set to the x axis
		move = true; // Move is assigned true
		audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(move)
		{
			// float v is assigned vertical input value times the speed times the Time.deltaTime
			float v = Input.GetAxis("Vertical") * speed * Time.deltaTime;
			// float v is assigned horizontal input value times the speed times the Time.deltaTime
			float h = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
			// Moves the player object
			transform.Translate(h,0,v,Space.World);
			//If K is press, player rotates left
			if(Input.GetKey(KeyCode.K))
			{
				transform.Rotate(-rotateDir * Time.deltaTime);
			}
			//If L is pressed, player rotates right
			if(Input.GetKey(KeyCode.L))
			{
				transform.Rotate(rotateDir * Time.deltaTime);
			}
			//If spacebar is pressed,
			if(Input.GetKeyDown(KeyCode.Semicolon))
			{
				if(mine > 0) //if player has more than zero mines
				{
					//Instantiate the trap to the position of where the player is
					Instantiate (trap, transform.position, transform.rotation); 
					mine--; //mine value is reduced by one
					mineText.text = "Mine:" + mine; // The UI text is updated
				}
			}
		}
	}
	
	void OnTriggerEnter(Collider col)
	{
		//If the player collides with the mine pickup.
		if(col.tag == "MinePickup")
		{
			audio.Play();
			mine ++; //Mine is incremented by 1
			// Player can't have more than one mine. Probably should have used a bool.
			if(mine >= 2)
			{
				mine = 1;
			}
			//Updates the amount of mines on the UI
			mineText.text = "Mine:" + mine;
			Destroy(col.gameObject); //Destroys the pickup object
		}
		//If the player touches the boss or a laser
		if(col.tag == "Boss" || col.tag == "Laser")
		{
			//Starts the Death() corouting
			StartCoroutine(Death ());
		}
	}
	
	//The co routine for Death
	IEnumerator Death()
	{
		//Makes player invisible
		GetComponent<MeshRenderer>().enabled = false;
		move = false; //Player can't move when it dies.
		//Instantiates the death particle effect in the same position as the player
		Instantiate(deathParticle, transform.position, Quaternion.identity);
		//Waits for 1.5 seconds
		yield return new WaitForSeconds(1.5f);
		//Reloads the current level
		Application.LoadLevel(Application.loadedLevel);
	}
}
