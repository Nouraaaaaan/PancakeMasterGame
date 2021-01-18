using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Es.InkPainter;
using DG.Tweening;
using Obi;
using System.IO;
using UnityEngine.SceneManagement;

public class PancakeLevelManager : MonoBehaviour
{
	#region Fields
	public SaveTest SaveTest;

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
	public bool RightFlip;

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
	public SyrupPoints[] SyrupCollisionPoints;
	[SerializeField]
	LayerMask PaintLayer;

	[Header("Order Attributes")]
	public OrderManager OrderManager;
	public string SyrupOrder;
	public string SweetsOrder;

	[Header("UI")]
	public GameObject CookingCanvas;
	public GameObject ResultCanvas;
	public GameObject OrderCanvas;
	public GameObject CollectCanvas;
	public GameObject NormalCustomerCanvas;
	public GameObject VipCustomerCanvas;
	public GameObject StoreCanvas;
	public GameObject UpgradeDinerStore;
	//Order Notes.
	public GameObject OrderNoteCanvas;
	public Image OrderNote;
	public Image SyrupNote;
	public Image ToppingNote;
	private int CoinsValue = 0;
	public Text Coinstext;
	public Text EarnedCoinsValue;

	[Header("VFX")]
	public ParticleSystem[] GoodEmojis;
	public ParticleSystem[] BadEmojis;
	public TextEffect TextEffect;
	//public GameObject Smoke;
	public GameObject CandelLight;
	public ParticleSystem ConfettiBlast;
	public ParticleSystem DollarBlast;
	public ParticleSystem HeartPoof;
	public ParticleSystem Steam;
	public ParticleSystem ConfettiShower;
	public ParticleSystem HeartStream;
	public ParticleSystem StarField;
	public ParticleSystem WhiteSmoke;

	[Header("Evaluation Attributes")]
	public Sprite SadEvaluationSprite;
	public Sprite AngryEvaluationSprite;

	public Sprite GoodEvaluationSprite;
	public Sprite HeartEyesEvaluationSprite;

	public Image FlippingStateImage;
	public Image OrderFlippingImage;

	public Image SyrupStateImage;
	public Image SyrupImage;
	public Image OrderSyrupImage;

	public Image SweetsStateImage;
	public Image SweetsImage;
	public Image OrderSweetsImage;

	[Header("Customers Attributes")]
	public GameObject CustomerScene;
	public CustomersManager CustomersManager;
	public GameObject PreparedOrder;
	//Syrup
	public GameObject CocolateSyrup;
	public GameObject StrawberrySyrup;
	public GameObject MapleSyrup;
	public GameObject HotSyrup;
	//Toppings
	public GameObject CocolateTopping;
	public GameObject StrawberryTopping;
	public GameObject BlueberryTopping;
	public GameObject SprinklesTopping;

	[Header("ObiFluid Attributes")]
	public ObiFluidRenderer ObiFluidRenderer;
	public ObiParticleRenderer ObiParticleRenderer;
	public ObiEmitter ObiEmitter;

	#endregion

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

