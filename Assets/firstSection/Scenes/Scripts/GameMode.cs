using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using textMeshPro
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameMode : MonoBehaviour
{
    // text mesh pro to display error message
    public TextMeshProUGUI errorMessage;
    string userSeedPhrase;
    // text user address
    public TextMeshProUGUI userAddress;
    string userSavedAddress;
    // text user name
    public TextMeshProUGUI userName;
    // session id
    string sessionID;
    // Start is called before the first frame update
    public GameObject mainetloadingIcon;
    public GameObject offchainloadingIcon;
    public Button mainNetBTN;
    public Button offChainBTN;
    public GameObject welcomePackage;


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
        // get user seed phrase
        userSeedPhrase = PlayerPrefs.GetString(Helper.userWalletDataKey);
        ImportWallet seedPhrase = JsonUtility.FromJson<ImportWallet>(userSeedPhrase);
        userSavedAddress = seedPhrase.bsc_wallet_address;
        userAddress.text = seedPhrase.bsc_wallet_address;

        var savedData2 = PlayerPrefs.GetString(Helper.userDetailsDataKey);
        loginResultsModel savedData2Jason = JsonUtility.FromJson<loginResultsModel>(savedData2);

        userName.text = savedData2Jason.username;
        sessionID = savedData2Jason.sessionID;
        check_firstTimeOpen();
    }

    void check_firstTimeOpen()
    {
       var isFirstRun = PlayerPrefs.GetInt("recived_firstTime_run_package", 1) == 1;

        if (isFirstRun && Helper.logedInFromSignUp)
        {
            welcomePackage.SetActive(true);

            PlayerPrefs.SetInt("recived_firstTime_run_package", 0);
        }
    }

    public void closeWelcomePackage()
    {
        welcomePackage.SetActive(false);
    }

    public void offChain()
    {
        Helper.chainId = "4000";
        string getNFTUrl = Helper.getUser1155NFT(userSavedAddress, Helper.chainId, "29", sessionID);
        if (checke_network_reachability(offChain))
        {
            StartCoroutine(getoffChainBTNTokenRequest(getNFTUrl));
        }
    }

    public void mainNet(){
        Helper.chainId = "97"; // MainNet:56 TestNet:97
        string getNFTUrl = Helper.getUser1155NFT(userSavedAddress, Helper.chainId, "29", sessionID);
        if (checke_network_reachability(mainNet))
        {
            StartCoroutine(getMainNetTokenRequest(getNFTUrl));
        }
    }




    IEnumerator getoffChainBTNTokenRequest(string url)
    {
        offchainloadingIcon.SetActive(true);
        offChainBTN.enabled = false;
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
            if (userLands.success != null && !userLands.success)
            {

                checkExpiredSession.checkSessionExpiration(userLands.msg);

                errorMessage.text = "Error: " + userLands.msg;
                errorMessage.color = Color.red;


            }
            else
            {

                if (userLands.msg == "0")
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("NoLand");
                }
                else
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("YourLands");
                }
            }

        }
        offchainloadingIcon.SetActive(true);
        offChainBTN.enabled = true;
    }




    IEnumerator getMainNetTokenRequest(string url)
    {
        mainetloadingIcon.SetActive(true);
        mainNetBTN.enabled = false;
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

                if(userLands.msg == "0"){
                    UnityEngine.SceneManagement.SceneManager.LoadScene("NoLand");
                }else {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("YourLands");
                }
            }
            
        }
        mainetloadingIcon.SetActive(true);
        mainNetBTN.enabled = true;
    }

    public void testNet(){

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
