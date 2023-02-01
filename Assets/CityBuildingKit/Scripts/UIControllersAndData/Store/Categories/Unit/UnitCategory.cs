/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using Assets.Scripts.UIControllersAndData.Store;
using UIControllersAndData.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIControllersAndData.Store.Categories.Unit
{
	[System.Serializable]
	public class UnitCategory:BaseStoreItemData, INamed, IId, ILevel, IAsset, IDamagePoints, IFireRate, IDamageType
	{
		public string name;
		public int id;
		public int level;
		[FormerlySerializedAs("UnitType")] public UnitType unitType;
		[FormerlySerializedAs("Size")] public int size;
		public int priceToSpeedUpUpgrade;
		public GameObject asset;
		public int damagePoints;
		[FormerlySerializedAs("attackSpeed")] public float fireRate;
		public DamageType damageType;
		
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

		public DamageType GetDamageType()
		{
			return damageType;
		}
	}
}
