using OpenCover.Framework.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LocalPlayer : Player
{
    [SerializeField] GameObject canvas;
    GraphicRaycaster gr;
    PointerEventData ped;
    EventSystem es;
    [SerializeField] SynergyExplain synergyExplain;
    [SerializeField] RerollController rc;

    static LocalPlayer localPlayer = null;
    public static LocalPlayer LP { get { return localPlayer; } set { localPlayer = value; } }

    [Header("�� ���� �̵� ���� ���")]
    Tile tmpTile; // Ÿ���� ���� ������ �����ϱ� ���� ����
    Tile.TileTypeEnum tt; // Ÿ�� Ÿ���� �����ϱ� ���� ����
    GameObject tmpGameObject; // ���� ������Ʈ�� ���� �����ϱ� ���� ����
    float tmpGameObject_Ypos; // ���� ����� ���� ������Ʈ�� y �� ��ġ ���� �����ϱ� ���� ����
    int originUnitPos_x, originUnitPos_y; // ���� ������Ʈ�� "Ÿ�Ͽ�����" ���� ��ġ�� �����ϱ� ���� ����

    [SerializeField] UnitCountLimit uc;

    public override bool IsLocalPlayer { get => base.IsLocalPlayer; set => base.IsLocalPlayer = true; }

    void Awake()
    {
        if (!localPlayer)
        {
            LP = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected override void Start()
    {
        base.Start();
        playerNum = 0;

        gr = canvas.GetComponent<GraphicRaycaster>();
        es = GetComponent<EventSystem>();
    }

    protected override void Update()
    {
        base.Update();
        MoveUnit();
        ShowSynergyExplain();
    }

    public GameObject GetWhatUnitIs(int i)
    {
        return unitArr[i];
    }

    public override GameObject Summon(int n)
    {
        return base.Summon(n);
    }

    public GameObject GetUnitArr(int i)
    {
        return unitArr[i];
    }

    protected override int SetUnitCountLimit()
    {
        switch (Level)
        {
            case 1:
                LimitUnitCount = 1;
                uc.ShowUnitCountText();
                break;
            case 2:
                LimitUnitCount = 2;
                uc.ShowUnitCountText();
                break;
            case 3:
                LimitUnitCount = 3;
                uc.ShowUnitCountText();
                break;
            case 4:
                LimitUnitCount = 4;
                uc.ShowUnitCountText();
                break;
            case 5:
                LimitUnitCount = 5;
                uc.ShowUnitCountText();
                break;
            case 6:
                LimitUnitCount = 6;
                uc.ShowUnitCountText();
                break;
        }

        return LimitUnitCount;
    }

    public override void SetUnitCount()
    {
        UnitCount = 0;

        for (int x = 0; x < BattleTileArray.GetLength(0); x++)
        {
            for (int y = 0; y < BattleTileArray.GetLength(1); y++)
            {
                if (BattleTileArray[x, y])
                {
                    UnitCount++;
                }
            }
        }

        uc.ShowUnitCountText();
    }

    protected override void MoveUnit()
    {
        bool isWait = GameManager.Instance.IsWaitTime;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        int layerMask = 1 << LayerMask.NameToLayer("Tile") | 1 << LayerMask.NameToLayer("Stage");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) // ray�� �Է°��� ������
        {
            if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư�� ������ ��
            {
                if (isWait) // �غ� �ð��̰�
                {
                    if (hit.collider.CompareTag("Tile")) // �ݶ��̴��� �±װ� Tile�̸�
                    {
                        Tile tile = hit.collider.GetComponent<Tile>(); // �� Ÿ���� ������ ���� ������ �����Ѵ�.

                        if (tile.LocalPlayer) // ���� �� Ÿ���� LocalPlayer(�÷��̾�)�� ���̶��
                        {
                            tmpTile = tile; // tmpTile�� tile�� �����س��´�.
                            
                            // Ÿ�Ͽ����� x, y ��ġ ���� ������ ���´�.
                            originUnitPos_x = tmpTile.TileWidth; 
                            originUnitPos_y = tmpTile.TileHeight;

                            if (tmpTile.tileType == Tile.TileTypeEnum.BattleTile) // ���� tmpTile�� Ÿ�� Ÿ���� ��Ʋ Ÿ���̰�
                            {
                                if (BattleTileArray[originUnitPos_x, originUnitPos_y]) // �� ��Ʋ Ÿ�Ͽ� ������ �����Ѵٸ�
                                {
                                    // �� ���� ������Ʈ�� �ٸ� ���� �����س��´�.
                                    tmpGameObject = BattleTileArray[originUnitPos_x, originUnitPos_y].gameObject;

                                    // ���� ������Ʈ�� y���� �����Ͽ� Ÿ�Ͽ� �Ĺ����ų� ���ϰ� ���� �ʰ� �ϱ� ����.
                                    tmpGameObject_Ypos = tmpGameObject.transform.position.y;
                                    tt = Tile.TileTypeEnum.BattleTile; // Ÿ���� Ÿ���� ������ ���´�.
                                }
                            }
                            else if (tmpTile.tileType == Tile.TileTypeEnum.ReadyTile) // ���� tmpTile�� Ÿ�� Ÿ���� ���� Ÿ���̰�
                            {
                                if (ReadyQue[originUnitPos_x, originUnitPos_y]) // �� ���� Ÿ�Ͽ� ������ �����Ѵٸ�
                                {
                                    // �� ���� ������Ʈ�� �ٸ� ���� ������ ���´�.
                                    tmpGameObject = ReadyQue[originUnitPos_x, originUnitPos_y].gameObject;

                                    // ���� ������Ʈ�� y���� �����Ͽ� Ÿ�Ͽ� �Ĺ����ų� ���ϰ� ���� �ʰ� �ϱ� ����.
                                    tmpGameObject_Ypos = tmpGameObject.transform.position.y;
                                    tt = Tile.TileTypeEnum.ReadyTile; // Ÿ���� Ÿ���� ������ ���´�.
                                }
                            }
                        }
                    }
                }
                else // �غ� �ð��� �ƴ϶��
                {
                    if (hit.collider.tag == "Tile") // �ݶ��̴��� �±װ� Ÿ���̸�
                    {
                        Tile tile = hit.collider.GetComponent<Tile>(); // �� Ÿ���� ������ ���� ������ �����Ѵ�.

                        if (tile.LocalPlayer) // ���� �� Ÿ���� ���� �÷��̾��� ���̶��
                        {
                            tmpTile = tile; // tmpTile�� tile�� �����س��´�.

                            // Ÿ�Ͽ����� x, y ��ġ ���� ������ ���´�.
                            originUnitPos_x = tmpTile.TileWidth;
                            originUnitPos_y = tmpTile.TileHeight;

                            if (tile.tileType != Tile.TileTypeEnum.BattleTile) // ���� Ÿ�� Ÿ���� ��ƲŸ���� �ƴ� ��츸
                            {
                                if (ReadyQue[originUnitPos_x, originUnitPos_y]) // �� Ÿ���� ������ �����Ѵٸ�
                                {
                                    // �� ���� ������Ʈ�� �ٸ� ���� �����س��´�.
                                    tmpGameObject = ReadyQue[originUnitPos_x, originUnitPos_y].gameObject;
                                }
                            }
                            // ���� �߿� ��Ʋ Ÿ���� ������ �̵��� ���� ����.
                        }
                    }
                }
            }
            else if (Input.GetMouseButton(0)) // ���콺�� ������ �ִ� �����϶�
            {
                if (tmpGameObject) // ���� tmpGameObject�� ������ ����ִٸ�
                {
                    // �ٸ� ���ֵ�� ������ ���˿� ���� �ǵ����� ���� ��Ȳ�� �������� ���� ���� ���� 
                    // RigidBody ���� �޾ƿͼ� ������ ������ ������ �Ŀ�
                    Rigidbody rigid = tmpGameObject.GetComponent<Rigidbody>();
                    if (rigid.useGravity == true) { rigid.useGravity = false; }
                    if (rigid.isKinematic == true) { rigid.isKinematic = false; }

                    // �� ������ ���콺�� ���� �ٴϵ��� ������ش�.
                    tmpGameObject.transform.position = new Vector3(hit.point.x, hit.point.y + 1.0f, hit.point.z);
                }
            }
            else if (Input.GetMouseButtonUp(0)) // ���������� ���콺�� ���� ��
            {
                ped = new PointerEventData(es);
                ped.position = Input.mousePosition;

                List<RaycastResult> results = new List<RaycastResult>();
                gr.Raycast(ped, results);
                
                GameObject go = null;

                if (results.Count > 0)
                {
                    go = results[0].gameObject;
                }

                if (tmpGameObject) // ���� tmpGmaeObject�� ������ ����ִٸ�
                {
                    if (go)
                    {
                        if (go.CompareTag("Sell"))
                        {
                            SellController sc = go.GetComponent<SellController>();

                            if (tt == Tile.TileTypeEnum.ReadyTile)
                            {
                                ReadyQue[originUnitPos_x, originUnitPos_y] = null;
                            }
                            else if(tt == Tile.TileTypeEnum.BattleTile) 
                            {
                                BattleTileArray[originUnitPos_x, originUnitPos_y] = null;
                                SynergyEffect();
                            }

                            sc.SellUnit(tmpGameObject);
                            return;
                        }
                    }

                    // �� ������ Rigidbody�� Unit ������Ʈ�� ������ ����
                    Rigidbody rigid = tmpGameObject.GetComponent<Rigidbody>();
                    Unit tmpUnit1 = tmpGameObject.GetComponent<Unit>();

                    // �ٽ� ������ ������ �߰������� �Ŀ�
                    if (rigid.useGravity == false) { rigid.useGravity = true; }
                    if (rigid.isKinematic == false) { rigid.isKinematic = true; }

                    if(hit.collider.tag == "Tile") // ray�� �±װ� Ÿ���̸�
                    {
                        Tile tile = hit.collider.GetComponent<Tile>(); // �� Ÿ���� ������ ������ �����Ѵ�.
                        //Debug.Log(tile.LocalPlayer);
                        if (tile.LocalPlayer) // �� Ÿ���� �÷��̾��� Ÿ���̰�
                        {
                            if (isWait) // ���� �ð��� �غ� �ð��� ��
                            {
                                int x = tile.TileWidth, y = tile.TileHeight; // Ÿ���� ������ x, y ��ȣ�� ������ �� �� 

                                if (tile.tileType != Tile.TileTypeEnum.BattleTile) // ����� Ÿ�� Ÿ���� ��ƲŸ���� �ƴ� ��
                                {
                                    if (!ReadyQue[x, y]) // ����ť�� ������ ����
                                    {
                                        if (tt == Tile.TileTypeEnum.ReadyTile) // ���콺 Ŭ������ ���� Ÿ���� Ÿ���� ����Ÿ���̾��ٸ�
                                        {
                                            ReadyQue[x, y] = tmpUnit1; // ������� ReadyQue[x, y]�� tmpGameObject�� �����Ѵ�.
                                            ReadyQue[originUnitPos_x, originUnitPos_y] = null; // tmpGameObject�� ���� �־��� �ڸ��� ����ش�.
                                            tmpUnit1.unitPosition = new Vector2Int(x, y); // tmpGameObject�� Ÿ�Ͽ����� ��ġ���� x, y�� �ٲ��ش�.
                                            tmpUnit1.IsReady = true; // ����ť�� ���������� �غ� ���·� �ٲ��ش�.

                                            // Ÿ���� x, z �����ǿ� ������ Ű�� ������ ��ġ ������ �̵����� Ÿ���� ���߾ӿ� ��ġ�ϰ� �Ѵ�.
                                            tmpGameObject.transform.position = new Vector3(tile.transform.position.x, tmpGameObject_Ypos, tile.transform.position.z);
                                        }
                                        else if (tt == Tile.TileTypeEnum.BattleTile) // ���� ���콺�� Ŭ������ ���� Ÿ���� Ÿ���� ��ƲŸ���̾��ٸ�
                                        {
                                            // tmpGameObject�� ���� ��ġ�� Ÿ���� ��ƲŸ���ΰ� �����ϸ�, ������ ���� ���� �����ϴ�.
                                            ReadyQue[x, y] = tmpGameObject.GetComponent<Unit>(); 
                                            BattleTileArray[originUnitPos_x, originUnitPos_y] = null; 
                                            tmpUnit1.unitPosition = new Vector2Int(x, y);
                                            tmpUnit1.IsReady = true;
                                            tmpGameObject.transform.position = new Vector3(tile.transform.position.x, tmpGameObject_Ypos, tile.transform.position.z);
                                        }
                                    }
                                    else if (ReadyQue[x, y]) // ���� ����ť�� ������ ���� ���
                                    {
                                        // �� ������ GameObject�� Unit ������Ʈ, ������ ���� ��ġ ���� �ٸ� ������ ������ �صд�.
                                        Unit tmpUnit2 = ReadyQue[x, y];
                                        GameObject tmpObj = tmpUnit2.gameObject;
                                        int tmpx = tmpUnit1.unitPosition.x, tmpy = tmpUnit1.unitPosition.y;

                                        if (tt == Tile.TileTypeEnum.ReadyTile) // ���콺 Ŭ������ ���� Ÿ���� Ÿ���� ����Ÿ���̾��ٸ�
                                        {
                                            ReadyQue[x, y] = tmpUnit1; // ����ť�� tmpGameObject�� �����ϰ�
                                            ReadyQue[originUnitPos_x, originUnitPos_y] = tmpUnit2; // tmpGameObject�� �ִ� �ڸ����� tmpUnit2�� �����Ѵ�.

                                            // ��ġ�� �ٲ� ���ֵ� ���� Ÿ�Ͽ����� ��ġ �� ��ǥ ���� �ٲ��ش�.
                                            tmpUnit1.unitPosition = new Vector2Int(x, y);
                                            tmpUnit2.unitPosition = new Vector2Int(tmpx, tmpy);
                                            tmpGameObject.transform.position = new Vector3(tile.transform.position.x, tmpGameObject_Ypos, tile.transform.position.z);
                                            tmpObj.transform.position = new Vector3(tmpTile.transform.position.x, tmpObj.transform.position.y, tmpTile.transform.position.z);

                                            // �� ���� ���� ����ť���� �ٲ�������� �غ� ���·� �������ش�.
                                            tmpUnit1.IsReady = true;
                                            tmpUnit2.IsReady = true;
                                        }
                                        else if (tt == Tile.TileTypeEnum.BattleTile) // ���콺 Ŭ������ ���� Ÿ���� Ÿ���� ��ƲŸ���̾��ٸ�
                                        {
                                            // �غ� ���� �� ������ �ִ� Ÿ���� �����ϰ�, ������ ��ġ�� �ٲٴ� ����� �����ϴ�.
                                            ReadyQue[x, y] = tmpUnit1;
                                            BattleTileArray[originUnitPos_x, originUnitPos_y] = tmpUnit2;

                                            tmpUnit1.unitPosition = new Vector2Int(x, y);
                                            tmpUnit2.unitPosition = new Vector2Int(tmpx, tmpy);

                                            tmpGameObject.transform.position = new Vector3(tile.transform.position.x, tmpGameObject_Ypos, tile.transform.position.z);
                                            tmpObj.transform.position = new Vector3(tmpTile.transform.position.x, tmpObj.transform.position.y, tmpTile.transform.position.z);

                                            tmpUnit1.IsReady = true;
                                            tmpUnit2.IsReady = false;
                                        }
                                    }
                                }
                                else if (tile.tileType == Tile.TileTypeEnum.BattleTile) // ����� Ÿ�� Ÿ���� ��ƲŸ���� ��
                                {
                                    if (!BattleTileArray[x, y]) // �� ��Ʋ Ÿ�� ���� ������ ����
                                    {
                                        if (tt == Tile.TileTypeEnum.BattleTile) // ���� ������ �ִ� Ÿ���� ��Ʋ Ÿ���̶��
                                        {
                                            BattleTileArray[x, y] = tmpUnit1; // �� ��Ʋ Ÿ�Ͽ� tmpGameObject�� ����(�̵�)�ϰ�
                                            BattleTileArray[originUnitPos_x, originUnitPos_y] = null; // ������ �ִ� Ÿ���� ����� ����
                                            tmpUnit1.unitPosition = new Vector2Int(x, y);// �� ������ Ÿ�Ͽ����� ��ġ �� ��ǥ�� �ٲ��� ����
                                            tmpGameObject.transform.position = new Vector3(tile.transform.position.x, tmpGameObject_Ypos, tile.transform.position.z);
                                            tmpUnit1.IsReady = false; // �غ� ���¸� �����Ѵ�.
                                        }
                                        else if (tt == Tile.TileTypeEnum.ReadyTile) // ���� ������ �ִ� Ÿ���� ���� Ÿ���̶��
                                        {
                                            if (UnitCount < LimitUnitCount) // �÷��̾��� ��Ʋ Ÿ�Ͽ� �ִ� ���� ���� ���� ���ּ� ���� ���� ��쿡��
                                            {
                                                // ������ ��ġ�� �ٲ��ش�.
                                                BattleTileArray[x, y] = tmpUnit1;
                                                ReadyQue[originUnitPos_x, originUnitPos_y] = null;
                                                tmpUnit1.unitPosition = new Vector2Int(x, y);
                                                tmpUnit1.IsReady = false;
                                                tmpGameObject.transform.position = new Vector3(tile.transform.position.x, tmpGameObject_Ypos, tile.transform.position.z);
                                            }
                                            else
                                            {
                                                tmpGameObject.transform.position = new Vector3(tmpTile.transform.position.x, tmpGameObject_Ypos, tmpTile.transform.position.z);
                                            }
                                        }
                                        else // Ÿ���� Ÿ���� ��Ʋ Ÿ�Ե� ���� Ÿ�ϵ� �ƴ� ��� ���콺�� �����鼭 �������� ������ ���� �ִ� �ڸ��� �ǵ�����.
                                        {
                                            tmpGameObject.transform.position = new Vector3(tmpTile.transform.position.x, tmpGameObject_Ypos, tmpTile.transform.position.z);
                                        }
                                    }
                                    else if(BattleTileArray[x, y]) // �� ��Ʋ Ÿ�� ���� ������ �ִٸ�
                                    {
                                        // ��Ʋ Ÿ�� ���� �ִ� GameObject�� Unit, GameObject ������Ʈ, Ÿ�Ͽ����� ��ġ ���� �����Ѵ�.
                                        Unit tmpUnit2 = BattleTileArray[x, y];
                                        GameObject tmpObj = tmpUnit2.gameObject;
                                        int tmpx = tmpUnit1.unitPosition.x, tmpy = tmpUnit1.unitPosition.y;

                                        if (tt == Tile.TileTypeEnum.BattleTile) // ���콺�� Ŭ������ ���� Ÿ�� Ÿ���� ��Ʋ Ÿ���̶��
                                        {
                                            // Ÿ�Ͽ����� ��ġ �� ��ǥ�� �����Ѵ�.
                                            BattleTileArray[x, y] = tmpUnit1;
                                            BattleTileArray[originUnitPos_x, originUnitPos_y] = tmpUnit2;
                                            tmpUnit1.unitPosition = new Vector2Int(x, y);
                                            tmpUnit2.unitPosition = new Vector2Int(tmpx, tmpy);
                                            tmpGameObject.transform.position = new Vector3(tile.transform.position.x, tmpGameObject_Ypos, tile.transform.position.z);
                                            tmpObj.transform.position = new Vector3(tmpTile.transform.position.x, tmpObj.transform.position.y, tmpTile.transform.position.z);

                                            // ��Ʋ Ÿ�Ͽ��� ���� ��ġ�� �ٲ� ������ ��� �غ� ���� �������ش�.
                                            tmpUnit1.IsReady = false;
                                            tmpUnit2.IsReady = false;
                                        }
                                        else if (tt == Tile.TileTypeEnum.ReadyTile) // ���콺�� Ŭ������ ���� Ÿ�� Ÿ���� �غ� Ÿ���̶��
                                        {
                                            // ���ΰ� �ִ� Ÿ�ϰ� �غ� ���¸� �����ϸ�, ���� ������ �����ϴ�.
                                            BattleTileArray[x, y] = tmpUnit1;
                                            ReadyQue[originUnitPos_x, originUnitPos_y] = tmpUnit2;

                                            tmpUnit1.unitPosition = new Vector2Int(x, y);
                                            tmpUnit2.unitPosition = new Vector2Int(tmpx, tmpy);

                                            tmpGameObject.transform.position = new Vector3(tile.transform.position.x, tmpGameObject_Ypos, tile.transform.position.z);
                                            tmpObj.transform.position = new Vector3(tmpTile.transform.position.x, tmpObj.transform.position.y, tmpTile.transform.position.z);

                                            tmpUnit1.IsReady = false;
                                            tmpUnit2.IsReady = true;
                                        }
                                    }
                                }
                                SynergyEffect();
                            }
                            else if(!isWait) // ���� �ð��� ���� �ð��̶��
                            {
                                int x = tile.TileWidth, y = tile.TileHeight; // �� Ÿ���� ��ȣ x, y�� ���� �����ϰ�

                                if (tile.tileType != Tile.TileTypeEnum.BattleTile) // �� Ÿ���� Ÿ���� ��Ʋ Ÿ���� �ƴ� ��
                                {
                                    if (!ReadyQue[x, y]) // ����ť�� �ƹ��͵� ���ٸ�
                                    {
                                        if (tt == Tile.TileTypeEnum.ReadyTile) // ���콺 Ŭ������ ���� Ÿ���� Ÿ���� ����Ÿ���� ��쿡��
                                        {
                                            // ������ ��ġ�� �ٲ��ش�.
                                            ReadyQue[x, y] = tmpGameObject.GetComponent<Unit>();
                                            ReadyQue[originUnitPos_x, originUnitPos_y] = null;
                                            tmpUnit1.unitPosition = new Vector2Int(x, y);
                                            tmpUnit1.IsReady = true;
                                            tmpGameObject.transform.position = new Vector3(tile.transform.position.x, tmpGameObject_Ypos, tile.transform.position.z);
                                        }
                                    }
                                    else if (ReadyQue[x, y]) // ���� ����ť�� ������ �ִٸ�
                                    {
                                        // �� ������ Unit, GameObject ������Ʈ�� ������ ��
                                        Unit tmpUnit2 = ReadyQue[x, y];
                                        GameObject tmpObj = tmpUnit2.gameObject;
                                        // tmp1�� Ÿ�Ͽ����� ��ġ x, y ���� �����Ѵ�.
                                        int tmpx = tmpUnit1.unitPosition.x, tmpy = tmpUnit1.unitPosition.y;

                                        if (tt == Tile.TileTypeEnum.ReadyTile) // ���콺 Ŭ������ ���� Ÿ���� Ÿ���� ����Ÿ���� ��쿡��
                                        {
                                            // ������ ��ġ(Ÿ�� �� ��ǥ)�� �ٲ��ְ� �غ� ���·� �����Ѵ�.
                                            ReadyQue[x, y] = tmpUnit1;
                                            ReadyQue[originUnitPos_x, originUnitPos_y] = tmpUnit2;

                                            tmpUnit1.unitPosition = new Vector2Int(x, y);
                                            tmpUnit2.unitPosition = new Vector2Int(tmpx, tmpy);

                                            tmpGameObject.transform.position = new Vector3(tile.transform.position.x, tmpGameObject_Ypos, tile.transform.position.z);
                                            tmpObj.transform.position = new Vector3(tmpTile.transform.position.x, tmpObj.transform.position.y, tmpTile.transform.position.z);

                                            tmpUnit1.IsReady = true;
                                            tmpUnit2.IsReady = true;
                                        }
                                    }
                                }
                                else // Ÿ���� Ÿ���� ��Ʋ Ÿ�Ե� ���� Ÿ�ϵ� �ƴ� ��� ���콺�� �����鼭 �������� ������ ���� �ִ� �ڸ��� �ǵ�����.
                                {
                                    tmpGameObject.transform.position = new Vector3(tmpTile.transform.position.x, tmpGameObject_Ypos, tmpTile.transform.position.z);
                                }
                            }
                        }
                        else
                        {
                            tmpGameObject.transform.position = new Vector3(tmpTile.transform.position.x, tmpGameObject_Ypos, tmpTile.transform.position.z);
                        }
                    }
                    else // ray�� �±װ� Ÿ���� �ƴϸ�(������ ������ ���콺�� ���� ���) ���콺�� �����鼭 �������� ������ ���� �ִ� �ڸ��� �ǵ�����.
                    {
                        tmpGameObject.transform.position = new Vector3(tmpTile.transform.position.x, tmpGameObject_Ypos, tmpTile.transform.position.z);
                    }
                }
                
                // ���� �̵��߿� ����ߴ� �������� �ʱ�ȭ�� ��
                tmpGameObject = null;
                tmpTile = null;

                // ��Ʋ Ÿ�Ͽ� �ִ� ������ ���ڸ� ����.
                SetUnitCount();
            }
        }
    }

    public override void SetUnitObject(GameObject obj, int i)
    {
        base.SetUnitObject(obj, i);
        rc.SetActive();
    }

    void ShowSynergyExplain()
    {
        ped = new PointerEventData(es);
        ped.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);

        //foreach(var r in results)
        //{
        //    Debug.Log(r.ToString());
        //}

        GameObject go = null;

        if (results.Count > 0)
        {
            go = results[0].gameObject;
        }

        if (go)
        {
            if(go.transform.parent.CompareTag("Synergy"))
            {
                synergyExplain.gameObject.SetActive(false);
                synergyExplain.gameObject.SetActive(true);
                synergyExplain.transform.position = Input.mousePosition;
                synergyExplain.ShowSynergyEffect(go.transform.parent.name);
            }
        }
        else
        {
            synergyExplain.gameObject.SetActive(false);
        }
    }
}

