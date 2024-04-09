using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : MonoBehaviour, IRange
{
    Unit unit;

    public void Awake()
    {
        unit = gameObject.GetComponent<Unit>();
    }

    private void OnEnable()
    {
        UpRange();
    }

    private void OnDisable()
    {
        DownRange();
    }

    public void UpRange()
    {
        unit.Range += 0.3f;
    }

    public void DownRange()
    {
        unit.Range -= 0.3f;
    }
}
