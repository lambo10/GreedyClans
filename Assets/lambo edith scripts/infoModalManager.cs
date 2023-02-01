using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class infoModalManager : MonoBehaviour
{
    public GameObject modal;
    public TextMeshProUGUI name;
    public TextMeshProUGUI health;
    public TextMeshProUGUI damage;
    public TextMeshProUGUI speed;
    public TextMeshProUGUI range;
    public TextMeshProUGUI description;
    public Image icon;

    public void passDetails(string name_txt, int health_value, int damage_value, int speed_value, int range_value, string description_txt, Sprite icon_img)
    {

        if (name_txt.Equals("Wood Wall") || name_txt.Equals("Stone Wall"))
        {
            name.text = name_txt; health.text = "-"; damage.text = "-"; speed.text = "-"; range.text = "-"; description.text = description_txt;
            icon.sprite = icon_img;
        }
        else if (name_txt.Equals("Toolhouse") || name_txt.Equals("Army Camp") || name_txt.Equals("Mega Army Camp") || name_txt.Equals("Barracks") || name_txt.Equals("Mega Barracks"))
        {
            name.text = name_txt; health.text = "" + health_value; damage.text = "-"; speed.text = "" + speed_value; range.text = "" + range_value; description.text = description_txt;
            icon.sprite = icon_img;
        }
        else
        {
            name.text = name_txt; health.text = "" + health_value; damage.text = "" + damage_value; speed.text = "" + speed_value; range.text = "" + range_value; description.text = description_txt;
            icon.sprite = icon_img;
        }

    }

}
