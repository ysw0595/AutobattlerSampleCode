using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using TMPro.EditorUtilities;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

public class GameManager : MonoBehaviour
{
    public enum Era
    {
        ���ô� = 0,
        �����ô�,
        �߼��ô�,
        �ٴ�ô�
    }

    List<Player> playerList = new List<Player>();
    System.Action ATurnEnd;
    int isLocalPlayer;
    public int IsLocalPlayer { get { return isLocalPlayer; } set { isLocalPlayer = value; } }

    static GameManager instance = null;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    [Header("�� ���� �̵��� ���� ��ҵ�")]
    [SerializeField] GameObject canvas;
    EventSystem currentSystem;
    [SerializeField] GraphicRaycaster raycaster;
    [SerializeField] Slider slider;
    GameObject tmpGameObject;
    Tile tmpTile;
    float tmpGameObject_Ypos;
    public GameObject PointOverObject { get; protected set; }

    [Header("�� ���� �������� ��ҵ�")]
    public const int unitPrefabSize = 3;
    [SerializeField] GameObject[] unitPrefabs1;
    [SerializeField] GameObject[] unitPrefabs2;
    [SerializeField] GameObject unitPool;

    [Header("�� UI")]
    [SerializeField] ShowTimeController showTimeController;
    //[SerializeField] UnitCountLimit unitCountLimit;
    [SerializeField] RerollController rerollController;
    [SerializeField] LockController lockController;
    [SerializeField] ShowAge showAge;
    [SerializeField] SynergyController synergyController;
    [SerializeField] GameOver gameOverController;

    [Header("�� ���� �������� ��ҵ�")]
    int stage = 1;
    public int Stage { get { return stage; } set { Stage = stage; } }
    [SerializeField] Era age;
    public Era Age { get { return age; } set { age = value; } }
    [SerializeField] bool isWaitTime = true;
    public bool IsWaitTime { get { return isWaitTime; } }
    [SerializeField] float timeFlow;
    [SerializeField] float timeLimit;
    Unit[] promotingUnitArr = new Unit[3];
    public int increaseGold;
    public int increaseExp;

    [SerializeField] TileController upTileController;
    [SerializeField] TileController downTileController;

    [SerializeField] bool isDealed = false;

