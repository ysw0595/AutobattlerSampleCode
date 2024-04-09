using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System;
using static GameManager;
using UnityEditor;
using System.Data;

public class TileController : MonoBehaviour/*, IFindNullIndex, IIsEmpty, IIsExist, ISetGameObject*/
{
    public static List<TileController> tileControllers = new List<TileController>();

    int BT_X = Player.battleTileX, BT_Y = Player.battleTileY;
    int QT_X = Player.queTileX, QT_Y = Player.queTileY;
    Tile[,] BattleTile;
    Tile[,] ReadyTile;
    [SerializeField] GameObject battleTilePrefab;
    [SerializeField] GameObject queTilePrefab;

    [SerializeField] int createDirection = 1;

    public Player currentPlayer;

    // Start is called before the first frame update
    void Start()
    {
        // 전투 타일 및 준비 타일을 생성하고 각 타일을 통제하는 tileControllers 리스트에 저장함
        CreateTile(transform.position + (Vector3.up * 0.001f), battleTilePrefab, BT_Y, BT_X, new Vector2(0.625f, -0.6f) * createDirection);
        CreateTile(transform.position + (Vector3.back * 1.95f * createDirection), queTilePrefab, QT_Y, QT_X, new Vector2(0.55f, 0) * createDirection);
        tileControllers.Add(this);
    }

    private void OnDestroy()
    {
        tileControllers.Remove(this);
    }

    public static void ClaimDispose()
    {
        // tileControllers 리스트의 모든 tileController마다 DisposeUnits를 실행하여 초기화
        foreach (TileController tileController in tileControllers)
        {
            tileController.DisposeUnits();
        }
    }

    void CreateTile(Vector3 offset, GameObject wantTile, int height, int width, Vector2 gap)
    {
        // 가장 처음 플레이어들의 전투 타일과, 준비 타일을 생성해주는 메서드 한 번만 실행
        Tile[,] unitTile = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject ut = Instantiate(wantTile);
                Tile tile = ut.GetComponent<Tile>();
                if (createDirection == 1) { tile.LocalPlayer = true; }
                ut.transform.position = new Vector3(offset.x + (x * gap.x), offset.y, offset.z + (y * gap.y));
                ut.transform.SetParent(transform, true);
                unitTile[x, y] = tile;
                tile.TileWidth = x;
                tile.TileHeight = y;

                if (wantTile == battleTilePrefab) tile.tileType = Tile.TileTypeEnum.BattleTile;
                else tile.tileType = Tile.TileTypeEnum.ReadyTile;
            }
        }

        if (wantTile == battleTilePrefab)
        {
            BattleTile = unitTile;
        }
        else
        {
            ReadyTile = unitTile;
        }
    }

    void DisposeUnits()
    {
        // 유닛 획득(구매 및 효과 등)시 실행되는 메서드
        // 유닛을 획득하면 기본적으로 ReadyQue에 들어옴으로 readyQue의 빈 칸 가장 앞에 획득 유닛 배치
        Unit[,] unitReadyArr = currentPlayer.GetReadyQue();
        Unit[,] unitBattleArr = currentPlayer.GetBattleQue();

        for (int x = 0; x < unitReadyArr.GetLength(0); x++)
        {
            for (int y = 0; y < unitReadyArr.GetLength(1); y++)
            {
                if (unitReadyArr[x, y])
                {
                    Unit unit = unitReadyArr[x, y];
                    //Debug.Log(unitReadyArr[x, y]);

                    if (unit.IsReady)
                    {
                        Transform trs = ReadyTile[unit.unitPosition.x, unit.unitPosition.y].transform;
                        unit.transform.position = new Vector3(trs.position.x, unit.transform.position.y, trs.position.z);
                    }
                }
            }
        }

        if (GameManager.Instance.IsWaitTime)
        {
            for (int x = 0; x < unitBattleArr.GetLength(0); x++)
            {
                for (int y = 0; y < unitBattleArr.GetLength(1); y++)
                {
                    if (unitBattleArr[x, y])
                    {
                        Unit unit = unitBattleArr[x, y];
                        //Debug.Log(unitReadyArr[x, y]);

                        if (!unit.IsReady)
                        {
                            Transform trs = BattleTile[unit.unitPosition.x, unit.unitPosition.y].transform;
                            unit.transform.position = new Vector3(trs.position.x, unit.transform.position.y, trs.position.z);
                        }
                    }
                }
            }
        }
    }
}