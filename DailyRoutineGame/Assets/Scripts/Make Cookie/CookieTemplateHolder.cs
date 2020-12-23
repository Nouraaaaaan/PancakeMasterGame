using System.Collections;
using UnityEngine;

public class CookieTemplateHolder : MonoBehaviour
{
    [Header("Cookie Item")]
    [SerializeField] GameObject cookieItem;
    public GameObject cookieTopping;
    //[SerializeField] bool changeEmissionColor;
    //public GameObject topDecoration;
    public Color toppingColor = Color.white;
    public CakeIcing cake;

    [Header("Paint Quad")]
    [SerializeField] Transform paintQuad;
    [SerializeField] Vector2 yPosRangesPaint = new Vector2(0.1f, 0.25f);
    [SerializeField] float speed = 0.08f;


    private float currentRaisetime = 0;

   // private UIManager uiManager;
    private bool paintFinished = false;

    private void Start()
    {
       // uiManager = UIManager.instance;
    }

    public void BackedCookie()
    {
        paintQuad.gameObject.SetActive(false);
        cookieItem.SetActive(true);
    } 
    
    public void ActivateTopping()
    {
        cookieTopping.SetActive(true);

    }

    public void PrePareForTopping()
    {
        StartCoroutine(PrepareToppingState());
    }

    IEnumerator PrepareToppingState()
    {
        float time = 0;
        Vector3 startPos = cookieItem.transform.localPosition;
        Vector3 endPos = startPos + Vector3.forward * 0.4f;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            cookieItem.transform.localPosition = Vector3.Lerp(startPos, endPos, time / 0.5f);
            yield return null;
        }

        cookieItem.transform.parent = transform.parent;
        time = 0;
        startPos = transform.position;
        endPos = startPos + Vector3.right * 10;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, time / 0.5f);
            yield return null;
        }

        startPos = cookieItem.transform.localPosition;
        endPos = startPos - Vector3.up * 2f;
        time = 0;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            cookieItem.transform.localPosition = Vector3.Lerp(startPos, endPos, time / 0.5f);
            yield return null;
        }
        ActivateTopping();
    }

    public void Rise()
    {
        paintQuad.transform.localPosition = new Vector3(paintQuad.transform.localPosition.x,
           paintQuad.transform.localPosition.y, Mathf.Lerp(yPosRangesPaint.x, yPosRangesPaint.y, currentRaisetime));

        currentRaisetime += Time.deltaTime * speed;
        if (currentRaisetime > 1 && !paintFinished)
        {
            paintFinished = true;
           
            MakeCookie_LevelManager.instance.OnLevelAction(true);
        }
        else
        {
            //uiManager.UpdateSliderVale(Mathf.Min( currentRaisetime / 3f , 0.33f));        
        }
    }
}
