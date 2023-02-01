using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadNewScene : MonoBehaviour
{
    public void loadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
