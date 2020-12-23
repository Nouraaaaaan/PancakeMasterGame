using Es.InkPainter;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MakeCookie_LevelManager : MonoBehaviour
{
    public static MakeCookie_LevelManager instance = null;
    private enum MakeCookieState
    {
        filling, backing, topping, toppingDone, sweets, sweetsDone
    }
    private MakeCookieState makeCookieState;

    [Header("Sweets Spawner")]
    [SerializeField] SweetsSpawnerfiller sweetsSpawnerfiller;
    [SerializeField] GameObject fillerModel, canModel;

    [Header("Filler data")]
    [SerializeField] Transform fillerTransform;
    [SerializeField] ParticleSystem fillerParticle;
    [SerializeField] LayerMask PaintLayer;
    [SerializeField] Vector4 fillerXZrange = new Vector4(30.7f, 33.1f, -5.45f, -2.25f);  // loclaposition

    [Header("Cookie Prefabs")]
    [SerializeField] Transform templatePlace;
    [SerializeField] GameObject[] templatesPrefabs;
    private CookieTemplateHolder currentHolder;
    private InkCanvas currentPainter, toppingPainter;
    private CakeIcing cakeIcing;

    [Header("Camera")]
    [SerializeField] Transform posA;
    [SerializeField] Transform posB;
    [SerializeField] Transform posC;

    [Header("UI")]
    [SerializeField] GameObject buttonCanvas;

    [Header("Effects")]
    [SerializeField] ParticleSystem endParticles;
    [SerializeField] AudioSource bakeSound;

    [Header("Paint Brush")]
    [SerializeField] Brush brush;

    [Header("Bake Settings")]
    [SerializeField] Color bakecolor;
    [SerializeField] Vector2 yPosRangesBack = new Vector2(0.3f, 0.5f);

    private Touch touch;
    private Vector3 offset, fillerStartPos, newPos;
    private Camera mainCamera;
    private float castDistance;
    private RaycastHit hit;

    private bool canPaint;
    //private bool bakeCalled = false;
    private float toppingTime = 0;
    private bool sweetsDone = false;

    public bool isPlaying = false;

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
        MakeTemplatePrefabe();
        /*if (UIManager.instance)
        {
            UIManager.instance.EnableSlider();
            UIManager.instance.StartCounterTime("Let’s Bake Cookies");
        }*/

        mainCamera = Camera.main;
        fillerStartPos = fillerTransform.position;
        castDistance = Vector3.Distance(mainCamera.transform.position, fillerTransform.position);     

        this.StartLevelPlaying();
    }

    private void MakeTemplatePrefabe()
    {
        GameObject obj = Instantiate(templatesPrefabs[Random.Range(0, templatesPrefabs.Length)], templatePlace);
        //GameObject obj = Instantiate(templatesPrefabs[0], templatePlace);
        currentHolder = obj.GetComponent<CookieTemplateHolder>();
        currentPainter = obj.transform.GetChild(1).GetComponent<InkCanvas>();
        toppingPainter = currentHolder.cookieTopping.GetComponent<InkCanvas>();
        cakeIcing = currentHolder.cake;
    }

    public void StartLevelPlaying()
    {
        isPlaying = true;
        StartCoroutine(MoveCamTo(posA, posB, true));
        currentPainter.PaintUVDirect(brush, Vector2.zero);
        makeCookieState = MakeCookieState.filling;
    }

    IEnumerator MoveCamTo(Transform from, Transform to, bool toLevel)
    {
        float time = 0;

        if (toLevel)
        {
            yield return new WaitForSeconds(0.5f);
        }

        while (time < 1.2f)
        {
            time += Time.deltaTime;
            mainCamera.transform.position = Vector3.Lerp(from.position, to.position, time / 1.2f);
            mainCamera.transform.rotation = Quaternion.Lerp(from.rotation, to.rotation, time / 1.2f);
            yield return null;
        }

        if (toLevel)
        {
            canPaint = true;
        }
    }


    private void Update()
    {
        if (!isPlaying || !canPaint)
            return;

        if (Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (makeCookieState == MakeCookieState.sweets || makeCookieState == MakeCookieState.sweetsDone)
                    {
                        sweetsSpawnerfiller.StartSpawning();
                    }
                    fillerParticle.Play();
                    offset = fillerTransform.position -
                        mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, castDistance));
                    break;
                case TouchPhase.Moved:
                    newPos = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, castDistance)) + offset;
                    MousePressed();
                    break;
                case TouchPhase.Stationary:
                    newPos = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, castDistance)) + offset;
                    MousePressed();
                    break;
                case TouchPhase.Ended:
                    MouseRelease();
                    break;
                case TouchPhase.Canceled:
                    MouseRelease();
                    break;
                default:
                    break;
            }
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            //currentPainter.isPainting = true;           
            if (makeCookieState == MakeCookieState.sweets || makeCookieState == MakeCookieState.sweetsDone)
            {
                sweetsSpawnerfiller.StartSpawning();
            }
            fillerParticle.Play();
            offset = fillerTransform.position -
                mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, castDistance));
        }

        if (Input.GetMouseButton(0))
        {
            newPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, castDistance)) + offset;
            MousePressed();
        }

        if (Input.GetMouseButtonUp(0))
        {
            MouseRelease();
        }

        Debug.DrawRay(fillerParticle.transform.position, -Vector3.up * 10);
        
