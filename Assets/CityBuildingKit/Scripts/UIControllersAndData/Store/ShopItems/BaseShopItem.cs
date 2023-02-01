/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using Assets.Scripts;
using Assets.Scripts.UIControllersAndData.Images;
using Assets.Scripts.UIControllersAndData.Store;
using JetBrains.Annotations;
using UIControllersAndData.Models;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UIControllersAndData.Store.Categories.Unit;
using System.Collections.Generic;
using TMPro;

namespace UIControllersAndData.Store.ShopItems
{
    public class BaseShopItem : MonoBehaviour, IBaseShopItem
    {
        public Action OnClick;

        [SerializeField] private Image _backgroundImage;

        [SerializeField] protected Image BigImage;

        [SerializeField] private Image _smallIcon;
        [SerializeField] protected Text TitleText;
        [SerializeField] protected Text TimeLabel;

        [SerializeField] private Image _iconOnBuyButton;
        [SerializeField] protected Text BuyButtonLabel;
        public GameObject infoModal;


        GameObject woodPlain;
        Transform NFTImage;
        Transform Spinner;
        Transform health;
        Transform alive;
        Transform dead;
        Transform name_;
        Transform inputs;
        Transform reviveSpinner;
        TMPro.TMP_InputField enterQuantityRevive;
        TextMeshProUGUI msgDisplay;
        Button reviveBtn;
        Color32 reviveBtnColor = new Color32(135, 40, 0, 255);
        

        public Text BuyButtonLabel1
        {
            get { return BuyButtonLabel; }
            set { BuyButtonLabel = value; }
        }

        [SerializeField] public Text QuantityOfItem;

        public Text QuantityOfItem1
        {
            get => QuantityOfItem;
            set => QuantityOfItem = value;
        }

        public BaseStoreItemData ItemData { get; set; }

        [UsedImplicitly]
        public void OnItemClick()
        {
            GameObject GameManagerLb = GameObject.FindGameObjectsWithTag("lbGameManager")[0];
            lb_nfts_details nftDetails_ = GameManagerLb.GetComponent<lb_nfts_details>();
            ItemCountingMax itemCountingMax = GameManagerLb.GetComponent<ItemCountingMax>();
            List<lb_nft> nfts_ = nftDetails_.nfts;

            for (int i = 16; i < 27; i++) {

                var nft = nfts_[i];
                    if (ItemData is INamed namedItem)
                    {

                        if (nft.name.ToLower() == namedItem.GetName().ToLower())
                        {
                            itemCountingMax.maxCount[nft.name] = TimeLabel.text;
                            
                            break;
                        }
                    }
                
        }

            if (OnClick != null)
            {
                OnClick();
            }
        }

        [UsedImplicitly]
        public void OnDescriptionClick()
        {

            if (ItemData is INamed namedItem)
            {
                Debug.Log(namedItem.GetName());
                GameObject GameManagerLb = GameObject.FindGameObjectsWithTag("lbGameManager")[0];
                lb_nfts_details nftDetails_ = GameManagerLb.GetComponent<lb_nfts_details>();
                List<lb_nft> nfts_ = nftDetails_.nfts;
                lb_nft selectedNft;
                foreach (var nft in nfts_)
                {
                    if (nft.name.ToLower() == namedItem.GetName().ToLower())
                    {
                        selectedNft = nft;
                        GameObject canvas = GameObject.Find("Canvas");

                        infoModal = canvas.transform.GetChild(12).gameObject;
                        var info_M_Manager = infoModal.GetComponent<infoModalManager>();
                        info_M_Manager.passDetails(selectedNft.name, selectedNft.health, selectedNft.damage, selectedNft.speed, selectedNft.range, selectedNft.description, selectedNft.icon);
                        infoModal.SetActive(true);
                        break;
                    }
                }


            }
            else
            {
                throw new Exception("named item is not null");
            }
        }

       



        





        public virtual void Initialize(DrawCategoryData data, ShopCategoryType shopCategoryType)
        {

            ItemData = data.BaseItemData;
            Id = data.Id.GetId();



            if (ItemData == null)
            {
                throw new Exception("Item data is null");
            }

            TitleText.text = data.Name.GetName();


            BuyButtonLabel.text = ItemData.Price.ToString();

            if (BigImage)
            {
                BigImage.enabled = !string.IsNullOrEmpty(ItemData.IdOfBigIcon);
                BigImage.sprite = ImageControler.GetImage(ItemData.IdOfBigIcon);
            }

            if (_smallIcon)
            {
                _smallIcon.enabled = !string.IsNullOrEmpty(ItemData.IdOfSmallIcon);
                _smallIcon.sprite = ImageControler.GetImage(ItemData.IdOfSmallIcon);
            }

            if (_iconOnBuyButton)
            {
                _iconOnBuyButton.enabled = !string.IsNullOrEmpty(ItemData.IdOfIconOnBuyButton);
                _iconOnBuyButton.sprite = ImageControler.GetImage(ItemData.IdOfIconOnBuyButton);
            }


        }





