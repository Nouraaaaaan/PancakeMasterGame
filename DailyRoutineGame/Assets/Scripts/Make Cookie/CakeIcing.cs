using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CakeIcing : MonoBehaviour
{
    [SerializeField] Vector2 fingerOffset = new Vector2(0, 200);
    //[SerializeField] Transform touchIndicator;
    public List<Color> icingColors;

    [Header("Icing")]
    [SerializeField] LayerMask cakeLayer;
    [SerializeField] LayerMask icingLayer;
    [SerializeField] float spawnRadius = 0.05f;
    [SerializeField] GameObject icingPrefab;
    [SerializeField] GameObject icingInstrument;
    //[SerializeField] Transform selector;
    [SerializeField] Material icingBagMaterial;

    Color selectedColor;

    //[Header("Smoother")]
    // [SerializeField] float smootingRadius = 1f;
    // [SerializeField] GameObject smootherInstrument;

    [Header("Icing Materials")]
    [SerializeField] Material[] icingMaterials;

    [Header("Components")]
    [SerializeField] Transform instrumentTransform;
    [SerializeField] Transform instrumentDefaultPos;
    //[SerializeField] Material cakeMat;
    [SerializeField] Cake cake; // refrence final cake
    //[SerializeField] Transform colorHolderParent;
    //[SerializeField] GameObject uiColorHolder;
    //[SerializeField] Color defaultColor;
    [SerializeField] Transform icingHolder;
    public GameObject completeIcing;

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
                UIManager.instance.UpdateSliderVale(Mathf.Min(icingFill / 3f + 0.33f, 0.66f));
            }*/
        }
    }

    public bool isPlacing = false;
    bool isTouchingCake = false;
    bool gameStarted = false;
    bool isfinishedCalled = false;
    Vector3 hitPoint;
    // Vector3 normal;
    Camera mainCamera;

    #region MonoBehaviour Callbacks

    private void Start()
    {
        gameStarted = false;
        currentIcingAmount = 0;
        IcingFill = 0;
        mainCamera = Camera.main;
        isfinishedCalled = false;

        int randomOne = Random.Range(0, icingMaterials.Length);
        icingPrefab.GetComponent<MeshRenderer>().material = icingMaterials[randomOne];
        completeIcing.GetComponent<MeshRenderer>().material = icingMaterials[randomOne];

       
        ChangeToIcing();
       
    }

    private void Update()
    {
        PlaceIcing();

       // SoundManager.Instance.Instrument(isTouchingCake);
    }
    #endregion

    #region Private Methods

    void GenerateIcingColors()
    {
        for (int i = 0; i < icingColors.Count; i++)
        {
            //GameObject currentIcingColorHorlder = Instantiate(uiColorHolder, colorHolderParent);
            //currentIcingColorHorlder.GetComponent<ColorChooser>().SetColor(icingColors[i]);

            if (i == 0)
            {
                SelectColor(icingColors[i]);
            }
        }
    }

    public void PlaceIcing()// called from update
    {
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
                        Instantiate(icingPrefab, hitPoint, Quaternion.identity, icingHolder);
                        //Vibration.Vibrate(15);
                        // normal = hit.normal;

                        IcingFill = (float)currentIcingAmount / cake.icingAmountNeeded;
                        if (IcingFill >= 0.75f)
                            IcingDone();

                        cake.ColorMesh(selectedColor, hitPoint, 0.15f);
                    }

                    isTouchingCake = true;
                }
                else
                {
                    isTouchingCake = false;
                }

                instrumentTransform.position = Vector3.Lerp(instrumentTransform.position, hitPoint , 0.2f);
                //instrumentTransform.up = (instrumentTransform.position - Vector3.one).normalized;
                //instrumentTransform.localRotation = Quaternion.Euler(77, 0, -45);

               // touchIndicator.position = Input.mousePosition + (Vector3)fingerOffset;

                //touchIndicator.gameObject.SetActive(true);
            }
            else
            {
                instrumentTransform.position = Vector3.Lerp(instrumentTransform.position, instrumentDefaultPos.position, 0.1f);
                instrumentTransform.rotation = Quaternion.Lerp(instrumentTransform.rotation, instrumentDefaultPos.rotation, 0.1f);

                //touchIndicator.gameObject.SetActive(false);
                isTouchingCake = false;
            }
        }
        else
        {
            instrumentTransform.position = Vector3.Lerp(instrumentTransform.position, instrumentDefaultPos.position, 0.1f);
            instrumentTransform.rotation = Quaternion.Lerp(instrumentTransform.rotation, instrumentDefaultPos.rotation, 0.1f);

            //touchIndicator.gameObject.SetActive(false);
            isTouchingCake = false;
        }
    }

    #endregion

    #region Public Methods
    public void ChangeToIcing()
    {
       
       // smootherInstrument.SetActive(false);
        icingInstrument.SetActive(true);
    }

    private void IcingDone()
    {
        if (isfinishedCalled)
            return;

        isfinishedCalled = true;
       // MakeCookie_LevelManager.instance.OnLevelAction(true);
    }

    public void TurnOffIcing()
    {
        icingInstrument.SetActive(false);
        isPlacing = false;
        icingHolder.gameObject.SetActive(false);
        completeIcing.SetActive(true);
    }

    public void SelectColor(Color color)
    {
        selectedColor = color;

        icingBagMaterial.SetColor("_BaseColor", color);

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
