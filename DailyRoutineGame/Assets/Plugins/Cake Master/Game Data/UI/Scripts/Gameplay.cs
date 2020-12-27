using System.Collections.Generic;
using UnityEngine;

public class Gameplay : Singleton<Gameplay>
{
    public enum Instrument
    {
        IcingBag,
        Smoother
    }

    [SerializeField] Vector2 fingerOffset;
    [SerializeField] Transform touchIndicator;
    public List<Color> icingColors;
    public Instrument instrument;

    [Header("Icing")]
    [SerializeField] LayerMask cakeLayer;
    [SerializeField] LayerMask icingLayer;
    [SerializeField] float spawnRadius = 0.05f;
    [SerializeField] GameObject icingPrefab;
    [SerializeField] GameObject icingInstrument;
    [SerializeField] Transform selector;
    [SerializeField] Material icingBagMaterial;

    Color selectedColor;

    [Header("Smoother")]
    [SerializeField] float smootingRadius = 1f;
    [SerializeField] GameObject smootherInstrument;

    [Header("Components")]
    [SerializeField] Transform instrumentTransform;
    [SerializeField] Transform instrumentDefaultPos;
    [SerializeField] Material cakeMat;
    [SerializeField] Cake cake;
    [SerializeField] Transform colorHolderParent;
    [SerializeField] GameObject uiColorHolder;
    [SerializeField] Color defaultColor;

    Mesh icingPrefabMesh;
    int currentIcingAmount = 0;
    float icingFill = 0f;
    float IcingFill
    {
        get
        {
            return icingFill;
        }
        set
        {
            icingFill = value;
           /* if (UIManager.instance)
            {
                UIManager.instance.UpdateSliderVale(icingFill);
            }  */        
        }
    }
    bool isPlacing = false;
    bool isTouchingCake = false;
    bool gameStarted = false;
    Vector3 hitPoint;
   // Vector3 normal;
    Camera mainCamera;

    #region MonoBehaviour Callbacks
    protected override void Awake()
    {
        base.Awake();      
    }

    private void Start()
    {
        gameStarted = false;
        currentIcingAmount = 0;
        IcingFill = 0;
        mainCamera = Camera.main;

        GenerateIcingColors();
        ChangeToIcing();
        cake.ColorCake(defaultColor);

        /*if (UIManager.instance)
        {
            UIManager.instance.EnableSlider();
            UIManager.instance.StartCounterTime("Lets Add Icing On Cake");
        }*/
       
    }

    private void Update()
    {
        if (instrument == Instrument.IcingBag)
            PlaceIcing();
        else if (instrument == Instrument.Smoother)
            Smoothing();

        SoundManager.Instance.Instrument(isTouchingCake);
    }
    #endregion

    #region Private Methods

    void GenerateIcingColors()
    {
        for (int i = 0; i < icingColors.Count; i++)
        {
            GameObject currentIcingColorHorlder = Instantiate(uiColorHolder, colorHolderParent);
            currentIcingColorHorlder.GetComponent<ColorChooser>().SetColor(icingColors[i]);

            if (i == 0)
            {
                SelectColor(icingColors[i], currentIcingColorHorlder.transform);
            }
        }
    }

