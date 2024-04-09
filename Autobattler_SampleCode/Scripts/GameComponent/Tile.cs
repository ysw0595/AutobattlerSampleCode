using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileTypeEnum
    {
        ReadyTile = 0,
        BattleTile
    }

    public TileTypeEnum tileType;

    [SerializeField] TileController Tc;
    
    [SerializeField] int tileWidth;
    public int TileWidth { get => tileWidth; set => tileWidth = value; }
    [SerializeField] int tileHeight;
    public int TileHeight { get => tileHeight; set => tileHeight = value; }

    [SerializeField] bool localPlayer;

    public bool LocalPlayer { get => localPlayer; set => localPlayer = value; }

    private void Start()
    {
        Tc = gameObject.GetComponentInParent<TileController>();
    }
}
