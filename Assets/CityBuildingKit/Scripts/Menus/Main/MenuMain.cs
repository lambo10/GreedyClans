using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UIControllersAndData;

public class MenuMain : MonoBehaviour {											//manages major interface elements and panels
	
	private const int 
		noAnchors = 4,//includes 3 fading "Plus buttons" + the day/night "clock"
		noResPB = 6; //all resources + dobbit/cloak

	public GameObject[] 
		Anchors = new GameObject[noAnchors],						//small interface elements to be deactivated when a panel is opened
		ResourcePB = new GameObject[noResPB];										

	//GameObject.Find only retrieves active objects; it will not find a disabled panel
	public GameObject 
		DeleteBuildingYN, 		//building
		DeleteDefenseYN, 		//defense
		DeleteWeaponYN;			//weapon	

	public bool constructionGreenlit = true, waitForPlacement = false;
	private bool waitForTween = false;

	private Component relay, menuArmy, buildingCreator, wallCreator, weaponCreator;

	void Start () {

		relay = GameObject.Find ("Relay").GetComponent<Relay> ();
		buildingCreator = GameObject.Find ("BuildingCreator").GetComponent<StructureCreator>();
		wallCreator = GameObject.Find ("WallCreator").GetComponent<StructureCreator>();
		weaponCreator = GameObject.Find ("WeaponCreator").GetComponent<StructureCreator>();

	}

	private void Delay()//brief delay to prevent selecting the buildint underneath menus if button positions overlap - the "double command" 
	{
		((Relay)relay).DelayInput();
	}
	private void PauseMovement(bool b)
	{
		((Relay)relay).pauseMovement = b;
	}

	//Building destroy
	public void OnConfirmationBuilding()								//the destroy building confirmation
	{ 
		Delay ();
		PauseMovement (true);
		DeleteBuildingYN.SetActive (true); 
	}
	public void OnCloseConfirmationBuilding() 
	{ 
		PauseMovement (false);
		DeleteBuildingYN.SetActive (false); 
		Delay();
	}

	public void OnDestroyBuilding()
	{
		((StructureCreator)buildingCreator).Cancel();
		DeleteBuildingYN.SetActive (false);//Delay();		//delay deferred to buildingCreator
	}
	public void OnCancelDestroyBuilding()
	{
		((StructureCreator)buildingCreator).OK();
		DeleteBuildingYN.SetActive (false);//Delay(); 
	}

	//Wall destroy

	public void OnConfirmationWall()								//the destroy fortification confirmation
	{ 
		Delay ();
		PauseMovement (true);
		DeleteDefenseYN.SetActive (true); 
		//StatsPadObj.SetActive (false);
	}
	public void OnCloseConfirmationWall() 
	{ 
		PauseMovement (false);
		DeleteDefenseYN.SetActive (false); 
		Delay();
	}
	
	public void OnDestroyWall()
	{
		((StructureCreator)wallCreator).Cancel();
		DeleteDefenseYN.SetActive (false);//Delay();		//delay deferred to buildingCreator
	}
	public void OnCancelDestroyWall()
	{
		((StructureCreator)wallCreator).OK();
		DeleteDefenseYN.SetActive (false);//Delay(); 
	}

	public void OnConfirmationWeapon()								//the destroy fortification confirmation
	{ 
		Delay ();
		PauseMovement (true);
		DeleteWeaponYN.SetActive (true); 
	}
	public void OnCloseConfirmationWeapon() 
	{ 
		PauseMovement (false);
		DeleteWeaponYN.SetActive (false); 
		Delay();
	}

	public void OnDestroyWeapon()
	{
		((StructureCreator)weaponCreator).Cancel();
		DeleteWeaponYN.SetActive (false);//Delay();		//delay deferred to buildingCreator
	}

	public void OnCancelDestroyWeapon()
	{
		((StructureCreator)weaponCreator).OK();
		DeleteWeaponYN.SetActive (false);//Delay(); 
	}
	
}
