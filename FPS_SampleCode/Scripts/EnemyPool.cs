using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] GameObject enemy;

    public static Queue<GameObject> enemyQueue = new Queue<GameObject>();

    [SerializeField] public int number;

    [Range(15f, 20f)]
    [SerializeField] float spawnTimeMin;
    [Range(50f, 60f)]
    [SerializeField] float spawnTimeMax;

    [SerializeField] float spawnTime;
    [SerializeField] float spawnTimeCheck = 0;


    private void Awake()
    {
        for (int i = 0; i < 20; i++)
        {
            GameObject obj = Instantiate(enemy);
            enemyQueue.Enqueue(obj);
        }

        spawnTime = Random.Range(spawnTimeMin, spawnTimeMax);
    }

    private void Update()
    {
        if (!GameManager.instance.gameOver && !GameManager.instance.putEsc)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (spawnTimeCheck >= spawnTime)
        {
            GameObject obj = enemyQueue.Dequeue();

            obj.SetActive(true);
            obj.transform.position = transform.position;
            spawnTimeCheck = 0;
        }
        else
        {
            spawnTimeCheck += Time.deltaTime;
        }
    }
}
