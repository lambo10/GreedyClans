/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2019.
 * All rights reserved.
 */

using UIControllersAndData.GameResources;

namespace UIControllersAndData.Models
{
    public interface IProdBuilding
    {
        GameResourceType GetProdType();
        int GetProdPerHour();
    }
}