using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowHp : MonoBehaviour
{
    [SerializeField] CharacterBase cb;
    Slider hpSlider;
    [SerializeField] TextMeshProUGUI hpText;

    private void Start()
    {
        hpSlider = GetComponent<Slider>();
        hpSlider.minValue = 0f;
        hpSlider.maxValue = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        float hp = cb.GetHp();
        if(hp < 0f) { hp = 0f;}
        hpText.text = $"{hp} / {hpSlider.maxValue}";
        ShowHP();
    }

    void ShowHP()
    {
        hpSlider.value = cb.GetHp();
    }
}
