/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIControllersAndData.Store.Categories.Weapon
{
	[System.Serializable]
	public class WeaponCategoryData:ScriptableObject 
	{
		[FormerlySerializedAs("StoreCategoryId")] 
		public string storeCategoryId;
		[FormerlySerializedAs("Category")] 
		[FormerlySerializedAs("WeaponCategory")]
		public List<WeaponCategoryLevels> category;
		
	}
}
