using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField]Slider soundSlider, musicSlider;
    [SerializeField] Button pushAlarmToggler,vibrationToggler;
    [SerializeField] Sprite switchOnTail, switchOnHead;
    [SerializeField] Sprite switchOffTail, switchOffHead;
    public float soundVal, musicVal;
    [SerializeField] int onValue = 43, offValue = -43;
    public bool pushAlarmVal, vibrationVal;

    string key = "skslalaksldia3i334ab6cd-6e2f-4094-88d6-d24215debdcd";
    // Start is called before the first frame update
    void Start()
    {
        soundSlider.onValueChanged.AddListener((float value) =>
        {
            setSoundFx(value);
        });
        musicSlider.onValueChanged.AddListener((float value) =>
        {
            setMusic(value);
        });

        pushAlarmToggler.onClick.AddListener(() =>
        {
            setPushAlarm(!pushAlarmVal);
        });

        vibrationToggler.onClick.AddListener(() =>
        {
            setVibration(!vibrationVal);
        });


        string managerKey = PlayerPrefs.GetString(key);
        SettingJson settingJson;
        if (managerKey != "")
        {
            settingJson = JsonUtility.FromJson<SettingJson>(managerKey);
            soundSlider.value = settingJson.soundFx;
            musicSlider.value = settingJson.music;
            setVibration(settingJson.vibration);
            setPushAlarm(settingJson.pushAlarm);
        }
        else
        {
            settingJson = new SettingJson();
            soundSlider.value = 1;
            musicSlider.value = 1;
            setVibration(true);
            setPushAlarm(true);
            PlayerPrefs.SetString(key, JsonUtility.ToJson(settingJson));
        }
        soundVal = soundSlider.value;
        musicVal = musicSlider.value;
    }

    public void setMusic(float value)
    {

        musicVal = value;
        string managerKey = PlayerPrefs.GetString(key);
        SettingJson settingJson;
        if (managerKey != "")
        {
             settingJson = JsonUtility.FromJson<SettingJson>(managerKey);

        }else
        {
             settingJson= new SettingJson();
        }

        settingJson.music = value;
        PlayerPrefs.SetString(key,JsonUtility.ToJson(settingJson));
    }

    public void setSoundFx(float value)
    {
        soundVal = value;
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

    public void setPushAlarm(bool value)
    {
        GameObject pushGameObj = pushAlarmToggler.gameObject;
        pushGameObj.GetComponent<Image>().sprite = value ? switchOnTail : switchOffTail;
        Transform childOfPush = pushGameObj.transform.GetChild(0);
        Vector3 formerPosition = childOfPush.gameObject.transform.localPosition;
        Debug.Log(formerPosition.x);
        formerPosition.x = value ? onValue : offValue;
        childOfPush.gameObject.transform.localPosition = formerPosition;
        childOfPush.GetComponent<Image>().sprite = value ? switchOnHead : switchOffHead;

        pushAlarmVal = value;
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

        settingJson.pushAlarm = value;
        PlayerPrefs.SetString(key, JsonUtility.ToJson(settingJson));
    }

    public void setVibration(bool value)
    {
        GameObject vibrationObj = vibrationToggler.gameObject;
        vibrationObj.GetComponent<Image>().sprite = value ? switchOnTail : switchOffTail;
        Transform childOfVibr = vibrationObj.transform.GetChild(0);
        Vector3 formerPosition = childOfVibr.gameObject.transform.localPosition;
        formerPosition.x = value ? onValue : offValue;
        childOfVibr.gameObject.transform.localPosition = formerPosition;
        childOfVibr.GetComponent<Image>().sprite = value ? switchOnHead : switchOffHead;
        vibrationVal = value;
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

        settingJson.vibration = value;
        PlayerPrefs.SetString(key, JsonUtility.ToJson(settingJson));
    }
}

public class SettingJson
{
    public bool pushAlarm;
    public float soundFx;
    public float music;
    public bool vibration;
}
