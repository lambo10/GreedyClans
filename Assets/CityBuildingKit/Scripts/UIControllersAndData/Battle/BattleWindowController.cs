/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System.Collections.Generic;
using UIControllersAndData.Units;
using UnityEngine;

namespace Battle
{
	public class BattleWindowController:MonoBehaviour 
	{

		
		[SerializeField] private List<BattleUnit> _listOfUnItems;
        
		public void Initialize()
		{
			foreach (ExistedUnit existedUnit in Stats.Instance.ExistingUnits)
			{
				_listOfUnItems[existedUnit.id].Initialize(existedUnit.count);
			}
		}
	}
}
