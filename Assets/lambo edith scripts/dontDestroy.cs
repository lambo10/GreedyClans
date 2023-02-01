using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class dontDestroy : MonoBehaviour
{
    private bool executed = false;
    private string previousSceneName;
    void Update()
    {
        if (!executed)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("backgroundMusic");

            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }
            if (SceneManager.GetActiveScene().name.Equals("Game") || SceneManager.GetActiveScene().name.Equals("Map01"))
            {
                 Destroy(this.gameObject);
            }
            else
            {
                DontDestroyOnLoad(this.gameObject);
            }
            executed = true;
            previousSceneName = SceneManager.GetActiveScene().name;
        }

        if (!SceneManager.GetActiveScene().name.Equals(previousSceneName))
        {
            executed = false;
        }


    }
}

