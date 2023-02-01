using System;
using Assets.Scripts.UIControllersAndData;
using Assets.Scripts.UIControllersAndData.Store;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

/**
 * Basic class for panel to upgrade the selected panel
 */
public class UpgradeBuildingWindow : MonoBehaviour
{
    public static UpgradeBuildingWindow Instance;

    [SerializeField] private GameObject _upgradeBuildingPanel;
    [SerializeField] private Text _messafeField;
    
    private string _msg = "Would you like to upgrade the {0} to the {1} level. It costs {2} {3}.";

    private int _id;
    private int _level;
    private ShopCategoryType _categoryType;

    private StructureCreator _creator;
    /**
     * Awake is called before the Start
     */
    void Awake()
    {
        Instance = this;
    }

    /**
     * Sets info to upgrade the selected building
     */
    public void SetInfo(int id, string structureName, int level, int toLevel, int price, CurrencyType currencyType, ShopCategoryType categoryType, StructureCreator creator)
    {
        if (_messafeField)
        {
            _messafeField.text = string.Format(_msg, structureName, toLevel, price, currencyType);
        }
        else
        {
            throw new Exception("Message field is missing");
        }

        _id = id;
        _level = toLevel;
        _categoryType = categoryType;
        _creator = creator;
        _upgradeBuildingPanel.SetActive(true);
    }
    
    [UsedImplicitly]
    public void OkHandler()
    {
        if (_creator != null)
        {
            _creator.BuildUpgradeForStructure(_id, _level, _categoryType, true);
        }

        HideUpgradeBuildingPanel();
    }
    
    /**
     * Hides the upgrade building panel
     */
    public void HideUpgradeBuildingPanel()
    {
        _upgradeBuildingPanel.SetActive(false);
    }
}
