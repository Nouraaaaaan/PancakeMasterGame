using UnityEngine;

public class ColorTrigger : MonoBehaviour
{
    public BoxCollider[] colorRegions;

    [HideInInspector]
    public Color[] vertexColors => colors;

    MeshFilter meshFilter;
    Mesh mesh;
    Color[] colors;
    Vector3[] vertices;
    LevelGenerator levelGenerator;

    private void Awake()
    {
        meshFilter = GetComponentInParent<MeshFilter>();
        mesh = meshFilter.mesh;

        vertices = mesh.vertices;
        colors = new Color[mesh.vertices.Length];
    }

    public void Init()
    {    
        levelGenerator = FindObjectOfType<LevelGenerator>();
        Gameplay gameplay = FindObjectOfType<Gameplay>();

        for (int i = 0; i < colorRegions.Length; i++)
        {
            Color currentColor = levelGenerator.GetColor();
            gameplay.icingColors.Add(currentColor);
            SetRegionColor(colorRegions[i], currentColor);
        }
    }

    public void SetRegionColor(BoxCollider collider, Color color)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 point = VertexPosition(vertices[i], Vector3.zero, transform.parent.rotation) + transform.parent.position;
            if (collider.bounds.Contains(point))
            {
                colors[i] = color;
            }
        }

        mesh.colors = colors;
    }

    private void OnDrawGizmos()
    {
        if (colorRegions != null)
        {
            for (int i = 0; i < colorRegions.Length; i++)
            {
                Color newColor = Color.blue;
                newColor.a = 0.4f;
                Gizmos.color = newColor;

                Gizmos.DrawCube(colorRegions[i].transform.position, colorRegions[i].transform.localScale);
            }
        }
    }

    Vector3 VertexPosition(Vector3 vertex, Vector3 center, Quaternion rotation)
    {
        return rotation * (vertex - center) + center;
    }
}
