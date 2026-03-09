using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearRotate : MonoBehaviour
{
    public float speed = 40f;

    void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}