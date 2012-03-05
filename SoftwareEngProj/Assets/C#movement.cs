using UnityEngine;
using System.Collections;



public class NewBehaviourScript : MonoBehaviour {
	float distance = 0;
	int acc = 1;
	float speed = (-0.1f);
	float jump = 0.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update () {
		
		distance = DistanceTraveled(transform.position.x);
		
		bool right = Input.GetKey("d");
		bool left = Input.GetKey("a");
		bool up = Input.GetKey("space");
		
		if(right){
			if(acc == 1){
				transform.Rotate(0,180,0);
				acc = 2;
				}
			animation.Play("Walk");
			transform.Translate(0,0,speed);
		}
		
		else if(left){
			if(acc == 2){
				transform.Rotate(0,180,0);
				acc = 1;
				}
			animation.Play("Walk");
			transform.Translate(0,0,speed);
		}
		
		if(up){
			animation.Play("Walk");
			transform.Translate(0,jump,0);
		}
		
	}
	
	public static float DistanceTraveled (float dis) {
		dis = 1 + dis;
		return dis;
	}
	
	public static bool keyPressed(bool direction) {
		return direction;
	}
	
	
}
