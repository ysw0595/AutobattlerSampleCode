using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements.Experimental;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine.UIElements;
using UnityEditor.Animations;
using System.Security.Cryptography;
using static UnityEngine.UI.CanvasScaler;
using System.Timers;

public class Unit : MonoBehaviour
{
    public enum UnitType
    {
        근접 = 0,
        원거리,
        기병
    }

    public enum UnitEra
    {
        고대시대 = 0,
        고전시대,
        중세시대,
        근대시대
    }

    [SerializeField] Animator animator;

    [Header("■ UnitInfo")]
    protected string group;
    [SerializeField] protected int maxHp;
    [SerializeField] protected int hp;
    public int MaxHp {  get { return maxHp; } set {  maxHp = value; } }
    [SerializeField] protected int atk;
    public int Atk { get { return atk; } set { atk = value; } }
    [SerializeField] protected float atkSpeed;
    protected float atkTime = 0;
    [SerializeField] protected float range;
    public float Range { get { return range; } set {  range = value; } }
    [SerializeField] protected float speed;
    [SerializeField] protected int cost;
    [SerializeField] Sprite unitImage;
    [SerializeField] UnitType unitType;
    [SerializeField] UnitEra unitEra;
    public string unitName;
    public bool dead = false;
    public Player Owner;

    public Vector2Int unitPosition;
    public bool IsReady;

    public int unitNum;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameManager.Instance.GameOver)
        {
            animator.SetBool("IsAttack", false);
            return;
        }

        if (!GameManager.Instance.IsWaitTime)
        {
            if (!IsReady)
            {
                if (!dead)
                {
                    Unit unit = TargetSearch(Owner.GetBattleArr());

                    float dst = Mathf.Abs(Vector3.Distance(transform.position, unit.transform.position));

                    if (dst > range)
                    {
                        MoveTo(unit);
                    }
                    else
                    {
                        if (atkTime >= atkSpeed)
                        {
                            Attack(unit);
                            unit.hp -= atk;
                            atkTime = 0.0f;
                        }
                        else
                        {
                            atkTime += Time.deltaTime;
                        }
                    }
                    
                    if(hp <= 0)
                    {
                        hp = 0;
                        dead = true;
                        animator.SetBool("IsDead", dead);
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void OffSet()
    {
        hp = maxHp;
        atkTime = atkSpeed;
        dead = false;
        animator.SetBool("IsDead", dead);

        if(Owner.playerNum == 0)
        {
            transform.forward = new Vector3(0, 0, 1);
        }
        else
        {
            transform.forward = new Vector3(0, 0, -1);
        }
        
        animator.SetBool("IsAttack", false);
        gameObject.SetActive(true);
    }

    public void SetMaxHp()
    {
        hp = MaxHp;
    }

    public string GetName()
    {
        return name;
    }

    public int GetGold()
    {
        return cost;
    }

    public UnitEra GetUnitEra()
    {
        return unitEra;
    }

    public string GetEra()
    {
        return unitEra.ToString();
    }

    public string GetEraClass()
    {
        string eraClass = unitEra.ToString() + "\n" + unitType.ToString();
        return eraClass;
    }

    public Sprite GetSprite()
    {
        return unitImage;
    }

    public UnitType GetUnitType()
    {
        return unitType;
    }

    protected virtual void Attack(Unit unit)
    {
        if (unit.dead) { animator.SetBool("IsAttack", false); return; }

        Vector3 dir = new Vector3(unit.transform.position.x, transform.position.y, unit.transform.position.z);
        transform.LookAt(dir);
        animator.SetBool("IsAttack", true);
    }

    Unit TargetSearch(List<Unit> unitLIst)
    {
        float min = 0;
        int num = 0;
        int i = -1;

        //foreach(Unit unit in unitLIst)
        //{
        //    //Debug.Log($"{unit}");
        //}

        foreach (Unit unit in unitLIst)
        {
            i++;
            if (!unit.dead)
            {
                GameObject obj = unit.gameObject;
                // Debug.Log($"unit: {unit}, obj: {obj}, {i}");

                float goX = transform.position.x;
                float goZ = transform.position.z;

                float objX = obj.transform.position.x;
                float objZ = obj.transform.position.z;

                Vector3 goDir = new Vector3(goX, 0, goZ);
                Vector3 objDir = new Vector3(objX, 0, objZ);

                float dst = Mathf.Abs(Vector3.Distance(goDir, objDir));

                if (dst < min || min == 0)
                {
                    min = dst;
                    num = i;
                }
            }
        }

        // Debug.Log($"{Owner}의 {gameObject} target은 {unitLIst[num].Owner}의 {unitLIst[num].gameObject}");

        return unitLIst[num];
    }

    void MoveTo(Unit unit)
    {
        Vector3 dir = new Vector3(unit.transform.position.x, transform.position.y, unit.transform.position.z);

        transform.position = Vector3.MoveTowards(transform.position, unit.transform.position, speed * Time.deltaTime);
        transform.LookAt(dir);
    }

    public int GetCost()
    {
        return cost;
    }

    public int GetMaxHp()
    {
        return maxHp;
    }

    public int GetHp()
    {
        return hp;
    }
}
