/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using Assets.Scripts.UIControllersAndData.Store;
using UIControllersAndData.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIControllersAndData.Store.Categories.Weapon
{
	[System.Serializable]
	public class WeaponCategory:BaseStoreItemData, INamed, IId, ILevel, IAsset, IDamagePoints, IFireRate, IDamageType
	{
		[FormerlySerializedAs("Name")] public string name;
		[FormerlySerializedAs("Id")] public int id;
		public int level;
		public GameObject asset;
		public int damagePoints;
		[FormerlySerializedAs("Range")] public int range;
		[FormerlySerializedAs("FireRate")] public float fireRate;
		[FormerlySerializedAs("DamageType")] public DamageType damageType;
		[FormerlySerializedAs("TargetType")] public TargetType targetType;
		[FormerlySerializedAs("PreferredTarget")] public PreferredTarget preferredTarget;
		[FormerlySerializedAs("DamageBonus")] public int damageBonus;
		public string GetName()
		{
			return name;
		}

		public int GetId()
		{
			return id;
		}

		public int GetLevel()
		{
			return level;
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

		public DamageType GetDamageType()
		{
			return damageType;
		}
	}
}
