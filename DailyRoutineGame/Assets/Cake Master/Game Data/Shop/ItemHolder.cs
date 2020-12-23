using UnityEngine;
using UnityEngine.UI;

public class ItemHolder : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Text costText;

    [Space]
    [SerializeField] Image[] imageColors;
    [SerializeField] Color unlockedColor, lockedColor;

    CakeData myData;

    public void SetItem(CakeData data)
    {
        myData = data;
        icon.sprite = data.icon;

        if (data.IsUnlocked)
        {
            costText.text = "Selected";
        }
        else
        {
            costText.text = data.cost.ToString();
        }

        for (int i = 0; i < imageColors.Length; i++)
        {
            imageColors[i].color = data.IsUnlocked ? unlockedColor : lockedColor;
        }
    }

    public void Buy()
    {
        if (GameManagerCakeMaster.Instance.Star >= myData.cost)
        {
            GameManagerCakeMaster.Instance.Star -= myData.cost;
            myData.IsUnlocked = true;
            SetItem(myData);
        }
    }
}
