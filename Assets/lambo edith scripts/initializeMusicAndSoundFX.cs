using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class initializeMusicAndSoundFX : MonoBehaviour
{
    string key = "skslalaksldia3i334ab6cd-6e2f-4094-88d6-d24215debdcd";

    void Start()
    {
        string managerKey = PlayerPrefs.GetString(key);
        SettingJson settingJson;
        if (managerKey == "")
        {
            setMusic(1f);
            setSoundFx(1f);
        }
    }

    public void setMusic(float value)
    {
        string managerKey = PlayerPrefs.GetString(key);
        SettingJson settingJson;
        if (managerKey != "")
        {
            settingJson = JsonUtility.FromJson<SettingJson>(managerKey);

        }
        else
        {
            settingJson = new SettingJson();
        }

        settingJson.music = value;
        PlayerPrefs.SetString(key, JsonUtility.ToJson(settingJson));
    }

    public void setSoundFx(float value)
    {
        string managerKey = PlayerPrefs.GetString(key);
        SettingJson settingJson;
        if (managerKey != "")
        {
            settingJson = JsonUtility.FromJson<SettingJson>(managerKey);

        }
        else
        {
            settingJson = new SettingJson();
        }

        settingJson.soundFx = value;
        PlayerPrefs.SetString(key, JsonUtility.ToJson(settingJson));
    }

}
