/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UIControllersAndData.Store;
using Assets.Scripts.UIControllersAndData.Store.Categories.Cloak;
using Assets.Scripts.UIControllersAndData.Store.Categories.Store;
using Assets.Scripts.UIControllersAndData.Store.ShopItems.Cloak;
using JetBrains.Annotations;
using UIControllersAndData.Models;
using UIControllersAndData.Store.Categories.Ambient;
using UIControllersAndData.Store.Categories.Buildings;
using UIControllersAndData.Store.Categories.Military;
using UIControllersAndData.Store.Categories.Unit;
using UIControllersAndData.Store.Categories.Walls;
using UIControllersAndData.Store.Categories.Weapon;
using UIControllersAndData.Store.ShopItems;
using UIControllersAndData.Store.ShopItems.Building;
using UIControllersAndData.Store.ShopItems.StoreItem;
using UIControllersAndData.Store.ShopItems.UnitItem;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

namespace UIControllersAndData.Store
{
    public class ShopController:MonoBehaviour
    {
        public static ShopController Intance;

        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private GridLayoutGroup _gridForUnits;
        [SerializeField] private Transform _parentForUnitStatusItem; 

        [SerializeField] private StoreItem _storeItem;
        [SerializeField] private BuildingItem _buildingItem;
        [SerializeField] private UnitItem _unitItem;
        [SerializeField] private CloakItem _cloakItem;
        [SerializeField] private UnitStatusItem _unitStatusItem;

        [SerializeField] private Button _closeButton;

        [SerializeField] private Text _timeToComplete;
        [SerializeField] private Text _finishButtonLabel;
        [SerializeField] private Text _hintText;
        
        [SerializeField] private StructureCreator _buildingCreator;
        [SerializeField] private StructureCreator _ambientCreator;
        [SerializeField] private StructureCreator _wallCreator;
        [SerializeField] private StructureCreator _weaponCreator;

        [SerializeField] private Button shopButton;
        [SerializeField] private Button unitsButton;

        public GameObject spinnerTroopsPanel;
        public GameObject spinnerotherSItems;

        private GameObject loadingOverlay;
        private Text reqResultTxt;

        private List<BaseShopItem> _listOfItemsInCategory = new List<BaseShopItem>();

        public List<BaseShopItem> ListOfItemsInCategory
        {
            get { return _listOfItemsInCategory; }
        }

        private List<UnitStatusItem> _listOfUnitStatusItem = new List<UnitStatusItem>();

        public List<UnitStatusItem> ListOfUnitStatusItem => _listOfUnitStatusItem;

        [UsedImplicitly]
        public bool showToast_on = true;

        private void Awake()
        {
            Intance = this;
        }



        /// <summary>
        /// Clears a shop category
        /// </summary>
        /// <param name="shopCategoryType"></param>
        private void ClearShopCategory(ShopCategoryType shopCategoryType)
        {
            Transform content = shopCategoryType == ShopCategoryType.Unit ? _gridForUnits.transform : _grid.transform;
            
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }
        }

