using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; //This is needed to access the UI through the script

public class Boss : MonoBehaviour 
{
	Transform boss,player; 			//Boss and PLayer are of type transform
	float hurtTimer; 				//Declaration of the variable hurtTimer of type float
	float dropTimer;				//Declaration of the variable dropTimer of type float
	float shootTimer;				//Declaration of the variable shootTimer of type float
	float lineGunTimer;				//Declaration of variable lineGunTimer of type float
	public float rotateHurtSpeed;	//Declaration of rotateHurtSpeed of type float. 
	Vector3 rotateHurtDir;			//Declaration of rotaeHurtDir of type Vector3
	float bossSpeed = 10f;			//Declaration of bossSpeed of type float
	float chargeSpeed = 20f;		//Declaration of chargeSpeed of type float
	int hp;							//Declaration of hp of type int
	int line;						//Declaration of line of type int
	int phase;						//Declaration of phase of type int
	public Text phaseText;			//Declaration of phaseText of type Text
	
	public GameObject pickup;		//Declaration of pickup of type GameObject
	public GameObject laser;		//Declaration of laser of type GameObject
	public Transform[] gun;			//Declaration of gun of type Transform
	public Transform[] lineGun;		//Declaration of a lineGun which is a Transform array/
	
	NavMeshAgent nma;				//Declaration of nma of type NavMeshAgent 
	
	Vector3 newPosition;			//Declaration of newPosition of type Vector3
	Vector3 positionA, positionB;	//Declaration of positionA, positionB
	Ray ray;						//Declaration of ray of type Ray
	RaycastHit target;				//Declaration of target of Type RaycastHit
	
	public AudioClip[] sfx;
	AudioSource sound;
	
	//Declaration of an enumeration list.
	enum States //our states 
	{
		Initialize,
		Move,
		ChargeForward,
		ChargeBack,
		Hurt,
		DropMine,
		Shoot,
		LineShoot
	}
	
	States currentState = States.Initialize; //create an instance of the enum and set it's default to Initialize
	
	void Awake ()
	{
		
	}
	
	void Start()
	{
	/*
		hurtTimer = 0f;
		dropTimer = 0f;
		rotateHurtDir = new Vector3(0,rotateHurtSpeed,0);
		phase = 1;
		phaseText.text = "Phase:" + phase;
		hp = 5;
		line = 1;
	*/	
	}
	
	void Update () 
	{
		switch(currentState) //pass in the current state
		{
		case States.Initialize: 	//If the current state is Initialize
			Initilize();			//Calls Initilize() method
			break;					//Breaks out of Switch
		case States.Move:			//If the current state is Move
			Move ();				//Calls Move() method
			break;					//Breaks out of Switch
		case States.ChargeForward:	//If the current state is ChargeForward
			ChargeForward();		//Calls ChargeForward() method
			break;					//Breaks out of Switch
		case States.ChargeBack:		//If the current state is ChargeBack
			ChargeBack();			//Calls ChargeBack() method
			break;					//Breaks out of Switch
		case States.Hurt:			//If the current state is Hurt
			Hurt();					//Calls Hurt() method
			break;					//Breaks out of Switch
		case States.DropMine:		//If the current state is DropMine
			DropMine();				//Calls DropMine() method
			break;					//Breaks out of Switch
		case States.Shoot:			//If the current state is Shoot
			Shoot();				//Calls Shoot() method
			break;					//Breaks out of Switch
		case States.LineShoot:		//If the current state is Line
			LineShoot();			//Calls LineShoot() method
			break;					//Breaks out of Switch
		}
		//Shows a magenta line from the boss position to -boss.transform.forward * 25f
		Debug.DrawRay(boss.position, -boss.transform.forward * 25f, Color.magenta);
		//Debug.Log (currentState);
	}
	
	//Definition for the initilize() state
	void Initilize()
	{
		hurtTimer = 0f; 									//hurtTimer is assigned 0
		dropTimer = 0f;										//dropTimer is assigned 0
		rotateHurtDir = new Vector3(0,rotateHurtSpeed,0); 	//This sets the speed of the rotation of the boss when he's hurt
		phase = 1;		//phase is set to 1
		phaseText.text = "Phase:" + phase; //Updates the text to Phase:1 on the HUD 
		hp = 5;	//boss hp is set to 5
		line = 1; // Initializes line to be 1
	
		boss = GameObject.FindWithTag("Boss").transform;		//Finds the transform component of boss and assigns
		player = GameObject.FindWithTag("Player").transform;	//Finds the transform component of player and assigns
		nma = transform.GetComponent<NavMeshAgent>();			//Finds the NavMeshAgent component and assigns
		
		currentState = States.Move; // The current state will now be move
		
		sound = GetComponent<AudioSource>();
	}
	
