using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class RerollController : MonoBehaviour
{
    [SerializeField] GameObject[] unitInfo;
    
    public static int rerollCost = 2;
    int noRerollCost = 0;

    public void Reroll()
    {
        SetActive();
        LocalPlayer.LP.DecideReroll(rerollCost);
    }

    public void RerollWithoutCost()
    {
        SetActive();
        LocalPlayer.LP.DecideReroll(noRerollCost);
    }

    public void SetActive()
    {
        for(int i = 0; i < unitInfo.Length; i++)
        {
            unitInfo[i].SetActive(true);
        }
    }
}
