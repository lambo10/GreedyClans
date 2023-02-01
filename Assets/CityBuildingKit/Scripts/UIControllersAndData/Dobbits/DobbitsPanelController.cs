/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using Assets.Scripts.UIControllersAndData.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIControllersAndData.Dobbits
{
	public class DobbitsPanelController:MonoBehaviour
	{
		[SerializeField] private Text _dobbitField;
		[SerializeField] private Slider _dobbitSlider;

		 
		/// <summary>
		/// Called before Start
		/// </summary>
		private void Awake()
		{	
			Player.Player.Instance.PlayerEvt.AddListener(Display);
		}

		private void Display(PlayerData data)
		{
			_dobbitSlider.value = 1 - data.PlayerResources.Dobbit.CurrentValue / (float)data.PlayerResources.Dobbit.MaxValue;	
			_dobbitField.text = (data.PlayerResources.Dobbit.MaxValue - data.PlayerResources.Dobbit.CurrentValue) + " / " + data.PlayerResources.Dobbit.MaxValue;
		}
	}
}
