using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

public class ShowTimeController : MonoBehaviour
{
    [SerializeField] Slider timeGraph;
    [SerializeField] Text timeText;
    [SerializeField] Text turnText;
    float time;

    private void Start()
    {
        time = GameManager.Instance.ReadyTime();
    }

    public void ShowTimeGraph(float maxValue, float timeFlow)
    {
        time = maxValue - timeFlow;
        timeGraph.maxValue = maxValue;
        timeGraph.value = time;
        timeText.text = $"{(int)time}";
    }

    public void ShowTurnText(bool turn)
    {
        if (turn) { turnText.text = "준비"; }
        else if(!turn) { turnText.text = "전투"; }
    }
}
