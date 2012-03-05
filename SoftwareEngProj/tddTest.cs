using System;
using NUnit.Framework;
namespace AssemblyCSharp
{
	[TestFixture()]
	public class tddTest
	{
		[Test()]
		public void GunTest ()
		{
			tddGun temp = new tddGun();
			Assert.Greater(temp.getClip(), 0);
			
			Assert.AreEqual(50, temp.getClip());
			temp.Shoot();
			Assert.AreEqual(49 , temp.getClip());
			
			temp.Reload();
			Assert.AreEqual(50, temp.getClip());
			
			Assert.Greater(temp.getTotalBullets(), 0);
			
		}
	}
}

