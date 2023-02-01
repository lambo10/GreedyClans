using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
public class QuestDialogUi : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    private Button yesBtn;
    private Button noBtn;
    private TMP_InputField inputField;
    
    private void Awake()
    {
        try
        {
            textMeshPro = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        }
        catch (Exception e)
        {

        }
        yesBtn = transform.Find("YesBtn").GetComponent<Button>();
        noBtn = transform.Find("NoBtn").GetComponent<Button>();
    }
    // Start is called before the first frame update
    public void showQuestion(string questionText,string okButtonText,string cancelBtnText,Action yesAction,Action noAction)
    {
        gameObject.SetActive(true);
        textMeshPro.text = questionText;
        yesBtn.gameObject.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = okButtonText;
        noBtn.gameObject.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = cancelBtnText;
        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();
        yesBtn.onClick.AddListener(()=> {
            Hide();
            yesAction();
        });
        noBtn.onClick.AddListener(()=>
        {
            Hide();
            noAction();
        });
    }

    public enum CopyKeys
    {
         PrivateKey,
         SeedPhrase
    }

    IEnumerator confirmPasswordFromServer(string passwordTxt,CopyKeys keyType,Action onCopyPassword)
    {
        GameObject spinner = transform.Find("SpinnerLand").gameObject;
  
        UnityWebRequest www = UnityWebRequest.Get(Helper.confirmPassword(playerDetails.usr_walletAddress, passwordTxt));
        // send the request
        yield return www.SendWebRequest();
        spinner.SetActive(false);
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // get the response
            string response = www.downloadHandler.text;

            TypicalJsonResponse confirmPassword = JsonUtility.FromJson<TypicalJsonResponse>(response);
            if (confirmPassword.success != null && !confirmPassword.success)
            {

                checkExpiredSession.checkSessionExpiration(confirmPassword.msg);

            }
            else
            {
                
                GameObject canvas = GameObject.Find("Canvas");

                String title = keyType == CopyKeys.PrivateKey ? "Private Key" : "Seed Phrase";
                String copy = keyType == CopyKeys.PrivateKey ? playerDetails.usr_private_key :playerDetails.usr_seed_phrase;

                QuestDialogUi questionDialog = canvas.transform.Find("QuestionDialog").GetComponent<QuestDialogUi>();
                

                Hide();
                questionDialog.showQuestion(copy, "Copy", "Close", () =>
                  {
                      onCopyPassword();
                      TextEditor editor = new TextEditor
                      {
                          text = copy
                      };
                      editor.SelectAll();
                      editor.Copy();

                     
                      Debug.Log("User choose yes");

                  }, () =>
                  {
                      Debug.Log("userChooseNo");
                  });


            }
        }
    }


    public void confirmPassword(CopyKeys key,Action onCopyPassword)
    {
       
        gameObject.SetActive(true);
        GameObject spinner = transform.Find("SpinnerLand").gameObject;
        inputField = transform.Find("confirmInput").gameObject.GetComponent<TMPro.TMP_InputField>();
        spinner.SetActive(false);
        Debug.Log(transform.Find("confirmInput"));
        Debug.Log(inputField);
        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();
        yesBtn.onClick.AddListener(() =>
        {
            spinner.SetActive(true);
            StartCoroutine(confirmPasswordFromServer(inputField.text,key, onCopyPassword));
            
            inputField.text = "";
        });
        noBtn.onClick.AddListener(() =>
        {
            inputField.text = "";
            Hide();
        });
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
