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
    public SaveTest SaveTest;

    public AudioSource AudioSource;
    public AudioSource MusicAudioSource;
    public AudioClip[] AudioClips;

    public GameObject MusicOnButton;
    public GameObject MusicOffButton;

    public GameObject SFXOnButton;
    public GameObject SFXOffButton;

    public bool CanPlaySFX;
    public bool CanPlayMusic;

    private void Start()
    {
        SaveTest.Load();

        CanPlayMusic = SaveTest.SaveObject.CanPlayMusic;
        if (CanPlayMusic)
        {
            TurnOnMusic();
        }
        else
        {
            TurnOffMusic();
        }

        CanPlaySFX = SaveTest.SaveObject.CanPlaySFX;
        if (CanPlaySFX)
        {
            TurnOnSFX();
        }
        else
        {
            TurnOffSFX();
        }
    }
    public void PlaySoundEffect(int index)
    {
        if (CanPlaySFX)
        {
            AudioSource.clip = AudioClips[index];
            AudioSource.Play();
        }
    }

    public void StopSoundEffect()
    {
        AudioSource.Stop();
    }

    public void StopLoopingOption()
    {
        AudioSource.loop = false;
    }

    public void EnableLoopingOption()
    {
        AudioSource.loop = true;
    }

    public void SetAudioVolume(float value)
    {
        AudioSource.volume = value;
    }

    public void TurnOnSFX()
    {
        CanPlaySFX = true;
        SaveTest.SaveObject.CanPlaySFX = true;
        SaveTest.Save();

        if (SFXOnButton != null & SFXOffButton != null)
        {
            SFXOnButton.SetActive(true);
            SFXOffButton.SetActive(false);
        }
    }

    public void TurnOffSFX()
    {
        CanPlaySFX = false;
        SaveTest.SaveObject.CanPlaySFX = false;
        SaveTest.Save();

        if (SFXOnButton != null & SFXOffButton != null)
        {
            SFXOnButton.SetActive(false);
            SFXOffButton.SetActive(true);
        }
    }

    public void TurnOnMusic()
    {
        MusicAudioSource.Play();
        SaveTest.SaveObject.CanPlayMusic = true;
        CanPlayMusic = true;
        SaveTest.Save();

        if (MusicOnButton != null & MusicOffButton != null)
        {
            MusicOnButton.SetActive(true);
            MusicOffButton.SetActive(false);
        }
    }

    public void TurnOffMusic()
    {
        MusicAudioSource.Stop();
        SaveTest.SaveObject.CanPlayMusic = false;
        CanPlayMusic = false;
        SaveTest.Save();

        if (MusicOnButton != null & MusicOffButton != null)
        {
            MusicOnButton.SetActive(false);
            MusicOffButton.SetActive(true);
        }
    }
}
