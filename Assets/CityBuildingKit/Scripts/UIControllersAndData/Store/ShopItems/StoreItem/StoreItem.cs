/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using Assets.Scripts.UIControllersAndData.Store;
using UIControllersAndData.GameResources;
using UIControllersAndData.Store.Categories.Store;

namespace UIControllersAndData.Store.ShopItems.StoreItem
{
	public class StoreItem:BaseShopItem 
	{
		public override void Initialize(DrawCategoryData data, ShopCategoryType shopCategoryType)
		{
			base.Initialize(data, shopCategoryType);
			
			StoreCategory itemData = data.BaseItemData as StoreCategory;
			if (itemData == null)
			{
				throw new Exception("Item data is null");
			}
			QuantityOfItem.text = itemData.quantity.ToString();
			if (itemData.purchasableResource == GameResourceType.Crystal)
			{
				BuyButtonLabel.text = itemData.Price + "$";	
			}
		}
	}
}
