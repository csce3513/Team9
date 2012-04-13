using UnityEngine;
using System.Collections;
using SharpUnit;

public class AlienDie : TestCase
{ 
	public override void SetUp()
    { 
    }

    /**
     * Dispose of test resources, called after each test.
     */
    public override void TearDown()
    {
    }

    
    [UnitTest]
    public void TestAlienDie()
    {
		
		Assert.Null(GameObject.Find("alienproject"));
    }
}