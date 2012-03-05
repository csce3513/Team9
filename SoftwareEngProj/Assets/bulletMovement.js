var bullet : GameObject;
function Update () {
        var tab = Input.GetKey("tab");
		if(tab){
			var instance : GameObject = Instantiate(bullet,transform.position, transform.rotation);
			
		}
}