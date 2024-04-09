using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ancient : MonoBehaviour, IAncient
{
    Unit unit;
    public int plus = 0;

    public void Awake()
    {
        unit = gameObject.GetComponent<Unit>();
    }

    private void OnEnable()
    {
        UpAncient();
    }

    private void OnDisable()
    {
        DownAncient();
    }

    public void UpAncient()
    {
        unit.MaxHp += 5;
        unit.SetMaxHp();
        unit.Atk += 2;
        unit.Range += 0.2f;
    }

    public void DownAncient()
    {
        unit.MaxHp -= 5;
        unit.SetMaxHp();
        unit.Atk -= 2;
        unit.Range -= 0.2f;
    }
}
