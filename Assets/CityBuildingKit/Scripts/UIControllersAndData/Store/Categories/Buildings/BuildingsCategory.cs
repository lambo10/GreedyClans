/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using Assets.Scripts.UIControllersAndData.Store;
using UIControllersAndData.GameResources;
using UIControllersAndData.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIControllersAndData.Store.Categories.Buildings
{
	[System.Serializable]
	public class BuildingsCategory:BaseStoreItemData, INamed, IId, ILevel, IAsset, IProdBuilding, IStoreBuilding, IStructure, IDamagePoints, IFireRate
	{
		[FormerlySerializedAs("Name")] public string name;
		[FormerlySerializedAs("Id")] public int id;
		[FormerlySerializedAs("Level")] public int level;
		[FormerlySerializedAs("Asset")] public GameObject asset;
		[FormerlySerializedAs("StructureType")] public string structureType; //same as prefab //TODO: I think it should be enum 
		[FormerlySerializedAs("ProdType")] public GameResourceType prodType;
		[FormerlySerializedAs("ProdPerHour")] public int prodPerHour;
		[FormerlySerializedAs("StoreType")] public StoreType storeType;
		[FormerlySerializedAs("StoreResource")] public GameResourceType storeResource;
		[FormerlySerializedAs("StoreCap")] public int storeCap;
		[FormerlySerializedAs("DamagePoints")] public int damagePoints;
		[FormerlySerializedAs("attackSpeed")] [FormerlySerializedAs("AttackSpeed")] public float fireRate;
		public GameResourceType GetProdType() 
		{
			return prodType;
		}

		public int GetProdPerHour()
		{
			return prodPerHour;
		}

		public int GetStoreCap()
		{
			return storeCap;
		}

		public StoreType GetStoreType()
		{
			return storeType;
		}

		public GameResourceType GetStoreResource()
		{
			return storeResource;
		}

		public string GetStructureType()
		{
			return structureType;
		}

		public float GetFireRate()
		{
			return fireRate;
		}

		public int GetDamagePoints()
		{
			return damagePoints;
		}

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
	}
}
