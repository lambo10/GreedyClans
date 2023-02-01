/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using Assets.Scripts.UIControllersAndData.Images;
using JetBrains.Annotations;
using UIControllersAndData.Store.Categories.Unit;
using UnityEngine;
using UnityEngine.UI;

namespace UIControllersAndData.Store.ShopItems.UnitItem
{
	public class UnitStatusItem:MonoBehaviour
	{

		[SerializeField] private Text _count;
		[SerializeField] private QIndex _qIndex;

		public QIndex QIndex => _qIndex;
		public Text Count => _count;
		public Slider Slider => _slider;
		public int Level => _level;
		
		[SerializeField] private Image _icon;
		[SerializeField] private Slider _slider;
		
		private UnitCategory _itemData;
		private int _level;

		public UnitCategory ItemData => _itemData;

		public void Initialize([NotNull] UnitCategory data, int level)
		{
			_itemData = data;
			_icon.sprite = ImageControler.GetImage(_itemData.IdOfBigIcon);
			_qIndex.Objindex = _itemData.GetId();
			_level = level;
		}
		
		[UsedImplicitly]
		public void OnCancel()
		{
			MenuUnit.Instance.UnbuildUnit(_itemData);
		}
	}
}
