using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// unity web request
using UnityEngine.Networking;
// import TextMeshPro
using TMPro;
using UnityEngine.UI;
using System;

public class CreateWallet : MonoBehaviour
{
    // text 1 to 12 to display seed phrase
    public TextMeshProUGUI word1;
    public TextMeshProUGUI word2;
    public TextMeshProUGUI word3;
    public TextMeshProUGUI word4;
    public TextMeshProUGUI word5;
    public TextMeshProUGUI word6;
    public TextMeshProUGUI word7;
    public TextMeshProUGUI word8;
    public TextMeshProUGUI word9;
    public TextMeshProUGUI word10;
    public TextMeshProUGUI word11;
    public TextMeshProUGUI word12;
    // input field to enter seed phrase tm pro
    public TMP_InputField enterSeedPhrase;

    // TextMeshProUGUI to display error message
    public TextMeshProUGUI errorMessage;

    public GameObject createBTN;
    public Color createBTN_Enable_color;
    public Color createBTN_disable_color;
    public GameObject loadingIcon;
    Button createBTNbuttonObj;
    Image createBTNimg;

    string userSeedPhrase;

     string sceneName;

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

    void Awake()
     {
         // get the seed phrase from the previous scene
         userSeedPhrase = PlayerPrefs.GetString(Helper.userWalletDataKey);
         sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (createBTN != null)
        {
            createBTNbuttonObj = createBTN.GetComponent<Button>();
            createBTNimg = createBTN.GetComponent<Image>();
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        startLayer2();
    }

    void startLayer2()
    {
        // get scene name

        if (sceneName == "CreateWallet")
        {
            // Call the CreateWallet API
            if (checke_network_reachability(startLayer2))
            {
                StartCoroutine(createSeedPhraseRequest());
            }
        }
    }

    public void navigateToConnectWallet(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("ConnectWallet");
    }

    // send a request to the server to create a seed phrase
    public void createSeedPhrase()
    {
        if (checke_network_reachability(createSeedPhrase))
        {
            StartCoroutine(createSeedPhraseRequest());
        }
    }

    // import wallet 
    public void importWalletWithPrivateKey()
    {
        if (sceneName == "ImportWalletWithPrivateKey")
        {
            userSeedPhrase = enterSeedPhrase.text;
            print(userSeedPhrase);
        }
        if (userSeedPhrase == null) return;
        // import wallet
        if (checke_network_reachability(importWalletWithPrivateKey))
        {
            StartCoroutine(importWalletWithPrivateKeyRequest());
        }
    }



    // send a request to the server to create a seed phrase
    IEnumerator importWalletWithPrivateKeyRequest()
    {
        Debug.Log(Helper.importWalletWithPrivateKeyUrl + userSeedPhrase);
        diacrtivateCreateBtn();
        // clear error message
        errorMessage.text = "";
        // unity web request
        UnityWebRequest www = UnityWebRequest.Get(Helper.importWalletWithPrivateKeyUrl + userSeedPhrase);
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
            ImportWallet seedPhrase = JsonUtility.FromJson<ImportWallet>(response);
            if (seedPhrase.eth_wallet_address != null)
            {
                PlayerPrefs.SetString(Helper.userWalletDataKey, response);
                stateUserDetails.usrWalletAddress = seedPhrase.bsc_wallet_address;
                // navigate to game mode
                UnityEngine.SceneManagement.SceneManager.LoadScene("SignUp");
            }
            else
            {
                Debug.Log(seedPhrase.msg);
                errorMessage.text = "Error: " + seedPhrase.msg;
            }

        }
        activateCreateBtn();

    }



    // import wallet 
    public void importWallet()
    {
         if(sceneName == "ImportWallet"){ 
            userSeedPhrase = enterSeedPhrase.text;
            print(userSeedPhrase);
         }
        if(userSeedPhrase == null) return;
        // import wallet
        if (checke_network_reachability(importWallet))
        {
            StartCoroutine(importWalletRequest());
        }
    }



