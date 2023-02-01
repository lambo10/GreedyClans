/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using UIControllersAndData.GameResources;
using UIControllersAndData.Models;
using UnityEngine.Serialization;

namespace UIControllersAndData.Store.Categories.Store
{
	[System.Serializable]
	public class StoreCategory:BaseStoreItemData, INamed, IId
	{
		public string name;
		public int id;
		
		[FormerlySerializedAs("Quantity")] public int quantity;
		[FormerlySerializedAs("PurchasableResource")] public GameResourceType purchasableResource;
		public string GetName()
		{
			return name;
		}

		public int GetId()
		{
			return id;
		}
	}
}
