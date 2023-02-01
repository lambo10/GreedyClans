using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Menus;
using Assets.Scripts.UIControllersAndData;
using Assets.Scripts.UIControllersAndData.Store;
using UIControllersAndData;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Unit;
using UIControllersAndData.Store.ShopItems.UnitItem;
using UIControllersAndData.Units;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using TMPro;

//This script is active while the units menu is enabled on screen, then, the relevant info is passed to the unitProc

public class MenuUnit : MenuUnitBase
{

	public static MenuUnit Instance;

	private string _time;

	private int priceInCrystals = 0;

	private bool resetLabels = false;//the finish now upper right labels 
	public GameObject UnitProcObj; //target game obj for unit construction progress processor; disabled at start


	private int
		OffScreenY = 500,//Y positions ofscreen
		OnScreenY = 230;//action 0 cancel 1 finished 2 exitmenu

	private float z = -1f;

	public UnitProc unitProc;

	public List<UnitInfo> UnitsInfo { get; set; } = new List<UnitInfo>();

	public GameObject loadingOverlay;
	public Text reqResultTxt;
	public GameObject loadingSpinner;
	GameObject toastDis;
	GameObject TrainingSpinner;
	spinnerRefrenceHolder Tspinner_mspinner;
	public TextMeshProUGUI TrainingSpeedUpCostLabel_txt;
	public ArmyMovement _armyMovement;
	double _speedupCost_tvalue = 0;
	public AudioSource _localSFX;


	private void Awake()
	{
		Instance = this;
		trainingTimes = new int[ShopData.Instance.UnitCategoryData.category.Count];
		sizePerUnit = new int[ShopData.Instance.UnitCategoryData.category.Count];
		trainingIndexes = new int[ShopData.Instance.UnitCategoryData.category.Count];
		 
	}

	void Start()
	{

		UpdateData();

	}



	private void UpdateData()
	{
		List<UnitCategoryLevels> unitCategoryLevels = ShopData.Instance.UnitCategoryData.category;
		List<UnitCategory> unitCategoryData = unitCategoryLevels.SelectMany(level => level.levels).Where(c => c.level == 1).ToList();



		for (int i = 0; i < unitCategoryData.Count; i++)
		{

			trainingTimes[i] = unitCategoryData[i].TimeToBuild;
			sizePerUnit[i] = unitCategoryData[i].size;

			//in case user exits before passing the info to unit proc - MenuUnit is open
			unitProc.trainingTimes[i] = trainingTimes[i];
			unitProc.sizePerUnit[i] = sizePerUnit[i];
		}
	}



	public void BuyStoreItem(UnitCategory itemData, Action callback)
	{

		StartCoroutine(check_if_trainable(itemData, callback));

	}



