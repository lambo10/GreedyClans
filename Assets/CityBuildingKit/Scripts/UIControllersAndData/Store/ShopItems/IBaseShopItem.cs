/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using Assets.Scripts.UIControllersAndData.Store;

namespace UIControllersAndData.Store.ShopItems
{
    public interface IBaseShopItem
    {
        void Initialize(DrawCategoryData data, ShopCategoryType shopCategoryType);
    }
}
