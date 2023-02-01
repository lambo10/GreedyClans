using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UIControllersAndData.Store;
using UIControllersAndData.GameResources;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UnityEngine.Serialization;

public class ResourceGenerator : MonoBehaviour {

	private const int noOfEconomyBuildings = 6;		//xml order: Forge Generator Vault Barrel Summon Tatami

	[HideInInspector]//this shows as empty in inspector - probably because it's a custom class?
	public EconomyBuilding[] basicEconomyValues = new EconomyBuilding[noOfEconomyBuildings]; //newly created buildings will request this - to avoid saving a lot of values

	//this shows as empty in inspector - probably because it's a custom class?
	[FormerlySerializedAs("existingEconomyBuildings")] 
	public List<EconomyBuilding> _existingEconomyBuildings = new List<EconomyBuilding>();

	public List<EconomyBuilding> ExistingEconomyBuildings
	{
		get { return _existingEconomyBuildings; }
		set
		{
			_existingEconomyBuildings = value;
		}
	}

	[FormerlySerializedAs("messageNotifications")] 
	public List<MessageNotification> _messageNotifications = new List<MessageNotification>();

	public List<MessageNotification> MessageNotifications
	{
		get => _messageNotifications;
		set => _messageNotifications = value;
	}

	//[HideInInspector]
	public int index = -1;

	public GameObject BasicValues;
	private Component stats;

	// Use this for initialization
	void Start () {		
			
		stats = GameObject.Find("Stats").GetComponent<Stats>();	
		Initialize ();
		InitializeEconomy ();
	}

	private void Initialize()
	{
		for (int i = 0; i < basicEconomyValues.Length; i++) 
		{
			EconomyBuilding eB = BasicValues.AddComponent<EconomyBuilding> ();//new EconomyBuilding ();

			basicEconomyValues [i] = eB;
		}
	}

	private void RunEconomy()
	{
		RunProduction (1);
	}

	public void FastPaceEconomyAll(int minutesPass) //called by save/load games for already finished/functional buildings
	{
		RunProduction (60 * minutesPass);
	}

	public void InitializeEconomy()
	{
		InvokeRepeating ("RunEconomy", 1, 1);
	}

	private void RunProduction(int timefactor)//timefactor seconds=1 minutes=60
	{
		for (int i = 0; i < ExistingEconomyBuildings.Count; i++) 
		{
			GameResourceType prodType = ExistingEconomyBuildings [i].ProdType;

			if (prodType != GameResourceType.None) 
			{
				float produce = ((float)ExistingEconomyBuildings [i].ProdPerHour / 3600)*timefactor;
				bool displayNotification = false;

				switch (prodType) {
				case GameResourceType.Gold:
					if (ExistingEconomyBuildings [i].storedGold + produce < ExistingEconomyBuildings [i].StoreCap) 
					{
						ExistingEconomyBuildings [i].ModifyGoldAmount (produce);
						//if((float)existingEconomyBuildings [i].storedGold/existingEconomyBuildings [i].StoreCap>0.1f)//to display when 10% full
						if(ExistingEconomyBuildings [i].storedGold>1)
							displayNotification = true;
						
					}	
					else //fill storage
					{
						ExistingEconomyBuildings [i].ModifyGoldAmount 
						(ExistingEconomyBuildings [i].StoreCap-
							ExistingEconomyBuildings [i].storedGold);
						displayNotification = true;
					}

					if(displayNotification)
					DisplayHarvestNotification (ExistingEconomyBuildings [i].structureIndex, ExistingEconomyBuildings [i].storedGold);//i

					break;
				case GameResourceType.Mana:
					if (ExistingEconomyBuildings [i].storedMana + produce < ExistingEconomyBuildings [i].StoreCap) 
					{						
						ExistingEconomyBuildings [i].ModifyManaAmount (produce);
						//if((float)existingEconomyBuildings [i].storedMana/existingEconomyBuildings [i].StoreCap>0.1f)//to display when 10% full
						if(ExistingEconomyBuildings [i].storedMana>1)
							displayNotification = true;						
					}
					else //fill storage
					{
						ExistingEconomyBuildings [i].ModifyManaAmount 
						(ExistingEconomyBuildings [i].StoreCap-
							ExistingEconomyBuildings [i].storedMana);
						displayNotification = true;	
					}
					if(displayNotification)
					DisplayHarvestNotification (ExistingEconomyBuildings [i].structureIndex, ExistingEconomyBuildings [i].storedMana);	
					break;				
				}	

			}			
		}
	}

