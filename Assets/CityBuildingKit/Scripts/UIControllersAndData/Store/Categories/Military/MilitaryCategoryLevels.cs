/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2019.
 * All rights reserved.
 */

using System.Collections.Generic;
using UIControllersAndData.Models;

namespace UIControllersAndData.Store.Categories.Military
{
	[System.Serializable]
	public class MilitaryCategoryLevels: INamed, IId
	{
		public string name;
		public int id;
		public List<MilitaryCategory> levels;

		public string GetName()
		{
			return name;
		}

		public int GetId()
		{
			return id;
		}
	
	}
}
