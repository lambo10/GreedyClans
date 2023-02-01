/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using UIControllersAndData.Models;
using UnityEngine.Serialization;

namespace UIControllersAndData.Store.Categories.Cloak
{
    [System.Serializable]
    public class CloakCategory:BaseStoreItemData, INamed, IId
    {
        public string name;
        public int id;
        [FormerlySerializedAs("Recharge")] public string recharge;
        [FormerlySerializedAs("RechargeAvailable")] public string rechargeAvailable; //I think it should be int value. 
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
