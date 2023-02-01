using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class buyBTN : MonoBehaviour
{
	public int nftIndex;
	public GameObject buyNftModal;
	public lb_nfts_details _nfts;
	GameObject input;
	GameObject woodPlain;
	GameObject quantityLeft;
	GameObject buySpinner6;
	TextMeshProUGUI msgDisplay;
	Color32 buyBtnColor = new Color32(255, 255, 255, 255);
	TMPro.TMP_InputField enterQuantity;
	ItemCountingMax itemCountingMax;
	lb_nft nftDetails;
	TextMeshProUGUI bnbBalanceTXT;


	private void Start()
    {
		GameObject GameManagerLb = GameObject.FindGameObjectsWithTag("lbGameManager")[0];
		 itemCountingMax = GameManagerLb.GetComponent<ItemCountingMax>();
	}

    private void init()
	{
		input = GameObject.Find("buyNftWindow/ModalBackground/DescriptionPanel/Group_Center/").gameObject;
		woodPlain = GameObject.Find("buyNftWindow/ModalBackground/DescriptionPanel/").gameObject;
		quantityLeft = woodPlain.transform.Find("Group_Left/quantityLeft").gameObject;
		buySpinner6 = input.transform.Find("Spinner 6").gameObject;
		msgDisplay = input.transform.Find("MsgErr").gameObject.GetComponent<TextMeshProUGUI>();
		enterQuantity = input.transform.Find("quantity").gameObject.GetComponent<TMPro.TMP_InputField>();

		bnbBalanceTXT = woodPlain.transform.Find("Top/StatusBar_Group/Stats_Gold/Text_Value").gameObject.GetComponent<TMPro.TextMeshProUGUI>();

	}

    void Update()
    {

        if (bnbBalanceTXT != null)
        {
			if(int.Parse(Helper.chainId) == 4000)
            {
				bnbBalanceTXT.text = BalanceHolder.c_bnbBalance + " GOLD";
			}
            else
            {
				bnbBalanceTXT.text = BalanceHolder.c_bnbBalance + " BNB";
			}
			
		}

	}

    public void click()
	{


		nftDetails = _nfts.nfts[nftIndex];
		var info_M_Manager = buyNftModal.GetComponent<buyNftWindow_modal_manager>();
		info_M_Manager.passDetails(nftDetails.name, nftDetails.health, nftDetails.damage, nftDetails.speed, nftDetails.range, nftDetails.id, nftDetails.fullBodyShinnyIcon, nftDetails.description);

		buyNftModal.SetActive(true);
		init();
		input.SetActive(false);
		enterQuantity.text = "";
		msgDisplay.text = "";
		msgDisplay.gameObject.SetActive(false);
		quantityLeft.SetActive(false);
		buySpinner6.SetActive(false);
		input.transform.Find("BottomMenu/buyBTN/cost").gameObject.SetActive(false);
		woodPlain.transform.Find("Spinner 6").gameObject.SetActive(true);
		StartCoroutine(checkItemCount(nftDetails.id));
		
	}
	IEnumerator mintNftRequest(string url,int nftID)
	{
		Debug.Log(url);
		// unity web request
		UnityWebRequest www = UnityWebRequest.Get(url);
		// send the request
		yield return www.SendWebRequest();
		buySpinner6.SetActive(false);
		Button buyButton = input.transform.Find("BottomMenu/buyBTN").gameObject.GetComponent<Button>();

		buyButton.gameObject.GetComponent<Image>().color = buyBtnColor;
		buyButton.gameObject.SetActive(true);
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
			msgDisplay.text = "";
			msgDisplay.gameObject.SetActive(true);
			if (reqResponse.success != null && !reqResponse.success)
			{
				if (reqResponse.msg.Equals("rE"))
				{
					
				}else
                {
					if ((userNftItems.nftsAmountInUsersWallet[nftID] >= 40) && (nftID == 1 || nftID == 4 || nftID == 5 || nftID == 6 || nftID == 7 || nftID == 9 || nftID == 10 || nftID == 11 || nftID == 12 || nftID == 13 || nftID == 15 || nftID == 16 || nftID == 17 || nftID == 18 || nftID == 19 || nftID == 23 || nftID == 24 || nftID == 25 || nftID == 26 || nftID == 27 || nftID == 28 || nftID == 29))
                    {
						msgDisplay.text = "Maximum of this item a wallet can hold is 40";
						msgDisplay.color = Color.red;
					}else if ((userNftItems.nftsAmountInUsersWallet[nftID] >= 30) && (nftID == 2 || nftID == 3))
                    {
						msgDisplay.text = "Maximum walls a wallet can hold is 30";
						msgDisplay.color = Color.red;
					}else if ((userNftItems.nftsAmountInUsersWallet[nftID] >= 2) && (nftID == 20 || nftID == 21))
					{
						msgDisplay.text = "Maximum of this item a wallet can hold is 30";
						msgDisplay.color = Color.red;
					}else if ((userNftItems.nftsAmountInUsersWallet[nftID] >= 3) && (nftID == 8 || nftID == 14 || nftID == 22))
                    {
						msgDisplay.text = "Maximum of this item a wallet can hold is 3";
						msgDisplay.color = Color.red;
                    }
                    else
                    {
						msgDisplay.text = "";
						msgDisplay.color = Color.red;
						msgDisplay.text = "" + reqResponse.msg;
					}

					if(int.Parse(Helper.chainId) == 4000)
                    {
						input.transform.Find("Top/StatusBar_Group/Stats_Gold/Text_Value").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = BalanceHolder.c_bnbBalance + " GOLD";
					}else
					{
						input.transform.Find("Top/StatusBar_Group/Stats_Gold/Text_Value").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = BalanceHolder.c_bnbBalance + " BNB";
					}

				}
			}
			else
			{
				msgDisplay.text = "";
				msgDisplay.color = new Color32(104, 195, 163, 255);
				msgDisplay.text = "Mint Successful.";

				StartCoroutine(checkItemCount(nftDetails.id));
				StartCoroutine(BalanceHolder.getUserBalances_routine());
			}
		}
	}

	public IEnumerator checkItemCount(string id)
	{
		string getNFTcount = Helper.getNftAmountLeftAndPrice(playerDetails.usr_walletAddress, Helper.chainId, id, playerDetails.usr_sessionID);

		Button buyButton = input.transform.Find("BottomMenu/buyBTN").gameObject.GetComponent<Button>();
		
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
			
			getNftAmountLeft_and_price reqResponse = JsonUtility.FromJson<getNftAmountLeft_and_price>(response);
			input.SetActive(true);
			
			if (reqResponse.success != null && !reqResponse.success)
			{

			}
			else
			{
				var price = double.Parse(reqResponse.price);
				buyButton.onClick.RemoveAllListeners();
				buyButton.onClick.AddListener(() => {
					try
					{
						var quantityEntered = double.Parse(enterQuantity.text);
						buyButton.gameObject.GetComponent<Image>().color = Color.black;
						buyButton.gameObject.SetActive(false);
						string mintNFTUrl = Helper.mintNft(playerDetails.usr_walletAddress, Helper.chainId, id, playerDetails.usr_sessionID, price * quantityEntered + "", enterQuantity.text, playerDetails.usr_private_key);
						buySpinner6.SetActive(true);
						msgDisplay.text = "";
						StartCoroutine(mintNftRequest(mintNFTUrl, int.Parse(id)));
						msgDisplay.gameObject.SetActive(false);

					}
					catch (Exception e)
					{

					}

				});
				buyButton.gameObject.GetComponent<Image>().color = buyBtnColor;
				buyButton.gameObject.SetActive(true);

				enterQuantity.onValueChanged.RemoveAllListeners();
				enterQuantity.onValueChanged.AddListener(value =>
				{
					try
					{
                        if (int.Parse(Helper.chainId) == 4000)
                        {
							input.transform.Find("BottomMenu/buyBTN/cost").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0:0.000}", price * double.Parse(value)) + " GOLD";
						}
                        else
                        {
							input.transform.Find("BottomMenu/buyBTN/cost").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0:0.000}", price * double.Parse(value)) + " BNB";
						}
						
						input.transform.Find("BottomMenu/buyBTN/cost").gameObject.SetActive(true);
					}
					catch (Exception e)
					{

					}

				});

				quantityLeft.SetActive(true);
				quantityLeft.GetComponent<TMPro.TextMeshProUGUI>().text = reqResponse.amountLeft;


				if (reqResponse.amountLeft == "0")
				{
					input.transform.Find("warning").gameObject.SetActive(true);

				}
				else
				{
					input.transform.Find("warning").gameObject.SetActive(false);
				}
			}

		}

		yield return null;

	}
}