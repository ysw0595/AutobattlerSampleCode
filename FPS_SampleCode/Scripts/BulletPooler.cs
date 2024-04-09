using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BulletPooler : MonoBehaviour
{
    public static Queue<GameObject> bulletQue = new Queue<GameObject>();

    [SerializeField] GameObject bulletPrefab;

    void OnEnable()
    {
        for (int i = 0; i < 300; i++)
        {
            GameObject obj = Instantiate(bulletPrefab);

            bulletQue.Enqueue(obj);

            DontDestroyOnLoad(obj);
        }
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletQue.Enqueue(bullet);
    }
}
