using System;
using System.Linq;
using Assets.Scripts.UIControllersAndData;
using Assets.Scripts.UIControllersAndData.Images;
using Assets.Scripts.UIControllersAndData.Store;
using JetBrains.Annotations;
using TMPro;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Unit;
using UnityEngine;
using UnityEngine.UI;

namespace UIControllersAndData.Units
{
    public class UpgradeUnitItem : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _nameField;
        [SerializeField] private TextMeshProUGUI _descriptionField;
        [SerializeField] private TextMeshProUGUI _countOfUnitsField;
        [SerializeField] private TextMeshProUGUI _timeToCompleteField;
        
        [SerializeField] private TextMeshProUGUI _priceToToUpgradeField;
        [SerializeField] private TextMeshProUGUI _priceToSpeedUpField;

        [SerializeField] private Button _upgradeUnitButton;
        [SerializeField] private Button _speedUpUpgradeUnitButton;

        [SerializeField] private Image _upgradeCurrenctyIcon;
        [SerializeField] private Image _speedupUpgradeCurrenctyIcon;
        
        private const string TIME_TO_COMPLETE = "Time to complete: \n";

        private int timeToFinishBuildingInSeconds;

        private UnitInfo _unitInfo;
        private int level;

        private int id;
        private int xpAward;
        private int size;
        private int price;
        private int priceToSpeedupUpgrade;
        private int timeToBuild;
        private CurrencyType currencyType;
        private int count;

        private int levelOfUpgradedUnit;
        /**
         * Called before Start
         */
        private void Awake()
        {
            if (!_icon)
            {
                throw new Exception("_icon is null");
            }
            if (!_nameField)
            {
                throw new Exception("_nameField is null");
            }
            if (!_descriptionField)
            {
                throw new Exception("_descriptionField is null");
            }
            if (!_countOfUnitsField)
            {
                throw new Exception("_countOfUnitsField is null");
            }
            if (!_timeToCompleteField)
            {
                throw new Exception("_timeToCompleteField is null");
            }
            if (!_priceToSpeedUpField)
            {
                throw new Exception("_priceToSpeedUpField is null");
            }

            if (!_priceToToUpgradeField)
            {
                throw new Exception("_priceToToUpgradeField is null");
            }

            if (!_upgradeUnitButton)
            {
                throw new Exception("_upgradeUnitButton is null");
            }
            
            if (!_speedUpUpgradeUnitButton)
            {
                throw new Exception("_speedUpUpgradeUnitButton is null");
            }

            if (!_upgradeCurrenctyIcon)
            {
                throw new Exception("_upgradeCurrenctyIcon is null");
            }
            
            if (!_speedupUpgradeCurrenctyIcon)
            {
                throw new Exception("_speedupUpgradeCurrenctyIcon is null");
            }
            
        }
        
        /**
         * Initialize unit data
         */
        public void Initialize(BaseStoreItemData unit)
        {
            if (unit != null)
            {
                _unitInfo = MenuUnit.Instance.UnitsInfo.Find(x => x.id == ((IId) unit).GetId() && x.level == ((UnitCategory)unit).level);

                //TODO : fix isUpgrade when add another unit and finish it
                
                if (_unitInfo != null && _unitInfo.isUpgrade)
                {
                    int passedSeconds = Convert.ToInt32(DateTime.Now.Subtract(DateTime.Parse(_unitInfo.startOfUpgrade)).TotalSeconds);
                    FormatTime(unit.TimeToBuild, passedSeconds);
                    count = _unitInfo.count;
                    levelOfUpgradedUnit = _unitInfo.level;
                    _upgradeUnitButton.interactable = false;
                    _speedUpUpgradeUnitButton.interactable = true;
                    
                    InvokeRepeating("UpdateTimer", 0.0f, 1.0f);
                }
                else
                {
                    _upgradeUnitButton.interactable = Stats.Instance.IsEnoughCurrency(unit.Price, unit.Currency);
                    _speedUpUpgradeUnitButton.interactable = false;

                    FormatTime(unit.TimeToBuild, 0);
                }

                _upgradeCurrenctyIcon.sprite = Stats.Instance.GetCurrencyIcon(unit.Currency);
                _speedupUpgradeCurrenctyIcon.sprite = Stats.Instance.GetCurrencyIcon(unit.Currency);
                
                _icon.sprite = ImageControler.GetImage(unit.IdOfBigIcon);
                _nameField.text = ((INamed) unit).GetName() + ". Upgrde to: " + ((UnitCategory) unit).level + " level";
                _descriptionField.text = unit.Description;
                _countOfUnitsField.text = Stats.Instance.ExistingUnits.Find(x => x.id == ((IId)unit).GetId() && x.level == ((UnitCategory)unit).level - 1).count.ToString();

                
                
                _priceToToUpgradeField.text = "Upgrade: " + unit.Price;
                _priceToSpeedUpField.text = "Speedup: " + (unit as UnitCategory)?.priceToSpeedUpUpgrade;

                level = ((UnitCategory) unit).level;
                id = ((UnitCategory) unit).id;
                xpAward = unit.XpAward;
                size = ((UnitCategory) unit).size;
                price = ((UnitCategory) unit).Price;
                priceToSpeedupUpgrade = ((UnitCategory) unit).priceToSpeedUpUpgrade;
                timeToBuild = ((UnitCategory) unit).TimeToBuild;
                currencyType = ((UnitCategory) unit).Currency;
            }
        }

