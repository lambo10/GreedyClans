/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UIControllersAndData
{
	public class MoveBuildingPanelController:MonoBehaviour
	{
		public static MoveBuildingPanelController Instance;

		[SerializeField] private Button _okButton;
		[SerializeField] private Button _moveButton;
		[SerializeField] private Button _removeButton;
		[SerializeField] private GameObject _removeButtonObject;
		[FormerlySerializedAs("_upgradeBuildingButton")] 
		[SerializeField] private Button _upgradeStructureButton;

		private UnityAction _upgradeStructureAction;

		public UnityAction UpgradeBuildingAction
		{
			get => _upgradeStructureAction;
			set => _upgradeStructureAction = value;
		}

		public Button UpgradeStructureButton => _upgradeStructureButton;
		
		
		public Button MoveButton => _moveButton;

		public Button OkButton => _okButton;

		public Button RemoveButton => _removeButton;

		public GameObject RemoveButtonObject => _removeButtonObject;

		private UnityAction _lastOkAction;

		public UnityAction LastOkAction
		{
			get { return _lastOkAction; }
			set { _lastOkAction = value; }
		}

		private UnityAction _lastMoveAction;

		public UnityAction LastMoveAction
		{
			get { return _lastMoveAction; }
			set { _lastMoveAction = value; }
		}

		private void Awake()
		{
			Instance = this;
		}


		[SerializeField] private GameObject _panel;

		public GameObject Panel => _panel;
	}
}
