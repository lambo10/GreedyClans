using Assets.Scripts.Menus;
using Assets.Scripts.UIControllersAndData.Store;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class ConstructionSelector : MonoBehaviour
{       //controls the behaviour of a "building under construction", from placement to completion

	/*
		<ProdType>None</ProdType>					<!-- resource produced - gold/mana/none-->	
		<ProdPerHour>0</ProdPerHour>				<!-- the amount of the resource generated per hour -->			
		
		<StoreType>None</StoreType>					<!-- resource stored - none/gold/mana/dual/soldiers-->	
		<StoreCap>0</StoreCap>						<!-- gold/mana/dual/soldiers storage -->
	*/

	private int _id = -1;
	private int _level = -1;
	private ShopCategoryType _categoryType = ShopCategoryType.None;

	public ShopCategoryType CategoryType
	{
		get => _categoryType;
		set => _categoryType = value;
	}
	public GameObject loadingOverlay, loadingSpinner;

	public int Level
	{
		get => _level;
		set => _level = value;
	}
	public Text txt;
	public int Id
	{
		get => _id;
		set => _id = value;
	}

	private bool inConstruction = true;

	public bool
		finishedOffLine,                                //for offline production then load
		isProductionBuilding,
		isSelected = true,                              //for initial processing, right after the construction is instantiated
		battleMap;                              //flag - some components only exist in hometown/battlemap

	//private bool 
	//isProductionBuilding = false;

	public float progTime = 0.57f, progCounter;     //for progress timer, one minute

	public int
		elapsedTime,                                    //for offline production then load
		iRow, jCol,
		buildingTime = 1,
		remainingTime = 1,
		//price displayed for "finish now" button. based on remaining time
		storageAdd,                                     //passes maxStorage to stats
		xpAdd,
		constructionIndex = -1,                         //unique ID for constructions
		grassType;

	double priceno = 0.05;

	private int hours, minutes, seconds;                //for time remaining label
	public Text TimeCounterLb;

	[SerializeField]
	private Text Price;                                     //own child obj - has the price label
	[SerializeField]
	private Slider ProgressBar;                                 //own child obj

	public GameObject ParentGroup;                  //to parent the building after it's finished

	private GameObject[] selectedGrassType;

	[FormerlySerializedAs("StructureType")]
	[FormerlySerializedAs("structureType")]
	[SerializeField]
	private string _structureType;          //Toolhouse, Cannon, ArcherTower, etc 

	public string StructureType
	{
		get => _structureType;
		set => _structureType = value;
	}

	public string structureClass;           //Building,Wall,Weapon,Ambient


	private Component soundFX, relay, stats;//, resourceGenerator;

	public string buildingName;

	void Start()
	{
		//GroupBuildings = GameObject.Find("GroupBuildings");

		relay = GameObject.Find("Relay").GetComponent<Relay>();
		soundFX = GameObject.Find("SoundFX").GetComponent<SoundFX>();



		Debug.Log(loadingOverlay + "this is overlay");

		if (!battleMap)
		{
			//resourceGenerator = GameObject.Find("ResourceGenerator").GetComponent<ResourceGenerator>();
			stats = GameObject.Find("Stats").GetComponent<Stats>();
		}

		//init price so user can't click fast on price 0
		//also adjusts by one minute to avoid starting in the next upper interval - visible at 30 or 60 minutes for example

		//remainingTime = buildingTime * (1 - (int)((UISlider)ProgressBar.GetComponent("UISlider")).value);

		remainingTime = buildingTime - 1;
		//UpdatePrice(remainingTime);
		StartCoroutine(SpeedUp());

	}

	public void DetermineParentGroup()
	{
		switch (structureClass) //Building,Wall,Weapon,Ambient
		{

			case "Building":
				ParentGroup = GameObject.Find("GroupBuildings");
				break;
			case "Wall":
				ParentGroup = GameObject.Find("GroupWalls");
				break;
			case "Weapon":
				ParentGroup = GameObject.Find("GroupWeapons");
				break;
			case "Ambient":
				ParentGroup = GameObject.Find("GroupAmbients");
				break;
		}
	}


	void FixedUpdate()
	{
		if (inConstruction)
		{
			ProgressBarUpdate();
		}
	}

	private void ProgressBarUpdate()
	{
		progCounter += Time.deltaTime * 0.5f;
		if (progCounter > progTime)
		{
			progCounter = 0;

			ProgressBar.value += Time.deltaTime / buildingTime;             //update progress bars values

			ProgressBar.value = Mathf.Clamp(ProgressBar.value, 0, 1);

			remainingTime = (int)(buildingTime * (1 - ProgressBar.value));

			//UpdatePrice(remainingTime);
			UpdateTimeCounter(remainingTime);

			if (ProgressBar.value == 1)             //building finished - the progress bar has reached 1												
			{
				((SoundFX)soundFX).BuildingFinished();

				if (!battleMap)                                                         //if this building is not finished on a battle map
				{
					//xml order: Forge Generator Vault Barrel Summon Tatami
					((Stats)stats).occupiedDobbits--;                                   //the dobbit previously assigned becomes available

					if (_structureType == "Toolhouse")                                      //increases total storage in Stats																	
					{
						((Stats)stats).dobbits += 1;
					}
					else if (_structureType == "Tatami")                                    //increases total storage in Stats																	
					{
						((Stats)stats).maxHousing += storageAdd;
					}
					else if (_structureType == "Forge")
					{
						isProductionBuilding = true;            //((Stats)stats).maxGold += storageAdd;	
					}
					else if (_structureType == "Generator")
					{
						isProductionBuilding = true;            //((Stats)stats).maxMana += storageAdd;					
					}

					else if (_structureType == "Barrel")                                    //increases total storage in Stats																	
					{
						((Stats)stats).maxMana += storageAdd;
					}
					else if (_structureType == "Vault")
					{
						((Stats)stats).maxGold += storageAdd;
					}

					((Stats)stats).UpdateUI();
				}

				foreach (Transform child in transform)                                  //parenting and destruction of components no longer needed
				{
					if (child.gameObject.CompareTag("Structure"))//structureType
					{
						child.gameObject.SetActive(true);

						StructureSelector structureSelector = child.gameObject.GetComponent<StructureSelector>();
						structureSelector.inConstruction = false;

						structureSelector.Id = Id;
						structureSelector.Level = Level;
						structureSelector.CategoryType = CategoryType;

						if (battleMap)
							structureSelector.battleMap = true;
						else if (isProductionBuilding)
						{
							structureSelector.structureType = _structureType;
							structureSelector.isProductionBuilding = true;

							structureSelector.LateRegisterAsProductionBuilding();//0.5f

							if (finishedOffLine)
							{
								structureSelector.LateCalculateElapsedProduction(elapsedTime);//1.0f
							}



							/*
							MessageNotification m = child.GetComponent<MessageNotification> ();
							m.structureIndex = constructionIndex;
							((ResourceGenerator)resourceGenerator).RegisterMessageNotification (m);
							*/
						}
						foreach (Transform childx in transform)
						{
							if (childx.gameObject.CompareTag("Grass"))
							{
								var o = child.gameObject;
								childx.gameObject.transform.parent = o.transform;
								o.transform.parent = ParentGroup.transform;

								break;
							}
						}
						break;
					}
				}

				Destroy(gameObject);
				inConstruction = false;
			}
		}
	}




	/*
	private void RegisterAsProductionBuilding()
	{	
		for (int i = 0; i < ((ResourceGenerator)resourceGenerator).basicEconomyValues.Length; i++) 
		{
			if (((ResourceGenerator)resourceGenerator).basicEconomyValues [i].StructureType == structureType) 
			{	
				CopyBasicValues (i, ((ResourceGenerator)resourceGenerator).basicEconomyValues [i]);
				break;
			}							
		}
	}

	private void CopyBasicValues(int i, EconomyBuilding basicValuesEB)
	{		
		EconomyBuilding myEconomyParams = new EconomyBuilding();

		myEconomyParams.structureIndex = constructionIndex;
		myEconomyParams.ProdPerHour = basicValuesEB.ProdPerHour;
		myEconomyParams.StoreCap = basicValuesEB.StoreCap;
		myEconomyParams.StructureType = structureType;
		myEconomyParams.ProdType = basicValuesEB.ProdType;
		myEconomyParams.StoreType = basicValuesEB.StoreType;
		myEconomyParams.StoreResource = basicValuesEB.StoreResource;

		((ResourceGenerator)resourceGenerator).existingEconomyBuildings.Add (myEconomyParams);
	}
	*/

	private void UpdateTimeCounter(int remainingTime)               //calculate remaining time
	{
		hours = remainingTime / 60;
		minutes = remainingTime % 60;
		seconds = (int)(60 - ((ProgressBar.value * buildingTime * 60) % 60));

		if (minutes == 60) minutes = 0;
		if (seconds == 60) seconds = 0;

		UpdateTimeLabel();
	}

	private void UpdateTimeLabel()                                  //update the time labels on top
	{
		if (hours > 0 && minutes > 0 && seconds >= 0)
		{
			TimeCounterLb.text = hours + " h " + minutes + " m " + seconds + " s ";
		}
		else if (minutes > 0 && seconds >= 0)
		{
			TimeCounterLb.text = minutes + " m " + seconds + " s ";
		}
		else if (seconds > 0)
		{
			TimeCounterLb.text = seconds + " s ";
		}

	}


	//private void UpdatePrice(int remainingTime)                 //update the price label on the button, based on remaining time		
	//{
 //       /*
	//	//0		30		1
	//	//30		60		3
	//	//60		180		7
	//	//180		600		15
	//	//600		1440	30
	//	//1440	2880	45
	//	//2880	4320	70
	//	//4320			150
	//	// */

 //       //Price.text = "0.05";
 //       //return;

 //       //if (remainingTime >= 4320) { priceno = 150; }
 //       //else if (remainingTime >= 2880) { priceno = 70; }
 //       //else if (remainingTime >= 1440) { priceno = 45; }
 //       //else if (remainingTime >= 600) { priceno = 30; }
 //       //else if (remainingTime >= 180) { priceno = 15; }
 //       //else if (remainingTime >= 60) { priceno = 7; }
 //       //else if (remainingTime >= 30) { priceno = 3; }
 //       //else if (remainingTime >= 0) { priceno = 1; }


 //   }

    public void Finish()
	{
		loadingOverlay = GameObject.Find("Canvas").transform.Find("LoadingOverlay").gameObject;
		loadingSpinner = loadingOverlay.transform.Find("loading").gameObject;
		loadingOverlay.SetActive(true);
		loadingSpinner.SetActive(true);
		StartCoroutine(AddItemToMap());

	}

	public void showToast(string text,
	   int duration)
	{
		StartCoroutine(showToastCOR(text, duration));
	}

	private IEnumerator showToastCOR(string text,
		int duration)
	{
		txt = GameObject.FindGameObjectsWithTag("SpinnerText")[0].GetComponent<Text>();
		Color orginalColor = txt.color;

		txt.text = text;
		txt.enabled = true;

		//Fade in
		yield return fadeInAndOut(txt, true, 0.5f);

		//Wait for the duration
		float counter = 0;
		while (counter < duration)
		{
			counter += Time.deltaTime;
			yield return null;
		}

		//Fade out
		yield return fadeInAndOut(txt, false, 0.5f);

		txt.enabled = false;
		txt.color = orginalColor;
		loadingOverlay.SetActive(false);
		loadingSpinner.SetActive(false);
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



	private IEnumerator SpeedUp()
	{
		Price.text = "Loading...";
		Debug.Log(_structureType);
		string getSpeedUpUrl = Helper.constructionSpeedUpCost(Helper.chainId, playerDetails.usr_sessionID, playerDetails.usr_walletAddress);



		Debug.Log(getSpeedUpUrl);
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
				if (loadingSpinner)
				{
					loadingSpinner.SetActive(false);
					showToast(reqResponse.msg, 4);
				}

				checkExpiredSession.checkSessionExpiration(reqResponse.msg);

				Debug.Log("Erro =>" + reqResponse.msg);

			}
			else
			{
				Price.text = reqResponse.msg;
                if (loadingOverlay)
                {
					loadingOverlay.SetActive(false);
					loadingSpinner.SetActive(false);
				}
				


			}
		}
	}


	private IEnumerator AddItemToMap()
	{
		Debug.Log(_structureType);
		GameObject GameManagerLb = GameObject.Find("lb_GameManager");
		lb_nfts_details nftDetails_ = GameManagerLb.GetComponent<lb_nfts_details>();


		Vector3 position = this.gameObject.transform.position;
		string getNFTUrl = "";
		foreach (Transform child in transform)                                  //parenting and destruction of components no longer needed
		{
			if (child.gameObject.CompareTag("Structure"))//structureType
			{
				StructureSelector structureSelector = child.gameObject.GetComponent<StructureSelector>();
				getNFTUrl = Helper.constructionSpeedUp(position.x + "", position.y + "", playerDetails.usr_walletAddress, Helper.chainId, userNftItems.getNftIndex(buildingName), playerDetails.usr_sessionID, playerDetails.landNo, playerDetails.usr_private_key);
				break;

			}
		}

		Debug.Log(getNFTUrl);

		//StructureSelector sStructSelector = child.gameObject.GetComponent<StructureSelector>();


		// unity web request
		UnityWebRequest www = UnityWebRequest.Get(getNFTUrl);
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

			getNftDetails_and_usedAmount reqResponse = JsonUtility.FromJson<getNftDetails_and_usedAmount>(response);


			if (reqResponse.success != null && !reqResponse.success)
			{
				loadingSpinner.SetActive(false);
				showToast(reqResponse.msg, 4);

				checkExpiredSession.checkSessionExpiration(reqResponse.msg);

				Debug.Log("Erro =>" + reqResponse.msg);

			}
			else
			{
				loadingOverlay.SetActive(false);
				loadingSpinner.SetActive(false);

				StartCoroutine(BalanceHolder.getUserBalances_routine());
				if (!battleMap) //no need to check, the finish button is not visible on the battle map
				{
					if (!((Relay)relay).pauseInput && !((Relay)relay).delay)  //panels are open / buttons were just pressed 
					{
						((SoundFX)soundFX).Click();
						
							
							ProgressBar.value = 1;
						
						
					}
				}

			}
		}
	}
}
