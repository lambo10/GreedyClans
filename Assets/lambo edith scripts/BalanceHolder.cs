using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
public class BalanceHolder : MonoBehaviour
{
    public static double c_bnbBalance;
    public static double c_gverseBalance;

    private void Start()
    {
        StartCoroutine(updateBalances(Helper.getBalances(playerDetails.usr_sessionID, playerDetails.usr_walletAddress, Helper.chainId, playerDetails.landNo)));
    }

    IEnumerator updateBalances(string url)
    {
        while (true)
        {
            yield return new WaitForSeconds(10);

            UnityWebRequest www = UnityWebRequest.Get(url);
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
                string response = www.downloadHandler.text;
            
                AllUserBalance userBalances = JsonUtility.FromJson<AllUserBalance>(response);

                if (userBalances.success != null && !userBalances.success)

                {


                }
                else
                {
                    GameObject trainedTroops = GameObject.FindGameObjectWithTag("TrainedTroopsBalance");
                    GameObject bnbBalance = GameObject.FindGameObjectWithTag("BnbBalance");
                    GameObject GverseBalance = GameObject.FindGameObjectWithTag("GverseBalance");

                    c_bnbBalance = double.Parse(userBalances.bnb_balance);
                    c_gverseBalance = double.Parse(userBalances.gverse_balance);

                    if (trainedTroops != null)
                    {
                        trainedTroops.GetComponent<TextMeshProUGUI>().text = "" + userBalances.trained_troops;
                    }

                    if (int.Parse(Helper.chainId) == 4000)
                    {
                        if (bnbBalance != null)
                        {

                            bnbBalance.GetComponent<TextMeshProUGUI>().text = userBalances.bnb_balance + " GOLD";
                        }

                        if (GverseBalance != null)
                        {

                            GverseBalance.GetComponent<TextMeshProUGUI>().text = userBalances.gverse_balance + " TGVERSE";
                        }
                    }
                    else { 
                    if (bnbBalance != null)
                    {

                        bnbBalance.GetComponent<TextMeshProUGUI>().text = userBalances.bnb_balance + " BNB";
                    }

                    if (GverseBalance != null)
                    {

                        GverseBalance.GetComponent<TextMeshProUGUI>().text = userBalances.gverse_balance + " GVERSE";
                    }
                }

                }

            }
        }
    }

    public static IEnumerator getUserBalances_routine()
    {
        string url = Helper.getBalances(playerDetails.usr_sessionID, playerDetails.usr_walletAddress, Helper.chainId, playerDetails.landNo);
        UnityWebRequest www = UnityWebRequest.Get(url);
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
            string response = www.downloadHandler.text;
            Debug.Log(response);
            AllUserBalance userBalances = JsonUtility.FromJson<AllUserBalance>(response);

            if (userBalances.success != null && !userBalances.success)

            {


            }
            else
            {
                GameObject trainedTroops = GameObject.FindGameObjectWithTag("TrainedTroopsBalance");
                GameObject bnbBalance = GameObject.FindGameObjectWithTag("BnbBalance");
                GameObject GverseBalance = GameObject.FindGameObjectWithTag("GverseBalance");

                if (trainedTroops != null)
                {
                    trainedTroops.GetComponent<TextMeshProUGUI>().text = "TrainedTroops: " + userBalances.trained_troops;
                }
                if (bnbBalance != null)
                {
                    if (int.Parse(Helper.chainId) == 4000)
                    {
                        bnbBalance.GetComponent<TextMeshProUGUI>().text = userBalances.bnb_balance + " GOLD";
                    }
                    else
                    {
                        bnbBalance.GetComponent<TextMeshProUGUI>().text = userBalances.bnb_balance + " BNB";
                    }
                }

                if (GverseBalance != null)
                {
                    if (int.Parse(Helper.chainId) == 4000)
                    {
                        GverseBalance.GetComponent<TextMeshProUGUI>().text = userBalances.gverse_balance + " TGVERSE";
                    }
                    else
                    {
                        GverseBalance.GetComponent<TextMeshProUGUI>().text = userBalances.gverse_balance + " GVERSE";
                    }
                }

            }

        }
    }



}