	//
	void Move()
	{
		//First phase movement
		if(phase == 1)
		{	//Boss moves along the xAxis
			transform.Translate(bossSpeed * Time.deltaTime, 0, 0);
			if(DoRayCast()) //checks to see if ray hits a collider using the bool return method
			{
				//if the tag of the object the raycast touches is equal to "Player"
				if(target.transform.tag == "Player") 
				{
					//Boss position is assigned to positionA
					positionA = transform.position; 
					//Position B is calculated by addings vector to Position A
					positionB = positionA + new Vector3(0,0, -25);
					//The boss is now in the ChargeForward state
					currentState = States.ChargeForward;
				}
			}
		}
		//Second phase movement
		if(phase == 2)
		{
			// The destination for the NavMeshAgent is the player's position.
			//The boss will move towards the player.
			nma.destination = player.position;
			dropTimer -= Time.deltaTime;	//Droptimer is counting down
			shootTimer -= Time.deltaTime;	//ShootTimer is counting down
			if(dropTimer <= 0)	//If the drop timer reaches zero
			{
				currentState = States.DropMine; //Enter the drop mine state
			}
			if(shootTimer <= 0) //If the shoot timer is less than or equal to zero
			{
				currentState = States.Shoot; // Enter the Shoot state
			}
			
			
		}
		if(phase == 3)
		{
			//transform.Translate(0, 0, bossSpeed * Time.deltaTime);
			nma.destination = player.position;
			dropTimer -= Time.deltaTime;
			shootTimer -= Time.deltaTime;
			lineGunTimer -= Time.deltaTime;
			if(dropTimer <= 0)
			{
				currentState = States.DropMine;
			}
			if(lineGunTimer <= 0)
			{
				currentState = States.LineShoot;
			}
			
		}
		
	}
	
	//The state where the boss will charge forward when the boss sees the enemy
	void ChargeForward()
	{
		//Boss moves down
		transform.Translate(0,0, -chargeSpeed * Time.deltaTime);
		if(transform.position.z <= positionB.z) //if the boss's z position is less than the z of position b 
		{
			//Current state is now the ChargeBack state
			currentState = States.ChargeBack;
		}
	}
	
	//State where the boss will back to it's orginal point on the z axis
	void ChargeBack()
	{
		transform.Translate(0,0, chargeSpeed * Time.deltaTime); // Boss moves back
		if(transform.position.z >= 12) // if the boss's zpoisiton is greater or equal to 12
		{
			currentState = States.Move; //Jump to move state
		}
	}
	
	// The state that is called when the boss gets hurt
	void Hurt()
	{
	
		//Rotates
		transform.Rotate(rotateHurtDir * Time.deltaTime);
		
		if(hurtTimer > 0) //if timer is greater than 0
		{
			hurtTimer -= Time.deltaTime; // hurtTimer is decremented by Time.deltaTime
		}
		
		if(hurtTimer < 0)//If timer is less than zero
		{
			hurtTimer = 0; // timer is zero
		}
		
		if(hurtTimer == 0)
		{
			transform.rotation = Quaternion.identity; //Makes sure the boss back to it's normal rotation
			//After he finishes spinning
			
			if(phase == 1) //if phase is 1
			{	
				currentState = States.ChargeBack; //Goes to charge backl state
			}
			if(phase == 2 || phase == 3) //If phase is 2 or 3
			{
				dropTimer = Random.Range(5f, 10f); //dropTimer is a random number beyween 5 and 9
				shootTimer = 3f; //shootTimer is 3 seconds
				currentState = States.Move; //Current state is Move
			}
			if(phase == 4)
			{
				Application.LoadLevel("EndGame");
			}
		}
	}
	
	void DropMine()
	{
		Instantiate(pickup, transform.position, transform.rotation); // Boss drops a mine
		dropTimer = Random.Range(5f, 10f); // Is set between 5 and 9 seconds
		currentState = States.Move; // Goes back to the move state
	}
	
	// This is the state where the boss shoots, happens during phase 2
	void Shoot()
	{
		sound.clip = sfx[1];
		sound.Play();
		Instantiate(laser, gun[0].position, gun[0].rotation); //Instantiates the laser
		Instantiate(laser, gun[1].position, gun[1].rotation); //Instantiates the laser
		Instantiate(laser, gun[2].position, gun[2].rotation); //Instantiates the laser
		shootTimer = 3f; //shootTimer set to 3
		currentState = States.Move; //Jumps to the Move state
	}
	
