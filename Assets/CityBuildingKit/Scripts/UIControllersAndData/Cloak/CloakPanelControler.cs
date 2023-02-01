/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using Assets.Scripts.UIControllersAndData.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIControllersAndData.Cloak
{		
	public class CloakPanelControler:MonoBehaviour
	{
		[SerializeField] private Text _cloakField;
		[SerializeField] private Slider _cloakSlider;
		
		void Awake() 
		{
			Player.Player.Instance.PlayerEvt.AddListener(Display);
		}

		[SerializeField]
		private Text _field;


		private void Display(PlayerData data)
		{
			_cloakField.text = data.CloakData.RemainingCloakTime + " / " + data.CloakData.PurchasedCloakTime;
			_cloakSlider.value = 1;
		}
	}
}