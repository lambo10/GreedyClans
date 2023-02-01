/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using System.Collections.Generic;
using UIControllersAndData.Store.Categories.Store;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Scripts.UIControllersAndData.Store.Categories.Store
{
	[Serializable]
	public class StoreCategoryData:ScriptableObject 
	{
		public string StoreCategoryId;
		[FormerlySerializedAs("StoreCategory")]
		public List<StoreCategory> Category;
		
	}
}