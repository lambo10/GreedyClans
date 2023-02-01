using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Menus;
using Assets.Scripts.UIControllersAndData;
using Assets.Scripts.UIControllersAndData.Store;
using JetBrains.Annotations;
using UIControllersAndData;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Ambient;
using UIControllersAndData.Store.Categories.Buildings;
using UIControllersAndData.Store.Categories.Military;
using UIControllersAndData.Store.Categories.Walls;
using UIControllersAndData.Store.Categories.Weapon;
using UIControllersAndData.Store.ShopItems;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// 
/// </summary>

public class BaseCreator : MonoBehaviour
{

	public static BaseCreator Instance;

	#region Variables
	public int totalStructures = 20;                //will use this for iterations, since the const below is not visible in the inspector; trim excess elements !
	protected const int noOfStructures = 20;        //number of maximum existing structures ingame, of any kind - buildings, weapons, walls

	protected int _structuressWithLevel = 1; // items with the level - 1

	public int structureIndex = -1;             //associates the underlying grass with the building on top, so they can be reselected together

	public bool
	inCollision;// prevents placing the structure in inaccessible areas/on top of another building

	public bool
	mouseFollow = true,
	isReselect = false,                 //building is under construction or reselected
	myTown = true,                      //stats game obj does not exist on battle map
	gridBased = false,                  //are objects placed by position or by grid Row/Col index
	allInstant = false;


	public GameObject loadingOverlay;
	// Commits on the 17-Oct-2022
	public Text reqResultTxt;
	public GameObject loadingSpinner;


	public GameObject
	Scope,                              // a crosshair that follows the middle of the screen, for placing a new building; position is adjusted to exact grid middle point
	ParentGroup,                        // to keep all structures in one place in the ierarchy, they are parented to this empty object
	MovingPad;                          // the arrow pad - when created/selected+move, the structures are parented to this object that can move

	public string
	structureXMLTag,
	structureClass;//Building,Wall,Weapon,Ambient

	public GameObject FieldFootstep;

	public TextAsset StructuresXML; //variables for loading building characteristics from XML

	//by correlating structurePrefabs with the associated grass patch, we will be able to use a common formula to instantiate them
	public GameObject[]
		structurePf = new GameObject[noOfStructures],
		grassPf = new GameObject[10],
		constructionPf = new GameObject[10];        //Grass1xWall, Grass1xFence, Grass1xDeco, Grass1xRemovableI, Grass1xRemovableII, Grass1xRemovableIII, Grass2x,Grass3x,Grass4x,Grass5x; separated because removables and walls can be hundreds

	public int[]            //buildings can have different size grass patches; weapons 2x2,walls 1x1 and removables 1x1 one type each
		grassTypes = new int[noOfStructures],
		constructionTypes = new int[noOfStructures],    //matching construction prefabs based on size
		pivotCorrections = new int[noOfStructures],     //set manually 0 false 1 true
		isArray = new int[noOfStructures],              //set manually 0 false 1 true - placement in array mode rows of flowers, walls
		isInstant = new int[noOfStructures];            //set manually 0 false 1 true - no construction sequence - instantiate immediately

	protected ShopCategoryType _currentCategoryType;
	private int _currentItemLevel;

	protected int
		currentSelection = 0,
		gridx = 256,                        //necessary to adjust the middle screen "target" to the exact grid X position
		gridy = 181,                        //necessary to adjust the middle screen "target" to the exact grid Y position
		padZ = -3,                          //moving pad
		zeroZ = 0,
		grassZ = 2;

	protected float
		touchMoveCounter = 0,               //drag-moves the buildings in steps
		touchMoveTime = 0.1f,
		xmlLoaddelay = 0.4f;                    //xml read is slow, some operations must be delayed

	protected Vector2 mousePosition;

	private bool
	dragPhase = false,
	pivotCorrection = false,
	displacedonZ = false;                   //adjusts the position to match the grid

	private GameObject//make private
		selectedStructure,
		selectedGrass,
		selectedConstruction;               //current selected "under construction" prefab

	//private float initSequencedelay = 0.2f;//necessary to load xml properly


	public List<Dictionary<string, string>> structures = new List<Dictionary<string, string>>();
	protected Dictionary<string, string> dictionary;

	public List<int> allowedStructures;
	public List<int> existingStructures;
	//Array creator
	//Multiple selection for fields

	public List<GameObject> starList = new List<GameObject>();//make private
	public List<Vector3> spawnPointList = new List<Vector3>();//make private
	public GameObject spawnPointStar;

	private bool //make private
	drawingField = false,
	startField = false,
	endField = false,
	isField = false,
	buildingFields = false;

	private float
	starSequencer = 0.2f;//0.2f

	private Vector3 startPosition, endPosition;

	private int
	startCell, endCell,             //for start/end cells to draw the field
	startRow, startCol,
	currentFieldIndex,
	fieldDetectorZ = 0;

	protected Component stats, soundFX, transData, cameraController, relay, menuMain, resourceGenerator;//protected

	public string currentSelectionName;


	private bool needToDeleteBeforeUpgrade = false;

	int lb_selected_gameObject;

	List<string> wallsRequests = new List<string>();
	List<GameObject> wallsRequests_selsected_structures = new List<GameObject>();
	public bool can_call_wall_request = false;

	//void Start () {}

	#endregion

	private void Awake()
	{
		Instance = this;
	}

	public void BuyStoreItem(DrawCategoryData data, ShopCategoryType shopCategoryType, Action callback)
	{
		if (data.Id == null)
		{
			throw new Exception("id is null");
		}

		_currentItemLevel = ((ILevel)(data.BaseItemData)).GetLevel();
		currentSelection = data.Id.GetId();

		currentSelectionName = data.Name.GetName();

		_currentCategoryType = shopCategoryType;
		Verify(callback);
	}

	public void BuildUpgradeForStructure(int id, int level, ShopCategoryType categoryType, bool needToDeteleStructure)
	{
		needToDeleteBeforeUpgrade = needToDeteleStructure;
		_currentItemLevel = level;
		currentSelection = id;
		_currentCategoryType = categoryType;
		Verify(null, true);
	}

	protected void InitializeComponents()
	{
		transData = GameObject.Find("TransData").GetComponent<TransData>();
		relay = GameObject.Find("Relay").GetComponent<Relay>();
		soundFX = GameObject.Find("SoundFX").GetComponent<SoundFX>();// //connects to SoundFx - a sound source near the camera
		cameraController = GameObject.Find("tk2dCamera").GetComponent<CameraController>();//to move building and not scroll map
		menuMain = GameObject.Find("Main").GetComponent<MenuMain>();

		if (myTown)
		{
			resourceGenerator = GameObject.Find("ResourceGenerator").GetComponent<ResourceGenerator>();
			stats = GameObject.Find("Stats").GetComponent<Stats>();//conects to Stats script
			structureIndex = ((Stats)stats).structureIndex;
		}
	}

	protected void Verify(Action callback = null, bool isUpgrade = false)
	{
		if (isArray[currentSelection] == 0)
		{
			VerifyConditions(callback, isUpgrade);
		}
		else
		{
			isField = true;
			drawingField = true;
			delay();
			VerifyConditions(callback, isUpgrade);
		}
	}

	protected void delay()
	{
		((Relay)relay).DelayInput();
	}

	// Update is called once per frame
	protected void Update()
	{
		if (MovingPad.activeSelf)
		{
			if (((Relay)relay).delay)
				return;


#if UNITY_STANDALONE || UNITY_EDITOR
			if (Input.GetMouseButtonDown(0))
			{
				int layer_mask = LayerMask.GetMask("Grass");
				RaycastHit hit = new RaycastHit();
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit, 100, layer_mask))
				{
					if (hit.transform.parent.gameObject == selectedGrass.gameObject)
					{
						//Debug.Log("coming here");
						//ItemPreviousPosition itemPosi = selectedStructure.GetComponent<ItemPreviousPosition>();
						//if (itemPosi == null)
						//{
						//	Debug.Log("adding new");
						//	itemPosi = selectedStructure.AddComponent<ItemPreviousPosition>();
						//}

						//if (itemPosi.canSetPositon)
						//{
						//	itemPosi.canSetPositon = false;
						//	itemPosi.temp_x = selectedStructure.transform.position.x;
						//	itemPosi.temp_y = selectedStructure.transform.position.y;
						//	itemPosi.temp_z = selectedStructure.transform.position.z;
						//}

						currentSelectionName = selectedStructure.GetComponent<StructureSelector>().structureType;
						CameraController.Instance.enabled = false;
						dragPhase = true;
						return;
					}
				}
			}

			if (Input.GetMouseButtonUp(0))
			{
				CameraController.Instance.enabled = true;
				dragPhase = false;
			}

			if (Input.GetMouseButton(0) && dragPhase)
			{
				MouseTouchMove();
			}

#elif UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    int layer_mask = LayerMask.GetMask("Grass");
                    RaycastHit hit = new RaycastHit();
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    if(Physics.Raycast(ray, out hit, 100, layer_mask))
                    {
                        if(hit.transform.parent.gameObject == selectedGrass.gameObject)
                        {
                            CameraController.Instance.enabled = false;
                            dragPhase = true;
                            return;
                        }
                    }
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    CameraController.Instance.enabled = true;
                    dragPhase = false;
                }

                if (touch.phase == TouchPhase.Moved && dragPhase)
                {
                    MouseTouchMove();
                }
            }
