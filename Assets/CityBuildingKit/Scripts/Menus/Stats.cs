using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Assets.Scripts.Menus;
using Assets.Scripts.UIControllersAndData;
using Assets.Scripts.UIControllersAndData.GameResources;
using Assets.Scripts.UIControllersAndData.Images;
using Assets.Scripts.UIControllersAndData.Player;
using UIControllersAndData.GameResources;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Ambient;
using UIControllersAndData.Store.Categories.Buildings;
using UIControllersAndData.Store.Categories.Military;
using UIControllersAndData.Store.Categories.Walls;
using UIControllersAndData.Store.Categories.Weapon;
using UIControllersAndData.Units;

public class Stats : MonoBehaviour { //the resource bars on top of the screen

	public static Stats Instance;

    [SerializeField]
    private bool _showTutorial = false;
    private bool tutorialCitySeen = false;

    private int hours, minutes, seconds;				//for cloak remaining label

    private const int
	    noOfCreators = 4,
	    noOfUnits = 12;					//correlate with MenuUnitBase.cs/ verify that the Inspector has 12 elements

	public BaseCreator[] creators = new BaseCreator[noOfCreators]; //the store must update other creators/interfaces after purchases

	public GameObject GhostHelper;

    public bool
        gameWasLoaded = false,
        tutorialBattleSeen = false,
        removablesCreated = false,
        resourcesAdded = false,
        resourcesSubstracted = false;        

    public int 	//when user hard buys resources, storage capacity permanently increases 
		
		structureIndex = -1,			//unique index that all structures have - buildings, weapons, walls, etc
		townHallLevel = 0,				
		level = 1,						//player level
		experience = 1,
		maxExperience = 100, 			//save this too, used for progress bar to next level
		
		dobbits = 1, 
		occupiedDobbits = 0,

		remainingCloakTime,
		purchasedCloakTime,

		occupiedHousing = 0, 		//based on size, not number of units
		maxHousing = 0,				//housing refers ONLY to soldiers, not npc/dobbits
		
		gold = 5000, 
		maxGold = 10000, 

		mana = 500,		
		maxMana = 1000, 

		crystals = 5,
		maxCrystals = 5, 

		
		deltaGoldPlus, deltaGoldMinus,					//when a resource is added/spent, it starts a counter; must be separate because the operations might be simultaneous  
		deltaManaPlus, deltaManaMinus,
		deltaCrystalsPlus, deltaCrystalsMinus;

	//public float[] productionRates;

	public int[]
		sizePerUnit = new int[noOfUnits];			//based on size, a soldier can occupy more than 1 space


	private List<ExistedUnit> _existingUnits = new List<ExistedUnit>(); //existing units

	public List<ExistedUnit> ExistingUnits
	{
		get => _existingUnits;
		set => _existingUnits = value;
	}

	public List<int> maxBuildingsAllowed;
	public List<int> maxWallsAllowed;
	public List<int> maxWeaponsAllowed;
	public List<int> maxAmbientsAllowed;
	

	public TextAsset EvoStructuresXML;	//variables for loading building characteristics from XML
	protected List<Dictionary<string,string>> levels = new List<Dictionary<string,string>>();
	protected Dictionary<string,string> dictionary;

	private Component transData;

	//Interface connections

	public Store store;//this component is disabled, GameObject.Find doesn't work

    public bool ShowTutorial
    {
        get
        {
            return _showTutorial;
        }

        set
        {
            _showTutorial = value;
        }
    }

    public bool TutorialCitySeen
    {
        get
        {
            return tutorialCitySeen;
        }

        set
        {
            tutorialCitySeen = value;
        }
    }

    private void Awake()
	{
		Instance = this;
	}

	void Start () {
		
		transData = GameObject.Find ("TransData").GetComponent<TransData> ();
		StartCoroutine ("ReturnFromBattle");
        if(ShowTutorial)
        {
            StartCoroutine("LaunchTutorial");
        }
        GhostHelper.SetActive(_showTutorial);

        UpdateBuildingData ();
		UpdateWallData ();
		UpdateWeaponData ();
		UpdateAmbientData ();

		UpdateUnitsNo ();
		StartCoroutine ("LateUpdateUI");

		
	}

	private IEnumerator LateUpdateUI()
	{
		yield return new WaitForSeconds (3);
		UpdateUI ();
	}

	private void UpdateBuildingData()
	{
		foreach (var lvl in ShopData.Instance.BuildingsCategoryData.category)
		{
			foreach (BuildingsCategory buildingsCategory in lvl.levels)
			{
				if (buildingsCategory.level == 1)
				{
					maxBuildingsAllowed.Add(buildingsCategory.MaxCountOfThisItem);	
				}
			}
		}

		foreach (var lvl in ShopData.Instance.MilitaryCategoryData.category)
		{
			foreach (MilitaryCategory militaryCategory in lvl.levels)
			{
				if (militaryCategory.level == 1)
				{
					maxBuildingsAllowed.Add(militaryCategory.MaxCountOfThisItem);	
				}
			}
		}
		
	}
	
