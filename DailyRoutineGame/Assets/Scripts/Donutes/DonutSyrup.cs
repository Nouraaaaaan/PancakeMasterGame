using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutSyrup : MonoBehaviour
{
    //[SerializeField] SwipeHintImage swipeHintImage;
    public bool canSwipe,selectSyrup = false;
    [SerializeField] ParticleSystem syrupParticles;
    [SerializeField] Transform syrupActivePos;
    [SerializeField] Transform donutTopPos;
    [SerializeField] LayerMask syrubLayer;

    public Donut donut;

    private MakeCookie_LevelManager levelManager;

    private Vector2 startPos, endPos;

    private SyrupItem selectedSyrup;

    private void Start()
    {
        levelManager = MakeCookie_LevelManager.instance;
       // swipeHintImage.FlashHand();
    }

    private void Update()
    {
        if (!levelManager.isPlaying)
            return;

        if (selectSyrup)
        {
            if (Input.touchCount > 0 )
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.GetTouch(0).position), out hit, 100, syrubLayer))
                {
                    selectedSyrup = hit.collider.GetComponent<SyrupItem>();
                    selectSyrup = false;
                    StartCoroutine(MoveSyrubItem());
                }
            }
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, syrubLayer))
                {
                    selectedSyrup = hit.collider.GetComponent<SyrupItem>();
                    selectSyrup = false;
                    StartCoroutine(MoveSyrubItem());
                }
            }
#endif
        }

        if (canSwipe)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        startPos = touch.position;
                        break;
                    case TouchPhase.Ended:
                        endPos = touch.position;
                        CheckSwipe();
                        break;
                    default:
                        break;
                }
            }

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                startPos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                endPos = Input.mousePosition;
                CheckSwipe();
            }
#endif
        }
    }

    private void CheckSwipe()
    {
        if (endPos.y < startPos.y) // swipe down
        {
            canSwipe = false;
            //swipeHintImage.StopHand();
            StartCoroutine(DonutInSyrup());
        }
    }

    IEnumerator MoveSyrubItem()
    {
       // swipeHintImage.StopHand();
        float time = 0;
        Vector3 startPos = selectedSyrup.transform.position;
        while (time < 0.4f)
        {
            time += Time.deltaTime;
            selectedSyrup.transform.position = Vector3.Lerp(startPos, syrupActivePos.position, time / 0.4f);
            yield return null;
        }
        var main = syrupParticles.main;
        main.startColor = selectedSyrup.ReturnColor();

        yield return new WaitForSeconds(0.4f);

        time = 0;
        startPos = donut.transform.position;
        while (time < 0.4f)
        {
            time += Time.deltaTime;
            donut.transform.position = Vector3.Lerp(startPos, donutTopPos.position, time / 0.4f);
            yield return null;
        }
        canSwipe = true;
        //swipeHintImage.SwipeDown();
    }

    IEnumerator DonutInSyrup()
    {
        float time = 0;
        while (time < 0.3f)
        {
            time += Time.deltaTime;
            donut.transform.position = Vector3.Lerp(donutTopPos.position, syrupActivePos.position, time / 0.3f);
            yield return null;
        }
        syrupParticles.transform.position = donut.transform.position;
        syrupParticles.Play();
        donut.ChangeSyrupColor(selectedSyrup.ReturnColor());
        yield return new WaitForSeconds(0.1f);
        

        time = 0;
        while (time < 0.3f)
        {
            time += Time.deltaTime;
            donut.transform.position = Vector3.Lerp(syrupActivePos.position, donutTopPos.position, time / 0.3f);
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);

        CheckToNextStep();
    }

    private void CheckToNextStep()
    {
        levelManager.OnLevelAction(true);
        enabled = false;
    }
}
