using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Classic : MonoBehaviour, IClassic
{
    Unit unit;

    public void Awake()
    {
        unit = gameObject.GetComponent<Unit>();
    }

    private void OnEnable()
    {
        UpClassic();
    }

    private void OnDisable()
    {
        DownClassic();
    }

    public void UpClassic()
    {
        unit.MaxHp += 10;
        unit.SetMaxHp();
        unit.Atk += 5;
        unit.Range += 0.3f;
    }

    public void DownClassic()
    {
        unit.MaxHp -= 10;
        unit.SetMaxHp();
        unit.Atk -= 5;
        unit.Range -= 0.3f;
    }
}
