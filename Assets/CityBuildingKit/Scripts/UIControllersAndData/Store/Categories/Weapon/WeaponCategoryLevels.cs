/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2019.
 * All rights reserved.
 */

using System.Collections.Generic;
using UIControllersAndData.Models;
using UnityEngine;

namespace UIControllersAndData.Store.Categories.Weapon
{
	[System.Serializable]
	public class WeaponCategoryLevels: INamed, IId 
	{
		public string name;
		public int id;
		public List<WeaponCategory> levels;

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
