using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SceneLoader : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string sceneToLoad;

    void Start()
    {
        videoPlayer.loopPointReached += LoadScene;
    }

    void LoadScene(VideoPlayer vp)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
