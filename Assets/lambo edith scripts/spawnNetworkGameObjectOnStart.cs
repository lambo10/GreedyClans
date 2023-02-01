using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class spawnNetworkGameObjectOnStart : NetworkBehaviour
{

    void Start()
    {
        if (int.Parse(Helper.chainId) != 4000)
      { 
            NetworkObject gameObjNetworkObj = this.gameObject.GetComponent<NetworkObject>();
            gameObjNetworkObj.Spawn(true);
        }
    }

  
}