        private IEnumerator DrawCategory(IEnumerable<DrawCategoryData> items, ShopCategoryType shopCategoryType)
        {
            ClearShopCategory(shopCategoryType);
            _listOfItemsInCategory.Clear();

            foreach (var data in items)
            {
                if (((uint)shopCategoryType) == 6)
                {
                    spinnerTroopsPanel.SetActive(true);
                    spinnerotherSItems.SetActive(false);
                }
                else
                {
                    spinnerTroopsPanel.SetActive(false);
                    spinnerotherSItems.SetActive(true);
                }

                string getNFTUrl = Helper.get_nft_details_and_used_amount(playerDetails.usr_walletAddress, Helper.chainId, userNftItems.getNftIndex(data.Name.GetName()), playerDetails.usr_sessionID);

                

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

                        checkExpiredSession.checkSessionExpiration(reqResponse.msg);

                        Debug.Log("Erro =>" + reqResponse.msg);

                    }
                    else
                    {
                        string requestResult = reqResponse.msg;

                        if (!data.Name.GetName().Equals("Stone Wall00")) {


                            userNftItems.nftsAmountInUsersWallet[int.Parse(userNftItems.getNftIndex(data.Name.GetName()))] = int.Parse(requestResult);
                        userNftItems.UsedNftsAmount[int.Parse(userNftItems.getNftIndex(data.Name.GetName()))] = int.Parse(reqResponse.usedAmount);

                        if (int.Parse(requestResult) > 0)
                        {




                            BaseShopItem cell;
                            cell = CreateCell(shopCategoryType, data, requestResult, reqResponse.usedAmount);
                            if (cell != null && data.BaseItemData != null)
                            {
                                AddListener(cell, shopCategoryType, data);
                                switch (shopCategoryType)
                                {
                                    case ShopCategoryType.Store:
                                        global::Store.Instance.InitUIStoreItems();
                                        break;
                                    case ShopCategoryType.Buildings:
                                    case ShopCategoryType.Military:
                                        _buildingCreator.ConfigureQuantityForItem(data.Id.GetId(), userNftItems.getNftIndex(data.Name.GetName()));
                                        break;
                                    case ShopCategoryType.Walls:
                                        _wallCreator.ConfigureQuantityForItem(data.Id.GetId(), userNftItems.getNftIndex(data.Name.GetName()));
                                        break;
                                    case ShopCategoryType.Weapon:
                                        _weaponCreator.ConfigureQuantityForItem(data.Id.GetId(), userNftItems.getNftIndex(data.Name.GetName()));
                                        break;
                                    case ShopCategoryType.Ambient:
                                        _ambientCreator.ConfigureQuantityForItem(data.Id.GetId(), userNftItems.getNftIndex(data.Name.GetName()));
                                        break;
                                }
                            }




                        }

                    }


                    }


                }

                spinnerTroopsPanel.SetActive(false);
                spinnerotherSItems.SetActive(false);

