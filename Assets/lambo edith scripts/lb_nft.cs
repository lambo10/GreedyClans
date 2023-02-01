using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class lb_nft : MonoBehaviour
{
    [SerializeField] public string name;
    [SerializeField] public string id;
    [SerializeField] public Sprite icon;
    [SerializeField] public Sprite fullBodyShinnyIcon;
    [SerializeField] public double price;
    [SerializeField] public int health;
    [SerializeField] public int damage;
    [SerializeField] public int speed;
    [SerializeField] public int range;
    [SerializeField] public string description;

}
