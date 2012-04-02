using UnityEngine;
using System.Collections;

public class LevelBounds : MonoBehaviour {

	public Car hero;
	
	public void Start()
	{
        hero = GameObject.Find("Car").GetComponent(typeof(Car)) as Car;
        
	}

    public void OnTriggerEnter()
    {
        hero.insideBoundsCount++;
    }
	public void OnTriggerExit()
	{
        hero.insideBoundsCount--;
        if (hero.insideBoundsCount <= 0)
        {
            hero.ResetLevel();
        }      
	}
}
