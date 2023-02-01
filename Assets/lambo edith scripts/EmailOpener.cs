using UnityEngine;
using System.Collections;

public class EmailOpener : MonoBehaviour
{
    public void OpenEmailApp(string email)
    {
        Application.OpenURL("mailto:" + email);
    }
}
