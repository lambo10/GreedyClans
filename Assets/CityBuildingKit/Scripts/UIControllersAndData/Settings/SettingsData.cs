/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

namespace UIControllersAndData.Settings
{
	public class SettingsData
	{
		private bool _isSoundsEnabled;

		public bool IsSoundsEnabled
		{
			get { return _isSoundsEnabled; }
			set { _isSoundsEnabled = value; }
		}

		public bool IsAmbientEnabled
		{
			get { return _isAmbientEnabled; }
			set { _isAmbientEnabled = value; }
		}

		public bool IsMusicEnabled
		{
			get { return _isMusicEnabled; }
			set { _isMusicEnabled = value; }
		}

		private bool _isAmbientEnabled;
		private bool _isMusicEnabled;
	}
}
