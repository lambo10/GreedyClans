/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIControllersAndData.Store.Categories.Unit
{	
	[System.Serializable]
	public class UnitCategoryData:ScriptableObject 
	{
		[FormerlySerializedAs("StoreCategoryId")] 
		public string storeCategoryId;
		[FormerlySerializedAs("Category")] 
		[FormerlySerializedAs("UnitCategory")]
		public List<UnitCategoryLevels> category;
		
	}
}