                var count = items.Count();
                _grid.constraintCount = count > 8 ? Mathf.CeilToInt(count / 2.0f) : 4;
                yield return null;
            }
            showToast("Done",2);
        }

        private BaseShopItem CreateCell(ShopCategoryType shopCategoryType, DrawCategoryData data, string nftAmount, string usedAmount)
        {
            BaseShopItem cell = null;

            if(shopCategoryType == ShopCategoryType.Store && TransData.Instance.ListOfIdSoldItems.Contains(data.Id.GetId()))
            {
                return null;
            }

            switch (shopCategoryType)
            {
                case ShopCategoryType.Store:
                    cell = Instantiate(_storeItem, _grid.transform);
                    cell.Initialize(data, shopCategoryType);
                    _listOfItemsInCategory.Add(cell);
                    break;				
                case ShopCategoryType.Buildings:
                case ShopCategoryType.Military:
                    cell = Instantiate(_buildingItem, _grid.transform);
                    cell.Initialize(data, shopCategoryType);
                    _listOfItemsInCategory.Add(cell);
                    break;
                case ShopCategoryType.Walls:
                    cell = Instantiate(_buildingItem, _grid.transform);
                    cell.Initialize(data, shopCategoryType);
                    _listOfItemsInCategory.Add(cell);
                    break;
                case ShopCategoryType.Weapon:
                    cell = Instantiate(_buildingItem, _grid.transform);
                    cell.Initialize(data, shopCategoryType);
                    _listOfItemsInCategory.Add(cell);
                    break;
                case ShopCategoryType.Unit:
                    _unitItem.QuantityOfItem.text = nftAmount+"/"+usedAmount;
                    cell = Instantiate(_unitItem, _gridForUnits.transform);
                    cell.Initialize(data, shopCategoryType);
                    _listOfItemsInCategory.Add(cell);
                    break;
                case ShopCategoryType.Cloak:
                    cell = Instantiate(_cloakItem, _grid.transform);
                    cell.Initialize(data, shopCategoryType);
                    _listOfItemsInCategory.Add(cell);
                    break;
                case ShopCategoryType.Ambient:
                    cell = Instantiate(_buildingItem, _grid.transform);
                    cell.Initialize(data, shopCategoryType);
                    _listOfItemsInCategory.Add(cell);
                    break;	
            }
            return cell;
        }
        
        private void AddListener(BaseShopItem cell, ShopCategoryType shopCategoryType, DrawCategoryData itemData)
        {
            cell.OnClick += () =>
            {
                switch (shopCategoryType)
                {
                    case ShopCategoryType.Store:
                        global::Store.Instance.BuyStoreItem(itemData);
                        break;
                    case ShopCategoryType.Buildings:
                    case ShopCategoryType.Military:
                        _buildingCreator.BuyStoreItem(itemData, shopCategoryType,
                            () => { _closeButton.onClick.Invoke(); });
                        break;
                    case ShopCategoryType.Ambient:    
                        _ambientCreator.BuyStoreItem(itemData, shopCategoryType,
                            () => { _closeButton.onClick.Invoke(); });
                        break;
                    case ShopCategoryType.Weapon:    
                        _weaponCreator.BuyStoreItem(itemData, shopCategoryType,
                            () => { _closeButton.onClick.Invoke(); });
                        break;
                    case ShopCategoryType.Walls:    
                        _wallCreator.BuyStoreItem(itemData, shopCategoryType,
                            () => { _closeButton.onClick.Invoke(); });
                        break;
                    case ShopCategoryType.Unit:
                        MenuUnit.Instance.BuyStoreItem(itemData.BaseItemData as UnitCategory, 
                            () => { AddStatusUnit(itemData.BaseItemData as UnitCategory, 1); });
                        break;
                }
            };
        }
        
        [UsedImplicitly]
        public void OnStoreCategoryHandler(int categoryIndex)
        {
            OnOpenShop();
            IEnumerable<DrawCategoryData> drawCategoryData;
            object category;
            StopAllCoroutines();
            switch (categoryIndex)
            {
                case 0:
                    showToast_on = false;
                    category = ShopData.Instance.StoreCategoryData;
                    drawCategoryData = ((StoreCategoryData) category).Category.Select(item =>
                        new DrawCategoryData {BaseItemData = item, Id = item, Name = item}); 
                    StartCoroutine(DrawCategory(drawCategoryData, ShopCategoryType.Store));
                    break;
                case 1:
                    showToast_on = true;
                    category = ShopData.Instance.BuildingsCategoryData;
                    var buildingCategory = ((BuildingsCategoryData) category); 
                    var buildingsCategories = buildingCategory.category.Select(item =>
                    { 
                        var baseData = item.levels.Find(level => level.level == 1);
                        return new DrawCategoryData{BaseItemData = baseData, Id = item, Name = item};
                    });
                    StartCoroutine(DrawCategory(buildingsCategories, ShopCategoryType.Buildings));
                    break;
                case 2:
                    showToast_on = false;
                    category = ShopData.Instance.MilitaryCategoryData;
                    var militaryCategory = ((MilitaryCategoryData) category); 
                    var militaryCategories = militaryCategory.category.Select(item =>
                    { 
                        var baseData = item.levels.Find(level => level.level == 1);
                        return new DrawCategoryData{BaseItemData = baseData, Id = item, Name = item};
                    });
                    StartCoroutine(DrawCategory(militaryCategories, ShopCategoryType.Military));
                    break;
                case 3:
                    showToast_on = true;
                    category = ShopData.Instance.WallsCategoryData;
                    var wallCategory = ((WallsCategoryData) category);
                    var wallCategories = wallCategory.category.Select(item =>
                    { 
                        var baseData = item.levels.Find(level => level.level == 1);
                        return new DrawCategoryData{BaseItemData = baseData, Id = item, Name = item};
                    });
                    StartCoroutine(DrawCategory(wallCategories, ShopCategoryType.Walls));
                    break;
                case 4:
                    showToast_on = true;
                    category = ShopData.Instance.WeaponCategoryData;
                    var weaponCategory = ((WeaponCategoryData) category); 
                    var weaponCategories = weaponCategory.category.Select(item =>
                    { 
                        var baseData = item.levels.Find(level => level.level == 1);
                        return new DrawCategoryData{BaseItemData = baseData, Id = item, Name = item};
                    });
                    StartCoroutine(DrawCategory(weaponCategories, ShopCategoryType.Weapon));
                    break;
                case 5:
                    showToast_on = true;
                    category = ShopData.Instance.UnitCategoryData;
                    var unitCategory = ((UnitCategoryData) category); 
                    var unitCategories = unitCategory.category.Select(item =>
                    { 
                        var baseData = item.levels.Find(level => level.level == 1);
                        return new DrawCategoryData{BaseItemData = baseData, Id = item, Name = item};
                    });
                    StartCoroutine(DrawCategory(unitCategories, ShopCategoryType.Unit));
                    MenuUnit.Instance.LoadValuesfromProc();
                    break;
                case 6:
                    showToast_on = false;
                    category = ShopData.Instance.CloakCategoryData;
                    drawCategoryData = ((CloakCategoryData) category).Category.Select(item =>
                        new DrawCategoryData {BaseItemData = item, Id = item, Name = item});
                    StartCoroutine(DrawCategory(drawCategoryData, ShopCategoryType.Cloak));
                    break;
                case 7:
                    showToast_on = false;
                    category = ShopData.Instance.AmbientCategoryData;
                    var ambientCategory = ((AmbientCategoryData) category); 
                    var ambientCategories = ambientCategory.category.Select(item =>
                    { 
                        var baseData = item.levels.Find(level => level.level == 1);
                        return new DrawCategoryData{BaseItemData = baseData, Id = item, Name = item};
                    });
                    StartCoroutine(DrawCategory(ambientCategories, ShopCategoryType.Ambient));
                    break;
            }
        }

        [UsedImplicitly]
        public void OnOpenShop()
        {
            CameraController.Instance.enabled = false;
            MenuUnit.Instance.LoadValuesfromProc();
        }
        
        [UsedImplicitly]
        public void OnCloseShop()
        {
            showToast_on = false;
            CameraController.Instance.enabled = true;
            MenuUnit.Instance.PassValuestoProc();
          
        }

        [UsedImplicitly]
        public void OnFinishUnitBuilding()
        {
            MenuUnit.Instance.FinishNow();
        }

        public void AddStatusUnitFromSave(int unitId)
        {
            var levels = ShopData.Instance.GetLevels(unitId, ShopCategoryType.Unit);

            var unitData = levels?.FirstOrDefault(x => ((ILevel) x).GetLevel() == 1);
            if (unitData != null)
            {
                AddStatusUnit(unitData as UnitCategory, 1);
            }
        }

        private void AddStatusUnit(UnitCategory data, int level)
        {   
            var unit = _listOfUnitStatusItem.Find(x => x.ItemData.GetId() == data.GetId());
            if (!unit)
            {
                unit = Instantiate(_unitStatusItem, _parentForUnitStatusItem);
                _listOfUnitStatusItem.Add(unit);
                
            }
            unit.Initialize(data, level);
        }

        public void UpdateUnitStatusData(string time, string price)
        {
            _timeToComplete.text = time;
            _finishButtonLabel.text = price;
            
            //TODO: show ((UILabel)HintLabel).text ="Training canceled.";
        }

        public void UpdateHitText(string text)
        {
            _hintText.text = text;
        }

        public void UpdateUnitsCountAndProgess(int id, int count, float progress = 0)
        {
            var unit = _listOfUnitStatusItem.Find(x => x.ItemData.GetId() == id);
            if (unit)
            {
                unit.Count.text = count.ToString();
                unit.QIndex.Count = count;
                unit.Slider.value = progress;
                //TODO: update hint
            }
        }
        
        public void UpdateUnitProgess(int id, float progress)
        {
            var unit = _listOfUnitStatusItem.Find(x => x.ItemData.GetId() == id);
            if (unit)
            {
                unit.Slider.value = progress;
            }
        }

        public void RemoveStatusItemFromList(int index)
        {
            Destroy(_listOfUnitStatusItem[index].gameObject);
            _listOfUnitStatusItem.RemoveAt(index);
        }

        public void OpenUnitsCategory()
        {
            shopButton.onClick.Invoke();
            OnStoreCategoryHandler(5);
            unitsButton.onClick.Invoke();
        }


        void showToast(string text,
   int duration)
        {
            if (showToast_on)
            {

                loadingOverlay = GameObject.Find("Canvas").transform.Find("LoadingOverlay").gameObject;
                loadingOverlay.SetActive(true);
                reqResultTxt = loadingOverlay.transform.GetChild(1).GetComponent<Text>();


                StartCoroutine(showToastCOR(text, duration));

            }
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
}
