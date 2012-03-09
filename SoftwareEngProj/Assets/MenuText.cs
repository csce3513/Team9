using UnityEngine;
using System.Collections;

public class MenuText : MonoBehaviour {
	public bool isClose;
	
	void Start ()
	{
		renderer.material.color = Color.magenta;	
	}
	
	void OnMouseEnter ()
	{
		renderer.material.color = Color.blue;
	}
	
	void OnMouseExit ()
	{
		renderer.material.color = Color.magenta;	
	}
	
	void OnMouseDown ()
	{
		if(isClose)
		{
			Application.Quit();
		}
		else
		{
			Application.LoadLevel(1);
		}
	}
}
