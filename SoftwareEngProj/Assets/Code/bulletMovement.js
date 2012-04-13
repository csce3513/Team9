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
		
	/*	if(currAmmo > 0)
		{
			if( mbd)
			{
				//var instance : GameObject = Instantiate(bullet,transform.position, transform.rotation);
				shooting();
				currAmmo = currAmmo - 1;
			}
		}
			
		if(currAmmo < 10)
		{
			if(tab)
			{	
				if( (10 - currAmmo) > reloadAmmo)
				{
					currAmmo = currAmmo + reloadAmmo;
					reload = 0;
				}
				else
				{
					reloadAmmo = reloadAmmo - ( 10 - currAmmo );
					currAmmo = 10;
				}
			}
		}*/
		
		
		
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