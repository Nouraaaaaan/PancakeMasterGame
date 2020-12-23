using UnityEngine;

public class LiquidPaint : MonoBehaviour 
{
	public int Resolution = 1024;
	public Shader LiquidPaintShader;
	[SerializeField] float paintMultiply = 1;
	[SerializeField] Color colorPaint = new Color(1.0f, 0.99f, 0.81f, 1f);

	Material material;
	RenderTexture RTA1, RTA2, RTB1, RTB2;
	bool swap = true;

	private Camera mainCamera;
	private RaycastHit hit;

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
			
	void Start () 
	{
		material = new Material(LiquidPaintShader);
		RTA1 = new RenderTexture(Resolution, Resolution, 0, RenderTextureFormat.ARGBFloat);  //buffer must be floating point RT
		RTA2 = new RenderTexture(Resolution, Resolution, 0, RenderTextureFormat.ARGBFloat);  //buffer must be floating point RT
		RTB1 = new RenderTexture(Resolution, Resolution, 0, RenderTextureFormat.ARGBFloat);  //buffer must be floating point RT
		RTB2 = new RenderTexture(Resolution, Resolution, 0, RenderTextureFormat.ARGBFloat);  //buffer must be floating point RT		
		GetComponent<Renderer>().material = material;

		mainCamera = Camera.main;
		material.SetColor("paintColor", colorPaint);
	}
	
	void Update () 
	{		
		//RaycastHit hit;
		if (Input.GetMouseButton(0))
		{
			if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition) , out hit))
				material.SetVector("iMouse", new Vector2(
					hit.textureCoord.x * Resolution, hit.textureCoord.y * Resolution));
		}
		else
		{
			material.SetVector("iMouse", new Vector2(-1000.0f, -1000.0f));
		}
		
		//material.SetInt("iFrame",Time.frameCount);
		material.SetVector("iResolution", new Vector2(Resolution,Resolution));
		
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
	
	void OnDestroy ()
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
		}
		
    }

    public void DestroyData()
    {
		OnDestroy();
    }
}