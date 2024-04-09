using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentLevelText : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] TextMeshProUGUI showCurrentLevel;

    // Update is called once per frame
    void Update()
    {
        ShowCurrentPlayerExp();
    }

    void ShowCurrentPlayerExp()
    {
        showCurrentLevel.text = $"·¹º§ {player.Level}";
    }
}
