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

namespace UIControllersAndData.Store.Categories.Military
{
	[System.Serializable]
	public class MilitaryCategory:BaseStoreItemData, INamed, IId, ILevel, IAsset, IProdBuilding, IStoreBuilding, IStructure
	{
		public string name;
		public int id;
		public int level;
		[FormerlySerializedAs("Asset")] public GameObject asset;
		[FormerlySerializedAs("StoreCap")] public int storeCap;
		[FormerlySerializedAs("StoreType")] public StoreType storeType;
		[FormerlySerializedAs("StoreResource")] public GameResourceType storeResource;
		[FormerlySerializedAs("ProdType")] public GameResourceType prodType;
		[FormerlySerializedAs("ProdPerHour")] public int prodPerHour;
		[FormerlySerializedAs("StructureType")] public string structureType;
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
