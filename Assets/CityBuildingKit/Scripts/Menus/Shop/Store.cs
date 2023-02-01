using UnityEngine;
using Assets.Scripts.Menus;
using UIControllersAndData;
using UIControllersAndData.GameResources;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Store;
using UIControllersAndData.Store.ShopItems;

public class Store : MonoBehaviour //ingame purchase for gold, mana, crystals
{			
	public static Store Instance;

	private const int resourceNo = 3;

	private float posX = 0;

    private bool firstStart = true;

	public int //make private
		quantity,	
		goldSelection,
		manaSelection,
		existingGold,
		existingMana,
		missingGold,
		missingMana; 							//position increment for the buttons as they are disabled/enabled 
		
	private int[]
		pricesGold = new int[resourceNo],			//stored prices in crystals for ingame resources		
		pricesMana = new int[resourceNo],
		quantityGold = new int[resourceNo],
		quantityMana = new int[resourceNo];
 

	void Awake()
	{
		Instance = this;
	}

    // variableGrid.Reposition ()doesn't work properly if the panel is disabled; also triggered by store tab left
    public void UpdateResources()
	{				
		UpdateMissingIngameResources ();
	}

	private void UpdateMissingIngameResources()
	{
		existingGold = Stats.Instance.gold + Stats.Instance.deltaGoldPlus - Stats.Instance.deltaGoldMinus;
		existingMana = Stats.Instance.mana + Stats.Instance.deltaManaPlus - Stats.Instance.deltaManaMinus;
		missingGold = Stats.Instance.maxGold - existingGold;
		missingMana = Stats.Instance.maxMana - existingMana;

        int goldFraction = 4;
        for (int i = 0; i < pricesGold.Length; i++)
        {
            pricesGold[i] = ((Stats.Instance.maxGold / goldFraction) - existingGold) / 1000;
            goldFraction = goldFraction / 2;
        }
        int manaFraction = 4;
        for (int i = 0; i < pricesGold.Length; i++)
        {
            pricesMana[i] = ((Stats.Instance.maxMana / manaFraction) - existingMana) / 1000;
            manaFraction = manaFraction / 2;
        }
    }
	
	private void BuyCrystal(StoreCategory data)
	{
		quantity = data.quantity;//int.Parse (store [crystalSelection] ["Quantity"]);//update quantity displayed on the button

		int maxCrystals = Stats.Instance.maxCrystals;
		int currentCrystals = Stats.Instance.crystals;//has 3 max 5, you buy 10

		if ((currentCrystals + quantity) > maxCrystals) 
		{
			Stats.Instance.maxCrystals += (quantity - (maxCrystals -currentCrystals));//5+(10-2)=13
		}

		//((Stats)stats).crystals += quantity;
		Stats.Instance.AddResources(0,0,quantity);
		Stats.Instance.UpdateUI();
		Stats.Instance.UpdateCreatorMenus();
	}

	public void BuyStoreItem(DrawCategoryData data)
	{
		var itemData = data.BaseItemData as StoreCategory;
		if (itemData == null)
		{
			return;
		}

		switch (itemData.purchasableResource)
		{
				case GameResourceType.Crystal:
					BuyCrystal(itemData);
					break;
				case GameResourceType.Gold:
                    if (itemData.GetId() == 6)
                    {
                        goldSelection = 0;
                    }
                    else if (itemData.GetId() == 7)
                    {
                        goldSelection = 1;
                    }
                    else if (itemData.GetId() == 8)
                    {
                        goldSelection = 2;
                    }
                UpdateResources ();
					BuyGold(itemData);
                    break;
				case GameResourceType.Mana:
                    if (itemData.GetId() == 9)
                    {
                        manaSelection = 0;
                    }
                    else if (itemData.GetId() == 10)
                    {
                        manaSelection = 1;
                    }
                    else if (itemData.GetId() == 11)
                    {
                        manaSelection = 2;
                    }
                UpdateResources ();
					BuyMana(itemData);
                    break;
		}
		
		Stats.Instance.UpdateUI();
		Stats.Instance.UpdateCreatorMenus();
	}

	private void BuyGold(StoreCategory itemData)
	{
        if (missingGold == 0)
		{
			MessengerDisplay ("Vaults are full.");
			return;
		}
		if (pricesGold [goldSelection] > Stats.Instance.crystals)
		{
			MessengerDisplay ("You have only " + Stats.Instance.crystals + " crystals.");
			return;
		}
        Stats.Instance.AddResources(UpdateGoldItem(itemData.GetId()),0,0);
		Stats.Instance.SubstractResources(0,0,pricesGold [goldSelection]);
	}

