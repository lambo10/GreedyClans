/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using Assets.Scripts.UIControllersAndData.Storages;
using JetBrains.Annotations;

namespace Assets.Scripts.UIControllersAndData.Player
{
	public class Player:IPlayer 
	{

		private PlayerEvent _playerEvt = new PlayerEvent();
		public PlayerEvent PlayerEvt
		{
			get { return _playerEvt; }
		}


		private readonly PlayerDataFactory _factory;
		[NotNull]
		private PlayerData _library;

		private static Player _instance;
		public static IPlayer Instance
		{
			get { return _instance ?? (_instance = new Player()); }
		}

		private Player()
		{
			_factory = new PlayerDataFactory(Configuration.DefaultStorage);
			_library = new PlayerData();
			_factory.Get(library => { _library = library; });
		}

		public void Save()
		{
			_factory.Put(_library);
		}
			
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public PlayerData GetPlayer()
		{
			return _library;
		}
	}
}