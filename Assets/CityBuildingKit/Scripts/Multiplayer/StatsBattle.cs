using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UIControllersAndData.Units;
using UnityEngine.UI;
using Unity.Netcode;

public class StatsBattle : MonoBehaviour {

	private const int noOfUnits = 16;//correlate with MenuUnitBase.cs/ verify that the Inspector has 12 elements

	private List<ExistedUnit> _deployedUnits = Enumerable.Range(1, 16).Select(i => new ExistedUnit()).ToList();

	public List<ExistedUnit> DeployedUnits
	{
		get => _deployedUnits;
		set => _deployedUnits = value;
	}

	private List<ExistedUnit> _availableUnits = Enumerable.Range(1, 16).Select(i => new ExistedUnit()).ToList();

	public List<ExistedUnit> AvailableUnits
	{
		get => _availableUnits;
		set => _availableUnits = value;
	}


	public bool tutorialBattleSeen = false; 

	public Slider goldBar, manaBar, crystalsBar;
	public Text goldLb, manaLb, crystalsLb, remainingUnitsNo;
	public Toggle soundToggle,ambientToggle,musicToggle;

	public float 
		gold = 0, 
		mana = 0;//increasing as map is attacked

	public int 
		crystals = 0, 
		unitsLost = 0, 
		buildingsDestroyed = 0,
		maxStorageGold = 0, //maximum loot existing on the map - necessary for progress bars
		maxStorageMana = 0, 
		maxCrystals = 0; 

	public GameObject GhostHelperBattle;
	private Component transData, soundFX;

	void Start () 
	{
		
		
		transData = GameObject.Find("TransData").GetComponent<TransData>();
		soundFX = GameObject.Find("SoundFX").GetComponent<SoundFX>();

		if (int.Parse(Helper.chainId) == 4000)
		{
			LoadTransData();
        }
        else
        {
			LoadTransDataServerRpc();
		}

		if (int.Parse(Helper.chainId) == 4000)
		{
			UpdateUI();
		}
        else{ 

			UpdateUIServerRpc();
		}
	}
	[ServerRpc]
	private void LoadTransDataServerRpc()
	{
		tutorialBattleSeen = ((TransData)transData).tutorialBattleSeen;
		//if (!tutorialBattleSeen) GhostHelperBattle.SetActive (true);

		AvailableUnits = ((TransData)transData).GoingToBattleUnits;



		if (!((TransData)transData).soundOn) 
		{			
			((SoundFX)soundFX).ChangeSound (((TransData)transData).soundOn);
//			soundToggle.Start ();
//			soundToggle.Set (false, false);
		}

		if (!((TransData)transData).ambientOn) 
		{
			((SoundFX)soundFX).ToggleAmbient ();
//			ambientToggle.Start ();
//			ambientToggle.Set (false, false);
		}

		if (!((TransData)transData).musicOn) 
		{
			((SoundFX)soundFX).ChangeMusic (((TransData)transData).musicOn);
//			musicToggle.Start ();
//			musicToggle.Set (false, false);
		}

	}
	[ServerRpc]
	public void ApplyMaxCapsServerRpc()//cannot exceed storage+bought capacity
	{
		if (gold > maxStorageGold) { gold = maxStorageGold; }
		if (mana > maxStorageMana) { mana = maxStorageMana; }
	}
	[ServerRpc]
	public void UpdateUIServerRpc()//updates numbers and progress bars
	{
		goldBar.maxValue = (float)maxStorageGold;
		goldBar.value = (float)gold/(float)maxStorageGold;
		manaBar.maxValue = (float)maxStorageMana;
		manaBar.value = (float)mana/(float)maxStorageMana;
		//crystalsBar.maxValue = (float)maxCrystals;
		//crystalsBar.value = (float)crystals/(float)maxCrystals;
			
		goldLb.text = ((int)gold).ToString ();
		manaLb.text = ((int)mana).ToString ();
		//crystalsLb.text = crystals.ToString ();
	}
	[ServerRpc]
	public void UpdateUnitsNoServerRpc()
	{		
		int remainingUnits = 0;
		for (int i = 0; i <  _availableUnits.Count; i++) 
		{
			remainingUnits += _availableUnits[i].count;
		}		
		remainingUnitsNo.text = remainingUnits.ToString ();// update remaining units
	}
	[ServerRpc]
	public void ReturnHomeServerRpc()
	{
		Application.LoadLevel ("Game");
	}


		
	private void LoadTransData()
	{
		tutorialBattleSeen = ((TransData)transData).tutorialBattleSeen;
		//if (!tutorialBattleSeen) GhostHelperBattle.SetActive (true);

		AvailableUnits = ((TransData)transData).GoingToBattleUnits;



		if (!((TransData)transData).soundOn) 
		{			
			((SoundFX)soundFX).ChangeSound (((TransData)transData).soundOn);
//			soundToggle.Start ();
//			soundToggle.Set (false, false);
		}

		if (!((TransData)transData).ambientOn) 
		{
			((SoundFX)soundFX).ToggleAmbient ();
//			ambientToggle.Start ();
//			ambientToggle.Set (false, false);
		}

		if (!((TransData)transData).musicOn) 
		{
			((SoundFX)soundFX).ChangeMusic (((TransData)transData).musicOn);
//			musicToggle.Start ();
//			musicToggle.Set (false, false);
		}

	}
	
	public void ApplyMaxCaps()//cannot exceed storage+bought capacity
	{
		if (gold > maxStorageGold) { gold = maxStorageGold; }
		if (mana > maxStorageMana) { mana = maxStorageMana; }
	}
	
	public void UpdateUI()//updates numbers and progress bars
	{
		goldBar.maxValue = (float)maxStorageGold;
		goldBar.value = (float)gold/(float)maxStorageGold;
		manaBar.maxValue = (float)maxStorageMana;
		manaBar.value = (float)mana/(float)maxStorageMana;
		//crystalsBar.maxValue = (float)maxCrystals;
		//crystalsBar.value = (float)crystals/(float)maxCrystals;
			
		goldLb.text = ((int)gold).ToString ();
		manaLb.text = ((int)mana).ToString ();
		//crystalsLb.text = crystals.ToString ();
	}
	
	public void UpdateUnitsNo()
	{		
		int remainingUnits = 0;
		for (int i = 0; i <  _availableUnits.Count; i++) 
		{
			remainingUnits += _availableUnits[i].count;
		}		
		remainingUnitsNo.text = remainingUnits.ToString ();// update remaining units
	}
	
	public void ReturnHome()
	{
		if (int.Parse(Helper.chainId) == 4000)
		{
			Application.LoadLevel("Game");
        }
        else
        {
			ReturnHomeServerRpc();

		}
	}

}
