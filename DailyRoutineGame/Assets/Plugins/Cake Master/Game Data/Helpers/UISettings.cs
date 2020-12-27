using UnityEngine.UI;
using UnityEngine;

public class UISettings : MonoBehaviour
{
    public GameObject active, inactive;

    public string settingName;
    public bool isActive = true;

    private void Start()
    {
        if (PlayerPrefs.GetInt(settingName, 1) == 1)
            isActive = true;
        else
            isActive = false;

        UpdateState();
    }

    private void UpdateState()
    {
        if (isActive)
            SoundManager.Instance.mainMixer.SetFloat(settingName, 0);
        else
            SoundManager.Instance.mainMixer.SetFloat(settingName, -80);

        active.SetActive(isActive);
        inactive.SetActive(!isActive);
    }

    public void SetSettings()
    {
        isActive = !isActive;

        if (isActive)
            PlayerPrefs.SetInt(settingName, 1);
        else
            PlayerPrefs.SetInt(settingName, 0);

        UpdateState();
    }
}