    bool gameOver = false;
    public bool GameOver { get { return gameOver; } set {  gameOver = value; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private IEnumerator Start()
    {
        SetPlayerUnitPrefabs();
        // currentSystem = FindObjectOfType<EventSystem>();

        yield return new WaitForEndOfFrame();
        downTileController.currentPlayer = LocalPlayer.LP;
        upTileController.currentPlayer = playerList[1].gameObject.GetComponent<AIPlayer>();
    }

    void Update()
    {
        if (playerList[0].currentHp <= 0)
        {
            if (!GameOver)
            {
                gameOverController.gameObject.SetActive(true);
                gameOverController.gameOver.text = $"�й��Ͽ����ϴ�s!";
                gameOverController.gameOver.color = new Color32(0, 0, 255, 255);
            }
            GameOver = true;
            return;
        }
        else if (playerList[1].currentHp <= 0)
        {
            if (!GameOver)
            {
                gameOverController.gameObject.SetActive(true);
                gameOverController.gameOver.text = $"�¸��Ͽ����ϴ�!";
                gameOverController.gameOver.color = new Color32(255, 0, 0, 255);
            }
            GameOver = true;
            return;
        }

        TimeController();
        if (!isWaitTime)
        {
            if (CheckDefeatedPlayer())
            {
                DealingScore();
                isDealed = true;
                timeFlow = timeLimit;
            }
        }        
        UIRay();
    }

    void TimeController()
    {
        timeFlow += Time.deltaTime;

        if (timeFlow > timeLimit)
        {
            timeFlow = 0;

            isWaitTime = !isWaitTime;

            if (isWaitTime)
            {
                if (!isDealed)
                {
                    DealingScore();
                    isDealed = true;
                }
                
                AgeSystem();
                SetPlayerUnitPrefabs();
                GiveResourcesTo();
                SetNormal();
                TileController.ClaimDispose();
            }
            else
            {
                HandOutList();
                isDealed = false;
            }

            showTimeController.ShowTurnText(isWaitTime);
        }

        showTimeController.ShowTimeGraph(timeLimit, timeFlow);
    }

    private void GiveResourcesTo()
    {
        foreach(Player p in playerList)
        {
            if (p != null)
            {
                p.CurrentExp += increaseExp;
                p.CurrentGold += increaseGold;
                p.ControlLevel();
            }
        }
    }

    void UIRay()
    {
        PointerEventData point = new PointerEventData(currentSystem);
        point.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(point, results);

        if (results.Count > 0)
        {
            PointOverObject = results[0].gameObject;
        }
        else
        {
            PointOverObject = null;
        }
    }

    public float ReadyTime() { return timeLimit; }

    public static void AddPlayerList(Player player)
    {
        if (player == LocalPlayer.LP)
        {
            instance.playerList.Insert(0, player);
        }
        else
        {
            instance.playerList.Add(player);
        };
    }

    public void AgeSystem()
    {
        stage++;
        if (stage > 5)
        {
            stage = 1;
            Age++;
        }

        showAge.ShowEra();
    }

    public void SetPlayerUnitPrefabs()
    {
        switch (Age)
        {
            case 0:
                foreach (Player player in playerList)
                {
                    ClaimReRoll(player);
                }
                break;
        }
    }

    public static void ClaimReRoll(Player player)
    {
        for (int i = 0; i < unitPrefabSize; i++)
        {
            int ran = UnityEngine.Random.Range(0, instance.unitPrefabs1.Length);

            player.SetUnitObject(instance.unitPrefabs1[ran], i);
        }
    }

    public GameObject GiveUnitToPlayer(GameObject obj, Player player, out Unit unit)
    {
        GameObject go = Instantiate(obj);
        unit = go.GetComponent<Unit>();
        unit.Owner = player;

        return go;
    }

    public bool GetTurn()
    {
        return isWaitTime;
    }

    void HandOutList()
    {
        foreach (Player p in playerList)
        {
            if(p.playerNum == 0)
            {
                p.SetBattleArr(playerList[1]);
            }
            else if(p.playerNum == 1)
            {
                p.SetBattleArr(playerList[0]);
            }
        }
    }

    public bool CheckPromote(Unit unit)
    {
        int owner = unit.Owner.playerNum;

        //Debug.Log(CountPromotingUnit(owner, unit));
        if (CountPromotingUnit(owner, unit) == 3) return true;
        
        return false;
    }

    int CountPromotingUnit(int playerNum, Unit unit)
    {
        int count = 0;

        //Debug.Log($"playerNum: {playerNum}, Unit: {unit}");
        //Debug.Log($"playerList[playerNum]: {playerList[playerNum]}");
        foreach (Unit u in playerList[playerNum].GetBattleQue())
        {
            //Debug.Log($"unit.name: {unit.name}, ");
            if (u)
            {
                // Debug.Log($"u.name: {u.name}");
                if (unit.unitName == u.unitName)
                {
                    promotingUnitArr[count] = u;
                    count++;
                }
            }
            if (count == 3) 
            { 
                //Debug.Log(count);
                return count;
            }
        }
        foreach (Unit u in playerList[playerNum].GetReadyQue())
        {
            //Debug.Log($"unit.name: {unit.unitName}, ");
            if (u)
            {
                //Debug.Log($"u.name: {u.unitName}");
                if (unit.unitName == u.unitName)
                {
                    promotingUnitArr[count] = u;
                    count++;
                }
            }
            if (count == 3) 
            { 
                //Debug.Log(count); 
                return count;
            }
        }

        //Debug.Log(count);
        return count;
    }

    public GameObject PromoteUnit(Unit unit, out Unit tmp)
    {
        GameObject obj = null;
        tmp = null;

        switch (unit.unitName)
        {
            case "�������":
                obj = Instantiate(unitPrefabs2[0]);
                tmp = obj.GetComponent<Unit>();
                break;
            case "���ü�":
                obj = Instantiate(unitPrefabs2[1]);
                tmp = obj.GetComponent<Unit>();
                break;
            case "���⺴":
                obj = Instantiate(unitPrefabs2[2]);
                tmp = obj.GetComponent<Unit>();
                break;
            case "�����к�":
                obj = Instantiate(unitPrefabs2[3]);
                tmp = obj.GetComponent<Unit>();
                break;
            case "���â��":
                obj = Instantiate(unitPrefabs2[4]);
                tmp = obj.GetComponent<Unit>();
                break;
            case "�����ô��":
                obj = Instantiate(unitPrefabs2[5]);
                tmp = obj.GetComponent<Unit>();
                break;

        }

        if (unit.Owner.playerNum != 0)
        {
            obj.transform.forward *= -1;
        }

        tmp.IsReady = promotingUnitArr[0].IsReady;
        tmp.Owner = promotingUnitArr[0].Owner;
        tmp.unitPosition.x = promotingUnitArr[0].unitPosition.x;
        tmp.unitPosition.y = promotingUnitArr[0].unitPosition.y;

        //Debug.Log($"{tmp.unitPosition.x} {tmp.unitPosition.y}");

        for (int i = 0; i < 3; i++)
        {
            int x = promotingUnitArr[i].unitPosition.x;
            int y = promotingUnitArr[i].unitPosition.y;

            promotingUnitArr[i].Owner.ReleaseQue(x, y, promotingUnitArr[i].IsReady);

            GameObject og = promotingUnitArr[i].gameObject;
            Destroy(og);

            promotingUnitArr[i] = null;
        }

        if (tmp.IsReady)
        {
            tmp.Owner.SetUnitInReadyQue(ref tmp);
        }
        else if(!tmp.IsReady) 
        {
            tmp.Owner.SetUnitInBattleQue(ref tmp);
        }


        return obj;
    }

    public void SetNormal()
    {
        foreach(Player p in playerList)
        {
            List<Unit> units = p.GetBattleArr();

            foreach(Unit u in units)
            {
                u.OffSet();
            }
        }
    }

    public bool CheckDefeatedPlayer()
    {
        foreach (Player p in playerList)
        {
            int n = 0;
            foreach (Unit u in p.GetBattleArr())
            {
                if (!u.dead)
                {
                    n++;
                }
            }

            if (n == 0)
            {
                return true;
            }
        }

        return false;
    }

    public void DealingScore()
    {
        foreach(Player p in playerList)
        {
            int cnt = 0;

            foreach(Unit u in p.GetBattleArr())
            {
                if (!u.dead)
                {
                    cnt++;
                }
            }

            if(cnt == 0)
            {
                Scoring(p);
            }
        }
    }

    public void Scoring(Player player)
    {
        if(player.playerNum == 0)
        {
            playerList[1].currentHp -= (int)(GetSumUnitHp(playerList[1].GetBattleArr(), player) * 0.1f);
        }
        else
        {
            playerList[0].currentHp -= (int)(GetSumUnitHp(playerList[0].GetBattleArr(), player) * 0.1f);
        }
    }

    public int GetSumUnitHp(List<Unit> arr, Player player)
    {
        int sum = 0;
        
        foreach(Unit u in arr)
        {
            sum += u.GetHp();
        }

        if(sum == 0)
        {
            return 100;
        }

        return sum;
    }

    public int Sorting(Unit u1, Unit u2)
    {
        if (u1.unitNum > u2.unitNum) return -1;
        else if (u1.unitNum == u2.unitNum) return 0;
        else return 1;
    }

    public void CheckSynergy(List<Unit> units)
    {
        units.Sort((a, b) => Sorting(a, b));
        List<Unit> list = units.GroupBy(u => u.unitNum).Select(u => u.First()).Distinct().ToList();

        Player owner = null;

        if (list.Count > 0)
        {
            owner = list[0].Owner;
        }
        else
        {
            synergyController.NoUnitInBattleArr();
        }

        int[] eras = Enumerable.Repeat<int>(0, Enum.GetValues(typeof(Unit.UnitEra)).Length).ToArray();
        int[] types = Enumerable.Repeat<int>(0, Enum.GetValues(typeof(Unit.UnitType)).Length).ToArray();

        foreach (Unit u in list)
        {
            switch(u.GetUnitEra())
            {
                case Unit.UnitEra.���ô�:
                    eras[Convert.ToInt32(Unit.UnitEra.���ô�)]++;
                    break;
                case Unit.UnitEra.�����ô�:
                    eras[Convert.ToInt32(Unit.UnitEra.�����ô�)]++;
                break;
                default:
                    break;
            }

            switch(u.GetUnitType())
            {
                case Unit.UnitType.����:
                    types[Convert.ToInt32(Unit.UnitType.����)]++; 
                    break;
                case Unit.UnitType.���Ÿ�:
                    types[Convert.ToInt32(Unit.UnitType.���Ÿ�)]++; 
                    break;
                case Unit.UnitType.�⺴:
                    types[Convert.ToInt32(Unit.UnitType.�⺴)]++;
                    break;
                default:
                    break;
            }
        }

        for (int i = 0; i < eras.Length; i++)
        {
            //Debug.Log($"{i}: {eras[i]}");
            synergyController.AbleEraSynergy(i, eras[i], owner);
        }

        for (int i = 0; i < types.Length; i++)
        {
            //Debug.Log($"{i}: {types[i]}");
            synergyController.AbleTypeSynergy(i, types[i], owner);
        }

        // ���ô� �ó��� (��)Ȱ��ȭ
        if (eras[Convert.ToInt32(Unit.UnitEra.���ô�)] >= 3)
        {
            foreach(Unit u in units)
            {
                if(u.GetUnitEra() == Unit.UnitEra.���ô�)
                {
                    Ancient ancient = u.gameObject.GetComponent<Ancient>();
                    ancient.enabled = true;
                }
            }
        }
        else
        {
            foreach (Unit u in units)
            {
                if (u.GetUnitEra() == Unit.UnitEra.���ô�)
                {
                    Ancient ancient = u.gameObject.GetComponent<Ancient>();
                    ancient.enabled = false;
                }
            }
        }

        // �����ô� �ó��� (��)Ȱ��ȭ
        if (eras[Convert.ToInt32(Unit.UnitEra.�����ô�)] >= 3)
        {
            foreach(Unit u in units)
            {
                if(u.GetUnitEra() == Unit.UnitEra.�����ô�)
                {
                    Classic classic = u.gameObject.GetComponent<Classic>();
                    classic.enabled = true;
                }
            }
        }
        else
        {
            foreach (Unit u in units)
            {
                if (u.GetUnitEra() == Unit.UnitEra.�����ô�)
                {
                    Classic classic = u.gameObject.GetComponent<Classic>();
                    classic.enabled = false;
                }
            }
        }

        // ���� �ó��� (��)Ȱ��ȭ
        if (types[Convert.ToInt32(Unit.UnitType.����)] >= 3)
        {
            foreach(Unit u in units)
            {
                if(u.GetUnitType() == Unit.UnitType.����)
                {
                    Melee melee = u.gameObject.GetComponent<Melee>();
                    melee.enabled = true;
                }
            }
        }
        else
        {
            foreach (Unit u in units)
            {
                if (u.GetUnitType() == Unit.UnitType.����)
                {
                    Melee melee = u.gameObject.GetComponent<Melee>();
                    melee.enabled = false;
                }
            }
        }

        // ���Ÿ� �ó��� (��)Ȱ��ȭ
        if (types[Convert.ToInt32(Unit.UnitType.���Ÿ�)] >= 3)
        {
            foreach(Unit u in units)
            {
                if(u.GetUnitType() == Unit.UnitType.���Ÿ�)
                {
                    Range range = u.gameObject.GetComponent<Range>();
                    range.enabled = true;
                }
            }
        }
        else
        {
            foreach (Unit u in units)
            {
                if (u.GetUnitType() == Unit.UnitType.���Ÿ�)
                {
                    Range range = u.gameObject.GetComponent<Range>();
                    range.enabled = false;
                }
            }
        }

        // �⺴ �ó��� (��)Ȱ��ȭ
        if (types[Convert.ToInt32(Unit.UnitType.�⺴)] >= 1)
        {
            foreach(Unit u in units)
            {
                if(u.GetUnitType() == Unit.UnitType.�⺴)
                {
                    Speed speed = u.gameObject.GetComponent<Speed>();
                    speed.enabled = true;
                }
            }
        }
        else
        {
            foreach (Unit u in units)
            {
                if (u.GetUnitType() == Unit.UnitType.�⺴)
                {
                    Speed speed = u.gameObject.GetComponent<Speed>();
                    speed.enabled = false;
                }
            }
        }
    }
}
