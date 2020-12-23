using System.Collections;
using UnityEngine;

public enum DonutState
{
     syrup, sweets
}

public class Donutes_LevelManager : MonoBehaviour
{
    public static Donutes_LevelManager instance = null;

    [Header("Donutes")]
    public DonutState donutState;
    [SerializeField] Donut donut;
    [SerializeField] DonutSyrup donutSyrup;
    [SerializeField] DonutSweet donutSweet;

    [Header("Cookies")]
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject[] cookiePrefabs;

    [Header("Camera")]
    [SerializeField] Camera mainCAmera;
    [SerializeField] float  syrupCam, sweetsCam; // store X positions

    [Header("Effects")]
    [SerializeField] ParticleSystem endParticles;

    private bool isPlaying = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        PrepareCookie();
        this.StartLevelPlaying();

        //UIManager.instance.EnableSlider();
       // UIManager.instance.StartCounterTime("Lets Decorate Cookies");
    }

    private void PrepareCookie()
    {
        donut = Instantiate(cookiePrefabs[Random.Range(0, cookiePrefabs.Length)], spawnPoint).GetComponent<Donut>();
        donutSyrup.donut = donut;
        donutSweet.donut = donut;
    }

    public  void StartLevelPlaying()
    {
        isPlaying = true;
        donutState = DonutState.syrup;
    }



    public void OnLevelAction(bool good)
    {
        if (!isPlaying)
            return;

        if (good)
        {
            switch (donutState) 
            {
                case DonutState.syrup:
                    StartCoroutine(SyrupDone());
                    donutState = DonutState.sweets;
                   // UIManager.instance.UpdateSliderVale(0.5f);
                    break;
                case DonutState.sweets:
                    this.LevelCompleted();
                   // UIManager.instance.UpdateSliderVale(1f);
                    break;
                default:
                    break;
            }
          //  Vibration.Vibrate(70);
        }
        else
        {
           // Vibration.Vibrate(300);
           // UIManager.instance.DecreaseLive();
        }
    }


    public  void OnBadLevelAction()
    {
        if (!isPlaying)
            return;

       //Vibration.Vibrate(300);
        //  UIManager.instance.DecreaseLive();
    }

    public void LevelCompleted()
    {
        isPlaying = false;
        endParticles.Play();
        //UIManager.instance.ShowWinPanel();
    }

    IEnumerator SyrupDone()
    {
        yield return StartCoroutine(MoveCam(syrupCam, sweetsCam));

        donutSweet.enabled = true;
        donutSweet.canSelect = true;
    }



    IEnumerator MoveCam(float from, float to)
    {
        yield return new WaitForSeconds(0.3f);

        float time = 0;
        while (time < 0.75f)
        {
            time += Time.deltaTime;
            mainCAmera.transform.localPosition = new Vector3(Mathf.Lerp(from, to, time / 0.75f),
                mainCAmera.transform.localPosition.y, mainCAmera.transform.localPosition.z);
            donut.transform.parent.localPosition = new Vector3(Mathf.Lerp(from, to, time / 0.75f),
                donut.transform.parent.localPosition.y, donut.transform.parent.localPosition.z);
            yield return null;
        }
    }

}
