using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Boss : MonoBehaviour 
{
	Transform boss,player;
	float hurtTimer;
	float dropTimer;
	public float rotateHurtSpeed;
	Vector3 rotateHurtDir;
	float coolDown;
	float bossSpeed = 10f;
	float chargeSpeed = 20f;
	int hp;
	int phase;
	public Text phaseText;
	public GameObject pickup;
	
	NavMeshAgent nma;
	
	Vector3 newPosition;
	Vector3 positionA, positionB;
	Ray ray;
	RaycastHit target;
	
	enum States //our states 
	{
		Initialize,
		//Sleep,
		//Attack,
		//Searching.
		Move,
		ChargeForward,
		ChargeBack,
		Hurt,
		DropMine
	}
	
	States currentState = States.Initialize; //create an instance of the enum and set it's default to Initialize
	
	void Awake ()
	{
		
	}
	
	void Start()
	{
		hurtTimer = 0f;
		dropTimer = 0f;
		coolDown = 3f;
		rotateHurtDir = new Vector3(0,rotateHurtSpeed,0);
		phase = 1;
		phaseText.text = "Phase:" + phase;
		hp = 5;
		
	}
	
	void Update () 
	{
		switch(currentState) //pass in the current state
		{
		case States.Initialize:
			Initilize();
			break;
		case States.Move:
			Move ();
			break;
		case States.ChargeForward:
			ChargeForward();
			break;
		case States.ChargeBack:
			ChargeBack();
			break;
		case States.Hurt:
			Hurt();
			break;
		case States.DropMine:
			DropMine();
			break;
		
		/*case States.Sleep:
			Sleep ();
			break;
		
		case States.Attack:
			Attack ();
			break;
		case States.Searching:
			Searching ();
			break;
		*/
		}
		Debug.DrawRay(boss.position, -boss.transform.forward * 25f, Color.magenta);
		Debug.Log (currentState);
	}
	
	void Initilize()
	{
		//turret = GameObject.Find ("GunPivot").transform;
		boss = GameObject.FindWithTag("Boss").transform;
		player = GameObject.FindWithTag("Player").transform;
		nma = transform.GetComponent<NavMeshAgent>();
		
		currentState = States.Move;
	}
	
	void Move()
	{
		if(phase == 1)
		{
			transform.Translate(bossSpeed * Time.deltaTime, 0, 0);
			if(DoRayCast()) //checks to see if ray hits a collider using the bool return method
			{
				if(target.transform.tag == "Player") //check to see if player
				{
					positionA = transform.position;
					positionB = positionA + new Vector3(0,0, -25);
					currentState = States.ChargeForward;
				}
			}
		}
		
		if(phase == 2)
		{
			//transform.Translate(0, 0, bossSpeed * Time.deltaTime);
			nma.destination = player.position;
			dropTimer -= Time.deltaTime;
			if(dropTimer <= 0)
			{
				currentState = States.DropMine;
			}
			
		}
	}
	
	void ChargeForward()
	{
		//transform.position = Vector3.Lerp(transform.position,positionB,Time.deltaTime);
		transform.Translate(0,0, -chargeSpeed * Time.deltaTime);
		if(transform.position.z <= positionB.z)
		{
			currentState = States.ChargeBack;
		}
		//StartCoroutine(ForwardBack());
	}
	
	void ChargeBack()
	{
		//transform.position = Vector3.Lerp(transform.position,positionA,Time.deltaTime);
		transform.Translate(0,0, chargeSpeed * Time.deltaTime);
		if(transform.position.z >= 12)
		{
			currentState = States.Move;
		}
	}
	
	void Hurt()
	{
		transform.Rotate(rotateHurtDir * Time.deltaTime);
		
		if(hurtTimer > 0)
		{
			hurtTimer -= Time.deltaTime;
		}
		
		if(hurtTimer < 0)
		{
			hurtTimer = 0;
		}
		
		if(hurtTimer == 0)
		{
			transform.rotation = Quaternion.identity;
			
			if(phase == 1)
			{
				currentState = States.ChargeBack;
			}
			if(phase == 2)
			{
				dropTimer = Random.Range(5f, 10f);
				currentState = States.Move;
			}
		}
	}
	
	void DropMine()
	{
		Instantiate(pickup, transform.position, transform.rotation);
		dropTimer = Random.Range(5f, 10f);
		currentState = States.Move;
	}
	
	bool DoRayCast() //bool return type, returns true if ray hits a collider
	{
		ray = new Ray(boss.position, -boss.transform.forward); //a new ray, which takes a start position and direction
		bool rayHit = Physics.Raycast(ray, out target, 25f); //raycast returns true or false
		return rayHit; //returns rayHit if true or false
	}
	
	void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Wall")
		{
			bossSpeed = -bossSpeed;
		}
		
		if(col.tag == "TrapForEnemy")
		{
			Destroy(col.gameObject);
			hurtTimer = 3f;
			hp--;
			if(hp <= 0)
			{
				hp = 5;
				phase++;
				phaseText.text = "Phase:" + phase;
			}
			currentState = States.Hurt;
		}
	}
}
