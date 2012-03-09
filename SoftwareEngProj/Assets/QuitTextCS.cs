using UnityEngine;
using System.Collections;

public class QuitTextCS : MonoBehaviour {
	
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
}
