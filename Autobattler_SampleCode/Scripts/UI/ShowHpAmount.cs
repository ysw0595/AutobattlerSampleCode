using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHpAmount : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Slider slider;

    private void Update()
    {
        ShowHp();
    }

    void ShowHp()
    {
        slider.value = player.currentHp;
        slider.maxValue = 100;
    }
}