	private void UpdateWallData()
	{
		foreach (var lvl in ShopData.Instance.WallsCategoryData.category)
		{
			foreach (WallsCategory wallCategory in lvl.levels)
			{
				if (wallCategory.level == 1)
				{
					maxWallsAllowed.Add(wallCategory.MaxCountOfThisItem);	
				}
			}
		}
	}

	private void UpdateWeaponData()
	{
		foreach (var lvl in ShopData.Instance.WeaponCategoryData.category)
		{
			foreach (WeaponCategory weaponCategory in lvl.levels)
			{
				if (weaponCategory.level == 1)
				{
					maxWeaponsAllowed.Add(weaponCategory.MaxCountOfThisItem);	
				}
			}
		}
	}
	private void UpdateAmbientData()
	{
		foreach (var lvl in ShopData.Instance.AmbientCategoryData.category)
		{
			foreach (AmbientCategory ambientCategory in lvl.levels)
			{
				if (ambientCategory.level == 1)
				{
					maxAmbientsAllowed.Add(ambientCategory.MaxCountOfThisItem);	
				}
			}
		}
	}
	
	public bool EnoughGold(int goldPrice)
	{
		return (goldPrice <= gold + deltaGoldPlus - deltaGoldMinus);
	}

	public bool EnoughMana(int manaPrice)
	{
		return (manaPrice <= mana + deltaManaPlus - deltaManaMinus);
	}

	public bool EnoughCrystals(int crystalPrice)
	{
		return ( crystalPrice <= crystals + deltaCrystalsPlus - deltaCrystalsMinus);
	}

	public void AddResources(int dGold, int dMana, int dCrystals)
	{
		deltaGoldPlus += dGold;
		deltaManaPlus += dMana;
		deltaCrystalsPlus += dCrystals;
			
		if (!resourcesAdded) {			
			resourcesAdded = true;
			InvokeRepeating ("GradualAddResources", 0.1f, 0.1f);
		}

		UpdateCreatorMenus ();
		//updates in sequence all buttons from all panels; 
	}

	private void GradualAddResources()
	{
		if (resourcesAdded) //verify if needed
		{			
			if(deltaGoldPlus>10)
			{
				int substract = deltaGoldPlus/10;
	

				deltaGoldPlus-= substract;
				gold += substract;
			}
			else if(deltaGoldPlus>0)
			{
				deltaGoldPlus--;
				gold ++;
			}

			if(deltaManaPlus>10)
			{
				int substract = deltaManaPlus/10;

				deltaManaPlus-= substract;
				mana += substract;
			}
			else if(deltaManaPlus>0)
			{
				deltaManaPlus--;
				mana ++;
			}

			if(deltaCrystalsPlus>10)
			{
				int substract = deltaCrystalsPlus/10;
				deltaCrystalsPlus -= substract;
				crystals += substract;
			}
			else if(deltaCrystalsPlus>0)
			{
				deltaCrystalsPlus--;
				crystals ++;
			}

			ApplyMaxCaps();
			UpdateUI();

			if (deltaGoldPlus == 0 && deltaManaPlus == 0 && deltaCrystalsPlus == 0) 
			{						
				CancelInvoke ("GradualAddResources");
				resourcesAdded = false;
			}
		}
	}

	public void SubstractResources(int dGold, int dMana, int dCrystals)
	{		
		deltaGoldMinus += dGold; deltaManaMinus += dMana; deltaCrystalsMinus += dCrystals;

		if (!resourcesSubstracted) 
		{			
			resourcesSubstracted = true;
			InvokeRepeating ("GradualSubstractResources", 0.1f, 0.1f);
		}

		UpdateCreatorMenus();
		//updates in sequence all buttons from all panels; 
	}


	private void GradualSubstractResources()
	{
		if (resourcesSubstracted) //verify if needed
		{			
			if(deltaGoldMinus>10)
			{
				int substract = deltaGoldMinus/10;

				deltaGoldMinus-= substract;//substract is a negative value here
				gold -= substract;
			}
			else if(deltaGoldMinus>0)
			{
				deltaGoldMinus--;
				gold --;
			}

			if(deltaManaMinus>10)
			{
				int substract = deltaManaMinus/10;

				deltaManaMinus-= substract;
				mana -= substract;
			}
			else if(deltaManaMinus>0)
			{
				deltaManaMinus--;
				mana --;
			}
		
			if(deltaCrystalsMinus>10)
			{
				int substract = deltaCrystalsMinus/10;
				deltaCrystalsMinus -= substract;
				crystals -= substract;
			}
			else if(deltaCrystalsMinus>0)
			{
				deltaCrystalsMinus--;
				crystals --;
			}

			ApplyMaxCaps();
			UpdateUI();

			if (deltaGoldMinus == 0 && deltaManaMinus == 0 && deltaCrystalsMinus==0) 
			{					
				CancelInvoke ("GradualSubstractResources");
				resourcesSubstracted = false;
			}
		}
	}

