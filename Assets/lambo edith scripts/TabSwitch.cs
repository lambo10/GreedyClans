using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TabSwitch : MonoBehaviour
{
    [SerializeField] private GameObject LandTab, NotificationTab, TokenTab, bottomGroup, stats, textStats, closeTabBtn,addressDisplay,emailDisplay,userDisplay,landNo,tokenNo,notificationNo;
    [SerializeField] private Transform scrollViewContainer;
    private GameObject loadingOverlay;
    private GameObject loadingSpinner;
    [SerializeField] private GameObject itemPrefab;
    private Text txt;

    private void Start()
    {
        addressDisplay.GetComponent<TextMeshProUGUI>().text = playerDetails.usr_walletAddress;
        emailDisplay.GetComponent<TextMeshProUGUI>().text = playerDetails.usr_email;
        userDisplay.GetComponent<TextMeshProUGUI>().text = playerDetails.usr_username;
        getLands();
        StartCoroutine(getUnclaimedTokens((result)=> {
            tokenNo.GetComponent<TextMeshProUGUI>().text =result;
        }));

        StartCoroutine(checkNotification((result) => {
            notificationNo.GetComponent<TextMeshProUGUI>().text = result.ToString();
        }));
    }
    // Start is called before the first frame update
    public void activateLand()
    {
        disactivateAllTab();
        closeTabBtn.SetActive(true);
        LandTab.SetActive(true);
    }


    public void activateNotifications()
    {
        disactivateAllTab();
        closeTabBtn.SetActive(true);
        NotificationTab.SetActive(true);
        StartCoroutine(checkNotification(null));
    }

    public void activateToken()
    {
        disactivateAllTab();
        closeTabBtn.SetActive(true);
        TokenTab.SetActive(true);
        StartCoroutine(getUnclaimedTokens(null));
    }

    public void activateDefault()
    {
        disactivateAllTab();
        stats.SetActive(true);
        textStats.SetActive(true);
    }

    public void disactivateAllTab()
    {
        StopAllCoroutines();
        LandTab.SetActive(false);
        NotificationTab.SetActive(false);
        TokenTab.SetActive(false);
        stats.SetActive(false);
        textStats.SetActive(false);
        closeTabBtn.SetActive(false);
    }

    void getLands()
    {
        string getNFTUrl = Helper.getUser1155NFT(playerDetails.usr_walletAddress, Helper.chainId, "29", playerDetails.usr_sessionID);
        
        StartCoroutine(getUserLands(getNFTUrl));
    }


    IEnumerator getUserLands(string url)
    {
        Debug.Log("land loading");
        // unity web request
        UnityWebRequest www = UnityWebRequest.Get(url);
        // send the request
        yield return www.SendWebRequest();
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

            TypicalJsonResponse userLands = JsonUtility.FromJson<TypicalJsonResponse>(response);
            

            if (userLands.success != null && !userLands.success)

            {
                checkExpiredSession.checkSessionExpiration(userLands.msg);
            }
            else
            { 
                landNo.GetComponent<TextMeshProUGUI>().text = userLands.msg.Trim();
            }

        }
    }





    IEnumerator getUnclaimedTokens(Action<string> callBack)
    {
        UnityWebRequest www = UnityWebRequest.Get(Helper.getUnclaimedToken(playerDetails.usr_sessionID, playerDetails.usr_walletAddress, Helper.chainId));
        GameObject spinnerLandUnclaimed = TokenTab.transform.Find("SpinnerLand").gameObject;

        // send the request
        yield return www.SendWebRequest();
        spinnerLandUnclaimed.SetActive(false);
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

            TypicalJsonResponse unclaimedTokensResp = JsonUtility.FromJson<TypicalJsonResponse>(response);


            if (unclaimedTokensResp.success != null && !unclaimedTokensResp.success)

            {

                checkExpiredSession.checkSessionExpiration(unclaimedTokensResp.msg);
                GameObject errMsg = TokenTab.transform.Find("ErrorMsg").gameObject;

                errMsg.SetActive(true);
                errMsg.GetComponent<TextMeshProUGUI>().text = unclaimedTokensResp.msg;

            }
            else
            {
                if(callBack == null)
                {
                    TextMeshProUGUI unclaimed_tokens = TokenTab.transform.Find("unclaimed_tokens").GetComponent<TextMeshProUGUI>();
                    GameObject claimBtn = TokenTab.transform.Find("claimBTN").gameObject;

                    claimBtn.SetActive(true);
                    unclaimed_tokens.text = unclaimedTokensResp.msg + " GVerse";
                }else
                {
                    callBack(unclaimedTokensResp.msg);
                }
                
                
            }
        }
    }

    public static string replaceLink(string link)
    {
        if (link == null) return "";
        return link.Replace("{{address}}", playerDetails.usr_walletAddress)
            .Replace("{{sessionID}}", playerDetails.usr_sessionID)
             .Replace("{{chainId}}", Helper.chainId);
    }

    IEnumerator checkLink(string url)
    {

        Debug.Log(url);
        UnityWebRequest www = UnityWebRequest.Get(url);

        // send the request
        yield return www.SendWebRequest();
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

            TypicalJsonResponse checkUrl = JsonUtility.FromJson<TypicalJsonResponse>(response);

            if (checkUrl.success != null && !checkUrl.success)

            {

                checkExpiredSession.checkSessionExpiration(checkUrl.msg);


            }
            loadingOverlay = GameObject.Find("Canvas").transform.Find("LoadingOverlay").gameObject;
            loadingSpinner = loadingOverlay.transform.Find("loading").gameObject;
            loadingOverlay.SetActive(true);
            loadingSpinner.SetActive(false);
            showToast(checkUrl.msg, 2);
        }
    }

    public void showToast(string text,
      int duration)
    {
        StartCoroutine(showToastCOR(text, duration));
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
        loadingOverlay.SetActive(false);
        loadingSpinner.SetActive(false);
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




    IEnumerator checkNotification(Action<int> callBack)
    {

        UnityWebRequest www = UnityWebRequest.Get(Helper.notifications(playerDetails.usr_sessionID, playerDetails.usr_walletAddress));
        GameObject spinnerNotifications = NotificationTab.transform.Find("SpinnerLand").gameObject;
        GameObject errMsg = NotificationTab.transform.Find("ErrorMsg").gameObject;
        Debug.Log("checking notificatios");
        // send the request
        yield return www.SendWebRequest();
        spinnerNotifications.SetActive(false);
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

            notificationsData notificationsData = JsonUtility.FromJson<notificationsData>(response);
            TypicalJsonResponse notificationStringMsg = JsonUtility.FromJson<TypicalJsonResponse>(response);
            bool failedResponse = notificationsData.success != null && !notificationsData.success;
            if (callBack != null && !failedResponse)
            {
                callBack(notificationsData.msg.Length);
                yield break;
            }
     
            if (notificationsData.msg == null)
            {
                if (notificationStringMsg.msg != null)
                {
                    checkExpiredSession.checkSessionExpiration(notificationStringMsg.msg);
                }
            }


            if (failedResponse)

            {
                errMsg.SetActive(true);
                errMsg.GetComponent<TextMeshProUGUI>().text = "Error getting notifications";

            }
            else
            {
                foreach (RectTransform child in scrollViewContainer.transform)
                {
                    Destroy(child.gameObject);
                }
                for (int i = 0; i < notificationsData.msg.Length; i++)
                {
                    int overloadIndex = i;
                    GameObject notificationList = Instantiate(itemPrefab);
                    notificationList.transform.Find("msg").GetComponent<TextMeshProUGUI>().text = notificationsData.msg[overloadIndex].msg;
                    Button notifierBtn = notificationList.transform.Find("btn").GetComponent<Button>();
                    notifierBtn.onClick.AddListener(() => {
                        StartCoroutine(checkLink(replaceLink(notificationsData.msg[overloadIndex].btn_link)));
                    });
                    if (notificationsData.msg[overloadIndex].btn_name != null)
                    {
                        notifierBtn.transform.Find("btnText").GetComponent<TextMeshProUGUI>().text = notificationsData.msg[overloadIndex].btn_name;
                    }
                    if (notificationsData.msg[overloadIndex].btn_active == "0")
                    {
                        notifierBtn.gameObject.SetActive(false);
                    }


                    notificationList.transform.SetParent(scrollViewContainer);
                    notificationList.transform.localScale = Vector2.one;
                }
            }
        }
    }


    

}