#endif

    }

    private void MousePressed()
    {
        switch (makeCookieState)
        {
            case MakeCookieState.filling:
                fillerTransform.transform.localPosition = new Vector3(Mathf.Clamp(newPos.x, fillerXZrange.x, fillerXZrange.y),
                fillerTransform.localPosition.y, Mathf.Clamp(newPos.z, fillerXZrange.z, fillerXZrange.w));

                if (Physics.Raycast(fillerParticle.transform.position, -Vector3.up, out hit, 10, PaintLayer))
                {
                    //Vibration.Vibrate(10);
                    currentPainter.Paint(brush, hit);
                    currentHolder.Rise();
                }
                break;
            case MakeCookieState.backing:
                fillerTransform.transform.localPosition = new Vector3(Mathf.Clamp(newPos.x, fillerXZrange.x, fillerXZrange.y),
                fillerTransform.localPosition.y, Mathf.Clamp(newPos.z, fillerXZrange.z, fillerXZrange.w));

                if (Physics.Raycast(fillerParticle.transform.position, -Vector3.up, out hit, 10, PaintLayer))
                {
                   // Vibration.Vibrate(10);
                    currentPainter.Paint(brush, hit);
                    currentHolder.Rise();
                }
                break;
            case MakeCookieState.topping:
                cakeIcing.isPlacing = true;
               /* fillerTransform.transform.localPosition = new Vector3(Mathf.Clamp(newPos.x, fillerXZrange.x, fillerXZrange.y),
                fillerTransform.localPosition.y, Mathf.Clamp(newPos.z, fillerXZrange.z, fillerXZrange.w));

                if (Physics.Raycast(fillerParticle.transform.position, -Vector3.up, out hit, 10, PaintLayer))
                {
                    toppingPainter.Paint(brush, hit);
                    // currentHolder.Rise();
                    toppingTime += Time.deltaTime;
                    UIManager.instance.UpdateSliderVale(((toppingTime / 6f) / 3f) + 0.33f);
                    if (toppingTime >= 6)
                    {
                       // MyObjectPool.instance.SpawnParticleFromPool(0, currentHolder.topDecoration.transform.position);
                        //currentHolder.topDecoration.SetActive(true);
                        Vibration.Vibrate(150);
                        canPaint = false;
                        MouseRelease();
                        OnLevelAction(true);
                        //StartCoroutine(EndWait());
                    }
                }*/
                break;
            case MakeCookieState.toppingDone:
                cakeIcing.isPlacing = true;
                break;
            case MakeCookieState.sweets:
                fillerTransform.transform.localPosition = new Vector3(Mathf.Clamp(newPos.x, fillerXZrange.x, fillerXZrange.y),
                fillerTransform.localPosition.y, Mathf.Clamp(newPos.z, fillerXZrange.z, fillerXZrange.w));         
 
                toppingTime += Time.deltaTime;
                //UIManager.instance.UpdateSliderVale(((toppingTime / 6f) / 3f) + 0.66f);
                if (toppingTime >= 6 && !sweetsDone)
                {
                    sweetsDone = true;
                   // Vibration.Vibrate(150);
                    //MouseRelease();
                    OnLevelAction(true);
                    //StartCoroutine(EndWait());
                }              
                break;
            case MakeCookieState.sweetsDone:
                fillerTransform.transform.localPosition = new Vector3(Mathf.Clamp(newPos.x, fillerXZrange.x, fillerXZrange.y),
                fillerTransform.localPosition.y, Mathf.Clamp(newPos.z, fillerXZrange.z, fillerXZrange.w));
                break;
            default:
                break;
        }
    }

    private void MouseRelease()
    {
        fillerTransform.position = fillerStartPos;
        fillerParticle.Stop();
        //currentPainter.isPainting = false;
        cakeIcing.isPlacing = false;
        sweetsSpawnerfiller.StopSpawning();
    }

    public void OnBakeButtonPressed()
    {
        switch (makeCookieState)    
        {
            case MakeCookieState.backing:
                buttonCanvas.SetActive(false);
               // Vibration.Vibrate(150);
                canPaint = false;
                MouseRelease();
                //currentPainter.LerpBakeCall();
                StartCoroutine(LerpBake());
                break;
            case MakeCookieState.toppingDone:
                //Vibration.Vibrate(150);
                buttonCanvas.SetActive(false);
                OnLevelAction(true);
                break;
            case MakeCookieState.sweetsDone:
                //Vibration.Vibrate(150);
                buttonCanvas.SetActive(false);
                OnLevelAction(true);
                break;
        }       
    }

    IEnumerator LerpBake()
    {
        float time = 0;
        Material material = currentPainter.GetComponent<MeshRenderer>().material;
        Color colorPaint = material.color;
        Transform moveTransform = currentPainter.transform;
        while (time < 1.5f)
        {
            time += Time.deltaTime;
            material.SetColor("_Color", Color.Lerp(colorPaint, bakecolor, time / 1.5f));
            moveTransform.localPosition = new Vector3(moveTransform.localPosition.x,
                moveTransform.localPosition.y, Mathf.Lerp(yPosRangesBack.x, yPosRangesBack.y, time / 1.5f));

            yield return null;
        }

        OnLevelAction(true);
    }

    public void OnLevelAction(bool good)
    {
        if (!isPlaying)
            return;

        if (good)
        {

           /* if (bakeCalled)
            {
                MyObjectPool.instance.SpawnParticleFromPool(0, currentPainter.transform.position);
                currentHolder.BackedCookie();
                StartCoroutine(BakeIEnumertor());
            }
            else
            {
                buttonCanvas.SetActive(true);
                Vibration.Vibrate(200);
                bakeCalled = true;
            }*/

            switch (makeCookieState)
            {
                case MakeCookieState.filling:
                    buttonCanvas.SetActive(true);
                   // Vibration.Vibrate(200);
                    //bakeCalled = true;
                    makeCookieState = MakeCookieState.backing;
                    break;
                case MakeCookieState.backing:
                   // MyObjectPool.instance.SpawnParticleFromPool(0, currentPainter.transform.position);
                    currentHolder.BackedCookie();
                    StartCoroutine(BakeIEnumertor());
                    bakeSound.Play();
                    makeCookieState = MakeCookieState.topping;
                    fillerParticle.gameObject.SetActive(false);
                    fillerModel.SetActive(false);
                   // UIManager.instance.StartCounterTime("Top it with icing!");
                    break;
                case MakeCookieState.topping:
                    makeCookieState = MakeCookieState.toppingDone;
                    buttonCanvas.SetActive(true);
                    buttonCanvas.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
                    //EmojiParticles_Manager.instance.PlayRandomGoodParticles();
                    break;
                case MakeCookieState.toppingDone:
                    toppingTime = 0;
                    canPaint = true;
                   // fillerParticle.gameObject.SetActive(false);
                    makeCookieState = MakeCookieState.sweets;
                    sweetsSpawnerfiller.StartSpawning();
                    fillerModel.SetActive(false);
                    canModel.SetActive(true);
                    cakeIcing.TurnOffIcing();
                   // MyObjectPool.instance.SpawnParticleFromPool(0, cakeIcing.completeIcing.transform.position);
                   // UIManager.instance.StartCounterTime("Decorate the cookie!");
                   // EmojiParticles_Manager.instance.PlayRandomGoodParticles();
                    break;
                case MakeCookieState.sweets:                   
                    makeCookieState = MakeCookieState.sweetsDone;
                    buttonCanvas.SetActive(true);
                  //  EmojiParticles_Manager.instance.PlayRandomGoodParticles();
                    break;
                case MakeCookieState.sweetsDone:
                    sweetsSpawnerfiller.StopSpawning();
                    MouseRelease();
                    StartCoroutine(EndWait());
                    break;
                default:
                    break;
            }


            if (Random.value > 0.8f)
            {
                //TextEffect.instance.Play();
            }
            else
            {
               // EmojiParticles_Manager.instance.PlayRandomGoodParticles();
            }
        }
        else
        {
            //Vibration.Vibrate(300);
          //  UIManager.instance.OnLoseLevel();
          //  EmojiParticles_Manager.instance.PlayRandomBadParticles();
        }
    }

    IEnumerator BakeIEnumertor()
    {
        isPlaying = false;
        yield return new WaitForSeconds(1.5f);
       /* if (EmojiParticles_Manager.instance)
        {
            EmojiParticles_Manager.instance.PlayRandomGoodParticles();
        }*/
        
        currentHolder.PrePareForTopping();
        yield return StartCoroutine(MoveCamTo(posB, posC, false));
        yield return new WaitForSeconds(1.5f);

        isPlaying = true;
        canPaint = true;
        brush.Color = currentHolder.toppingColor;

        var main = fillerParticle.main;
        main.startColor = currentHolder.toppingColor; // (currentHolder.toppingColor + Color.white)/2f; 
       // yield return StartCoroutine(MoveCamTo(posB, posA, false));
        //this.LevelCompleted();
    }

    IEnumerator EndWait()
    {
        isPlaying = false;
        //EmojiParticles_Manager.instance.PlayRandomGoodParticles();
        yield return StartCoroutine(MoveCamTo(posC, posA, false));
        LevelCompleted();
    }

    public void LevelCompleted()
    {
        isPlaying = false;
        endParticles.Play();
       // UIManager.instance.ShowWinPanel();
        //EmojiParticles_Manager.instance.PlayRandomGoodParticles();
    }
}
