using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lb_modal_handler : MonoBehaviour
{
    revivalModalPanel modalPanel;
    public String modalTag;
    public int nftId;

    private void Awake()
    {
        var gObjects = GameObject.FindGameObjectsWithTag(modalTag);
        if (gObjects.Length > 0)
        {
            modalPanel = gObjects[0].GetComponent<revivalModalPanel>();
        }
    }


    public void openRivival_modal()
    {
        // do something before opening the modal
        modalPanel.openModal();
    }
}
