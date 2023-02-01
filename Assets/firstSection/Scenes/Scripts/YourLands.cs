using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using textMeshPro
using TMPro;
using UnityEngine.Networking;

public class YourLands : MonoBehaviour
{
    // text mesh pro to display error message
    public TextMeshProUGUI errorMessage;
    string userSavedAddress;
    // text user address
    public TextMeshProUGUI userAddress;
    string userSeedPhrase;
    public GameObject land1, land2, land3, land4, land5;
    // Start is called before the first frame update
    string sessionID;
    public TextMeshProUGUI userName;
    public GameObject spiner;


    bool checke_network_reachability(UnityEngine.Events.UnityAction _fun)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            networkErroPanelManager.canShow = true;
            networkErroPanelManager._functionToExecute = _fun;
            return false;
        }
        else
        {
            networkErroPanelManager.canShow = false;
            return true;
        }
    }


    void Start()
    {
        userSeedPhrase = PlayerPrefs.GetString(Helper.userWalletDataKey);
        ImportWallet seedPhrase = JsonUtility.FromJson<ImportWallet>(userSeedPhrase);
        userSavedAddress = seedPhrase.eth_wallet_address;
        userAddress.text = seedPhrase.eth_wallet_address;

        var savedData2 = PlayerPrefs.GetString(Helper.userDetailsDataKey);
        loginResultsModel savedData2Jason = JsonUtility.FromJson<loginResultsModel>(savedData2);

        userName.text = savedData2Jason.username;
        sessionID = savedData2Jason.sessionID;

        

        getLands();
    }

    void getLands(){
        if (checke_network_reachability(getLands))
        {
            string getNFTUrl = Helper.getUser1155NFT(userSavedAddress, Helper.chainId, "29", sessionID);
            StartCoroutine(getMainNetTokenRequest(getNFTUrl));
        }
    }

      IEnumerator getMainNetTokenRequest(string url)
    {
        spiner.SetActive(true);
        // clear error message
        errorMessage.text = "";
        // unity web request
        UnityWebRequest www = UnityWebRequest.Get(url);
        // send the request
        yield return www.SendWebRequest();
        // check for errors
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            errorMessage.text = "Error: " + www.error;
        }
        else
        {
            // get the response
            string response = www.downloadHandler.text;
      
            TypicalJsonResponse userLands = JsonUtility.FromJson<TypicalJsonResponse>(response);
            if(userLands.success != null && !userLands.success){

                checkExpiredSession.checkSessionExpiration(userLands.msg);

                errorMessage.text = "Error: " + userLands.msg;
                errorMessage.color = Color.red;
            }else {
                switch (userLands.msg.Trim())
                {   
                    case "1":
                        land1.SetActive(true);
                        
                    break;
                    case "2":
                        land1.SetActive(true);
                        

                        land2.SetActive(true);
                        
                    break;
                    case "3":
                        land1.SetActive(true);
                        

                        land2.SetActive(true);
                        

                        land3.SetActive(true);
                        
                    break;
                    case "4":
                        land1.SetActive(true);
                        

                        land2.SetActive(true);
                        

                        land3.SetActive(true);
                        

                        land4.SetActive(true);
                        
                    break;
                    case "5":
                        land1.SetActive(true);
                        

                        land2.SetActive(true);
                        

                        land3.SetActive(true);
                        

                        land4.SetActive(true);
                        

                        land5.SetActive(true);
                        
                    break;
                    
                    default:
                        errorMessage.text =  $"{userLands.msg} Land{(userLands.msg != "0" ? "s" : "")} Available...";
                        errorMessage.color = Color.red;
                        openNoLand();
                    break;
                }
            }
            
        }
        spiner.SetActive(false);
    }

    public void openLand(int landNo)
    {
        playerDetails.landNo = landNo+"";
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void openNoLand()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("NoLand");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
