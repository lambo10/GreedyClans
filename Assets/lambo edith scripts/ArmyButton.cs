using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ArmyButton : MonoBehaviour
{
    [SerializeField]
    private Transform loadingPanel;
    private GameObject toastPanel, toastSpinner;
    public Text txt;
    public MenuArmy _menuArmy;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void showToast(string text,
       int duration)
    {
        toastPanel = GameObject.Find("Canvas").transform.Find("LoadingOverlay").gameObject;
        toastSpinner = toastPanel.transform.Find("loading").gameObject;
        toastPanel.SetActive(true);
        toastSpinner.SetActive(false);
        StartCoroutine(showToastCOR(text, duration));
    }

    IEnumerator fadeInAndOut(Text targetText, bool fadeIn, float duration)
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

        Color currentColor = Color.clear;
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
    }

    private IEnumerator showToastCOR(string text,
        int duration)
    {
        txt = GameObject.FindGameObjectsWithTag("SpinnerText")[0].GetComponent<Text>();
        Color orginalColor = txt.color;

        txt.text = text;
        txt.enabled = true;

        //Fade in
        yield return fadeInAndOut(txt, true, 0.5f);

        //Wait for the duration
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Fade out
        yield return fadeInAndOut(txt, false, 0.5f);

        txt.enabled = false;
        txt.color = orginalColor;
        toastPanel.SetActive(false);
        toastSpinner.SetActive(false);
    }

    public void loadAttackMap()
    {
        loadingPanel.gameObject.SetActive(true);
        StartCoroutine(loadAttackMapCour(Helper.findAndMatchPlayer(playerDetails.usr_sessionID, playerDetails.usr_walletAddress, Helper.chainId, playerDetails.landNo)));
        Debug.Log(loadingPanel);
    }

    IEnumerator loadAttackMapCour(string url)
    {

        Debug.Log(url);
        UnityWebRequest www = UnityWebRequest.Get(url);

        // send the request
        yield return www.SendWebRequest();
        loadingPanel.gameObject.SetActive(false);
        // check for errors
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            //errorMessage.text = "Error: " + www.error;
        }
        else
        {
            // get the response
            string response = www.downloadHandler.text;
            Debug.Log(response);

            find_and_match getMatch = JsonUtility.FromJson<find_and_match>(response);

            if (getMatch.success != null && !getMatch.success)
            {
                checkExpiredSession.checkSessionExpiration(getMatch.msg);
                showToast(getMatch.msg, 4);
            }
            else
            {
                Helper.connection_instance_id = getMatch.instanceID;

                connecToInstance(Helper.connection_instance_id);

            }
        }
    }

    async void connecToInstance(string connection_instance_id)
    {
        // check if offchain is true
        //doawnloadMapData()
        if (int.Parse(Helper.chainId) == 4000)
        {
            doawnloadMapData(connection_instance_id);
        }
        else { 
                bool connected = await relayManager.joinRelay(connection_instance_id);
            if (connected)
            {
                doawnloadMapDataServerRpc(connection_instance_id);
            }
            else
            {

                showToast("Could not establish connection with server instance", 4);
            }
        }
        
    }



    [ServerRpc]
    void doawnloadMapDataServerRpc(string connection_instance_id)
    {
        PlayerPrefs.DeleteKey(Helper.battleMapKey);
        loadingPanel.gameObject.SetActive(true);
        StartCoroutine(loadAttackMapCourServerRpc(Helper.getInstanceMapData(connection_instance_id)));
    }

    [ServerRpc]
    IEnumerator loadAttackMapCourServerRpc(string url)
    {

        UnityWebRequest www = UnityWebRequest.Get(url);

        // send the request
        yield return www.SendWebRequest();
        loadingPanel.gameObject.SetActive(false);
        // check for errors
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            //errorMessage.text = "Error: " + www.error;
        }
        else
        {
            // get the response
            string response = www.downloadHandler.text;
        

            battleMapJsonHandler getMatch = JsonUtility.FromJson<battleMapJsonHandler>(response);

            if (getMatch.success != null && !getMatch.success)
            {
                checkExpiredSession.checkSessionExpiration(getMatch.msg);
                showToast(getMatch.msg, 4);
            }
            else
            {
                
                PlayerPrefs.SetString(Helper.battleMapKey, response);
                
                _menuArmy.LoadCampaign(0);

            }
        }
    }

       



    void doawnloadMapData(string connection_instance_id)
    {
        PlayerPrefs.DeleteKey(Helper.battleMapKey);
        loadingPanel.gameObject.SetActive(true);
        StartCoroutine(loadAttackMapCourOffchain(Helper.getInstanceMapData(connection_instance_id)));
    }

    [ServerRpc]
    IEnumerator loadAttackMapCourOffchain(string url)
    {

        UnityWebRequest www = UnityWebRequest.Get(url);

        // send the request
        yield return www.SendWebRequest();
        loadingPanel.gameObject.SetActive(false);
        // check for errors
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            //errorMessage.text = "Error: " + www.error;
        }
        else
        {
            // get the response
            string response = www.downloadHandler.text;
            battleMapJsonHandler getMatch = JsonUtility.FromJson<battleMapJsonHandler>(response);

            if (getMatch.success != null && !getMatch.success)
            {
                checkExpiredSession.checkSessionExpiration(getMatch.msg);
                showToast(getMatch.msg, 4);
            }
            else
            {

                PlayerPrefs.SetString(Helper.battleMapKey, response);

                _menuArmy.LoadCampaign(0);

            }
        }
    }


}