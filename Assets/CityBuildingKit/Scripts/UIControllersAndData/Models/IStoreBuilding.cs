/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2019.
 * All rights reserved.
 */

using Assets.Scripts.UIControllersAndData.Store;
using UIControllersAndData.GameResources;

namespace UIControllersAndData.Models
{
    public interface IStoreBuilding
    {
        int GetStoreCap();
        StoreType GetStoreType();
        GameResourceType GetStoreResource();
    }
}