using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterBase characterBase;
    [SerializeField] GunController gunController;

    bool reloading = false;
    float reloadTime;

    void Start()
    {
        StartLoad();
    }

    private IEnumerator StartLoad()
    {
        Load();
        yield return new WaitForEndOfFrame();
    }

    private void Update()
    {
        if (!GameManager.instance.gameOver && !GameManager.instance.putEsc)
        {
            GiveDirTo();
            Jump();

            if (!reloading)
            {
                Shoot();
            }
            else if (reloading)
            {
                reloadTime += Time.deltaTime;
            }

            if (reloadTime >= 0.8f)
            {
                Load();
                reloading = false;
                reloadTime = 0;
            }

            if (Input.GetKeyDown(KeyCode.R) || gunController.GetCapacity() <= 0)
            {
                reloading = true;
            }
        }
    }

    void GiveDirTo()
    {
        float dirX = Input.GetAxisRaw("Horizontal");
        float dirZ = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(dirX, 0, dirZ);

        characterBase.MoveTo(dir);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            characterBase.Jump();
        }
    }

    void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            gunController.ShootFor();
        }
    }

    public void Load()
    {
        gunController.Load();
    }

    public float GetHp()
    {
        return characterBase.GetHp();
    }
}
