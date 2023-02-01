using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerDetails : MonoBehaviour
{
    public static string usr_walletAddress;
    public static string usr_email;
    public static string usr_username;
    public static string usr_land;
    public static string usr_sessionID;
    public static string usr_seed_phrase;
    public static string usr_private_key;
    public static string landNo;
    public static bool canShowMap = false;
    public static string erroMsg = "";

    void Awake()
    {
        try
        {
            var savedData1 = PlayerPrefs.GetString(Helper.userWalletDataKey);
            ImportWallet savedData1Jason = JsonUtility.FromJson<ImportWallet>(savedData1);
            usr_walletAddress = savedData1Jason.bsc_wallet_address;
            usr_seed_phrase = savedData1Jason.mnemonic;
            usr_private_key = savedData1Jason.bsc_wallet_privateKey;


            var savedData2 = PlayerPrefs.GetString(Helper.userDetailsDataKey);
            loginResultsModel savedData2Jason = JsonUtility.FromJson<loginResultsModel>(savedData2);
            usr_email = savedData2Jason.email;
            usr_username = savedData2Jason.username;
            usr_sessionID = savedData2Jason.sessionID;

            StartCoroutine(BalanceHolder.getUserBalances_routine());

            
        }
        catch (Exception i)
        {
    
        }
    }

    void Update()
    {
        if (canShowMap) {
            this.gameObject.SetActive(false);
        }
    }

}


public class loginResultsModel
{
    public string msg;
    public string email;
    public string username;
    public string sessionID;
    public string password;
}