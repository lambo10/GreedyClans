using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectWallet : MonoBehaviour
{
    public void navigateToCreateWallet(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("CreateWallet");
    }
 
    public void navigateToImportWallet(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("ImportWallet");
    }
}
