using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Menus;
using Assets.Scripts.UIControllersAndData.Store;
using UIControllersAndData;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UnityEngine.UI;
using Unity.Netcode;

public class Helios : MonoBehaviour {

	private const int unitGroupsNo = 4;//the maximum number of groups
	private float 
		elapsedTime = 0.0f, 
		intervalTime = 1.0f,	// interval between attack updates
		pathUpdateTime = 0.2f,  // due to the wall attack absence, paths are long and take time; normally 0.2f
		updateOTimer,			// used to scatter the updates for units - timers
		updateITimer,
		updateIITimer,
		updateIIITimer; 
		
	//status variables:
	public bool networkLoadReady = false;
	private bool 
		mapIsEmpty = false, 
		battleOver = false,
		allDestroyed = false,
		updatePathMaster,			// used to scatter the updates for units - bools
		updatePathO,
		updatePathI,
		updatePathII,
		updatePathIII;

	//numeric data:
	private int 
		starZ = -6,
		smokeZ = -10,	// z depths
		fireZ = -11,
		zeroZ = 0,
		targetZ = -11,	//to avoid gioded projectiles from changing Z and going under sprites
		allLootGold, 	// total gold/mana on map - passed to StatsBattle to properly dimension the gold/mana loot progress bars.
		allLootMana,
		gain,			// the loot fraction, based on total building value and damage procentage;
		updateOCounter,	// used to scatter the updates for units - cycles through the unit lists
		updateICounter,
		updateIICounter,
		updateIIICounter,
		processCounter;
	
	public int 
		instantiationGroupIndex = -1,	//keeps track of total number of groups- must not exceed 4
		instantiationUnitIndex = -1,	//each unit will have a unique ID- necessary when killed - not necessarily in order
		selectedGroupIndex = 0,			//0 when initialized, reset to -1 afterwards 
		selectedStructureIndex = 0;		//necessary only for indestructible on/off before any units are deployed

	//interface elements:
	public Text goldNoLb, manaNoLb, buildingsDestroyedNoLb, unitsLostNoLb, unitsRecoveredNoLb;//missioncomplete panel labels
	public Text[] unitsNo = new Text[4];

	public GameObject[] 
		UnitGroupBt = new GameObject[4],
		//InterfaceElements = new GameObject[6], 
		//MenuPanels = new GameObject[3],					//UnitsBattlePanel, OptionsPanel, GhostHelper
		//SelectedGroupLb = new GameObject[unitGroupsNo],	//the red/blue labels alternating when manually selecting groups
		//UnselectedGroupLb = new GameObject[unitGroupsNo],
		grassTarget = new GameObject[unitGroupsNo],		//each group heads for its assigned grass target
		unitStar = new GameObject[unitGroupsNo];		//activate when a unit grop is made immortal	

	public GameObject 
		MissionCompleteServerRpcPanel,                          //activated at battle end
		MissionCompletePanel,
		NavigationButtons, 								// moved at the corner of the panel above 
		SmokePf, ExplosionPf, FirePf, Rubble1x1Pf, Rubble2x2Pf, Rubble3x3Pf, GravePf, GainGoldPf, GainManaPf, //prefabs
		EffectsGroup,									//parent instantiated effect prefabs
		HeliosUI,
		buildingStar;

    [SerializeField]
    private AnimatorHelper heliosAnimation;

    public List<int> 
		BuildingValues = new List<int>(),	//holds loot values for each building; 0 when completely destroyed
		BuildingsHealth = new List<int>();	//100, 100, 100 - you can make this private - useful to increase health to study unit behavior 

	//lists and arrays for buildings and units:
	public List<bool> 
		//BuildingGoldBased = new List<bool>(),
		DamageEffectsRunning = new List<bool> ();	//0,0,0... initially then 1,1,1...

	public List<string> BuildingCurrency = new List<string>();

	public int[] 
		targetStructureIndex = new int[unitGroupsNo],
		currentDamage = new int[unitGroupsNo],		//if 5 units attack a building, the building health (100) is decreased by 5 each second
		surroundIndex = new int[unitGroupsNo];		//for each separate group, increments an index to "surround" the buildings - occupy the aiTargets in sequence - 0,1,2, etc 

	private GameObject[] grassPatches;				//all grass patches on the map - found by label
	public Vector3[] targetCenter = new Vector3[unitGroupsNo];
	public Vector2[] unitPosition = new Vector2[unitGroupsNo];	// the position of the group is set by the position of element 0

	public bool[] 
		pauseAttack = new bool[unitGroupsNo],		//necessary to avoid errors untill the units state has changed
		updateTarget = new bool[unitGroupsNo],
		userSelect = new bool[unitGroupsNo];

	public List<GameObject> 
		Buildings = new List<GameObject>(),			//buildings and cannons, instantiated and passed at LoadMap
		//Cannons = new List<GameObject>(),
		DamageBars = new List<GameObject>(),

		DeployedUnits = new List<GameObject>(),		//all units deployed
		selectedList =  new List<GameObject>(),		//currently selected from the groups below 
		GroupO = new List<GameObject>(), 
		GroupI = new List<GameObject>(), 
		GroupII = new List<GameObject>(), 
		GroupIII = new List<GameObject>(); 

	public List<Vector2> 
		grassTargets = new List<Vector2>(),				// distance, grassIndex
		aiTargetsFree = new List<Vector2>(),			//##make private - to see if the list is sorted properly
	 	aiTargetVectorsCurrent = new List<Vector2>(),	//List of current target vectors - will be passed to each group; aiTargets are the green squares around the building - units go there to attack
		aiTargetVectorsO = new List<Vector2>(),
		aiTargetVectorsI = new List<Vector2>(),
		aiTargetVectorsII = new List<Vector2>(),
		aiTargetVectorsIII = new List<Vector2>();		//O I II III IV V VI VII VIII IX X XI XII XIII XIV XV XVI XVII XVIII 0-18 Roman numerals + O for nulla, in case anyone is wondering

	private Component statsBattle, transData, saveLoadBattle, soundFx, relay, heliosMsg;//components needed to pass info

	// Use this for initialization
	void Start () {

		statsBattle = GameObject.Find ("StatsBattle").GetComponent<StatsBattle> ();
		transData = GameObject.Find ("TransData").GetComponent<TransData> ();
		saveLoadBattle = GameObject.Find ("SaveLoadBattle").GetComponent<SaveLoadBattle> ();
		soundFx = GameObject.Find ("SoundFX").GetComponent<SoundFX> ();
		relay = GameObject.Find ("Relay").GetComponent<Relay> ();
		heliosMsg = GameObject.Find ("HeliosMsg").GetComponent<MessageController> ();

        //no longer needed
        //makes sure the players are all facing the correct direction while walking (centralizes this function)
        //InvokeRepeating ("SetDirection", 0.5f, 0.5f);
    }

	private void Talk(string text)
	{
		((MessageController)heliosMsg).DisplayMessage (text);
	}

	private IEnumerator GarbleMessage()	//EMP in progress
	{
		yield return new WaitForSeconds (2.5f);
		((MessageController)heliosMsg).GarbleMessage ();		
	}

	[ServerRpc]
	public void Select0ServerRpc()	
	{
		selectedList = GroupO; selectedGroupIndex = 0; ResetGroupLabelsServerRpc (0); UpdateSelectStarsServerRpc (); 

		if (((Relay)relay).deploying)
		{
			Talk ("Deploying Squad 1.");
		}
		else
		Talk ("Chimera to Squad 1.");

		if(battleOver)					
			Talk ("Return to base.");		

	}
	public void Select0()	
	{
		selectedList = GroupO; selectedGroupIndex = 0; ResetGroupLabels (0); UpdateSelectStars (); 

		if (((Relay)relay).deploying)
		{
			Talk ("Deploying Squad 1.");
		}
		else
		Talk ("Chimera to Squad 1.");

		if(battleOver)					
			Talk ("Return to base.");		

	}



	[ServerRpc]
	public void UserSelect0ServerRpc()
	{	
		if (((Relay)relay).deploying)
		{
			Talk ("Deployment in progress. Please wait.");
			return;
		}
		userSelect [0] = true;	Delay ();
		if(GroupO.Count>0)
		{
			Talk ("The Emperor is sending new orders.");
		}
		else
		{
			Talk ("Sorry, sir. Squad 1 has been lost. Disabling controls.");
			StartCoroutine(DisableGroupButtonServerRpc(0));
		}
	}
	public void UserSelect0()
	{	if (int.Parse(Helper.chainId) == 4000)
		{
		if (((Relay)relay).deploying)
		{
			Talk ("Deployment in progress. Please wait.");
			return;
		}
		userSelect [0] = true;	Delay ();
		if(GroupO.Count>0)
		{
			Talk ("The Emperor is sending new orders.");
		}
		else
		{
			Talk ("Sorry, sir. Squad 1 has been lost. Disabling controls.");
			StartCoroutine(DisableGroupButton(0));
		}
		}else{
			UserSelect0ServerRpc();
		}
	}

	[ServerRpc]
	public void Select1ServerRpc() 
	{
		selectedList = GroupI; selectedGroupIndex = 1; ResetGroupLabelsServerRpc (1); UpdateSelectStarsServerRpc ();

		if (((Relay)relay).deploying)
		{
			Talk ("Deploying Squad 2.");
		}
		else
		Talk ("Chimera to Squad 2.");

		if(battleOver)			
			Talk ("Return to base.");
	}
	public void Select1() 
	{
		selectedList = GroupI; selectedGroupIndex = 1; ResetGroupLabels (1); UpdateSelectStars ();

		if (((Relay)relay).deploying)
		{
			Talk ("Deploying Squad 2.");
		}
		else
		Talk ("Chimera to Squad 2.");

		if(battleOver)			
			Talk ("Return to base.");
	}


	[ServerRpc]
	public void UserSelect1ServerRpc()
	{ 		
		if (((Relay)relay).deploying)
		{
			Talk ("Deployment in progress. Please wait.");
			return;
		}
		userSelect [1] = true;	Delay ();

		if(GroupI.Count>0)
		{
			Talk ("The Emperor is sending new orders.");
		}
		else
		{
			Talk ("Sorry, sir. Squad 2 has been lost. Disabling controls.");
			StartCoroutine(DisableGroupButton(1));
		}
	}

		public void UserSelect1()
	{
		// check if offchain false
		// UserSelect1ServerRpc()
		if (int.Parse(Helper.chainId) == 4000)
		{

			if (((Relay)relay).deploying)
			{
				Talk("Deployment in progress. Please wait.");
				return;
			}
			userSelect[1] = true; Delay();

			if (GroupI.Count > 0)
			{
				Talk("The Emperor is sending new orders.");
			}
			else
			{
				Talk("Sorry, sir. Squad 2 has been lost. Disabling controls.");
				StartCoroutine(DisableGroupButton(1));
			}
		}
        else {

			UserSelect1ServerRpc();

        }
	}




	[ServerRpc]
	public void Select2ServerRpc()	
	{
		selectedList = GroupII; selectedGroupIndex = 2; ResetGroupLabelsServerRpc (2); UpdateSelectStarsServerRpc ();
		if (((Relay)relay).deploying)
		{
			Talk ("Deploying Squad 3.");
		}
		else
		Talk ("Chimera to Squad 3.");

		if(battleOver)		
			Talk ("Return to base.");

	}
	public void Select2()	
	{
		selectedList = GroupII; selectedGroupIndex = 2; ResetGroupLabels (2); UpdateSelectStars ();
		if (((Relay)relay).deploying)
		{
			Talk ("Deploying Squad 3.");
		}
		else
		Talk ("Chimera to Squad 3.");

		if(battleOver)		
			Talk ("Return to base.");

	}



	[ServerRpc]
	public void UserSelect2ServerRpc()
	{			
		if (((Relay)relay).deploying)
		{
			Talk ("Deployment in progress. Please wait.");
			return;
		}

		userSelect [2] = true;	Delay ();
		if(GroupII.Count>0)
		{
			Talk ("The Emperor is sending new orders.");
		}
		else
		{
			Talk ("Sorry, sir. Squad 3 has been lost. Disabling controls.");
			StartCoroutine(DisableGroupButtonServerRpc(2));
		}
	}
	public void UserSelect2()
	{
		if (int.Parse(Helper.chainId) == 4000)
		{
			if (((Relay)relay).deploying)
			{
				Talk("Deployment in progress. Please wait.");
				return;
			}

			userSelect[2] = true; Delay();
			if (GroupII.Count > 0)
			{
				Talk("The Emperor is sending new orders.");
			}
			else
			{
				Talk("Sorry, sir. Squad 3 has been lost. Disabling controls.");
				StartCoroutine(DisableGroupButton(2));
			}
        }
        else
        {
			UserSelect2ServerRpc();

		}
	}



