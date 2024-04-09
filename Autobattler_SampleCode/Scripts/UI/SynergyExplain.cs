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
                text.text = "  ��Ʋ Ÿ�� �� ���� �ٸ� ���� ���� >= 3,\n\n��� ���� ���� ���ݷ� +3";
                break;
            case "Range":
                text.text = "  ��Ʋ Ÿ�� �� ���� �ٸ� ���Ÿ� ���� >= 3,\n\n��� ���Ÿ� ���� ��Ÿ� +0.3";
                break;
            case "Speed":
                text.text = "  ��Ʋ Ÿ�� �� �⺴ ���� >= 1,\n\n��� �⺴ ���� ���ݷ� +3,\n�ִ� ü�� + 10";
                break;
            case "Ancient":
                text.text = "  ��Ʋ Ÿ�� �� ���� �ٸ� ���ô� ���� >= 3,\n\n��� ���ô� ���� ���ݷ� +3,\n��Ÿ� +0.2\n�ִ� ü�� +5";
                break;
            case "Classic":
                text.text = "  ��Ʋ Ÿ�� �� ���� �ٸ� �����ô� ���� >= 3,\n\n��� �����ô� ���� ���ݷ� +5,\n��Ÿ� +0.3\n�ִ� ü�� +10";
                break;
            default:
                break;
        }

        ri.rectTransform.sizeDelta = new Vector2(text.rectTransform.rect.width, text.rectTransform.rect.height);
    }
}
