/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using Assets.Scripts.UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Unit;
using UnityEngine;
using UnityEngine.UI;

namespace UIControllersAndData.Store.ShopItems.UnitItem
{
	public class UnitItem:BaseShopItem 
	{
		[SerializeField] private Text _timeLabel;
	
		
		public override void Initialize(DrawCategoryData data, ShopCategoryType shopCategoryType)
		{
			base.Initialize(data, shopCategoryType);

			if (data.BaseItemData == null)
			{
				throw new Exception("Item data is null");
			}

			//QuantityOfItem.text = (data.BaseItemData as UnitCategory)?.size.ToString();
		}
	}
}
