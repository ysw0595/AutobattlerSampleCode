using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowTime : MonoBehaviour
{
    TextMeshProUGUI time;

    private void Start()
    {
        time = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        time.text = $"Time : {GameManager.instance.GetTime()}";
    }
}
