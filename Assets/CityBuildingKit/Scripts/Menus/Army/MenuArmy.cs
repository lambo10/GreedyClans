using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Menus;
using UIControllersAndData.Units;
using Unity.Netcode;
using System.Linq;

public class MenuArmy : MonoBehaviour {//the panel with all the units, also used for selecting the army before attack

	public static MenuArmy Instance;
	private const int unitsNo = 10, buildingsNo = 7;  //correlate with MenuUnitBase.cs

	private Component removableCreator, transData, saveLoadMap, soundFX;

	public StructureCreator buildingCreator;

	private List<ExistedUnit> _existingBattleUnits;

	public List<ExistedUnit> ExistingBattleUnits
	{
		get => _existingBattleUnits;
		set => _existingBattleUnits = value;
	}

	private void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start () {

		removableCreator = GameObject.Find ("RemovableCreator").GetComponent<RemovableCreator> ();

		transData = GameObject.Find("TransData").GetComponent<TransData>();
		saveLoadMap = GameObject.Find("SaveLoadMap").GetComponent<SaveLoadMap>();
		soundFX = GameObject.Find("SoundFX").GetComponent<SoundFX>();
	
	}
	
	public async void LoadCampaign(int campaignLevel)
	{
		//string battleMapData = PlayerPrefs.GetString(Helper.battleMapKey);
		//while (string.IsNullOrEmpty(battleMapData))
		//{
  //          new WaitForSeconds(1f);
		//}
		try
        {
			if (int.Parse(Helper.chainId) == 4000)
			{
				LoadCampaignOffchain(campaignLevel);
			}
			else
			{
				LoadCampaignServerRpc(campaignLevel);
			}

		}
        catch (Exception e)
        {
			Debug.Log(e);
        }
	}


	[ServerRpc]
	public int[] StringToIntArrayServerRpc(string str)
	{
		// Remove the brackets and split the string by commas
		string[] stringArray = str.TrimStart('[').TrimEnd(']').Split(',');

		// Convert the string array to an int array
		return stringArray.Select(int.Parse).ToArray();
	}

	public int[] StringToIntArray(string str)
	{
		// Remove the brackets and split the string by commas
		string[] stringArray = str.TrimStart('[').TrimEnd(']').Split(',');

		// Convert the string array to an int array
		return stringArray.Select(int.Parse).ToArray();
	}



	[ServerRpc]
	public int GetIdFromNftIdServerRpc(int nftID)
	{
		// Create a dictionary to map the nftID to the id
		Dictionary<int, int> nftIdToId = new Dictionary<int, int>()
	{
		{ 25, 0 },
		{ 14, 1 },
		{ 22, 2 },
		{ 18, 3 },
		{ 24, 4 },
		{ 27, 5 },
		{ 17, 6 },
		{ 20, 7 },
		{ 12, 8 },
		{ 21, 9 },
		{ 13, 10 },
		{ 16, 11 },
		{ 19, 12 },
		{ 15, 13 },
		{ 26, 14 },
		{ 23, 15 },
	};

		// Look up the id based on the nftID
		int id = nftIdToId[nftID];

		return id;
	}

	public int GetIdFromNftId(int nftID)
	{
		// Create a dictionary to map the nftID to the id
		Dictionary<int, int> nftIdToId = new Dictionary<int, int>()
	{
		{ 25, 0 },
		{ 14, 1 },
		{ 22, 2 },
		{ 18, 3 },
		{ 24, 4 },
		{ 27, 5 },
		{ 17, 6 },
		{ 20, 7 },
		{ 12, 8 },
		{ 21, 9 },
		{ 13, 10 },
		{ 16, 11 },
		{ 19, 12 },
		{ 15, 13 },
		{ 26, 14 },
		{ 23, 15 },
	};

		// Look up the id based on the nftID
		int id = nftIdToId[nftID];

		return id;
	}




