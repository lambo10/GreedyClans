using System;
using System.Collections;
using System.Linq;
using Assets.Scripts.UIControllersAndData;
using Assets.Scripts.UIControllersAndData.Store;
using UIControllersAndData;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UnityEngine;

public class StructureSelector : BaseSelector {//attached to each building as an invisible 2dtoolkit button

	private int _id = -1;
	private int _level = -1;
	private ShopCategoryType _categoryType = ShopCategoryType.None;

	public ShopCategoryType CategoryType
	{
		get => _categoryType;
		set => _categoryType = value;
	}

	public int Level
	{
		get => _level;
		set => _level = value;
	}

	public int Id
	{
		get => _id;
		set => _id = value;
	}

	// Use this for initialization
	void Start () {
		InitializeComponents ();
		InitializeSpecificComponents ();
	}
	public void DeSelect()
	{
		if (structureClass == "Weapon")
			alphaTween.FadeAlpha (false, 1);
	}

	public void ReSelect()
	{
		if (!CameraController.Instance.enabled)
		{
			return;
		}
		
		if(((Relay)relay).delay||((Relay)relay).pauseInput) return;

		((StructureTween)scaleTween).Tween();

		if (((Relay)relay).currentAlphaTween != null) 
		{			

			if (((Relay)relay).currentAlphaTween.inTransition)			//force fade even if in transition
				((Relay)relay).currentAlphaTween.CancelTransition ();

			((Relay)relay).currentAlphaTween.FadeAlpha (false, 1);
			((Relay)relay).currentAlphaTween = null;
		}

        //TODO: rework this adding of listeners. To fix it, mechanism of building selection should be reworked first
        if(MoveBuildingPanelController.Instance.LastMoveAction != null)
        {
            MoveBuildingPanelController.Instance.MoveButton.onClick.RemoveListener(MoveBuildingPanelController.Instance.LastMoveAction);
        }

        MoveBuildingPanelController.Instance.LastMoveAction = () => { ((StructureCreator)structureCreator).ActivateMovingPad(); };
        MoveBuildingPanelController.Instance.MoveButton.onClick.AddListener(MoveBuildingPanelController.Instance.LastMoveAction);


        if(MoveBuildingPanelController.Instance.LastOkAction != null)
        {
            MoveBuildingPanelController.Instance.OkButton.onClick.RemoveListener(MoveBuildingPanelController.Instance.LastOkAction);
        }

        if(MoveBuildingPanelController.Instance.UpgradeBuildingAction != null)
        {
	        MoveBuildingPanelController.Instance.UpgradeStructureButton.onClick.RemoveListener(MoveBuildingPanelController.Instance.UpgradeBuildingAction);
        }
        
        MoveBuildingPanelController.Instance.LastOkAction = () => { ((StructureCreator)structureCreator).OK(); };
        MoveBuildingPanelController.Instance.OkButton.onClick.AddListener(MoveBuildingPanelController.Instance.LastOkAction);
        
       
        
        if (structureClass == "Weapon") 
		{					
			alphaTween.FadeAlpha (true, 1);
			((Relay)relay).currentAlphaTween = alphaTween;
		} 

		((SoundFX)soundFX).Click();

		if(!battleMap)
		{		
			if(!((StructureCreator)structureCreator).isReselect &&
				!((Relay)relay).pauseInput)
				{

				if (messageNotification != null&&messageNotification.isReady) 
					{
					messageNotification.FadeOut ();
					ResourceGenerator.Harvest (structureIndex);
					messageNotification.isReady = false;
					return;
					}

					((BaseCreator)structureCreator).isReselect = true;						
					int childrenNo = gameObject.transform.childCount;//grass was parented last
					((BaseCreator)structureCreator).OnReselect(gameObject, gameObject.transform.GetChild (childrenNo-1).gameObject, structureType);	
				}
		}
		else if(structureClass=="Building"||structureClass=="Weapon")//the target select on the battle map
		{
			((Helios)helios).selectedStructureIndex = structureIndex;
			if(((Helios)helios).DeployedUnits.Count == 0)return; //ignore if there are no units deployed
	
			int assignedToGroup = -1;
			bool userSelect = false;  //auto or user target select

			for (int i = 0; i <= ((Helios)helios).instantiationGroupIndex; i++) //((BattleProc)battleProcSc).userSelect.Length
			{			
				if(((Helios)helios).userSelect[i])
				{
					assignedToGroup = i;
					((Helios)helios).userSelect[i] = false;
					userSelect = true;
					break;
				}
			}

			if(!userSelect)
			{
				if (int.Parse(Helper.chainId) == 4000)
				{
					assignedToGroup = ((Helios)helios).FindNearestGroup(transform.position);//designate a group to attack this building

				}
				else {
					if (int.Parse(Helper.chainId) == 4000)
					{
						assignedToGroup = ((Helios)helios).FindNearestGroup(transform.position);//designate a group to attack this building
                    }
                    else
                    {
						assignedToGroup = ((Helios)helios).FindNearestGroupServerRpc(transform.position);//designate a group to attack this building

					}
				}
			}

			if(assignedToGroup == -1) return;

			if(((Helios)helios).targetStructureIndex[assignedToGroup] != structureIndex)//if this building is not already the target of the designated group
			{

				if (int.Parse(Helper.chainId) == 4000)
				{
					switch (assignedToGroup)
					{
						case 0:
							((Helios)helios).Select0();
							break;

						case 1:
							((Helios)helios).Select1();
							break;

						case 2:
							((Helios)helios).Select2();
							break;

						case 3:
							((Helios)helios).Select3();
							break;
					}
				}
                else {

					switch (assignedToGroup)
					{
						case 0:
							((Helios)helios).Select0ServerRpc();
							break;

						case 1:
							((Helios)helios).Select1ServerRpc();
							break;

						case 2:
							((Helios)helios).Select2ServerRpc();
							break;

						case 3:
							((Helios)helios).Select3ServerRpc();
							break;
					}

				}

				((Helios)helios).targetStructureIndex[assignedToGroup] = structureIndex;	//pass relevant info to BattleProc for this new target building		
				((Helios)helios).targetCenter[assignedToGroup] = transform.position;
				if (int.Parse(Helper.chainId) == 4000)
				{
					((Helios)helios).FindSpecificBuilding();
                }
                else
                {
					((Helios)helios).FindSpecificBuildingServerRpc();
				}
				((Helios)helios).updateTarget[assignedToGroup] = true;
				((Helios)helios).pauseAttack[assignedToGroup] = true;
			}

		}

		if (!battleMap)
		{
			CheckLevels();	
		}
	}

