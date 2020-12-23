using UnityEngine;

public class Donut : MonoBehaviour
{
    [SerializeField] MeshRenderer syrupRender;

    public void ChangeSyrupColor(Color newColor)
    {
        syrupRender.gameObject.SetActive(true);
        syrupRender.material.color = newColor;
    }
}
