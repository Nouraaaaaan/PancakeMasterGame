using UnityEngine;
using UnityEngine.UI;

public class Stars : MonoBehaviour
{
    [SerializeField] Color activeColor, inactiveColor;
    [Space]
    [SerializeField] Image[] stars;

    public void SetStars(int amount)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (i < amount)
                stars[i].color = activeColor;
            else
                stars[i].color = inactiveColor;
        }
    }
}
