using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentGoldText : MonoBehaviour
{
    [SerializeField] Text currentGold;

    // Update is called once per frame
    void Update()
    {
        ShowCurrentPlayerGold();
    }

    void ShowCurrentPlayerGold()
    {
        currentGold.text = $"{LocalPlayer.LP.CurrentGold}";
    }
}
