var acc = 1;


function Update () {

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
	}
	
	else if(left){
		if(acc == 2){
			transform.Rotate(0,180,0);
			acc = 1;
			}
		animation.Play("Walk");
		transform.Translate(0,0,-.1);
	}
	
	if(up){
		animation.Play("Walk");
		transform.Translate(0,.5,0);
	}
}