        public void onRevivalClick()
        {
            GameObject GameManagerLb = GameObject.Find("lb_GameManager");
            lb_nfts_details nftDetails_ = GameManagerLb.GetComponent<lb_nfts_details>();
            List<lb_nft> nfts_ = nftDetails_.nfts;
            foreach (var nft in nfts_)
            {
                if (ItemData is INamed namedItem)
                {

                    if (nft.name.ToLower() == namedItem.GetName().ToLower())
                    {
                        var gObjects = GameObject.FindGameObjectsWithTag("lbRevivalModal");
                        if (gObjects.Length > 0)
                        {
                            revivalModalPanel modalPanel = gObjects[0].GetComponent<revivalModalPanel>();
                            var nftImage = GameObject.FindGameObjectWithTag("nftImage");
                            modalPanel.openModal();
                            //Debug.Log(nftImage);
                            woodPlain = GameObject.Find("revivalWindow/ModalBackground/DescriptionPanel/woodPlain");
                            NFTImage = woodPlain.transform.Find("NFTImage");
                            Spinner = woodPlain.transform.Find("Spinner 6");
                            health = woodPlain.transform.Find("descriptions/healthGroup/value");
                            alive = woodPlain.transform.Find("descriptions/alive/value");
                            dead = woodPlain.transform.Find("descriptions/dead/value");
                            name_ = woodPlain.transform.Find("descriptions/Name");
                            inputs = woodPlain.transform.Find("inputs");
                            msgDisplay = inputs.transform.Find("MsgErr").gameObject.GetComponent<TextMeshProUGUI>();
                            reviveSpinner = inputs.transform.Find("Spinner 6");
                            enterQuantityRevive = inputs.transform.Find("quantity").gameObject.GetComponent<TMPro.TMP_InputField>();
                            enterQuantityRevive.gameObject.SetActive(false);
                            enterQuantityRevive.text = "";
                            reviveBtn = inputs.transform.Find("reviveBTN").gameObject.GetComponent<Button>();
                            Spinner.gameObject.SetActive(true);
                            reviveBtn.gameObject.SetActive(false);
                            reviveSpinner.gameObject.SetActive(false);
                            msgDisplay.gameObject.SetActive(false);
                            inputs.gameObject.SetActive(false);
                            inputs.transform.Find("cost/value").gameObject.SetActive(false);
                            alive.GetComponent<TMPro.TextMeshProUGUI>().text = "";
                            dead.GetComponent<TMPro.TextMeshProUGUI>().text = "";
                            name_.GetComponent<TMPro.TextMeshProUGUI>().text = nft.name;
                            NFTImage.GetComponent<Image>().sprite = nft.icon;
                            health.gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = nft.health.ToString();
                            StartCoroutine(GetAliveOrDead(nft.id));
                        }
                    }
                }
            }

        }




