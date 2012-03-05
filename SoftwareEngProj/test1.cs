using System;
using NUnit.Framework;
namespace AssemblyCSharp
{
	[TestFixture()]
	public class test
	{
		[Test()]
		public void TestCase ()
		{
			
			
		}
		
		[Test()]
		public void testdistance(){
			Assert.Greater(NewBehaviourScript.DistanceTraveled(1f),0f);
			Assert.IsNotNull(NewBehaviourScript.DistanceTraveled(1f));
			Assert.IsFalse(NewBehaviourScript.keyPressed(false));
			Assert.IsTrue(NewBehaviourScript.keyPressed(true));
		}
		
	}
}