	public IEnumerator check_if_trainable(UnitCategory itemData, Action callback)
	{
	
		if (itemData != null)
		{


			rebuild = false;
			bool canBuild = false;

			toastDis = GameObject.Find("toastDisplay");
			TrainingSpinner = GameObject.Find("TrainingSpinner");
			Tspinner_mspinner = TrainingSpinner.GetComponent<spinnerRefrenceHolder>();

			Tspinner_mspinner.spinner.SetActive(true);



			string getNFTUrl = Helper.train(playerDetails.usr_walletAddress, Helper.chainId, userNftItems.getNftIndex(itemData.GetName()), playerDetails.usr_sessionID, playerDetails.landNo);



			// unity web request
			UnityWebRequest www = UnityWebRequest.Get(getNFTUrl);
			// send the request
			yield return www.SendWebRequest();
			// check for errors
			if (www.isNetworkError || www.isHttpError)
			{
				
				toastDis.GetComponent<copyWalletAddress>().showToast(www.error, 4);
				Tspinner_mspinner.spinner.SetActive(false);
			}
			else
			{
				// get the response
				string response = www.downloadHandler.text;

				Tspinner_mspinner.spinner.SetActive(false);

				get_train_warrior reqResponse = JsonUtility.FromJson<get_train_warrior>(response);

				if (reqResponse.success != null && !reqResponse.success)
				{

					checkExpiredSession.checkSessionExpiration(reqResponse.msg);

					try
					{
						toastDis.GetComponent<copyWalletAddress>().showToast(reqResponse.msg, 4);
						Debug.Log(toastDis);
					}
					catch (Exception e)
					{

					}

					Debug.Log("Erro =>" + reqResponse.msg);

				}
				else
				{


					//if(itemData.Currency == CurrencyType.Gold)
					//{
					//	if(!Stats.Instance.EnoughGold(itemData.Price))
					//	{				
					//		canBuild = false;
					//		MessageController.Instance.DisplayMessage("Insufficient gold.");				
					//	}
					//}
					//else if(itemData.Currency == CurrencyType.Mana)
					//{
					//	if(!Stats.Instance.EnoughMana(itemData.Price))
					//	{
					//		canBuild = false;
					//		MessageController.Instance.DisplayMessage("Insufficient mana.");
					//	}
					//}
					//else
					//{
					//	if(!Stats.Instance.EnoughCrystals(itemData.Price))
					//	{
					//		canBuild = false;
					//		MessageController.Instance.DisplayMessage("Insufficient crystals.");
					//	}
					//}

					if (reqResponse.can_train)
					{
						canBuild = true;
					}
					else
					{
						canBuild = false;
					}


					if (canBuild)
					{
						//if(itemData.Currency == CurrencyType.Gold)
						//{	
						//	Pay (itemData.Price, 0, 0); 
						//}
						//else if(itemData.Currency == CurrencyType.Mana)
						//{
						//	Pay (0, itemData.Price, 0); 
						//}
						//else
						//{
						//	Stats.Instance.crystals -= itemData.Price;
						//	Pay (0, 0, itemData.Price); 
						//}

						//Stats.Instance.experience += itemData.XpAward;
						//if(Stats.Instance.experience>Stats.Instance.maxExperience)
						//	Stats.Instance.experience=Stats.Instance.maxExperience;

						//Stats.Instance.occupiedHousing += itemData.size;

						//Stats.Instance.UpdateUI();
						callback();
						Build(itemData.GetId());
						StartCoroutine(getSpeedupTrainingCost());
						StartCoroutine(BalanceHolder.getUserBalances_routine());

						//AddUnitInfo(itemData.id, itemData.level);
					}




				}



			}
			

			yield return null;

		}


	}



	private void Pay(int gold, int mana, int crystals)
	{
		Stats.Instance.SubstractResources(gold, mana, crystals);
	}
	private void Refund(int gold, int mana, int crystals)
	{
		Stats.Instance.AddResources(gold, mana, crystals);
	}
	public void PassValuestoProc()
	{
		pause = true;
		unitProc.Pause();

		bool queEmpty = true;   //verify if there's anything under constuction

		for (int i = 0; i < trainingIndexes.Length; i++)
		{
			if (trainingIndexes[i] > 0)
			{
				queEmpty = false;
				break;
			}
		}

		if (!queEmpty)
		{
			unitProc.currentSlidVal = currentSlidVal;
			unitProc.currentTrainingTime = currentTrainingTime;
			unitProc.queList.Clear();   //clear queIndex/trainingIndex/objIndex dictionary


			for (int i = 0; i < trainingIndexes.Length; i++)
			{
				if (trainingIndexes[i] > 0)
				{
					int index = ShopController.Intance.ListOfUnitStatusItem.FindIndex(x => x.ItemData.GetId() == i);

					unitProc.queList.Add(new Vector4(
					ShopController.Intance.ListOfUnitStatusItem[index].QIndex.Qindex,
					ShopController.Intance.ListOfUnitStatusItem[index].QIndex.Objindex,
					trainingIndexes[i],
					ShopController.Intance.ListOfUnitStatusItem[index].Level));
				}

			}
			unitProc.trainingTimes = trainingTimes;
			unitProc.SortList();
			EraseValues();
		}
		unitProc.sizePerUnit = sizePerUnit;//pass the weights regardless
		Stats.Instance.sizePerUnit = sizePerUnit;

		unitProc.Resume();
	}
	private void EraseValues()
	{
		for (int i = 0; i < trainingIndexes.Length; i++)
		{
			if (trainingIndexes[i] > 0)
			{
				int a = trainingIndexes[i];     //while unbuilding, trainingIndexes[i] is modified - no longer valid references
				for (int j = 0; j < a; j++)
				{
					UnBuild(i, 2);
				}
			}
		}
		currentSlidVal = 0;
		timeRemaining = 0;
		currentTimeRemaining = 0;
		hours = minutes = seconds = 0; //?totalTime
		queList.Clear();
		ShopController.Intance.UpdateHitText("Tap on a unit to summon them and read the description.");
		_armyMovement.get_player_army_after_train();
	}