	[ServerRpc]
	public void Select3ServerRpc()	
	{
		selectedList = GroupIII; selectedGroupIndex = 3; ResetGroupLabelsServerRpc (3); UpdateSelectStarsServerRpc ();
		if (((Relay)relay).deploying)
		{
			Talk ("Deploying Squad 4.");
		}
		else
		Talk ("Chimera to Squad 4.");

		if(battleOver)			
			Talk ("Return to base.");		
	}
	public void Select3()	
	{
		if (int.Parse(Helper.chainId) == 4000)
		{
			selectedList = GroupIII; selectedGroupIndex = 3; ResetGroupLabels(3); UpdateSelectStars();
			if (((Relay)relay).deploying)
			{
				Talk("Deploying Squad 4.");
			}
			else
				Talk("Chimera to Squad 4.");

			if (battleOver)
				Talk("Return to base.");
        }
        else
        {
			Select3ServerRpc();
        }
	}




	[ServerRpc]
	public void UserSelect3ServerRpc()
	{			
		if (((Relay)relay).deploying)
		{
			Talk ("Deployment in progress. Please wait.");
			return;
		}

		userSelect [3] = true;	Delay ();
		if(GroupIII.Count>0)
		{
			Talk ("The Emperor is sending new orders.");
		}
		else
		{
			Talk ("Sorry, sir. Squad 4 has been lost. Disabling controls.");
			StartCoroutine(DisableGroupButtonServerRpc(3));
		}
	}
	public void UserSelect3()
	{
		if (int.Parse(Helper.chainId) == 4000)
		{

			if (((Relay)relay).deploying)
			{
				Talk("Deployment in progress. Please wait.");
				return;
			}

			userSelect[3] = true; Delay();
			if (GroupIII.Count > 0)
			{
				Talk("The Emperor is sending new orders.");
			}
			else
			{
				Talk("Sorry, sir. Squad 4 has been lost. Disabling controls.");
				StartCoroutine(DisableGroupButton(3));
			}
        }
        else
        {
			UserSelect3ServerRpc();
        }
	}





	[ServerRpc]
	private IEnumerator DisableGroupButtonServerRpc(int i)
	{
		yield return new WaitForSeconds (1.0f);
		UnitGroupBt [i].SetActive (false);
	}
	private IEnumerator DisableGroupButton(int i)
	{
		yield return new WaitForSeconds (1.0f);
		UnitGroupBt [i].SetActive (false);
	}




	private void Delay()
	{
		((Relay)relay).DelayInput ();
	}
	[ServerRpc]
	private void ResetGroupLabelsServerRpc(int index)
	{
		for (int i = 0; i < unitGroupsNo; i++) 
		{
			//SelectedGroupLb[i].SetActive(false);
			//UnselectedGroupLb[i].SetActive(true);
			userSelect[i] = false;
		}

		//UnselectedGroupLb[index].SetActive(false);
		//SelectedGroupLb[index].SetActive(true);
	}
	private void ResetGroupLabels(int index)
	{
		for (int i = 0; i < unitGroupsNo; i++) 
		{
			//SelectedGroupLb[i].SetActive(false);
			//UnselectedGroupLb[i].SetActive(true);
			userSelect[i] = false;
		}

		//UnselectedGroupLb[index].SetActive(false);
		//SelectedGroupLb[index].SetActive(true);
	}




	[ServerRpc]
	private IEnumerator GetFirstBuildingServerRpc()	
	{
		yield return new WaitForSeconds(3.0f);
		Talk ("Tactical situation assessment:");

		FindNearestBuildingServerRpc();
		/*
		for (int i = 0; i < updateTarget.Length; i++) 
		{
			updateTarget[i] = true;
		}
		*/
	}
	private IEnumerator GetFirstBuilding()	
	{
		yield return new WaitForSeconds(3.0f);
		Talk ("Tactical situation assessment:");

		FindNearestBuilding();
		/*
		for (int i = 0; i < updateTarget.Length; i++) 
		{
			updateTarget[i] = true;
		}
		*/
	}



	
	public void NetworkLoadReady()
	{	
		//HeliosUI.SetActive (true);
		Talk ("Chimera online, ready to receive.");
		//HeliosTween.Instance.PlayFadeOut();
		if (int.Parse(Helper.chainId) == 4000)
		{
			PrepareBuildings();
			StartCoroutine("GetFirstBuilding");
		}
        else { 
			PrepareBuildingsServerRpc();
			StartCoroutine("GetFirstBuildingServerRpc");
		}
		networkLoadReady = true;
	}
	[ServerRpc]
	private void PrepareBuildingsServerRpc()
	{
		BuildingsHealth.Clear ();

		for (int i = 0; i < Buildings.Count; i++) 
		{
			var structureSelector = Buildings[i].GetComponent<StructureSelector>();
			PrepareLootServerRpc(structureSelector.structureType);		//adds the buildings half-value from TransData 
			
			int id = structureSelector.Id;
			ShopCategoryType categoryType = structureSelector.CategoryType;
				
			var levels = ShopData.Instance.GetLevels(id, categoryType);

			if (levels != null)
			{
				var building = levels?.FirstOrDefault(x => ((ILevel) x).GetLevel() == structureSelector.Level);
				if (building != null)
				{
					BuildingsHealth.Add(building.HP);
				}
			}
			DamageEffectsRunning.Add(false);
		}

		((StatsBattle)statsBattle).maxStorageGold = allLootGold;
		((StatsBattle)statsBattle).maxStorageMana = allLootMana;
		((StatsBattle)statsBattle).UpdateUnitsNoServerRpc ();
	}
	private void PrepareBuildings()
	{
		BuildingsHealth.Clear ();

		for (int i = 0; i < Buildings.Count; i++) 
		{
			var structureSelector = Buildings[i].GetComponent<StructureSelector>();
			PrepareLoot(structureSelector.structureType);		//adds the buildings half-value from TransData 
			
			int id = structureSelector.Id;
			ShopCategoryType categoryType = structureSelector.CategoryType;
				
			var levels = ShopData.Instance.GetLevels(id, categoryType);

			if (levels != null)
			{
				var building = levels?.FirstOrDefault(x => ((ILevel) x).GetLevel() == structureSelector.Level);
				if (building != null)
				{
					BuildingsHealth.Add(building.HP);
				}
			}
			DamageEffectsRunning.Add(false);
		}

		((StatsBattle)statsBattle).maxStorageGold = allLootGold;
		((StatsBattle)statsBattle).maxStorageMana = allLootMana;
		((StatsBattle)statsBattle).UpdateUnitsNo ();
	}






	[ServerRpc]
	private void PrepareLootServerRpc(string buidingType)
	{
		int i = 0;
		switch (buidingType) 
		{									//0 Gold 1 Mana 2 Crystals
		case "Toolhouse": i=0; break;		//2	
		case "Forge": i=1; break;			//1	
		case "Generator": i=2; break;		//0	
		case "Vault": i=3; break;			//1
		case "Barrel": i=4;	break;			//0
		case "Summon": i=5;	break;			//1	
		case "Academy": i=6; break;			//0
		case "Classroom": i=7; break;		//0	
		case "Chessboard": i=8;	break;		//1
		case "Globe": i=9; break;			//1
		case "Workshop": i=10; break;		//0
		case "Tatami": i=11; break;			//1
		} 

		string currency = ((TransData)transData).buildingCurrency [i];

		int value = ((TransData)transData).buildingValues [i];

		BuildingValues.Add(value);
		BuildingCurrency.Add(currency);

		if(currency=="Gold") 
			allLootGold += value;
		else if(currency=="Crystals") 
			allLootGold += value;
		else 
			allLootMana += value;
	}
	private void PrepareLoot(string buidingType)
	{
		int i = 0;
		switch (buidingType) 
		{									//0 Gold 1 Mana 2 Crystals
		case "Toolhouse": i=0; break;		//2	
		case "Forge": i=1; break;			//1	
		case "Generator": i=2; break;		//0	
		case "Vault": i=3; break;			//1
		case "Barrel": i=4;	break;			//0
		case "Summon": i=5;	break;			//1	
		case "Academy": i=6; break;			//0
		case "Classroom": i=7; break;		//0	
		case "Chessboard": i=8;	break;		//1
		case "Globe": i=9; break;			//1
		case "Workshop": i=10; break;		//0
		case "Tatami": i=11; break;			//1
		} 

		string currency = ((TransData)transData).buildingCurrency [i];

		int value = ((TransData)transData).buildingValues [i];

		BuildingValues.Add(value);
		BuildingCurrency.Add(currency);

		if(currency=="Gold") 
			allLootGold += value;
		else if(currency=="Crystals") 
			allLootGold += value;
		else 
			allLootMana += value;
	}




	[ServerRpc]
	public void FindSpecificBuildingServerRpc() 		//user has tapped on a building
	{
		aiTargetsFree.Clear();
		
		grassPatches = GameObject.FindGameObjectsWithTag("Grass");

		for (int i = 0; i < grassPatches.Length; i++) 
		{
			if(grassPatches[i].GetComponent<GrassSelector>().grassIndex == targetStructureIndex[selectedGroupIndex])
			{
				grassTarget[selectedGroupIndex] = grassPatches[i];
				break;
			}
		}
			
		SelectTargetGridServerRpc();
	}
	public void FindSpecificBuilding() 		//user has tapped on a building
	{
		aiTargetsFree.Clear();
		
		grassPatches = GameObject.FindGameObjectsWithTag("Grass");

		for (int i = 0; i < grassPatches.Length; i++) 
		{
			if(grassPatches[i].GetComponent<GrassSelector>().grassIndex == targetStructureIndex[selectedGroupIndex])
			{
				grassTarget[selectedGroupIndex] = grassPatches[i];
				break;
			}
		}
			
		SelectTargetGrid();
	}




