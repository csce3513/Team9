var player : GameObject;
var temp;
function Start(){
	transform.position.y = player.transform.position.y + 3.5;
	transform.position.x = player.transform.position.x;
	temp = transform.position.y;
}
function Update () {
		transform.position.x = player.transform.position.x;
		if (player.transform.position.y > 5){
			transform.position.y = player.transform.position.y;
		}
		else
		{
			transform.position.y = temp;
		}
		
}