/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using Assets.Scripts.UIControllersAndData.Player;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets.Scripts.UIControllersAndData.GameResources
{
	public class HousingPanelController:MonoBehaviour 
	{
		[SerializeField] private TextMeshProUGUI _currentHousingCountField;
		[SerializeField] private TextMeshProUGUI _maxHousingCountField;
		[SerializeField] private TextMeshProUGUI _allUnits;

		private void Awake()
		{
			Player.Player.Instance.PlayerEvt.AddListener(Display);
		}

		private void Display(PlayerData data)
		{
			//_currentHousingCountField.text = data.PlayerResources.Housing.CurrentValue.ToString();
			//_maxHousingCountField.text = data.PlayerResources.Housing.MaxValue.ToString();
			//_housingSlider.value = (float)data.PlayerResources.Housing.CurrentValue / data.PlayerResources.Housing.MaxValue;
			_allUnits.text = data.AllUnits.ToString();
		}
	}
}
