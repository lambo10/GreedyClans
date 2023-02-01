/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using System.Xml.Serialization;
using Assets.Scripts.UIControllersAndData.Cloak;
using Assets.Scripts.UIControllersAndData.GameResources;
using Assets.Scripts.UIControllersAndData.Models;
using Assets.Scripts.UIControllersAndData.Storages;
using UIControllersAndData.Settings;

namespace Assets.Scripts.UIControllersAndData.Player
{
	[Serializable]
	public class PlayerData:IModel<PlayerData>
	{
		private string _playerName;

		public PlayerData()
		{
			_playerName = "Player";
			LevelData = new LevelData(){Level = 1};
			ExperienceData = new ExperienceData();
			PlayerResources = new PlayerResources();
			CloakData = new CloakData();
			SettingsData = new SettingsData(){ IsSoundsEnabled = true, IsAmbientEnabled = true, IsMusicEnabled = true};
			_allUnits = 0;
		}
		
		public string PlayerName
		{
			get { return _playerName; }
			set { _playerName = value; }
		}

		public LevelData LevelData { get; private set; }

		public ExperienceData ExperienceData { get; private set; }

		public PlayerResources PlayerResources { get; private set; }

		//TODO: check it, maybe to some separate class
		private int _allUnits;

		public int AllUnits
		{
			get { return _allUnits; }
			set { _allUnits = value; }
		}


		public SettingsData SettingsData { get; private set; }

		public CloakData CloakData { get; private set; }


		[XmlIgnore]
		public IModelFactory<PlayerData> ModelFactory { get; set; }
	}
}