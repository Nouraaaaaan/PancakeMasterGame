using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (PancakeLevelManager.Instance.IsArrowInsideGreenArea)
            {
                //Debug.Log("true !");
                PancakeLevelManager.Instance.ClickAtrightTime();
                
            }
            else
            {
                //Debug.Log("false !");
                PancakeLevelManager.Instance.ClickAtWrongTime();
            }
        }
    }

}
