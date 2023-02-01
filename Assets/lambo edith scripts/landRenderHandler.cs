using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class landRenderHandler : MonoBehaviour
{
    public GameObject tileMap1;
    public GameObject tileMap2;
    public GameObject tileMap3;

    void Start()
    {
        if (int.Parse(playerDetails.landNo) == 1 || int.Parse(playerDetails.landNo) == 2 || int.Parse(playerDetails.landNo) == 5)
        {
            tileMap1.SetActive(true);
            tileMap2.SetActive(false);
            tileMap3.SetActive(false);
        }else if (int.Parse(playerDetails.landNo) == 3)
        {
            tileMap1.SetActive(false);
            tileMap2.SetActive(true);
            tileMap3.SetActive(false);
        }
        else if (int.Parse(playerDetails.landNo) == 4)
        {
            tileMap1.SetActive(false);
            tileMap2.SetActive(false);
            tileMap3.SetActive(true);
        }
    }

}
