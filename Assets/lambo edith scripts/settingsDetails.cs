using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class settingsDetails : MonoBehaviour
{
    public static float soundVal, musicVal;
    public static bool pushAlarmVal, vibrationVal;
    string key = "skslalaksldia3i334ab6cd-6e2f-4094-88d6-d24215debdcd";

    private void Start()
    {
        getSettingsData(key);
        DontDestroyOnLoad(this.gameObject);
    }

    public static void getSettingsData(string key)
    {
        string managerKey = PlayerPrefs.GetString(key);
        if (managerKey == null)
            return;
        SettingJson settingJson = JsonUtility.FromJson<SettingJson>(managerKey);
        soundVal = settingJson.soundFx;
        musicVal = settingJson.music;
        pushAlarmVal = settingJson.pushAlarm;
        vibrationVal = settingJson.vibration;

    }
}