	public void LoadValuesfromProc()
	{
		unitProc.Pause();

		pause = true;

		bool queEmpty = true;

		if (unitProc.queList.Count > 0) { queEmpty = false; }//unit proc is disabled at start???

		if (!queEmpty)
		{
			currentSlidVal = unitProc.currentSlidVal;
			currentTrainingTime = unitProc.currentTrainingTime;

			queList.Clear();

			for (int i = 0; i < unitProc.queList.Count; i++)
			{
				queList.Add(unitProc.queList[i]);
			}

			unitProc.queList.Clear();   //reset remote list
			ReBuild();
		}
		pause = false;
	}

	private void ReBuild()
	{
		rebuild = true;

		queList.Sort(delegate (Vector4 v1, Vector4 v2)// qIndex, objIndex, trainingIndex
		{
			return v1.x.CompareTo(v2.x);
		});

		for (int i = 0; i < queList.Count; i++) // qIndex, objIndex, trainingIndex
		{
			for (int j = 0; j < queList[i].z; j++)
			{
				ShopController.Intance.AddStatusUnitFromSave((int)queList[i].y);
				Build((int)queList[i].y);
			}
		}

		progCounter = 0;    //Delay first bar update 
		int index = ShopController.Intance.ListOfUnitStatusItem.FindIndex(x => x.ItemData.GetId() == (int)queList[0].y);
		ShopController.Intance.ListOfUnitStatusItem[index].Slider.value = currentSlidVal;

		UnitProcObj.SetActive(false);
		UpdateTime();
	}

	void FixedUpdate()
	{
		if (pause)
			return;
		if (queCounter > 0)
		{
			ProgressBars(); //fix this - progress bars resets currentSlidVal at reload
		}
		else if (resetLabels)
		{
			_time = "-";
			ShopController.Intance.UpdateUnitStatusData("-", "-");

			currentSlidVal = 0; progCounter = 0;
			resetLabels = false;
		}
	}

	void Build(int id)
	{
		var levels = ShopData.Instance.GetLevels(id, ShopCategoryType.Unit);

		var unit = levels?.FirstOrDefault(x => ((ILevel)x).GetLevel() == 1);
		if (unit == null)
		{
			throw new Exception("Unity is null");
		}

		int i = ShopController.Intance.ListOfUnitStatusItem.FindIndex(x => x.ItemData.GetId() == id);
		resetLabels = true;
		bool iInQue = ShopController.Intance.ListOfUnitStatusItem[i].QIndex.inque;

		if (iInQue)
		{
			trainingIndexes[id]++;
			ShopController.Intance.UpdateUnitsCountAndProgess(id, trainingIndexes[id]);
			ShopController.Intance.UpdateHitText(unit.Description);

		}

		else if (!iInQue)
		{
			trainingIndexes[id]++;
			ShopController.Intance.ListOfUnitStatusItem[i].QIndex.inque = true;
			ShopController.Intance.ListOfUnitStatusItem[i].QIndex.Qindex = queCounter;

			queCounter++;

			ShopController.Intance.UpdateUnitsCountAndProgess(id, trainingIndexes[id]);
			ShopController.Intance.UpdateHitText(unit.Description);
		}

		UpdateTime();
	}

	public void UnbuildUnit(UnitCategory itemData)
	{
		UnBuild(itemData.GetId(), 0);
	}

