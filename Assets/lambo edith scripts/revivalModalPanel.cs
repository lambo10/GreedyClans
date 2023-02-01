using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class revivalModalPanel : MonoBehaviour
{
    public GameObject modal;

    public void openModal()
    {
        try
        {
                modal.SetActive(true);  
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    public void closeModal()
    {
        try
        {
            if (modal != null)
            {
                modal.SetActive(false);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
