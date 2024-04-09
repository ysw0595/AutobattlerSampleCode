using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowAge : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI showAge;

    public void ShowEra()
    {
        showAge.text = GameManager.Instance.Age.ToString();
    }
}
