using System.Collections;
using UnityEngine;

public class DonutSweet : MonoBehaviour
{
    public bool canSelect, canPut = false;
    [SerializeField] LayerMask syrubLayer;
    [SerializeField] Transform sweetActivePos;
    [SerializeField] Transform donutPutPos;
    public Donut donut;
   // [SerializeField] float moveSpeed = 3;
    [SerializeField] Vector2 moveXRange = new Vector2(-3.532f, -2.532f);
    [SerializeField] Vector2 moveZRange = new Vector2(0.504f, 0.804f);

    private MakeCookie_LevelManager levelManager;
    private SweetHolder selectedSweet;
    private float deltaX, deltaY;
    private float castDistance;

    private Camera mainCamera;

    private void OnEnable()
    {
        levelManager = MakeCookie_LevelManager.instance;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!levelManager.isPlaying)
            return;

        if (canSelect)
        {
            if (Input.touchCount > 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.GetTouch(0).position), out hit, 100, syrubLayer))
                {
                    selectedSweet = hit.collider.GetComponent<SweetHolder>();
                    canSelect = false;
                    StartCoroutine(MoveSweetItem());
                }
            }
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, syrubLayer))
                {
                    selectedSweet = hit.collider.GetComponent<SweetHolder>();
                    canSelect = false;
                    StartCoroutine(MoveSweetItem());
                }
            }
#endif
        }

        if (canPut)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        Vector2 touchPos = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, castDistance));
                        deltaX = touchPos.x - selectedSweet.transform.position.x;
                        deltaY = touchPos.y - selectedSweet.transform.position.z;
                        break;
                    case TouchPhase.Moved:
                        Vector2 touchPosM = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, castDistance));
                        selectedSweet.transform.position = new Vector3(Mathf.Clamp(touchPosM.x + deltaX, moveXRange.x, moveXRange.y),
                            selectedSweet.transform.position.y, Mathf.Clamp(touchPosM.y + deltaY, moveZRange.x, moveZRange.y));
                        break;
                    case TouchPhase.Ended:
                        selectedSweet.transform.position = sweetActivePos.position;
                        break;
                    case TouchPhase.Canceled:
                        selectedSweet.transform.position = sweetActivePos.position;
                        break;
                    default:
                        break;
                }
            }

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 touchPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, castDistance));
                deltaX = touchPos.x - selectedSweet.transform.position.x;
                deltaY = touchPos.y - selectedSweet.transform.position.z;
     
            }
            if (Input.GetMouseButton(0))
            {
                Vector2 touchPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, castDistance)) ;
                selectedSweet.transform.position = new Vector3(Mathf.Clamp(touchPos.x + deltaX, moveXRange.x, moveXRange.y),
                    selectedSweet.transform.position.y, Mathf.Clamp(touchPos.y + deltaY, moveZRange.x, moveZRange.y));
 
            }
            if (Input.GetMouseButtonUp(0))
            {
                selectedSweet.transform.position = sweetActivePos.position;
            }
#endif
        }
    }

    IEnumerator MoveSweetItem()
    {
        float time = 0;
        Vector3 startPos = donut.transform.position;
        while (time < 0.4f)
        {
            time += Time.deltaTime;
            donut.transform.position = Vector3.Lerp(startPos, donutPutPos.position, time / 0.4f);
            yield return null;
        }
        
        yield return new WaitForSeconds(0.4f);

        time = 0;
        startPos = selectedSweet.transform.position;
        while (time < 0.4f)
        {
            time += Time.deltaTime;
            selectedSweet.transform.position = Vector3.Lerp(startPos, sweetActivePos.position, time / 0.4f);
            selectedSweet.transform.rotation = Quaternion.Slerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(0, 0, 180), time / 0.4f);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        castDistance = Vector3.Distance(mainCamera.transform.position, donut.transform.position);
        canPut = true;
        selectedSweet.Spawn = true;

        yield return new WaitForSeconds(5f);
        canPut = false;
        selectedSweet.Spawn = false;

        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(selectedSweet.ReturnToInitialPos());

        CheckToNextStep();
    }

    private void CheckToNextStep()
    {
        levelManager.OnLevelAction(true);
        enabled = false;
    }
}
