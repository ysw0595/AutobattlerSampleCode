using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellController : MonoBehaviour
{
    public void SellUnit(GameObject obj)
    {
        Unit unit = obj.GetComponent<Unit>();
        LocalPlayer.LP.CurrentGold += unit.GetCost();

        Destroy(obj);
    }
}
