/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

namespace Assets.Scripts.UIControllersAndData.Cloak
{
	
	public class CloakData
	{
		private int _remainingCloakTime;

		public int RemainingCloakTime
		{
			get { return _remainingCloakTime; }
			set { _remainingCloakTime = value; }
		}

		public int PurchasedCloakTime
		{
			get { return _purchasedCloakTime; }
			set { _purchasedCloakTime = value; }
		}

		private int _purchasedCloakTime;
	}
}