	private void CheckLevels()
	{
		var levels = ShopData.Instance.GetLevels(Id, CategoryType);

		var building = levels?.FirstOrDefault(x => ((ILevel) x).GetLevel() == Level + 1);
		if (building != null)
		{
			int toLevel = Level + 1;
			int price = building.Price;
			CurrencyType currencyType = building.Currency;
			string structureName = ((INamed) building).GetName();
			MoveBuildingPanelController.Instance.UpgradeBuildingAction = () =>
			{
				((StructureCreator)structureCreator).UpgradeBuilding(Id, structureName, Level, toLevel, price, currencyType, CategoryType, ((StructureCreator)structureCreator));
			};
			MoveBuildingPanelController.Instance.UpgradeStructureButton.onClick.AddListener(MoveBuildingPanelController.Instance.UpgradeBuildingAction);
			
			MoveBuildingPanelController.Instance.UpgradeStructureButton.gameObject.SetActive(true);
		}
	}
	
	
	public void LateRegisterAsProductionBuilding()
	{
		StartCoroutine ("RegisterAsProductionBuilding");
	}

	private IEnumerator  RegisterAsProductionBuilding()//private IEnumerator 
	{			
		yield return new WaitForSeconds (0.5f);

		for (int i = 0; i < ResourceGenerator.basicEconomyValues.Length; i++) 
		{
			if (ResourceGenerator.basicEconomyValues [i].StructureType == structureType) 
			{					
				CopyBasicValues (ResourceGenerator.basicEconomyValues [i]);
				break;
			}							
		}
	}

	private void CopyBasicValues( EconomyBuilding basicValuesEB)
	{		
		//EconomyBuilding myEconomyParams = new EconomyBuilding();

		EconomyBuilding[] EBArray = EconomyBuildings.GetComponentsInChildren<EconomyBuilding>();

		bool buildingRegistered = false;

		if (EBArray.Length != 0) 
		{	
			foreach (EconomyBuilding eb in EBArray) 
			{
				if (eb.structureIndex == structureIndex) 
				{
					eb.structureIndex = structureIndex;
					eb.ProdPerHour = basicValuesEB.ProdPerHour;
					eb.StoreCap = basicValuesEB.StoreCap;
					eb.StructureType = structureType;
					eb.ProdType = basicValuesEB.ProdType;
					eb.StoreType = basicValuesEB.StoreType;
					eb.StoreResource = basicValuesEB.StoreResource;

					ResourceGenerator.index++;
					productionListIndex = ResourceGenerator.index;
					ResourceGenerator.ExistingEconomyBuildings.Add (eb);
					RegisterNotification ();

					buildingRegistered = true;
				}
			}
		}

		if (!buildingRegistered) 
		{
			EconomyBuilding	eb = EconomyBuildings.AddComponent<EconomyBuilding> ();

			eb.structureIndex = structureIndex;
			eb.ProdPerHour = basicValuesEB.ProdPerHour;
			eb.StoreCap = basicValuesEB.StoreCap;
			eb.StructureType = structureType;
			eb.ProdType = basicValuesEB.ProdType;
			eb.StoreType = basicValuesEB.StoreType;
			eb.StoreResource = basicValuesEB.StoreResource;

			ResourceGenerator.index++;
			productionListIndex = ResourceGenerator.index;
			ResourceGenerator.ExistingEconomyBuildings.Add (eb);

			EconomyBuilding = eb;
			
			RegisterNotification ();
		}

	}

	private void RegisterNotification()
	{
		MessageNotification m = GetComponent<MessageNotification> ();
		m.structureIndex = structureIndex;
		ResourceGenerator.RegisterMessageNotification (m);		
	}

	public void LateCalculateElapsedProduction(int elapsedTime)
	{
		StartCoroutine(CalculateElapsedProduction(elapsedTime));
	}
	private IEnumerator CalculateElapsedProduction(int elapsedTime)//private IEnumerator
	{
		yield return new WaitForSeconds (1.0f);

		ResourceGenerator.FastPaceProductionIndividual (productionListIndex, elapsedTime);
	}
}
