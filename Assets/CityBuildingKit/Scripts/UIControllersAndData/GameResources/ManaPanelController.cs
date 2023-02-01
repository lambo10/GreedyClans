/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using Assets.Scripts.UIControllersAndData.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIControllersAndData.GameResources
{
	public class ManaPanelController:MonoBehaviour 
	{
		[SerializeField] private Text _currentManaCountField;
		[SerializeField] private Text _maxManaCountField;
		//[SerializeField] private Slider _manaSlider;

		private void Awake()
		{
			Player.Player.Instance.PlayerEvt.AddListener(Display);
		}

		private void Display(PlayerData data)
		{
			//_currentManaCountField.text = data.PlayerResources.Mana.CurrentValue.ToString();
			//_maxManaCountField.text = data.PlayerResources.Mana.MaxValue.ToString();
			//_manaSlider.value = (float)data.PlayerResources.Mana.CurrentValue / data.PlayerResources.Mana.MaxValue;
		}
	}
}