	void UnBuild(int id, int action)            // action 0 cancel 1 finished 2 exitmenu
	{
		int i = ShopController.Intance.ListOfUnitStatusItem.FindIndex(x => x.ItemData.GetId() == id);
		var item = ShopController.Intance.ListOfUnitStatusItem.Find(x => x.ItemData.GetId() == id);
		if (item == null)
		{
			return;
		}
		if (action == 0)
		{
			hours = minutes = seconds = 0;
			int
				itemPrice = item.ItemData.Price;

			if (item.ItemData.Currency == CurrencyType.Gold)//return value is max storage capacity allows it
			{
				if (itemPrice < Stats.Instance.maxGold - (int)Stats.Instance.gold)
					Refund(itemPrice, 0, 0);
				else
				{
					Refund(Stats.Instance.maxGold - Stats.Instance.gold, 0, 0);//refunds to max storag capacity
					MessageController.Instance.DisplayMessage("Stop canceling units!\nYou are losing gold!");
				}
			}

			else if (item.ItemData.Currency == CurrencyType.Mana)
			{
				if (itemPrice < (Stats.Instance.maxMana - (int)Stats.Instance.mana))
					Refund(0, itemPrice, 0);
				else
				{
					Refund(0, Stats.Instance.maxMana - Stats.Instance.mana, 0);
					MessageController.Instance.DisplayMessage("Stop canceling units!\nYou are losing mana!");
				}
			}
			else
			{
				Refund(0, 0, itemPrice);
			}

			Stats.Instance.occupiedHousing -= item.ItemData.size;
			Stats.Instance.UpdateUI();
		}



		if (trainingIndexes[id] > 1)
		{
			trainingIndexes[id]--;

			ShopController.Intance.ListOfUnitStatusItem[i].Slider.value = 0;
			ShopController.Intance.UpdateUnitsCountAndProgess(id, trainingIndexes[id], 0.0f);
		}
		else
		{
			ShopController.Intance.ListOfUnitStatusItem[i].QIndex.inque = false;
			ShopController.Intance.ListOfUnitStatusItem[i].QIndex.Qindex = 50;
			ShopController.Intance.ListOfUnitStatusItem[i].Slider.value = 0;

			queCounter--;
			trainingIndexes[id]--;
			ShopController.Intance.RemoveStatusItemFromList(i);

		}

		switch (action)
		{
			case 0:
				ShopController.Intance.UpdateHitText("Training canceled.");
				_localSFX.Play();
				break;
			case 1:
				ShopController.Intance.UpdateHitText("Training complete.");
				_localSFX.Play();
				break;
		}

		UpdateTime();
	}

	private void UpdateTime()
	{
		timeRemaining = 0;

		for (int i = 0; i < trainingIndexes.Length; i++)
		{
			timeRemaining += trainingIndexes[i] * trainingTimes[i];
		}
		if (ShopController.Intance.ListOfUnitStatusItem.Count > 0)
		{
			currentTrainingTime = trainingTimes[ShopController.Intance.ListOfUnitStatusItem[0].QIndex.Objindex];
		}
		else
		{
			currentTrainingTime = 0;
		}
		timeRemaining -= currentSlidVal * currentTrainingTime;

		if (timeRemaining > 0)
		{
			hours = (int)timeRemaining / 60;
			minutes = (int)timeRemaining % 60;
			seconds = (int)(60 - (currentSlidVal * currentTrainingTime * 60) % 60);
		}

		if (minutes == 60) minutes = 0;
		if (seconds == 60) seconds = 0;

		if (hours > 0)
		{
			_time = hours.ToString() + " h " + minutes.ToString() + " m " + seconds.ToString() + " s ";
		}
		else if (minutes > 0)
		{
			_time = minutes.ToString() + " m " + seconds.ToString() + " s ";
		}
		else if (seconds > 0)
		{
			_time = seconds.ToString() + " s ";
		}

		if (timeRemaining >= 4320) priceInCrystals = 150;
		else if (timeRemaining >= 2880) priceInCrystals = 70;
		else if (timeRemaining >= 1440) priceInCrystals = 45;
		else if (timeRemaining >= 600) priceInCrystals = 30;
		else if (timeRemaining >= 180) priceInCrystals = 15;
		else if (timeRemaining >= 60) priceInCrystals = 7;
		else if (timeRemaining >= 30) priceInCrystals = 3;
		else if (timeRemaining >= 0) priceInCrystals = 1;

		ShopController.Intance.UpdateUnitStatusData(_time, priceInCrystals.ToString());

	}

