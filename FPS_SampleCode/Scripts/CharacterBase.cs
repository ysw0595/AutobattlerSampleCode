using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    protected Rigidbody rb;

    protected Vector3 moveDir;
    [SerializeField] protected float speed = 5f;
    [SerializeField] protected float hp;
    [SerializeField] float jump;

    bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.gameOver && !GameManager.instance.putEsc)
        {
            Vector3 vec = Vector3.zero;

            vec += transform.forward * moveDir.z * speed;
            vec += transform.right * moveDir.x * speed;
            vec.y = rb.velocity.y;

            rb.velocity = vec;
        }
    }

    protected virtual void Update()
    {
        Dead();
        if(GameManager.instance.restart)
        {
            transform.position = new Vector3(300f, 3f, 300f);
        }
    }

    protected virtual void Dead()
    {
        if(hp <= 0.0f || GameManager.instance.GetTime() >= 300f)
        {
            GameManager.instance.GameOver();
        }
    }

    public void MoveTo(Vector3 dir)
    {
        moveDir = dir;
    }

    public void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jump);
        }
    }

    public void GetDamaged(float dmg)
    {
        hp -= dmg;
    }

    public float GetHp()
    {
        return hp;
    }

    private void OnTriggerEnter(Collider other)
    {
        isGrounded = true;
    }

    private void OnTriggerStay(Collider other)
    {
        isGrounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isGrounded = false;
    }
}
