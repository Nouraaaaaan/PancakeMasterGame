using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerCakeMaster : Singleton<GameManagerCakeMaster>
{
   
    [SerializeField] ParticleSystem endParticles;

    int level = 1;
    int star = 0;
    public int Star
    {
        get
        {
            return star;
        }
        set
        {
            star = value;
            //UIManagerCakeMaster.Instance.UpdateRating(star);
        }
    } 
    public int Level
    {
        get
        {
            return level;
        }
        set
        {
            level = value;
            //UIManagerCakeMaster.Instance.SetLevel(level);
        }
    }
    int stars = 0;
    bool gameOver = false;


    protected override void Awake()
    {
        base.Awake();

        
    }

    private void Start()
    {
        //Level = PlayerPrefs.GetInt("Level", 1);
        Star = PlayerPrefs.GetInt("Stars", 0);

        /*if (ServicesManager.instance != null)
        {
            ServicesManager.instance.InitializeAdmob();
            ServicesManager.instance.InitializeUnityAds();
        }*/
    }

    public void Reload()
    {
        Star += stars;
        PlayerPrefs.SetInt("Stars", Star);
        SceneManager.LoadScene(0);
    }

    public void GameOver(float match)
    {
        if (!gameOver)
        {
            gameOver = true;
            //PlayerPrefs.SetInt("Level", Level + 1);

            if (match > 0.2f)
                stars++;

            if (match > 0.5f)
                stars++;

            if (match >= 0.9f)
                stars++;

           // UIManagerCakeMaster.Instance.UpdateRating(Star + stars);

            StartCoroutine(GameOverSteps(match));
            SoundManager.Instance.Win();
        }
    }

    IEnumerator GameOverSteps(float match)
    {
        endParticles.Play();

        yield return new WaitForSeconds(0.7f);
      //  UIManager.instance.ShowWinPanel();
        //EmojiParticles_Manager.instance.PlayRandomGoodParticles();

        /* if (ServicesManager.instance != null)
         {
             ServicesManager.instance.ShowInterstitialAdmob();
             ServicesManager.instance.ShowInterstitialUnityAds();
         }*/
    }

    public void GetAllStars()
    {

       // UIManagerCakeMaster.Instance.UpdateRating(Star + stars);
    }

    public void RewardedCompleted(bool completed)
    {
        stars = 3;
       // UIManagerCakeMaster.Instance.UpdateStars(3);
    }
}
