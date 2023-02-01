using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class forgotPassword2 : MonoBehaviour
{
    public TMP_InputField enterCode;
    public TMP_InputField enterNewPass;
    // input field to enter password
    public TextMeshProUGUI errorMessage;
    public GameObject ResetPassBTN;
    public Color ResetPassBTN_Enable_color;
    public Color ResetPassBTN_disable_color;
    public GameObject loadingIcon;
    Button ResetPassBTNbuttonObj;
    Image ResetPassBTNimg;
    string forgetPasswordEmailLocal;


    void Awake()
    {
        if (ResetPassBTN != null)
        {
            ResetPassBTNbuttonObj = ResetPassBTN.GetComponent<Button>();
            ResetPassBTNimg = ResetPassBTN.GetComponent<Image>();
        }
    }

    public void resetPass()
    {
        // make sure all fields are filled
        if (enterCode.text == "" || enterNewPass.text == "")
        {
            errorMessage.text = "Please fill in all fields";
        }
        else
        {
            forgetPasswordEmailLocal = stateUserDetails.forgetPasswordEmail;
            if (forgetPasswordEmailLocal != null) {
                StartCoroutine(trnNewPassCode());
            }
            else
            {
                Debug.Log("Forget Password Variable is Empty");
            }
        }
    }

    IEnumerator trnNewPassCode()
    {
        diacrtivateResetPassBTN();
        // clear error message
        errorMessage.text = "";
        // unity web request
        UnityWebRequest www = UnityWebRequest.Get($"https://greedyverse.co/api/forget-password.php?email={forgetPasswordEmailLocal}&newPassword={enterNewPass.text}&code={enterCode.text}");
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
                navigateToLogin();
            }

        }
        activateResetPassBTN();

    }

    void activateResetPassBTN()
    {
        ResetPassBTNbuttonObj.enabled = true;
        ResetPassBTNimg.color = ResetPassBTN_Enable_color;
        if (loadingIcon != null)
        {
            loadingIcon.SetActive(false);
        }
    }

    void diacrtivateResetPassBTN()
    {
        ResetPassBTNbuttonObj.enabled = false;
        ResetPassBTNimg.color = ResetPassBTN_disable_color;
        if (loadingIcon != null)
        {
            loadingIcon.SetActive(true);
        }
    }


    void navigateToLogin()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
    }
}