	private IEnumerator ReturnFromBattle()
	{
		yield return new WaitForSeconds (1.5f);

		if (((TransData)transData).battleOver) 
		{
			if (int.Parse(Helper.chainId) == 4000)
			{
				((TransData)transData).ReturnFromBattle();
            }
            else
            {
				((TransData)transData).ReturnFromBattleServerRpc();
			}
			tutorialCitySeen = true;//since we have already been to battle, no tutorial 
		}	
	}

	private IEnumerator LaunchTutorial()
	{
		yield return new WaitForSeconds (10.0f);
		if (!tutorialCitySeen)
			GhostHelper.SetActive (true);//since this is a Delayed function, we will activate the first time tutorial here 
	}

	public void ApplyMaxCaps()//cannot exceed storage+bought capacity
	{
		if (gold > maxGold) { gold = maxGold; }
		if (mana > maxMana) { mana = maxMana; }
		//if (experience > maxExperience) { experience = maxExperience; }
	}

	public void VerifyMaxReached()
	{
		if (gold == maxGold) 
		{ 
			MessageController.Instance.DisplayMessage("Increase Gold storage capacity.");	
		}
		if (mana == maxMana) 
		{ 
			MessageController.Instance.DisplayMessage("Increase Mana storage capacity.");
		}
	}
	public void UpdateCreatorMenus()
	{
		for (int i = 0; i < creators.Length ; i++) {
			creators [i].UpdateButtons ();
		}
	}

	public void UpdateUI()//updates numbers and progress bars
	{

		PlayerData data = Player.Instance.GetPlayer();
		data.PlayerName = "Player Name";
		data.LevelData.Level = level;
		data.ExperienceData.CurrentExp = experience;

		data.PlayerResources.Dobbit.Type = GameResourceType.Dobbit;
		data.PlayerResources.Dobbit.CurrentValue = occupiedDobbits;
		data.PlayerResources.Dobbit.MaxValue = dobbits;
		
		data.PlayerResources.Housing.Type = GameResourceType.Housing;
		data.PlayerResources.Housing.CurrentValue = occupiedHousing;
		data.PlayerResources.Housing.MaxValue = maxHousing;
		
		data.PlayerResources.Gold.Type = GameResourceType.Gold;
		data.PlayerResources.Gold.CurrentValue = gold;
		data.PlayerResources.Gold.MaxValue = maxGold;
		
		data.PlayerResources.Mana.Type = GameResourceType.Mana;
		data.PlayerResources.Mana.CurrentValue = mana;
		data.PlayerResources.Mana.MaxValue = maxMana;
		
		data.PlayerResources.Crystal.Type = GameResourceType.Crystal;
		data.PlayerResources.Crystal.CurrentValue = crystals;
		data.PlayerResources.Crystal.MaxValue = maxCrystals;

		data.CloakData.RemainingCloakTime = remainingCloakTime;
		data.CloakData.PurchasedCloakTime = purchasedCloakTime;
		
		
		Player.Instance.PlayerEvt.Invoke(data);
	}

	public void UpdateUnitsNo()
	{	
		int allUnits = 0;
		for (int i = 0; i <  ExistingUnits.Count; i++) 
		{
			allUnits += ExistingUnits[i].count;
		}

		Player.Instance.GetPlayer().AllUnits = allUnits;
		Player.Instance.PlayerEvt.Invoke(Player.Instance.GetPlayer());
	}

	public bool IsEnoughCurrency(int requiredAmount, CurrencyType currencyType)
	{
		switch (currencyType)
		{
			case CurrencyType.Crystal:
				return requiredAmount <= crystals;
//			case CurrencyType.Dollars: //I'm not sure what do I need to use here...
//				return requiredAmount <= 0;
			case CurrencyType.Gold:
				return requiredAmount <= gold;
			case CurrencyType.Mana:
				return requiredAmount <= mana;
		}
		return false;
	}

	public Sprite GetCurrencyIcon(CurrencyType currencyType)
	{
		switch (currencyType)
		{
			case CurrencyType.Crystal:
				return ImageControler.GetImage("crystal");
//			case CurrencyType.Dollars: //I'm not sure what do I need to use here...
//				return requiredAmount <= 0;
			case CurrencyType.Gold:
				return ImageControler.GetImage("gold");
			case CurrencyType.Mana:
				return ImageControler.GetImage("mana");
		}
		return null;
	}
}