using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NoLand : MonoBehaviour
{
    // text user address
    public TextMeshProUGUI userAddress;
    string userSavedAddress;
    public TextMeshProUGUI cost;
    public TextMeshProUGUI userName;
    public GameObject amountLeftObj;
    TextMeshProUGUI amountLeft;
    public GameObject warning;
    string userSeedPhrase;
    string sessionID;
    public int landNFTID = 29;
    public Color amountLeftNormalColor;
    public GameObject spinner;
    public GameObject mint_spinner;
    public TMP_InputField amount;
    double landPrice;
    public TextMeshProUGUI erromsg;
    private bool canMint = false;
    private double purchaseCost;
    private string usrPrivateKey;
    public TextMeshProUGUI bnbBalance;
    public GameObject bnbBalanceSpinner;
    public GameObject buyBTNObj;
    Button buyBTN;
    Image buyBTNimg;
    public Color buyBTNactiveColor;
    public Color buyBTNinactiveColor;
    public copyWalletAddress _copyWalletAddress;
    public tutorialManager _tutorialManager;

    // Start is called before the first frame update
    void Start()
    {
        userSeedPhrase = PlayerPrefs.GetString(Helper.userWalletDataKey);
        ImportWallet seedPhrase = JsonUtility.FromJson<ImportWallet>(userSeedPhrase);
        userSavedAddress = seedPhrase.bsc_wallet_address;
        userAddress.text = seedPhrase.bsc_wallet_address;
        usrPrivateKey = seedPhrase.bsc_wallet_privateKey;

        _copyWalletAddress.walletAddress = seedPhrase.bsc_wallet_address;

        var savedData2 = PlayerPrefs.GetString(Helper.userDetailsDataKey);
        loginResultsModel savedData2Jason = JsonUtility.FromJson<loginResultsModel>(savedData2);

        userName.text = savedData2Jason.username;
        sessionID = savedData2Jason.sessionID;

        amountLeft = amountLeftObj.GetComponent<TextMeshProUGUI>();
        buyBTNimg = buyBTNObj.GetComponent<Image>();
        buyBTN = buyBTNObj.GetComponent<Button>();
    }

    public void openRefinery(){
        Application.OpenURL(Helper.refineryURL);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void unSetCanMint()
    {
        canMint = false;
    }

    public void calcCost()
    {
        try
        {
            double costValue = landPrice * int.Parse(amount.text);
            if (int.Parse(Helper.chainId) != 4000)
            {
                cost.text = costValue + " BNB";
            }
            else
            {
                cost.text = costValue + " GOLD";
            }

            canMint = true;
            purchaseCost = costValue;
        }
        catch(Exception e)
        {
            Debug.Log("Exception calc cost =>"+e);
        }
    }

    public void getNftAmountLeft()
    {
        string getNFTUrl = Helper.getNftAmountLeftAndPrice(userSavedAddress, Helper.chainId, "29", sessionID);
        StartCoroutine(getNftAmountLeftRequest(getNFTUrl));
    }


    IEnumerator getNftAmountLeftRequest(string url)
    {
        deactivateBuyBTN();
        amountLeftObj.SetActive(false);
        amountLeft.text = "";
        spinner.SetActive(true);
        // unity web request
        UnityWebRequest www = UnityWebRequest.Get(url);
        // send the request
        yield return www.SendWebRequest();
        // check for errors
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            amountLeft.text = "Amount left: Error => " + www.error;
        }
        else
        {
            // get the response
            string response = www.downloadHandler.text;
            
            getNftAmountLeft_and_price userLands = JsonUtility.FromJson<getNftAmountLeft_and_price>(response);
            if (userLands.success != null && !userLands.success)
            {
         
                checkExpiredSession.checkSessionExpiration(userLands.msg);

                spinner.SetActive(false);
                amountLeft.text = "Amount left: Error => " + userLands.msg;
                amountLeft.color = Color.red;
                amountLeftObj.SetActive(true);

            }
            else
            {

                if (userLands.amountLeft == "0")
                {
                    spinner.SetActive(false);
                    amountLeftObj.SetActive(false);
                    warning.SetActive(true);
                    
                }
                else
                {
                    spinner.SetActive(false);
                    amountLeft.text = "Amount left: " + userLands.amountLeft;
                    amountLeft.color = amountLeftNormalColor;
                    amountLeftObj.SetActive(true);

                }
                landPrice = Convert.ToDouble(userLands.price);
                erromsg.text = "";
                calcCost();
            }

        }
        activateBuyBTN();
    }

    public void mintNft()
    {
        if (canMint) {
        string mintNFTUrl = Helper.mintNft(userSavedAddress, Helper.chainId, "29", sessionID, purchaseCost+"", amount.text, usrPrivateKey);
        StartCoroutine(mintNftRequest(mintNFTUrl));
        }
        else
        {
            erromsg.text = "Getting nft price";
            erromsg.color = Color.red;
        }
    }

    IEnumerator mintNftRequest(string url)
    {
        deactivateBuyBTN();
        erromsg.text = "";
        mint_spinner.SetActive(true);
        // unity web request
        UnityWebRequest www = UnityWebRequest.Get(url);
        // send the request
        yield return www.SendWebRequest();
        // check for errors
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            erromsg.text = "Error => " + www.error;
        }
        else
        {
            // get the response
            string response = www.downloadHandler.text;
            Debug.Log(url);
            TypicalJsonResponse userLands = JsonUtility.FromJson<TypicalJsonResponse>(response);

          
            if (userLands.success != null && !userLands.success)
            {

                checkExpiredSession.checkSessionExpiration(userLands.msg);

                mint_spinner.SetActive(false);

                erromsg.text = "Error => " + userLands.msg;
                erromsg.color = Color.red;

                Debug.Log("Error => " + userLands.msg);

            }
            else
            {
                erromsg.text = "Mint Successfull ";
                erromsg.color = Color.green;
                mint_spinner.SetActive(false);
                getBNBbalance();


            }

        }
        activateBuyBTN();
    }


    public void getBNBbalance()
    {
        string getNFTUrl = Helper.getBNBbalance(userSavedAddress, Helper.chainId, sessionID);
        StartCoroutine(getBNBbalanceRequest(getNFTUrl));
    }

    IEnumerator getBNBbalanceRequest(string url)
    {
        bnbBalance.text = "";
        bnbBalanceSpinner.SetActive(true);
        // unity web request
        UnityWebRequest www = UnityWebRequest.Get(url);
        // send the request
        yield return www.SendWebRequest();
        // check for errors
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            bnbBalance.text = "Error => " + www.error;
        }
        else
        {
            // get the response
            string response = www.downloadHandler.text;

            TypicalJsonResponse reqResponse = JsonUtility.FromJson<TypicalJsonResponse>(response);


            if (reqResponse.success != null && !reqResponse.success)
            {

                checkExpiredSession.checkSessionExpiration(reqResponse.msg);

                bnbBalanceSpinner.SetActive(false);

                bnbBalance.text = "Error => " + reqResponse.msg;
                bnbBalance.color = Color.red;

            }
            else
            {
                if (int.Parse(Helper.chainId) == 4000)
                {
                    bnbBalance.text = "GOLD Balance: " + RoundToSignificantDigits(Double.Parse(reqResponse.msg), 3);
                }
                else
                {
                    bnbBalance.text = "BNB Balance: " + RoundToSignificantDigits(Double.Parse(reqResponse.msg), 3) + " BNB";
                }
                
                bnbBalanceSpinner.SetActive(false);

            }

        }
    }

    void activateBuyBTN()
    {
        //buyBTN.enabled = true;
        //buyBTNimg.color = buyBTNactiveColor;
        buyBTNObj.SetActive(true);


    }

    void deactivateBuyBTN()
    {
        //buyBTN.enabled = false;
        //buyBTNimg.color = buyBTNinactiveColor;
        buyBTNObj.SetActive(false);


    }


    double RoundToSignificantDigits(double d, int digits)
    {
        if (d == 0)
            return 0;

        double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
        return scale * Math.Round(d / scale, digits);
    }


}