	[ServerRpc]
	public (int value, int count)[] CountValuesServerRpc(int[] array)
	{
		// Group the array by value and count the number of elements in each group
		var valueCounts =
			from value in array
			group value by value into g
			select new { value = g.Key, count = g.Count() };

		// Convert the result to an array of tuples
		return valueCounts.Select(vc => (vc.value, vc.count)).ToArray();
	}

	public (int value, int count)[] CountValues(int[] array)
	{
		// Group the array by value and count the number of elements in each group
		var valueCounts =
			from value in array
			group value by value into g
			select new { value = g.Key, count = g.Count() };

		// Convert the result to an array of tuples
		return valueCounts.Select(vc => (vc.value, vc.count)).ToArray();
	}




	[ServerRpc]
	private void lb_LoadBattlleUnitsServerRpc(string battleUnitsString)
    {
		int [] battleUnitsArray = StringToIntArrayServerRpc(battleUnitsString);

		Debug.Log("Here ---------------- -3-3-3");

		(int value, int count)[] unitCoun = CountValuesServerRpc(battleUnitsArray);

		for (int i = 0; i < unitCoun.Length; i++)
		{
			Stats.Instance.ExistingUnits.Add(new ExistedUnit
			{
				id = GetIdFromNftIdServerRpc(unitCoun[i].value),
				count = unitCoun[i].count,
				level = 1
			});
		}
		Debug.Log("Here ---------------- -2-2-2");
	}

	private void lb_LoadBattlleUnits(string battleUnitsString)
	{

		int[] battleUnitsArray = StringToIntArray(battleUnitsString);

		Debug.Log("Here ---------------- -3-3-3");

		(int value, int count)[] unitCoun = CountValues(battleUnitsArray);

		for (int i = 0; i < unitCoun.Length; i++)
		{
			Stats.Instance.ExistingUnits.Add(new ExistedUnit
			{
				id = GetIdFromNftId(unitCoun[i].value),
				count = unitCoun[i].count,
				level = 1
			});
		}
		Debug.Log("Here ---------------- -2-2-2");
	}






	[ServerRpc]
	public void LoadCampaignServerRpc(int campaignLevel)
	{
		
		string battleMapData = PlayerPrefs.GetString(Helper.battleMapKey);
		Debug.Log("Here ---------------- Getting to this point"+battleMapData);
		if (!string.IsNullOrEmpty(battleMapData))
		{
			//LoadCampaignServerRpc(campaignLevel);
		
		((TransData)transData).campaignLevel = campaignLevel;

			TypicalJsonResponse battleMapDataJson_raw = JsonUtility.FromJson<TypicalJsonResponse>(battleMapData);

			battleMapJsonHandler battleMapDataJson = JsonUtility.FromJson<battleMapJsonHandler>(battleMapDataJson_raw.msg);

			lb_LoadBattlleUnitsServerRpc(battleMapDataJson.playerArmy);

			Debug.Log("Here ---------------- -1-1-1");

			LoadMultiplayer0ServerRpc();
		}
	}
	
	public void LoadCampaignOffchain(int campaignLevel)
	{

		string battleMapData = PlayerPrefs.GetString(Helper.battleMapKey);
		Debug.Log("Here ---------------- Getting to this point ------" + battleMapData);
		if (!string.IsNullOrEmpty(battleMapData))
		{
			//LoadCampaignOffchain(campaignLevel);

			

			((TransData)transData).campaignLevel = campaignLevel;

			Debug.Log(battleMapData);

			TypicalJsonResponse battleMapDataJson_raw = JsonUtility.FromJson<TypicalJsonResponse>(battleMapData);

			battleMapJsonHandler battleMapDataJson = JsonUtility.FromJson<battleMapJsonHandler>(battleMapDataJson_raw.msg);

			lb_LoadBattlleUnits(battleMapDataJson.playerArmy);

			Debug.Log("Here ---------------- -1-1-1");

			LoadMultiplayer0();
		}
	}


