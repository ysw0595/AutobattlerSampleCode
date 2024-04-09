using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockController : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Sprite[] sprites;

    public void ShopLock()
    {
        LocalPlayer.LP.ShopLock = !LocalPlayer.LP.ShopLock;

        image.sprite = sprites[LocalPlayer.LP.ShopLock ? 0:1];
    }
}
