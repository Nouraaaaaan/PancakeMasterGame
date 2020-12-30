using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Syrup : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && (PancakeLevelManager.Instance.CanAddSyrup))
        {
            PancakeLevelManager.Instance.MoveSyrup();
            PancakeLevelManager.Instance.CanAddSyrup = false;
        }
    }

}
