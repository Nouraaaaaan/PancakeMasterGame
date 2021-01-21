using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void OnClickSettingsButton()
    {
        Time.timeScale = 0;
    }

    public void OnClickCancelButton()
    {
        Time.timeScale = 1;
    }

}
