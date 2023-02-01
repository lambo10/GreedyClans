using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class checkLoggedIn_in_opening_scene : MonoBehaviour
{

    public relayManager _relayManager;
    private bool started = false;
    public GameObject update_panel;
    public Button update_btn;
    private string install_link = "";
   
    // remove after test relay and serverrpc function
    public TextMeshProUGUI tempTXT;


    public GameObject network_erro_panel;
    public Button network_erro_panel_retryBTN;

    void checke_network_reachability(UnityEngine.Events.UnityAction _fun)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            network_erro_panel.SetActive(false);
            network_erro_panel_retryBTN.onClick.AddListener(_fun);
        }
        else
        {
            network_erro_panel.SetActive(true);
        }
    }


    private void Update()
    {
       

        if (!started)
        {
            if (_relayManager.serverInstance)
            {
                if (_relayManager._joinCode.Length < 1 || _relayManager._joinCode == null)
                { }
                else
                {

                    StartCoroutine(WaitAndDoSomething());
                    started = true;
                }
            }
            else
            {
                StartCoroutine(WaitAndDoSomething());
                started = true;
            }
        }




        // remove after test relay and serverrpc function

        tempTXT.text = _relayManager._joinCode;
}

   


public void check_app_update()
    {
            StartCoroutine("check_app_update_request");
    }

    IEnumerator check_app_update_request()
    {

        // unity web request
        UnityWebRequest www = UnityWebRequest.Get(Helper.check_new_version(Helper.versionNo));
        // send the request
        yield return www.SendWebRequest();
        // check for errors
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            checke_network_reachability(check_app_update);
        }
        else
        {
            // get the response
            string response = www.downloadHandler.text;

            check_app_new_version update_availability = JsonUtility.FromJson<check_app_new_version>(response);
            if (!update_availability.success)
            {
                loginInUser();

            }
            else
            {
                install_link = update_availability.install_link;
                update_panel.SetActive(true);
                update_btn.onClick.AddListener(OpenLink);
            }

        }

    }

    void OpenLink()
    {
        Application.OpenURL(install_link);
    }


    // remove after test relay and serverrpc function
    public void continueTempBTNclick()
    {
        StartCoroutine(WaitAndDoSomething());
    }






    IEnumerator WaitAndDoSomething()
    {
        // Wait for 10 seconds
        yield return new WaitForSeconds(3);

        // Call the DoSomething function
        check_app_update();
    }

    public void loginInUser()
    {
        var userSeedPhrase = PlayerPrefs.GetString(Helper.userWalletDataKey);
        
        var savedData2 = PlayerPrefs.GetString(Helper.userDetailsDataKey);


        if (userSeedPhrase == null || userSeedPhrase.Length <= 0)
        {
            navigateTo("ConnectWallet");
            return;
        }else if (savedData2 == null || savedData2.Length <= 0)
        {
            navigateTo("Login");
            return;
        }



        ImportWallet seedPhrase = JsonUtility.FromJson<ImportWallet>(userSeedPhrase);
        loginResultsModel savedData2Jason = JsonUtility.FromJson<loginResultsModel>(savedData2);


        StartCoroutine(loginInRequest(seedPhrase.bsc_wallet_address, savedData2Jason.password));
    }

    IEnumerator loginInRequest(string userAddress,string enterPassword)
    {
        
        // unity web request
        UnityWebRequest www = UnityWebRequest.Get($"{Helper.loginUrl}?address={userAddress}&password={enterPassword}");
        // send the request
        yield return www.SendWebRequest();
        // check for errors
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // get the response
            string response = www.downloadHandler.text;

            TypicalJsonResponse userLoggedIn = JsonUtility.FromJson<TypicalJsonResponse>(response);
            if (!userLoggedIn.success)
            {
                navigateTo("ConnectWallet");

            }
            else
            {
                string storeString = AddPasswordToJson(response, enterPassword);
                PlayerPrefs.SetString(Helper.userDetailsDataKey, storeString);
                navigateTo("GameMode");
            }

        }

    }

    public void navigateTo(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
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
