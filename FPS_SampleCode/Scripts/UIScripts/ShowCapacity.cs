using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowCapacity : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp;

    [SerializeField] GunController gc;

    private void Update()
    {
        tmp.text = $"{gc.GetCapacity().ToString()} / 30";
    }
}