using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixingUI : MonoBehaviour
{
    Camera cam;
    Vector3 dir;

    private void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        transform.eulerAngles = new Vector3(50f, 0, 0);
    }
}
