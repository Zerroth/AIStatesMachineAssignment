using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour 
{
	int mine;
	public Text mineText;
	public GameObject trap;
	float speed = 10f;
	
	void Start()
	{
		mine = 0;
		mineText.text = "Mine:" + mine;
	}
	
	// Update is called once per frame
	void Update () 
	{
		float v = Input.GetAxis("Vertical") * speed * Time.deltaTime;
		float h = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
		
		transform.Translate(h,0,v,Space.World);
		
		if(Input.GetKeyDown(KeyCode.Space))
		{
			if(mine > 0)
			{
				Instantiate (trap, transform.position, transform.rotation);
				mine--;
				mineText.text = "Mine:" + mine;
			}
		}
	}
	
	void OnTriggerEnter(Collider col)
	{
		if(col.tag == "MinePickup")
		{
			mine ++;
			if(mine >= 2)
			{
				mine = 1;
			}
			mineText.text = "Mine:" + mine;
			Destroy(col.gameObject);
		}
		if(col.tag == "Boss")
		{
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}
