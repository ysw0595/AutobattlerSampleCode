using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowUnitHp : MonoBehaviour
{
    [SerializeField] Slider hp;
    [SerializeField] TextMeshProUGUI hpText;

    Unit unit;

    private void Start()
    {
        unit = gameObject.GetComponentInParent<Unit>();
    }

    private void Update()
    {
        hp.maxValue = unit.GetMaxHp();
        hp.value = unit.GetHp();
        hpText.text = $"{hp.value} / {hp.maxValue}";
    }
}
