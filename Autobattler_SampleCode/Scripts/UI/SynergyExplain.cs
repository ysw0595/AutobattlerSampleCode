using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SynergyExplain : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] RawImage ri;

    public void ShowSynergyEffect(string str)
    {
        switch (str)
        {
            case "Melee":
                text.text = "  배틀 타일 위 서로 다른 근접 유닛 >= 3,\n\n모든 근접 유닛 공격력 +3";
                break;
            case "Range":
                text.text = "  배틀 타일 위 서로 다른 원거리 유닛 >= 3,\n\n모든 원거리 유닛 사거리 +0.3";
                break;
            case "Speed":
                text.text = "  배틀 타일 위 기병 유닛 >= 1,\n\n모든 기병 유닛 공격력 +3,\n최대 체력 + 10";
                break;
            case "Ancient":
                text.text = "  배틀 타일 위 서로 다른 고대시대 유닛 >= 3,\n\n모든 고대시대 유닛 공격력 +3,\n사거리 +0.2\n최대 체력 +5";
                break;
            case "Classic":
                text.text = "  배틀 타일 위 서로 다른 고전시대 유닛 >= 3,\n\n모든 고전시대 유닛 공격력 +5,\n사거리 +0.3\n최대 체력 +10";
                break;
            default:
                break;
        }

        ri.rectTransform.sizeDelta = new Vector2(text.rectTransform.rect.width, text.rectTransform.rect.height);
    }
}
