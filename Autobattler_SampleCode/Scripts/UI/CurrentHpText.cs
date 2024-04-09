using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentHpText : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] TextMeshProUGUI showCurrentHp;

    // Update is called once per frame
    void Update()
    {
        ShowCurrentExpAmount();
    }

    void ShowCurrentExpAmount()
    {
        showCurrentHp.text = $"{player.currentHp} / 100";
    }
}
