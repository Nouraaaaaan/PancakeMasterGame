using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.InkPainter;
using DG.Tweening;

public class PancakeLevelManager : MonoBehaviour
{
	enum State
	{
		FillingState,
		CookingState,
		FlippingState,
		SyrupState,
		SweetingState
	}
	private State currentState;

	[Header("Filling Attributes")]
	public GameObject Filler;
	public GameObject FillerCollisionPoints;
	public ParticleSystem FillingParticle;
	public int NumberOfPointsToFill;
	int NumberOfFilledPoints;
	
	[Header("Painting Attributes")]
	[SerializeField]
	private Brush brush;
	[SerializeField]
	LayerMask PaintLayer;
	RaycastHit hit;
	public GameObject PaintingQuad;
	[SerializeField]
	private Brush brush2;

	[Header("Cooking Attributes")]
	public Pancake Pancake;
	public Material PancakeMaterial;
	private Color color;

	[Header("Flipping Attributes")]
	public Meter Meter;
	public GameObject Arrow;
	public bool IsArrowInsideGreenArea;

	[Header("Sweeting Attributes")]
	public Camera Camera;
	public Transform CameraNewPos;
	public GameObject Pan;
	public Transform PanNewPos;
	public Transform PancakeNewPos;
	public Transform PancakeInitialPos;
	public GameObject Sweeter;
	public Transform SweeterInitialPos;
	public Transform SweeterFinalPos;

	[Header("Syrup Attributes")]
	public GameObject Syrup;
	public ParticleSystem SyrupVFX;
	public Transform SyrupInitialPos;
	public Transform SyrupFinalPos;
	public GameObject PouringPoint;
	public GameObject SyrupPaintingQuad;
	public int NumberofSyrupPoints;

	[Header("UI")]
	public GameObject CookingCanvas;

	[Header("VFX")]
	public ParticleSystem[] GoodEmojis;
	public ParticleSystem[] BadEmojis;
	public TextEffect TextEffect;
	public GameObject Smoke;

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

    private void Start()
    {
		currentState = State.FillingState;

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

				}
				else
				{
					ClickAtWrongTime();
				}
			}
        }

		else if (currentState == State.SyrupState)
		{
			if (Input.GetMouseButtonDown(0))
			{
				SyrupVFX.Play();
			}

			else if (Input.GetMouseButton(0))
			{
				if (Physics.Raycast(PouringPoint.transform.position, -Vector3.up, out hit, 50, PaintLayer))
				{
					var paintObject = hit.transform.GetComponent<InkCanvas>();

					if (paintObject != null)
						paintObject.Paint(brush2, hit);

				}
			}

			else if (Input.GetMouseButtonUp(0))
			{
				SyrupVFX.Stop();
			}
		}

	}

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

	#endregion

	#region Cooking state
	private void StartCookingState()
	{
		currentState = State.CookingState;
		Filler.gameObject.SetActive(false);
		FillerCollisionPoints.gameObject.SetActive(false);

		CookingCanvas.SetActive(true);
		Pancake.gameObject.SetActive(true);
	}

	public void ClickCookButton()
	{
		StartCoroutine(Cook());
		CookingCanvas.SetActive(false);

	}

	IEnumerator Cook()
	{
		Smoke.SetActive(true);

		while (PancakeMaterial.color.a < 1f)
		{
			PancakeMaterial.color = new Color(color.r, color.g, color.b, PancakeMaterial.color.a + 0.008f);
			yield return new WaitForEndOfFrame();
		}

		yield return null;
		Smoke.SetActive(false);
		PaintingQuad.SetActive(false);
		StartFlippingState();
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
		Meter.StopArrow();
		Pancake.Flip();
	}

	public void ClickAtWrongTime()
	{
		//Meter.StopArrow();
		BadEmojis[Random.Range(0, BadEmojis.Length - 1)].Play();
	}
	#endregion

	#region Syrup State
	public void UpdateNumberOfSyrupPoints()
	{
		NumberofSyrupPoints++;

		if (NumberofSyrupPoints >= 4)
		{
			StartCoroutine(FinishSyrupState());
		}
	}

	private IEnumerator FinishSyrupState()
	{
		currentState = State.SweetingState;

		SyrupVFX.Stop();
		Syrup.transform.DOMove(SyrupInitialPos.position, 1f);
		Syrup.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 1f);
		Syrup.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

		yield return new WaitForSeconds(2f);

		MoveSweeter();
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
		Camera.transform.DOMove(CameraNewPos.position , 2f);
		yield return new WaitForSeconds(1f);

		Pancake.transform.parent = Pan.transform;
		Pan.transform.DOMove(PanNewPos.position, 2f);
		yield return new WaitForSeconds(1f);

		Pan.transform.DORotate(new Vector3(40f, 0f, 0f), 2f);
		yield return new WaitForSeconds(1f);

		Pancake.gameObject.transform.DOMove(PancakeNewPos.position, 1f);
		Pancake.gameObject.transform.DOLocalRotate(new Vector3(-48.963f, 0.001f, 180f), 1f);
		yield return new WaitForSeconds(1f);

		Pancake.transform.parent = null;
		Pan.SetActive(false);

		//MoveSweeter();
		MoveSyrup();
	}

	private void MoveSyrup()
	{
		currentState = State.SyrupState;

		Syrup.transform.DOMove(SyrupFinalPos.position, 1f);
		Syrup.transform.DOLocalRotate(new Vector3(0f, 0f, -51.728f), 1f);

		SyrupPaintingQuad.SetActive(true);
	}

	private void MoveSweeter()
    {
		Sweeter.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 1f);
		Sweeter.transform.DOMove(SweeterFinalPos.position, 1f);
	}

	public void FinishSweetingStage()
	{
		//Debug.Log("Sweeting Stage has Finished !");

		//SweeterSatge.SetActive(false);
		SFXManager.Instance.StopSoundEffect();
		TextEffect.PlayEffect();
	}

    #endregion

}
