using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.InkPainter;

public class PancakeLevelManager : MonoBehaviour
{
	enum State
	{
		FillingState,
		CookingState,
		FlippingState,
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

	[Header("Cooking Attributes")]
	public Pancake Pancake;
	public Material PancakeMaterial;
	private Color color;

	[Header("Flipping Attributes")]
	public Meter Meter;
	public GameObject Arrow;
	public bool IsArrowInsideGreenArea;

	[Header("Sweeting Attributes")]
	public GameObject SweeterSatge;

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

    #region Sweeting State

    public void StartSweetingState()
	{
		//Debug.Log("Sweeting Stage has Started !");
		currentState = State.SweetingState;
		Pancake.FreezePancake();
		SweeterSatge.SetActive(true);
	}

	public void FinishSweetingStage()
	{
		//Debug.Log("Sweeting Stage has Finished !");

		SweeterSatge.SetActive(false);
		SFXManager.Instance.StopSoundEffect();
		TextEffect.PlayEffect();
	}

    #endregion

}