	[ServerRpc]
	public void LoadMultiplayer0ServerRpc()
	{	
		bool unitsExist = false;

		for (int i = 0; i < Stats.Instance.ExistingUnits.Count; i++) 
		{
			if(Stats.Instance.ExistingUnits[i].count > 0)
			{
				unitsExist = true;
				break;
			}
		}

		if(Application.internetReachability == NetworkReachability.NotReachable)
		{
			MessageController.Instance.DisplayMessage("Can't download map.\nNo internet connection.");
        }
        else
        {
			Debug.Log("Here ---------------- 000");
			StartCoroutine(LoadMultiplayerMapServerRpc(0));
		}
		//else if (unitsExist && ((TransData)transData).campaignLevel != -1) {
		//	StartCoroutine (LoadMultiplayerMapServerRpc (0)); 		
		//}
		//else if(!unitsExist)
		//{
		//	MessageController.Instance.DisplayMessage("Train units for battle.");
		//}
		//else if(Stats.Instance.gold >= 250 && ((TransData)transData).campaignLevel==-1)		
		//{
		//	StartCoroutine(LoadMultiplayerMapServerRpc(0)); 
		//}
		//else
		//{
		//	MessageController.Instance.DisplayMessage("You need more gold.");
		//}
	}

	public void LoadMultiplayer0()
	{
		bool unitsExist = false;

		for (int i = 0; i < Stats.Instance.ExistingUnits.Count; i++)
		{
			if (Stats.Instance.ExistingUnits[i].count > 0)
			{
				unitsExist = true;
				break;
			}
		}

		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			MessageController.Instance.DisplayMessage("Can't download map.\nNo internet connection.");
		}
		else
		{
			Debug.Log("Here ---------------- 000");
			StartCoroutine(LoadMultiplayerMap(0));
		}
		//else if (unitsExist && ((TransData)transData).campaignLevel != -1) {
		//	StartCoroutine (LoadMultiplayerMap (0)); 		
		//}
		//else if(!unitsExist)
		//{
		//	MessageController.Instance.DisplayMessage("Train units for battle.");
		//}
		//else if(Stats.Instance.gold >= 250 && ((TransData)transData).campaignLevel==-1)		
		//{
		//	StartCoroutine(LoadMultiplayerMap(0)); 
		//}
		//else
		//{
		//	MessageController.Instance.DisplayMessage("You need more gold.");
		//}
	}


	[ServerRpc]
	private IEnumerator LoadMultiplayerMapServerRpc(int levelToLoad)				//building loot values = half the price
	{
		//if(((TransData)transData).campaignLevel==-1)
		//Stats.Instance.gold -= 250;										//this is where the price for the battle is payed, before saving the game

		

		ExistingBattleUnits = Enumerable.Range(1, 16).Select(i => new ExistedUnit()).ToList();

        foreach (ExistedUnit existedUnit in Stats.Instance.ExistingUnits)
        {
            ExistingBattleUnits[existedUnit.id].id = existedUnit.id;
            ExistingBattleUnits[existedUnit.id].count = existedUnit.count;
            ExistingBattleUnits[existedUnit.id].level = existedUnit.level;
        }

		Debug.Log("Here ---------------- 222");

		//for (int i = 0; i < ExistingBattleUnits.Count; i++) 
		//{

		//	Stats.Instance.occupiedHousing -= Stats.Instance.sizePerUnit[i] * ExistingBattleUnits[i].count;
		//	Stats.Instance.ExistingUnits.RemoveAll(unit => unit.id == ExistingBattleUnits[i].id);
		//}

		//Stats.Instance.UpdateUI ();// - optional- no element of the UI is visible at this time
		//Stats.Instance.UpdateUnitsNo();

		//#if !UNITY_WEBPLAYER
		//((SaveLoadMap)saveLoadMap).SaveGameLocalFile ();							//local autosave at battle load
		//#endif

		//#if UNITY_WEBPLAYER
		//((SaveLoadMap)saveLoadMap).SaveGamePlayerPrefs ();
		//#endif

		((TransData)transData).removeTimes = ((RemovableCreator)removableCreator).removeTimes;
		((TransData)transData).housingPerUnit = Stats.Instance.sizePerUnit;

		((TransData) transData).GoingToBattleUnits = ExistingBattleUnits;

		((TransData)transData).tutorialBattleSeen = Stats.Instance.tutorialBattleSeen;

		((TransData)transData).soundOn = ((SoundFX)soundFX).soundOn;
		((TransData)transData).ambientOn = ((SoundFX)soundFX).ambientOn;
		((TransData)transData).musicOn = ((SoundFX)soundFX).musicOn;

		for (int i = 0; i < buildingCreator.structures.ToArray().Length; i++) 
		{
			

			((TransData)transData).buildingValues [i] = int.Parse(buildingCreator.structures [i] ["Price"]);
			((TransData)transData).buildingCurrency [i] = buildingCreator.structures [i] ["Currency"];
		}

		Debug.Log("Here ---------------- 111");

		yield return new WaitForSeconds (0.2f);
		switch (levelToLoad) 
		{
		case 0:	 
			Application.LoadLevel("Map01");
			break;		
		}
	}

	private IEnumerator LoadMultiplayerMap(int levelToLoad)                //building loot values = half the price
	{
		//if(((TransData)transData).campaignLevel==-1)
		//Stats.Instance.gold -= 250;										//this is where the price for the battle is payed, before saving the game



		ExistingBattleUnits = Enumerable.Range(1, 16).Select(i => new ExistedUnit()).ToList();

		foreach (ExistedUnit existedUnit in Stats.Instance.ExistingUnits)
		{
			ExistingBattleUnits[existedUnit.id].id = existedUnit.id;
			ExistingBattleUnits[existedUnit.id].count = existedUnit.count;
			ExistingBattleUnits[existedUnit.id].level = existedUnit.level;
		}

		Debug.Log("Here ---------------- 222");

		//for (int i = 0; i < ExistingBattleUnits.Count; i++) 
		//{

		//	Stats.Instance.occupiedHousing -= Stats.Instance.sizePerUnit[i] * ExistingBattleUnits[i].count;
		//	Stats.Instance.ExistingUnits.RemoveAll(unit => unit.id == ExistingBattleUnits[i].id);
		//}

		//Stats.Instance.UpdateUI ();// - optional- no element of the UI is visible at this time
		//Stats.Instance.UpdateUnitsNo();

		//#if !UNITY_WEBPLAYER
		//((SaveLoadMap)saveLoadMap).SaveGameLocalFile ();							//local autosave at battle load
		//#endif

		//#if UNITY_WEBPLAYER
		//((SaveLoadMap)saveLoadMap).SaveGamePlayerPrefs ();
		//#endif

		((TransData)transData).removeTimes = ((RemovableCreator)removableCreator).removeTimes;
		((TransData)transData).housingPerUnit = Stats.Instance.sizePerUnit;

		((TransData)transData).GoingToBattleUnits = ExistingBattleUnits;

		((TransData)transData).tutorialBattleSeen = Stats.Instance.tutorialBattleSeen;

		((TransData)transData).soundOn = ((SoundFX)soundFX).soundOn;
		((TransData)transData).ambientOn = ((SoundFX)soundFX).ambientOn;
		((TransData)transData).musicOn = ((SoundFX)soundFX).musicOn;

		for (int i = 0; i < buildingCreator.structures.ToArray().Length; i++)
		{


			((TransData)transData).buildingValues[i] = int.Parse(buildingCreator.structures[i]["Price"]);
			((TransData)transData).buildingCurrency[i] = buildingCreator.structures[i]["Currency"];
		}

		Debug.Log("Here ---------------- 111");

		yield return new WaitForSeconds(0.2f);
		switch (levelToLoad)
		{
			case 0:
                Application.LoadLevel("Map01");
                break;
		}
	}

}
