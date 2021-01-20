using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class HapticsManager : MonoBehaviour
{
    public static HapticsManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("AdmobManager Singletone Error");
        }

        //DontDestroyOnLoad(this.gameObject);
    }

    public void HapticPulse(HapticTypes hapticType)
    {
        MMVibrationManager.Haptic(hapticType, false, true);
    }
}
