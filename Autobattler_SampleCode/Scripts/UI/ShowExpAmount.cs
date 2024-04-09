using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class ShowExpAmount : MonoBehaviour
{
    [SerializeField] Slider slider;

    // Update is called once per frame
    void Update()
    {
        ShowExpLevel();
    }

    void ShowExpLevel()
    {
        slider.value = LocalPlayer.LP.CurrentExp;
        slider.maxValue = LocalPlayer.LP.ExpForUp;
    }
}
