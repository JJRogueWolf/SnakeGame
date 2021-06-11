using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class knockBackRotate : MonoBehaviour
{
    void Update()
    {
        // continuous rotation of stars above snake
        // for when snake hits wall
        transform.Rotate(new Vector3(0f, 180f * Time.deltaTime, 0f));
    }
}
