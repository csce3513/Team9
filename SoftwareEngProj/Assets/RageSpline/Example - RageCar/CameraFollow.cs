using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	
	public GameObject targetObj;
	public Vector3 startDistance;
	// Use this for initialization
	void Start () {
		startDistance = targetObj.transform.position - transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position = Vector3.Lerp(transform.position, targetObj.transform.position - startDistance, Time.deltaTime*3f);
	}
}
