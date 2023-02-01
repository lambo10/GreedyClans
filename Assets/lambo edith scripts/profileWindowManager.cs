using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEditor;
using Newtonsoft.Json.Linq;

public class profileWindowManager : MonoBehaviour
{
    public GameObject profileWindow;
    public TextMeshProUGUI walletAddress;
    public TextMeshProUGUI username;
    public TextMeshProUGUI email;
    public GameObject[] lands;
    public TextMeshProUGUI[] landLabels;
    public TextMeshProUGUI[] landIDs;

    private GameObject spinnerLand;
    private GameObject loadingOverlay;
    private GameObject loadingSpinner;

    [SerializeField] private Transform scrollViewContainer;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject land, claimToken, notifications;


    private Text txt;

    public void claimTokens()
    {
        GameObject errMsg = claimToken.transform.Find("ErrorMsg").gameObject;

        GameObject spinnerLandUnclaimed = claimToken.transform.Find("SpinnerLand").gameObject;
        spinnerLandUnclaimed.SetActive(true);
        errMsg.SetActive(false);
        StartCoroutine(claimTokensroutine());
    }

    public void logOut()
    {
        GameObject canvas = GameObject.Find("Canvas");

        QuestDialogUi questionDialog = canvas.transform.Find("QuestionDialog").GetComponent<QuestDialogUi>();
        questionDialog.showQuestion("Please Remember to Backup your private key or seed phrase before you logout", "Yes", "No", () =>
        {
            Debug.Log("User choose yes");

            GameObject errMsg = land.transform.Find("ErrorMsg").gameObject;
            errMsg.SetActive(false);
            StartCoroutine(logOutRoutine());
        }, () =>
        {

            Debug.Log("userChooseNo");
        });




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


    public void copyPrivateKey()
    {
        GameObject canvas = GameObject.Find("Canvas");

        QuestDialogUi questionDialog = canvas.transform.Find("ConfirmPassword").GetComponent<QuestDialogUi>();
        questionDialog.confirmPassword(QuestDialogUi.CopyKeys.PrivateKey, () => {
            loadingOverlay = GameObject.Find("Canvas").transform.Find("LoadingOverlay").gameObject;
            loadingSpinner = loadingOverlay.transform.Find("loading").gameObject;
            loadingOverlay.SetActive(true);
            loadingSpinner.SetActive(false);
            showToast("Private Key Copied", 2);
        });

    }

    public void copySeedPhrase()
    {
        GameObject canvas = GameObject.Find("Canvas");

        QuestDialogUi questionDialog = canvas.transform.Find("ConfirmPassword").GetComponent<QuestDialogUi>();
        questionDialog.confirmPassword(QuestDialogUi.CopyKeys.SeedPhrase, () => {
            loadingOverlay = GameObject.Find("Canvas").transform.Find("LoadingOverlay").gameObject;
            loadingSpinner = loadingOverlay.transform.Find("loading").gameObject;
            loadingOverlay.SetActive(true);
            loadingSpinner.SetActive(false);
            showToast("Seed Phrase Copied", 2);
        });

    }

    IEnumerator logOutRoutine()
    {
        UnityWebRequest www = UnityWebRequest.Get(Helper.logOut(playerDetails.usr_walletAddress));
        GameObject spinnerLandUnclaimed = claimToken.transform.Find("SpinnerLand").gameObject;

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
            GameObject errMsg = land.transform.Find("ErrorMsg").gameObject;


            if (unclaimedTokensResp.success != null && !unclaimedTokensResp.success)

            {
                checkExpiredSession.checkSessionExpiration(unclaimedTokensResp.msg);
                errMsg.SetActive(true);
                errMsg.GetComponent<TextMeshProUGUI>().text = unclaimedTokensResp.msg;

            }
            else
            {
                PlayerPrefs.DeleteAll();
                UnityEngine.SceneManagement.SceneManager.LoadScene("ConnectWallet");
            }
        }
    }


