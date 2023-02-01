using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class firstTimeClickOfBuyNftTab : MonoBehaviour
{
    bool clicked = false;
    public buySectonTabCaller _buySectonTabCaller;

    public void click()
    {
        if (!clicked)
        {
            _buySectonTabCaller.click();
            clicked = true;
        }
    }
}