    // send a request to the server to create a seed phrase
    IEnumerator importWalletRequest()
    {
        diacrtivateCreateBtn();
        // clear error message
        errorMessage.text = "";
        // unity web request
        UnityWebRequest www = UnityWebRequest.Get(Helper.importWalletUrl + userSeedPhrase);
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
            ImportWallet seedPhrase = JsonUtility.FromJson<ImportWallet>(response);
            if(seedPhrase.eth_wallet_address != null){
                PlayerPrefs.SetString(Helper.userWalletDataKey, response);
                stateUserDetails.usrWalletAddress = seedPhrase.bsc_wallet_address;
                // navigate to game mode
                UnityEngine.SceneManagement.SceneManager.LoadScene("SignUp");
            }else {
                errorMessage.text = "Error: " + seedPhrase.msg;
            }
            
        }
        activateCreateBtn();

    }
    
    // send a request to the server to create a seed phrase
    IEnumerator createSeedPhraseRequest()
    {
        diacrtivateCreateBtn();
        // clear error message
        errorMessage.text = "";
        // unity web request
        UnityWebRequest www = UnityWebRequest.Get(Helper.createWalletUrl);
        // send the request
        yield return www.SendWebRequest();
        // check for errors
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            // set error message color
            
            // display error message
            errorMessage.text = "Error: " + www.error;
        }
        else
        {
            // get the response
            string response = www.downloadHandler.text;
            // to json
            CreateWalletResponse seedPhrase = JsonUtility.FromJson<CreateWalletResponse>(response);
            userSeedPhrase = seedPhrase.seed;
            // split the response into an array of words
            string[] words = userSeedPhrase.Split(' ');
            // assign each text
            word1.text = words[0];
            word2.text = words[1];
            word3.text = words[2];
            word4.text = words[3];
            word5.text = words[4];
            word6.text = words[5];
            word7.text = words[6];
            word8.text = words[7];
            word9.text = words[8];
            word10.text = words[9];
            word11.text = words[10];
            word12.text = words[11];
            activateCreateBtn();
        }

    }

    void activateCreateBtn()
    {
        createBTNbuttonObj.enabled = true;
        createBTNimg.color = createBTN_Enable_color;
        if (loadingIcon != null) {
            loadingIcon.SetActive(false);
        }
    }

    void diacrtivateCreateBtn()
    {
        createBTNbuttonObj.enabled = false;
        createBTNimg.color = createBTN_disable_color;
        if (loadingIcon != null)
        {
            loadingIcon.SetActive(true);
        }
    }

    public void copySeedPhrase()
    {
        TextEditor editor = new TextEditor
        {
            text = userSeedPhrase
        };
        editor.SelectAll();
        editor.Copy();
        showToast("Copied", 2);
    }

    void showToast(string text,
       int duration)
    {
        StartCoroutine(showToastCOR(text, duration));
    }

    private IEnumerator showToastCOR(string text,
        int duration)
    {
        Color orginalColor = errorMessage.color;

        errorMessage.text = text;
        errorMessage.enabled = true;

        //Fade in
        yield return fadeInAndOut(errorMessage, true, 0.5f);

        //Wait for the duration
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Fade out
        yield return fadeInAndOut(errorMessage, false, 0.5f);

        errorMessage.enabled = false;
        errorMessage.color = orginalColor;
    }

    IEnumerator fadeInAndOut(TextMeshProUGUI targetText, bool fadeIn, float duration)
    {
        //Set Values depending on if fadeIn or fadeOut
        float a, b;
        if (fadeIn)
        {
            a = 0f;
            b = 1f;
        }
        else
        {
            a = 1f;
            b = 0f;
        }

        Color currentColor = Color.white;
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

// json response class
public class CreateWalletResponse
{
    public string seed;
}


public class ImportWallet {
    public string mnemonic;
    public string msg;
    public string eth_wallet_address;
    public string eth_wallet_privateKey;
    public string bsc_wallet_address;
    public string bsc_wallet_privateKey;
}
public class TypicalJsonResponse
{
    public string msg;
    public bool success;
}
public class getNftAmountLeft_and_price
{
    public string amountLeft;
    public string price;
    public string msg;
    public bool success;
}
public class getNftDetails_and_usedAmount
{
    public string msg;
    public string usedAmount;
    public bool success;
}
public class get_train_warrior
{
    public string msg;
    public bool can_train;
    public bool success;
}
public class ItemOnMapResponse
{
    public string msg;
    public bool success;
    public string cloneName;
}
public class getAliveOrDead
{
    public string health;
    public string alive;
    public string dead;
    public string msg;
    public bool success;
}
public class AllUserBalance
{
    public bool success;
    public string trained_troops;
    public string bnb_balance;
    public string gverse_balance;
}
public class check_app_new_version
{
    public bool success;
    public string msg;
    public string install_link;
}
public class notificationsData
{
    public notificationsMsg[] msg;
    public bool success;
}
[Serializable]
public class notificationsMsg
{
    public string msg;
    public string btn_active;
    public string btn_name;
    public string btn_link;
}
public class find_and_match
{
    public string msg;
    public string instanceID;
    public bool success;
}
public class battleMapJsonHandler
{
    public bool success;
    public string msg;
    public string opponentAddress;
    public string opponentMap;
    public string playerAddress;
    public string playerArmy;
}
public class server_weather
{
    public string winterSwitchCounter;
    public bool isWinter;
    public string transitionInterval;
    public string transitionSpeed;
    public bool firstTime;
    public string wetherPathern;
    public string msg;
    public bool success;
}
public class armyInit
{
    public bool success;
    public string playerArmy;
    public string msg;
}
public class profileDetailsResult
{
    public bool success;
    public string username;
    public string email;
    public string sessionID;
    public string msg;
}

[Serializable]
public class armyJsonArry
{
    public int[] armyCamp;
}