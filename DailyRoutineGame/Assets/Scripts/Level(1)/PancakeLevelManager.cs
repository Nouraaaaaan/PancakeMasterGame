using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Es.InkPainter;
using DG.Tweening;
using Obi;

public class PancakeLevelManager : MonoBehaviour
{
	public enum State
	{
		FillingState,
		CookingState,
		FlippingState,
		SyrupState,
		SweetingState
	}
	public State currentState;

	[Header("Camera Attributes")]
	public Camera Camera;
	public Transform CameraNewPos;

	[Header("Filling Attributes")]
	public GameObject Filler;
	public GameObject FillerCollisionPointsHolder;
	public FillerCollisionPoint[] FillerCollisionPoints;
	public int NumberOfPointsToFill;
	int NumberOfFilledPoints;
	public GameObject PaintingQuad;
	private GameObject paintingQuad;

	
	[Header("Cooking Attributes")]
	public GameObject PancakeModel;
	public Pancake Pancake;
	public Material PancakeMaterial;
	private Color color;

	[Header("Flipping Attributes")]
	public GameObject Pan;
	public Transform PanNewPos;
	public Meter Meter;
	public GameObject Arrow;
	public bool IsArrowInsideGreenArea;
	public Material BurntMaterial;
	private bool RightFlip;

	[Header("Sweeting Attributes")]
	public GameObject SweetsStage;
	public Transform PancakeNewPos;
	public Transform PancakeInitialPos;
	public Sweeter CurrentSweeter;
	public bool CanAddSweets;

	[Header("Syrup Attributes")]
	public GameObject SyrupStage;
	public Syrup CurrentSyrup;
	public bool CanAddSyrup;
	private int NumberofSyrupPoints;
	public SyrupPoints[] SyrupCollisionPoints;
	[SerializeField]
	LayerMask PaintLayer;
	RaycastHit hit;

	[Header("Order Attributes")]
	public OrderManager OrderManager;
	public string SyrupOrder;
	public string SweetsOrder;

	[Header("UI")]
	public GameObject CookingCanvas;
	public GameObject ResultCanvas;
	public GameObject OrderCanvas;
	//public GameObject OrderButton;
	public GameObject NormalCustomerCanvas;
	public GameObject VipCustomerCanvas;
	private int CoinsValue = 0;
	public Text Coinstext;

	[Header("VFX")]
	public ParticleSystem[] GoodEmojis;
	public ParticleSystem[] BadEmojis;
	public TextEffect TextEffect;
	public GameObject Smoke;

	[Header("Evaluation Attributes")]
	public Sprite BadEvaluationSprite;
	public Sprite GoodEvaluationSprite;
	public Image FlippingStateImage;
	public Image SyrupStateImage;
	public Image SweetsStateImage;

	[Header("Customers Attributes")]
	public CustomersManager CustomersManager;

	[Header("ObiFluid Attributes")]
	public ObiFluidRenderer ObiFluidRenderer;
	public ObiEmitter ObiEmitter;

