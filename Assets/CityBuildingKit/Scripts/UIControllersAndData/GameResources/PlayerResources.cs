/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

namespace Assets.Scripts.UIControllersAndData.GameResources
{
	
	public class PlayerResources 
	{
		public PlayerResources() 
		{
			_dobbit = new ResourceData();
			_housing = new ResourceData();
			_gold = new ResourceData();
			_mana = new ResourceData();
			_crystal = new ResourceData();
		}

		private ResourceData _dobbit;

		public ResourceData Dobbit
		{
			get { return _dobbit; }
			set { _dobbit = value; }
		}

		public ResourceData Gold
		{
			get { return _gold; }
			set { _gold = value; }
		}

		public ResourceData Mana
		{
			get { return _mana; }
			set { _mana = value; }
		}

		public ResourceData Crystal
		{
			get { return _crystal; }
			set { _crystal = value; }
		}

		private ResourceData _housing;

		public ResourceData Housing
		{
			get { return _housing; }
			set { _housing = value; }
		}

		private ResourceData _gold;
		private ResourceData _mana;
		private ResourceData _crystal;
	}
}