       /**
        * Format a time
        */
        private void FormatTime(int timeToBuild, int passedSeconds)
        {
            TimeSpan formattedTime = TimeSpan.FromSeconds((timeToBuild * 60) - passedSeconds);
            _timeToCompleteField.text = TIME_TO_COMPLETE + formattedTime.ToString(@"hh\:mm\:ss");
            timeToFinishBuildingInSeconds = Convert.ToInt32(formattedTime.TotalSeconds);
        }
        
        /**
         * Handle to start unit upgrade
         */
        [UsedImplicitly]
        public void UpgradeUnit()
        {
            _upgradeUnitButton.interactable = false;

            if (_unitInfo == null)
            {
                _unitInfo = new UnitInfo();
                _unitInfo.id = id;
                _unitInfo.count = Stats.Instance.ExistingUnits.Find(x => x.id == id).count;
                _unitInfo.readyToUse = false;
                _unitInfo.timeToBuild = timeToBuild;
                _unitInfo.level = level;
                MenuUnit.Instance.UnitsInfo.Add(_unitInfo);
            }
            _unitInfo.isUpgrade = true;
            _unitInfo.startOfUpgrade = DateTime.Now.ToString();
            _speedUpUpgradeUnitButton.interactable = Stats.Instance.IsEnoughCurrency(priceToSpeedupUpgrade, currencyType);
            
            
            InvokeRepeating("UpdateTimer", 0.0f, 1.0f);
 
            Stats.Instance.crystals -= price;
            
            Stats.Instance.experience += xpAward;
            if(Stats.Instance.experience>Stats.Instance.maxExperience)
                Stats.Instance.experience=Stats.Instance.maxExperience;

            Stats.Instance.occupiedHousing += size;
            Stats.Instance.UpdateUI();

        }

        /**
         * Handle to speedup the upgrade of unit
         */
        [UsedImplicitly]
        public void SpeedUpUpgrade()
        {
            Stats.Instance.crystals -= price;	
            Stats.Instance.UpdateUI();
            UpgradeAndMergeUnits();
            Stats.Instance.UpdateUnitsNo();
        }

        private void AutoRemove()
        {
            Destroy(gameObject);
        }
        
        /**
         * Update time
         */
        void UpdateTimer()
        {
            if (timeToFinishBuildingInSeconds > 0)
            {
                timeToFinishBuildingInSeconds = timeToFinishBuildingInSeconds - 1;
                var timespan = TimeSpan.FromSeconds(timeToFinishBuildingInSeconds);
                _timeToCompleteField.text = TIME_TO_COMPLETE + timespan.ToString(@"hh\:mm\:ss");
            }
            else
            {
//                Debug.Log("Time's up!, Time has elapsed");
                UpgradeAndMergeUnits();
                UpgradeArmyWindow.Instance.SetInfoVisibility();
            }
        }

        private void UpgradeAndMergeUnits()
        {
            ExistedUnit existedUnit = Stats.Instance.ExistingUnits.Find(x => x.id == _unitInfo.id && x.level == level - 1);
            if (existedUnit != null)
            {
                existedUnit.level = level;
                MenuUnit.Instance.UnitsInfo.Remove(_unitInfo);        
            }

            var sameUnit = Stats.Instance.ExistingUnits.FindAll(x => x.id == _unitInfo.id && x.level == level);
            if (sameUnit.Count > 1)
            {
                Stats.Instance.ExistingUnits.RemoveAll(x => x.id == _unitInfo.id && x.level == level);
                var unit = new ExistedUnit
                {
                    id = _unitInfo.id,
                    level = level,
                    count = sameUnit.Aggregate(0, (unitsCount, exitedUnit) => unitsCount + exitedUnit.count)
                };
                Stats.Instance.ExistingUnits.Add(unit);
                Enumerable.Range(1, 10).Aggregate(0, (acc, x) => acc + x);
            }
            AutoRemove();
        }
    }
}
