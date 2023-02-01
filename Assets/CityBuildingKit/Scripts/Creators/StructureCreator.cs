using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UIControllersAndData.Store;
using UIControllersAndData.GameResources;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Buildings;
using UIControllersAndData.Store.Categories.Military;

public class StructureCreator : BaseCreator
{

	private int index;	//instead of xml index building 2,3 producing/storing something, we will have 0,1,2...
							//xml order: Forge Generator Vault Barrel Summon

	void Start () {

		InitializeComponents ();		//this is the only class who will initiate component, since there is no need to receive thee same call from all children
		ReadStructures();
		StartCoroutine("UpdateLabelStats");

		if (structureXMLTag == "Building")
		{
			List<BuildingCategoryLevels> buildingCategoryLevels = ShopData.Instance.BuildingsCategoryData.category;
			List<BuildingsCategory> buildingsCategoryData = buildingCategoryLevels.SelectMany(level => level.levels).Where(c => c.level == _structuressWithLevel).ToList();
			
			List<MilitaryCategoryLevels> militaryCategoryLevels = ShopData.Instance.MilitaryCategoryData.category;
			List<MilitaryCategory> militaryCategoryData = militaryCategoryLevels.SelectMany(level => level.levels).Where(c => c.level == _structuressWithLevel).ToList();
			
			RegisterBasicEconomyValues(buildingsCategoryData);
			RegisterBasicEconomyValues(militaryCategoryData);
		}
		
	}
	
	private void RegisterBasicEconomyValues<T>(List<T> data) where T:IProdBuilding, IStoreBuilding, IStructure 
	{
		for (int i = 0; i < data.Count; i++) 
		{
			bool isvalid = false;

			if (data[i].GetProdType() != GameResourceType.None)
			{
				((ResourceGenerator)resourceGenerator).basicEconomyValues [index].ProdType = data[i].GetProdType();
				((ResourceGenerator)resourceGenerator).basicEconomyValues [index].ProdPerHour = data[i].GetProdPerHour();
				isvalid = true;
			}
			if (data[i].GetStoreType() != StoreType.None) 
			{
				((ResourceGenerator)resourceGenerator).basicEconomyValues [index].StoreType = data[i].GetStoreType().ToString();//Internal, Distributed
				((ResourceGenerator)resourceGenerator).basicEconomyValues [index].StoreResource = data[i].GetStoreResource().ToString();
				((ResourceGenerator)resourceGenerator).basicEconomyValues [index].StoreCap = data[i].GetStoreCap();
				isvalid = true;
			}

			if (isvalid) 
			{				
				((ResourceGenerator)resourceGenerator).basicEconomyValues [index].StructureType = data[i].GetStructureType();
				index++;
			}
		}
	}
}
