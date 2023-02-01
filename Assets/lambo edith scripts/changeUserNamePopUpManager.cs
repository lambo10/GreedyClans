using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json.Linq;

public class changeUserNamePopUpManager : MonoBehaviour
{
    public GameObject spinner;
    public TMP_InputField username;
    public GameObject submitBTN;
    public TextMeshProUGUI erroMsg;

    public void closePanel()
    {
        this.gameObject.SetActive(false);
    }

    public void _updateUsername()
    {
        StartCoroutine(usernameChangeRequest(username.text));
    }

    IEnumerator usernameChangeRequest(string username)
    {
        spinner.SetActive(true);
        submitBTN.SetActive(false);
        string url = Helper.changeUsername(playerDetails.usr_walletAddress, playerDetails.usr_sessionID, Helper.chainId, username);
        UnityWebRequest www = UnityWebRequest.Get(url);
        // send the request
        yield return www.SendWebRequest();
        // check for errors
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            //errorMessage.text = "Error: " + www.error;
            erroMsg.text = www.error;
            erroMsg.color = Color.red;

            spinner.SetActive(false);
            submitBTN.SetActive(true);
        }
        else
        {
            string response = www.downloadHandler.text;
      
            TypicalJsonResponse reqresponse = JsonUtility.FromJson<TypicalJsonResponse>(response);

            if (reqresponse.success != null && !reqresponse.success)

            {
                erroMsg.text = reqresponse.msg;
                erroMsg.color = Color.red;

                spinner.SetActive(false);
                submitBTN.SetActive(true);
            }
            else
            {
                
                getProfileDetails();
            }

        }
        
    }



    private void getProfileDetails()
    {
        StartCoroutine(getProfileDetailsRequest());
    }

    IEnumerator getProfileDetailsRequest()
    {

        string url = Helper.getProfileDetails(playerDetails.usr_walletAddress, playerDetails.usr_sessionID, Helper.chainId);
        UnityWebRequest www = UnityWebRequest.Get(url);
        // send the request
        yield return www.SendWebRequest();
        // check for errors
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string response = www.downloadHandler.text;

            profileDetailsResult reqresponse = JsonUtility.FromJson<profileDetailsResult>(response);

            if (reqresponse.success != null && !reqresponse.success)

            { }
            else
            {
              
                playerDetails.usr_username = reqresponse.username;
                playerDetails.usr_email = reqresponse.email;

                var savedData2 = PlayerPrefs.GetString(Helper.userDetailsDataKey);
                loginResultsModel savedData2Jason = JsonUtility.FromJson<loginResultsModel>(savedData2);
                var password = savedData2Jason.password;

                string storeString = AddPasswordToJson(response, password);
                PlayerPrefs.SetString(Helper.userDetailsDataKey, storeString);
                erroMsg.text = "Username Updated";
                erroMsg.color = Color.green;
                Invoke("navigateToLogin", 1);
            }

        }
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

    public void navigateToLogin()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
    }





}
