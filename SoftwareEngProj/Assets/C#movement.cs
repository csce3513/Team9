using UnityEngine;
using System.Collections;



public class NewBehaviourScript : MonoBehaviour {
	
	int acc = 1;
	float speed = (-0.1f);
	float jump = 0.5f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update () {
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
}
