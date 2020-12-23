using UnityEngine;
using UnityEngine.UI;

public class UIManagerCakeMaster : MonoBehaviour
{
    public static UIManagerCakeMaster Instance = null;

   // [Header("Panels")]
   // [SerializeField] Animator winAnimator;
    //[SerializeField] Animator gameAnimator;

    //[Header("Components")]
   // [SerializeField] Text gameStateText;
    //[SerializeField] Text levelText;
   // [SerializeField] Text ratingText;
    //[SerializeField] Image fillImage;
   //[SerializeField] Stars stars;

    //[Header("Match")]
    //[SerializeField] Text matchText;
   // [SerializeField] Image matchFill;

    //Animator gameStateAnim;

    private  void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

       // gameStateAnim = gameStateText.gameObject.GetComponent<Animator>();
    }

    public void ChangeFill(float fill)
    {
       // UIManager.instance.UpdateSliderVale(fill);
       // fillImage.fillAmount = fill;
    }

   /* public void SetLevel(int level)
    {
        levelText.text = "Level " + level;
    }*/

    public void Win(float match, int starAmount)
    {
       // matchText.text = (int)(match * 100) + "%";
       // matchFill.fillAmount = match;

        //winAnimator.gameObject.SetActive(true);

        //stars.SetStars(starAmount);
    }

    public void ChangeToSmoothing()
    {
       // UIManager.instance.StartCounterTime("Smooth it Out!");
       // gameStateAnim.SetTrigger("Change");
    }

    public void UpdateStars(int amount)
    {
       // stars.SetStars(amount);
    }

   /* public void UpdateRating(int stars)
    {
        ratingText.text = stars.ToString();
    }*/
}
