/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using Assets.Scripts.UIControllersAndData.Images;
using Assets.Scripts.UIControllersAndData.Player.MaxCap;
using Assets.Scripts.UIControllersAndData.Store.Categories.Cloak;
using Assets.Scripts.UIControllersAndData.Store.Categories.Store;
using UIControllersAndData.Store.Categories.Ambient;
using UIControllersAndData.Store.Categories.Buildings;
using UIControllersAndData.Store.Categories.Military;
using UIControllersAndData.Store.Categories.Unit;
using UIControllersAndData.Store.Categories.Walls;
using UIControllersAndData.Store.Categories.Weapon;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.UIControllersAndData
{
	/// <summary>
	/// Helper to create asset files.
	/// </summary>
	public class AssetsHelper:MonoBehaviour 
	{

		/// <summary>
		/// Creates an asset file for MaxCap
		/// </summary>
		[MenuItem("Tools/Create max cap table")]
		static void CreateMaxCapTable()
		{
			MaxCapItemList asset = ScriptableObject.CreateInstance<MaxCapItemList>();
			AssetDatabase.CreateAsset(asset, Constants.PATH_FOR_MAX_CAP_ASSET_CREATE);
			AssetDatabase.SaveAssets();
		}
		
		/// <summary>
		/// Creates an asset file for Images
		/// </summary>
		[MenuItem("Tools/Create images table")]
		static void CreateImagesTable()
		{
			ImageItemList asset = ScriptableObject.CreateInstance<ImageItemList>();
			AssetDatabase.CreateAsset(asset, Constants.PATH_FOR_IMAGE_ASSET);
			AssetDatabase.SaveAssets();
		}
		
		/// <summary>
		/// Creates an asset file for store category
		/// </summary>
		[MenuItem("Tools/Create store category table")]
		static void CreateStoreCategoryTable()
		{
			StoreCategoryData asset = ScriptableObject.CreateInstance<StoreCategoryData>();
			AssetDatabase.CreateAsset(asset, Constants.PATH_FOR_STORE_CATEGORY_ASSET);
			AssetDatabase.SaveAssets();
		}
		
		/// <summary>
		/// Creates an asset file for buildings category
		/// </summary>
		[MenuItem("Tools/Create buildings category table")]
		static void CreateBuildingsCategoryTable()
		{
			BuildingsCategoryData asset = ScriptableObject.CreateInstance<BuildingsCategoryData>();
			AssetDatabase.CreateAsset(asset, Constants.PATH_FOR_BUILDINGS_CATEGORY_ASSET);
			AssetDatabase.SaveAssets();
		}
		
		/// <summary>
		/// Creates an asset file for walls category
		/// </summary>
		[MenuItem("Tools/Create walls category table")]
		static void CreateWallsCategoryTable()
		{
			WallsCategoryData asset = ScriptableObject.CreateInstance<WallsCategoryData>();
			AssetDatabase.CreateAsset(asset, Constants.PATH_FOR_WALLS_CATEGORY_ASSET);
			AssetDatabase.SaveAssets();
		}
		
		/// <summary>
		/// Creates an asset file for weapon category
		/// </summary>
		[MenuItem("Tools/Create weapon category table")]
		static void CreateWeaponCategoryTable()
		{
			WeaponCategoryData asset = ScriptableObject.CreateInstance<WeaponCategoryData>();
			AssetDatabase.CreateAsset(asset, Constants.PATH_FOR_WEAPON_CATEGORY_ASSET);
			AssetDatabase.SaveAssets();
		}
		
		/// <summary>
		/// Creates an asset file for ambient category
		/// </summary>
		[MenuItem("Tools/Create ambient category table")]
		static void CreateAmbientCategoryTable()
		{
			AmbientCategoryData asset = ScriptableObject.CreateInstance<AmbientCategoryData>();
			AssetDatabase.CreateAsset(asset, Constants.PATH_FOR_AMBIENT_CATEGORY_ASSET);
			AssetDatabase.SaveAssets();
		}
		
		/// <summary>
		/// Creates an asset file for unit category
		/// </summary>
		[MenuItem("Tools/Create unit category table")]
		static void CreateUnitCategoryTable()
		{
			UnitCategoryData asset = ScriptableObject.CreateInstance<UnitCategoryData>();
			AssetDatabase.CreateAsset(asset, Constants.PATH_FOR_UNIT_CATEGORY_ASSET);
			AssetDatabase.SaveAssets();
		}
		
		/// <summary>
		/// Creates an asset file for military category
		/// </summary>
		[MenuItem("Tools/Create military category table")]
		static void CreateMilitaryCategoryTable()
		{
			MilitaryCategoryData asset = ScriptableObject.CreateInstance<MilitaryCategoryData>();
			AssetDatabase.CreateAsset(asset, Constants.PATH_FOR_MILITARY_CATEGORY_ASSET);
			AssetDatabase.SaveAssets();
		}
		
		/// <summary>
		/// Creates an asset file for cloak category
		/// </summary>
		[MenuItem("Tools/Create cloak category table")]
		static void CreateCloakCategoryTable()
		{
			CloakCategoryData asset = ScriptableObject.CreateInstance<CloakCategoryData>();
			AssetDatabase.CreateAsset(asset, Constants.PATH_FOR_CLOAK_CATEGORY_ASSET);
			AssetDatabase.SaveAssets();
		}
	}
}
