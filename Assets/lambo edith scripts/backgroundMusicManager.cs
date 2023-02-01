using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundMusicManager : MonoBehaviour
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
        backgroundMusic.volume = settingsDetails.musicVal;
    }

}