		string dir = Path.Combine(Application.persistentDataPath, "SaveData");
		if (!Directory.Exists(dir))
		{
			SaveTest.Save();
		}
	}
	#endregion

	#region Callbacks Region
	private void Start()
	{
		SaveTest.Load();
		LoadConisNumber();

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
				currentState = State.SyrupState;

				if (IsArrowInsideGreenArea)
				{
					ClickAtrightTime();
				}
				else
				{
					ClickAtWrongTime();
				}
			}
		}

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

		//5.
		Steam.Play();
		CandelLight.SetActive(true);
	}

	private IEnumerator FillingstateCameraMovement()
	{
		Camera.transform.DORotate(new Vector3(28.108f, 180f, 0f), 0.5f);
		Camera.transform.DOMoveZ(2.09f, 0.5f);

		yield return new WaitForSeconds(1f);

		//disable customer scene.
		CustomerScene.SetActive(false);
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

		CookingCanvas.SetActive(true);
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
		//deactivate Filling Attributes.
		Filler.gameObject.SetActive(false);
		FillerCollisionPointsHolder.gameObject.SetActive(false);

		StartCookingState();
		StartCoroutine(Cook());

		CookingCanvas.SetActive(false);
		//StoreCanvas.SetActive(false);
	}

	private IEnumerator Cook()
	{
		//Smoke.SetActive(true);

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

		//VFX.
		Steam.Stop();
		HeartPoof.Play();
		
		Meter.StopArrow();
		Pancake.Flip();
		PanAnimation();

		//Smoke.SetActive(false);
	}

	public void ClickAtWrongTime()
	{
		RightFlip = false;

		//VFX.
		Steam.Stop();
		WhiteSmoke.Play();
		//Smoke.SetActive(false);
		ChangePancakeMaterial();
		BadEmojis[Random.Range(0, BadEmojis.Length - 1)].Play();

		Meter.StopArrow();
		Pancake.Flip();
		PanAnimation();	
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

		//5.VFX
		CandelLight.SetActive(false);
		WhiteSmoke.Stop();

		//6.Pan
		Pancake.transform.parent = null;
		Pan.SetActive(false);

		StartCoroutine(MoveCamera());
	}

	private IEnumerator MoveCamera()
	{
		Camera.transform.DOMove(new Vector3(32.58f, 1.84f, 1.81f), 0.5f);
		Camera.transform.DORotate(new Vector3(40.43f, 180f, 0f), 0.5f);

		yield return new WaitForSeconds(1f);

		SetOrderNotesImages();
		PopupOrderNote();
	}

	#region Syrup State
	public void SetSyrupColor(Color color)
	{
		ObiParticleRenderer.particleColor = color;
	}

	public void StartSyrupState()
    {
		SyrupStage.SetActive(true);
	}

	public void StartObiFluid()
	{
		ObiEmitter.transform.position = CurrentSyrup.PouringPoint.transform.position;

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

		yield return new WaitForSeconds(1f);

		ObiEmitter.enabled = false;
	}

	public void FinishSyrupState()
	{
		currentState = State.SweetingState;

		SyrupStage.SetActive(false);
		SweetsStage.SetActive(true);
	}

	#endregion

	#region Sweeting State

	public void FinishSweetingStage()
	{
		OrderNoteCanvas.SetActive(false);

		//VFX.
		TextEffect.PlayEffect();
		ConfettiBlast.Play();


		//Rest Sweeting Stage.
		CurrentSweeter.ReturnSweeter();
		CurrentSweeter.NumberOfSpawnedSweets = 0;
		CurrentSweeter.Finished = false;
		CurrentSweeter.arrived = false;
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
		if (CustomersManager.CheckVipCustomer())
		{
			AdsManager.ins.ShowRewardedVideo(AdsManager.RewardType.VipCustomer);
		}
		else
		{
			MoveToKitchen();
		}
	}

	public void MoveToKitchen()
	{
		//1.Move to kitchen.
		Camera.transform.DOMoveX(35f, 0.25f).OnComplete(StartFillingState);

		//2.Disable btn.
		DisableCustomerCanvas();

		//3.Disable order.
		OrderCanvas.SetActive(false);
	}

	private IEnumerator ReturnToCustomer()
	{
		//enable customer scene.
		CustomerScene.SetActive(true);

		yield return new WaitForSeconds(2f);

		//1.Move Camera.
		Camera.transform.DOMove(new Vector3(0.8f, 1.030993f, 1.684223f), 0.25f);
		Camera.transform.DORotate(new Vector3(31f, 180f, 0.07f), 0.25f);

		//2.Show Prepared Order.
		PreparedOrder.SetActive(true);
		PreparedOrder.transform.DOMoveX(PreparedOrder.transform.position.x + 0.8f, 0.8f);
		PrepareCustomerOrder();

		//3.Reset
		Reset();

		//4.Result
		CheckResult();
		ShowResultCanvas();

		yield return new WaitForSeconds(2f);

		//5.Enable Collect Canvas.
		EnableCollectCanvas();
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
		//1.Stage One
		FlippingStateImage.rectTransform.sizeDelta = new Vector2(0f, 0f);
		OrderFlippingImage.rectTransform.sizeDelta = new Vector2(0f, 0f);
		//2.Stage Two
		SyrupStateImage.rectTransform.sizeDelta = new Vector2(0f, 0f);
		OrderSyrupImage.rectTransform.sizeDelta = new Vector2(0f, 0f);
		//3.Stage Three
		SweetsStateImage.rectTransform.sizeDelta = new Vector2(0f, 0f);
		OrderSweetsImage.rectTransform.sizeDelta = new Vector2(0f, 0f);

		//Disable Prepared Order.
		PreparedOrder.SetActive(false);
		PreparedOrder.transform.localPosition = new Vector3(-109.568f , 0.237f, 3.082f);

		yield return new WaitForSeconds(2f);

		//enable orderCanvas and generate random order.
		OrderCanvas.SetActive(true);
		OrderManager.GenerateRandomOrder();

		//enable let's cook button.
		EnableCustomerCanvas();
	}

	#endregion

	#region Result Region
	public void CheckResult()
	{
		//Syrup Evaluation.
		EvaluateSyrupStage();

		//Toppings Evaluation.
		EvaluateToppingStage();

		//Flipping Evaluation.
		EvaluateFlippingStage();

		SetEmojiBackgroundImages();
	}

	private void EvaluateSyrupStage()
	{
		if (CurrentSyrup != null && CurrentSyrup.IsSpecialSyrup)
		{
			//Debug.Log("Special Syrup");
			SyrupStateImage.sprite = HeartEyesEvaluationSprite;
		}
		else if (OrderManager.CustomerSyrupOrder.ToString().Equals(SyrupOrder))
		{
			//Debug.Log("Right Syrup");
			SyrupStateImage.sprite = GoodEvaluationSprite;
		}
		else if (CurrentSyrup != null && CurrentSyrup.IsHotSauce)
		{
			//Debug.Log("HotSauce Syrup");
			SyrupStateImage.sprite = AngryEvaluationSprite;
		}
		else
		{
			//Debug.Log("Wrong Syrup");
			SyrupStateImage.sprite = SadEvaluationSprite;
		}
	}

	private void EvaluateToppingStage()
	{
		if (CurrentSweeter != null && CurrentSweeter.IsSpecialSweeter)
		{
			SweetsStateImage.sprite = HeartEyesEvaluationSprite;
		}
		else if (!OrderManager.CustomerSweetsOrder.ToString().Equals(SweetsOrder))
		{
			SweetsStateImage.sprite = SadEvaluationSprite;
		}
		else
		{
			SweetsStateImage.sprite = GoodEvaluationSprite;
		}
	}

	private void EvaluateFlippingStage()
	{
		if (RightFlip)
		{
			FlippingStateImage.sprite = GoodEvaluationSprite;
		}
		else
		{
			FlippingStateImage.sprite = AngryEvaluationSprite;
		}
	}

	public void ShowResultCanvas()
	{
		ResultCanvas.SetActive(true);
		//UpdateCoinsNumber();
		StartCoroutine(PopupEvaluationIcons());
	}

	private void SetEmojiBackgroundImages()
	{
		OrderSyrupImage.sprite = SyrupImage.sprite;
		OrderSweetsImage.sprite = SweetsImage.sprite;
    }

	private IEnumerator PopupEvaluationIcons()
	{
		yield return new WaitForSeconds(0.5f);

		//1.Stage One
		FlippingStateImage.rectTransform.DOSizeDelta(new Vector2(90.20227f, 97.96326f), 0.5f);
		OrderFlippingImage.rectTransform.DOSizeDelta(new Vector2(250f, 250f), 0.5f);

		yield return new WaitForSeconds(0.5f);

		//2.Stage Two
		SyrupStateImage.rectTransform.DOSizeDelta(new Vector2(90.20227f, 97.96326f), 0.5f);
		OrderSyrupImage.rectTransform.DOSizeDelta(new Vector2(190f, 190f), 0.5f);

		yield return new WaitForSeconds(0.5f);


		//3.Stage Three
		SweetsStateImage.rectTransform.DOSizeDelta(new Vector2(90.20227f, 97.96326f), 0.5f);
		OrderSweetsImage.rectTransform.DOSizeDelta(new Vector2(190f, 190f), 0.5f);

		
		if ( (OrderManager.CustomerSyrupOrder.ToString().Equals(SyrupOrder)) && (OrderManager.CustomerSweetsOrder.ToString().Equals(SweetsOrder)) && (RightFlip) )
		{
			//All Items Are Correct.
			ConfettiShower.Play();
		}

		if((CurrentSyrup != null && CurrentSyrup.IsSpecialSyrup) ||(CurrentSweeter != null && CurrentSweeter.IsSpecialSweeter))
		{
			HeartStream.Play();
		}
		
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

	private void UpdateCoinsNumber()
	{
		if (CustomersManager.CheckVipCustomer())
		{
			CoinsValue += 100;
		}
		else
		{
			CoinsValue += 50;
		}
		
		Coinstext.text = CoinsValue.ToString();

		SaveTest.SaveObject.PlayerCurrency = CoinsValue;
		SaveTest.Save();
	}

	private void LoadConisNumber()
	{
		CoinsValue = SaveTest.SaveObject.PlayerCurrency;
		Coinstext.text = CoinsValue.ToString();
	}

	#region CustomersUI
	public void EnableCustomerCanvas()
	{
		StoreCanvas.SetActive(true);

		if (CustomersManager.CheckVipCustomer())
		{
			VipCustomerCanvas.SetActive(true);
			StarField.Play();
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
		StoreCanvas.SetActive(false);
	}

	public void OnClick_NoThanksButtons()
	{
		StartCoroutine(NoThanksButton());
	}

	IEnumerator NoThanksButton()
	{
		StarField.Stop();

		CustomersManager.NextCustomer();
		DisableCustomerCanvas();
		OrderCanvas.SetActive(false);


		yield return new WaitForSeconds(2f);

		//enable orderCanvas and generate random order.
		yield return new WaitForSeconds(3f);
		OrderCanvas.SetActive(true);
		EnableCustomerCanvas();
		OrderManager.GenerateRandomOrder();
	}

	private  void EnableCollectCanvas()
	{
		CollectCanvas.SetActive(true);

		if (CustomersManager.CheckVipCustomer())
		{
			EarnedCoinsValue.text = "100";
		}
		else
		{
			EarnedCoinsValue.text = "50";
		}
	}

	private void DisableCollectCanvas()
	{
		CollectCanvas.SetActive(false);
	}

	public void OnClickCollectButton()
	{
		UpdateNumberOfInitialCustomers();

		if (SaveTest.SaveObject.NumberOfInitialCustomers == 3)
		{
			UpgradeDiner();
			UpdateCoinsNumber();
			return;
		}

		//vfx.
		StarField.Stop();
		ConfettiShower.Stop();
		HeartStream.Stop();
		DollarBlast.Play();

		UpdateCoinsNumber();
		DisableCollectCanvas();
		NextCustomer();
	}

	#endregion

	#region Order Notes
	private void SetOrderNotesImages()
	{
		SyrupNote.sprite = SyrupImage.sprite;
		ToppingNote.sprite = SweetsImage.sprite;
	}

	private void PopupOrderNote()
    {
		OrderNoteCanvas.SetActive(true);

		/*
		OrderNote.rectTransform.DOSizeDelta(new Vector2(429.8884f, 366.8954f) ,1f);
		SyrupNote.rectTransform.DOSizeDelta(new Vector2(240f, -344f), 1f);
		ToppingNote.rectTransform.DOSizeDelta(new Vector2(383f, -368.0001f), 1f);*/
	}

    #endregion

    private void PrepareCustomerOrder()
	{
		CocolateSyrup.SetActive(false);
	    StrawberrySyrup.SetActive(false);
		MapleSyrup.SetActive(false);
		HotSyrup.SetActive(false);
		//Syrup
		if (SyrupOrder.Equals("withChocolateSyrup"))
		{
			CocolateSyrup.SetActive(true);
		}
        else if (SyrupOrder.Equals("withStrawberrySyrup"))
        {
			StrawberrySyrup.SetActive(true);
		}
		else if (SyrupOrder.Equals("withMapleSyrup"))
		{
			MapleSyrup.SetActive(true);
		}
		else
		{
			HotSyrup.SetActive(true);
		}

		//Topping
		CocolateTopping.SetActive(false);
		StrawberryTopping.SetActive(false);
		BlueberryTopping.SetActive(false);
		SprinklesTopping.SetActive(false);

		if (SweetsOrder.Equals("withChocolate"))
		{
			CocolateTopping.SetActive(true);
		}
		else if (SweetsOrder.Equals("withStrawberry"))
		{
			StrawberryTopping.SetActive(true);
		}
		else if(SweetsOrder.Equals("withBlueBerries"))
		{
			BlueberryTopping.SetActive(true);
		}
		else
        {
			SprinklesTopping.SetActive(true);
		}
		
	}

	public void Onclick_StoreButton()
	{
		SceneManager.LoadScene("StoreScene");
	}

	private void UpdateNumberOfInitialCustomers()
	{
		SaveTest.SaveObject.NumberOfInitialCustomers++;
		SaveTest.Save();
	}

	#region UpgradeDinerStore
	private void CheckForDinerUpgrade()
	{
		if (SaveTest.SaveObject.NumberOfInitialCustomers == 3)
		{
			UpgradeDiner();
		}
	}

	private void UpgradeDiner()
	{
		UpgradeDinerStore.SetActive(true);

		CustomerScene.SetActive(false);
		ResultCanvas.SetActive(false);
		CollectCanvas.SetActive(false);

		ConfettiShower.Stop();
		HeartStream.Stop();
	}

	public void Onclick_ClaimUpgradeButton()
	{
		SceneManager.LoadScene("StoreScene");
	}
	#endregion

}
