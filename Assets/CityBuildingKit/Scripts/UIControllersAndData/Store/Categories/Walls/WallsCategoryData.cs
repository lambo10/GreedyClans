/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIControllersAndData.Store.Categories.Walls
{
	[Serializable]
	public class WallsCategoryData:ScriptableObject 
	{
		[FormerlySerializedAs("StoreCategoryId")] 
		public string storeCategoryId;
		[FormerlySerializedAs("Category")] 
		[FormerlySerializedAs("WallsCategory")]
		public List<WallCategoryLevels> category;
	}
}
