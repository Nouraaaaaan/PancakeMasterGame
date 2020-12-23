using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cake : MonoBehaviour
{
    [SerializeField] ColorTrigger colorTrigger;
    [Space]
    public int icingAmountNeeded = 0;
    [SerializeField] float extrRange = 1;
    [SerializeField] bool done = false;
    [SerializeField] MeshFilter rawCake, doneCake;

    int[] triangles;
    Color[] colors;
    Vector3[] vertices;
    Vector3[] originalVertices;
    Vector3[] normals;

    Mesh meshRaw;
    Mesh meshDone;

    private void Awake()
    {
        meshRaw = rawCake.mesh;
        meshDone = doneCake.mesh;

        originalVertices = meshDone.vertices;
        vertices = meshDone.vertices;
        triangles = meshDone.triangles;
        normals = meshDone.normals;

        colors = new Color[meshDone.vertices.Length];

        HideMesh();
    }

    void HideMesh()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 dir = vertices[i] - transform.position;

            vertices[i] = originalVertices[i] + dir.normalized * extrRange;
        }

        UpdateMesh();
    }

    void CheckForCompletness()
    {
        float averegDistance = 0;
        for (int i = 0; i < vertices.Length; i++)
        {
            averegDistance += Vector3.Distance(vertices[i], originalVertices[i]);
        }
        averegDistance /= vertices.Length;

        if (averegDistance < 0.01f)
            done = true;
        else
            done = false;

        float fill = (0.29f - averegDistance) / 0.29f;
       
        /*if (UIManager.instance)
        {
            UIManager.instance.UpdateSliderVale(fill);
        }*/

        if (fill >= 0.92f)
        {
            GameManagerCakeMaster.Instance.GameOver(CheckMatchingPrcent());
            enabled = false;
        }
    }

    public void UpdateMesh()
    {
        meshDone.vertices = vertices;
        meshDone.triangles = triangles;
        meshDone.colors = colors;
    }

    public void ReturToNormal(Vector3 center, float radius)
    {
        bool isDoneHere = true;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 point = transform.rotation * (meshRaw.vertices[i] - rawCake.transform.position) + rawCake.transform.position;
            float dist = Vector3.Distance(point, center);

            if (dist < radius)
            {
                vertices[i] = Vector3.Lerp(vertices[i], originalVertices[i], 0.2f);
            }
        }

        done = isDoneHere;

        UpdateMesh();
        CheckForCompletness();
    }

    public void ColorMesh(Color color, Vector3 center, float radius)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 point = VertexPosition(originalVertices[i], Vector3.zero, transform.rotation) + transform.position;
            float dist = Vector3.Distance(point, center);

            if (dist < radius)
            {
                colors[i] = color;
            }
        }
    }

    public void ColorCake(Color color)
    {
        StartCoroutine(ColorCakeIEnum(color));
    }

    IEnumerator ColorCakeIEnum(Color color)
    {
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = color;
        }
        UpdateMesh();
    }

    public float CheckMatchingPrcent()
    {
        int points = vertices.Length;

        if (colorTrigger == null)
            colorTrigger = FindObjectOfType<ColorTrigger>();

        for (int i = 0; i < vertices.Length; i++)
        {
            if (colorTrigger.vertexColors[i] != colors[i])
                points--;
        }

        return (float)points / vertices.Length;
    }

    /////////////////////////////////////////

    private void OnDrawGizmos()
    {
        if (vertices != null)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 point = VertexPosition(originalVertices[i], Vector3.zero, transform.rotation) + transform.position;
                Gizmos.DrawSphere(point, 0.03f);
            }
        }
    }

    Vector3 VertexPosition(Vector3 vertex, Vector3 center, Quaternion rotation)
    {
        return rotation * (vertex - center) + center;
    }
}
