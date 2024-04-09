using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AIPlayer : Player
{
    float playTerm = 0f;
    [SerializeField] float doPlay;

    [Header("¡á Percent of Ai Behavior")]
    [SerializeField] [Range(0f, 1f)] float summonRatio;
    [SerializeField] [Range(0f, 1f)] float dk;

    bool b = false;

    protected override void Start()
    {
        base.Start();
        playerNum = 1;
    }

    protected override void Update()
    {
        base.Update();
        playTerm += Time.deltaTime;

        if (GameManager.Instance.IsWaitTime) { 
            if (playTerm >= doPlay)
            {
                // Debug.Log($"{unitArr[0]}, {unitArr[1]}, {unitArr[2]}");
                // Debug.Log("AI is doing something.");
                if (GameManager.Instance.IsWaitTime)
                {
                    //Debug.Log("SetUnitCount() is operating");
                    SetUnitCount();

                    //Debug.Log(UnitCount);
                    if (UnitCount < LimitUnitCount)
                    {
                        if (CheckUnitArrComp() == 0)
                        {
                            //Debug.Log("CheckUnitArrComp()");
                            DecideReroll(RerollController.rerollCost);
                            playTerm = 0f;
                            return;
                        }
                        else
                        {
                            if (!b)
                            {
                                //Debug.Log("Summon()");
                                Summon(0);
                                b = true;
                            }
                            //Debug.Log("MoveUnit()");
                            else
                            {
                                MoveUnit();
                                b = false;
                            }
                            playTerm = 0f;
                            return;
                        }
                    }
                    else if (AbleLevelUp())
                    {
                        DecideBuyingExp(4);
                    }
                    else
                    {
                        int ranBehave = Random.Range(0, 3);
                        switch (ranBehave)
                        {
                            case 0:
                                if (!b)
                                {
                                    Summon(0);
                                    b = true;
                                }
                                else
                                {
                                    MoveUnit();
                                    b = false;
                                }
                                playTerm = 0f;
                                break;
                            case 1:
                                DecideBuyingExp(4);
                                break;
                            default:
                                break;
                        }
                    }
                }
                playTerm = 0f;
            }
        }
    }

    public override GameObject Summon(int n)
    {
        GameObject obj = null;

        while (true)
        {
            if (CheckUnitArrComp() == 0) return null;

            n = Random.Range(0, unitArr.Length);

            if (unitArr[n])
            {
                obj = base.Summon(n);
                
                if(obj) obj.transform.forward = -obj.transform.forward;
                break;
            }
            else
            {
                continue;
            }
        }

        unitArr[n] = null;
        TileController.ClaimDispose();

        return obj;
    }

    protected int CheckUnitArrComp()
    {
        int cnt = 0;

        for(int i = 0; i < unitArr.Length; i++)
        {
            if (unitArr[i]) cnt++;
        }

        return cnt;
    }

    protected override void MoveUnit()
    {
        if (UnitCount >= LimitUnitCount) return;

        if (CountUnitInReadyQue())
        {
            Unit unit = GetRanUnitInReadyQue();
            Unit.UnitType ut = unit.GetUnitType();
            MoveToBattleTile(unit, ut);
            SynergyEffect();
        }
    }

    protected Unit GetRanUnitInReadyQue()
    {
        List<Unit> unitList = new List<Unit>();
        int ran = 0;
        int k = 0;

        for(int i = 0; i < ReadyQue.GetLength(0); i++)
        {
            for (int j = 0; j < ReadyQue.GetLength(1); j++, k++)
            {
                //Debug.Log($"{i}, {j}, {k}, {ReadyQue[i, j]}");
                if (ReadyQue[i, j])
                {
                    unitList.Add(ReadyQue[i, j]);
                    //Debug.Log(ReadyQue[i, j]);
                }
            }
        }

        ran = Random.Range(0, unitList.Count);

        return unitList[ran];
    }

    protected void MoveToBattleTile(Unit unit, Unit.UnitType unitType)
    {
        GameObject obj = unit.GetComponent<GameObject>();
        int ranPosX, ranPosY;

        if (unitType != Unit.UnitType.¿ø°Å¸®)
        {
            while (true)
            {
                ranPosX = Random.Range(0, battleTileX);
                ranPosY = Random.Range(0, 2);

                if (BattleTileArray[ranPosX, ranPosY]) continue;

                int oriX = unit.unitPosition.x, oriY = unit.unitPosition.y;

                unit.IsReady = false;
                unit.unitPosition.x = ranPosX;
                unit.unitPosition.y = ranPosY;
                BattleTileArray[ranPosX, ranPosY] = unit;
                
                ReadyQue[oriX, oriY] = null;
                break;
            }
        }
        else
        {
            while (true)
            {
                ranPosX = Random.Range(0, battleTileX);
                ranPosY = Random.Range(1, 3);

                if (BattleTileArray[ranPosX, ranPosY]) continue;

                int oriX = unit.unitPosition.x, oriY = unit.unitPosition.y;

                unit.unitPosition.x = ranPosX;
                unit.unitPosition.y = ranPosY;
                BattleTileArray[ranPosX, ranPosY] = unit;
                unit.IsReady = false;
                ReadyQue[oriX, oriY] = null;
                break;
            }
        }

        unit.IsReady = false;

        TileController.ClaimDispose();
    }

    protected bool AbleLevelUp()
    {
        if(expForUp - currentExp <= 8 && currentGold > 8f)
        {
            DecideBuyingExp(4);
            return true;
        }
        else if(expForUp - currentExp <= 4 && currentGold > 4f)
        {
            return true;
        }
        
        return false;
    }
}
