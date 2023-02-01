/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using Assets.Scripts.UIControllersAndData.Store;
using JetBrains.Annotations;

namespace UIControllersAndData.Store.ShopItems.Building
{
	public class BuildingItem:BaseShopItem 
	{
		public override void Initialize(DrawCategoryData data, ShopCategoryType shopCategoryType)
		{
			base.Initialize(data, shopCategoryType);
			
			if (data.BaseItemData != null)
			{
				TimeLabel.text = data.BaseItemData.TimeToBuild.ToString();
				QuantityOfItem.text = "0" + "/" + data.BaseItemData.MaxCountOfThisItem;
			}
			else
			{
				throw new Exception("base item data is null");
			}
		}
		
		//TODO: update this label with data
		[UsedImplicitly]
		public void UpdateQuantity(string data)
		{
			QuantityOfItem.text = "0" + "/" + data;
		}
	
	}
}
