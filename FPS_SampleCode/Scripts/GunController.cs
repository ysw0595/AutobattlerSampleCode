using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] Transform pointTransform;
    [SerializeField] Transform sightTransform;
    [SerializeField] BulletPooler bulletPooler;
    Vector3 gunVec;

    Queue<GameObject> bulletQueMagazine = new Queue<GameObject>();
    int capacity = 0;

    public bool shoot = false;
    public float shootSpeed = 0;

    private void Start()
    {
        gunVec = Vector3.zero;
    }

    private void Update()
    {
        if (!GameManager.instance.gameOver && !GameManager.instance.putEsc)
        {
            if (ViewController.aimVec != null)
            {
                gunVec = ViewController.aimVec - pointTransform.position;
                Debug.DrawRay(pointTransform.position, gunVec * 200f, Color.green);
            }
            else
            {
                gunVec = ViewController.aimVec;
                Debug.DrawRay(pointTransform.position, gunVec * 200f, Color.green);
            }

            shootSpeed += Time.deltaTime;

            if (shootSpeed >= 0.8f)
            {
                shoot = true;
            }
        }
    }

    public void Load()
    {
        int size = 30 - capacity;

        for(int i = 0; i < size; i++)
        {
            GameObject obj = BulletPooler.bulletQue.Dequeue();

            bulletQueMagazine.Enqueue(obj);
        }

        capacity = 30;
    }

    public void Load(int n)
    {
        int size = n - capacity;

        for (int i = 0; i < size; i++)
        {
            GameObject obj = BulletPooler.bulletQue.Dequeue();

            bulletQueMagazine.Enqueue(obj);
        }

        capacity = 10;
    }

    public void Shoot()
    {
        if (capacity > 0)
        {
            if (shoot)
            {
                GameObject bullet = bulletQueMagazine.Dequeue();
                bullet.SetActive(true);
                bullet.transform.position = pointTransform.position;
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                rb.AddForce(pointTransform.forward * 800f);
                capacity--;
                shootSpeed = 0;
                shoot = false;
            }
        }
    }

    public void ShootFor()
    {
        if (capacity > 0)
        {
            if (shoot)
            {
                GameObject bullet = bulletQueMagazine.Dequeue();
                bullet.SetActive(true);
                bullet.transform.position = pointTransform.position;
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                rb.AddForce(gunVec.normalized * 800f);
                capacity--;
                shootSpeed = 0;
                shoot = false;
            }
        }
    }

    public int GetCapacity()
    {
        return capacity;
    }

    public GameObject GetMagazine()
    {
        return bulletQueMagazine.Dequeue();
    }
}
