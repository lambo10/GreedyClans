using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class removeNetworkObjOnStart : MonoBehaviour
{
    void Start()
    {
        if(int.Parse(Helper.chainId) == 4000)
        {
            ServerRpc.removeNetworkObject(this.gameObject);
        }
    }

}
