/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIControllersAndData.Store.Categories.Ambient
{
	[System.Serializable]
	public class AmbientCategoryData:ScriptableObject 
	{
		[FormerlySerializedAs("StoreCategoryId")] 
		public string storeCategoryId;
		[FormerlySerializedAs("Category")] 
		[FormerlySerializedAs("AmbientCategory")]
		public List<AmbientCategoryLevels> category;
		
	}
}
