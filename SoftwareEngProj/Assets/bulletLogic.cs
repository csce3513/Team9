using UnityEngine;
using System;

public class bulletLogic : MonoBehaviour {
	
	float distance;
	void Start ()
	{
		distance = transform.position.x;
	}

	// Update is called once per frame
	void Update () {
		if(farEnough(distance))
		{
			Destroy(GameObject.Find("bullet(Clone)"));
		}
		else
		{
			transform.Translate(0,0,0.1f);
		}
	}
	
	public bool farEnough(float begdis)
	{
		float temp = transform.position.x - begdis;
		temp = Math.Abs(temp);
		if(temp >5)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}