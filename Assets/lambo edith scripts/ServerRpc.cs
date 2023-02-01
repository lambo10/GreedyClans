using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;

public class ServerRpc : NetworkManager
{

    private relayManager _relayManager;
    public static string createdJoinCode;
    bool Relayconnected = false;

    void checke_network_reachability(UnityEngine.Events.UnityAction _fun)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            networkErroPanelManager.canShow = true;
            networkErroPanelManager._functionToExecute = _fun;
        }
        else
        {
            networkErroPanelManager.canShow = false;
        }
    }

    public static void removeNetworkObject(GameObject gameObject)
    {
        NetworkObject gameObjNetworkObj = gameObject.GetComponent<NetworkObject>();
        if (gameObjNetworkObj != null) {
            DestroyImmediate(gameObjNetworkObj);
            Debug.Log(gameObject.name + " >>>>>>>>>>>>>");
        }

    }


    public static void SpawnObjectServerRpc(GameObject gameObject)
    {
        if (gameObject == null)
        {
            Debug.Log("game object can not be null ");
            return;
        }
        string activeSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        NetworkObject gameObjNetworkObj = gameObject.GetComponent<NetworkObject>();
        if (!activeSceneName.Equals("Map01"))
        {
            if (gameObjNetworkObj != null)
            {
                DestroyImmediate(gameObjNetworkObj);
            }
            return;
        }
        if (int.Parse(Helper.chainId) == 4000)
        {
            //serverSpawn(gameObject, gameObjNetworkObj, activeSceneName);
            if (gameObjNetworkObj != null) {
                gameObjNetworkObj.enabled = false;
            }
        }
        else
        {
            serverSpawnServerRpc(gameObject, gameObjNetworkObj, activeSceneName);
        }


    }

    [ServerRpc]
    public static void serverSpawnServerRpc(GameObject gameObject, NetworkObject gameObjNetworkObj, string sceneName)
    {
        if (sceneName.Equals("Map01") && gameObjNetworkObj == null)
        {
            Debug.Log("add network object to " + gameObject.name);
            return;
        }
        Debug.Log("network object added to " + gameObject.name);
        gameObjNetworkObj.Spawn(true);
    }


    public static void serverSpawn(GameObject gameObject, NetworkObject gameObjNetworkObj, string sceneName)
    {
        if (int.Parse(Helper.chainId) != 4000) {
            if (sceneName.Equals("Map01") && gameObjNetworkObj == null)
            {
                Debug.Log("add network object to " + gameObject.name);
                return;
            }
            Debug.Log("network object added to " + gameObject.name);
            gameObjNetworkObj.Spawn(true);
        }
    }


    void Start()
    {
        checke_network_reachability(startLayer2);
        startLayer2();
    }

    void startLayer2()
    {
        checke_network_reachability(startLayer2);
        _relayManager = this.gameObject.GetComponent<relayManager>();
        _relayManager.authRelayServer();
    }

    public void _startClient()
    {
        _relayManager.createRelayID();
        StartClient();
    }

    void Update()
    {
        updatelayer2();
    }

    void updatelayer2()
    {
        if (!Relayconnected)
        {
            if (_relayManager.serverInstance)
            {
                if (_relayManager._playerID.Length < 1 || _relayManager._playerID == null)
                { }
                else
                {
                    checke_network_reachability(updatelayer2);
                    _relayManager.createRelayID();
                    StartServer();
                    Relayconnected = true;
                }
            }
        }
    } 


}