using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
	public int maxHealth = 100;
	public int curHealth = 100;
	public int maxAmmo = 32;
	public int curAmmo = 32;
	
	public float healthBarLength;
	public float ammoBarLength;
	
	// Use this for initialization
	void Start () {
		healthBarLength = Screen.width / 2;
		ammoBarLength = Screen.width / 2;
	}
	
	// Update is called once per frame
	void Update () {
		AdjustCurrentHealth(0);
		AdjustCurrentAmmo(0);
	}
	
	
	void OnGUI() {
		GUI.color = Color.white;
		GUI.Box(new Rect(20, 25, Screen.width / 4 , 20), curHealth + "/" + maxHealth);
		
		GUI.Box(new Rect(20, 25, healthBarLength, 20), " ");

		GUI.Box(new Rect(20, 50, Screen.width / 5, 20), curAmmo + "/" + maxAmmo);
		
		GUI.Box(new Rect(20, 50, ammoBarLength, 20), " ");
	
	}
	
	public void AdjustCurrentHealth(int adj) {
		curHealth += adj;
		
		healthBarLength = (Screen.width / 4) * (curHealth / (float)maxHealth);
		
	}
	
	public void AdjustCurrentAmmo(int adj) {
		curAmmo += adj;
		
		ammoBarLength = (Screen.width / 5) * (curAmmo / (float)maxAmmo);
		
	}
}
