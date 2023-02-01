using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using textMeshPro
using TMPro;
using UnityEngine.UI;
// unity web request
using UnityEngine.Networking;
//

public class SignUp : MonoBehaviour
{
    // text mesh pro to display error message
    public TextMeshProUGUI errorMessage;
    // input field to enter email
    public TMP_InputField enterEmail;
    // input field to enter password
    public TMP_InputField enterPassword;
    // input field to enter username
    public TMP_InputField enterUsername;
    // 
    // Start is called before the first frame update
    string userAddress;
    string userSeedPhrase;

    public GameObject signupBTN;
    public Color signupBTN_Enable_color;
    public Color signupBTN_disable_color;
    public GameObject loadingIcon;
    Button signupBTNbuttonObj;
    Image signupBTNimg;


    void Awake()
    {
        if (signupBTN != null)
        {
            signupBTNbuttonObj = signupBTN.GetComponent<Button>();
            signupBTNimg = signupBTN.GetComponent<Image>();
        }
    }


    void Start()
    {
        userSeedPhrase = PlayerPrefs.GetString(Helper.userWalletDataKey);
        ImportWallet seedPhrase = JsonUtility.FromJson<ImportWallet>(userSeedPhrase);
        userAddress = seedPhrase.eth_wallet_address;
    }

    public void navigateToLogin(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
    }

    public void signUpUser(){
        // make sure all fields are filled
        if(enterEmail.text == "" || enterPassword.text == "" || enterUsername.text == ""){
            errorMessage.text = "Please fill in all fields";
        }
        else{
            // create a new user
            StartCoroutine(signUpRequest());
        }
    }

    // coroute to send a request to the server to sign up
    IEnumerator signUpRequest()
    {
        diacrtivatesignupBTN();
        // clear error message
        errorMessage.text = "";
        // unity web request
        UnityWebRequest www = UnityWebRequest.Get($"{Helper.signupUrl}?address={userAddress}&password={enterPassword.text}&email={enterEmail.text}&username={enterUsername.text}");
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
      
            TypicalJsonResponse userCreated = JsonUtility.FromJson<TypicalJsonResponse>(response);
            if(!userCreated.success){
                errorMessage.text = "Error: " + userCreated.msg;
                errorMessage.color = Color.red;
            }else {
                errorMessage.text = userCreated.msg;
                // color to green
                errorMessage.color = Color.green;
                StartCoroutine(sendVerificationCodeRequest());
                
            }
            
        }
        activatesignupBTN();

    }


    IEnumerator sendVerificationCodeRequest()
    {
        // clear error message
        errorMessage.text = "";
        // unity web request
        UnityWebRequest www = UnityWebRequest.Get($"{Helper.send_verification_email}?email={enterEmail.text}");
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
            if (!userLoggedIn.success)
            {
                errorMessage.text = "Error: " + userLoggedIn.msg;
                errorMessage.color = Color.red;
            }
            else
            {
                errorMessage.text = userLoggedIn.msg;
                // color to green
                errorMessage.color = Color.green;
                stateUserDetails.usrEmail = enterEmail.text;
                navigateToVerifyEmail();
            }

        }
    }



    void activatesignupBTN()
    {
        signupBTNbuttonObj.enabled = true;
        signupBTNimg.color = signupBTN_Enable_color;
        if (loadingIcon != null)
        {
            loadingIcon.SetActive(false);
        }
    }

    void diacrtivatesignupBTN()
    {
        signupBTNbuttonObj.enabled = false;
        signupBTNimg.color = signupBTN_disable_color;
        if (loadingIcon != null)
        {
            loadingIcon.SetActive(true);
        }
    }
    public void navigateToVerifyEmail()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("VerifyEmail");
    }
}
