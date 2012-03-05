using UnityEngine;
using System.Collections;


	public class tddGun{
		private int clip;
		private int totalBullets;
		public int damage = 10;
	
		public tddGun(){
			clip = 50;
			totalBullets = 100;
		}
		
		public int getClip()
		{
			return clip;
		}
		
		public int getTotalBullets()
		{
				return totalBullets;
		}
		
		public void Shoot()
		{
			if(clip == 0)
			{
				Reload();	
			}
			else
			{
				clip = clip-1;
			}	
		}
		
		public void Reload()
		{
			if(clip < 50)
			{
				if((50 - clip) > totalBullets)
				{
					clip = clip + totalBullets;
					totalBullets = 0;
				}
				else
				{
					totalBullets = totalBullets - (50 - clip);
					clip = 50;
				}
			}
		}
		
		
	}
	
