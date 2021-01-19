using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.InkPainter;

public class Filler : MonoBehaviour
{
	[Header("Painting Attributes")]
	[SerializeField]
	private Brush brush;
	[SerializeField]
	LayerMask PaintLayer;
	RaycastHit hit;

	[Header("Dragging Attributes")]
	public float DragSpeed;
	private Vector3 lastMousePos;

	[Header("Boundaries Attributes")]
	public float MaxPosX;
	public float MinPosX;
	public float MaxPosZ;
	public float MinPosZ;

	[Header("VFX Attributes")]
	public ParticleSystem FillingParticle;

	private void Update()
    {
		if (PancakeLevelManager.Instance.currentState == PancakeLevelManager.State.FillingState)
		{
			if (Input.GetMouseButtonDown(0))
			{
				//VFX.
				FillingParticle.Play();

				//SFX.
				if (!SFXManager.Instance.AudioSource.isPlaying)
				{
					//SFX
					SFXManager.Instance.EnableLoopingOption();
					SFXManager.Instance.SetAudioVolume(0.3f);
					SFXManager.Instance.PlaySoundEffect(7);
				}
			}

			else if (Input.GetMouseButton(0))
			{
				if (Physics.Raycast(transform.position, -Vector3.up, out hit, 50, PaintLayer))
				{
					var paintObject = hit.transform.GetComponent<InkCanvas>();

					if (paintObject != null)
						paintObject.Paint(brush, hit);

				}

				DragFiller();
			}

			else if (Input.GetMouseButtonUp(0))
			{
				FillingParticle.Stop();
			}
		}
	}

	private void DragFiller()
	{
		//Debug.Log("Dragging");

		Vector3 delta = Input.mousePosition - lastMousePos;
		Vector3 pos = transform.position;

		pos.x += delta.x * DragSpeed * -1f;
		pos.z += delta.y * DragSpeed * -1f;

		if ((MinPosX <= pos.x) && (MaxPosX >= pos.x) && (MinPosZ <= pos.z) && (MaxPosZ >= pos.z))
		{
			transform.position = pos;
		}

		lastMousePos = Input.mousePosition;
	}
}