    void PlaceIcing()// called from update
    {
        if (Input.GetMouseButtonDown(0))
        {
            isPlacing = true;
            touchIndicator.gameObject.SetActive(true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPlacing = false;
            touchIndicator.gameObject.SetActive(false);
        }

        if (isPlacing)
        {
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition + (Vector3)fingerOffset), out RaycastHit hit, 50f, cakeLayer))
            {
                if (!gameStarted)
                {
                    gameStarted = true;
                }

                hitPoint = hit.point;

                if (hit.collider.tag == "Cake")
                {
                    if (!Physics.CheckSphere(hitPoint, spawnRadius, icingLayer)) //Returns true if there are any colliders overlapping the sphere defined 
                    {
                        currentIcingAmount++;
                        Instantiate(icingPrefab, hitPoint, Quaternion.identity, cake.transform.parent);
                       // normal = hit.normal;

                        IcingFill = (float)currentIcingAmount / cake.icingAmountNeeded;
                        if (IcingFill >= 0.90)
                            ChangeToSmoothing();

                        cake.ColorMesh(selectedColor, hitPoint, 0.15f);
                    }

                    isTouchingCake = true;
                }
                else
                {
                    isTouchingCake = false;
                }

                instrumentTransform.position = Vector3.Lerp(instrumentTransform.position, hitPoint, 0.2f);
                instrumentTransform.up = (instrumentTransform.position - Vector3.zero).normalized;

                touchIndicator.position = Input.mousePosition + (Vector3)fingerOffset;

                touchIndicator.gameObject.SetActive(true);
            }
            else
            {
                instrumentTransform.position = Vector3.Lerp(instrumentTransform.position, instrumentDefaultPos.position, 0.1f);
                instrumentTransform.rotation = Quaternion.Lerp(instrumentTransform.rotation, instrumentDefaultPos.rotation, 0.1f);

                touchIndicator.gameObject.SetActive(false);
                isTouchingCake = false;
            }
        }
        else
        {
            instrumentTransform.position = Vector3.Lerp(instrumentTransform.position, instrumentDefaultPos.position, 0.1f);
            instrumentTransform.rotation = Quaternion.Lerp(instrumentTransform.rotation, instrumentDefaultPos.rotation, 0.1f);

            touchIndicator.gameObject.SetActive(false);
            isTouchingCake = false;
        }
    }

    void Smoothing()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isPlacing = true;          
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPlacing = false;
        }

        if (isPlacing)
        {
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition + (Vector3)fingerOffset), out RaycastHit hit, 50f, cakeLayer))
            {
                hitPoint = hit.point;

                if (hit.collider.tag == "Cake")
                {
                    cake.ReturToNormal(hitPoint, smootingRadius);
                   // normal = hit.normal;
                    instrumentTransform.up = Vector3.Lerp(instrumentTransform.up, hit.normal, 0.2f);
                    isTouchingCake = true;
                }
                else
                {
                    instrumentTransform.up = Vector3.Lerp(instrumentTransform.up, Vector3.back, 0.1f);
                    isTouchingCake = false;
                }

                instrumentTransform.position = Vector3.Lerp(instrumentTransform.position, hitPoint, 0.2f);

                touchIndicator.position = Input.mousePosition + (Vector3)fingerOffset;

                touchIndicator.gameObject.SetActive(true);
            }
            else
            {
                instrumentTransform.position = Vector3.Lerp(instrumentTransform.position, instrumentDefaultPos.position, 0.1f);
                instrumentTransform.rotation = Quaternion.Lerp(instrumentTransform.rotation, instrumentDefaultPos.rotation, 0.1f);

                touchIndicator.gameObject.SetActive(false);
                isTouchingCake = false;
            }
        }
        else
        {
            instrumentTransform.position = Vector3.Lerp(instrumentTransform.position, instrumentDefaultPos.position, 0.1f);
            instrumentTransform.rotation = Quaternion.Lerp(instrumentTransform.rotation, instrumentDefaultPos.rotation, 0.1f);

            touchIndicator.gameObject.SetActive(false);
            isTouchingCake = false;
        }
    }
    #endregion

    #region Public Methods
    public void ChangeToIcing()
    {
        instrument = Instrument.IcingBag;
        smootherInstrument.SetActive(false);
        icingInstrument.SetActive(true);
    }

    public void ChangeToSmoothing()
    {
        instrument = Instrument.Smoother;
        smootherInstrument.SetActive(true);
        icingInstrument.SetActive(false);

       // UIManager.instance.StartCounterTime("Smooth it Out!");
        SoundManager.Instance.Change();
       // EmojiParticles_Manager.instance.PlayRandomGoodParticles();
    }

    public void SelectColor(Color color, Transform colorHolder)
    {
        selectedColor = color;

        icingBagMaterial.SetColor("_BaseColor", color);

        if (colorHolder != null)
        {
            selector.SetParent(colorHolder);
            selector.localPosition = Vector3.zero;
        }

        GameObject oldIcingPrefab = icingPrefab;
        icingPrefab = Instantiate(icingPrefab, icingPrefab.transform.position, Quaternion.identity);
        icingPrefabMesh = icingPrefab.GetComponent<MeshFilter>().mesh;
        Destroy(oldIcingPrefab);

        Color[] colors = new Color[icingPrefabMesh.vertexCount];
        for (int i = 0; i < icingPrefabMesh.vertexCount; i++)
        {
            colors[i] = color;
        }
        icingPrefabMesh.colors = colors;
    }
    #endregion
}
