using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lb_nfts_details : MonoBehaviour
{
    
    public string [] _name;
    public string [] id;
    public Sprite [] icon;
    public double [] price;
    public int [] health;
    public int [] damage;
    public int [] speed;
    public int [] range;
    public string [] description;
    public Sprite [] fullBodyShinnyIcon;

    public List<lb_nft> nfts;

    private void Start()
    {
         for(int i=0; i < _name.Length; i++){
            lb_nft wkrValue = new lb_nft();
            wkrValue.name = _name[i];
            wkrValue.id = id[i];
            wkrValue.icon = icon[i];
            wkrValue.price = price[i];
            wkrValue.health = health[i];
            wkrValue.damage = damage[i];
            wkrValue.speed = speed[i];
            wkrValue.range = range[i];
            wkrValue.description = description[i];
            wkrValue.fullBodyShinnyIcon = fullBodyShinnyIcon[i];
            nfts.Add(wkrValue);

        }
    }

}
