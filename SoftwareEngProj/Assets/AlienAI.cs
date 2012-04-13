using UnityEngine;
using System.Collections;

public class AlienAI : MonoBehaviour {
	public Transform target;
	public int moveSpeed = 1;
	public int rotationSpeed = 10;
	public float Health = 30;
	public float distance;
	
	
	private Transform myTransform; 
	
	
	void Awake(){
		myTransform = transform;
		
	}

	// Use this for initialization
	void Start () {
		GameObject go = GameObject.FindGameObjectWithTag("Player");
		target = go.transform;
		
		
	}
	
	
	// Update is called once per frame
	void Update () {
		
		//Trigger Aliens by distance
		Vector3 desiredHeading = target.position - myTransform.position;
		distance = desiredHeading.magnitude;
		
		
		//Turn towards player
		myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(target.position - myTransform.position), rotationSpeed * Time.deltaTime);
		
		//Move towards player
		if (distance < 12.5)
		myTransform.position += myTransform.forward * moveSpeed * Time.deltaTime;
		
		
	}
	
	public void TakeDamage(float amount){	
		Health -= amount;
		transform.Translate(0,0,-1);
		
	}
	
	public bool DidAlienDie()
	{
		if (Health <= 0.0)
			return true;
		else
			return false;
		
	}
	
	}
		
		
		
	