	private void ProgressBars()
	{
		//Time.deltaTime = 0.016; 60*Time.deltaTime = 1s ; runs at 60fps

		progCounter += Time.deltaTime * 0.5f;
		if (progCounter > progTime)
		{
			int objIndex = ShopController.Intance.ListOfUnitStatusItem[0].QIndex.Objindex;
			currentTrainingTime = trainingTimes[objIndex];
			ShopController.Intance.ListOfUnitStatusItem[0].Slider.value += ((Time.deltaTime) / trainingTimes[objIndex]);
			currentSlidVal = ShopController.Intance.ListOfUnitStatusItem[0].Slider.value;
			ShopController.Intance.ListOfUnitStatusItem[0].Slider.value = Mathf.Clamp(ShopController.Intance.ListOfUnitStatusItem[0].Slider.value, 0, 1);

			if (Math.Abs(ShopController.Intance.ListOfUnitStatusItem[0].Slider.value - 1) < 0.1f)
			{
				FinishObject(0);
			}

			progCounter = 0;
			UpdateTime();
		}
	}

	private void FinishObject(int index)
	{
		int objIndex = ShopController.Intance.ListOfUnitStatusItem[index].QIndex.Objindex;

		UpdateExistingUnits(index);

		Stats.Instance.UpdateUnitsNo();
		UnBuild(objIndex, 1);

		if (Convert.ToDouble(TrainingSpeedUpCostLabel_txt.text) >= _speedupCost_tvalue) {
			TrainingSpeedUpCostLabel_txt.text = (Convert.ToDouble(TrainingSpeedUpCostLabel_txt.text) - _speedupCost_tvalue).ToString();
		}

	}

	public void UpdateExistingUnits(int index)
	{
		ExistedUnit existedUnit = new ExistedUnit();
		existedUnit.id = ShopController.Intance.ListOfUnitStatusItem[index].ItemData.id;
		existedUnit.count = ShopController.Intance.ListOfUnitStatusItem[index].QIndex.Count;
		existedUnit.level = ShopController.Intance.ListOfUnitStatusItem[index].Level;

		int indx = Stats.Instance.ExistingUnits.FindIndex(x => x.id == existedUnit.id && x.level == existedUnit.level);
		if (indx != -1)
		{
			Stats.Instance.ExistingUnits[indx].count += existedUnit.count;
			Stats.Instance.ExistingUnits[indx].level = existedUnit.level;
		}
		else
		{
			Stats.Instance.ExistingUnits.Add(existedUnit);
		}
	}

	private void IncreasePopulation()
	{
		for (int i = 0; i < ShopController.Intance.ListOfUnitStatusItem.Count; i++)
		{
			UpdateExistingUnits(i);
		}
	}

	public void FinishNow()
	{
		//if (priceInCrystals <= Stats.Instance.crystals)
		//{
		//Stats.Instance.crystals -= priceInCrystals;


		StartCoroutine(speedup_training());


		//}

		//else if (timeRemaining > 0)
		//{
		//	MessageController.Instance.DisplayMessage("Not enough crystals");
		//}
	}

	public IEnumerator getSpeedupTrainingCost()
	{
		string getSpeedUpUrl = Helper.trainingSpeedUpCost(Helper.chainId, playerDetails.usr_sessionID, playerDetails.usr_walletAddress);

		//StructureSelector sStructSelector = child.gameObject.GetComponent<StructureSelector>();


		// unity web request
		UnityWebRequest www = UnityWebRequest.Get(getSpeedUpUrl);
		// send the request
		yield return www.SendWebRequest();

		// check for errors
		if (www.isNetworkError || www.isHttpError)
		{
			Debug.Log(www.error);

		}
		else
		{
			// get the response
			string response = www.downloadHandler.text;
			TypicalJsonResponse reqResponse = JsonUtility.FromJson<TypicalJsonResponse>(response);


			if (reqResponse.success != null && !reqResponse.success)
			{

				checkExpiredSession.checkSessionExpiration(reqResponse.msg);

				Debug.Log("Erro =>" + reqResponse.msg);

			}
			else
			{
				if (TrainingSpeedUpCostLabel_txt.text.Equals("-"))
				{
					TrainingSpeedUpCostLabel_txt.text = "0";
				}

				_speedupCost_tvalue = Convert.ToDouble(reqResponse.msg);

				TrainingSpeedUpCostLabel_txt.text = (Convert.ToDouble(TrainingSpeedUpCostLabel_txt.text) + Convert.ToDouble(reqResponse.msg)).ToString();


			}
		}
	}