	#region Singelton Region
	public static PancakeLevelManager Instance;
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}
	}
	#endregion

	#region Callbacks Region
	private void Start()
	{
		OrderManager.GenerateRandomOrder();

		color = PancakeMaterial.color;
		PancakeMaterial.color = new Color(color.r, color.g, color.b, 0f);
	}

	private void Update()
	{
		if (currentState == State.FlippingState)
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (IsArrowInsideGreenArea)
				{
					ClickAtrightTime();
					currentState = State.SyrupState;

				}
				else
				{
					ClickAtWrongTime();
				}
			}
		}

		/*
		else if ((currentState == State.SyrupState) && (CurrentSyrup != null))
		{
			if (Input.GetMouseButtonDown(0))
			{
				CurrentSyrup.PouringSyrup();
			}

			else if (Input.GetMouseButton(0))
			{
				if (Physics.Raycast(CurrentSyrup.PouringPoint.transform.position, -Vector3.up, out hit, 50, PaintLayer))
				{
					var paintObject = hit.transform.GetComponent<InkCanvas>();

					if (paintObject != null)
						paintObject.Paint(CurrentSyrup.Syrupbrush, hit);

				}
			}

			else if (Input.GetMouseButtonUp(0))
			{
				CurrentSyrup.StopPouringSyrup();
			}
		}
		*/
	}

	#endregion

	#region Filling State
	private void StartFillingState()
    {
		currentState = State.FillingState;

		//1.Set Pan.
		Pan.SetActive(true);
		Pan.transform.localPosition = new Vector3(3.788265f, -0.3218973f, -0.2844839f);
		Pan.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

		//2.Set Filler.
		Filler.SetActive(true);

		//3.Set Points.
		NumberOfFilledPoints = 0;
		FillerCollisionPointsHolder.SetActive(true);
		foreach (var point in FillerCollisionPoints)
		{
			point.IsCollidedWithFiller = false;
		}

		//4.Create Painting Quad.
		CreatePaintingQuad();

		//4.Move Camera.
		StartCoroutine(FillingstateCameraMovement());
	}

	private IEnumerator FillingstateCameraMovement()
	{
		Camera.transform.DORotate(new Vector3(28.108f, 180f, 0f), 0.5f);
		Camera.transform.DOMoveZ(2.09f, 0.5f);

		yield return new WaitForSeconds(1f);
	}

	public void UpdateNumberOfFilledPoints()
	{
		NumberOfFilledPoints++;

		if (NumberOfFilledPoints == NumberOfPointsToFill)
		{
			//Debug.Log("Filling Stage has finished !");
			StartCoroutine(FinishFillingState());
		}
	}

	IEnumerator FinishFillingState()
	{
		//Play Random Emojii.
		GoodEmojis[Random.Range(0, GoodEmojis.Length - 1)].Play();
		yield return new WaitForSeconds(1f);

		//deactivate Filling Attributes.
		Filler.gameObject.SetActive(false);
		FillerCollisionPointsHolder.gameObject.SetActive(false);

		StartCookingState();
	}

	private void CreatePaintingQuad()
	{
		paintingQuad = Instantiate(PaintingQuad);
	}

	private void DestroyPaintingQuad()
	{
		if (paintingQuad != null)
		{
			Destroy(paintingQuad);
		}
	}
	#endregion

	#region Cooking state
	private void StartCookingState()
	{
		currentState = State.CookingState;

		SetPancake();
		CookingCanvas.SetActive(true);
	}

	private void SetPancake()
	{
		//set transform
		PancakeModel.transform.position = new Vector3(35.05699f, -0.4554057f, -0.13f);
		PancakeModel.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

		//set rb constrains
		Pancake.FreePancakeConstrains();

		//set material
		PancakeMaterial.color = new Color(color.r, color.g, color.b, 0f);

	}

	public void ClickCookButton()
	{
		StartCoroutine(Cook());
		CookingCanvas.SetActive(false);

	}

	private IEnumerator Cook()
	{
		Smoke.SetActive(true);

		while (PancakeMaterial.color.a < 1f)
		{
			PancakeMaterial.color = new Color(color.r, color.g, color.b, PancakeMaterial.color.a + 0.04f);
			yield return new WaitForEndOfFrame();
		}

		yield return null;

		DestroyPaintingQuad();
		StartFlippingState();
		StartCoroutine(ReturnCameraToInitialPos());
	}

	private IEnumerator ReturnCameraToInitialPos()
	{
		Camera.transform.DORotate(new Vector3(5.408f, 180f, 0f), 0.5f);
		Camera.transform.DOMoveZ(3.414223f, 0.5f);

		yield return new WaitForSeconds(1f);
	}

	#endregion

	#region Flipping State
	private void StartFlippingState()
	{
		currentState = State.FlippingState;

		Meter.gameObject.SetActive(true);
		Arrow.SetActive(true);
		Meter.MoveArrow();
	}

	public void ClickAtrightTime()
	{
		RightFlip = true;
		Meter.StopArrow();
		Pancake.Flip();
		PanAnimation();
		Smoke.SetActive(false);
	}

	public void ClickAtWrongTime()
	{
		RightFlip = false;
		ChangePancakeMaterial();
		BadEmojis[Random.Range(0, BadEmojis.Length - 1)].Play();
		Smoke.SetActive(false);
	}

	private void PanAnimation()
	{
		Pan.transform.DOMoveY(Pan.transform.position.y + 0.1f, 0.5f);
		Pan.transform.DOMoveY(Pan.transform.position.y - 0.08f, 0.5f).SetDelay(0.5f);
	}

	private void ChangePancakeMaterial()
	{
		Pancake.GetComponent<Renderer>().material = BurntMaterial;
	}

	public void FinishFlippingState()
	{
		Meter.gameObject.SetActive(false);
		Arrow.SetActive(false);

		Pancake.FreezePancake();

		StartCoroutine(MoveToPlate());		
	}
	#endregion

	private IEnumerator MoveToPlate()
	{
		StartSyrupState();

		//1.Move Camera.
		Camera.transform.DOMove(CameraNewPos.position, 1f);

		//2.Move Pan.
		Pancake.transform.parent = Pan.transform;
		Pan.transform.DOMove(PanNewPos.position, 1f);
		yield return new WaitForSeconds(1f);

		//3.Rotate Pan.
		Pan.transform.DORotate(new Vector3(40f, 0f, 0f), 1f);
		yield return new WaitForSeconds(0.5f);

		//4.Move Pancake to pan.
		Pancake.gameObject.transform.DOMove(PancakeNewPos.position, 0.5f);
		Pancake.gameObject.transform.DOLocalRotate(new Vector3(-48.963f, 0.001f, 180f), 0.5f);
		yield return new WaitForSeconds(0.65f);


		Pancake.transform.parent = null;
		Pan.SetActive(false);

		StartCoroutine(MoveCamera());
	}

	private IEnumerator MoveCamera()
	{
		Camera.transform.DOMove(new Vector3(32.58f, 1.84f, 1.81f), 0.5f);
		Camera.transform.DORotate(new Vector3(40.43f, 180f, 0f), 0.5f);

		yield return new WaitForSeconds(1f);
	}

	#region Syrup State

	public void StartSyrupState()
    {
		SyrupStage.SetActive(true);
	}

	public void StartObiFluid()
	{
		ObiEmitter.gameObject.SetActive(true);
		ObiEmitter.speed = 1.5f;

		ObiFluidRenderer.enabled = true;
		
		ObiEmitter.enabled = true;
	}

	public void FinishObi()
	{
		StartCoroutine(FinishObiFluid());
	}

	public IEnumerator FinishObiFluid()
	{
		ObiEmitter.speed = 0f;

		yield return new WaitForSeconds(0.1f);

		ObiEmitter.enabled = false;
	}

    public void UpdateNumberOfSyrupPoints()
	{
		NumberofSyrupPoints++;

		if (NumberofSyrupPoints >= 5)
		{
			StartCoroutine(ReturnSyrupToInitialPosition());
		}
	}

	public void ResetSyrupCollisionPoints()
	{
		foreach (var point in SyrupCollisionPoints)
		{
			point.IsCollidedWithFiller = false;
		}
	}

	public void FinishSyrupState()
	{
		//StartCoroutine(FinishObiFluid());
		
		currentState = State.SweetingState;

		SyrupStage.SetActive(false);
		NumberofSyrupPoints = 0;
		ResetSyrupCollisionPoints();

		SweetsStage.SetActive(true);
	}

	private IEnumerator ReturnSyrupToInitialPosition()
	{
		yield return new WaitForSeconds(4f);

		CurrentSyrup.ReturnSyrupToInitialPosition();
	}

	#endregion

	#region Sweeting State

	public void FinishSweetingStage()
	{
		TextEffect.PlayEffect();

		CurrentSweeter.ReturnSweeter();
		CurrentSweeter.NumberOfSpawnedSweets = 0;
		CurrentSweeter.Finished = false;
		CurrentSweeter.arrived = false;
		//SweetsStage.SetActive(false);
		CanAddSweets = true;

		
		StartCoroutine(ReturnToCustomer());
	}

	public void DiableSweetingStage()
	{
		SweetsStage.SetActive(false);
	}

	#endregion

    #region Transition Region

	public void Onclick_CookBtn()
    {
		//1.Move to kitchen.
		Camera.transform.DOMoveX(35f, 0.25f).OnComplete(StartFillingState);

		//2.Disable btn.
		//OrderButton.SetActive(false);
		DisableCustomerCanvas();

		//3.Disable order.
		OrderCanvas.SetActive(false);

	}

	private IEnumerator ReturnToCustomer()
	{
		yield return new WaitForSeconds(2f);

		//1.Move Camera.
		Camera.transform.DOMove(new Vector3(0.7f, 1.160993f, 3.414223f), 0.25f).OnComplete(NextCustomer);
		Camera.transform.DORotate(new Vector3(5.408f, 180f, 0f), 0.25f);

		//2.Reset
		Reset();

		//3.
		CheckResult();
		ShowResultCanvas();
	}

	private void NextCustomer()
	{
		//2.Move Customer.
		StartCoroutine(NextCustomerCorotinue());
	}

	private IEnumerator NextCustomerCorotinue()
	{
		yield return new WaitForSeconds(1f);
			 
		CustomersManager.NextCustomer();

		//Disable result canvas.
		ResultCanvas.SetActive(false);

		//enable let's cook button.
		//OrderButton.SetActive(true);
		EnableCustomerCanvas();

		//enable orderCanvas and generate random order.
		OrderCanvas.SetActive(true);
		OrderManager.GenerateRandomOrder();
	}

	#endregion

	#region Result Region
	public void CheckResult()
	{
		if (!OrderManager.CustomerSyrupOrder.ToString().Equals(SyrupOrder))
		{
			SyrupStateImage.sprite = BadEvaluationSprite;
		}
		else
		{
			SyrupStateImage.sprite = GoodEvaluationSprite;
		}

		if (!OrderManager.CustomerSweetsOrder.ToString().Equals(SweetsOrder))
		{
			SweetsStateImage.sprite = BadEvaluationSprite;
		}
		else
		{
			SweetsStateImage.sprite = GoodEvaluationSprite;
		}

		if (RightFlip)
		{
			FlippingStateImage.sprite = GoodEvaluationSprite;
		}
		else
		{
			FlippingStateImage.sprite = BadEvaluationSprite;
		}
	}

	public void ShowResultCanvas()
	{
		ResultCanvas.SetActive(true);
		UpdateCoinsNumber();

	}

	#endregion

	public void Reset()
	{
		//CurrentSyrup.DestroySyrupMesh();
		CanAddSyrup = true;
		CurrentSweeter.ClearChildren();
		Pancake.GetComponent<Renderer>().material = PancakeMaterial;

		ObiFluidRenderer.enabled = false;
		ObiEmitter.gameObject.SetActive(false);

	}

	public void EnableCustomerCanvas()
	{
        if (CustomersManager.CheckVipCustomer())
        {
			VipCustomerCanvas.SetActive(true);
        }
		else
        {
			NormalCustomerCanvas.SetActive(true);
        }
	}

	public void DisableCustomerCanvas()
	{
	    VipCustomerCanvas.SetActive(false);

	    NormalCustomerCanvas.SetActive(false);	
	}

	public void OnClick_NoThanksButtons()
	{
		CustomersManager.NextCustomer();
		DisableCustomerCanvas();
		EnableCustomerCanvas();
	}

	private void UpdateCoinsNumber()
    {
		CoinsValue += 50;
		Coinstext.text = CoinsValue.ToString();

	}

}
