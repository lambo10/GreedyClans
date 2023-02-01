using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class infoBtn : MonoBehaviour
{
    public int infoBtnIndex;
    public GameObject infoModal;
    public lb_nfts_details _nfts;


    public void click()
    {
        lb_nft nftDetails = _nfts.nfts[infoBtnIndex];
        var info_M_Manager = infoModal.GetComponent<infoModalManager>();
        info_M_Manager.passDetails(nftDetails.name, nftDetails.health, nftDetails.damage, nftDetails.speed, nftDetails.range, nftDetails.description, nftDetails.icon);
        infoModal.SetActive(true);
    }

}
