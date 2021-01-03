using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Es.InkPainter;
using DG.Tweening;

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
	public GameObject FillerCollisionPoints;
	public ParticleSystem FillingParticle;
	public int NumberOfPointsToFill;
	int NumberOfFilledPoints;
	[SerializeField]
	private Brush brush;
	[SerializeField]
	LayerMask PaintLayer;
	RaycastHit hit;
	public GameObject PaintingQuad;
	private GameObject paintingQuad;

	[Header("Cooking Attributes")]
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
	public Sweeter Sweeter;
	public bool CanAddSweets;

	[Header("Syrup Attributes")]
	public GameObject SyrupStage;
	public Syrup[] Syrups;
	public Syrup CurrentSyrup;
	public bool CanAddSyrup;
	private int NumberofSyrupPoints;

	[Header("Order Attributes")]
	public OrderManager OrderManager;
	public string SyrupOrder;
	public string SweetsOrder;

	[Header("UI")]
	public GameObject CookingCanvas;
	public GameObject ResultCanvas;
	public GameObject OrderCanvas;
	public GameObject OrderButton;

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

		currentState = State.FillingState;
		CreatePaintingQuad();

		//StartCoroutine(FillingstateCameraMovement());


		color = PancakeMaterial.color;
		PancakeMaterial.color = new Color(color.r, color.g, color.b, 0f);
	}

	private void Update()
	{
		if (currentState == State.FillingState)
		{
			if (Input.GetMouseButtonDown(0))
			{
				FillingParticle.Play();
			}

			else if (Input.GetMouseButton(0))
			{
				if (Physics.Raycast(Filler.transform.position, -Vector3.up, out hit, 50, PaintLayer))
				{
					var paintObject = hit.transform.GetComponent<InkCanvas>();

					if (paintObject != null)
						paintObject.Paint(brush, hit);

				}
			}

			else if (Input.GetMouseButtonUp(0))
			{
				FillingParticle.Stop();
			}
		}

		else if (currentState == State.FlippingState)
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

	}

	#endregion

	#region Filling State
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
		GoodEmojis[Random.Range(0, GoodEmojis.Length - 1)].Play();

		yield return new WaitForSeconds(1f);

		StartCookingState();
	}

	private void StartFillingState()
    {
		StartCoroutine(FillingstateCameraMovement());
    }

	private IEnumerator FillingstateCameraMovement()
	{
		Camera.transform.DORotate(new Vector3(28.108f, 180f, 0f), 0.5f);
		Camera.transform.DOMoveZ(2.09f, 0.5f);

		yield return new WaitForSeconds(1f);
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
		Filler.gameObject.SetActive(false);
		FillerCollisionPoints.gameObject.SetActive(false);
		CookingCanvas.SetActive(true);
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
		//Smoke.SetActive(false);
		paintingQuad.SetActive(false);
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

		OrderCanvas.SetActive(false);
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

		//Meter.StopArrow();
		//Pancake.Flip();
		//PanAnimation();

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
	#endregion

	#region Syrup State
	public void UpdateNumberOfSyrupPoints()
	{
		NumberofSyrupPoints++;

		if (NumberofSyrupPoints >= 5)
		{
			CurrentSyrup.ReturnSyrupToInitialPosition();
		}
	}

	public void FinishSyrupState()
	{
		currentState = State.SweetingState;
		SyrupStage.SetActive(false);
		SweetsStage.SetActive(true);
	}

	#endregion

	#region Sweeting State
	public void StartSweetingState()
	{
		//Debug.Log("Sweeting Stage has Started !");

		//currentState = State.SweetingState;
		Pancake.FreezePancake();
		//SweeterSatge.SetActive(true);
		StartCoroutine(MoveToPlate());

		Meter.gameObject.SetActive(false);
		Arrow.SetActive(false);
	}

	private IEnumerator MoveToPlate()
	{
		//1.Move Camera.
		Camera.transform.DOMove(CameraNewPos.position , 1f);
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

		//MoveSyrup();
	}

	public void FinishSweetingStage()
	{
		//Debug.Log("Sweeting Stage has Finished !");
		Sweeter.ReturnSweeter();

		SFXManager.Instance.StopSoundEffect();
		TextEffect.PlayEffect();

		//SyrupStage.SetActive(true);
		//SweetsStage.SetActive(false);

		CheckResult();
		ShowResultCanvas();
		ReturnToCustomer();
	}

	#endregion

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

		if(!OrderManager.CustomerSweetsOrder.ToString().Equals(SweetsOrder))
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

	}

    public void Reset()
    {
		DestroyPaintingQuad();
		CurrentSyrup.DestroySyrupMesh();
		Sweeter.ClearChildren();
	}

    #region Transition Region

	public void Onclick_CookBtn()
    {
		//1.Move to kitchen.
		Camera.transform.DOMoveX(35f, 0.25f).OnComplete(StartFillingState);

		//2.Disable btn.
		OrderButton.SetActive(false);

		//3.Disable order.
		OrderCanvas.SetActive(false);

	}

	private void ReturnToCustomer()
	{
		//1.Move Camera.
		Camera.transform.DOMove(new Vector3(0.7f, 1.160993f, 3.414223f), 0.25f).OnComplete(NextCustomer);
		Camera.transform.DORotate(new Vector3(5.408f, 180f, 0f), 0.25f);
	}

	private void NextCustomer()
	{
		//2.Move Customer.
		StartCoroutine(NextCustomerCorotinue());
	}

	private IEnumerator NextCustomerCorotinue()
	{
		yield return new WaitForSeconds(6f);
			 
		CustomersManager.NextCustomer();

		//Disable result canvas.
		ResultCanvas.SetActive(false);

		//enable let's cook button.
		OrderButton.SetActive(true);

		//enable orderCanvas and generate random order.
		OrderCanvas.SetActive(true);
		OrderManager.GenerateRandomOrder();
	}

	#endregion

}
