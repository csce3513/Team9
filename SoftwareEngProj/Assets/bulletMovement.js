//var bullet : GameObject;
var bullet : Rigidbody;

function Update () {
        //var tab = Input.GetKey("tab");
		var tab = Input.GetMouseButtonDown(0);
		if(tab)
		{
			//var instance : GameObject = Instantiate(bullet,transform.position, transform.rotation);
			shooting();
		}
}

function shooting()
{
	var instance : Rigidbody;
	//var instance : GameObject = Instantiate(bullet,transform.position, transform.rotation);
	
	instance = Instantiate(bullet, transform.position, transform.rotation);
	
	instance.velocity = transform.TransformDirection(Vector3.forward * 50);
	
	yield WaitForSeconds(3);
	if(instance)
	{
		Destroy(instance.gameObject);
	}
}