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
	public class GoldPanelController:MonoBehaviour
	{
		[SerializeField] private Text _currentGoldCountField;
		[SerializeField] private Text _maxGoldCountField;
		//[SerializeField] private Slider _goldSlider;

		private void Awake()
		{
			Player.Player.Instance.PlayerEvt.AddListener(Display);
		}

		private void Display(PlayerData data)
		{
			//_currentGoldCountField.text = data.PlayerResources.Gold.CurrentValue.ToString();
			//_maxGoldCountField.text = data.PlayerResources.Gold.MaxValue.ToString();
			//_goldSlider.value = (float)data.PlayerResources.Gold.CurrentValue / data.PlayerResources.Gold.MaxValue;
		}
	}
}
