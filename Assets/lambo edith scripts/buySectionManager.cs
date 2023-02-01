using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buySectionManager : MonoBehaviour
{
    public GameObject[] content;
    public Button[] tabButtons;
    public int contentIndex;
    public Color noneActiveTabColor;

    public void displayContent()
    {
        for (int i = 0; i < content.Length; i++)
        {
            if (i != contentIndex)
            {
                content[i].SetActive(false);
                tabButtons[i].GetComponent<Image>().color = noneActiveTabColor;
            }
            else
            {
                content[i].SetActive(true);
                tabButtons[i].GetComponent<Image>().color = Color.black;
            }
        }

    }
}