	[ServerRpc]
	public int FindNearestGroupServerRpc(Vector2 newTargetPos)		// finds the closest group to the new target building
	{	
		if (instantiationGroupIndex == -1)					//give up if there are no units deployed
						return -1;

		List<Vector2> closeGroups = new List<Vector2> ();
		closeGroups.Clear ();

		switch (instantiationGroupIndex) 
		{
		case 0:

			if(GroupO.Count>0)
			{
				closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupO[0].transform.position),0));//calculates position based on first unit
			}
			break;
		case 1:
			if(GroupO.Count>0)
			{
				closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupO[0].transform.position),0));
			}

			if(GroupI.Count>0)
			{
				closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupI[0].transform.position),1));
			}
			break;
		case 2:

			if(GroupO.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupO[0].transform.position),0));			
			}

			if(GroupI.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupI[0].transform.position),1));			
			}

			if(GroupII.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupII[0].transform.position),2));			
			}
			break;
		case 3:
				
			if(GroupO.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupO[0].transform.position),0));			
			}

			if(GroupI.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupI[0].transform.position),1));			
			}

			if(GroupII.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupII[0].transform.position),2));			
			}

			if(GroupIII.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupIII[0].transform.position),3));			
			}

			break;
		}

		closeGroups.Sort(delegate (Vector2 d1,Vector2 d2){return d1.x.CompareTo(d2.x);});
		return (int)closeGroups [0].y;//returns the closest group index

	}
	public int FindNearestGroup(Vector2 newTargetPos)		// finds the closest group to the new target building
	{	
		if (instantiationGroupIndex == -1)					//give up if there are no units deployed
						return -1;

		List<Vector2> closeGroups = new List<Vector2> ();
		closeGroups.Clear ();

		switch (instantiationGroupIndex) 
		{
		case 0:

			if(GroupO.Count>0)
			{
				closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupO[0].transform.position),0));//calculates position based on first unit
			}
			break;
		case 1:
			if(GroupO.Count>0)
			{
				closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupO[0].transform.position),0));
			}

			if(GroupI.Count>0)
			{
				closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupI[0].transform.position),1));
			}
			break;
		case 2:

			if(GroupO.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupO[0].transform.position),0));			
			}

			if(GroupI.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupI[0].transform.position),1));			
			}

			if(GroupII.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupII[0].transform.position),2));			
			}
			break;
		case 3:
				
			if(GroupO.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupO[0].transform.position),0));			
			}

			if(GroupI.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupI[0].transform.position),1));			
			}

			if(GroupII.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupII[0].transform.position),2));			
			}

			if(GroupIII.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupIII[0].transform.position),3));			
			}

			break;
		}

		closeGroups.Sort(delegate (Vector2 d1,Vector2 d2){return d1.x.CompareTo(d2.x);});
		return (int)closeGroups [0].y;//returns the closest group index

	}





	[ServerRpc]
	public void FindNearestBuildingServerRpc()		//auto-select next target
	{	
		grassTargets.Clear();
		aiTargetsFree.Clear();	
		

		GameObject[] allGrassPatches = GameObject.FindGameObjectsWithTag ("Grass");
		List<GameObject> selectedGrassPatches = new List<GameObject> ();	

		for (int i = 0; i < allGrassPatches.Length; i++) 
		{ 
			
			if (allGrassPatches [i].GetComponent<GrassSelector> ().structureClass == "Building" ||
				allGrassPatches [i].GetComponent<GrassSelector> ().structureClass == "Weapon") 
			{
				selectedGrassPatches.Add (allGrassPatches [i]);
			}
		}
		grassPatches = selectedGrassPatches.ToArray ();

		if (grassPatches.Length == 0) //nothing was found on the map
		{
			mapIsEmpty = true;
			if(!battleOver)
			StartCoroutine("MissionCompleteServerRpc");
			return;
		}

		SearchAliveBuildingsServerRpc ();

		grassTargets.Sort(delegate (Vector2 d1,Vector2 d2)
		{
			return d1.x.CompareTo(d2.x); 
		});

		grassTarget[selectedGroupIndex] = grassPatches[(int)grassTargets[0].y];
		targetStructureIndex[selectedGroupIndex] = grassPatches[(int)grassTargets[0].y].GetComponent<GrassSelector>().grassIndex;

		targetCenter[selectedGroupIndex] = grassTarget [selectedGroupIndex].transform.position;
		SelectTargetGridServerRpc();
	}
	public void FindNearestBuilding()		//auto-select next target
	{	
		grassTargets.Clear();
		aiTargetsFree.Clear();	
		

		GameObject[] allGrassPatches = GameObject.FindGameObjectsWithTag ("Grass");
		List<GameObject> selectedGrassPatches = new List<GameObject> ();	

		for (int i = 0; i < allGrassPatches.Length; i++) 
		{ 
			
			if (allGrassPatches [i].GetComponent<GrassSelector> ().structureClass == "Building" ||
				allGrassPatches [i].GetComponent<GrassSelector> ().structureClass == "Weapon") 
			{
				selectedGrassPatches.Add (allGrassPatches [i]);
			}
		}
		grassPatches = selectedGrassPatches.ToArray ();

		if (grassPatches.Length == 0) //nothing was found on the map
		{
			mapIsEmpty = true;
			if(!battleOver)
			StartCoroutine("MissionComplete");
			return;
		}

		SearchAliveBuildings ();

		grassTargets.Sort(delegate (Vector2 d1,Vector2 d2)
		{
			return d1.x.CompareTo(d2.x); 
		});

		grassTarget[selectedGroupIndex] = grassPatches[(int)grassTargets[0].y];
		targetStructureIndex[selectedGroupIndex] = grassPatches[(int)grassTargets[0].y].GetComponent<GrassSelector>().grassIndex;

		targetCenter[selectedGroupIndex] = grassTarget [selectedGroupIndex].transform.position;
		SelectTargetGrid();
	}





	[ServerRpc]
	private int GetListIndexServerRpc(int structureIndex)//translates a real structureIndex (ex: 32) into a possibly smaller list index (ex:2)
	{												   //this happends when the user builds and destroys buildings - the structureIndex grows	
		int listIndex = 0;
		for (int i = 0; i < Buildings.Count; i++) 
		{
			if(Buildings[i].GetComponent<StructureSelector>().structureIndex == structureIndex)
			{
				listIndex = i;
				break;
			}
		}
		return listIndex;
	}
	private int GetListIndex(int structureIndex)//translates a real structureIndex (ex: 32) into a possibly smaller list index (ex:2)
	{												   //this happends when the user builds and destroys buildings - the structureIndex grows	
		int listIndex = 0;
		for (int i = 0; i < Buildings.Count; i++) 
		{
			if(Buildings[i].GetComponent<StructureSelector>().structureIndex == structureIndex)
			{
				listIndex = i;
				break;
			}
		}
		return listIndex;
	}




	[ServerRpc]
	public void KillUnitServerRpc(int groupIndex, int unitIndex)
	{	
		switch (groupIndex) 
		{
		case 0:
			for (int i = 0; i < GroupO.Count; i++) 
			{
				if(GroupO[i].GetComponent<Selector>().index==unitIndex)
				{
					((SoundFX)soundFx).SoldierDie();
					GroupO.RemoveAt(i);
					RemoveFromDeployedServerRpc(unitIndex);
					break;
				}
			}		
			break;

		case 1:
			for (int i = 0; i < GroupI.Count; i++) 
			{
				if(GroupI[i].GetComponent<Selector>().index==unitIndex)
				{
					((SoundFX)soundFx).SoldierDie();
					GroupI.RemoveAt(i);
					RemoveFromDeployedServerRpc(unitIndex);
					break;
				}
			}
			break;

		case 2:
			for (int i = 0; i < GroupII.Count; i++) 
			{
				if(GroupII[i].GetComponent<Selector>().index==unitIndex)
				{
					((SoundFX)soundFx).SoldierDie();
					GroupII.RemoveAt(i);
					RemoveFromDeployedServerRpc(unitIndex);
					break;
				}
			}
			break;

		case 3:
			for (int i = 0; i < GroupIII.Count; i++) 
			{
				if(GroupIII[i].GetComponent<Selector>().index==unitIndex)
				{
					((SoundFX)soundFx).SoldierDie();
					GroupIII.RemoveAt(i);
					RemoveFromDeployedServerRpc(unitIndex);
					break;
				}
			}
			break;
		}
	}
	public void KillUnit(int groupIndex, int unitIndex)
	{
		// check if offchain is false
		// KillUnitServerRpc()
		if (int.Parse(Helper.chainId) == 4000)
		{
			switch (groupIndex)
			{
				case 0:
					for (int i = 0; i < GroupO.Count; i++)
					{
						if (GroupO[i].GetComponent<Selector>().index == unitIndex)
						{
							((SoundFX)soundFx).SoldierDie();
							GroupO.RemoveAt(i);
							RemoveFromDeployed(unitIndex);
							break;
						}
					}
					break;

				case 1:
					for (int i = 0; i < GroupI.Count; i++)
					{
						if (GroupI[i].GetComponent<Selector>().index == unitIndex)
						{
							((SoundFX)soundFx).SoldierDie();
							GroupI.RemoveAt(i);
							RemoveFromDeployed(unitIndex);
							break;
						}
					}
					break;

				case 2:
					for (int i = 0; i < GroupII.Count; i++)
					{
						if (GroupII[i].GetComponent<Selector>().index == unitIndex)
						{
							((SoundFX)soundFx).SoldierDie();
							GroupII.RemoveAt(i);
							RemoveFromDeployed(unitIndex);
							break;
						}
					}
					break;

				case 3:
					for (int i = 0; i < GroupIII.Count; i++)
					{
						if (GroupIII[i].GetComponent<Selector>().index == unitIndex)
						{
							((SoundFX)soundFx).SoldierDie();
							GroupIII.RemoveAt(i);
							RemoveFromDeployed(unitIndex);
							break;
						}
					}
					break;
			}

        }
        else
        {
			KillUnitServerRpc(groupIndex, unitIndex);

		}
	}






	[ServerRpc]
	private void RemoveFromDeployedServerRpc(int unitIndex)
	{
		for (int i = 0; i < DeployedUnits.Count; i++) 
		{
			if(DeployedUnits[i].GetComponent<Selector>().index==unitIndex)
			{
				DeployedUnits.RemoveAt(i);
				((StatsBattle)statsBattle).unitsLost++;
				break;
			}
		}
	}
	private void RemoveFromDeployed(int unitIndex)
	{
		for (int i = 0; i < DeployedUnits.Count; i++) 
		{
			if(DeployedUnits[i].GetComponent<Selector>().index==unitIndex)
			{
				DeployedUnits.RemoveAt(i);
				((StatsBattle)statsBattle).unitsLost++;
				break;
			}
		}
	}





	[ServerRpc]
	private void SearchAliveBuildingsServerRpc()
	{
		allDestroyed = true;

		UpdateGroupPositionServerRpc (selectedGroupIndex);

		for (int i = 0; i < grassPatches.Length; i++) 
		{
			if(!grassPatches[i].GetComponent<GrassSelector>().isDestroyed)
			{
				grassTargets.Add(new Vector2( Vector2.Distance(grassPatches[i].transform.position, unitPosition[selectedGroupIndex]), i ));
				allDestroyed = false;
			}
		}

		if(allDestroyed)
		{

			mapIsEmpty = true;
			if(!battleOver)
			StartCoroutine("MissionCompleteServerRpc");
		}
	}
	private void SearchAliveBuildings()
	{
		allDestroyed = true;

		UpdateGroupPosition (selectedGroupIndex);

		for (int i = 0; i < grassPatches.Length; i++) 
		{
			if(!grassPatches[i].GetComponent<GrassSelector>().isDestroyed)
			{
				grassTargets.Add(new Vector2( Vector2.Distance(grassPatches[i].transform.position, unitPosition[selectedGroupIndex]), i ));
				allDestroyed = false;
			}
		}

		if(allDestroyed)
		{

			mapIsEmpty = true;
			if(!battleOver)
			StartCoroutine("MissionComplete");
		}
	}




	[ServerRpc]
	private void SelectTargetGridServerRpc()
	{

		Transform AITargetsParentObj = grassTarget[selectedGroupIndex].transform.Find("AITargets");//AiTargets Parent Object
		
		Transform[] aiTargetsChildren = AITargetsParentObj.GetComponentsInChildren<Transform>();//Transform of all AI green squares sorounding a building, free or not

		//also selects AITargets parent, but it's in the middle
		UpdateGroupPositionServerRpc (selectedGroupIndex);

		int col = 0, row = 0;

		for (int i = 0; i < aiTargetsChildren.Length; i++) // 1 skip parent
		{
			int indexCell = GridManager.instance.GetGridIndex(aiTargetsChildren[i].position);
			col = GridManager.instance.GetColumn(indexCell);
			row = GridManager.instance.GetRow(indexCell);
			if(i==0)
			{
				if(DeployedUnits.Count>0)
				Talk ("New target in grid " + col +"-"+ row + ".\n"+
					                                       "Transmitting coordinates.");
				else
				{
					Talk ("Estimated value " +allLootGold.ToString() +" gold, "+ allLootMana.ToString() + " mana.");
				}
			}
			else if(!GridManager.instance.nodes[row,col].isObstacle)			 
			{
				aiTargetsFree.Add(new Vector2( Vector2.Distance(aiTargetsChildren[i].position, unitPosition[selectedGroupIndex]), i ));
			}
		}

		aiTargetsFree.Sort(delegate (Vector2 d1,Vector2 d2)
		               {
			return d1.x.CompareTo(d2.x); 
		});

		aiTargetVectorsCurrent.Clear ();

		for (int i = 0; i < aiTargetsFree.Count; i++) 
		{
			aiTargetVectorsCurrent.Add(aiTargetsChildren[(int)aiTargetsFree[i].y].position);
		}

		CopyTargetListsServerRpc ();
		pauseAttack[selectedGroupIndex] = false;
	}
	private void SelectTargetGrid()
	{

		Transform AITargetsParentObj = grassTarget[selectedGroupIndex].transform.Find("AITargets");//AiTargets Parent Object
		
		Transform[] aiTargetsChildren = AITargetsParentObj.GetComponentsInChildren<Transform>();//Transform of all AI green squares sorounding a building, free or not

		//also selects AITargets parent, but it's in the middle
		UpdateGroupPosition (selectedGroupIndex);

		int col = 0, row = 0;

		for (int i = 0; i < aiTargetsChildren.Length; i++) // 1 skip parent
		{
			int indexCell = GridManager.instance.GetGridIndex(aiTargetsChildren[i].position);
			col = GridManager.instance.GetColumn(indexCell);
			row = GridManager.instance.GetRow(indexCell);
			if(i==0)
			{
				if(DeployedUnits.Count>0)
				Talk ("New target in grid " + col +"-"+ row + ".\n"+
					                                       "Transmitting coordinates.");
				else
				{
					Talk ("Estimated value " +allLootGold.ToString() +" gold, "+ allLootMana.ToString() + " mana.");
				}
			}
			else if(!GridManager.instance.nodes[row,col].isObstacle)			 
			{
				aiTargetsFree.Add(new Vector2( Vector2.Distance(aiTargetsChildren[i].position, unitPosition[selectedGroupIndex]), i ));
			}
		}

		aiTargetsFree.Sort(delegate (Vector2 d1,Vector2 d2)
		               {
			return d1.x.CompareTo(d2.x); 
		});

		aiTargetVectorsCurrent.Clear ();

		for (int i = 0; i < aiTargetsFree.Count; i++) 
		{
			aiTargetVectorsCurrent.Add(aiTargetsChildren[(int)aiTargetsFree[i].y].position);
		}

		CopyTargetLists ();
		pauseAttack[selectedGroupIndex] = false;
	}



	[ServerRpc]
	private void MarkAsPassableServerRpc(Transform transform)
	{
		int indexCell = GridManager.instance.GetGridIndex(transform.position);
		int col = GridManager.instance.GetColumn(indexCell);
		int row = GridManager.instance.GetRow(indexCell);
		GridManager.instance.nodes [row, col].MarkAsFree ();
	}
		private void MarkAsPassable(Transform transform)
	{
		int indexCell = GridManager.instance.GetGridIndex(transform.position);
		int col = GridManager.instance.GetColumn(indexCell);
		int row = GridManager.instance.GetRow(indexCell);
		GridManager.instance.nodes [row, col].MarkAsFree ();
	}




	[ServerRpc]
	private void CopyTargetListsServerRpc()
	{
		switch (selectedGroupIndex) 
		{
		case 0:
			aiTargetVectorsO.Clear();
			for (int i = 0; i < aiTargetVectorsCurrent.Count; i++) 
			{
				aiTargetVectorsO.Add(aiTargetVectorsCurrent[i]);
			}
			break;
		case 1:
			aiTargetVectorsI.Clear();			
			for (int i = 0; i < aiTargetVectorsCurrent.Count; i++) 
			{
				aiTargetVectorsI.Add(aiTargetVectorsCurrent[i]);
			}
			break;
		case 2:
			aiTargetVectorsII.Clear();			
			for (int i = 0; i < aiTargetVectorsCurrent.Count; i++) 
			{
				aiTargetVectorsII.Add(aiTargetVectorsCurrent[i]);
			}
			break;
		case 3:
			aiTargetVectorsIII.Clear();			
			for (int i = 0; i < aiTargetVectorsCurrent.Count; i++) 
			{
				aiTargetVectorsIII.Add(aiTargetVectorsCurrent[i]);
			}
			break;		
		}
		ResetSurroundIndexServerRpc ();
	}
	private void CopyTargetLists()
	{
		switch (selectedGroupIndex) 
		{
		case 0:
			aiTargetVectorsO.Clear();
			for (int i = 0; i < aiTargetVectorsCurrent.Count; i++) 
			{
				aiTargetVectorsO.Add(aiTargetVectorsCurrent[i]);
			}
			break;
		case 1:
			aiTargetVectorsI.Clear();			
			for (int i = 0; i < aiTargetVectorsCurrent.Count; i++) 
			{
				aiTargetVectorsI.Add(aiTargetVectorsCurrent[i]);
			}
			break;
		case 2:
			aiTargetVectorsII.Clear();			
			for (int i = 0; i < aiTargetVectorsCurrent.Count; i++) 
			{
				aiTargetVectorsII.Add(aiTargetVectorsCurrent[i]);
			}
			break;
		case 3:
			aiTargetVectorsIII.Clear();			
			for (int i = 0; i < aiTargetVectorsCurrent.Count; i++) 
			{
				aiTargetVectorsIII.Add(aiTargetVectorsCurrent[i]);
			}
			break;		
		}
		ResetSurroundIndex ();
	}





	[ServerRpc]
	private void ResetSurroundIndexServerRpc()
	{
		surroundIndex[selectedGroupIndex] = 0;
	}
	private void ResetSurroundIndex()
	{
		surroundIndex[selectedGroupIndex] = 0;
	}




	[ServerRpc]
	public void StopUpdateUnitsServerRpc()
	{
		updatePathMaster = false;
	}
	public void StopUpdateUnits()
	{
		if (int.Parse(Helper.chainId) == 4000)
		{
			updatePathMaster = false;
        }
        else
        {
			StopUpdateUnitsServerRpc();

		}
	}



	
	[ServerRpc]
	public void StartUpdateUnitsServerRpc()
	{
		StartCoroutine ("ResumeUpdateUnitsServerRpc");
	}
	public void StartUpdateUnits()
	{
		if (int.Parse(Helper.chainId) == 4000)
		{
			StartCoroutine("ResumeUpdateUnits");
        }
        else
        {
			StartUpdateUnitsServerRpc();

		}
	}




	[ServerRpc]
	private void UpdatePathsServerRpc()			//makes sure that not all units update paths at the same time - performance
	{
		if(updatePathO)
		{
			updateOTimer += Time.deltaTime;

			if(updateOTimer > pathUpdateTime)//time between nextunit updatePath
			{
				updateOTimer = 0;
				if(updateOCounter < GroupO.Count)		//counter is an integer that cycles through list.Count
				{		
					if(GroupO[updateOCounter]==null) return; // the unit just died

					GroupO[updateOCounter].GetComponent<FighterPathFinder>().FindPath();
					GroupO[updateOCounter].GetComponent<FighterController>().ChangeTarget();
					GroupO[updateOCounter].GetComponent<FighterController>().RevertToPathWalk();

					updateOCounter++;
				}
				else
				{
					ResetUpdatePathsServerRpc(0);
				}
			}
		}
		else if(updatePathI)
		{
			updateITimer += Time.deltaTime;
			
			if(updateITimer > pathUpdateTime)//time between nextunit updatePath
			{
				updateITimer = 0;
				if(updateICounter < GroupI.Count)		//counter is an integer that cycles through list.Count
				{
					if(GroupI[updateICounter]==null) return;
					GroupI[updateICounter].GetComponent<FighterPathFinder>().FindPath();
					GroupI[updateICounter].GetComponent<FighterController>().ChangeTarget();
					GroupI[updateICounter].GetComponent<FighterController>().RevertToPathWalk();
					
					updateICounter++;
				}
				else
				{
					ResetUpdatePathsServerRpc(1);
				}
			}
		}
		else if(updatePathII)
		{
			updateIITimer += Time.deltaTime;
			
			if(updateIITimer > pathUpdateTime)//time between nextunit updatePath
			{
				updateIITimer = 0;
				if(updateIICounter < GroupII.Count)		//counter is an integer that cycles through list.Count
				{
					if(GroupII[updateIICounter]==null) return;
					GroupII[updateIICounter].GetComponent<FighterPathFinder>().FindPath();
					GroupII[updateIICounter].GetComponent<FighterController>().ChangeTarget();
					GroupII[updateIICounter].GetComponent<FighterController>().RevertToPathWalk();
					
					updateIICounter++;
				}
				else
				{
					ResetUpdatePathsServerRpc(2);
				}
			}
		}
		else if(updatePathIII)
		{
			updateIIITimer += Time.deltaTime;
			
			if(updateIIITimer > pathUpdateTime)//time between nextunit updatePath
			{
				updateIIITimer = 0;
				if(updateIIICounter < GroupIII.Count)		//counter is an integer that cycles through list.Count
				{
					if(GroupIII[updateIIICounter]==null) return;
					GroupIII[updateIIICounter].GetComponent<FighterPathFinder>().FindPath();
					GroupIII[updateIIICounter].GetComponent<FighterController>().ChangeTarget();
					GroupIII[updateIIICounter].GetComponent<FighterController>().RevertToPathWalk();
					
					updateIIICounter++;
				}
				else
				{
					ResetUpdatePathsServerRpc(3);
				}
			}
		}

	}
		private void UpdatePaths()			//makes sure that not all units update paths at the same time - performance
	{
		if(updatePathO)
		{
			updateOTimer += Time.deltaTime;

			if(updateOTimer > pathUpdateTime)//time between nextunit updatePath
			{
				updateOTimer = 0;
				if(updateOCounter < GroupO.Count)		//counter is an integer that cycles through list.Count
				{		
					if(GroupO[updateOCounter]==null) return; // the unit just died

					GroupO[updateOCounter].GetComponent<FighterPathFinder>().FindPath();
					GroupO[updateOCounter].GetComponent<FighterController>().ChangeTarget();
					GroupO[updateOCounter].GetComponent<FighterController>().RevertToPathWalk();

					updateOCounter++;
				}
				else
				{
					ResetUpdatePaths(0);
				}
			}
		}
		else if(updatePathI)
		{
			updateITimer += Time.deltaTime;
			
			if(updateITimer > pathUpdateTime)//time between nextunit updatePath
			{
				updateITimer = 0;
				if(updateICounter < GroupI.Count)		//counter is an integer that cycles through list.Count
				{
					if(GroupI[updateICounter]==null) return;
					GroupI[updateICounter].GetComponent<FighterPathFinder>().FindPath();
					GroupI[updateICounter].GetComponent<FighterController>().ChangeTarget();
					GroupI[updateICounter].GetComponent<FighterController>().RevertToPathWalk();
					
					updateICounter++;
				}
				else
				{
					ResetUpdatePaths(1);
				}
			}
		}
		else if(updatePathII)
		{
			updateIITimer += Time.deltaTime;
			
			if(updateIITimer > pathUpdateTime)//time between nextunit updatePath
			{
				updateIITimer = 0;
				if(updateIICounter < GroupII.Count)		//counter is an integer that cycles through list.Count
				{
					if(GroupII[updateIICounter]==null) return;
					GroupII[updateIICounter].GetComponent<FighterPathFinder>().FindPath();
					GroupII[updateIICounter].GetComponent<FighterController>().ChangeTarget();
					GroupII[updateIICounter].GetComponent<FighterController>().RevertToPathWalk();
					
					updateIICounter++;
				}
				else
				{
					ResetUpdatePaths(2);
				}
			}
		}
		else if(updatePathIII)
		{
			updateIIITimer += Time.deltaTime;
			
			if(updateIIITimer > pathUpdateTime)//time between nextunit updatePath
			{
				updateIIITimer = 0;
				if(updateIIICounter < GroupIII.Count)		//counter is an integer that cycles through list.Count
				{
					if(GroupIII[updateIIICounter]==null) return;
					GroupIII[updateIIICounter].GetComponent<FighterPathFinder>().FindPath();
					GroupIII[updateIIICounter].GetComponent<FighterController>().ChangeTarget();
					GroupIII[updateIIICounter].GetComponent<FighterController>().RevertToPathWalk();
					
					updateIIICounter++;
				}
				else
				{
					ResetUpdatePaths(3);
				}
			}
		}

	}





	[ServerRpc]
	private void ResetUpdatePathsServerRpc(int index)
	{
		switch (index) 
		{	

		case 0:
			updatePathO = false;
			updateOCounter = 0;
			updateOTimer = 0;
		break;

		case 1:
			updatePathI = false;
			updateICounter = 0;
			updateITimer = 0;
		break;

		case 2:
			updatePathII = false;
			updateIICounter = 0;
			updateIITimer = 0;
		break;

		case 3:
			updatePathIII = false;
			updateIIICounter = 0;
			updateIIITimer = 0;
		break;

		}
		if (!updatePathO && !updatePathI && !updatePathII && !updatePathIII)
			updatePathMaster = false;
	}
	private void ResetUpdatePaths(int index)
	{
		switch (index) 
		{	

		case 0:
			updatePathO = false;
			updateOCounter = 0;
			updateOTimer = 0;
		break;

		case 1:
			updatePathI = false;
			updateICounter = 0;
			updateITimer = 0;
		break;

		case 2:
			updatePathII = false;
			updateIICounter = 0;
			updateIITimer = 0;
		break;

		case 3:
			updatePathIII = false;
			updateIIICounter = 0;
			updateIIITimer = 0;
		break;

		}
		if (!updatePathO && !updatePathI && !updatePathII && !updatePathIII)
			updatePathMaster = false;
	}





	[ServerRpc]
	void lb_network_updateServerRpc()
    {
		if (mapIsEmpty || battleOver) { return; }

		for (int i = 0; i <= instantiationGroupIndex; i++)
		{
			if (updateTarget[i])
			{
				StartCoroutine(UpdateTargetServerRpcGroupServerRpc(i));
				updateTarget[i] = false;
			}
		}

		if (updatePathMaster)
		{
			UpdatePathsServerRpc();
		}

		//Damage
		elapsedTime += Time.deltaTime;

		if (elapsedTime >= intervalTime)
		{
			UpdateUnitsNoServerRpc();

			if (DeployedUnits.Count == 0) //all deployed units have died, no more slots to deploy another group/no more available units
			{
				int availableUnits = 0;


				for (int i = 0; i < ((StatsBattle)statsBattle).AvailableUnits.Count; i++)
				{
					availableUnits += ((StatsBattle)statsBattle).AvailableUnits[i].count;

					availableUnits += ((StatsBattle)statsBattle).DeployedUnits[i].count;

				}


				if (instantiationGroupIndex == 3 || availableUnits == 0)
				{
					if (!battleOver)
						StartCoroutine("MissionCompleteServerRpc");
					return;
				}
			}
			elapsedTime = 0.0f;

			//UpdateCannons ();

			for (int i = 0; i < currentDamage.Length; i++) { currentDamage[i] = 0; }//reset the current damage array of 4

			bool noBuildingUnderAttack = true;

			for (int i = 0; i < DeployedUnits.Count; i++)
			{
				if (DeployedUnits[i] != null)
				{
					if (DeployedUnits[i].GetComponent<FighterController>().currentState == FighterController.UnitState.Attack)
					{
						currentDamage[DeployedUnits[i].GetComponent<FighterController>().assignedToGroup] = DeployedUnits[i].GetComponent<FighterController>().DamagePoints;
						noBuildingUnderAttack = false;
					}
				}
			}

			if (BuildingsHealth.Count == 0 || noBuildingUnderAttack)
				return;//print ("no building under attack + buildingshealth.count=0");}//empty map - no buildings || no building under attack

			int[] gainPerBuilding = new int[Buildings.Count];//same count as Buildings

			for (int i = 0; i <= instantiationGroupIndex; i++)
			{
				if (!pauseAttack[i] && currentDamage[i] > 0)
				{
					int k = GetListIndexServerRpc(targetStructureIndex[i]);
					//print(k.ToString());
					gain = (currentDamage[i] * BuildingValues[k]) / 100;
					gainPerBuilding[k] += gain;
					BuildingsHealth[k] -= 1 * currentDamage[i];
					DamageBars[k].GetComponent<Slider>().value = BuildingsHealth[k] * 0.01f;//since full building health was 100

				}
			}

			for (int i = 0; i < gainPerBuilding.Length; i++)
			{
				if (gainPerBuilding[i] > 0)
				{

					Vector2 pos = Buildings[i].transform.position;

					if (BuildingCurrency[i] == "Gold")
					{
						GameObject GainGold = Instantiate(GainGoldPf);
						if (int.Parse(Helper.chainId) != 4000)
						{
							ServerRpc.SpawnObjectServerRpc(GainGold);
						}
						else
						{
							ServerRpc.removeNetworkObject(GainGold);
						}
						GainGold.transform.SetParent(EffectsGroup.transform);
						GainGold.transform.position = new Vector3(pos.x, pos.y + 100, smokeZ);
						((StatsBattle)statsBattle).gold += gainPerBuilding[i];//approximates a bit- the building health might reach -17 at last hit 
						GainGold.GetComponentInChildren<Text>().text = "+ " + gainPerBuilding[i].ToString();
						GainGold.transform.SetParent(EffectsGroup.transform);
					}
					else
					{
						GameObject GainMana = Instantiate(GainManaPf);
						if (int.Parse(Helper.chainId) != 4000)
						{
							ServerRpc.SpawnObjectServerRpc(GainMana);
						}
						else
						{
							ServerRpc.removeNetworkObject(GainMana);
						}
						GainMana.transform.SetParent(EffectsGroup.transform);
						GainMana.transform.position = new Vector3(pos.x, pos.y + 100, smokeZ);
						((StatsBattle)statsBattle).mana += gainPerBuilding[i];
						GainMana.GetComponentInChildren<Text>().text = "+ " + gainPerBuilding[i].ToString();
					}
				}
			}
			if (int.Parse(Helper.chainId) != 4000)
			{
				((StatsBattle)statsBattle).UpdateUIServerRpc();
			}

			for (int k = 0; k < targetStructureIndex.Length; k++)
			{
				int structureIndex = targetStructureIndex[k];
				int i = GetListIndexServerRpc(structureIndex);

				if (!DamageEffectsRunning[i] && BuildingsHealth[i] < 90) //instantiates smoke the first time the building is attacked/below 90 health
				{
					//((SoundFX)SoundFx).BuildingBurn(); //sound gets too clogged, remain silent
					DamageEffectsRunning[i] = true;

					GameObject Smoke = (GameObject)Instantiate(SmokePf, new Vector3(Buildings[i].transform.position.x,
																					  Buildings[i].transform.position.y,
																					  smokeZ), Quaternion.identity);

					if (int.Parse(Helper.chainId) != 4000)
					{
						ServerRpc.SpawnObjectServerRpc(Smoke);
					}
					else
					{
						ServerRpc.removeNetworkObject(Smoke);
					}
					Smoke.transform.parent = EffectsGroup.transform;

					GameObject Fire = (GameObject)Instantiate(FirePf, new Vector3(Buildings[i].transform.position.x,
																				  Buildings[i].transform.position.y,
																				  fireZ), Quaternion.identity);
					if (int.Parse(Helper.chainId) != 4000)
					{
						ServerRpc.SpawnObjectServerRpc(Fire);
					}
					else
					{
						ServerRpc.removeNetworkObject(Fire);
					}
					Fire.transform.parent = EffectsGroup.transform;

				}

				if (BuildingsHealth[i] < 0 && !grassPatches[i].GetComponent<GrassSelector>().isDestroyed)//explodes once, then marks the grass as destroyed  
				{
					((SoundFX)soundFx).BuildingExplode();
					((StatsBattle)statsBattle).buildingsDestroyed++;

					Talk("Target destroyed. Incoming EMP shockwave.\nRetreatServerRpc to a safe distance.\nExpect brief communications blackout.");
					HeliosTween.Instance.PlayFadeOut();
					StartCoroutine("GarbleMessage");

					//DamageBars[i].GetComponent<UIPanel>().alpha = 0;//make them invisible
					DamageBars[i].SetActive(false);

					GameObject Explosion = (GameObject)Instantiate(ExplosionPf, new Vector3(Buildings[i].transform.position.x, Buildings[i].transform.position.y, smokeZ), Quaternion.identity);

					if (int.Parse(Helper.chainId) != 4000)
					{
						ServerRpc.SpawnObjectServerRpc(Explosion);
					}
					else
					{
						ServerRpc.removeNetworkObject(Explosion);
					}

					//to instantiate rubble of different sizes - different prefabs or you can scale down a bit
					string buildingType = ((StructureSelector)Buildings[i].GetComponent<StructureSelector>()).structureType,
						buildingClass = ((StructureSelector)Buildings[i].GetComponent<StructureSelector>()).structureClass;

					if (buildingType == "Chessboard" || buildingType == "Toolhouse" || buildingType == "Summon")
					{
						GameObject Rubble2x2 = (GameObject)Instantiate(Rubble2x2Pf, new Vector3(Buildings[i].transform.position.x, Buildings[i].transform.position.y, zeroZ), Quaternion.identity);
						if (int.Parse(Helper.chainId) != 4000)
						{
							ServerRpc.SpawnObjectServerRpc(Rubble2x2);
						}
						else
						{
							ServerRpc.removeNetworkObject(Rubble2x2);
						}
						AdjustRubbleZServerRpc(Rubble2x2);
						Rubble2x2.transform.parent = EffectsGroup.transform;
					}

					else if (buildingClass == "Weapon" || buildingType == "Tatami")
					{
						GameObject Rubble1x1 = (GameObject)Instantiate(Rubble1x1Pf, new Vector3(Buildings[i].transform.position.x, Buildings[i].transform.position.y, zeroZ), Quaternion.identity);
						if (int.Parse(Helper.chainId) != 4000)
						{
							ServerRpc.SpawnObjectServerRpc(Rubble1x1);
						}
						else
						{
							ServerRpc.removeNetworkObject(Rubble1x1);
						}
						AdjustRubbleZServerRpc(Rubble1x1);
						Rubble1x1.transform.parent = EffectsGroup.transform;
					}
					else
					{
						GameObject Rubble3x3 = (GameObject)Instantiate(Rubble3x3Pf, new Vector3(Buildings[i].transform.position.x, Buildings[i].transform.position.y, zeroZ), Quaternion.identity);
						if (int.Parse(Helper.chainId) != 4000)
						{
							ServerRpc.SpawnObjectServerRpc(Rubble3x3);
						}
						else
						{
							ServerRpc.removeNetworkObject(Rubble3x3);
						}
						AdjustRubbleZServerRpc(Rubble3x3);
						Rubble3x3.transform.parent = EffectsGroup.transform;
					}

					string
						structureClass = ((StructureSelector)Buildings[i].GetComponent<StructureSelector>()).structureClass,
						structureType = ((StructureSelector)Buildings[i].GetComponent<StructureSelector>()).structureType;

					if (structureClass == "Weapon" && (structureType == "Cannon" || structureType == "ArcherTower" || structureType == "Catapult"))
					{
						Buildings[i].GetComponentInChildren<PerimeterCollider>().enabled = false;//prevent the perimeter collider from firing again
						Buildings[i].GetComponent<TurretControllerBase>().fire = false;
						Buildings[i].GetComponent<TurretControllerBase>().enabled = false;
						Buildings[i].transform.Find("Turret").gameObject.SetActive(false);
					}
					else if (structureType == "DronePad" || structureType == "Bomb")
					{
						//Buildings [i].GetComponentInChildren<PerimeterCollider> ().enabled = false;
					}

					Buildings[i].transform.Find("Sprites").gameObject.SetActive(false);
					Buildings[i].transform.Find("SelectBuildingButton").gameObject.SetActive(false);
					//Cannons[i].transform.FindChild("Turret").gameObject.SetActive(false);

					if (!Buildings[i].activeSelf)//constructions- the building is disabled
					{
						Buildings[i].transform.parent.GetComponent<ConstructionSelector>().enabled = false;
						//Buildings [i].transform.parent.FindChild ("TimeCounterLb").gameObject.SetActive (false);
						Buildings[i].transform.parent.Find("Dobbit").gameObject.SetActive(false);
						Buildings[i].transform.parent.Find("Sprites").gameObject.SetActive(false);
					}

					for (int j = 0; j < targetStructureIndex.Length; j++) //change the obstacles after a building is destroyed
					{
						if (targetStructureIndex[j] == structureIndex)
						{
							grassTarget[j].GetComponent<GrassSelector>().isDestroyed = true;

							int grassType = grassTarget[j].GetComponent<GrassSelector>().grassType;     //this section will make the grass passable, except its center

							if (grassType == 2) break;

							//disregard type 2 grass, the rubble prefab is too big to make any portion of it passable; 
							//applies only to toolhouse, but since you can have only 1 toolhouse, there is no situation
							//when a building is surrounded with toolhouses

							Transform AIObstaclesParentObj = grassTarget[j].transform.Find("AIObstacles");//AiObstacles parent object						
							Transform[] aiObstaclesChildren = AIObstaclesParentObj.GetComponentsInChildren<Transform>();//all the little obstacle cubes

							switch (grassType) // to see the red obstacles/green targets, activate the mesh renderers of the cubes inside the battlemap grass prefabs
							{
								case 4:
									//skip all 4 center tiles - they remain obstacles
									for (int l = 1; l < aiObstaclesChildren.Length; l++) // 1 skip parent
									{
										if (l != 3 && l != 4 && l != 6 && l != 7)//skip center
										{
											MarkAsPassableServerRpc(aiObstaclesChildren[l]);
											aiObstaclesChildren[l].gameObject.tag = "Untagged";
										}
									}

									break;
							}
							GridManager.instance.UpdateObstacleArray();//for the square gizmos to appear properly at GridManager/ShowObstacleBlocks
							break;
						}
					}
					processCounter = 0;

					for (int j = 0; j <= instantiationGroupIndex; j++)  //the index for TargetBuildingIndex is the unit group !!!
					{
						if (targetStructureIndex[j] == structureIndex)
						{
							ProcessUnitGroupServerRpc(j);
						}
					}
				}
			}
		}
	}
	void lb_network_update()
    {
		if (mapIsEmpty || battleOver) { return; }

		for (int i = 0; i <= instantiationGroupIndex; i++)
		{
			if (updateTarget[i])
			{
				StartCoroutine(UpdateTargetGroup(i));
				updateTarget[i] = false;
			}
		}

		if (updatePathMaster)
		{
			UpdatePaths();
		}

		//Damage
		elapsedTime += Time.deltaTime;

		if (elapsedTime >= intervalTime)
		{
			UpdateUnitsNo();

			if (DeployedUnits.Count == 0) //all deployed units have died, no more slots to deploy another group/no more available units
			{
				int availableUnits = 0;


				for (int i = 0; i < ((StatsBattle)statsBattle).AvailableUnits.Count; i++)
				{
					availableUnits += ((StatsBattle)statsBattle).AvailableUnits[i].count;

					availableUnits += ((StatsBattle)statsBattle).DeployedUnits[i].count;

				}


				if (instantiationGroupIndex == 3 || availableUnits == 0)
				{
					if (!battleOver)
						StartCoroutine("MissionComplete");
					return;
				}
			}
			elapsedTime = 0.0f;

			//UpdateCannons ();

			for (int i = 0; i < currentDamage.Length; i++) { currentDamage[i] = 0; }//reset the current damage array of 4

			bool noBuildingUnderAttack = true;

			for (int i = 0; i < DeployedUnits.Count; i++)
			{
				if (DeployedUnits[i] != null)
				{
					if (DeployedUnits[i].GetComponent<FighterController>().currentState == FighterController.UnitState.Attack)
					{
						currentDamage[DeployedUnits[i].GetComponent<FighterController>().assignedToGroup] = DeployedUnits[i].GetComponent<FighterController>().DamagePoints;
						noBuildingUnderAttack = false;
					}
				}
			}

			if (BuildingsHealth.Count == 0 || noBuildingUnderAttack)
				return;//print ("no building under attack + buildingshealth.count=0");}//empty map - no buildings || no building under attack

			int[] gainPerBuilding = new int[Buildings.Count];//same count as Buildings

			for (int i = 0; i <= instantiationGroupIndex; i++)
			{
				if (!pauseAttack[i] && currentDamage[i] > 0)
				{
					int k = GetListIndex(targetStructureIndex[i]);
					//print(k.ToString());
					gain = (currentDamage[i] * BuildingValues[k]) / 100;
					gainPerBuilding[k] += gain;
					BuildingsHealth[k] -= 1 * currentDamage[i];
					DamageBars[k].GetComponent<Slider>().value = BuildingsHealth[k] * 0.01f;//since full building health was 100

				}
			}

			for (int i = 0; i < gainPerBuilding.Length; i++)
			{
				if (gainPerBuilding[i] > 0)
				{

					Vector2 pos = Buildings[i].transform.position;

					if (BuildingCurrency[i] == "Gold")
					{
						GameObject GainGold = Instantiate(GainGoldPf);
						if (int.Parse(Helper.chainId) != 4000)
						{
							ServerRpc.SpawnObjectServerRpc(GainGold);
						}
						else
						{
							ServerRpc.removeNetworkObject(GainGold);
						}
						GainGold.transform.SetParent(EffectsGroup.transform);
						GainGold.transform.position = new Vector3(pos.x, pos.y + 100, smokeZ);
						((StatsBattle)statsBattle).gold += gainPerBuilding[i];//approximates a bit- the building health might reach -17 at last hit 
						GainGold.GetComponentInChildren<Text>().text = "+ " + gainPerBuilding[i].ToString();
						GainGold.transform.SetParent(EffectsGroup.transform);
					}
					else
					{
						GameObject GainMana = Instantiate(GainManaPf);
						if (int.Parse(Helper.chainId) != 4000)
						{
							ServerRpc.SpawnObjectServerRpc(GainMana);
						}
						else
						{
							ServerRpc.removeNetworkObject(GainMana);
						}
						GainMana.transform.SetParent(EffectsGroup.transform);
						GainMana.transform.position = new Vector3(pos.x, pos.y + 100, smokeZ);
						((StatsBattle)statsBattle).mana += gainPerBuilding[i];
						GainMana.GetComponentInChildren<Text>().text = "+ " + gainPerBuilding[i].ToString();
					}
				}
			}

			((StatsBattle)statsBattle).UpdateUI();

			for (int k = 0; k < targetStructureIndex.Length; k++)
			{
				int structureIndex = targetStructureIndex[k];
				int i = GetListIndex(structureIndex);

				if (!DamageEffectsRunning[i] && BuildingsHealth[i] < 90) //instantiates smoke the first time the building is attacked/below 90 health
				{
					//((SoundFX)SoundFx).BuildingBurn(); //sound gets too clogged, remain silent
					DamageEffectsRunning[i] = true;

					GameObject Smoke = (GameObject)Instantiate(SmokePf, new Vector3(Buildings[i].transform.position.x,
																					  Buildings[i].transform.position.y,
																					  smokeZ), Quaternion.identity);

				
					

					GameObject Fire = (GameObject)Instantiate(FirePf, new Vector3(Buildings[i].transform.position.x,
																				  Buildings[i].transform.position.y,
																				  fireZ), Quaternion.identity);

					if (int.Parse(Helper.chainId) != 4000)
					{
						ServerRpc.SpawnObjectServerRpc(Smoke);
						ServerRpc.SpawnObjectServerRpc(Fire);

					}
					else
					{
						ServerRpc.removeNetworkObject(Smoke);
						ServerRpc.removeNetworkObject(Fire);
					}

					Smoke.transform.parent = EffectsGroup.transform;
					Fire.transform.parent = EffectsGroup.transform;

				}

				if (BuildingsHealth[i] < 0 && !grassPatches[i].GetComponent<GrassSelector>().isDestroyed)//explodes once, then marks the grass as destroyed  
				{
					((SoundFX)soundFx).BuildingExplode();
					((StatsBattle)statsBattle).buildingsDestroyed++;

					Talk("Target destroyed. Incoming EMP shockwave.\nRetreat to a safe distance.\nExpect brief communications blackout.");
					//HeliosTween.Instance.PlayFadeOut();
					StartCoroutine("GarbleMessage");

					//DamageBars[i].GetComponent<UIPanel>().alpha = 0;//make them invisible
					DamageBars[i].SetActive(false);

					GameObject Explosion = (GameObject)Instantiate(ExplosionPf, new Vector3(Buildings[i].transform.position.x, Buildings[i].transform.position.y, smokeZ), Quaternion.identity);

					if (int.Parse(Helper.chainId) != 4000)
					{
						ServerRpc.SpawnObjectServerRpc(Explosion);
					}
					else
					{
						ServerRpc.removeNetworkObject(Explosion);
					}

					//to instantiate rubble of different sizes - different prefabs or you can scale down a bit
					string buildingType = ((StructureSelector)Buildings[i].GetComponent<StructureSelector>()).structureType,
						buildingClass = ((StructureSelector)Buildings[i].GetComponent<StructureSelector>()).structureClass;

					if (buildingType == "Chessboard" || buildingType == "Toolhouse" || buildingType == "Summon")
					{
						GameObject Rubble2x2 = (GameObject)Instantiate(Rubble2x2Pf, new Vector3(Buildings[i].transform.position.x, Buildings[i].transform.position.y, zeroZ), Quaternion.identity);

						if (int.Parse(Helper.chainId) != 4000)
						{
							ServerRpc.SpawnObjectServerRpc(Rubble2x2);
						}
						else
						{
							ServerRpc.removeNetworkObject(Rubble2x2);
						}

						AdjustRubbleZ(Rubble2x2);
						Rubble2x2.transform.parent = EffectsGroup.transform;
					}

					else if (buildingClass == "Weapon" || buildingType == "Tatami")
					{
						GameObject Rubble1x1 = (GameObject)Instantiate(Rubble1x1Pf, new Vector3(Buildings[i].transform.position.x, Buildings[i].transform.position.y, zeroZ), Quaternion.identity);

						if (int.Parse(Helper.chainId) != 4000)
						{
							ServerRpc.SpawnObjectServerRpc(Rubble1x1);
						}
						else
						{
							ServerRpc.removeNetworkObject(Rubble1x1);
						}

						AdjustRubbleZ(Rubble1x1);
						Rubble1x1.transform.parent = EffectsGroup.transform;
					}
					else
					{
						GameObject Rubble3x3 = (GameObject)Instantiate(Rubble3x3Pf, new Vector3(Buildings[i].transform.position.x, Buildings[i].transform.position.y, zeroZ), Quaternion.identity);

						if (int.Parse(Helper.chainId) != 4000)
						{
							ServerRpc.SpawnObjectServerRpc(Rubble3x3);
						}
						else
						{
							ServerRpc.removeNetworkObject(Rubble3x3);
						}

						AdjustRubbleZ(Rubble3x3);
						Rubble3x3.transform.parent = EffectsGroup.transform;
					}

					string
						structureClass = ((StructureSelector)Buildings[i].GetComponent<StructureSelector>()).structureClass,
						structureType = ((StructureSelector)Buildings[i].GetComponent<StructureSelector>()).structureType;

					if (structureClass == "Weapon" && (structureType == "Cannon" || structureType == "ArcherTower" || structureType == "Catapult"))
					{
						Buildings[i].GetComponentInChildren<PerimeterCollider>().enabled = false;//prevent the perimeter collider from firing again
						Buildings[i].GetComponent<TurretControllerBase>().fire = false;
						Buildings[i].GetComponent<TurretControllerBase>().enabled = false;
						Buildings[i].transform.Find("Turret").gameObject.SetActive(false);
					}
					else if (structureType == "DronePad" || structureType == "Bomb")
					{
						//Buildings [i].GetComponentInChildren<PerimeterCollider> ().enabled = false;
					}

					Buildings[i].transform.Find("Sprites").gameObject.SetActive(false);
					Buildings[i].transform.Find("SelectBuildingButton").gameObject.SetActive(false);
					//Cannons[i].transform.FindChild("Turret").gameObject.SetActive(false);

					if (!Buildings[i].activeSelf)//constructions- the building is disabled
					{
						Buildings[i].transform.parent.GetComponent<ConstructionSelector>().enabled = false;
						//Buildings [i].transform.parent.FindChild ("TimeCounterLb").gameObject.SetActive (false);
						Buildings[i].transform.parent.Find("Dobbit").gameObject.SetActive(false);
						Buildings[i].transform.parent.Find("Sprites").gameObject.SetActive(false);
					}

					for (int j = 0; j < targetStructureIndex.Length; j++) //change the obstacles after a building is destroyed
					{
						if (targetStructureIndex[j] == structureIndex)
						{
							grassTarget[j].GetComponent<GrassSelector>().isDestroyed = true;

							int grassType = grassTarget[j].GetComponent<GrassSelector>().grassType;     //this section will make the grass passable, except its center

							if (grassType == 2) break;

							//disregard type 2 grass, the rubble prefab is too big to make any portion of it passable; 
							//applies only to toolhouse, but since you can have only 1 toolhouse, there is no situation
							//when a building is surrounded with toolhouses

							Transform AIObstaclesParentObj = grassTarget[j].transform.Find("AIObstacles");//AiObstacles parent object						
							Transform[] aiObstaclesChildren = AIObstaclesParentObj.GetComponentsInChildren<Transform>();//all the little obstacle cubes

							switch (grassType) // to see the red obstacles/green targets, activate the mesh renderers of the cubes inside the battlemap grass prefabs
							{
								case 4:
									//skip all 4 center tiles - they remain obstacles
									for (int l = 1; l < aiObstaclesChildren.Length; l++) // 1 skip parent
									{
										if (l != 3 && l != 4 && l != 6 && l != 7)//skip center
										{
											MarkAsPassable(aiObstaclesChildren[l]);
											aiObstaclesChildren[l].gameObject.tag = "Untagged";
										}
									}

									break;
							}
							GridManager.instance.UpdateObstacleArray();//for the square gizmos to appear properly at GridManager/ShowObstacleBlocks
							break;
						}
					}
					processCounter = 0;

					for (int j = 0; j <= instantiationGroupIndex; j++)  //the index for TargetBuildingIndex is the unit group !!!
					{
						if (targetStructureIndex[j] == structureIndex)
						{
							ProcessUnitGroup(j);
						}
					}
				}
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (int.Parse(Helper.chainId) == 4000)
		{
			lb_network_update();
		}
        else {
			lb_network_updateServerRpc();
		}
	}
	[ServerRpc]
	private void AdjustRubbleZServerRpc(GameObject rubble)
	{
		Vector3 pivotPos = rubble.transform.GetChild (0).position; //pivot
		Vector3 spritesPos = rubble.transform.GetChild (1).position;//sprites

		float correctiony = 10 / (pivotPos.y + 3300);//ex: fg 10 = 0.1   bg 20 = 0.05  
		//all y values must be positive, so we add the grid origin y 3207 +100 to avoid divide by 0; 
		//otherwise depth glitches around y 0
		
		rubble.transform.GetChild(1).position = new Vector3(spritesPos.x, spritesPos.y, zeroZ - correctiony);//	transform.GetChild(2).position   
		
	}
	private void AdjustRubbleZ(GameObject rubble)
	{
		Vector3 pivotPos = rubble.transform.GetChild (0).position; //pivot
		Vector3 spritesPos = rubble.transform.GetChild (1).position;//sprites

		float correctiony = 10 / (pivotPos.y + 3300);//ex: fg 10 = 0.1   bg 20 = 0.05  
		//all y values must be positive, so we add the grid origin y 3207 +100 to avoid divide by 0; 
		//otherwise depth glitches around y 0
		
		rubble.transform.GetChild(1).position = new Vector3(spritesPos.x, spritesPos.y, zeroZ - correctiony);//	transform.GetChild(2).position   
		
	}



	[ServerRpc]
	public void UpdateUnitsNoServerRpc()
	{
		unitsNo [0].text = GroupO.Count.ToString ();
		unitsNo [1].text = GroupI.Count.ToString ();
		unitsNo [2].text = GroupII.Count.ToString ();
		unitsNo [3].text = GroupIII.Count.ToString ();
	}
	public void UpdateUnitsNo()
	{
		unitsNo [0].text = GroupO.Count.ToString ();
		unitsNo [1].text = GroupI.Count.ToString ();
		unitsNo [2].text = GroupII.Count.ToString ();
		unitsNo [3].text = GroupIII.Count.ToString ();
	}



	[ServerRpc]
	private IEnumerator LateProcessUnitGroupServerRpcServerRpc(int index)
	{
		processCounter += 2;
		yield return new WaitForSeconds (processCounter);

		if(((Relay)relay).deploying)
		{
			StartCoroutine(LateProcessUnitGroupServerRpcServerRpc(index));
		}
		else
			ProcessUnitGroupServerRpc (index);

	}
	private IEnumerator LateProcessUnitGroup(int index)
	{
		processCounter += 2;
		yield return new WaitForSeconds (processCounter);

		if(((Relay)relay).deploying)
		{
			StartCoroutine(LateProcessUnitGroup(index));
		}
		else
			ProcessUnitGroup (index);

	}




	[ServerRpc]
	public void ProcessUnitGroupServerRpc(int index)
	{
		if (battleOver)
			return;

		if(((Relay)relay).deploying)
		{
			StartCoroutine(LateProcessUnitGroupServerRpcServerRpc(index));
			return;
		}

		bool groupDead = true;

		UpdateGroupPositionServerRpc (index);//necessary to separate since it is called by other methods

		switch (index)
		{
		case 0:
			if(GroupO.Count>0)
			{
				Select0ServerRpc();					
				groupDead = false;
			}
			break;

		case 1:
			if(GroupI.Count>0)
			{
				Select1ServerRpc();					
				groupDead = false;
			}
			break;

		case 2:
			if(GroupII.Count>0)
			{
				Select2ServerRpc();
				groupDead = false;
			}				
			break;

		case 3:
			if(GroupIII.Count>0)
			{
				Select3ServerRpc();
				groupDead = false;
			}								
			break;
		}

		if (groupDead) 		
			return;			

		SearchAliveBuildingsServerRpc ();
		
		if(!allDestroyed)
		{ 
			FindNearestBuildingServerRpc();
		}
		else
		{
			mapIsEmpty = true;
			if(!battleOver)
			StartCoroutine("MissionCompleteServerRpc");
			return;
		}

		pauseAttack[index] = true;
		updateTarget[index] = true;
	}

	public void ProcessUnitGroup(int index)
	{
		if (battleOver)
			return;

		if(((Relay)relay).deploying)
		{
			StartCoroutine(LateProcessUnitGroup(index));
			return;
		}

		bool groupDead = true;

		UpdateGroupPosition (index);//necessary to separate since it is called by other methods

		switch (index)
		{
		case 0:
			if(GroupO.Count>0)
			{
				Select0();					
				groupDead = false;
			}
			break;

		case 1:
			if(GroupI.Count>0)
			{
				Select1();					
				groupDead = false;
			}
			break;

		case 2:
			if(GroupII.Count>0)
			{
				Select2();
				groupDead = false;
			}				
			break;

		case 3:
			if(GroupIII.Count>0)
			{
				Select3();
				groupDead = false;
			}								
			break;
		}

		if (groupDead) 		
			return;			

		SearchAliveBuildings ();
		
		if(!allDestroyed)
		{ 
			FindNearestBuilding();
		}
		else
		{
			mapIsEmpty = true;
			if(!battleOver)
			StartCoroutine("MissionComplete");
			return;
		}

		pauseAttack[index] = true;
		updateTarget[index] = true;
	}

	[ServerRpc]
	private void UpdateSelectStarsServerRpc()//the little stars above each unit
	{
		for (int i = 0; i < DeployedUnits.Count; i++) 
		{
			if(DeployedUnits[i]!=null)
			{
				DeployedUnits[i].transform.GetChild(0).gameObject.SetActive(false);
			}
		}

		for (int i = 0; i < selectedList.Count; i++) 
		{ 
			if(selectedList[i]!=null)
			{
				selectedList[i].transform.GetChild(0).gameObject.SetActive(true);
			}
		}
	}
	private void UpdateSelectStars()//the little stars above each unit
	{
		for (int i = 0; i < DeployedUnits.Count; i++) 
		{
			if(DeployedUnits[i]!=null)
			{
				DeployedUnits[i].transform.GetChild(0).gameObject.SetActive(false);
			}
		}

		for (int i = 0; i < selectedList.Count; i++) 
		{ 
			if(selectedList[i]!=null)
			{
				selectedList[i].transform.GetChild(0).gameObject.SetActive(true);
			}
		}
	}




	[ServerRpc]
	private void UpdateGroupPositionServerRpc(int index)		//the group position is given by the first alive member for simplification
	{
		switch (index) 
		{
		case 0:
			if(GroupO.Count>0)
			{			
				unitPosition[0] = GroupO[0].transform.position;
			}
			break;
		case 1:
			if(GroupI.Count>0)
			{
				unitPosition[1] = GroupI[0].transform.position;
			}
			break;
		case 2:
			if(GroupII.Count>0)
			{
				unitPosition[2] = GroupII[0].transform.position;
			}
			break;
		case 3:
			if(GroupIII.Count>0)
			{
				unitPosition[3] = GroupIII[0].transform.position;
			}
			break;
		}
	}
		private void UpdateGroupPosition(int index)		//the group position is given by the first alive member for simplification
	{
		switch (index) 
		{
		case 0:
			if(GroupO.Count>0)
			{			
				unitPosition[0] = GroupO[0].transform.position;
			}
			break;
		case 1:
			if(GroupI.Count>0)
			{
				unitPosition[1] = GroupI[0].transform.position;
			}
			break;
		case 2:
			if(GroupII.Count>0)
			{
				unitPosition[2] = GroupII[0].transform.position;
			}
			break;
		case 3:
			if(GroupIII.Count>0)
			{
				unitPosition[3] = GroupIII[0].transform.position;
			}
			break;
		}
	}





	[ServerRpc]
	IEnumerator UpdateTargetServerRpcGroupServerRpc(int index)
	{	
		yield return new WaitForSeconds (1.5f);//1.5f
		if(!allDestroyed)
		UpdateTargetServerRpc(index);
	}
	IEnumerator UpdateTargetGroup(int index)
	{	
		yield return new WaitForSeconds (1.5f);//1.5f
		if(!allDestroyed)
		UpdateTarget(index);
	}





	[ServerRpc]
	private void UpdateTargetServerRpc(int index)
	{	
		if (instantiationGroupIndex == -1)
						return;

		updatePathMaster = false; //stop update sequence for all
		StopCoroutine ("ResumeUpdateUnitsServerRpc"); //stop resumeupdate in case it was called

		List<GameObject> currentList = new List<GameObject> ();
		currentList.Clear ();

		UpdateGroupPositionServerRpc (index);

		switch (index) 
		{
		case 0:
			if(GroupO.Count>0)
			{
				Talk("Squad 1 moving to objective.");
				currentList= GroupO;
				updatePathO = true;
				updateOCounter = 0;
			}
			break;
		case 1:
			if(GroupI.Count>0)
			{
				Talk("Squad 2 moving to objective.");
				currentList= GroupI;
				updatePathI = true;
				updateICounter = 0;
			}
			break;
		case 2:
			if(GroupII.Count>0)
			{
				Talk("Squad 3 moving to objective.");
				currentList= GroupII;
				updatePathII = true;
				updateIICounter = 0;
			}
			break;
		case 3:
			if(GroupIII.Count>0)
			{
				Talk("Squad 4 moving to objective.");
				currentList= GroupIII;
				updatePathIII = true;
				updateIIICounter = 0;
			}
			break;
		}

		ResetSurroundIndexServerRpc (); //this is used to suround a building with units

		for (int i = 0; i < currentList.Count; i++)
		{
			currentList[i].GetComponent<FighterController>().targetCenter = new Vector3(targetCenter[index].x,targetCenter[index].y,targetZ);//selectedGroupIndex
			currentList[i].GetComponent<FighterController>().RevertToIdle();
		}

		StartCoroutine ("ResumeUpdateUnitsServerRpc");
		elapsedTime = 0.0f;
		pauseAttack[index] = false;
	}
	[ServerRpc]
	private IEnumerator ResumeUpdateUnitsServerRpc()
	{
		yield return new WaitForSeconds(0.3f);
		updatePathMaster = true;
	}
	[ServerRpc]
	private void DisablePanelsServerRpc()
	{
		((Relay)relay).pauseInput = true;
	}
	[ServerRpc]
	public void RetreatServerRpc()
	{
		Talk ("RetreatServerRpc order received.");
		StartCoroutine ("MissionCompleteServerRpc");
	}
	[ServerRpc]
	IEnumerator MissionCompleteServerRpc()	
	{
		Talk ("Well done. Return to base.");
		battleOver = true;
		DisablePanelsServerRpc ();
		yield return new WaitForSeconds(0.5f);

		for (int i = 0; i < DeployedUnits.Count; i++) 
		{
			if(DeployedUnits[i]!=null)
			{
				DeployedUnits[i].GetComponent<FighterController>().RevertToIdle();
			}
		}
		ActivateEndGameServerRpc ();
	}
	[ServerRpc]
	private void ActivateEndGameServerRpc()
	{
		MissionCompleteServerRpcPanel.SetActive (true);
		
		goldNoLb.text = ((StatsBattle)statsBattle).gold.ToString();
		manaNoLb.text = ((StatsBattle)statsBattle).mana.ToString();
		buildingsDestroyedNoLb.text = ((StatsBattle)statsBattle).buildingsDestroyed.ToString();
		unitsLostNoLb.text = ((StatsBattle)statsBattle).unitsLost.ToString();

		int remainingUnits = 0;


		for (int i = 0; i < ((StatsBattle)statsBattle).AvailableUnits.Count; i++) 
		{
			remainingUnits += ((StatsBattle)statsBattle).AvailableUnits[i].count;
			((TransData)transData).ReturnedFromBattleUnits[i] = ((TransData)transData).GoingToBattleUnits[i];
		}

		unitsRecoveredNoLb.text = remainingUnits.ToString();

		((SaveLoadBattle)saveLoadBattle).SaveAttack();

		((TransData)transData).goldGained = (int)((StatsBattle)statsBattle).gold;
		((TransData)transData).manaGained = (int)((StatsBattle)statsBattle).mana;
		((TransData)transData).battleOver = true;//this variable is checked at game.unity load to see if the user is returning from battle or just started the game
		((TransData)transData).tutorialBattleSeen = ((StatsBattle)statsBattle).tutorialBattleSeen;

	}


	//          CHEATS             \\

	//Developer section - cheats for testing
	[ServerRpc]
	private void AllUnitStarsServerRpc(bool b)
	{
		if (instantiationUnitIndex == -1) return;
		for (int i = 0; i <= instantiationGroupIndex; i++) //
		{
			unitStar[i].SetActive(b);
		}
	}
	[ServerRpc]
	private void SelectedUnitStarServerRpc(bool b)
	{
		unitStar[selectedGroupIndex].SetActive(b);
	}
	[ServerRpc]
	private void AllUnitsImmortalServerRpc(bool b)
	{
		int life = 100;
		if(b) life = 1000000;

		for (int i = 0; i < DeployedUnits.Count; i++) 
		{
			if(DeployedUnits[i]!=null)
			DeployedUnits[i].GetComponent<FighterController>().Life = life;
		}
		AllUnitStarsServerRpc (b);
	}
	[ServerRpc]
	private void SelectedImmortalServerRpc(bool b)//Helios may selects another group
	{
		int life = 100;
		if(b) life = 1000000;
		SelectedUnitStarServerRpc(b);

		switch (selectedGroupIndex) 
		{
		case 0:
			for (int i = 0; i < GroupO.Count; i++) 
			{	
				if(GroupO[i]!=null)
				GroupO[i].GetComponent<FighterController>().Life = life;
			}
			break;
		case 1:
			for (int i = 0; i < GroupI.Count; i++) 
			{
				if(GroupI[i]!=null)
				GroupI[i].GetComponent<FighterController>().Life = life;
			}
			break;
		case 2:
			for (int i = 0; i < GroupII.Count; i++) 
			{
				if(GroupII[i]!=null)
				GroupII[i].GetComponent<FighterController>().Life = life;
			}
			break;
		case 3:
			for (int i = 0; i < GroupIII.Count; i++) 
			{
				if(GroupIII[i]!=null)
				GroupIII[i].GetComponent<FighterController>().Life = life;
			}
			break;
		}
	}
	[ServerRpc]
	private void AllBuildingsIndestructibleServerRpc(bool b)
	{
		int life = 100;
		if(b) life = 1000000;

		for (int i = 0; i < BuildingsHealth.Count; i++) 
		{
			BuildingsHealth[i]=life ;		
			CreateorDestroyStarServerRpc(i,b);//i is a list index
		}
	}
	[ServerRpc]
	private bool ExistingBuildingStarServerRpc(int listIndex, bool b)//make sure the star doesn't already exist / destroy if required
	{
		//print ("check star exists ");

		bool starExists = false;

		GameObject[] buildingStarType = GameObject.FindGameObjectsWithTag("BuildingStar");

		foreach (GameObject buildingStar in buildingStarType) 
		{
			if(((Selector)buildingStar.GetComponent("Selector")).index == listIndex)	//listIndex	
			{
				if (b) 
				{			
					starExists = true;
				}			
				else	
				{
					Destroy (buildingStar);
				}
				break;
			}
		}

		return starExists;
	}
	[ServerRpc]
	private int GetStructureIndexServerRpc(int listIndex)
	{
		int structureIndex = Buildings[listIndex].GetComponent<StructureSelector>().structureIndex;
		return structureIndex;
	}
	[ServerRpc]
	private void CreateBuildingStarServerRpc(int listIndex)
	{
		Vector3 pos = Buildings[listIndex].transform.position;

		GameObject BuildingStar = (GameObject)Instantiate(buildingStar, new Vector3(pos.x,pos.y-230,starZ), Quaternion.identity);
		if (int.Parse(Helper.chainId) != 4000)
		{
			ServerRpc.SpawnObjectServerRpc(BuildingStar);
		}
		else
		{
			ServerRpc.removeNetworkObject(BuildingStar);
		}
		ProcessBuildingStarServerRpc(BuildingStar, listIndex);// index
	}
	[ServerRpc]
	private void ProcessBuildingStarServerRpc(GameObject BuildingStar, int listIndex)
	{	
		BuildingStar.transform.parent = EffectsGroup.transform;
		((Selector)BuildingStar.GetComponent<Selector>()).index = listIndex; 
	}
	[ServerRpc]
	private void CreateorDestroyStarServerRpc(int listIndex, bool b)
	{		
		if (b) 
		{				
			if (!ExistingBuildingStarServerRpc (listIndex, b)) 
			{				
				CreateBuildingStarServerRpc (listIndex);
			}
		} 
		else 
		{			
			ExistingBuildingStarServerRpc (listIndex, b);
		}
	}
	[ServerRpc]
	private void SelectedIndestructibleServerRpc(bool b)
	{
		//can also be thwarted by Helios; when a building is selected, 
		//the nearest unit group becomes selectedgroup (by index)
		//and his targetStructureIndex receives the Building list index of this building 
		//the structureIndex may differ from Building list index - the user built and destroyed buildings
		//which makes it necessary to translate it like this: GetListIndexServerRpc(targetStructureIndex [k])
		
		int life = 100;
		if(b) life = 1000000;
		//to work independently of units deployed and targets
		int listIndex = GetListIndexServerRpc(selectedStructureIndex);

		CreateorDestroyStarServerRpc(listIndex,b);

		BuildingsHealth[listIndex] = life;
	}
	[ServerRpc]
	private void AllStopServerRpc()
	{
		allStop = !allStop;
		if(allStop)
		{
			for (int i = 0; i < DeployedUnits.Count; i++) 
			{
				if(DeployedUnits[i]!=null)
				DeployedUnits[i].GetComponent<FighterController>().RevertToIdle();
			}
		}
		else
			for (int i = 0; i < DeployedUnits.Count; i++) 
			{
				if(DeployedUnits[i]!=null)
				DeployedUnits[i].GetComponent<FighterController>().RevertToPathWalk();
			}
	}




	private void UpdateTarget(int index)
	{	
		if (instantiationGroupIndex == -1)
						return;

		updatePathMaster = false; //stop update sequence for all
		StopCoroutine ("ResumeUpdateUnits"); //stop resumeupdate in case it was called

		List<GameObject> currentList = new List<GameObject> ();
		currentList.Clear ();

		UpdateGroupPosition (index);

		switch (index) 
		{
		case 0:
			if(GroupO.Count>0)
			{
				Talk("Squad 1 moving to objective.");
				currentList= GroupO;
				updatePathO = true;
				updateOCounter = 0;
			}
			break;
		case 1:
			if(GroupI.Count>0)
			{
				Talk("Squad 2 moving to objective.");
				currentList= GroupI;
				updatePathI = true;
				updateICounter = 0;
			}
			break;
		case 2:
			if(GroupII.Count>0)
			{
				Talk("Squad 3 moving to objective.");
				currentList= GroupII;
				updatePathII = true;
				updateIICounter = 0;
			}
			break;
		case 3:
			if(GroupIII.Count>0)
			{
				Talk("Squad 4 moving to objective.");
				currentList= GroupIII;
				updatePathIII = true;
				updateIIICounter = 0;
			}
			break;
		}

		ResetSurroundIndex (); //this is used to suround a building with units

		for (int i = 0; i < currentList.Count; i++)
		{
			currentList[i].GetComponent<FighterController>().targetCenter = new Vector3(targetCenter[index].x,targetCenter[index].y,targetZ);//selectedGroupIndex
			currentList[i].GetComponent<FighterController>().RevertToIdle();
		}

		StartCoroutine ("ResumeUpdateUnits");
		elapsedTime = 0.0f;
		pauseAttack[index] = false;
	}
	
	private IEnumerator ResumeUpdateUnits()
	{
		yield return new WaitForSeconds(0.3f);
		updatePathMaster = true;
	}
	
	private void DisablePanels()
	{
		((Relay)relay).pauseInput = true;
	}
	
	public void Retreat()
	{
		Talk ("Retreat order received.");
		StartCoroutine ("MissionComplete");
	}
	
	IEnumerator MissionComplete()	
	{
		Talk ("Well done. Return to base.");
		battleOver = true;
		DisablePanels ();
		yield return new WaitForSeconds(0.5f);

		for (int i = 0; i < DeployedUnits.Count; i++) 
		{
			if(DeployedUnits[i]!=null)
			{
				DeployedUnits[i].GetComponent<FighterController>().RevertToIdle();
			}
		}
		ActivateEndGame ();
	}
	
	private void ActivateEndGame()
	{
		MissionCompletePanel.SetActive (true);
		
		goldNoLb.text = ((StatsBattle)statsBattle).gold.ToString();
		manaNoLb.text = ((StatsBattle)statsBattle).mana.ToString();
		buildingsDestroyedNoLb.text = ((StatsBattle)statsBattle).buildingsDestroyed.ToString();
		unitsLostNoLb.text = ((StatsBattle)statsBattle).unitsLost.ToString();

		int remainingUnits = 0;


		for (int i = 0; i < ((StatsBattle)statsBattle).AvailableUnits.Count; i++) 
		{
			remainingUnits += ((StatsBattle)statsBattle).AvailableUnits[i].count;
			((TransData)transData).ReturnedFromBattleUnits[i] = ((TransData)transData).GoingToBattleUnits[i];
		}

		unitsRecoveredNoLb.text = remainingUnits.ToString();

		((SaveLoadBattle)saveLoadBattle).SaveAttack();

		((TransData)transData).goldGained = (int)((StatsBattle)statsBattle).gold;
		((TransData)transData).manaGained = (int)((StatsBattle)statsBattle).mana;
		((TransData)transData).battleOver = true;//this variable is checked at game.unity load to see if the user is returning from battle or just started the game
		((TransData)transData).tutorialBattleSeen = ((StatsBattle)statsBattle).tutorialBattleSeen;

	}


	//          CHEATS             \\

	//Developer section - cheats for testing
	
	private void AllUnitStars(bool b)
	{
		if (instantiationUnitIndex == -1) return;
		for (int i = 0; i <= instantiationGroupIndex; i++) //
		{
			unitStar[i].SetActive(b);
		}
	}
	
	private void SelectedUnitStar(bool b)
	{
		unitStar[selectedGroupIndex].SetActive(b);
	}
	
	private void AllUnitsImmortal(bool b)
	{
		int life = 100;
		if(b) life = 1000000;

		for (int i = 0; i < DeployedUnits.Count; i++) 
		{
			if(DeployedUnits[i]!=null)
			DeployedUnits[i].GetComponent<FighterController>().Life = life;
		}
		AllUnitStars (b);
	}
	
	private void SelectedImmortal(bool b)//Helios may selects another group
	{
		int life = 100;
		if(b) life = 1000000;
		SelectedUnitStar(b);

		switch (selectedGroupIndex) 
		{
		case 0:
			for (int i = 0; i < GroupO.Count; i++) 
			{	
				if(GroupO[i]!=null)
				GroupO[i].GetComponent<FighterController>().Life = life;
			}
			break;
		case 1:
			for (int i = 0; i < GroupI.Count; i++) 
			{
				if(GroupI[i]!=null)
				GroupI[i].GetComponent<FighterController>().Life = life;
			}
			break;
		case 2:
			for (int i = 0; i < GroupII.Count; i++) 
			{
				if(GroupII[i]!=null)
				GroupII[i].GetComponent<FighterController>().Life = life;
			}
			break;
		case 3:
			for (int i = 0; i < GroupIII.Count; i++) 
			{
				if(GroupIII[i]!=null)
				GroupIII[i].GetComponent<FighterController>().Life = life;
			}
			break;
		}
	}
	
	private void AllBuildingsIndestructible(bool b)
	{
		int life = 100;
		if(b) life = 1000000;

		for (int i = 0; i < BuildingsHealth.Count; i++) 
		{
			BuildingsHealth[i]=life ;		
			CreateorDestroyStar(i,b);//i is a list index
		}
	}
	
	private bool ExistingBuildingStar(int listIndex, bool b)//make sure the star doesn't already exist / destroy if required
	{
		//print ("check star exists ");

		bool starExists = false;

		GameObject[] buildingStarType = GameObject.FindGameObjectsWithTag("BuildingStar");

		foreach (GameObject buildingStar in buildingStarType) 
		{
			if(((Selector)buildingStar.GetComponent("Selector")).index == listIndex)	//listIndex	
			{
				if (b) 
				{			
					starExists = true;
				}			
				else	
				{
					Destroy (buildingStar);
				}
				break;
			}
		}

		return starExists;
	}
	
	private int GetStructureIndex(int listIndex)
	{
		int structureIndex = Buildings[listIndex].GetComponent<StructureSelector>().structureIndex;
		return structureIndex;
	}
	
	private void CreateBuildingStar(int listIndex)
	{
		Vector3 pos = Buildings[listIndex].transform.position;

		GameObject BuildingStar = (GameObject)Instantiate(buildingStar, new Vector3(pos.x,pos.y-230,starZ), Quaternion.identity);

		if (int.Parse(Helper.chainId) != 4000)
		{
			ServerRpc.SpawnObjectServerRpc(BuildingStar);
		}
		else
		{
			ServerRpc.removeNetworkObject(BuildingStar);
		}

		ProcessBuildingStar(BuildingStar, listIndex);// index
	}
	
	private void ProcessBuildingStar(GameObject BuildingStar, int listIndex)
	{	
		BuildingStar.transform.parent = EffectsGroup.transform;
		((Selector)BuildingStar.GetComponent<Selector>()).index = listIndex; 
	}
	
	private void CreateorDestroyStar(int listIndex, bool b)
	{		
		if (b) 
		{				
			if (!ExistingBuildingStar (listIndex, b)) 
			{				
				CreateBuildingStar (listIndex);
			}
		} 
		else 
		{			
			ExistingBuildingStar (listIndex, b);
		}
	}
	
	private void SelectedIndestructible(bool b)
	{
		//can also be thwarted by Helios; when a building is selected, 
		//the nearest unit group becomes selectedgroup (by index)
		//and his targetStructureIndex receives the Building list index of this building 
		//the structureIndex may differ from Building list index - the user built and destroyed buildings
		//which makes it necessary to translate it like this: GetListIndex(targetStructureIndex [k])
		
		int life = 100;
		if(b) life = 1000000;
		//to work independently of units deployed and targets
		int listIndex = GetListIndex(selectedStructureIndex);

		CreateorDestroyStar(listIndex,b);

		BuildingsHealth[listIndex] = life;
	}
	
	private void AllStop()
	{
		allStop = !allStop;
		if(allStop)
		{
			for (int i = 0; i < DeployedUnits.Count; i++) 
			{
				if(DeployedUnits[i]!=null)
				DeployedUnits[i].GetComponent<FighterController>().RevertToIdle();
			}
		}
		else
			for (int i = 0; i < DeployedUnits.Count; i++) 
			{
				if(DeployedUnits[i]!=null)
				DeployedUnits[i].GetComponent<FighterController>().RevertToPathWalk();
			}
	}







	bool 
		allImmortalb, 
		allIndestructibleb,
		allStop;

	string 
		allImmortalText,
		allIndestructibleText,
		allStopText;

	//void OnGUI()
	//{
	//	if (battleOver)
	//					return;
	//	if (! allImmortalb)
	//		allImmortalText = "All Units\nImmortal";
	//	else
	//		allImmortalText = "All Units\nMortal";

	//	if (! allIndestructibleb)
	//		allIndestructibleText = "All Buildings\nIndestructible";
	//	else
	//		allIndestructibleText = "All Buildings\nDestructible";

	//	if(allStop)
	//		allStopText="All Units\nMove";
	//	else
	//		allStopText="All Units\nStop";

	//	if(GUI.Button(new Rect(5,5,65,30),allStopText))
	//	{
	//		Delay ();
	//		AllStopServerRpc();
	//	}

	//	if(GUI.Button(new Rect(75,5,65,30), allImmortalText))//Screen.width,Screen.height
	//	{
	//		Delay ();
	//		allImmortalb = !allImmortalb;
	//		AllUnitsImmortalServerRpc(allImmortalb);
	//	}

	//	if(GUI.Button(new Rect(145,5,65,30),"Selected\nImmortal"))
	//	{
	//		Delay ();
	//		if (int.Parse(Helper.chainId) == 4000)
	//		{
	//			SelectedImmortal(true);
	//		}
	//		else
	//		{
	//			SelectedImmortalServerRpc(true);
	//		}
	//	}
		
	//	if(GUI.Button(new Rect(215,5,65,30),"Selected\nMortal"))
	//	{
	//		Delay ();
	//		if (int.Parse(Helper.chainId) == 4000)
	//		{
	//			SelectedImmortal(false);
	//		}
 //           else { 
	//			SelectedImmortalServerRpc(false);
	//		}
	//	}

	//	// For now, the cheats create an issue with buildings that become stuck indestructible
	//	// The cheats feature will be fixed in our next release


	//	if(GUI.Button(new Rect(285,5,90,30),allIndestructibleText))
	//	{
	//		Delay ();
	//		allIndestructibleb=!allIndestructibleb;
	//		AllBuildingsIndestructibleServerRpc(allIndestructibleb);
	//	}

	//	if(GUI.Button(new Rect(380,5,90,30),"Selected\nIndestructible"))
	//	{
	//		Delay ();
	//		SelectedIndestructibleServerRpc(true);
	//	}
	//	if(GUI.Button(new Rect(475,5,85,30),"Selected\nDestructible"))
	//	{
	//		Delay ();
	//		SelectedIndestructibleServerRpc(false);
	//	}
	//}
}