	// LineShoot state, during the 3rd phase, lasers will shoot from 1 of 4 sides.
	void LineShoot()
	{
		line = Random.Range (1,5); //Choose a random int from 1 to 4
		
		sound.clip = sfx[2];
		sound.Play();
		
		if(line == 1) //if line is equal to 1, instantiate these lasers
		{
			Instantiate(laser, lineGun[0].position, lineGun[0].rotation);
			Instantiate(laser, lineGun[1].position, lineGun[1].rotation);
			Instantiate(laser, lineGun[2].position, lineGun[2].rotation);
			Instantiate(laser, lineGun[3].position, lineGun[3].rotation);
			Instantiate(laser, lineGun[4].position, lineGun[4].rotation);
			Instantiate(laser, lineGun[5].position, lineGun[5].rotation);
			Instantiate(laser, lineGun[6].position, lineGun[6].rotation);
		}
		if(line == 2) //if line is equal to 2, instantiate these lasers
		{
			Instantiate(laser, lineGun[7].position, lineGun[7].rotation);
			Instantiate(laser, lineGun[8].position, lineGun[8].rotation);
			Instantiate(laser, lineGun[9].position, lineGun[9].rotation);
			Instantiate(laser, lineGun[10].position, lineGun[10].rotation);
			Instantiate(laser, lineGun[11].position, lineGun[11].rotation);
			Instantiate(laser, lineGun[12].position, lineGun[12].rotation);
			Instantiate(laser, lineGun[13].position, lineGun[13].rotation);
		}
		if(line == 3) //if line is equal to 3, instantiate these lasers
		{
			Instantiate(laser, lineGun[14].position, lineGun[14].rotation);
			Instantiate(laser, lineGun[15].position, lineGun[15].rotation);
			Instantiate(laser, lineGun[16].position, lineGun[16].rotation);
			Instantiate(laser, lineGun[17].position, lineGun[17].rotation);
			Instantiate(laser, lineGun[18].position, lineGun[18].rotation);
			Instantiate(laser, lineGun[19].position, lineGun[19].rotation);
			Instantiate(laser, lineGun[20].position, lineGun[20].rotation);
			Instantiate(laser, lineGun[21].position, lineGun[21].rotation);
			Instantiate(laser, lineGun[22].position, lineGun[22].rotation);
			Instantiate(laser, lineGun[23].position, lineGun[23].rotation);
			Instantiate(laser, lineGun[24].position, lineGun[24].rotation);
			Instantiate(laser, lineGun[25].position, lineGun[25].rotation);
			Instantiate(laser, lineGun[26].position, lineGun[26].rotation);
			Instantiate(laser, lineGun[27].position, lineGun[27].rotation);
			Instantiate(laser, lineGun[28].position, lineGun[28].rotation);
		}
		if(line == 4) //if line is equal to 4, instantiate these lasers
		{
			Instantiate(laser, lineGun[29].position, lineGun[29].rotation);
			Instantiate(laser, lineGun[30].position, lineGun[30].rotation);
			Instantiate(laser, lineGun[31].position, lineGun[31].rotation);
			Instantiate(laser, lineGun[32].position, lineGun[32].rotation);
			Instantiate(laser, lineGun[33].position, lineGun[33].rotation);
			Instantiate(laser, lineGun[34].position, lineGun[34].rotation);
			Instantiate(laser, lineGun[35].position, lineGun[35].rotation);
			Instantiate(laser, lineGun[36].position, lineGun[36].rotation);
			Instantiate(laser, lineGun[37].position, lineGun[37].rotation);
			Instantiate(laser, lineGun[38].position, lineGun[38].rotation);
			Instantiate(laser, lineGun[39].position, lineGun[39].rotation);
			Instantiate(laser, lineGun[40].position, lineGun[40].rotation);
			Instantiate(laser, lineGun[41].position, lineGun[41].rotation);
			Instantiate(laser, lineGun[42].position, lineGun[42].rotation);
			Instantiate(laser, lineGun[43].position, lineGun[43].rotation);
		}
		lineGunTimer = 5f;
		currentState = States.Move;
	
	}
	
	bool DoRayCast() //bool return type, returns true if ray hits a collider
	{
		ray = new Ray(boss.position, -boss.transform.forward); //a new ray, which takes a start position and direction
		bool rayHit = Physics.Raycast(ray, out target, 25f); //raycast returns true or false
		return rayHit; //returns rayHit if true or false
	}
	
	// For when the boss touches another objecy
	void OnTriggerEnter(Collider col)
	{
		//If the boss touches the wall, go the other way
		if(col.tag == "Wall")
		{
			bossSpeed = -bossSpeed;
		}
		//if the other is a TrapFrEnemy
		if(col.tag == "TrapForEnemy")
		{
			sound.clip = sfx[0];
			sound.Play();
			Destroy(col.gameObject); // Destroy TrapForEnemy gameObject
			hurtTimer = 3f;	//hurtTimer is set to 3 for the hurt state
			hp--; // reduce hit points of the boss by one
			if(hp <= 0) //if the hitpoints reaches zero or lower
			{
				sound.clip = sfx[3];
				sound.Play();
				hp = 5; //Bring back the hit points to 5
				phase++; //Adds one to phase, bringing the boss to the next phase
				phaseText.text = "Phase:" + phase; //Updates the phase number in the HUD
			}
			currentState = States.Hurt; //Sets the current state to be te hurt state.
		}
	}
}
