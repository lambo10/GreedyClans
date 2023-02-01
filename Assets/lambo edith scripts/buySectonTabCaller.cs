using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buySectonTabCaller : MonoBehaviour
{
    public int contentIndex;
    public buySectionManager _buySectionManager;

    public void click()
    {
        _buySectionManager.contentIndex = contentIndex;
        _buySectionManager.displayContent();
    }
}