    IEnumerator claimTokensroutine()
    {
        UnityWebRequest www = UnityWebRequest.Get(Helper.claimTokens(playerDetails.usr_sessionID, playerDetails.usr_walletAddress, Helper.chainId, playerDetails.usr_private_key));
        GameObject spinnerLandUnclaimed = claimToken.transform.Find("SpinnerLand").gameObject;

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
            GameObject errMsg = claimToken.transform.Find("ErrorMsg").gameObject;
            TextMeshProUGUI unclaimed_tokens = claimToken.transform.Find("unclaimed_tokens").GetComponent<TextMeshProUGUI>();


            if (unclaimedTokensResp.success != null && !unclaimedTokensResp.success)

            {

                checkExpiredSession.checkSessionExpiration(unclaimedTokensResp.msg);
                errMsg.SetActive(true);
                errMsg.GetComponent<TextMeshProUGUI>().text = unclaimedTokensResp.msg;

            }
            else
            {
                unclaimed_tokens.text = unclaimedTokensResp.msg;
            }
        }
    }

    IEnumerator getNotifications()
    {
        //spiner.SetActive(true);
        // clear error message
        //errorMessage.text = "";
        // unity web request

        UnityWebRequest www = UnityWebRequest.Get(Helper.notifications(playerDetails.usr_sessionID, playerDetails.usr_walletAddress));
        GameObject spinnerNotifications = notifications.transform.Find("SpinnerLand").gameObject;
        GameObject errMsg = notifications.transform.Find("ErrorMsg").gameObject;

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

            if (notificationsData.msg == null)
            {
                if (notificationStringMsg.msg != null)
                {
                    checkExpiredSession.checkSessionExpiration(notificationStringMsg.msg);
                }
            }


            if (notificationsData.success != null && !notificationsData.success)

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



    public static string replaceLink(string link)
    {
        if (link == null) return "";
        return link.Replace("{{address}}", playerDetails.usr_walletAddress)
            .Replace("{{sessionID}}", playerDetails.usr_sessionID)
             .Replace("{{chainId}}", Helper.chainId);
    }


    public void setActiveWindow(int index)
    {
        GameObject indexOne = land;
        GameObject indexTwo = claimToken;
        GameObject indexThree = notifications;
        indexOne.SetActive(false);
        indexTwo.SetActive(false);
        indexThree.SetActive(false);
        if (index == 0)
        {
            GameObject errMsg = notifications.transform.Find("ErrorMsg").gameObject;

            GameObject spinnerNotifications = notifications.transform.Find("SpinnerLand").gameObject;
            spinnerNotifications.SetActive(true);
            errMsg.SetActive(false);
            StartCoroutine(getNotifications());
            indexThree.SetActive(true);
        }
        else if (index == 1)
        {
            indexTwo.SetActive(true);
            GameObject errMsg = claimToken.transform.Find("ErrorMsg").gameObject;

            GameObject spinnerLandUnclaimed = claimToken.transform.Find("SpinnerLand").gameObject;
            GameObject claimBtn = claimToken.transform.Find("claimBTN").gameObject;
            claimBtn.SetActive(false);
            errMsg.SetActive(false);
            spinnerLandUnclaimed.SetActive(true);
            StartCoroutine(getUnclaimedTokens());
        }
        else if (index == 2)
        {

            indexOne.SetActive(true);
        }
    }
    public void diisplayProfile()
    {

        
        profileWindow.SetActive(true);
        for (int i = 0; i < lands.Length; i++)
        {
            int closureIndex = i;
            if (lands[closureIndex].GetComponent<Button>() == null)
            {
                lands[closureIndex].AddComponent<Button>().onClick.AddListener(() => {

                    playerDetails.landNo = closureIndex + 1 + "";
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
                });
            }
            lands[closureIndex].SetActive(false);
        }
        getAmountofLand();
        getLands();
    }

    void getLands()
    {
        string getNFTUrl = Helper.getUser1155NFT(playerDetails.usr_walletAddress, Helper.chainId, "29", playerDetails.usr_sessionID);
        spinnerLand = land.transform.Find("SpinnerLand").gameObject;
        spinnerLand.SetActive(true);
        StartCoroutine(getMainNetTokenRequest(getNFTUrl));
    }


    IEnumerator getUnclaimedTokens()
    {
        //spiner.SetActive(true);
        // clear error message
        //errorMessage.text = "";
        // unity web request
        GameObject errMsg = claimToken.transform.Find("ErrorMsg").gameObject;

        UnityWebRequest www = UnityWebRequest.Get(Helper.getUnclaimedToken(playerDetails.usr_sessionID, playerDetails.usr_walletAddress, Helper.chainId));
        GameObject spinnerLandUnclaimed = claimToken.transform.Find("SpinnerLand").gameObject;
        GameObject claimBtn = claimToken.transform.Find("claimBTN").gameObject;

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
            TextMeshProUGUI unclaimed_tokens = claimToken.transform.Find("unclaimed_tokens").GetComponent<TextMeshProUGUI>();


            if (unclaimedTokensResp.success != null && !unclaimedTokensResp.success)

            {

                checkExpiredSession.checkSessionExpiration(unclaimedTokensResp.msg);
                errMsg.SetActive(true);
                errMsg.GetComponent<TextMeshProUGUI>().text = unclaimedTokensResp.msg;

            }
            else
            {
                claimBtn.SetActive(true);
                unclaimed_tokens.text = unclaimedTokensResp.msg + " GVerse";
            }
        }
    }

    IEnumerator getMainNetTokenRequest(string url)
    {
        //spiner.SetActive(true);
        // clear error message
        //errorMessage.text = "";
        // unity web request
        UnityWebRequest www = UnityWebRequest.Get(url);
        Debug.Log("here");
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
            GameObject errMsg = land.transform.Find("ErrorMsg").gameObject;

            if (userLands.success != null && !userLands.success)

            {

                checkExpiredSession.checkSessionExpiration(userLands.msg);
                errMsg.SetActive(true);
                errMsg.GetComponent<TextMeshProUGUI>().text = userLands.msg;

            }
            else
            {

                Debug.Log(gameObject.name);
                switch (userLands.msg.Trim())
                {
                    case "1":
                        lands[0].SetActive(true);

                        break;
                    case "2":
                        lands[0].SetActive(true);


                        lands[1].SetActive(true);

                        break;
                    case "3":
                        lands[0].SetActive(true);


                        lands[1].SetActive(true);


                        lands[2].SetActive(true);

                        break;
                    case "4":
                        lands[0].SetActive(true);


                        lands[1].SetActive(true);


                        lands[2].SetActive(true);


                        lands[3].SetActive(true);

                        break;
                    case "5":
                        lands[0].SetActive(true);


                        lands[1].SetActive(true);


                        lands[2].SetActive(true);


                        lands[3].SetActive(true);


                        lands[4].SetActive(true);

                        break;

                    default:
                        errMsg.GetComponent<TextMeshProUGUI>().text = $"{userLands.msg} Land{(userLands.msg != "0" ? "s" : "")} Available...";
                        //errorMessage.color = Color.red;
                        break;
                }
            }

        }
        spinnerLand.SetActive(false);
    }


    private void getAmountofLand()
    {
        int amountOofLand = int.Parse(playerDetails.landNo);
        string[] requestedLandIDs = new string[] {
            "",
            "",
            "",
            "",
            ""
        };

        for (int i = 0; i < lands.Length; i++)
        {
            landLabels[i].text = "Land " + (i + 1);
        }
    }
}