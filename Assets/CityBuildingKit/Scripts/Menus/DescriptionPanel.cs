using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPanel : MonoBehaviour {

    const string NO_DESCRIPTION_TEXT = "There is no description for this item";

    [SerializeField]
    private GameObject _descriptionPanel = null;
    [SerializeField]
    private Text _titleText = null;
    [SerializeField]
    private Text _descriptionText = null;

    public static DescriptionPanel Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetDescriptionInfo(string titleText, string descriptionText)
    {
        if(_titleText)
        {
            _titleText.text = titleText;
        }
        else
        {
            throw new Exception("Title text component is missing.");
        }

        if(_descriptionText)
        {
            if(string.IsNullOrEmpty(descriptionText))
            {
                _descriptionText.text = NO_DESCRIPTION_TEXT;
            }
            else
            {
                _descriptionText.text = descriptionText;
            }
        }
        else
        {
            throw new Exception("Description text component is missing.");
        }

        _descriptionPanel.SetActive(true);
    }

    public void HideDescriptionPanel()
    {
        _descriptionPanel.SetActive(false);
    }
}
