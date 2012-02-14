
function Update () {

	var right = Input.GetKey("d");
	var left = Input.GetKey("a");
	
	if(right){
		animation.Play("Walk");
		transform.Translate(0,0,-Input.GetAxis("Horizontal") * .1);
	}
	
	else if(left){
		animation.Play("Walk");
		transform.Translate(0,0,Input.GetAxis("Vertical") * .1);
	}
}