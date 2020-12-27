using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextEffect : MonoBehaviour
{
    [Header("Text")]
    public bool HasText;
    public Text Text;
    public string[] TextQuotes;

    [Header("Particles")]
    public bool HasParticles;
    public ParticleSystem ParticleSystem;

    [Header("Image")]
    public bool HasImage;
    public Image Image;
    public Sprite[] Images;
    private bool CanShowImage = true;


    public void PlayEffect()
    {
        if (HasText)
        {
            SetRandomText();

            LeanTween.scale(Text.rectTransform, new Vector3(1f, 1f, 1f), 0.5f);
            LeanTween.scale(Text.rectTransform, new Vector3(0f, 0f, 0f), 0.5f).setDelay(1.5f);
        }

        if (HasParticles)
        {
            PlayRandomParticles();
        }

        if (HasImage & CanShowImage)
        {
            ShowImage();
        }

    }

    private void ShowImage()
    {
        CanShowImage = false;
        int randomIndex = Random.Range(0, Images.Length);
        Image.GetComponent<Image>().sprite = Images[randomIndex];     
        LeanTween.scale(Image.rectTransform, new Vector3(1f, 1f, 1f), 0.3f);
        LeanTween.scale(Image.rectTransform, new Vector3(0f, 0f, 0f), 0.3f).setDelay(1.2f).setOnComplete(ToggleImageStatus);
    }

    private void ToggleImageStatus()
    {
        CanShowImage = true;
    }

    private void SetRandomText()
    {
        int randomIndex = Random.Range(0, TextQuotes.Length);
        Text.text = TextQuotes[randomIndex];
    }

    private void PlayRandomParticles()
    {
        ParticleSystem.Play();
    }

}
