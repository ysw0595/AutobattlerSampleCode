using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : MonoBehaviour, ISpeed
{
    Unit unit;

    public void Awake()
    {
        unit = gameObject.GetComponent<Unit>();
    }

    private void OnEnable()
    {
        UpSpeed();
    }

    private void OnDisable()
    {
        DownSpeed();
    }

    public void UpSpeed()
    {
        unit.Atk += 3;
        unit.MaxHp += 10;
        unit.SetMaxHp();
    }

    public void DownSpeed()
    {
        unit.Atk -= 3;
        unit.MaxHp -= 10;
        unit.SetMaxHp();
    }
}
