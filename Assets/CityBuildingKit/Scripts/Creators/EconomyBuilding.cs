using UnityEngine;
using System.Collections;
using UIControllersAndData.GameResources;

public class EconomyBuilding : MonoBehaviour {

		
	public int structureIndex, ProdPerHour, StoreCap;								
	public string  StructureType, StoreType, StoreResource;

	public GameResourceType ProdType;
	
	public float storedGold, storedMana, storedSoldiers;

	public void ModifyGoldAmount(float f)
	{
		storedGold += f;
	}
	public void ModifyManaAmount(float f)
	{
		storedMana += f;
	}

}
