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

	[Header("Baking Attributes")]
	public GameObject Pancake;

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

	public void UpdateNumberOfFilledPoints()
	{
		NumberOfFilledPoints++;

		if (NumberOfFilledPoints == NumberOfPointsToFill)
		{
			Debug.Log("Filling Stage has finished !");
			StartBakingState();
		}
	}

	private void StartBakingState()
	{
		Filler.gameObject.SetActive(false);
		Pancake.SetActive(true);
	}
}
