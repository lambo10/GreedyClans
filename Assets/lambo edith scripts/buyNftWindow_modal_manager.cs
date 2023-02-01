using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class buyNftWindow_modal_manager : MonoBehaviour
{
    public GameObject modal;
    public TextMeshProUGUI name;
    public TextMeshProUGUI health;
    public TextMeshProUGUI damage;
    public TextMeshProUGUI speed;
    public TextMeshProUGUI range;
    public TextMeshProUGUI cost;
    public TextMeshProUGUI quantityLeft;
    public TextMeshProUGUI description;
    public Image icon;

    public void passDetails(string name_txt, int health_value, int damage_value, int speed_value, int range_value, string id, Sprite icon_img, string _description)
    {
        
        name.text = name_txt; health.text = "" + health_value; damage.text = "" + damage_value; speed.text = "" + speed_value; range.text = "" + range_value;
        icon.sprite = icon_img; description.text = _description;
    }
}
