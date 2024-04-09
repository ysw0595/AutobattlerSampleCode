using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour, IMelee
{
    Unit unit;

    public void Awake()
    {
        unit = gameObject.GetComponent<Unit>();
    }

    private void OnEnable()
    {
        UpMelee();
    }

    private void OnDisable()
    {
        DownMelee();
    }

    public void UpMelee()
    {
        unit.Atk += 3;
    }

    public void DownMelee()
    {
        unit.Atk -= 3;
    }
}
