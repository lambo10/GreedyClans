using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
// unity web request
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class Login : MonoBehaviour
{
    string userAddress;
    string userSeedPhrase;
    // input field to enter password
    public TMP_InputField enterPassword;
    // text mesh pro to display error message
    public TextMeshProUGUI errorMessage;

    public GameObject loginBTN;
    public Color loginBTN_Enable_color;
    public Color loginBTN_disable_color;
    public GameObject loadingIcon;
    Button loginBTNbuttonObj;
    Image loginBTNimg;

    void Awake()
    {
        if (loginBTN != null)
        {
            loginBTNbuttonObj = loginBTN.GetComponent<Button>();
            loginBTNimg = loginBTN.GetComponent<Image>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        userSeedPhrase = PlayerPrefs.GetString(Helper.userWalletDataKey);
        ImportWallet seedPhrase = JsonUtility.FromJson<ImportWallet>(userSeedPhrase);
        userAddress = seedPhrase.bsc_wallet_address;
    }


    public void loginInUser(){
        // make sure all fields are filled
        if(enterPassword.text == ""){
            errorMessage.text = "Please fill in all fields";
        }
        else{
            // create a new user
            StartCoroutine(loginInRequest());
        }
    }

    IEnumerator loginInRequest()
    {
        diacrtivateloginBTN();
        // clear error message
        errorMessage.text = "";
        // unity web request
        UnityWebRequest www = UnityWebRequest.Get($"{Helper.loginUrl}?address={userAddress}&password={enterPassword.text}");
  
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
      
            TypicalJsonResponse userLoggedIn = JsonUtility.FromJson<TypicalJsonResponse>(response);
            if(!userLoggedIn.success){
                errorMessage.text = "Error: " + userLoggedIn.msg;
                errorMessage.color = Color.red;

                if (userLoggedIn.msg.Equals("Email not verified"))
                {
                    navigateToVerifyEmail();
                }


            }else {
                errorMessage.text = userLoggedIn.msg;
                // color to green
                errorMessage.color = Color.green;

                string storeString = AddPasswordToJson(response,enterPassword.text);
                PlayerPrefs.SetString(Helper.userDetailsDataKey, storeString);
                navigateToGameMode();
            }
            
        }
        activateloginBTN();

    }

    void activateloginBTN()
    {
        loginBTNbuttonObj.enabled = true;
        loginBTNimg.color = loginBTN_Enable_color;
        if (loadingIcon != null)
        {
            loadingIcon.SetActive(false);
        }
    }

    void diacrtivateloginBTN()
    {
        loginBTNbuttonObj.enabled = false;
        loginBTNimg.color = loginBTN_disable_color;
        if (loadingIcon != null)
        {
            loadingIcon.SetActive(true);
        }
    }

    public void navigateToVerifyEmail(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("VerifyEmail");
    }

    public void navigateToGameMode()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameMode");
    }

    public void navigateToForgotPassword()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("forgotPassword");
    }

    public void navigateToSignUp(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("SignUp");
    }

    public string AddPasswordToJson(string input_json_string, string password)
    {
        // Parse the input_json_string into a JObject
        JObject inputJson = JObject.Parse(input_json_string);

        // Add a new property called "password" to the JObject
        inputJson["password"] = password;

        // Return the modified JObject as a string
        return inputJson.ToString();
    }
}
