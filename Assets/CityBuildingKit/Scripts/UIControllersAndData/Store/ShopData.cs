/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.UIControllersAndData.Store;
using Assets.Scripts.UIControllersAndData.Store.Categories.Cloak;
using Assets.Scripts.UIControllersAndData.Store.Categories.Store;
using UIControllersAndData.Models;
using UIControllersAndData.Store.Categories.Ambient;
using UIControllersAndData.Store.Categories.Buildings;
using UIControllersAndData.Store.Categories.Military;
using UIControllersAndData.Store.Categories.Unit;
using UIControllersAndData.Store.Categories.Walls;
using UIControllersAndData.Store.Categories.Weapon;
using UnityEngine;

namespace UIControllersAndData.Store
{	
	public class ShopData
	{
		private static ShopData _instance;

		public static ShopData Instance => _instance ?? (_instance = new ShopData());


		private StoreCategoryData _storeCategoryData;
		private BuildingsCategoryData _buildingsCategoryData;
		private MilitaryCategoryData _militaryCategoryData;
		private WallsCategoryData _wallsCategoryData;
		private WeaponCategoryData _weaponCategoryData;
		private UnitCategoryData _unitCategoryData;
		private CloakCategoryData _cloakCategoryData;
		private AmbientCategoryData _ambientCategoryData;

		public StoreCategoryData StoreCategoryData
		{
			get
			{
				if (_storeCategoryData == null)
				{
					_storeCategoryData =
						(StoreCategoryData) LoadShopCategoryTable<StoreCategoryData>(ShopCategoryType.Store);
				}
				return _storeCategoryData;
			}
		}
		
		public BuildingsCategoryData BuildingsCategoryData
		{
			get
			{
				if (_buildingsCategoryData == null)
				{
					_buildingsCategoryData =
						(BuildingsCategoryData) LoadShopCategoryTable<BuildingsCategoryData>(ShopCategoryType.Buildings);
				}
				return _buildingsCategoryData;
			}
		}

		public MilitaryCategoryData MilitaryCategoryData
		{
			get
			{
				if (_militaryCategoryData == null)
				{
					_militaryCategoryData =
						(MilitaryCategoryData) LoadShopCategoryTable<MilitaryCategoryData>(ShopCategoryType.Military);
				}
				return _militaryCategoryData;
			}
		}

		public WallsCategoryData WallsCategoryData
		{
			get
			{
				if (_wallsCategoryData == null)
				{
					_wallsCategoryData =
						(WallsCategoryData) LoadShopCategoryTable<WallsCategoryData>(ShopCategoryType.Walls);
				}
				return _wallsCategoryData;
			}
		}
		
		public WeaponCategoryData WeaponCategoryData
		{
			get
			{
				if (_weaponCategoryData == null)
				{
					_weaponCategoryData =
						(WeaponCategoryData) LoadShopCategoryTable<WeaponCategoryData>(ShopCategoryType.Weapon);
				}
				return _weaponCategoryData;
			}
		}
		
		public UnitCategoryData UnitCategoryData
		{
			get
			{
				if (_unitCategoryData == null)
				{
					_unitCategoryData =
						(UnitCategoryData) LoadShopCategoryTable<UnitCategoryData>(ShopCategoryType.Unit);
				}
				return _unitCategoryData;
			}
		}
		
		public CloakCategoryData CloakCategoryData
		{
			get
			{
				if (_cloakCategoryData == null)
				{
					_cloakCategoryData =
						(CloakCategoryData) LoadShopCategoryTable<CloakCategoryData>(ShopCategoryType.Cloak);
				}
				return _cloakCategoryData;
			}
		}
		
		public AmbientCategoryData AmbientCategoryData
		{
			get
			{
				if (_ambientCategoryData == null)
				{
					_ambientCategoryData =
						(AmbientCategoryData) LoadShopCategoryTable<AmbientCategoryData>(ShopCategoryType.Ambient);
				}
				return _ambientCategoryData;
			}
		}
		
		/// <summary>
		/// Loads shop category cap table
		/// </summary>
		/// <returns></returns>
		private static object LoadShopCategoryTable<T>(ShopCategoryType categoryType)
		{
			switch (categoryType)
			{
				case ShopCategoryType.Store:
					return (T) (object) Resources.Load<StoreCategoryData>(Constants.PATH_FOR_STORE_CATEGORY_ASSET_LOAD);					
				case ShopCategoryType.Buildings:
					return (T) (object) Resources.Load<BuildingsCategoryData>(Constants.PATH_FOR_BUILDINGS_CATEGORY_ASSET_LOAD); 
				case ShopCategoryType.Military:
					return (T) (object) Resources.Load<MilitaryCategoryData>(Constants.PATH_FOR_MILITARY_CATEGORY_ASSET_LOAD);
				case ShopCategoryType.Walls:
					return (T) (object) Resources.Load<WallsCategoryData>(Constants.PATH_FOR_WALLS_CATEGORY_ASSET_LOAD);
				case ShopCategoryType.Weapon:
					return (T) (object) Resources.Load<WeaponCategoryData>(Constants.PATH_FOR_WEAPON_CATEGORY_ASSET_LOAD);
				case ShopCategoryType.Unit:
					return (T) (object) Resources.Load<UnitCategoryData>(Constants.PATH_FOR_UNIT_CATEGORY_ASSET_LOAD);
				case ShopCategoryType.Cloak:
					return (T) (object) Resources.Load<CloakCategoryData>(Constants.PATH_FOR_CLOAK_CATEGORY_ASSET_LOAD);
				case ShopCategoryType.Ambient:
					return (T) (object) Resources.Load<AmbientCategoryData>(Constants.PATH_FOR_AMBIENT_CATEGORY_ASSET_LOAD);	
			}
			return null;
		}
		
		/**
		 * Get list of items
		 */
		public List<BaseStoreItemData> GetLevels(int id, ShopCategoryType category)
		{
			switch (category)
			{
				case ShopCategoryType.Buildings:
					return	BuildingsCategoryData.category.Find(x => x.id == id).levels.Select(item => item as BaseStoreItemData).ToList();
				case ShopCategoryType.Military:
					return MilitaryCategoryData.category.Find(x => x.id == id).levels.Select(item => item as BaseStoreItemData).ToList();
				case ShopCategoryType.Weapon:
					return WeaponCategoryData.category.Find(x => x.id == id).levels.Select(item => item as BaseStoreItemData).ToList();
				case ShopCategoryType.Walls:
					return WallsCategoryData.category.Find(x => x.id == id).levels.Select(item => item as BaseStoreItemData).ToList();
				case ShopCategoryType.Ambient:
					return AmbientCategoryData.category.Find(x => x.id == id).levels.Select(item => item as BaseStoreItemData).ToList();
				case ShopCategoryType.Unit:
					return UnitCategoryData.category.Find(x => x.id == id).levels.Select(item => item as BaseStoreItemData).ToList();
			}
			return null;
		}

		public GameObject GetAssetForLevel(int id, ShopCategoryType categoryType, int level)
		{
			var levels = GetLevels(id, categoryType);
			GameObject asset;
			if (levels != null)
			{
				asset = ((IAsset) levels.Find(x => ((ILevel) x).GetLevel() == level)).GetAsset();
				if (asset)
				{
					return asset;
				}
				throw new Exception("Can't get an asset. The id is: " + id + ", the category is: " + categoryType + ", the level is: " + level);
			}
			return null;
		}
	}
}