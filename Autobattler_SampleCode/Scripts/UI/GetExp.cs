using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetExp : MonoBehaviour
{
    [SerializeField] int costForGetExp;
    public int CostForGetExp { get { return costForGetExp; } set { costForGetExp = value; } }

    public void BuyExp()
    {
        LocalPlayer.LP.DecideBuyingExp(CostForGetExp);
    }
}
