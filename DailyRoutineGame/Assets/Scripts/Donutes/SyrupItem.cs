using UnityEngine;

public class SyrupItem : MonoBehaviour
{
    [SerializeField] Color syrupColor;

    public Color ReturnColor()
    {
        return syrupColor;
    }
}
