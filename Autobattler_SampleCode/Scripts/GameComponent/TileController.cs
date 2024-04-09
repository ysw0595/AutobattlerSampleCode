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
        // ���� Ÿ�� �� �غ� Ÿ���� �����ϰ� �� Ÿ���� �����ϴ� tileControllers ����Ʈ�� ������
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
        // tileControllers ����Ʈ�� ��� tileController���� DisposeUnits�� �����Ͽ� �ʱ�ȭ
        foreach (TileController tileController in tileControllers)
        {
            tileController.DisposeUnits();
        }
    }

    void CreateTile(Vector3 offset, GameObject wantTile, int height, int width, Vector2 gap)
    {
        // ���� ó�� �÷��̾���� ���� Ÿ�ϰ�, �غ� Ÿ���� �������ִ� �޼��� �� ���� ����
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
        // ���� ȹ��(���� �� ȿ�� ��)�� ����Ǵ� �޼���
        // ������ ȹ���ϸ� �⺻������ ReadyQue�� �������� readyQue�� �� ĭ ���� �տ� ȹ�� ���� ��ġ
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