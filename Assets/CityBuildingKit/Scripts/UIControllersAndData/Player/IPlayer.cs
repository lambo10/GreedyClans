/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

namespace Assets.Scripts.UIControllersAndData.Player
{
    public interface IPlayer
    {

        PlayerData GetPlayer();

        void Save();

        PlayerEvent PlayerEvt { get; }
    }
}
