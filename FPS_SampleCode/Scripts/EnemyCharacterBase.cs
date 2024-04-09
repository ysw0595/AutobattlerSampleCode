using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacterBase : CharacterBase
{
    float shootRange = 30f;
    float accessRange = 5f;
    [SerializeField] GunController gunController;

    float reloadTime;

    private void OnEnable()
    {
        StartLoad();
        hp = 50f;
    }

    private IEnumerator StartLoad()
    {
        Load();
        yield return new WaitForEndOfFrame();
    }

    protected override void Update()
    {
        if (!GameManager.instance.gameOver && !GameManager.instance.putEsc)
        {
            Dead();
            FindPlayer();
            ShootTo();

            if (gunController.GetCapacity() <= 0)
            {
                reloadTime += Time.deltaTime;
                if (reloadTime >= 1.0f)
                {
                    Load();
                    reloadTime = 0.0f;
                }
            }
        }
        else if (GameManager.instance.restart)
        {
            Back2Pool();
        }
    }

    private void OnDisable()
    {
        Back2Pool();
    }

    public void Back2Pool()
    {
        transform.position = Vector3.zero;
        rb.velocity = Vector3.zero;
        ReturnBulletTo();
    }

    void FindPlayer()
    {
        transform.LookAt(PlayerInstance.Instance.transform);

        if (Vector3.Distance(PlayerInstance.Instance.transform.position, gameObject.transform.position) >= accessRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, PlayerInstance.Instance.transform.position, speed * Time.deltaTime);
        }
    }

    void ShootTo()
    {
        if(Vector3.Distance(PlayerInstance.Instance.transform.position, gameObject.transform.position) <= shootRange)
        {
            gunController.Shoot();
        }
    }

    void ReturnBulletTo()
    {
        if(gunController.GetCapacity() > 0)
        {
            for(int i = 0; i< gunController.GetCapacity(); i++)
            {
                BulletPooler.bulletQue.Enqueue(gunController.GetMagazine());
            }
        }
    }

    void Load()
    {
        gunController.Load(10);
    }

    protected override void Dead()
    {
        if (hp <= 0.0f)
        {
            EnemyPool.enemyQueue.Enqueue(gameObject);
            gameObject.SetActive(false);
            GameManager.instance.Score += 1;
        }
    }
}
