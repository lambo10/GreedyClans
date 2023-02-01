using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class bigShopBTNclick : MonoBehaviour
{
    public GameObject button;
    public void click()
    {
        var go = button;
        var ped = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(go, ped, ExecuteEvents.pointerEnterHandler);
        ExecuteEvents.Execute(go, ped, ExecuteEvents.submitHandler);
    }
}
