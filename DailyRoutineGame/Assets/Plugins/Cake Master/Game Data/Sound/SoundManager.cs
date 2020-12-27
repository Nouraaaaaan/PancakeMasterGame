using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource instrumentSource;
    public AudioMixer mainMixer;

    [Header("Clips")]
    [SerializeField] AudioClip winClip;
    [SerializeField] AudioClip changeIntrumentClip;

    public void Win()
    {
        sfxSource.PlayOneShot(winClip);
    }

    public void Change()
    {
        sfxSource.PlayOneShot(changeIntrumentClip);
    }

    public void Instrument(bool value)
    {
        if (value && !instrumentSource.isPlaying)
            instrumentSource.Play();
        else if (!value && instrumentSource.isPlaying)
            instrumentSource.Stop();
    }
}
