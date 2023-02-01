using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
using Assets.Scripts.Menus;

public class relayManager : MonoBehaviour
{
    public string _playerID = null;
    public string _joinCode = null;
    public bool serverInstance = false;
    // Start is called before the first frame update
    void Start()
    {
        //StartThread();
    }

    public void StartThread()
    {
        // Create a new thread and pass it the function to be executed
        Thread thread = new Thread(authRelayServer);

        // Start the thread
        thread.Start();
    }

    public void authRelayServer()
    {
         UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Relay Signed In => playerID =>" + AuthenticationService.Instance.PlayerId);
            _playerID = AuthenticationService.Instance.PlayerId;
        };
         AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void createRelayID()
    {
        try
        {
            Allocation _allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(_allocation.AllocationId);
            Debug.Log("Created joinCode =>"+ joinCode);
            ServerRpc.createdJoinCode = joinCode;
            RelayServerData _relayServerData = new RelayServerData(_allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(_relayServerData);
            NetworkManager.Singleton.StartServer();
            _joinCode = joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    public static async Task<bool> joinRelay(string joinCode) {
            try
            {
                Debug.Log("Joining Relay with "+ joinCode);
                JoinAllocation _joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                RelayServerData _relayServerData = new RelayServerData(_joinAllocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(_relayServerData);
                Debug.Log("JoinedRelay with " + joinCode);
                MessageController.Instance.DisplayMessage("JoinedRelay with " + joinCode);
            return true;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e+"-------------Here");
                MessageController.Instance.DisplayMessage(e.ToString());
            return false;
            }
 
        }



}
