using UnityEngine;
using System;
using System.Collections;


public class bulletLogic : MonoBehaviour {
	public AudioClip shot;
	float distance;
	public float Damage = 10;
	public float Health;
	//public int maxAmmo = 32;
	//public int curAmmo = 32;
	//public float ammoBarLength;
	
	void Start ()
	{
		audio.PlayOneShot(shot);
		distance = transform.position.x;
		transform.Translate(0,0.3f,-0.3f);	
		//ammoBarLength = Screen.width / 2;
		
	}

	// Update is called once per frame
	void Update () {
		//AdjustCurrentAmmo(0);
		if(farEnough(distance))
		{
			Destroy(GameObject.Find("bullet(Clone)"));
		}
		else
		{
			transform.Translate(0,0,-0.3f);
		}
		
		
	}
	
	/*void OnGUI() {
		GUI.color = Color.white;

		GUI.Box(new Rect(20, 50, Screen.width / 5, 20), curAmmo + "/" + maxAmmo);
		
		GUI.Box(new Rect(20, 50, ammoBarLength, 20), " ");
	
	}*/
	
	/*public void AdjustCurrentAmmo(int adj) {
		curAmmo += adj;
		
		ammoBarLength = (Screen.width / 5) * (curAmmo / (float)maxAmmo);
		
	}*/
		
	public bool farEnough(float begdis)
	{
		float temp = transform.position.x - begdis;
		temp = Math.Abs(temp);
		if(temp >10)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	public void OnTriggerEnter(Collider col){
		col.SendMessage("TakeDamage", Damage);	
		AlienAI AI = col.gameObject.GetComponent(typeof(AlienAI)) as AlienAI;
		Destroy(GameObject.Find("bullet(Clone)"));
		if (AI.DidAlienDie()){
			//Destroy(GameObject.Find("bullet(Clone)"));
			Destroy(col.gameObject);
			Debug.Log("Alien die");
		}
		
	}
	

		

}