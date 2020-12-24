using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.InkPainter;

public class PancakeLevelManager : MonoBehaviour
{
	[Header("Filling Attributes")]
	public GameObject Filler;
	public ParticleSystem FillingParticle;
	public int NumberOfPointsToFill;
	int NumberOfFilledPoints;
	
	[Header("Painting Attributes")]
	[SerializeField]
	private Brush brush;
	[SerializeField]
	LayerMask PaintLayer;
	RaycastHit hit;

	[Header("Cooking Attributes")]
	public GameObject Pancake;
	public Material PancakeMaterial;

	[Header("Sweeting Attributes")]
	public Sweeter Sweeter;

	[Header("UI")]
	public GameObject CookingCanvas;

	[Header("VFX")]
	public ParticleSystem[] GoodEmojis;

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

	private void Update()
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

	#region Filling State
	public void UpdateNumberOfFilledPoints()
	{
		NumberOfFilledPoints++;

		if (NumberOfFilledPoints == NumberOfPointsToFill)
		{
			Debug.Log("Filling Stage has finished !");
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
		CookingCanvas.SetActive(true);
		Filler.gameObject.SetActive(false);
		Pancake.SetActive(true);
	}

	public void ClickCookButton()
	{
		StartCoroutine(Cook());
	}

	IEnumerator Cook()
	{
		var color = PancakeMaterial.color;

		while (PancakeMaterial.color.a < 1f)
		{
			PancakeMaterial.color = new Color(color.r, color.g, color.b, PancakeMaterial.color.a + 0.008f);
			yield return new WaitForEndOfFrame();
		}

		yield return null;
		StartSweetingState();
	}

	#endregion

	#region Sweeting State

	private void StartSweetingState()
	{
		Sweeter.gameObject.SetActive(true);
	}

    #endregion

}
