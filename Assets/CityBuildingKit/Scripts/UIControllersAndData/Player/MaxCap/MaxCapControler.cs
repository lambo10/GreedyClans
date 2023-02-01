/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

//using UnityEditor;

using UIControllersAndData.Player.MaxCap;
using UnityEngine;

namespace Assets.Scripts.UIControllersAndData.Player.MaxCap
{
	
	/// <summary>
	/// Basic class for MaxCapController
	/// </summary>
	public class MaxCapControler 
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public static int GetMaxCap(LevelData level)
		{
			MaxCapItemList list = LoadMaxCapTable();
			return list.MaxCapList.Find(x => x.Level == level.Level).MaxCap;
		}

		/// <summary>
		/// Loads max cap table
		/// </summary>
		/// <returns></returns>
		private static MaxCapItemList LoadMaxCapTable()
		{
			return Resources.Load<MaxCapItemList>(Constants.PATH_FOR_MAX_CAP_ASSET_LOAD);
		}
	}
}