#endif
		}
		if (drawingField)
		{
			if (!((Relay)relay).delay && Input.GetMouseButtonUp(0))
			{
				RecordSpawnPoint();
			}
		}


        if (can_call_wall_request && wallsRequests.Count() > 0)
        {
			StartCoroutine(makeWallRequests());
			can_call_wall_request = false;
		}

	}

	private IEnumerator MouseOperations(int index)
	{
		yield return new WaitForSeconds(0.2f);

		if (!((Relay)relay).delay)
		{
			switch (index)
			{
				case 0:
					//OK ();
					((MenuMain)menuMain).OnCloseConfirmationBuilding();
					break;
				case 1:
					((MenuMain)menuMain).OnConfirmationBuilding();
					break;
			}
		}
	}

	protected void ReadStructures()
	{
		int countOfExisting = 0;
		switch (structureXMLTag)
		{
			case "Building":
				countOfExisting = ShopData.Instance.BuildingsCategoryData.category.Count + ShopData.Instance.MilitaryCategoryData.category.Count;
				GetBuildingsXML();
				break;
			case "Wall":
				countOfExisting = ShopData.Instance.WallsCategoryData.category.Count;
				GetWallsXML();
				break;
			case "Weapon":
				countOfExisting = ShopData.Instance.WeaponCategoryData.category.Count;
				GetWeaponsXML();
				break;
			case "Ambient":
				countOfExisting = ShopData.Instance.AmbientCategoryData.category.Count;
				GetAmbientXML();
				break;
		}
		existingStructures = Enumerable.Repeat(0, countOfExisting).ToList();
	}


	protected void GetBuildingsXML()//reads structures XML
	{
		List<BuildingCategoryLevels> buildingCategoryLevels = ShopData.Instance.BuildingsCategoryData.category;
		List<BuildingsCategory> buildingsCategoryData = buildingCategoryLevels.SelectMany(level => level.levels).Where(c => c.level == _structuressWithLevel).ToList();

		List<MilitaryCategoryLevels> militaryCategoryLevels = ShopData.Instance.MilitaryCategoryData.category;
		List<MilitaryCategory> militaryCategoryData = militaryCategoryLevels.SelectMany(level => level.levels).Where(c => c.level == _structuressWithLevel).ToList();

		FillBuildingData(buildingsCategoryData);
		FillBuildingData(militaryCategoryData);
	}

	private void FillBuildingData<T>(List<T> data) where T : BaseStoreItemData, INamed, IProdBuilding, IStoreBuilding, IStructure
	{

		foreach (var structureItem in data)
		{


			dictionary = new Dictionary<string, string>();
			dictionary.Add("Name", structureItem.GetName()); // put this in the dictionary.
			dictionary.Add("StructureType", structureItem.GetStructureType()); // put this in the dictionary.
			dictionary.Add("Description", structureItem.Description);
			dictionary.Add("Currency", structureItem.Currency.ToString());
			dictionary.Add("Price", structureItem.Price.ToString());
			dictionary.Add("ProdType", structureItem.GetProdType().ToString());
			dictionary.Add("ProdPerHour", structureItem.GetProdPerHour().ToString());
			dictionary.Add("StoreType", structureItem.GetStoreType().ToString());
			dictionary.Add("StoreResource", structureItem.GetStoreResource().ToString());
			dictionary.Add("StoreCap", structureItem.GetStoreCap().ToString());

			//			if(structureItem.Name == "PopCap"){dictionary.Add("PopCap",structureItem.PopCap.to);}

			dictionary.Add("TimeToBuild", structureItem.TimeToBuild.ToString());
			dictionary.Add("HP", structureItem.HP.ToString());
			dictionary.Add("XpAward", structureItem.XpAward.ToString());
			dictionary.Add("UpRatio", structureItem.UpRatio.ToString());
			structures.Add(dictionary);


		}
	}

	protected void GetWallsXML()//reads buildings XML
	{
		List<WallCategoryLevels> wallCategoryLevels = ShopData.Instance.WallsCategoryData.category;
		List<WallsCategory> wallCategoryData = wallCategoryLevels.SelectMany(level => level.levels).Where(c => c.level == _structuressWithLevel).ToList();


		foreach (WallsCategory structureItem in wallCategoryData)
		{
			dictionary = new Dictionary<string, string>();
			dictionary.Add("Name", structureItem.GetName()); // put this in the dictionary.
			dictionary.Add("Currency", structureItem.Currency.ToString());
			dictionary.Add("Price", structureItem.Price.ToString());
			dictionary.Add("TimeToBuild", structureItem.TimeToBuild.ToString());
			dictionary.Add("HP", structureItem.HP.ToString());
			dictionary.Add("XpAward", structureItem.XpAward.ToString());
			dictionary.Add("UpRatio", structureItem.UpRatio.ToString());
			structures.Add(dictionary);
		}
	}

	protected void GetWeaponsXML()//reads buildings XML
	{
		List<WeaponCategoryLevels> weaponCategoryLevels = ShopData.Instance.WeaponCategoryData.category;
		List<WeaponCategory> weaponCategoryData = weaponCategoryLevels.SelectMany(level => level.levels).Where(c => c.level == _structuressWithLevel).ToList();


		foreach (WeaponCategory structureItem in weaponCategoryData)
		{
			dictionary = new Dictionary<string, string>();
			dictionary.Add("Name", structureItem.GetName()); // put this in the dictionary.
			dictionary.Add("Description", structureItem.Description); // put this in the dictionary.
			dictionary.Add("Currency", structureItem.Currency.ToString());
			dictionary.Add("Price", structureItem.Price.ToString());
			dictionary.Add("TimeToBuild", structureItem.TimeToBuild.ToString());
			dictionary.Add("HP", structureItem.HP.ToString());
			dictionary.Add("Range", structureItem.range.ToString());
			dictionary.Add("FireRate", structureItem.fireRate.ToString());
			dictionary.Add("DamageType", structureItem.damageType.ToString());
			dictionary.Add("TargetType", structureItem.targetType.ToString());
			dictionary.Add("PreferredTarget", structureItem.preferredTarget.ToString());
			dictionary.Add("DamageBonus", structureItem.damageBonus.ToString());
			dictionary.Add("XpAward", structureItem.XpAward.ToString());
			dictionary.Add("UpRatio", structureItem.UpRatio.ToString());
			structures.Add(dictionary);
		}
	}

	protected void GetAmbientXML()//reads buildings XML
	{
		List<AmbientCategoryLevels> ambientCategoryLevels = ShopData.Instance.AmbientCategoryData.category;
		List<AmbientCategory> ambientCategoryData = ambientCategoryLevels.SelectMany(level => level.levels).Where(c => c.level == _structuressWithLevel).ToList();

		foreach (AmbientCategory structureItem in ambientCategoryData)
		{
			dictionary = new Dictionary<string, string>();
			dictionary.Add("Name", structureItem.GetName()); // put this in the dictionary.
			dictionary.Add("Description", structureItem.Description); // put this in the dictionary.
			dictionary.Add("Currency", structureItem.Currency.ToString());
			dictionary.Add("Price", structureItem.Price.ToString());
			dictionary.Add("TimeToBuild", structureItem.TimeToBuild.ToString());
			dictionary.Add("XpAward", structureItem.XpAward.ToString());
			structures.Add(dictionary);
		}
	}

	private void UpdateStructuresAllowed()//string structureType
	{
		switch (structureXMLTag)
		{
			case "Building":
				allowedStructures = ((Stats)stats).maxBuildingsAllowed;
				break;
			case "Wall":
				allowedStructures = ((Stats)stats).maxWallsAllowed;
				break;
			case "Weapon":
				allowedStructures = ((Stats)stats).maxWeaponsAllowed;
				break;
			case "Ambient":
				allowedStructures = ((Stats)stats).maxAmbientsAllowed;
				break;
		}
	}

	public void UpdateButtons()
	{
		StartCoroutine("UpdateLabelStats");
	}

	protected IEnumerator UpdateLabelStats()
	{
		yield return new WaitForSeconds(xmlLoaddelay);

		UpdateStructuresAllowed();

		bool buildingAllowed;


		for (int i = 0; i < structures.ToArray().Length; i++)
		{
			//buildingAllowed =(allowedStructures[i]-existingStructures[i]>0);
			bool hasMoney = false;

			//Debug.Log(structures[i]["Name"]+ "----index: "+ i + "---------- here ----------------");

			if (structures[i]["Currency"] == "Gold")//refunds the gold/mana 
			{
				if (((Stats)stats).gold + ((Stats)stats).deltaGoldPlus - ((Stats)stats).deltaGoldMinus >= int.Parse(structures[i]["Price"]))
				{
					hasMoney = true;
				}
			}
			else if (structures[i]["Currency"] == "Mana")
			{
				if (((Stats)stats).mana + ((Stats)stats).deltaManaPlus - ((Stats)stats).deltaManaMinus >= int.Parse(structures[i]["Price"]))
				{
					hasMoney = true;
				}
			}
			else
			{
				if (((Stats)stats).crystals + ((Stats)stats).deltaCrystalsPlus - ((Stats)stats).deltaCrystalsMinus >= int.Parse(structures[i]["Price"]))
				{
					hasMoney = true;
				}
			}
		}
	}





	//	xy
	public void MoveNW() { Move(0); }   //	-+
	public void MoveNE() { Move(1); }   //	++
	public void MoveSE() { Move(2); }   //	+-
	public void MoveSW() { Move(3); }   //	--


	protected void MovingPadOn()//move pad activated and translated into position
	{
		MovingPad.SetActive(true);

		selectedStructure.transform.parent = MovingPad.transform;
		selectedGrass.transform.parent = MovingPad.transform;

		if (isReselect)
		{
			selectedGrass.transform.position = new Vector3(selectedGrass.transform.position.x,
															selectedGrass.transform.position.y,
															selectedGrass.transform.position.z - 2.0f);//move to front 

			selectedStructure.transform.position = new Vector3(selectedStructure.transform.position.x,
																selectedStructure.transform.position.y,
																selectedStructure.transform.position.z - 6);//move to front
			displacedonZ = true;
		}
		((CameraController)cameraController).movingBuilding = true;

	}

	protected void Move(int i)
	{
		if (((Relay)relay).pauseMovement || ((Relay)relay).delay) return;

		((SoundFX)soundFX).Move(); //128x64

		float stepX = (float)gridx / 2;
		float stepY = (float)gridy / 2;//cast float, otherwise  181/2 = 90, and this accumulats a position error;

		switch (i)
		{
			case 0:
				MovingPad.transform.position += new Vector3(-stepX, stepY, 0);      //NW	
				break;

			case 1:
				MovingPad.transform.position += new Vector3(stepX, stepY, 0);           //NE		
				break;

			case 2:
				MovingPad.transform.position += new Vector3(stepX, -stepY, 0);      //SE		
				break;

			case 3:
				MovingPad.transform.position += new Vector3(-stepX, -stepY, 0);     //SW		
				break;
		}
	}

	protected void MouseTouchMove()
	{
		touchMoveCounter += Time.deltaTime;

		if (touchMoveCounter > touchMoveTime)
		{
			touchMoveCounter = 0;
			TouchMove();
			if (mouseFollow)
				MouseMove();
		}
	}

	private void TouchMove()
	{
		if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
		{
			Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

			if (touchDeltaPosition.x < 0)
			{
				if (touchDeltaPosition.y < 0)
				{
					MoveSW();
				}
				else if (touchDeltaPosition.y > 0)
				{
					MoveNW();
				}
			}
			else if (touchDeltaPosition.x > 0)
			{
				if (touchDeltaPosition.y < 0)
				{
					MoveSE();

				}
				else if (touchDeltaPosition.y > 0)
				{
					MoveNE();
				}
			}
		}
	}

	public void MouseMove()
	{
		GetMousePosition();
		Vector2 deltaPosition = mousePosition - new Vector2(selectedStructure.transform.position.x,
															selectedStructure.transform.position.y);

		if (Mathf.Abs(deltaPosition.x) > gridx || Mathf.Abs(deltaPosition.y) > gridy)
		{
			if (deltaPosition.x < 0)
			{
				if (deltaPosition.y < 0)
				{
					MoveSW();
				}
				else if (deltaPosition.y > 0)
				{
					MoveNW();
				}
			}
			else if (deltaPosition.x > 0)
			{
				if (deltaPosition.y < 0)
				{
					MoveSE();

				}
				else if (deltaPosition.y > 0)
				{
					MoveNE();
				}
			}
		}

	}

	private void GetMousePosition()
	{
		Vector3 gridPos = new Vector3(0, 0, 0);

		// Generate a plane that intersects the transform's position with an upwards normal.
		Plane playerPlane = new Plane(Vector3.back, new Vector3(0, 0, 0));//transform.position + 

		// Generate a ray from the cursor position

		Ray RayCast;

		RayCast = Camera.main.ScreenPointToRay(Input.mousePosition);

		// Determine the point where the cursor ray intersects the plane.
		float HitDist = 0;

		// If the ray is parallel to the plane, Raycast will return false.
		if (playerPlane.Raycast(RayCast, out HitDist))//playerPlane.Raycast
		{
			// Get the point along the ray that hits the calculated distance.
			Vector3 RayHitPoint = RayCast.GetPoint(HitDist);

			int indexCell = GridManager.instance.GetGridIndex(RayHitPoint);

			int col = GridManager.instance.GetColumn(indexCell);
			int row = GridManager.instance.GetRow(indexCell);

			gridPos = GridManager.instance.nodes[row, col].position;
		}
		mousePosition = gridPos;
	}

	public void AdjustStructureZ(int pivotIndex, int spriteIndex)
	{
		var pivot = selectedStructure.transform.GetChild(pivotIndex);
		Vector3 pivotPos = pivot.position; //pivot
		Vector3 spritesPos = selectedStructure.transform.GetChild(spriteIndex).position;//sprites
		float correctiony = 10 / (pivotPos.y + 3300);//ex: fg 10 = 0.1   bg 20 = 0.05  
													 //		all y values must be positive, so we add the grid origin y 3207 +100 to avoid divide by 0; 
													 //		otherwise depth glitches around y 0
		selectedStructure.transform.GetChild(spriteIndex).position = new Vector3(spritesPos.x, spritesPos.y, zeroZ - correctiony - 20);//	transform.GetChild(2).position
	}

	protected void InstantiateStructure(bool isUpgrade = false)//instantiates the building and grass prefabs
	{

		if (isInstant[currentSelection] == 0)
			((Stats)stats).occupiedDobbits++;   //get one dobbit

		((Stats)stats).UpdateUI();          //to reflect the free/total dobbit ratio

		((Relay)relay).pauseInput = true;   //pause all other input - the user starts moving the building

		pivotCorrection = pivotCorrections[currentSelection] == 1;

		var someStructure =
			ShopData.Instance.GetAssetForLevel(currentSelection, _currentCategoryType, this._currentItemLevel);
		if (someStructure)
		{
			Vector3 structurePos;
			Vector3 grassPos;
			if (selectedStructure)
			{
				var position = selectedStructure.transform.position;
				structurePos = new Vector3(position.x, position.y, zeroZ);
				grassPos = new Vector3(position.x, position.y, grassZ);
			}
			else
			{
				structurePos = new Vector3(0, 0, zeroZ);
				grassPos = new Vector3(0, 0, grassZ);
			}

			GameObject structure = Instantiate(someStructure, structurePos, Quaternion.identity);
			GameObject grass = Instantiate(grassPf[grassTypes[currentSelection]], grassPos, Quaternion.identity);
			if (int.Parse(Helper.chainId) != 4000)
			{
				ServerRpc.SpawnObjectServerRpc(structure);
				ServerRpc.SpawnObjectServerRpc(grass);
			}
			else
			{
				ServerRpc.removeNetworkObject(structure);
				ServerRpc.removeNetworkObject(grass);
			}

			if (currentSelection == 3 && _currentCategoryType == ShopCategoryType.Weapon)
			{
				structure.GetComponent<DronePad>().LaunchDrone(currentSelection, this._currentItemLevel, _currentCategoryType);
			}

			selectedStructure = structure;
			selectedGrass = grass;
	
            if (gridBased && !isReselect)
            {
				grass.transform.SetParent(selectedStructure.transform);
            }

			SelectStructure(isUpgrade);
		}
		else
		{
			throw new Exception("Can't build a structure, didn't get building");
		}
	}

	void lb_startDeselection()
	{
		isReselect = false;
		StartCoroutine("Deselect");
		((MenuMain)menuMain).waitForPlacement = false;
		delay();
	}

	protected void SelectStructure(bool isUpgrade = false) //after the grass/building prefabs are instantiated, they must be selected from the existing structures on the map
	{
		//MoveBuildingPanelController.Instance.MoveButton.onClick.AddListener(ActivateMovingPad);
		MoveBuildingPanelController.Instance.RemoveButtonObject.SetActive(false);
		MoveBuildingPanelController.Instance.MoveButton.onClick.AddListener(CancelObject);
		MoveBuildingPanelController.Instance.OkButton.onClick.AddListener(OK);
		MoveBuildingPanelController.Instance.RemoveButton.onClick.AddListener(RemoveItemFromMap);


		MoveBuildingPanelController.Instance.Panel.SetActive(true);

		selectedStructure.GetComponent<StructureSelector>().grassType =
			selectedGrass.GetComponent<GrassSelector>().grassType;

		int posX = 0, posY = 0;

		//		if (!isUpgrade)
		{
			if (!isField)
			{
				posX = (int)(Scope.transform.position.x -       //calculates the middle of the screen - the Scope position,
							  Scope.transform.position.x % gridx); //and adjusts it to match the grid; the dummy is attached to the 2DToolkit camera
				posY = (int)(Scope.transform.position.y -
							 Scope.transform.position.y % gridy);
			}
			else
			{
				posX = (int)spawnPointList[currentFieldIndex].x;
				posY = (int)spawnPointList[currentFieldIndex].y;
			}

		}

		MovingPad.SetActive(true);//activates the arrow move platform					
								  //		if (!isUpgrade)
		{
			if (pivotCorrection)
			{
				selectedStructure.transform.position = new Vector3(posX + gridx / 2, posY, zeroZ - 6);  //moves the building to position				
				selectedGrass.transform.position = new Vector3(posX + gridx / 2, posY, grassZ - 2);     //grass
				MovingPad.transform.position = new Vector3(posX + gridx / 2, posY, padZ);               //move pad
			}

			else
			{
				selectedStructure.transform.position = new Vector3(posX, posY, zeroZ - 6);          //the building must appear in front				
				selectedGrass.transform.position = new Vector3(posX, posY, grassZ - 2);
				MovingPad.transform.position = new Vector3(posX, posY, padZ);
			}
		}

		if (!isField)
		{
			selectedStructure.transform.parent = MovingPad.transform;       //parents the selected building to the arrow moving platform
			selectedGrass.transform.parent = MovingPad.transform;           //parents the grass to the move platform
		}

		((Relay)relay).pauseInput = true;                               //pause other input so the user can move the building	
		((CameraController)cameraController).movingBuilding = true;

	}

	public void PlaceStructure()
	{
		
		Vector3
		grassPos = selectedGrass.transform.position,
		structurePos = selectedStructure.transform.position;
		if (!isReselect)
		{
			((Stats)stats).structureIndex++;
			structureIndex = ((Stats)stats).structureIndex;//unique number for harvesting

			var sSel = selectedStructure.GetComponent<StructureSelector>();
			var gSel = selectedGrass.GetComponent<GrassSelector>();

			if (gridBased)
				RegisterGridPosition(sSel);

			sSel.structureIndex = structureIndex;
			gSel.grassIndex = structureIndex;//grassIndex and structureIex are paired

			//instantiates the construction prefab and pass the relevant info;
			if (isInstant[currentSelection] == 0)
			{
				var Construction = Instantiate(constructionPf[constructionTypes[currentSelection]],
					new Vector3(structurePos.x, structurePos.y, structurePos.z + 6),
					Quaternion.identity);
				if (int.Parse(Helper.chainId) != 4000)
				{
					ServerRpc.SpawnObjectServerRpc(Construction);
				}
				else
				{
					ServerRpc.removeNetworkObject(Construction);
				}

				sSel.Id = currentSelection;
				sSel.CategoryType = _currentCategoryType;
				sSel.Level = _currentItemLevel;

				selectedConstruction = Construction;
				ConstructionSelector cSel = selectedConstruction.GetComponent<ConstructionSelector>();
				cSel.constructionIndex = structureIndex;

				cSel.Id = currentSelection;
				cSel.CategoryType = _currentCategoryType;
				cSel.Level = _currentItemLevel;
				cSel.buildingName = currentSelectionName;

				var levels = ShopData.Instance.GetLevels(currentSelection, _currentCategoryType);
				var structure = levels.FirstOrDefault(x => ((ILevel)x).GetLevel() == _currentItemLevel);
				if (structure != null)
				{
					cSel.buildingTime = structure.TimeToBuild;
					cSel.StructureType = sSel.structureType;
					cSel.grassType = gSel.grassType;

					cSel.ParentGroup = ParentGroup;
					cSel.structureClass = structureClass;

					if (structureXMLTag == "Building")
					{
						cSel.storageAdd = ((IStoreBuilding)structure).GetStoreCap();
					}
				}


			}

			if (gridBased)
			{
				ConstructionSelector cSel = selectedConstruction.GetComponent<ConstructionSelector>();
				cSel.iRow = sSel.iRow;
				cSel.jCol = sSel.jCol;
			}
		}
		else
		{
			selectedStructure.GetComponent<StructureSelector>().DeSelect();
		}

		((GrassCollider)selectedGrass.GetComponentInChildren<GrassCollider>()).isMoving = false;
		selectedGrass.GetComponentInChildren<GrassCollider>().enabled = false;

		//--> Reselect
		if (!isReselect)
		{
			if (isInstant[currentSelection] == 0)
			{
				selectedConstruction.transform.SetParent(ParentGroup.transform);
				selectedGrass.transform.SetParent(selectedConstruction.transform);
			}
			else
			{
				selectedGrass.transform.SetParent(selectedStructure.transform);
			}


			selectedGrass.transform.position = new Vector3(grassPos.x, grassPos.y, grassPos.z + 2);//cancel the instantiation z correction
			selectedStructure.transform.position = new Vector3(structurePos.x, structurePos.y, structurePos.z + 6); //6

			if (isInstant[currentSelection] == 0)
			{
				selectedStructure.transform.SetParent(selectedConstruction.transform);
				selectedStructure.SetActive(false);
				AdjustConstructionZ();
			}
			else
			{
				selectedStructure.transform.SetParent(ParentGroup.transform);
			}
		}
		else if (displacedonZ)
		{

			//send the structures 6 z unit to the background
			selectedGrass.transform.position = new Vector3(grassPos.x, grassPos.y, grassPos.z + 2);//move to back
			selectedStructure.transform.position = new Vector3(structurePos.x, structurePos.y, structurePos.z + 6); //6

			selectedStructure.transform.SetParent(ParentGroup.transform);
			selectedGrass.transform.SetParent(selectedStructure.transform);
			displacedonZ = false;
		}

		AdjustStructureZ(1, 2);

		MovingPad.SetActive(false);
		((CameraController)cameraController).movingBuilding = false;
		MoveBuildingPanelController.Instance.MoveButton.onClick.RemoveAllListeners();
		MoveBuildingPanelController.Instance.RemoveButton.onClick.RemoveAllListeners();
		MoveBuildingPanelController.Instance.OkButton.onClick.RemoveAllListeners();
		MoveBuildingPanelController.Instance.Panel.SetActive(false);

		StartCoroutine("Deselect");
		((MenuMain)menuMain).waitForPlacement = false;
		delay();            //delay and pause input = two different things 
	}




	public void PlaceStructureGridInstant_for_walls(GameObject passedStructure)
	{


		if (!isReselect)
		{
			((Stats)stats).structureIndex++;
			structureIndex = ((Stats)stats).structureIndex;//unique number for pairing the buildings and the grass patches underneath

			((StructureSelector)passedStructure.GetComponent("StructureSelector")).structureIndex = structureIndex;
			((GrassSelector)selectedGrass.GetComponent("GrassSelector")).grassIndex = structureIndex;
			PutBackInPlace();
		}
		else if (displacedonZ)
		{
			PutBackInPlace();
			displacedonZ = false;
		}


		

		passedStructure.transform.SetParent(ParentGroup.transform);




		//selectedGrass.transform.SetParent(passedStructure.transform);
		StructureSelector sSel = passedStructure.GetComponent<StructureSelector>();

		RegisterGridPosition(sSel);

		((GrassCollider)selectedGrass.GetComponentInChildren<GrassCollider>()).isMoving = false;
		selectedGrass.GetComponentInChildren<GrassCollider>().enabled = false;

		AdjustStructureZ(1, 2);
		MovingPad.SetActive(false);
		((CameraController)cameraController).movingBuilding = false;
		MoveBuildingPanelController.Instance.OkButton.onClick.RemoveAllListeners();
		MoveBuildingPanelController.Instance.MoveButton.onClick.RemoveAllListeners();
		MoveBuildingPanelController.Instance.RemoveButton.onClick.RemoveAllListeners();
		MoveBuildingPanelController.Instance.Panel.SetActive(false);

		if (!isReselect)
		{
			sSel.Id = currentSelection;
			sSel.CategoryType = _currentCategoryType;
			//TODO : set the level of the building, wall, etc
			sSel.Level = _currentItemLevel;
		}

		isReselect = false;
		StartCoroutine("Deselect");
		((MenuMain)menuMain).waitForPlacement = false;
		delay();            //delay and pause input = two different things
	}





	public void PlaceStructureGridInstant()
	{
		

		if (!isReselect)
		{
			((Stats)stats).structureIndex++;
			structureIndex = ((Stats)stats).structureIndex;//unique number for pairing the buildings and the grass patches underneath

			((StructureSelector)selectedStructure.GetComponent("StructureSelector")).structureIndex = structureIndex;
			((GrassSelector)selectedGrass.GetComponent("GrassSelector")).grassIndex = structureIndex;
			PutBackInPlace();
		}
		else if (displacedonZ)
		{
			PutBackInPlace();
			displacedonZ = false;
		}




		selectedStructure.transform.SetParent(ParentGroup.transform);
		selectedGrass.transform.SetParent(selectedStructure.transform);
		StructureSelector sSel = selectedStructure.GetComponent<StructureSelector>();

		RegisterGridPosition(sSel);

		((GrassCollider)selectedGrass.GetComponentInChildren<GrassCollider>()).isMoving = false;
		selectedGrass.GetComponentInChildren<GrassCollider>().enabled = false;

		AdjustStructureZ(1, 2);
		MovingPad.SetActive(false);
		((CameraController)cameraController).movingBuilding = false;
		MoveBuildingPanelController.Instance.OkButton.onClick.RemoveAllListeners();
		MoveBuildingPanelController.Instance.MoveButton.onClick.RemoveAllListeners();
		MoveBuildingPanelController.Instance.RemoveButton.onClick.RemoveAllListeners();
		MoveBuildingPanelController.Instance.Panel.SetActive(false);

		if (!isReselect)
		{
			sSel.Id = currentSelection;
			sSel.CategoryType = _currentCategoryType;
			//TODO : set the level of the building, wall, etc
			sSel.Level = _currentItemLevel;
		}

		isReselect = false;
		StartCoroutine("Deselect");
		((MenuMain)menuMain).waitForPlacement = false;
		delay();            //delay and pause input = two different things
	}

	private void PutBackInPlace()
	{
		selectedGrass.transform.position = new Vector3(selectedGrass.transform.position.x,
			selectedGrass.transform.position.y,
			selectedGrass.transform.position.z + 2);//move to back

		selectedStructure.transform.position = new Vector3(selectedStructure.transform.position.x,
			selectedStructure.transform.position.y,
			selectedStructure.transform.position.z + 6); //6		
	}
	private void RegisterGridPosition(StructureSelector sSel)
	{
		int indexCell = GridManager.instance.GetGridIndex(selectedStructure.transform.position);

		int row = GridManager.instance.GetRow(indexCell);
		int col = GridManager.instance.GetColumn(indexCell);

		sSel.iRow = row;
		sSel.jCol = col;
	}

	public void CancelObject()//cancel construction, or reselect building and destroy/cancel
	{
		StructureSelector selectedStructureSelector = selectedStructure.GetComponent<StructureSelector>();

		//var levels = ShopData.Instance.GetLevels(currentSelection, _currentCategoryType);
		//var structure = levels.FirstOrDefault(x => ((ILevel) x).GetLevel() == selectedStructureSelector.Level);


		//if (structure == null)
		//{
		//	return;
		//}



		//if (!isReselect) 
		//{
		//	((Stats)stats).occupiedDobbits--;//frees the dobbit

		//	if (structure.Currency == CurrencyType.Gold)//refunds the gold/mana 
		//	{	
		//		((Stats)stats).AddResources (structure.Price,0,0);
		//	} 
		//	else if (structure.Currency == CurrencyType.Mana)
		//	{	
		//		((Stats)stats).AddResources (0, structure.Price, 0);
		//	}
		//	else
		//	{
		//		((Stats)stats).AddResources (0, 0, structure.Price);
		//	}

		//	((Stats)stats).ApplyMaxCaps();

		//}

		//if(structure is IStoreBuilding && ((IStoreBuilding)structure).GetStoreType() != StoreType.None)
		//{
		//	DecreaseStorage(((IStoreBuilding)structure).GetStoreType().ToString(), 
		//		((IStoreBuilding)structure).GetStoreCap());
		//}



		//((Stats)stats).experience -= structure.XpAward;

		//We comment this line because we don't have PopCap'
		//		((Stats)stats).maxHousing -= int.Parse (structures [currentSelection] ["PopCap"]);

		//selectedStructureSelector.ResourceGenerator.RemoveMessageNotifications(selectedStructureSelector.MessageNotification);
		//selectedStructureSelector.ResourceGenerator.RemoveFromExisting(selectedStructureSelector.EconomyBuilding);
		//if (structure is IId && structure is IStructure)
		//{
		//	selectedStructureSelector.ResourceGenerator.UpdateBasicValues(((IId) structure).GetId(), _currentCategoryType,
		//		_currentItemLevel, ((IStructure)structure).GetStructureType());	
		//}


		if (currentSelection == 3 && _currentCategoryType == ShopCategoryType.Weapon)
		{
			Destroy(selectedStructure.GetComponent<DronePad>().CreatedDrone);
		}
		Destroy(selectedStructure);
		//UpdateExistingStructures (-1);//decreases the array which counts how many structures of each type you have 

		Destroy(selectedGrass);

		MovingPad.SetActive(false);

		if (!isReselect)
		{
			var levels = ShopData.Instance.GetLevels(currentSelection, _currentCategoryType);
			var building = levels.FirstOrDefault(x => ((ILevel)x).GetLevel() == _currentItemLevel);
			int id = ((IId)building).GetId();
			existingStructures[id] -= 1;
		}

		//deactivates the arrow building moving platform
		MoveBuildingPanelController.Instance.MoveButton.onClick.RemoveAllListeners();
		MoveBuildingPanelController.Instance.RemoveButton.onClick.RemoveAllListeners();
		MoveBuildingPanelController.Instance.OkButton.onClick.RemoveAllListeners();
		MoveBuildingPanelController.Instance.Panel.SetActive(false); //deactivates the buttons move/upgrade/place/cancel, at the bottom of the screen
		((Relay)relay).pauseInput = false;          //while the building is selected, pressing other buttons has no effect
		((Relay)relay).pauseMovement = false;       //the confirmation screen is closed
		if (isReselect) { isReselect = false; }     //end the reselect state	
		((Stats)stats).UpdateUI();

		


    }

	private void DecreaseStorage(string resType, int value)//when a building is reselected and destroyed, the gold/mana storage capacity decrease; 
	{
		if (resType == "Gold")
		{
			((Stats)stats).maxGold -= value;//the destroyed building storage cap

		}
		else if (resType == "Mana")
		{
			((Stats)stats).maxMana -= value;

		}
		else if (resType == "Dual") //gold+mana
		{
			((Stats)stats).maxGold -= value;
			((Stats)stats).maxMana -= value;
		}

		((Stats)stats).UpdateUI();//updates the interface numbers
	}

	//  verifies if the building can be constructed:
	//  exceeds max number of structures / enough gold/mana/free dobbits to build?
	//  pays the price to Stats; updates the Stats interface numbers
	protected void VerifyConditions(Action callback, bool isUpgrade = false)
	{

		var levels = ShopData.Instance.GetLevels(currentSelection, _currentCategoryType);
		var building = levels.FirstOrDefault(x => ((ILevel)x).GetLevel() == _currentItemLevel);


		if (building == null)
		{
			return;
		}

		string structureName = ((INamed)building)?.GetName();
		GameObject GameManagerLb = GameObject.Find("lb_GameManager");
		ItemCountingMax itemCountingMax = GameManagerLb.GetComponent<ItemCountingMax>();
		string[] wordSplited = itemCountingMax.maxCount[structureName].Split("/");
		int maxAllowed = Int32.Parse(wordSplited[1]) - Int32.Parse(wordSplited[0]);
		int maxCount = building.MaxCountOfThisItem;
		int id = ((IId)building).GetId();

		if (itemCountingMax.itemCreated.ContainsKey(structureName))
		{
			itemCountingMax.itemCreated[structureName]++;
		}
		else
		{
			itemCountingMax.itemCreated[structureName] = Int32.Parse(wordSplited[0]);
			existingStructures[id] = Int32.Parse(wordSplited[0]);
		}


		int priceOfStructure = building.Price;
		int xwAward = building.XpAward;
		CurrencyType currencyType = building.Currency;

		UpdateStructuresAllowed();//structureXMLTag

		bool canBuild = true;//must pass as true through all verifications

		//max allowed structures ok?

		if (existingStructures[id] >= Int32.Parse(wordSplited[1]))//max already reached
		{
			//existingStructures

			canBuild = false;
			loadingOverlay.SetActive(true);
			showToast("Maximum " + existingStructures[id] +
				" structures of type " + structureName,4); //displays the hint - you can have only 3 structures of this type

		}



		// check for nft here




		//enough gold?

		//      if (currencyType == CurrencyType.Gold) //this needs gold
		//{
		//	int existingGold = ((Stats)stats).gold + ((Stats)stats).deltaGoldPlus - ((Stats)stats).deltaGoldMinus;

		//	if (existingGold < priceOfStructure) 
		//	{
		//		canBuild = false;
		//		MessageController.Instance.DisplayMessage("Not enough gold.");//updates hint text
		//	}
		//} 
		//else  if (currencyType == CurrencyType.Mana)
		//{
		//	int existingMana = ((Stats)stats).mana + ((Stats)stats).deltaManaPlus - ((Stats)stats).deltaManaMinus;

		//	if(existingMana < priceOfStructure)
		//	{
		//		canBuild = false;
		//		MessageController.Instance.DisplayMessage("Not enough mana.");//updates hint text
		//	}
		//}
		//else
		//{
		//	int existingCrystals = ((Stats)stats).crystals + ((Stats)stats).deltaCrystalsPlus - ((Stats)stats).deltaCrystalsMinus;

		//	if(existingCrystals < priceOfStructure)
		//	{
		//		canBuild = false;
		//		MessageController.Instance.DisplayMessage("Not enough crystals.");//updates hint text
		//	}
		//}


		//if (((Stats)stats).occupiedDobbits >= ((Stats)stats).dobbits) //dobbit available?
		//{
		//	canBuild = false;
		//	MessageController.Instance.DisplayMessage("You need more Builders.");
		//}

		if (canBuild)
		{
			if (callback != null)
			{
				callback.Invoke();
			}


			((MenuMain)menuMain).constructionGreenlit = true;//ready to close menus and place the building; 
															 //constructionGreenlit bool necessary because the command is sent by pressing the button anyway

			//((Stats)stats).experience += xwAward; //incre	ases Stats experience  // move this to building finished 

			//if(((Stats)stats).experience>((Stats)stats).maxExperience)
			//{
			//	((Stats)stats).level++;
			//	((Stats)stats).experience-=((Stats)stats).maxExperience;
			//	((Stats)stats).maxExperience+=100;
			//}

			//pays the gold/mana price to Stats
			//if(currencyType == CurrencyType.Gold)
			//{
			//	((Stats)stats).SubstractResources (priceOfStructure, 0, 0);
			//}
			//else if(currencyType == CurrencyType.Mana)
			//{
			//	((Stats)stats).SubstractResources (0, priceOfStructure, 0);
			//}
			//else
			//{

			//	((Stats)stats).SubstractResources (0, 0, priceOfStructure);

			//}

			UpdateButtons();//payments are made, update all

			((Stats)stats).UpdateUI();//tells stats to update the interface - otherwise new numbers are updated but not displayed

			if (!isField || buildingFields)
			{
				if (needToDeleteBeforeUpgrade)
				{
					CancelObject();
					needToDeleteBeforeUpgrade = false;
				}
				UpdateExistingStructures(+1); //an array that keeps track of how many structures of each type exist
				InstantiateStructure(isUpgrade);
			}
		}
		else
		{
			((MenuMain)menuMain).constructionGreenlit = false;//halts construction - the button message is sent anyway, but ignored
		}


	}

	public void ReloadExistingStructures(int index)
	{
		currentSelection = index;
		UpdateExistingStructures(1);
	}

	public void ConfigureQuantityForItem(int id, string mainNftID)
	{
		var building = GetExistingItem(id);
		if (building)
		{
			building.UpdateQuantity(existingStructures[building.Id]);
			building.lambo_UpdateQuantity(mainNftID);
		}
	}

	public BaseShopItem GetExistingItem(int id)
	{
		return ShopController.Intance.ListOfItemsInCategory.Find(x => x.Id == id);
	}

	private void UpdateExistingStructures(int value)// +1 or -1
	{
		/*
		if you are allowed to have 50 wals/50 wooden fences, you can build any type, and the number 50 is decreased as a hole		
		*/
		switch (structureXMLTag)
		{
			case "Building":
				existingStructures[currentSelection] += value;
				//_imageTable.ImageList.Find(x => x.TextureId == id);
				var building = ShopController.Intance.ListOfItemsInCategory.Find(x => x.Id == currentSelection);
				if (building)
				{
					building.UpdateQuantity(existingStructures[currentSelection]);
				}

				break;

			case "Wall":
				//stone walls and wood fences are considered 2 different types, each type can have 50 pieces of any kind at level 1
				if (currentSelection < 3)
				{
					for (int i = 0; i < existingStructures.ToArray().Length; i++)
					{
						existingStructures[i] += value;
					}
				}
				else
				{
					for (int i = 3; i < existingStructures.Count; i++)
					{
						existingStructures[i] += value;
					}
				}
				break;

			case "Weapon":
				existingStructures[currentSelection] += value;
				break;
			case "Ambient":
				existingStructures[currentSelection] += value;
				break;
		}
	}

	public void ConstructionFinished(string constructionType)//called by construction selector finish sequence
	{
		switch (constructionType)
		{  // move this to building finished 

			case "Building":
				/*
				<StoreType>None</StoreType>					<!-- resource stored - none/gold/mana/dual/soldiers-->	
				<StoreCap>0</StoreCap>						<!-- gold/mana/dual/soldiers storage -->			
				*/
				((Stats)stats).occupiedDobbits--;                                   //the dobbit previously assigned becomes available


				int structureTypeIndex = BuildingTypeToIndex(constructionType);
				int storeCapIncrease = int.Parse(structures[structureTypeIndex]["StoreCap"]);

				if (structures[structureTypeIndex]["StoreType"] == "None") { }          //get rid of the none types, most buildings store nothing
				else if (structures[structureTypeIndex]["StoreType"] == "Soldiers")
					((Stats)stats).maxHousing += storeCapIncrease;
				else if (structures[structureTypeIndex]["StoreType"] == "Gold")
					((Stats)stats).maxGold += storeCapIncrease;
				else if (structures[structureTypeIndex]["StoreType"] == "Mana")
					((Stats)stats).maxMana += storeCapIncrease;
				else if (structures[structureTypeIndex]["StoreType"] == "Dual")
				{
					((Stats)stats).maxGold += storeCapIncrease;
					((Stats)stats).maxMana += storeCapIncrease;
				}
				break;

			default:
				break;
		}

	}
	private int BuildingTypeToIndex(string constructionType)
	{
		int structureTypeIndex = 0;
		switch (constructionType)
		{


			default:
				break;
		}

		return structureTypeIndex;
	}

	//receive a Tk2d button message to select an existing building; the button is in the middle of each building prefab and is invisible 

	public void OnReselect(GameObject structure, GameObject grass, string structureType)//string defenseType, 
	{
		selectedStructure = structure;
		selectedGrass = grass;

		ItemPreviousPosition itemPosi = selectedStructure.GetComponent<ItemPreviousPosition>();
		if (itemPosi == null)
		{
			Debug.Log("adding new");
			itemPosi = selectedStructure.AddComponent<ItemPreviousPosition>();
		}

		if (itemPosi.canSetPositon)
		{
			itemPosi.canSetPositon = false;
			itemPosi.temp_x = selectedStructure.transform.position.x;
			itemPosi.temp_y = selectedStructure.transform.position.y;
			itemPosi.temp_z = selectedStructure.transform.position.z;

			if (gridBased) {
				var sSel = selectedStructure.GetComponent<StructureSelector>();
				itemPosi.temp_x = sSel.iRow;
				itemPosi.temp_y = sSel.jCol;
				itemPosi.temp_z = 0;
			}

		}

		currentSelectionName = selectedStructure.GetComponent<StructureSelector>().structureType;


		GetCurrentSelection(structureType);
		ReselectStructure();
	}

	private void GetCurrentSelection(string structureType)
	{
		//{"Academy","Barrel","Chessboard","Classroom","Forge","Generator","Globe","Summon","Toolhouse","Vault","Workshop"};
		switch (structureType)
		{
			case "Toolhouse": currentSelection = 0; break;
			case "Forge": currentSelection = 1; break;
			case "Generator": currentSelection = 2; break;
			case "Vault": currentSelection = 3; break;
			case "Barrel": currentSelection = 4; break;
			case "Summon": currentSelection = 5; break;
			case "Academy": currentSelection = 6; break;
			case "Classroom": currentSelection = 7; break;
			case "Chessboard": currentSelection = 8; break;
			case "Globe": currentSelection = 9; break;
			case "Workshop": currentSelection = 10; break;
			case "Tatami": currentSelection = 11; break;
		}
	}

	private void ignoreChanges()
	{
		//TODO: remove eventlisteners after

		ItemPreviousPosition _itemPreviousPosition = selectedStructure.GetComponent<ItemPreviousPosition>();
	
		if (_itemPreviousPosition == null) return;

		MovingPad.transform.position =
					new Vector3((float)_itemPreviousPosition.temp_x,
						(float)_itemPreviousPosition.temp_y, padZ);
		try
		{
			var levels = ShopData.Instance.GetLevels(currentSelection, _currentCategoryType);
			var building = levels.FirstOrDefault(x => ((ILevel)x).GetLevel() == _currentItemLevel);
			int id = ((IId)building).GetId();
			existingStructures[id] -= 1;
	
			
		}
		catch (Exception e) { }
		StartCoroutine("ignoreCouroutine");
	}

	IEnumerator ignoreCouroutine()
	{
		yield return new WaitForSeconds(0.3f);
		isReselect = false;
		((Relay)relay).pauseInput = false;
	}

	private void changeItemPosition()
	{
		if (canPlaceItem())
		{
			loadingOverlay.SetActive(true);
			loadingSpinner.SetActive(true);
			StartCoroutine(change_item_on_map());
		}

	}


	public IEnumerator change_item_on_map()
	{
		//var structurePos = selectedStructure.transform.position;

		ItemPreviousPosition _itemPreviousPositon = selectedStructure.GetComponent<ItemPreviousPosition>();

		//StructureSelector sStructSelector = selectedStructure.GetComponent<StructureSelector>();

		string changeItemOnMap;

		if (gridBased)
        {
			int indexCell = GridManager.instance.GetGridIndex(selectedStructure.transform.position);

			int Item_row = GridManager.instance.GetRow(indexCell);
			int Item_col = GridManager.instance.GetColumn(indexCell);

	
			changeItemOnMap = Helper.change_item_on_map(_itemPreviousPositon.temp_x + "",
				_itemPreviousPositon.temp_y + "", playerDetails.usr_walletAddress, Helper.chainId, userNftItems.getNftIndex(currentSelectionName), playerDetails.usr_sessionID, playerDetails.landNo, Item_row + "", Item_col + "");

		}
		else
        {
			 changeItemOnMap = Helper.change_item_on_map(_itemPreviousPositon.temp_x + "",
				_itemPreviousPositon.temp_y + "", playerDetails.usr_walletAddress, Helper.chainId, userNftItems.getNftIndex(currentSelectionName), playerDetails.usr_sessionID, playerDetails.landNo, MovingPad.transform.position.x + "", MovingPad.transform.position.y + "");

		}


		// unity web request
		UnityWebRequest www = UnityWebRequest.Get(changeItemOnMap);
		// send the request
		yield return www.SendWebRequest();
		// check for errors
		if (www.isNetworkError || www.isHttpError)
		{
			showToast(www.error, 4);
			loadingOverlay.SetActive(false);
			loadingSpinner.SetActive(false);

		}
		else
		{

			string response = www.downloadHandler.text;

			TypicalJsonResponse reqResponse = JsonUtility.FromJson<TypicalJsonResponse>(response);

			if (reqResponse.success != null && !reqResponse.success)
			{

				checkExpiredSession.checkSessionExpiration(reqResponse.msg);
				showToast(reqResponse.msg, 4);
			}
			else
			{
				_itemPreviousPositon.canSetPositon = true;
				_itemPreviousPositon.temp_x = selectedStructure.transform.position.x;
				_itemPreviousPositon.temp_y = selectedStructure.transform.position.y;
				_itemPreviousPositon.temp_z = selectedStructure.transform.position.z;
			}

			loadingOverlay.SetActive(false);
			loadingSpinner.SetActive(false);

			selectedStructure.transform.SetParent(ParentGroup.transform);
			selectedGrass.transform.SetParent(selectedStructure.transform);
			StructureSelector sSel = selectedStructure.GetComponent<StructureSelector>();

			RegisterGridPosition(sSel);

			if (((Relay)relay).currentAlphaTween != null)
			{

				if (((Relay)relay).currentAlphaTween.inTransition)          //force fade even if in transition
					((Relay)relay).currentAlphaTween.CancelTransition();

				((Relay)relay).currentAlphaTween.FadeAlpha(false, 1);
				((Relay)relay).currentAlphaTween = null;
			}



			delay();

			((GrassCollider)selectedGrass.GetComponentInChildren<GrassCollider>()).isMoving = false;
			selectedGrass.GetComponentInChildren<GrassCollider>().enabled = false;

			AdjustStructureZ(1, 2);
			MovingPad.SetActive(false);
			((CameraController)cameraController).movingBuilding = false;
			MoveBuildingPanelController.Instance.OkButton.onClick.RemoveAllListeners();
			MoveBuildingPanelController.Instance.MoveButton.onClick.RemoveAllListeners();
			MoveBuildingPanelController.Instance.RemoveButton.onClick.RemoveAllListeners();
			MoveBuildingPanelController.Instance.Panel.SetActive(false);

			if (!isReselect)
			{
				sSel.Id = currentSelection;
				sSel.CategoryType = _currentCategoryType;
				//TODO : set the level of the building, wall, etc
				sSel.Level = _currentItemLevel;
			}

			isReselect = false;
			StartCoroutine("Deselect");
			((MenuMain)menuMain).waitForPlacement = false;
			delay();



		}

		yield return null;

	}


	private void RemoveItemFromMap()
    {
		loadingOverlay.SetActive(true);
		loadingSpinner.SetActive(true);
		StartCoroutine(RemoveItemFromMap_req());
	}


	public IEnumerator RemoveItemFromMap_req()
	{

		ItemPreviousPosition _itemPreviousPositon = selectedStructure.GetComponent<ItemPreviousPosition>();
		
		string changeItemOnMap = Helper.remove_item_from_map(_itemPreviousPositon.temp_x + "",
				_itemPreviousPositon.temp_y + "", playerDetails.usr_walletAddress, Helper.chainId, userNftItems.getNftIndex(currentSelectionName), playerDetails.usr_sessionID, playerDetails.landNo);

		


		// unity web request
		UnityWebRequest www = UnityWebRequest.Get(changeItemOnMap);
		// send the request
		yield return www.SendWebRequest();
		// check for errors
		if (www.isNetworkError || www.isHttpError)
		{
			showToast(www.error, 4);
			loadingOverlay.SetActive(false);
			loadingSpinner.SetActive(false);

		}
		else
		{

			string response = www.downloadHandler.text;

			TypicalJsonResponse reqResponse = JsonUtility.FromJson<TypicalJsonResponse>(response);

			if (reqResponse.success != null && !reqResponse.success)
			{

				checkExpiredSession.checkSessionExpiration(reqResponse.msg);
				showToast(reqResponse.msg, 4);
			}
			else
			{
				CancelObject();
				loadingOverlay.SetActive(false);
				loadingSpinner.SetActive(false);


			}



		}

		yield return null;

	}

	private void ReselectStructure()
	{
		MoveBuildingPanelController.Instance.RemoveButtonObject.SetActive(true);
		MoveBuildingPanelController.Instance.MoveButton.onClick.RemoveAllListeners();
		MoveBuildingPanelController.Instance.RemoveButton.onClick.RemoveAllListeners();
		MoveBuildingPanelController.Instance.OkButton.onClick.RemoveAllListeners();
		MoveBuildingPanelController.Instance.MoveButton.onClick.AddListener(ignoreChanges);
		MoveBuildingPanelController.Instance.OkButton.onClick.AddListener(changeItemPosition);
		MoveBuildingPanelController.Instance.RemoveButton.onClick.AddListener(RemoveItemFromMap);
		MoveBuildingPanelController.Instance.Panel.SetActive(true);

		((Relay)relay).pauseInput = true;


		MovingPad.transform.position =
			new Vector3(selectedStructure.transform.position.x,
				selectedStructure.transform.position.y, padZ);

		selectedStructure.GetComponent<StructureSelector>().grassType =
			selectedGrass.GetComponent<GrassSelector>().grassType;

		float posX = 0, posY = 0;

		{
			posX = selectedStructure.transform.position.x;
			posY = selectedStructure.transform.position.y;

		}

		MovingPad.SetActive(true);//activates the arrow move platform					


		{
			if (pivotCorrection)
			{
				selectedStructure.transform.position = new Vector3(posX + gridx / 2, posY, zeroZ - 6);  //moves the building to position				
				selectedGrass.transform.position = new Vector3(posX + gridx / 2, posY, grassZ - 2);     //grass
				MovingPad.transform.position = new Vector3(posX + gridx / 2, posY, padZ);               //move pad
			}

			else
			{
				selectedStructure.transform.position = new Vector3(posX, posY, zeroZ - 6);          //the building must appear in front				
				selectedGrass.transform.position = new Vector3(posX, posY, grassZ - 2);
				MovingPad.transform.position = new Vector3(posX, posY, padZ);
			}
		}

		if (!isField)
		{
			selectedStructure.transform.parent = MovingPad.transform;       //parents the selected building to the arrow moving platform
			selectedGrass.transform.parent = MovingPad.transform;           //parents the grass to the move platform
		}


		((Relay)relay).pauseInput = true;                               //pause other input so the user can move the building	
		((CameraController)cameraController).movingBuilding = true;
	}

	public void Cancel()
	{
		CancelObject();
		delay();
	}

	public void OK()
	{
		int _nftID = int.Parse(userNftItems.getNftIndex(currentSelectionName));
        if (_nftID == 2 || _nftID == 3)
        {
			wall_OK_execution();

		}
        else
        {
			OK_l2();

		}
	}

	IEnumerator makeWallRequests()
	{
		
		while (true)
		{
			yield return new WaitForSeconds(1);

			if(wallsRequests.Count() <= 0)
            {
				break;
            }

			reqResultTxt.text = "";
			loadingOverlay.SetActive(true);
			loadingSpinner.SetActive(true);

			string url = wallsRequests[0];
			GameObject _selectedStructure = wallsRequests_selsected_structures[0];
			wallsRequests.RemoveAt(0);
			wallsRequests_selsected_structures.RemoveAt(0);

			UnityWebRequest www = UnityWebRequest.Get(url);
			// send the request
			yield return www.SendWebRequest();
			// check for errors
			if (www.isNetworkError || www.isHttpError)
			{
				loadingSpinner.SetActive(false);
				showToast(www.error, 4);

			}
			else
			{
				// get the response
				string response = www.downloadHandler.text;

				ItemOnMapResponse reqResponse = JsonUtility.FromJson<ItemOnMapResponse>(response);

				if (reqResponse.success != null && !reqResponse.success)
				{

					checkExpiredSession.checkSessionExpiration(reqResponse.msg);

					// Commits on the 17-Oct-2022
					loadingSpinner.SetActive(false);
					//loadingOverlay.SetActive(false);
					//GameObject.Find(reqResponse.cloneName).SetActive(false);
					//try
					//{
					//	GameObject.Find("Grass" + reqResponse.cloneName).SetActive(false);
					//}
					//catch (Exception e)
					//{

					//}



					var levels = ShopData.Instance.GetLevels(currentSelection, _currentCategoryType);
					var building = levels.FirstOrDefault(x => ((ILevel)x).GetLevel() == _currentItemLevel);
					int id = ((IId)building).GetId();
					existingStructures[id] -= 1;


					showToast(reqResponse.msg, 4);

					Debug.Log(selectedStructure.name);

				}
				else
				{





					if (((Relay)relay).currentAlphaTween != null)
					{

						if (((Relay)relay).currentAlphaTween.inTransition)          //force fade even if in transition
							((Relay)relay).currentAlphaTween.CancelTransition();

						((Relay)relay).currentAlphaTween.FadeAlpha(false, 1);
						((Relay)relay).currentAlphaTween = null;
					}



					delay();

					inCollision = selectedGrass.GetComponentInChildren<GrassCollider>().inCollision;

					if (!inCollision)
					{

						if (allInstant)
						{
							if (gridBased)
							{
								PlaceStructureGridInstant_for_walls(_selectedStructure);


							}
							else
							{
								PlaceStructureGridInstant();
							}

						}
						else
						{
							PlaceStructure();
						}
					}

					((Relay)relay).pauseMovement = false;   //the confirmation screen is closed



					loadingSpinner.SetActive(false);
					loadingOverlay.SetActive(false);
				}



			}




		}
	}


	void wall_OK_execution()
    {
		var _selectedStructure = selectedStructure;
		var structurePos = selectedStructure.transform.position;

		StructureSelector sStructSelector = selectedStructure.GetComponent<StructureSelector>();
		selectedStructure.name = selectedStructure.name + structurePos.x + "," + structurePos.y;
		selectedGrass.name = "Grass" + selectedStructure.name;

		int indexCell = GridManager.instance.GetGridIndex(selectedStructure.transform.position);

		int row = GridManager.instance.GetRow(indexCell);
		int col = GridManager.instance.GetColumn(indexCell);
		string getNFTUrl = Helper.add_to_map(col + "", row + "", structurePos.x + "", structurePos.y + "", playerDetails.usr_walletAddress, Helper.chainId, userNftItems.getNftIndex(currentSelectionName), playerDetails.usr_sessionID, playerDetails.landNo, sStructSelector.structureIndex + "", sStructSelector.grassType + "", sStructSelector.Id + "", selectedStructure.name);

		wallsRequests.Add(getNFTUrl);
		wallsRequests_selsected_structures.Add(_selectedStructure);

		if (wallsRequests.Count() == 1)
		{
			can_call_wall_request = true;
		}

	}

	private void OK_l2()
    {
		if (canPlaceItem())
		{
			reqResultTxt.text = "";
			loadingOverlay.SetActive(true);
			loadingSpinner.SetActive(true);
			StartCoroutine(check_if_addable());
		}
	}




	public IEnumerator check_if_addable()
	{
		var _selectedStructure = selectedStructure;
		var structurePos = selectedStructure.transform.position;

		

		StructureSelector sStructSelector = selectedStructure.GetComponent<StructureSelector>();
		selectedStructure.name = selectedStructure.name + structurePos.x + "," + structurePos.y;
		selectedGrass.name = "Grass" + selectedStructure.name;

		int indexCell = GridManager.instance.GetGridIndex(selectedStructure.transform.position);

		int row = GridManager.instance.GetRow(indexCell);
		int col = GridManager.instance.GetColumn(indexCell);
		string getNFTUrl = Helper.add_to_map(col + "", row + "", structurePos.x + "", structurePos.y + "", playerDetails.usr_walletAddress, Helper.chainId, userNftItems.getNftIndex(currentSelectionName), playerDetails.usr_sessionID, playerDetails.landNo, sStructSelector.structureIndex + "", sStructSelector.grassType + "", sStructSelector.Id + "", selectedStructure.name);

		Debug.Log(getNFTUrl+ "**************************");
		//ConstructionSelector sStructSelector = selectedStructure.GetComponent<ConstructionSelector>();
		//selectedStructure.name = selectedStructure.name + structurePos.x + "," + structurePos.y;
		//selectedGrass.name = "Grass" + selectedStructure.name;


		//string getNFTUrl = Helper.add_to_map(structurePos.x + "", structurePos.y + "", playerDetails.usr_walletAddress, Helper.chainId, userNftItems.getNftIndex(currentSelectionName), playerDetails.usr_sessionID, playerDetails.landNo, sStructSelector.constructionIndex + "", sStructSelector.grassType + "", sStructSelector.Id + "", selectedStructure.name);



		// unity web request
		UnityWebRequest www = UnityWebRequest.Get(getNFTUrl);
		// send the request
		yield return www.SendWebRequest();
		// check for errors
		if (www.isNetworkError || www.isHttpError)
		{
			loadingSpinner.SetActive(false);
			showToast(www.error, 4);

		}
		else
		{
			// get the response
			string response = www.downloadHandler.text;

			ItemOnMapResponse reqResponse = JsonUtility.FromJson<ItemOnMapResponse>(response);

			if (reqResponse.success != null && !reqResponse.success)
			{

				checkExpiredSession.checkSessionExpiration(reqResponse.msg);

				// Commits on the 17-Oct-2022
				loadingSpinner.SetActive(false);
				//loadingOverlay.SetActive(false);
				//GameObject.Find(reqResponse.cloneName).SetActive(false);
				//try
				//{
				//	GameObject.Find("Grass" + reqResponse.cloneName).SetActive(false);
				//}
				//catch (Exception e)
				//{

				//}

				

				var levels = ShopData.Instance.GetLevels(currentSelection, _currentCategoryType);
				var building = levels.FirstOrDefault(x => ((ILevel)x).GetLevel() == _currentItemLevel);
				int id = ((IId)building).GetId();
				existingStructures[id] -= 1;


				showToast(reqResponse.msg, 4);

				Debug.Log(selectedStructure.name);

			}
			else
			{





				if (((Relay)relay).currentAlphaTween != null)
				{

					if (((Relay)relay).currentAlphaTween.inTransition)          //force fade even if in transition
						((Relay)relay).currentAlphaTween.CancelTransition();

					((Relay)relay).currentAlphaTween.FadeAlpha(false, 1);
					((Relay)relay).currentAlphaTween = null;
				}



				delay();

				inCollision = selectedGrass.GetComponentInChildren<GrassCollider>().inCollision;

				if (!inCollision)
				{

					if (allInstant)
					{
                        if (gridBased)
                        {
							PlaceStructureGridInstant_for_walls(_selectedStructure);


                        }
                        else
                        {
							PlaceStructureGridInstant();
						}
						
					}
					else
					{
						PlaceStructure();
					}
				}

				((Relay)relay).pauseMovement = false;   //the confirmation screen is closed



				loadingSpinner.SetActive(false);
				loadingOverlay.SetActive(false);
			}



		}

		yield return null;




	}








	private IEnumerator Deselect()
	{
		yield return new WaitForSeconds(0.3f);
		isReselect = false;
		((Relay)relay).pauseInput = false;      //main menu butons work again
	}

	public void UpgradeBuilding(int id, string structureName, int level, int toLevel, int price, CurrencyType currencyType,
		ShopCategoryType categoryType, StructureCreator creator)
	{
		UpgradeBuildingWindow.Instance.SetInfo(id, structureName, level, toLevel, price, currencyType, categoryType, creator);
		MoveBuildingPanelController.Instance.UpgradeStructureButton.gameObject.SetActive(false);
		OK();
	}

	private void AdjustConstructionZ()
	{
		Vector3 pivotPos = selectedConstruction.transform.GetChild(1).position; //pivot
		Vector3 pos = selectedConstruction.transform.GetChild(3).position;//sprites
		float correctiony = 10 / (pivotPos.y + 3300);//ex: fg 10 = 0.1   bg 20 = 0.05  
													 //all y values must be positive, so we add the grid origin y 3207 +100 to avoid divide by 0; 
													 //otherwise depth glitches around y 0
		selectedConstruction.transform.GetChild(3).position = new Vector3(pos.x, pos.y, zeroZ - correctiony);
	}

	[UsedImplicitly]
	public void ActivateMovingPad()//move pad activated and translated into position
	{
		if (!MovingPad.activeSelf)
		{
			MovingPadOn();
		}
	}

	private void RecordSpawnPoint()
	{
		Vector3 gridPos = new Vector3(0, 0, 0);

		// Generate a plane that intersects the transform's position with an upwards normal.
		Plane playerPlane = new Plane(Vector3.back, new Vector3(0, 0, 0));

		// Generate a ray from the cursor position

		Ray RayCast;

		if (Input.touchCount > 0)
			RayCast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
		else
			RayCast = Camera.main.ScreenPointToRay(Input.mousePosition);

		// Determine the point where the cursor ray intersects the plane.
		float HitDist = 0;

		// If the ray is parallel to the plane, Raycast will return false.
		if (playerPlane.Raycast(RayCast, out HitDist))//playerPlane.Raycast
		{
			// Get the point along the ray that hits the calculated distance.
			Vector3 RayHitPoint = RayCast.GetPoint(HitDist);

			int indexCell = GridManager.instance.GetGridIndex(RayHitPoint);

			int col = GridManager.instance.GetColumn(indexCell);
			int row = GridManager.instance.GetRow(indexCell);


			if (!GridManager.instance.nodes[row, col].isObstacle)
			{
				if (!startField)
				{
					if (!FieldFootstep.activeSelf)
					{
						FieldFootstep.SetActive(true);
						FieldFootstep.GetComponentInChildren<GrassCollider>().collisionCounter = 0;
						FieldFootstep.GetComponentInChildren<GrassCollider>().inCollision = false;
					}

					gridPos = GridManager.instance.nodes[row, col].position;
					startPosition = gridPos;
					startCell = indexCell;
					startCol = col; startRow = row;
					StartCoroutine(CreateStar(gridPos, row, col));
					startField = true;
				}
				else if (!endField)
				{
					gridPos = GridManager.instance.nodes[row, col].position;

					//don't overlapp || not on the same row/column
					if ((gridPos == startPosition) || (startCol != col) && (startRow != row))
						return;

					startCol = 0; startRow = 0;
					endCell = indexCell;
					CreateStarArray();
					starSequencer += 0.1f;
					StartCoroutine(CreateStar(gridPos, row, col));
					endField = true;
				}
			}
		}
	}
	private void CreateStarArray()
	{
		int startRow, startCol, endRow, endCol, leftRow, leftCol, rightRow, rightCol,
		highCell, lowCell,
		highRow, highCol, lowRow, lowCol;

		Vector3 gridPos = new Vector3(0, 0, 0);

		if (startCell > endCell)
		{
			highCell = startCell;
			lowCell = endCell;
		}
		else
		{
			highCell = endCell;
			lowCell = startCell;
		}

		startRow = GridManager.instance.GetRow(highCell);
		startCol = GridManager.instance.GetColumn(highCell);

		endRow = GridManager.instance.GetRow(lowCell);
		endCol = GridManager.instance.GetColumn(lowCell);

		leftRow = startRow; leftCol = endCol; rightRow = endRow; rightCol = startCol;

		if (leftRow >= rightRow) { highRow = leftRow; lowRow = rightRow; }
		else { highRow = rightRow; lowRow = leftRow; }

		if (leftCol >= rightCol) { highCol = leftCol; lowCol = rightCol; }
		else { highCol = rightCol; lowCol = leftCol; }

		for (int i = highRow; i >= lowRow; i--)
		{
			for (int j = lowCol; j <= highCol; j++)
			{
				gridPos = GridManager.instance.nodes[i, j].position;
				if ((i != startRow || j != startCol) && (i != endRow || j != endCol))
				{
					starSequencer += 0.1f;
					StartCoroutine(CreateStar(gridPos, i, j));
				}
			}
		}

		StartCoroutine(LateOnCreateFields());
	}
	private IEnumerator LateOnCreateFields()
	{
		yield return new WaitForSeconds(starSequencer + 0.2f);
		OnCreateFields();
	}

	private IEnumerator CreateStar(Vector3 gridPos, int iRow, int jCol)
	{

		yield return new WaitForSeconds(starSequencer);
		FieldFootstep.transform.position = new Vector3(gridPos.x, gridPos.y, fieldDetectorZ);

		
		GameObject Star = (GameObject)Instantiate(spawnPointStar, gridPos, Quaternion.identity);
		if (int.Parse(Helper.chainId) != 4000)
		{
			ServerRpc.SpawnObjectServerRpc(Star);
		}
		else
		{
			ServerRpc.removeNetworkObject(Star);
		}
		spawnPointList.Add(gridPos);

		starList.Add(Star);
		Component sSel = Star.GetComponent<Selector>();
		((Selector)sSel).iRow = iRow;
		((Selector)sSel).jCol = jCol;
	}

	public void OnCreateFields() { CreateFields(); }
	public void OnCloseFields() { CloseFields(); }

	private void CreateFields()
	{
		buildingFields = true;

		for (int i = 0; i < starList.Count; i++)
		{
			currentFieldIndex = i;
			OnFieldBuild();
		}

		DestroyStars();
		CloseFields();
	}


	private void OnFieldBuild()
	{
		Verify();

		if (((MenuMain)menuMain).constructionGreenlit)
			OK();
	}
	private void CloseFields()
	{
		buildingFields = false;
		isField = false;
		drawingField = false;
		if (startField || endField)
		{
			DestroyStars();
		}
		((Relay)relay).pauseInput = false;
		ResetFootstepPosition();
		StartCoroutine("DeactivateFootstep");
	}
	private IEnumerator DeactivateFootstep()
	{
		yield return new WaitForSeconds(0.2f);
		FieldFootstep.SetActive(false);
	}

	private void ResetFootstepPosition()
	{
		Vector3 gridPos = GridManager.instance.nodes[2, 2].position;
		FieldFootstep.transform.position = new Vector3(gridPos.x, gridPos.y, fieldDetectorZ);
	}


	private void DestroyStars()
	{
		for (int i = 0; i < starList.Count; i++)
		{
			((Star)starList[i].GetComponent("Star")).die = true;
		}
		starList.Clear();
		spawnPointList.Clear();
		startField = false;
		endField = false;
		starSequencer = 0.2f;
	}







	// Commits on the 17-Oct-2022


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


	private bool canPlaceItem()
	{
		foreach (Transform child in MovingPad.transform)
		{
			if (child.tag == "Grass")
			{
				GameObject tiles = child.Find("IsoCollider").gameObject;
			
				if (tiles.GetComponent<GrassCollider>().inCollision)
				{
					loadingOverlay.SetActive(true);
					showToast("You can not place objects on each other.", 4);
					return false;
				}
			}
		}
		return true;
	}


}