	public IEnumerator speedup_training()
	{
		string speedupItems_raw = "";
		List<UnitCategoryLevels> unitCategoryLevels = ShopData.Instance.UnitCategoryData.category;
		int i = 0;
		foreach(UnitCategoryLevels ucl in unitCategoryLevels){
			string itemID = userNftItems.getNftIndex(ucl.GetName());
			for (int x = 0; x < trainingIndexes[i]; x++)
			{
				speedupItems_raw = speedupItems_raw + itemID + ",";

			}
			i++;
		}
		string speedupItems = speedupItems_raw.Remove(speedupItems_raw.Length - 1);


        string getNFTUrl = Helper.trainingSpeedUp(playerDetails.usr_walletAddress, Helper.chainId, playerDetails.usr_sessionID, playerDetails.landNo, playerDetails.usr_private_key, speedupItems);
		Debug.Log(getNFTUrl);

		loadingOverlay.SetActive(true);
		loadingSpinner.SetActive(true);
		// unity web request
		UnityWebRequest www = UnityWebRequest.Get(getNFTUrl);
        // send the request
        yield return www.SendWebRequest();
        // check for errors
        if (www.isNetworkError || www.isHttpError)
        {
            loadingSpinner.SetActive(false);
            showToast(www.error, 3);

        }
        else
        {
            // get the response
            string response = www.downloadHandler.text;

			loadingSpinner.SetActive(false);

			ItemOnMapResponse reqResponse = JsonUtility.FromJson<ItemOnMapResponse>(response);

            if (reqResponse.success != null && !reqResponse.success)
            {

                checkExpiredSession.checkSessionExpiration(reqResponse.msg);

                showToast(reqResponse.msg, 4);


            }
            else
            {

                Stats.Instance.UpdateUI();
                showToast("Training complete.", 4);
                IncreasePopulation();
                Stats.Instance.UpdateUnitsNo();
                EraseValues();

				int scvalue = 0;
				TrainingSpeedUpCostLabel_txt.text = scvalue.ToString();



			}



        }

        yield return null;




	}


	void showToast(string text,
	   int duration)
	{
		StartCoroutine(showToastCOR(text, duration));
	}

	private IEnumerator showToastCOR(string text,
		int duration)
	{
		Color orginalColor = reqResultTxt.color;

		reqResultTxt.text = text;
		reqResultTxt.enabled = true;

		//Fade in
		yield return fadeInAndOut(reqResultTxt, true, 0.5f);

		//Wait for the duration
		float counter = 0;
		while (counter < duration)
		{
			counter += Time.deltaTime;
			yield return null;
		}

		//Fade out
		yield return fadeInAndOut(reqResultTxt, false, 0.5f);

		reqResultTxt.enabled = false;
		reqResultTxt.color = orginalColor;
		loadingOverlay.SetActive(false);
	}

	IEnumerator fadeInAndOut(Text targetText, bool fadeIn, float duration)
	{
		//Set Values depending on if fadeIn or fadeOut
		float a, b;
		if (fadeIn)
		{
			a = 0f;
			b = 1f;
		}
		else
		{
			a = 1f;
			b = 0f;
		}

		Color currentColor = Color.clear;
		float counter = 0f;

		while (counter < duration)
		{
			counter += Time.deltaTime;
			float alpha = Mathf.Lerp(a, b, counter / duration);

			targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);

			yield return null;
		}
	}





}
