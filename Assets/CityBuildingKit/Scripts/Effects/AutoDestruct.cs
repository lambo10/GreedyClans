using UnityEngine;
using System.Collections;

public class AutoDestruct : MonoBehaviour
{
    public float DestructTime = 2.0f;

    void Start()
    {
        if (int.Parse(Helper.chainId) != 4000)
        {
            ServerRpc.SpawnObjectServerRpc(this.gameObject);
        }
        else
        {
            ServerRpc.removeNetworkObject(this.gameObject);
        }
        transform.parent = GameObject.Find("GroupEffects").transform;
        Destroy(gameObject, DestructTime);
    }
}
