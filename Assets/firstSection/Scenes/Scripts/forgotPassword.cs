using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class forgotPassword : MonoBehaviour
{
    public TMP_InputField enterEmail;
    // input field to enter password
    public TextMeshProUGUI errorMessage;
    public GameObject sendCodeBTN;
    public Color sendCodeBTN_Enable_color;
    public Color sendCodeBTN_disable_color;
    public GameObject loadingIcon;
    Button sendCodeBTNbuttonObj;
    Image sendCodeBTNimg;


    void Awake()
    {
        if (sendCodeBTN != null)
        {
            sendCodeBTNbuttonObj = sendCodeBTN.GetComponent<Button>();
            sendCodeBTNimg = sendCodeBTN.GetComponent<Image>();
        }
    }

    public void requestForgotPassCode()
    {
        // make sure all fields are filled
        if (enterEmail.text == "")
        {
            errorMessage.text = "Please fill in all fields";
        }
        else
        {
            // create a new user
            StartCoroutine(sendCode());
        }
    }

    IEnumerator sendCode()
    {
        diacrtivatesendCodeBTN();
        // clear error message
        errorMessage.text = "";
        // unity web request
        UnityWebRequest www = UnityWebRequest.Get($"https://greedyverse.co/api/send-recovery-mail.php?email={enterEmail.text}");
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
                stateUserDetails.forgetPasswordEmail = enterEmail.text;
                navigateToforgotPassword2();
            }

        }
        activatesendCodeBTN();

    }

    void activatesendCodeBTN()
    {
        sendCodeBTNbuttonObj.enabled = true;
        sendCodeBTNimg.color = sendCodeBTN_Enable_color;
        if (loadingIcon != null)
        {
            loadingIcon.SetActive(false);
        }
    }

    void diacrtivatesendCodeBTN()
    {
        sendCodeBTNbuttonObj.enabled = false;
        sendCodeBTNimg.color = sendCodeBTN_disable_color;
        if (loadingIcon != null)
        {
            loadingIcon.SetActive(true);
        }
    }


    void navigateToforgotPassword2()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("forgotPassword2");
    }
}
