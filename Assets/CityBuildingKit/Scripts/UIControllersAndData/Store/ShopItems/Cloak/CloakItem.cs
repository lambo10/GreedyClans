/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using UIControllersAndData;
using UIControllersAndData.Store.Categories.Cloak;
using UIControllersAndData.Store.ShopItems;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIControllersAndData.Store.ShopItems.Cloak
{
	public class CloakItem:BaseShopItem 
	{
		[SerializeField] private Text _rechargeAvailableNoLb;
		[SerializeField] private Text _rechargeAvailableLb;
		
		public override void Initialize(DrawCategoryData data, ShopCategoryType shopCategoryType)
		{
			base.Initialize(data, shopCategoryType);
			CloakCategory itemData = data.BaseItemData as CloakCategory;

			if (itemData != null)
			{
				_rechargeAvailableNoLb.text = itemData.recharge;
				_rechargeAvailableLb.text = itemData.rechargeAvailable;
			}
		}	
	}
}
