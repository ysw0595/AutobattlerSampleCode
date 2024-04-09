using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.AI;

public class UnitInfo : MonoBehaviour
{
    Unit unit;
    [SerializeField] int num;

    [Header("бс unitInfo")]
    [SerializeField] Image unitImage;
    [SerializeField] Text unitGold;
    [SerializeField] TextMeshProUGUI unitName;
    [SerializeField] TextMeshProUGUI unitClass;

    private void Update()
    {
        SetUnitInfo();
    }

    void ShowUnit(Unit unit)
    {
        unitImage.sprite = unit.GetSprite();
        unitName.text = unit.name;
        unitGold.text = unit.GetGold().ToString();
        unitClass.text = unit.GetEraClass();
    }

    public void SetUnitInfo()
    {
        unit = LocalPlayer.LP.GetUnitArr(num).GetComponent<Unit>();

        ShowUnit(unit);
    }

    public void CallSummon()
    {
        GameObject obj = LocalPlayer.LP.Summon(num);

        if (obj)
        {
            gameObject.SetActive(false);
        }
    }
}
