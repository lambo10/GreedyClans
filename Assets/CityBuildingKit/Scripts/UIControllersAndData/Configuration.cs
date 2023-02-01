/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using Assets.Scripts.UIControllersAndData.Storages;
using JetBrains.Annotations;

namespace Assets.Scripts.UIControllersAndData
{
	/**
	 * This class keeps configuration of the application.
	 */
	public static class Configuration
	{
		#region PrivateProperties
		private static readonly IPersistentStorage _defaultStorage = new FileSystemStorage();
		private static readonly SnappingConfig _snapping = new SnappingConfig{IsGridSnappingEnabled = false};
		#endregion //PrivateProperties

		#region Properties
		/// <summary>
		/// Default persistent storage.
		/// </summary>
		[NotNull]
		public static IPersistentStorage DefaultStorage
		{
			get {return _defaultStorage;}
		}

		/// <summary>
		/// Snapping configuration.
		/// </summary>
		[NotNull]
		public static SnappingConfig Snapping
		{
			get { return _snapping; }
		}
		#endregion //Properties;
	}

	public class SnappingConfig
	{
		public bool IsGridSnappingEnabled;
		public bool IsAudioSnappingEnabled;
	}
}