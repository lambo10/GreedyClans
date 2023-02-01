using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkExpiredSession : MonoBehaviour
{
    public static void checkSessionExpiration(string response)
    {
        if (response.Equals("session does not exsist"))
        {
            navigateToLogin();
        }
    }
    static void navigateToLogin()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
    }
}
