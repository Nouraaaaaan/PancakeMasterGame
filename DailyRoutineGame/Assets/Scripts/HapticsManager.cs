using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class HapticsManager : MonoBehaviour
{
    public static HapticsManager Instance;
    private bool CanPlayHaptic = true;

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
        if(CanPlayHaptic)
           MMVibrationManager.Haptic(hapticType, false, true);
    }

    public void RepetitiveHaptic(int duration)
    {
        if(CanPlayHaptic)
           StartCoroutine(RepetitiveHapticCoroutine(duration));
    }

    private IEnumerator RepetitiveHapticCoroutine(int duration)
    {
        for (int i = 0; i < duration; i++)
        {
            MMVibrationManager.Haptic(HapticTypes.HeavyImpact, false, true);

            yield return new WaitForSeconds(0.15f);
        }
    }

    public void TurnOnHaptic()
    {
        CanPlayHaptic = true;
    }

    public void TurnOffHaptic()
    {
        CanPlayHaptic = false;
    }
}
