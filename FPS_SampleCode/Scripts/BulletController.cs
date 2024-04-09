using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] BulletPooler bulletPooler;

    Queue<GameObject> bulletQueue = new Queue<GameObject>();
    [SerializeField] GameObject player;

    Rigidbody rb;

    public float bulletLifeTime = 0;

    public float distanceBetweenPlayer;

    float dmg = 30f;

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        dmg = 30f;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bulletPooler = GameObject.FindGameObjectWithTag("BulletPool").GetComponent<BulletPooler>();
    }

    private void Update()
    {
        if (!GameManager.instance.gameOver && !GameManager.instance.putEsc)
        {
            distanceBetweenPlayer = Mathf.Abs(Vector3.Distance(player.transform.position, transform.position));

            if (bulletLifeTime >= 3f || distanceBetweenPlayer > 200f)
            {
                bulletPooler.ReturnBullet(gameObject);
                distanceBetweenPlayer = 0;
            }
            else if (rb.velocity.magnitude <= 5f)
            {
                rb.velocity = Vector3.zero;
                bulletLifeTime += Time.deltaTime;
            }
        }
    }

    private void OnDisable()
    {
        transform.position = Vector3.zero;
        rb.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Field")
        {
            rb.useGravity = true;
            dmg = 0f;
        }
        else if(collision.collider.tag == "Enemy" || collision.collider.tag == "Player")
        {
            CharacterBase cb = collision.collider.GetComponent<CharacterBase>();

            cb.GetDamaged(dmg);
            bulletPooler.ReturnBullet(gameObject);
        }
    }
}
