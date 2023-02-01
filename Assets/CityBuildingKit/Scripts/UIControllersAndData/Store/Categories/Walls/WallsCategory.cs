/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using UIControllersAndData.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIControllersAndData.Store.Categories.Walls
{
	[Serializable]
	public class WallsCategory:BaseStoreItemData, INamed, IId, ILevel, IAsset, IDamagePoints, IFireRate
	{
		[FormerlySerializedAs("Name")] public string name;
		public int id;
		public int level;
		public GameObject asset;
		public int damagePoints;
		[FormerlySerializedAs("attackSpeed")] public float fireRate;
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

		public int GetDamagePoints()
		{
			return damagePoints;
		}

		public float GetFireRate()
		{
			return fireRate;
		}

		public int GetLevel()
		{
			return level;
		}
	}
}
