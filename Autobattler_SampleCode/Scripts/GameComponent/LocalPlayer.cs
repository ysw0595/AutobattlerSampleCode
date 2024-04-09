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

    [Header("■ 유닛 이동 위한 요소")]
    Tile tmpTile; // 타일의 변경 사항을 저장하기 위한 변수
    Tile.TileTypeEnum tt; // 타일 타입을 저장하기 위한 변수
    GameObject tmpGameObject; // 게임 오브젝트를 따로 저장하기 위한 변수
    float tmpGameObject_Ypos; // 따로 저장된 게임 오브젝트의 y 축 위치 값을 저장하기 위한 변수
    int originUnitPos_x, originUnitPos_y; // 게임 오브젝트의 "타일에서의" 본래 위치를 저장하기 위한 변수

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

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) // ray의 입력값이 있으면
        {
            if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼을 눌렀을 때
            {
                if (isWait) // 준비 시간이고
                {
                    if (hit.collider.CompareTag("Tile")) // 콜라이더의 태그가 Tile이면
                    {
                        Tile tile = hit.collider.GetComponent<Tile>(); // 그 타일의 정보를 담은 변수를 생성한다.

                        if (tile.LocalPlayer) // 만약 그 타일이 LocalPlayer(플레이어)의 것이라면
                        {
                            tmpTile = tile; // tmpTile에 tile을 저장해놓는다.
                            
                            // 타일에서의 x, y 위치 값을 저장해 놓는다.
                            originUnitPos_x = tmpTile.TileWidth; 
                            originUnitPos_y = tmpTile.TileHeight;

                            if (tmpTile.tileType == Tile.TileTypeEnum.BattleTile) // 만약 tmpTile의 타일 타입이 배틀 타일이고
                            {
                                if (BattleTileArray[originUnitPos_x, originUnitPos_y]) // 그 배틀 타일에 유닛이 존재한다면
                                {
                                    // 그 게임 오브젝트를 다른 곳에 저장해놓는다.
                                    tmpGameObject = BattleTileArray[originUnitPos_x, originUnitPos_y].gameObject;

                                    // 게임 오브젝트의 y값을 저장하여 타일에 파묻히거나 과하게 뜨지 않게 하기 위함.
                                    tmpGameObject_Ypos = tmpGameObject.transform.position.y;
                                    tt = Tile.TileTypeEnum.BattleTile; // 타일의 타입을 저장해 놓는다.
                                }
                            }
                            else if (tmpTile.tileType == Tile.TileTypeEnum.ReadyTile) // 만약 tmpTile의 타일 타입이 레디 타일이고
                            {
                                if (ReadyQue[originUnitPos_x, originUnitPos_y]) // 그 레디 타일에 유닛이 존재한다면
                                {
                                    // 그 게임 오브젝트를 다른 곳에 저장해 놓는다.
                                    tmpGameObject = ReadyQue[originUnitPos_x, originUnitPos_y].gameObject;

                                    // 게임 오브젝트의 y값을 저장하여 타일에 파묻히거나 과하게 뜨지 않게 하기 위함.
                                    tmpGameObject_Ypos = tmpGameObject.transform.position.y;
                                    tt = Tile.TileTypeEnum.ReadyTile; // 타일의 타입을 저장해 놓는다.
                                }
                            }
                        }
                    }
                }
                else // 준비 시간이 아니라면
                {
                    if (hit.collider.tag == "Tile") // 콜라이더의 태그가 타일이면
                    {
                        Tile tile = hit.collider.GetComponent<Tile>(); // 그 타일의 정보를 담은 변수를 생성한다.

                        if (tile.LocalPlayer) // 만약 그 타일이 로컬 플레이어의 것이라면
                        {
                            tmpTile = tile; // tmpTile에 tile을 저장해놓는다.

                            // 타일에서의 x, y 위치 값을 저장해 놓는다.
                            originUnitPos_x = tmpTile.TileWidth;
                            originUnitPos_y = tmpTile.TileHeight;

                            if (tile.tileType != Tile.TileTypeEnum.BattleTile) // 만약 타일 타입이 배틀타입이 아닐 경우만
                            {
                                if (ReadyQue[originUnitPos_x, originUnitPos_y]) // 그 타일의 유닛이 존재한다면
                                {
                                    // 그 게임 오브젝트를 다른 곳에 저장해놓는다.
                                    tmpGameObject = ReadyQue[originUnitPos_x, originUnitPos_y].gameObject;
                                }
                            }
                            // 전투 중에 배틀 타일의 유닛의 이동을 막기 위함.
                        }
                    }
                }
            }
            else if (Input.GetMouseButton(0)) // 마우스를 누르고 있는 상태일때
            {
                if (tmpGameObject) // 만약 tmpGameObject에 유닛이 들어있다면
                {
                    // 다른 유닛들과 물리적 접촉에 의해 의도하지 않은 상황이 벌어지는 것을 막기 위해 
                    // RigidBody 값을 받아와서 물리적 성질을 차단한 후에
                    Rigidbody rigid = tmpGameObject.GetComponent<Rigidbody>();
                    if (rigid.useGravity == true) { rigid.useGravity = false; }
                    if (rigid.isKinematic == true) { rigid.isKinematic = false; }

                    // 그 유닛이 마우스를 따라 다니도록 만들어준다.
                    tmpGameObject.transform.position = new Vector3(hit.point.x, hit.point.y + 1.0f, hit.point.z);
                }
            }
            else if (Input.GetMouseButtonUp(0)) // 마지막으로 마우스를 뗐을 때
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

                if (tmpGameObject) // 만약 tmpGmaeObject에 유닛이 들어있다면
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

                    // 그 유닛의 Rigidbody와 Unit 컴포넌트를 저장한 다음
                    Rigidbody rigid = tmpGameObject.GetComponent<Rigidbody>();
                    Unit tmpUnit1 = tmpGameObject.GetComponent<Unit>();

                    // 다시 물리적 성질을 추가시켜준 후에
                    if (rigid.useGravity == false) { rigid.useGravity = true; }
                    if (rigid.isKinematic == false) { rigid.isKinematic = true; }

                    if(hit.collider.tag == "Tile") // ray의 태그가 타일이면
                    {
                        Tile tile = hit.collider.GetComponent<Tile>(); // 그 타일을 저장한 변수를 생성한다.
                        //Debug.Log(tile.LocalPlayer);
                        if (tile.LocalPlayer) // 그 타일이 플레이어의 타일이고
                        {
                            if (isWait) // 게임 시간이 준비 시간일 때
                            {
                                int x = tile.TileWidth, y = tile.TileHeight; // 타일의 각각의 x, y 번호를 저장해 준 후 

                                if (tile.tileType != Tile.TileTypeEnum.BattleTile) // 저장된 타일 타입이 배틀타입이 아닐 때
                                {
                                    if (!ReadyQue[x, y]) // 레디큐에 유닛이 없고
                                    {
                                        if (tt == Tile.TileTypeEnum.ReadyTile) // 마우스 클릭했을 때의 타일의 타입이 레디타일이었다면
                                        {
                                            ReadyQue[x, y] = tmpUnit1; // 비어있을 ReadyQue[x, y]에 tmpGameObject를 저장한다.
                                            ReadyQue[originUnitPos_x, originUnitPos_y] = null; // tmpGameObject가 원래 있었던 자리를 비워준다.
                                            tmpUnit1.unitPosition = new Vector2Int(x, y); // tmpGameObject의 타일에서의 위치값도 x, y로 바꿔준다.
                                            tmpUnit1.IsReady = true; // 레디큐로 움직였으니 준비 상태로 바꿔준다.

                                            // 타일의 x, z 포지션에 유닛의 키를 포함한 위치 값으로 이동시켜 타일의 정중앙에 위치하게 한다.
                                            tmpGameObject.transform.position = new Vector3(tile.transform.position.x, tmpGameObject_Ypos, tile.transform.position.z);
                                        }
                                        else if (tt == Tile.TileTypeEnum.BattleTile) // 만약 마우스를 클릭했을 때의 타일의 타입이 배틀타일이었다면
                                        {
                                            // tmpGameObject가 원래 위치한 타일이 배틀타일인걸 제외하면, 과정은 위와 거의 동일하다.
                                            ReadyQue[x, y] = tmpGameObject.GetComponent<Unit>(); 
                                            BattleTileArray[originUnitPos_x, originUnitPos_y] = null; 
                                            tmpUnit1.unitPosition = new Vector2Int(x, y);
                                            tmpUnit1.IsReady = true;
                                            tmpGameObject.transform.position = new Vector3(tile.transform.position.x, tmpGameObject_Ypos, tile.transform.position.z);
                                        }
                                    }
                                    else if (ReadyQue[x, y]) // 만약 레디큐에 유닛이 있을 경우
                                    {
                                        // 그 유닛의 GameObject와 Unit 컴포넌트, 유닛의 기존 위치 또한 다른 공간에 저장을 해둔다.
                                        Unit tmpUnit2 = ReadyQue[x, y];
                                        GameObject tmpObj = tmpUnit2.gameObject;
                                        int tmpx = tmpUnit1.unitPosition.x, tmpy = tmpUnit1.unitPosition.y;

                                        if (tt == Tile.TileTypeEnum.ReadyTile) // 마우스 클릭했을 때의 타일의 타입이 레디타일이었다면
                                        {
                                            ReadyQue[x, y] = tmpUnit1; // 레디큐에 tmpGameObject를 저장하고
                                            ReadyQue[originUnitPos_x, originUnitPos_y] = tmpUnit2; // tmpGameObject가 있던 자리에는 tmpUnit2를 저장한다.

                                            // 위치를 바꾼 유닛들 간의 타일에서의 위치 및 좌표 또한 바꿔준다.
                                            tmpUnit1.unitPosition = new Vector2Int(x, y);
                                            tmpUnit2.unitPosition = new Vector2Int(tmpx, tmpy);
                                            tmpGameObject.transform.position = new Vector3(tile.transform.position.x, tmpGameObject_Ypos, tile.transform.position.z);
                                            tmpObj.transform.position = new Vector3(tmpTile.transform.position.x, tmpObj.transform.position.y, tmpTile.transform.position.z);

                                            // 두 유닛 서로 레디큐에서 바뀌었음으로 준비 상태로 설정해준다.
                                            tmpUnit1.IsReady = true;
                                            tmpUnit2.IsReady = true;
                                        }
                                        else if (tt == Tile.TileTypeEnum.BattleTile) // 마우스 클릭했을 때의 타일의 타입이 배틀타일이었다면
                                        {
                                            // 준비 상태 및 기존에 있던 타일을 제외하고, 유닛의 위치를 바꾸는 방식은 동일하다.
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
                                else if (tile.tileType == Tile.TileTypeEnum.BattleTile) // 저장된 타일 타입이 배틀타입일 때
                                {
                                    if (!BattleTileArray[x, y]) // 그 배틀 타일 위에 유닛이 없고
                                    {
                                        if (tt == Tile.TileTypeEnum.BattleTile) // 원래 유닛이 있던 타일이 배틀 타일이라면
                                        {
                                            BattleTileArray[x, y] = tmpUnit1; // 그 배틀 타일에 tmpGameObject를 저장(이동)하고
                                            BattleTileArray[originUnitPos_x, originUnitPos_y] = null; // 기존에 있던 타일은 비워준 다음
                                            tmpUnit1.unitPosition = new Vector2Int(x, y);// 그 유닛의 타일에서의 위치 및 좌표를 바꿔준 다음
                                            tmpGameObject.transform.position = new Vector3(tile.transform.position.x, tmpGameObject_Ypos, tile.transform.position.z);
                                            tmpUnit1.IsReady = false; // 준비 상태를 해제한다.
                                        }
                                        else if (tt == Tile.TileTypeEnum.ReadyTile) // 원래 유닛이 있던 타일이 레디 타일이라면
                                        {
                                            if (UnitCount < LimitUnitCount) // 플레이어의 배틀 타일에 있는 유닛 수가 제한 유닛수 보다 작을 경우에만
                                            {
                                                // 유닛의 위치를 바꿔준다.
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
                                        else // 타일의 타입이 배틀 타입도 레디 타일도 아닐 경우 마우스를 누르면서 움직였던 유닛을 본래 있던 자리로 되돌린다.
                                        {
                                            tmpGameObject.transform.position = new Vector3(tmpTile.transform.position.x, tmpGameObject_Ypos, tmpTile.transform.position.z);
                                        }
                                    }
                                    else if(BattleTileArray[x, y]) // 그 배틀 타일 위에 유닛이 있다면
                                    {
                                        // 배틀 타일 위에 있던 GameObject의 Unit, GameObject 컴포넌트, 타일에서의 위치 값을 저장한다.
                                        Unit tmpUnit2 = BattleTileArray[x, y];
                                        GameObject tmpObj = tmpUnit2.gameObject;
                                        int tmpx = tmpUnit1.unitPosition.x, tmpy = tmpUnit1.unitPosition.y;

                                        if (tt == Tile.TileTypeEnum.BattleTile) // 마우스를 클릭했을 때의 타일 타입이 배틀 타일이라면
                                        {
                                            // 타일에서의 위치 및 좌표를 변경한다.
                                            BattleTileArray[x, y] = tmpUnit1;
                                            BattleTileArray[originUnitPos_x, originUnitPos_y] = tmpUnit2;
                                            tmpUnit1.unitPosition = new Vector2Int(x, y);
                                            tmpUnit2.unitPosition = new Vector2Int(tmpx, tmpy);
                                            tmpGameObject.transform.position = new Vector3(tile.transform.position.x, tmpGameObject_Ypos, tile.transform.position.z);
                                            tmpObj.transform.position = new Vector3(tmpTile.transform.position.x, tmpObj.transform.position.y, tmpTile.transform.position.z);

                                            // 배틀 타일에서 서로 위치를 바꾼 것으로 모두 준비 상태 해제해준다.
                                            tmpUnit1.IsReady = false;
                                            tmpUnit2.IsReady = false;
                                        }
                                        else if (tt == Tile.TileTypeEnum.ReadyTile) // 마우스를 클릭했을 때의 타일 타입이 준비 타일이라면
                                        {
                                            // 서로가 있던 타일과 준비 상태를 제외하면, 위와 과정을 동일하다.
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
                            else if(!isWait) // 게임 시간이 전투 시간이라면
                            {
                                int x = tile.TileWidth, y = tile.TileHeight; // 그 타일의 번호 x, y를 각각 저장하고

                                if (tile.tileType != Tile.TileTypeEnum.BattleTile) // 그 타일의 타입이 배틀 타일이 아닐 때
                                {
                                    if (!ReadyQue[x, y]) // 레디큐에 아무것도 없다면
                                    {
                                        if (tt == Tile.TileTypeEnum.ReadyTile) // 마우스 클릭했을 때의 타일의 타입이 레디타일일 경우에만
                                        {
                                            // 유닛의 위치를 바꿔준다.
                                            ReadyQue[x, y] = tmpGameObject.GetComponent<Unit>();
                                            ReadyQue[originUnitPos_x, originUnitPos_y] = null;
                                            tmpUnit1.unitPosition = new Vector2Int(x, y);
                                            tmpUnit1.IsReady = true;
                                            tmpGameObject.transform.position = new Vector3(tile.transform.position.x, tmpGameObject_Ypos, tile.transform.position.z);
                                        }
                                    }
                                    else if (ReadyQue[x, y]) // 만약 레디큐에 유닛이 있다면
                                    {
                                        // 그 유닛의 Unit, GameObject 컴포넌트를 저장한 후
                                        Unit tmpUnit2 = ReadyQue[x, y];
                                        GameObject tmpObj = tmpUnit2.gameObject;
                                        // tmp1의 타일에서의 위치 x, y 값을 저장한다.
                                        int tmpx = tmpUnit1.unitPosition.x, tmpy = tmpUnit1.unitPosition.y;

                                        if (tt == Tile.TileTypeEnum.ReadyTile) // 마우스 클릭했을 때의 타일의 타입이 레디타일일 경우에만
                                        {
                                            // 서로의 위치(타일 및 좌표)를 바꿔주고 준비 상태로 설정한다.
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
                                else // 타일의 타입이 배틀 타입도 레디 타일도 아닐 경우 마우스를 누르면서 움직였던 유닛을 본래 있던 자리로 되돌린다.
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
                    else // ray의 태그가 타일이 아니면(엉뚱한 곳에서 마우스를 뗐을 경우) 마우스를 누르면서 움직였던 유닛을 본래 있던 자리로 되돌린다.
                    {
                        tmpGameObject.transform.position = new Vector3(tmpTile.transform.position.x, tmpGameObject_Ypos, tmpTile.transform.position.z);
                    }
                }
                
                // 유닛 이동중에 사용했던 변수들을 초기화한 후
                tmpGameObject = null;
                tmpTile = null;

                // 배틀 타일에 있는 유닛의 숫자를 센다.
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

