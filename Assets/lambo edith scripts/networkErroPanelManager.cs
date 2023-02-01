using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class networkErroPanelManager : MonoBehaviour
{
    public GameObject network_erro_panel;
    public Button network_erro_panel_retryBTN;
    public static bool canShow = false;
    public static bool prev_canShow = false;
    public static UnityAction _functionToExecute;
    public static bool canExecute = true;

    // Update is called once per frame
    void Update()
    {
        if (canExecute)
        {
            if (canShow)
            {
                Debug.Log("Here ------ Here ---------");
                network_erro_panel.SetActive(true);
                network_erro_panel_retryBTN.onClick.AddListener(_functionToExecute);
            }
            else
            {
                network_erro_panel.SetActive(false);
                network_erro_panel_retryBTN.onClick.RemoveAllListeners();
            }
            prev_canShow = canShow;

        }

        if(prev_canShow == canShow)
        {
            canExecute = false;
        }
        else
        {
            canExecute = true;
        }

    }

}