        public IEnumerator checkItemCount(string id)
        {
            string getNFTcount = Helper.getNftAmountLeftAndPrice(playerDetails.usr_walletAddress, Helper.chainId, id, playerDetails.usr_sessionID);

            Debug.Log(getNFTcount);
            // unity web request
            UnityWebRequest www = UnityWebRequest.Get(getNFTcount);
            // send the request
            yield return www.SendWebRequest();
            // check for errors
            woodPlain.transform.Find("Spinner 6").gameObject.SetActive(false);
            if (www.isNetworkError || www.isHttpError)
            {


            }
            else
            {
                // get the response
                string response = www.downloadHandler.text;
                Debug.Log(response);


                getNftAmountLeft_and_price reqResponse = JsonUtility.FromJson<getNftAmountLeft_and_price>(response);
                var price = double.Parse(reqResponse.price);
                if (reqResponse.success != null && !reqResponse.success)
                {

                }
                else
                {
                    Spinner.gameObject.SetActive(false);
                    enterQuantityRevive.gameObject.SetActive(true);
                    reviveBtn.gameObject.SetActive(true);
                    enterQuantityRevive.onValueChanged.AddListener(value =>
                    {
                        try
                        {
                            if (int.Parse(Helper.chainId) == 4000)
                            {
                                inputs.transform.Find("cost/value").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0:0.000}", (price / 2) * double.Parse(value)) + " GOLD";
                            }
                            else
                            {
                                inputs.transform.Find("cost/value").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0:0.000}", (price / 2) * double.Parse(value)) + " BNB";
                            }
                            inputs.transform.Find("cost/value").gameObject.SetActive(true);
                        }
                        catch (Exception e)
                        {

                        }

                    });

                    reviveBtn.onClick.AddListener(() => {
                        try
                        {
                            var quantityEntered = double.Parse(enterQuantityRevive.text);
                            reviveBtn.gameObject.GetComponent<Image>().color = Color.black;
                            reviveBtn.gameObject.SetActive(false);
                            string reviveUrl = Helper.revive(playerDetails.usr_private_key, Helper.chainId, playerDetails.usr_walletAddress, id, playerDetails.usr_sessionID, quantityEntered.ToString(), ((price / 2) * quantityEntered).ToString());
                            Debug.Log(reviveUrl);
                            reviveSpinner.gameObject.SetActive(true);
                            msgDisplay.gameObject.SetActive(false);
                            StartCoroutine(reviveNFTRequest(reviveUrl,id));

                        }
                        catch (Exception e)
                        {

                        }

                    });
                }
            }
        }


        IEnumerator reviveNFTRequest(string url, string nftId)
        {

            // unity web request
            UnityWebRequest www = UnityWebRequest.Get(url);
            // send the request
            yield return www.SendWebRequest();
            reviveSpinner.gameObject.SetActive(false);

            reviveBtn.gameObject.GetComponent<Image>().color = reviveBtnColor;
            reviveBtn.gameObject.SetActive(true);
            // check for errors
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                //erromsg.text = "Error => " + www.error;
            }
            else
            {
                string response = www.downloadHandler.text;
                TypicalJsonResponse reqResponse = JsonUtility.FromJson<TypicalJsonResponse>(response);
                msgDisplay.gameObject.SetActive(true);
                if (reqResponse.success != null && !reqResponse.success)
                {
                    msgDisplay.color = Color.red;
                    msgDisplay.text = reqResponse.msg;
                }
                else
                {
                    msgDisplay.color = new Color32(104, 195, 163, 255);
                    msgDisplay.text = "Item Revived.";
                    StartCoroutine(GetAliveOrDead(nftId));
                    StartCoroutine(BalanceHolder.getUserBalances_routine());
                }
            }
        }


        IEnumerator GetAliveOrDead(string nftId)
        {
            string getAliveUrl = Helper.getHealthAliveOrDead(playerDetails.usr_walletAddress, Helper.chainId, nftId, playerDetails.usr_sessionID);
            UnityWebRequest www = UnityWebRequest.Get(getAliveUrl);
            // send the request
            yield return www.SendWebRequest();
            // check for errors
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                //errorMessage.text = "Error: " + www.error;
            }
            else
            {
                // get the response
                string response = www.downloadHandler.text;
                getAliveOrDead getAliveO_Dead = JsonUtility.FromJson<getAliveOrDead>(response);
                if (!getAliveO_Dead.success)
                {

                }
                else
                {

                    alive.GetComponent<TMPro.TextMeshProUGUI>().text = getAliveO_Dead.alive;
                    dead.GetComponent<TMPro.TextMeshProUGUI>().text = getAliveO_Dead.dead;
                    if (getAliveO_Dead.dead == "0")
                    {

                        woodPlain.transform.Find("Spinner 6").gameObject.SetActive(false);

                    }
                    else
                    {
                        inputs.gameObject.SetActive(true);
                        StartCoroutine(checkItemCount(nftId));
                    }

                    //errorMessage.text = "Error: " + seedPhrase.msg;
                }

            }
            //activateCreateBtn();

        }







        public int Id { get; set; }

        public void UpdateQuantity(int quantity)
        {
            if (quantity == ItemData.MaxCountOfThisItem)
            {
                _backgroundImage.sprite = ImageControler.GetImage(Constants.ID_ITEM_BACKGROUND);
                BigImage.sprite = ImageControler.GetImage(ItemData.IdOfBlackBigIcon);
                BuyButtonLabel.transform.parent.GetComponent<Button>().interactable = false;
                TitleText.color = Color.black;
                TimeLabel.color = Color.black;
                QuantityOfItem.color = Color.black;
            }

            //QuantityOfItem.text = quantity + "/" + ItemData.MaxCountOfThisItem;
        }



        public void lambo_UpdateQuantity(string mainNftID)
        {
            TimeLabel.text = userNftItems.UsedNftsAmount[int.Parse(mainNftID)] + "/" + userNftItems.nftsAmountInUsersWallet[int.Parse(mainNftID)];
        }


    }
}
