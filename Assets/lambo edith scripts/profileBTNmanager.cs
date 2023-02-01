using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class profileBTNmanager : MonoBehaviour
{
    public TextMeshProUGUI walletAddress;
    public TextMeshProUGUI username;
    public TextMeshProUGUI land;

    private void Start()
    {
        var _walletAddress = playerDetails.usr_walletAddress;
        var _username = playerDetails.usr_username;
        
        land.text = "Land " + playerDetails.landNo;

        walletAddress.text = (_walletAddress.Substring(0, 1)+ _walletAddress.Substring(1, 1)+ _walletAddress.Substring(2, 1)+ _walletAddress.Substring(3, 1) + _walletAddress.Substring(4, 1) + "...." + _walletAddress.Substring(37, 1) + _walletAddress.Substring(38, 1) + _walletAddress.Substring(39, 1) + _walletAddress.Substring(40, 1) + _walletAddress.Substring(41, 1));
        
        if (_username.Length >= 15)
        {
            username.text = (_username.Substring(0, 1) + _username.Substring(1, 1) + _username.Substring(2, 1) + _username.Substring(3, 1) + _username.Substring(4, 1) + _username.Substring(5, 1) + _username.Substring(6, 1) + _username.Substring(7, 1) + _username.Substring(8, 1) + _username.Substring(9, 1) + _username.Substring(10, 1) + "....");
        }
        else
        {
            username.text = playerDetails.usr_username;
        }
    }
}