	public void FastPaceProductionIndividual(int buildingListIndex, int timefactor)//timefactor seconds=1 minutes=60
	{
		GameResourceType prodType = ExistingEconomyBuildings [buildingListIndex].ProdType;

			if (prodType != GameResourceType.None) 
			{
			float produce = ((float)ExistingEconomyBuildings [buildingListIndex].ProdPerHour / 3600)*60*timefactor;

			switch (prodType) {
			case GameResourceType.Gold:
				print ("produces gold");
				if (ExistingEconomyBuildings [buildingListIndex].storedGold + produce <= ExistingEconomyBuildings [buildingListIndex].StoreCap) 
				{
					ExistingEconomyBuildings [buildingListIndex].ModifyGoldAmount (produce);										
				} 
				else //fill storage
				{
					ExistingEconomyBuildings [buildingListIndex].ModifyGoldAmount 
					(ExistingEconomyBuildings [buildingListIndex].StoreCap-
						ExistingEconomyBuildings [buildingListIndex].storedGold);
				}
			break;

			case GameResourceType.Mana:
				print ("produces mana");
				if (ExistingEconomyBuildings [buildingListIndex].storedMana + produce <= ExistingEconomyBuildings [buildingListIndex].StoreCap) 
				{						
					ExistingEconomyBuildings [buildingListIndex].ModifyManaAmount (produce);
				} 
				else //fill storage
				{
					ExistingEconomyBuildings [buildingListIndex].ModifyManaAmount 
					(ExistingEconomyBuildings [buildingListIndex].StoreCap-
						ExistingEconomyBuildings [buildingListIndex].storedMana);
				}
			break;				
				}	
				
			}
	}

	public void RegisterMessageNotification(MessageNotification m)
	{
		MessageNotifications.Add (m);	//ProductionBuildings.Add (building);
	}

	private void DisplayHarvestNotification(int index, float amount)
	{
		for (int i = 0; i < MessageNotifications.Count; i++) {
			if (MessageNotifications [i].structureIndex == index) 
			{
				if (!MessageNotifications [i].isReady) 
				{
					MessageNotifications [i].FadeIn ();
					MessageNotifications [i].isReady = true;
				}
				MessageNotifications [i].SetLabel (0, "+ " + amount.ToString("0.00"));
				break;
			}
		}
	}
	private void ResetHarvestNotification(int index)
	{
		for (int i = 0; i < MessageNotifications.Count; i++) {
			if (MessageNotifications [i].structureIndex == index) 
			{
				if (MessageNotifications [i].isReady) 
				{
					MessageNotifications [i].FadeOut ();
					MessageNotifications [i].isReady = false;
				}
				break;
			}
		}
	}
	public void Harvest(int index)
	{
		for (int i = 0; i < ExistingEconomyBuildings.Count; i++) 
		{
			if (ExistingEconomyBuildings [i].structureIndex == index) 
			{
				switch (ExistingEconomyBuildings [i].ProdType) 
				{
				case GameResourceType.Gold:
					((Stats)stats).AddResources ((int)ExistingEconomyBuildings [i].storedGold, 0, 0);
					ExistingEconomyBuildings [i].storedGold -= (int)ExistingEconomyBuildings [i].storedGold;
					break;
				case GameResourceType.Mana:
					((Stats)stats).AddResources (0, (int)ExistingEconomyBuildings [i].storedMana, 0);
					ExistingEconomyBuildings [i].storedMana -= (int)ExistingEconomyBuildings [i].storedMana;
					break;
				}
				ResetHarvestNotification(i);
				((Stats)stats).UpdateUI ();
				break;
			}	
		}
	}
	public EconomyBuilding GetEconomyBuilding(int index)
	{
		return ExistingEconomyBuildings [index];
	}

	public void UpdateBasicValues(int idOfStructure, ShopCategoryType categoryType, int level, string strutureType)
	{
		List<EconomyBuilding> tmp = BasicValues.GetComponents<EconomyBuilding>().ToList();

		var economyBuilding =tmp.Find(x => x.StructureType == strutureType);
		
		var levels = ShopData.Instance.GetLevels(idOfStructure, categoryType);
		var structure = levels.FirstOrDefault(x => ((ILevel) x).GetLevel() == level);
		if (structure != null && economyBuilding != null)
		{
			economyBuilding.ProdType = ((IProdBuilding)structure).GetProdType();
			economyBuilding.ProdPerHour = ((IProdBuilding)structure).GetProdPerHour();
			economyBuilding.StoreType = ((IStoreBuilding)structure).GetStoreType().ToString();//Internal, Distributed
			economyBuilding.StoreResource = ((IStoreBuilding)structure).GetStoreResource().ToString();
			economyBuilding.StoreCap = ((IStoreBuilding)structure).GetStoreCap();
		}
	}
	
	public void RemoveFromExisting(EconomyBuilding item)
	{
		Destroy(item);
		ExistingEconomyBuildings.Remove(item);
	}

	public void RemoveMessageNotifications(MessageNotification item)
	{
		MessageNotifications.Remove(item);
	}
}
