using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // 각 플레이어들이 사용할 타일
    public const int battleTileX = 8;
    public const int battleTileY= 3;
    public const int queTileX = 9;
    public const int queTileY = 1;

    [Header("■ 유닛 구입 및 판매 위한 요소들")]
    // 유닛을 담을 공간(플레이어로 치면 유닛 상점 창에서 보이는 3개의 유닛들)
    protected GameObject[] unitArr = new GameObject[GameManager.unitPrefabSize];
    protected GameObject[] UnitPrefabs;

    [SerializeField] protected Unit[,] BattleTileArray = new Unit[battleTileX, battleTileY];
    [SerializeField] protected Unit[,] ReadyQue = new Unit[queTileX, queTileY];
    protected List<Unit> unitList;

    [Header("■ 게임 진행위한 요소들")]
    [SerializeField] protected int level = 1;
    public int Level { get { return level; } set { level = value; } }

    [SerializeField] protected int expForUp;
    public int ExpForUp { get { return expForUp; } set { expForUp = value; } }

    [SerializeField] protected int currentExp;
    public int CurrentExp { get { return currentExp; } set { currentExp = value; } }

    [SerializeField] protected int currentGold;
    public int CurrentGold { get { return currentGold; } set { currentGold = value; } }

    [SerializeField] public int currentHp = 100;

    [SerializeField] protected int limitUnitCount;
    public int LimitUnitCount { get => limitUnitCount; set => limitUnitCount = value; }
    protected int unitCount = 0;
    public int UnitCount { get { return unitCount; } set { unitCount = value; } }
    [SerializeField] protected GameObject[] unitInBattleField;

    bool shopLock;
    public bool ShopLock { get { return shopLock; } set { shopLock = value; } }

    protected bool isLocalPlayer = false;
    public virtual bool IsLocalPlayer { get { return isLocalPlayer; } set { isLocalPlayer = false; } }

    public int playerNum;

    GameManager.Era era = 0;
    public GameManager.Era Era { get { return era; } set { era = value; } }

    protected virtual void Start()
    {
        GameManager.AddPlayerList(this);
        GameManager.ClaimReRoll(this);
        ControlLevel();
    }

    protected virtual void Update()
    {
        if(GameManager.Instance.GameOver) { return; }
    }

    public void ControlLevel()
    { 
        switch (Level)
        {
            case 1:
                LevelSystem(6);
                SetUnitCountLimit();
                break;
            case 2:
                LevelSystem(12);
                SetUnitCountLimit();
                break;
            case 3:
                LevelSystem(20);
                SetUnitCountLimit();
                break;
            case 4:
                LevelSystem(30);
                SetUnitCountLimit();
                break;
            case 5:
                LevelSystem(42);
                SetUnitCountLimit();
                break;
            case 6:
                LevelSystem(56);
                SetUnitCountLimit();
                break;
            case 7:
                LevelSystem(72);
                SetUnitCountLimit();
                break;
            case 8:
                LevelSystem(90);
                SetUnitCountLimit();
                break;
            default:
                break;
        }
    }

    void LevelSystem(int maxExp)
    {
        ExpForUp = maxExp;

        if (currentExp >= ExpForUp)
        {
            if (level < 8)
            {
                level++;
                currentExp -= ExpForUp;
            }
        }
    }

    protected virtual int SetUnitCountLimit()
    {
        switch (Level)
        {
            case 1:
                LimitUnitCount = 1;
                break;
            case 2:
                LimitUnitCount = 2;
                break;
            case 3:
                LimitUnitCount = 3;
                break;
            case 4:
                LimitUnitCount = 4;
                break;
            case 5:
                LimitUnitCount = 5;
                break;
            case 6:
                LimitUnitCount = 6;
                break;
            case 7:
                LimitUnitCount = 7;
                break;
            case 8:
                LimitUnitCount = 6;
                break;
            default: 
                break;
        }

        return LimitUnitCount;
    }

    public void DecideBuyingExp(int cost)
    {
        if (CurrentGold >= cost)
        {
            CurrentGold -= cost;
            currentExp += 4;
            ControlLevel();
        }
    }

    public virtual void DecideReroll(int cost)
    {
        if(CurrentGold >= cost)
        {
            CurrentGold -= cost;
            GameManager.ClaimReRoll(this);
        }
    }

    public virtual GameObject Summon(int n)
    {
        // n 번째 unitArr의 Unit 컴포넌트를 가져와서 buyunit에 저장해줘
        Unit buyunit = unitArr[n].GetComponent<Unit>();
        // GameObject를 담을 obj도 하나 만들어 준 다음
        GameObject obj;

        // 사려고 하는 유닛의 가격을 저장해서
        int unitGold = buyunit.GetGold();

        // 현재 가지고 있는 재화와 비교를 해서 충분하면
        if (currentGold >= unitGold)
        {
            // 현재 재화에서 유닛의 가격을 빼고
            currentGold -= unitGold;

            // 그 유닛의 유닛 컴포넌트를 담을 공간을 만든 다음.
            Unit createdUnit;

            // n 번째 unitArr의 GameObject를 만들어 obj에 저장하는데
            // 생성된 GameObject의 Unit 컴포넌트를 createdUnit에 저장하고
            // 해당플레이어의 소유로 변경해줌.
            obj = GameManager.Instance.GiveUnitToPlayer(unitArr[n], this, out createdUnit);

            // 만들어진 유닛의 유닛 컴포넌트를 비어있는 레디 큐의 가장 앞 부분에 위치를 고정해줌.
            SetUnitInReadyQue(ref createdUnit);

            // 만약 방금 만들어진 유닛과 같은 유닛을 해당 플레이어가 얼마나 가지고 있느냐에 따라 진급 검사를 한다.
            if (GameManager.Instance.CheckPromote(createdUnit))
            {
                // 만약 방금 만든 유닛 포함 총 3기의 유닛을 보유하고 있다면
                // 유닛을 진급을 시켜줘야 한다.
                // createdUnit의 상위 유닛을 만들어줘야한다.
                Unit tmp = null;
                GameManager.Instance.PromoteUnit(createdUnit, out tmp);
                createdUnit = tmp;
                //Debug.Log($"{createdUnit.unitPosition.x}, {createdUnit.unitPosition.y}");
            }
        }
        else
        {
            obj = null;
        }

        TileController.ClaimDispose();

        return obj;
    }

    public virtual void SetUnitCount()
    {
        UnitCount = 0;

        for(int x = 0; x < BattleTileArray.GetLength(0); x++)
        {
            for(int y = 0; y < BattleTileArray.GetLength(1); y++)
            {
                if (BattleTileArray[x, y])
                {
                    UnitCount++;
                }
            }
        }
    }

    public void SetUnitInReadyQue(ref Unit unit)
    {
        if (!ReadyQue[unit.unitPosition.x, unit.unitPosition.y])
        {
            ReadyQue[unit.unitPosition.x, unit.unitPosition.y] = unit;
        }
        else
        {
            for (int x = 0; x < queTileX; x++)
            {
                for (int y = 0; y < queTileY; y++)
                {
                    if (x == unit.unitPosition.x && y == unit.unitPosition.y) continue;

                    if (!ReadyQue[x, y])
                    {
                        ReadyQue[x, y] = unit;
                        unit.IsReady = true;
                        unit.unitPosition.x = x;
                        unit.unitPosition.y = y;
                        return;
                    }
                }
            }
        }
    }

    public void SetUnitInBattleQue(ref Unit unit)
    {
        if (!BattleTileArray[unit.unitPosition.x, unit.unitPosition.y])
        {
            //Debug.Log($"{BattleTileArray[unit.unitPosition.x, unit.unitPosition.y]}");
            BattleTileArray[unit.unitPosition.x, unit.unitPosition.y] = unit;
            //Debug.Log($"{BattleTileArray[unit.unitPosition.x, unit.unitPosition.y]}");
        }
        else
        {
            for (int x = 0; x < battleTileX; x++)
            {
                for (int y = 0; y < battleTileY; y++)
                {
                    if (x == unit.unitPosition.x && y == unit.unitPosition.y) continue;

                    if (!BattleTileArray[x, y])
                    {
                        BattleTileArray[x, y] = unit;
                        unit.IsReady = false;
                        unit.unitPosition.x = x;
                        unit.unitPosition.y = y;
                        return;
                    }
                }
            }
        }
    }

    public void ReleaseQue(int x, int y, bool isReady)
    {
        if (isReady)
        {
            ReadyQue[x, y] = null;
        }
        else
        {
            BattleTileArray[x, y] = null;
        }
    }

    public virtual void SetUnitObject(GameObject obj, int i)
    {
        if (!ShopLock) unitArr[i] = obj;
        // Debug.Log(unitArr[i]);
    }

    public int[] GetReadyQueEmeptyArrayTile()
    {
        int[] arr = new int[2];

        for(int i = 0; i < ReadyQue.GetLength(0); i++)
        {
            for(int j = 0; j < ReadyQue.GetLength(1); j++)
            {
                if (!ReadyQue[i, j])
                {
                    arr[0] = i;
                    arr[1] = j;
                    return arr;
                }
            }
        }

        return null;
    }

    public Unit[,] GetReadyQue()
    {
        return ReadyQue;
    }

    public Unit[,] GetBattleQue()
    {
        return BattleTileArray;
    }

    protected bool CountUnitInReadyQue()
    {
        bool exist = false;

        foreach (Unit unit in ReadyQue)
        {
            if (unit)
            {
                exist = true;
                break;
            }
        }

        return exist;
    }

    protected virtual void MoveUnit()
    {
        
    }

    public void SetBattleArr(Player player)
    {
        unitList = new List<Unit>();

        foreach (Unit unit in player.BattleTileArray)
        {
            if (unit)
            {
                unitList.Add(unit);
            }
        }
    }

    public List<Unit> GetBattleArr()
    {
        List<Unit> list = new List<Unit>();

        for(int i = 0; i < BattleTileArray.GetLength(0); i++)
        {
            for(int j = 0; j < BattleTileArray.GetLength(1); j++)
            {
                if (BattleTileArray[i, j])
                {
                    list.Add(BattleTileArray[i, j]);
                }
            }
        }

        //Debug.Log($"{playerNum}");

        return unitList;
    }

    public void ShowQue()
    {
        for(int i = 0; i < BattleTileArray.GetLength(0); i++)
        {
            for(int j = 0; j < BattleTileArray.GetLength(1); j++)
            {
                Debug.Log($"{i}, {j}, {BattleTileArray[i, j]}");
            }
        }

        for (int i = 0; i < ReadyQue.GetLength(0); i++)
        {
            for (int j = 0; j < ReadyQue.GetLength(1); j++)
            {
                Debug.Log($"{i}, {j}, {ReadyQue[i, j]}");
            }
        }
    }

    public void SynergyEffect()
    {
        List<Unit> units = new();

        foreach(Unit unit in BattleTileArray)
        {
            if(unit) units.Add(unit);
        }

        GameManager.Instance.CheckSynergy(units);
    }
}

