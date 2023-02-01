using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using textMeshPro
using TMPro;
using UnityEngine.UI;
// unity web request
using UnityEngine.Networking;

public class VerifyEmail : MonoBehaviour
{
    // input field to enter password
    public TMP_InputField verificationCode;
    // text mesh pro to display error message
    public TextMeshProUGUI errorMessage;

    public GameObject verifyBTN;
    public Color verifyBTN_Enable_color;
    public Color verifyBTN_disable_color;
    public GameObject loadingIcon;
    Button verifyBTNbuttonObj;
    Image verifyBTNimg;

    public GameObject resendBTN;
    public Color resendBTN_Enable_color;
    public Color resendBTN_disable_color;
    Button resendBTNbuttonObj;
    Image resendBTNimg;

    string forgetPasswordEmailLocal;
    string walletAddress;

    private void Start()
    {
        var userSeedPhrase = PlayerPrefs.GetString(Helper.userWalletDataKey);
        ImportWallet seedPhrase = JsonUtility.FromJson<ImportWallet>(userSeedPhrase);
        walletAddress = seedPhrase.bsc_wallet_address;
    }

    void Awake()
    {
        if (verifyBTN != null)
        {
            verifyBTNbuttonObj = verifyBTN.GetComponent<Button>();
            verifyBTNimg = verifyBTN.GetComponent<Image>();
        }

        if (resendBTN != null)
        {
            resendBTNbuttonObj = resendBTN.GetComponent<Button>();
            resendBTNimg = resendBTN.GetComponent<Image>();
        }
    }



    public void verifyUserEmail(){
        // make sure all fields are filled
        if(verificationCode.text == ""){
            errorMessage.text = "Please fill in all fields";
        }
        else{
            // create a new user
            StartCoroutine(verifyUserEmailRequest());
        }
    }

    public void sendVerificationCode(){
        

            forgetPasswordEmailLocal = stateUserDetails.usrEmail;


        if (forgetPasswordEmailLocal != null)
            {
                StartCoroutine(sendVerificationCodeRequest());
            }
            else
            {
                Debug.Log("usrEmail Variable is Empty");
            }
        
    }

    IEnumerator sendVerificationCodeRequest()
    {
        diacrtivateresendBTN();
        // clear error message
        errorMessage.text = "";
        // unity web request
        UnityWebRequest www = UnityWebRequest.Get($"{Helper.send_verification_email}?email={forgetPasswordEmailLocal}");
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
            }else {
                errorMessage.text = userLoggedIn.msg;
                // color to green
                errorMessage.color = Color.green;
            }
            
        }
        activateresendBTN();
    }

    public void navigateToLogin(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
    }


     IEnumerator verifyUserEmailRequest()
    {
        diacrtivateverifyBTN();
        // clear error message
        errorMessage.text = "";
        // unity web request
        Debug.Log(walletAddress);
        UnityWebRequest www = UnityWebRequest.Get($"{Helper.verify_email}?address={walletAddress}&code={verificationCode.text}");
        
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
            }else {
                errorMessage.text = userLoggedIn.msg;
                // color to green
                errorMessage.color = Color.green;
                Helper.logedInFromSignUp = true;
                navigateToLogin();
            }
            
        }
        activateverifyBTN();
    }

    void activateverifyBTN()
    {
        verifyBTNbuttonObj.enabled = true;
        verifyBTNimg.color = verifyBTN_Enable_color;
        if (loadingIcon != null)
        {
            loadingIcon.SetActive(false);
        }
    }

    void diacrtivateverifyBTN()
    {
        verifyBTNbuttonObj.enabled = false;
        verifyBTNimg.color = verifyBTN_disable_color;
        if (loadingIcon != null)
        {
            loadingIcon.SetActive(true);
        }
    }

    void activateresendBTN()
    {
        resendBTNbuttonObj.enabled = true;
        resendBTNimg.color = resendBTN_Enable_color;
        if (loadingIcon != null)
        {
            loadingIcon.SetActive(false);
        }
    }

    void diacrtivateresendBTN()
    {
        resendBTNbuttonObj.enabled = false;
        resendBTNimg.color = resendBTN_disable_color;
        if (loadingIcon != null)
        {
            loadingIcon.SetActive(true);
        }
    }
}
