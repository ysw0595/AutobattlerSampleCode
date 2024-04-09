using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitCountLimit : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI unitCountText;

    public void ShowUnitCountText()
    {
        unitCountText.text = $"{LocalPlayer.LP.UnitCount} / {LocalPlayer.LP.LimitUnitCount}";
    }
}
