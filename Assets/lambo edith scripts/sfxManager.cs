using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sfxManager : MonoBehaviour
{
    private void Start()
    {
        setVolume();
    }

    void setVolume()
    {
        AudioSource backgroundMusic = GetComponent<AudioSource>();
        if (backgroundMusic == null)
            return;
        backgroundMusic.volume = settingsDetails.soundVal;
    }

}
