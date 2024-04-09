using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentExpText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI showCurrentExp;

    // Update is called once per frame
    void Update()
    {
        ShowCurrentExpAmount();
    }

    void ShowCurrentExpAmount()
    {
        showCurrentExp.text = $"{LocalPlayer.LP.CurrentExp} / {LocalPlayer.LP.ExpForUp}";
    }
}
