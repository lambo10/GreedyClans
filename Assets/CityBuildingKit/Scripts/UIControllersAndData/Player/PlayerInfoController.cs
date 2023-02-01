/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using Assets.Scripts.UIControllersAndData.Player.MaxCap;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIControllersAndData.Player{
	
	
	public class PlayerInfoController:MonoBehaviour 
	{
		[SerializeField] private Text _playerNameField;
		[SerializeField] private Text _levelField;
		[SerializeField] private Text _experienceField;
		[SerializeField] private Slider _experienceSlider;

		/// <summary>
		/// Called before Start
		/// </summary>
		private void Awake()
		{
			Player.Instance.PlayerEvt.AddListener(Display);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		private void Display(PlayerData data) 
		{
			// TODO implement here
			_playerNameField.text = data.PlayerName;
			_levelField.text = data.LevelData.Level.ToString();
			_experienceField.text = data.ExperienceData.CurrentExp + "/" + MaxCapControler.GetMaxCap(data.LevelData);


			_experienceSlider.value = data.ExperienceData.CurrentExp/(float)MaxCapControler.GetMaxCap(data.LevelData);
			
//			Player.Instance.Save();
		}

	}
}