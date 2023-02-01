/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2019.
 * All rights reserved.
 */

using System;
using System.Linq;
using Assets.Scripts.UIControllersAndData.Store;
using JetBrains.Annotations;
using TMPro;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Unit;
using UnityEngine;

namespace UIControllersAndData.Units
{
	public class UpgradeArmyWindow:MonoBehaviour
	{
		public static UpgradeArmyWindow Instance;
		
		[SerializeField] private UpgradeUnitItem _item;
		[SerializeField] private Transform _content;
		[SerializeField] private GameObject _infoText;
		
		/// <summary>
		/// Called before Start
		/// </summary>
		void Awake()
		{
			Instance = this;
			
			if (!_item)
			{
				throw new Exception("The _item is null");
			}

			if (!_content)
			{
				throw new Exception("The _content is null");
			}

			if (!_infoText)
			{
				throw new Exception("The _infoText is null");
			}
		
		}

		/**
		 * Use to initialize units for upgrade
		 */
		[UsedImplicitly]
		public void Initialize()
		{
			foreach (ExistedUnit existedUnit in Stats.Instance.ExistingUnits)
			{
				var levels = ShopData.Instance.GetLevels(existedUnit.id, ShopCategoryType.Unit);

				var unit = levels?.FirstOrDefault(x => ((ILevel) x).GetLevel() == existedUnit.level + 1);
				if (unit != null)
				{
					UpgradeUnitItem item = Instantiate(_item, _content, false);
					item.Initialize(unit);
				}	
			}

			SetInfoVisibility();
		}

		[UsedImplicitly]
		public void CloseHandler()
		{
			BroadcastMessage("AutoRemove", SendMessageOptions.DontRequireReceiver);
		}

		public void SetInfoVisibility()
		{
			_infoText.SetActive(_content.childCount == 0);
		}
	}
}
