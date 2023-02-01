using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class displaySection : MonoBehaviour
{
    public GameObject section;
    public GameObject section0;


    public void opensection()
    {
        try
        {
            section.SetActive(true);
            if (section0 != null) {
                section0.SetActive(false);
            }
            
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    public void closesection()
    {
        try
        {
            if (section != null)
            {
                section.SetActive(false);
                if (section0 != null)
                {
                    section0.SetActive(true);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
