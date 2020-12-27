using UnityEngine;
using UnityEngine.UI;

public class ColorChooser : MonoBehaviour
{
    [SerializeField] Image image;
    Color color;

    public void SetColor(Color color)
    {
        this.color = color;
        image.color = color;
    }

    public void SelectColor()
    {
        Gameplay.Instance.SelectColor(color, transform);
    }
}
