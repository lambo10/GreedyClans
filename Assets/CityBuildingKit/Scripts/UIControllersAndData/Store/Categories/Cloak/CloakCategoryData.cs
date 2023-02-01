/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System.Collections.Generic;
using UIControllersAndData.Store.Categories.Cloak;
using UnityEngine;

namespace Assets.Scripts.UIControllersAndData.Store.Categories.Cloak
{
	[System.Serializable]
	public class CloakCategoryData:ScriptableObject 
	{
		public string StoreCategoryId;
		public List<CloakCategory> Category;
	}
}
