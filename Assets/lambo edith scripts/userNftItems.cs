using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class userNftItems : MonoBehaviour
{
    static string requestResult;
    static userNftItems instance;
    public static int[] nftsAmountInUsersWallet = new int[30];
    public static int[] UsedNftsAmount = new int[30];
    public static int noOfBarracks;
    public static int noArmyCamps;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        instance = this;
    }

    
    public static string getNftIndex (string nftName){
        string result = "";
        if (nftName.Equals("Army Camp"))
        {
            result = "0";
        }else if (nftName.Equals("Mega Army Camp"))
        {
            result = "1";
        }
        else if (nftName.Equals("Wood Wall") || nftName.Equals("WoodFenceNW") || nftName.Equals("WoodWall"))
        {
            result = "2"; 
        }
        else if (nftName.Equals("Stone Wall") || nftName.Equals("StoneWallNW") || nftName.Equals("StoneWall"))
        {
            result = "3";
        }
        else if (nftName.Equals("Cannon"))
        {
            result = "4";
        }
        else if (nftName.Equals("Archer Tower"))
        {
            result = "5";
        }
        else if (nftName.Equals("Bomb"))
        {
            result = "6";
        }
        else if (nftName.Equals("Air Mine"))
        {
            result = "7";
        }
        else if (nftName.Equals("Drone") || nftName.Equals("DronePad"))
        {
            result = "8";
        }
        else if (nftName.Equals("Catapult"))
        {
            result = "9";
        }
        else if (nftName.Equals("Barracks"))
        {
            result = "10";
        }
        else if (nftName.Equals("MegaBarracks"))
        {
            result = "11";
        }
        else if (nftName.Equals("Barbarian King"))
        {
            result = "12";
        }
        else if (nftName.Equals("Archer Queen"))
        {
            result = "13";
        }
        else if (nftName.Equals("Grand Warden"))
        {
            result = "14";
        }
        else if (nftName.Equals("Barbarian"))
        {
            result = "15";
        }
        else if (nftName.Equals("Archer"))
        {
            result = "16";
        }
        else if (nftName.Equals("Ent"))
        {
            result = "17";
        }
        else if (nftName.Equals("Goblin"))
        {
            result = "18";
        }
        else if (nftName.Equals("Wizard"))
        {
            result = "19";
        }
        else if (nftName.Equals("dragon"))
        {
            result = "20";
        }
        else if (nftName.Equals("Baby Dragon"))
        {
            result = "21";
        }
        else if (nftName.Equals("Golem"))
        {
            result = "22";
        }
        else if (nftName.Equals("Troll"))
        {
            result = "23";
        }
        else if (nftName.Equals("Ghost"))
        {
            result = "24";
        }
        else if (nftName.Equals("Griffin"))
        {
            result = "25";
        }
        else if (nftName.Equals("Peasant"))
        {
            result = "26";
        }
        else if (nftName.Equals("Fairy"))
        {
            result = "27";
        }
        else if (nftName.Equals("Toolhouse"))
        {
            result = "28";
        }
        else if (nftName.Equals("Land"))
        {
            result = "29";
        }

        return result;
        }


}
