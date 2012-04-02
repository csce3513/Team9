using UnityEngine;
using System.Collections;


public class bulletMovement{
	
	GameObject missile;
	void Start ()
	{
		missile = new GameObject();
	}
	
	
	void Update () {
		bool tab = Input.GetKey("tab");
		if(tab){
			//missile = Instantiate(bullet,transform.position, transform.rotation) as GameObject;
		}
	
	}
}
