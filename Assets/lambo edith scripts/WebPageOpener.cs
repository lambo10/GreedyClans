using UnityEngine;
using System.Collections;

public class WebPageOpener : MonoBehaviour
{
    public void OpenWebPage(string url)
    {
        Application.OpenURL(url);
    }
}
