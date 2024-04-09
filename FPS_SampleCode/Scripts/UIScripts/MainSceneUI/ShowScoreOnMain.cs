using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public class ShowScoreOnMain : MonoBehaviour
{
    TextMeshProUGUI text;

    void OnEnable()
    {
        SaveData.Load();
    }

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        ShowScore();
    }

    void ShowScore()
    {
        text.text = $"Best Score : {GameManager.instance.Score}";
    }
}
