var acc = 1;
var jumpspeed = 100.0f;
var Background : GameObject;
function FixedUpdate () {

	var right = Input.GetKey("d");
	var left = Input.GetKey("a");
	var up = Input.GetKey("space");
	
	
	if(right){
		if(acc == 1){
			transform.Rotate(0,180,0);
			acc = 2;
			}
		animation.Play("Walk");
		transform.Translate(0,0,-.1);
		Background.transform.Translate(.05,0,0,Space.World);
	}
	
	else if(left){
		if(acc == 2){
			transform.Rotate(0,180,0);
			acc = 1;
			}
		animation.Play("Walk");
		transform.Translate(0,0,-.1);
		Background.transform.Translate(-.05,0,0,Space.World);
	}
	
	if(up){
		Jump();
	}
}

function Jump()
{
	rigidbody.AddForce(Vector3.up *jumpspeed);
}