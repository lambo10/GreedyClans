/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using UIControllersAndData.Models;
using UnityEngine;

namespace UIControllersAndData.Store.Categories.Ambient
{
	[System.Serializable]
	public class AmbientCategory:BaseStoreItemData, INamed, IId, IAsset, ILevel
	{
		public string name;
		public int id;
		public GameObject asset;
		public int level;
		public string GetName()
		{
			return name;
		}

		public int GetId()
		{
			return id;
		}

		public GameObject GetAsset()
		{
			return asset;
		}

		public int GetLevel()
		{
			return level;
		}
	}
}
