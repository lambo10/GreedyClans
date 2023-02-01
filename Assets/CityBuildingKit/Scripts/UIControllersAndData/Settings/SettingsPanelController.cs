/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using Assets.Scripts.UIControllersAndData.Player;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace UIControllersAndData.Settings
{
	public class SettingsPanelController:MonoBehaviour
	{
		public static SettingsPanelController Instance;
		
		
		[SerializeField] private Toggle _soundsToggle;
		[SerializeField] private Toggle _ambientToggle;
		[SerializeField] private Toggle _musicToggle;

		private PlayerData _data;

		private void Awake()
		{
			Instance = this;
			_data = Assets.Scripts.UIControllersAndData.Player.Player.Instance.GetPlayer();
		}

		//TODO: save/load it with new mechanism (local, prefs, server)...

		public void UpdateSettings()
		{
			_soundsToggle.isOn = _data.SettingsData.IsSoundsEnabled;
			_ambientToggle.isOn = _data.SettingsData.IsAmbientEnabled;
			_musicToggle.isOn = _data.SettingsData.IsMusicEnabled;
		}
		
		[UsedImplicitly]
		public void SaveGameLocalFileHandler()
		{
			SaveLoadMap.Instance.SaveGameLocalFile();
		}

		[UsedImplicitly]
		public void LoadFromLocalFileHandler()
		{
			SaveLoadMap.Instance.LoadFromLocalFile();
		}

		[UsedImplicitly]
		public void SaveToServerHandler()
		{
			SaveLoadWWW.Instance.SaveToServer();
		}

		[UsedImplicitly]
		public void LoadFromServerHandler()
		{
			SaveLoadWWW.Instance.LoadFromServer();
		}

		[UsedImplicitly]
		public void SaveGamePlayerPrefsHandler()
		{
			SaveLoadMap.Instance.SaveGamePlayerPrefs();
		}

		[UsedImplicitly]
		public void LoadFromPlayerPrefsHandler()
		{
			SaveLoadMap.Instance.LoadFromPlayerPrefs();
		}

		[UsedImplicitly]
		public void SavePlayerPrefsToServerHandler()
		{
			SaveLoadWWW.Instance.SavePlayerPrefsToServer();
		}

		[UsedImplicitly]
		public void LoadPlayerPrefsFromServer()
		{
			SaveLoadWWW.Instance.LoadPlayerPrefsFromServer();
		}

		[UsedImplicitly]
		public void LoadInitialMapFromServer()
		{
			SaveLoadWWW.Instance.LoadInitialMapFromServer();
		}

		[UsedImplicitly]
		public void SoundsHandler()
		{
			SoundFX.Instance.ToggleSound();
			_data.SettingsData.IsSoundsEnabled = _soundsToggle.isOn;
		}

		[UsedImplicitly]
		public void AmbientHandler()
		{
			SoundFX.Instance.ToggleAmbient();
			_data.SettingsData.IsAmbientEnabled = _ambientToggle.isOn;
		}

		[UsedImplicitly]
		public void MusicHandler()
		{
			SoundFX.Instance.ToggleMusic();
			_data.SettingsData.IsMusicEnabled = _musicToggle.isOn;
		}
		
		[UsedImplicitly]
		public void ExitHanlder()
		{
			Application.Quit();
		}
		
	}
}
