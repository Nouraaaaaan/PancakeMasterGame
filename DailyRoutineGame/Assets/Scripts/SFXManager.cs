using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    #region Singelton
    public static SFXManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    #endregion

    public AudioSource AudioSource;
    public AudioClip[] AudioClips;

    public void PlaySoundEffect(int index)
    {     
        /*
        if (GameData.Instance.GetSoundSetting().Equals("false"))
            return; 
        */
        
        AudioSource.clip = AudioClips[index];
        AudioSource.Play();
    }

    public void StopSoundEffect()
    {
        AudioSource.Stop();
    }

    public void StopLoopingOption()
    {
        AudioSource.loop = false;
    }
}
