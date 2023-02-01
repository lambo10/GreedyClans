/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

namespace Assets.Scripts.UIControllersAndData.Player
{
	
	public class ExperienceData 
	{

		public ExperienceData() 
		{
		}

		private int _currentExp;

		public int CurrentExp
		{
			get { return _currentExp; }
			set { _currentExp = value; }
		}
	}
}