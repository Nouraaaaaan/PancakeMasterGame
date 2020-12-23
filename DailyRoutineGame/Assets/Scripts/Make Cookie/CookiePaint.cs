using System.Collections;
using UnityEngine;

public class CookiePaint : MonoBehaviour
{
	[Range(16,1024)]
	[SerializeField] int Resolution = 256;
	[SerializeField] Shader LiquidPaintShader;
	[SerializeField] float paintMultiply = 1;

	[Header("Mat Settings")]
	[SerializeField][ColorUsage(true,true)] Color colorPaint = new Color(1.0f, 0.99f, 0.81f, 1f);
	[SerializeField][Range(0.0f,2.0f)] float diffuseAddition = 1.5f;

	[Header("Bake Settings")]
	[SerializeField] Color bakecolor;
	[SerializeField] Vector2 yPosRangesBack = new Vector2(0.3f, 0.5f);

	Material material;
	RenderTexture RTA1, RTA2, RTB1, RTB2;
	bool swap = true;

	//private Camera mainCamera;
	//private RaycastHit hit;

	[HideInInspector] public bool isPainting = false;
	[HideInInspector]  public Vector2 hitTexCord;


	void Blit(RenderTexture source, RenderTexture destination, Material mat, string name, int pass)
	{
		RenderTexture.active = destination;
		mat.SetTexture(name, source);
		GL.PushMatrix();
		GL.LoadOrtho();
		GL.invertCulling = true;
		mat.SetPass(pass);
		GL.Begin(GL.QUADS);
		GL.MultiTexCoord2(0, 0.0f, 0.0f);
		GL.Vertex3(0.0f, 0.0f, 0.0f);
		GL.MultiTexCoord2(0, 1.0f, 0.0f);
		GL.Vertex3(1.0f, 0.0f, 0.0f);
		GL.MultiTexCoord2(0, 1.0f, 1.0f);
		GL.Vertex3(1.0f, 1.0f, 0.0f);
		GL.MultiTexCoord2(0, 0.0f, 1.0f);
		GL.Vertex3(0.0f, 1.0f, 0.0f);
		GL.End();
		GL.invertCulling = false;
		GL.PopMatrix();
	}

	void Start()
	{
		material = new Material(LiquidPaintShader);
		RTA1 = new RenderTexture(Resolution, Resolution, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Default);  //buffer must be floating point RT
		RTA2 = new RenderTexture(Resolution, Resolution, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Default);  //buffer must be floating point RT
		RTB1 = new RenderTexture(Resolution, Resolution, 32, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Default);  //buffer must be floating point RT
		RTB2 = new RenderTexture(Resolution, Resolution, 32, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Default);  //buffer must be floating point RT		
		GetComponent<Renderer>().material = material;

		Debug.Log(SystemInfo.supportedRenderTargetCount + " : " + SystemInfo.SupportsBlendingOnRenderTextureFormat(RenderTextureFormat.ARGBHalf) + " : " + SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf));
		//mainCamera = Camera.main;
		material.SetColor("paintColor", colorPaint);
		material.SetFloat("diffuseAddition", diffuseAddition);
	}

	void Update()
	{
		//RaycastHit hit;
		if (isPainting)
		{
			//if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit))
				material.SetVector("iMouse", new Vector2(
					hitTexCord.x * Resolution, hitTexCord.y * Resolution));
		}
		else
		{
			material.SetVector("iMouse", new Vector2(-1000.0f, -1000.0f));
		}

		//material.SetInt("iFrame",Time.frameCount);
		material.SetVector("iResolution", new Vector2(Resolution, Resolution));

		if (swap)
		{
			material.SetTexture("_BufferA", RTA1);
			Blit(RTA1, RTA2, material, "_BufferA", 0);
			material.SetTexture("_BufferA", RTA2);

			material.SetTexture("_BufferB", RTB1);
			Blit(RTB1, RTB2, material, "_BufferB", 1);
			material.SetTexture("_BufferB", RTB2);
		}
		else
		{
			material.SetTexture("_BufferA", RTA2);
			Blit(RTA2, RTA1, material, "_BufferA", 0);
			material.SetTexture("_BufferA", RTA1);

			material.SetTexture("_BufferB", RTB2);
			Blit(RTB2, RTB1, material, "_BufferB", 1);
			material.SetTexture("_BufferB", RTB1);
		}

		swap = !swap;
	}

	void OnDestroy()
	{
		RTA1.Release();
		RTA2.Release();
		RTB1.Release();
		RTB2.Release();
	}

	private void OnValidate()
	{
		if (material != null)
		{
			material.SetColor("paintColor", colorPaint);
			material.SetFloat("diffuseAddition", diffuseAddition);
		}

	}

    private void OnDisable()
    {
		OnDestroy();
	}

    public void DestroyData()
	{
		OnDestroy();
	}

	public void LerpBakeCall()
    {
		StartCoroutine(LerpBake());
    }

	IEnumerator LerpBake()
    {
		float time = 0;
        while (time < 1.5f)
        {
			time += Time.deltaTime;
			material.SetColor("paintColor", Color.Lerp(colorPaint,bakecolor, time/1.5f));
			transform.localPosition = new Vector3(transform.localPosition.x,
				transform.localPosition.y, Mathf.Lerp(yPosRangesBack.x, yPosRangesBack.y, time/1.5f));

			yield return null;
        }

		MakeCookie_LevelManager.instance.OnLevelAction(true);
    }
}
