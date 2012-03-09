using UnityEngine;
using System.Collections;
using SharpUnit;

public class Everythings_There_Test : TestCase
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
    public void TestPlayer()
    {
		Assert.NotNull(GameObject.Find("Test"));
    }
	
	[UnitTest]
    public void TestPlatforms()
    {
		Assert.NotNull(GameObject.Find("Platform"));
		Assert.NotNull(GameObject.Find("Platform1"));
	}
	
    [UnitTest]
    public void TestFloor()
    {
		Assert.NotNull(GameObject.Find("Floor"));
    }	
	
    [UnitTest]
    public void TestCamera()
    {
		Assert.NotNull(GameObject.Find("Main Camera"));
    }	
	
    [UnitTest]
    public void TestLights()
    {
		Assert.NotNull(GameObject.Find("Directional light"));
    }	
}