	private int UpdateGoldItem(int id)
	{
        int fraction = 0;
        switch (id)
		{
			case 6:
                fraction = 4;
                break;
			case 7:
                fraction = 2;
                break;
			case 8:
                fraction = 1;
                break;
		}

        int value = (Stats.Instance.maxGold / fraction) - existingGold;
        int quantity = existingGold + value;
        UpdateUIGold(quantity);
        existingGold = value;
        quantityGold[goldSelection] = value;
        pricesGold[goldSelection] = value / 1000;

        return existingGold;
	}

    private void UpdateUIGold(int quantity)
    {
        UpdateUI(6, Stats.Instance.maxGold, quantity, 4);
        UpdateUI(7, Stats.Instance.maxGold, quantity, 2);
        UpdateUI(8, Stats.Instance.maxGold, quantity, 1);
    }

	private void BuyMana(StoreCategory itemData)
	{
		if (missingMana == 0) 
		{
			MessengerDisplay ("Barrels are full.");
			return;
		}
		if (pricesMana [manaSelection] > Stats.Instance.crystals) 
		{
			MessengerDisplay ("You have only " + Stats.Instance.crystals + " crystals.");
			return;
		}
        Stats.Instance.AddResources(0, UpdateManaItem(itemData.GetId()), 0);
		Stats.Instance.SubstractResources(0,0,pricesMana [manaSelection]);
	}

    private int UpdateManaItem(int id)
    {
        int fraction = 4;
        switch (id)
        {
            case 9:
                fraction = 4;
                break;
            case 10:
                fraction = 2;
                break;
            case 11:
                fraction = 1;
                break;
        }

        int value = (Stats.Instance.maxMana / fraction) - existingMana;
        int quantity = existingMana + value;
        UpdateUIMana(quantity);
        existingMana = value;
        quantityMana[manaSelection] = value;
        pricesMana[manaSelection] = value / 1000;

        return existingMana;
    }

    private void UpdateUIMana(int quantity)
    {
        UpdateUI(9, Stats.Instance.maxMana, quantity, 4);
        UpdateUI(10, Stats.Instance.maxMana, quantity, 2);
        UpdateUI(11, Stats.Instance.maxMana, quantity, 1);
    }

    private void UpdateUI(int itemID, int maxValue, int existingValue, int fraction, bool needDisableItem = true)
    {
	    if (ShopController.Intance != null && ShopController.Intance.ListOfItemsInCategory != null && ShopController.Intance.ListOfItemsInCategory.Count > 0)
	    {


		    BaseShopItem item = ShopController.Intance.ListOfItemsInCategory.Find(x => x != null && x.Id == itemID);
		    if (item != null)
		    {
			    item.QuantityOfItem1.text = ((maxValue / fraction) - existingValue).ToString();
			    item.BuyButtonLabel1.text = (((maxValue / fraction) - existingValue) / 1000).ToString();
			    if (needDisableItem)
			    {
				    if (existingValue >= (maxValue / fraction))
				    {
					    item.gameObject.SetActive(false);
					    if (!TransData.Instance.ListOfIdSoldItems.Contains(itemID))
					    {
						    TransData.Instance.ListOfIdSoldItems.Add(itemID);   
					    }
				    }
			    }
		    }
	    }
    }

    public void InitUIStoreItems()
    {
        UpdateResources();
        UpdateUI(6, Stats.Instance.maxGold, existingGold, 4, false);
        UpdateUI(7, Stats.Instance.maxGold, existingGold, 2, false);
        UpdateUI(8, Stats.Instance.maxGold, existingGold, 1, false);
        UpdateUI(9, Stats.Instance.maxMana, existingMana, 4, false);
        UpdateUI(10, Stats.Instance.maxMana, existingMana, 2, false);
        UpdateUI(11, Stats.Instance.maxMana, existingMana, 1, false);
    }

    public void SetGoldSelectionClick(int value)
    {
        goldSelection = value;
    }

    public void SetManaSelectionClick(int value)
    {
        manaSelection = value;
    }

    private void MessengerDisplay(string s)
	{
		MessageController.Instance.DisplayMessage(s);
	}
}