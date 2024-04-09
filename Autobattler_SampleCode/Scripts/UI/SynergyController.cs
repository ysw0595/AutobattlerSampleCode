using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;

public class SynergyController : MonoBehaviour
{
    [SerializeField] GameObject[] typeSynergy;
    [SerializeField] GameObject[] eraSynergy;
    [SerializeField] TextMeshProUGUI count;

    public void NoUnitInBattleArr()
    {
        foreach(GameObject type in typeSynergy)
        {
            type.SetActive(false);
        }
        foreach(GameObject type in eraSynergy)
        {
            type.SetActive(false);
        }
    }

    public void AbleTypeSynergy(int a, int b, Player owner)
    {
        if (owner == LocalPlayer.LP)
        {
            if (b >= 1)
            {
                typeSynergy[a].SetActive(true);
                ApplyTypeSynergy(a, b);
            }
            else if (b < 1)
            {
                typeSynergy[a].SetActive(false);
            }
        }
    }

    public void AbleEraSynergy(int a, int b, Player owner)
    {
        if (owner == LocalPlayer.LP)
        {
            if (b >= 1)
            {
                eraSynergy[a].SetActive(true);
                count = eraSynergy[a].GetComponentInChildren<TextMeshProUGUI>();
                count.text = $"{b} / 3";
            }
            else if (b < 1)
            {
                eraSynergy[a].SetActive(false);
            }
        }
    }

    void ApplyTypeSynergy(int a, int b)
    {
        switch (a)
        {
            case 0:
                count = typeSynergy[a].GetComponentInChildren<TextMeshProUGUI>();
                count.text = $"{b} / 3";
                break;
            case 1:
                count = typeSynergy[a].GetComponentInChildren<TextMeshProUGUI>();
                count.text = $"{b} / 3";
                break;
            case 2:
                count = typeSynergy[a].GetComponentInChildren<TextMeshProUGUI>();
                count.text = $"{b} / 1";
                break;
            default:
                break;
        }
    }
    
    void ApplyEraSynergy(int a, int b)
    {
        switch (a)
        {
            case 0:
                count = eraSynergy[a].GetComponentInChildren<TextMeshProUGUI>();
                if (b >= 3)
                {
                    count.text = $"{b} / 6";
                }
                count.text = $"{b} / 3";
                break;
            case 1:
                count = eraSynergy[a].GetComponentInChildren<TextMeshProUGUI>();
                if (b >= 3)
                {
                    count.text = $"{b} / 6";
                }
                count.text = $"{b} / 3";
                break;
        }
    }
}

