using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class food : MonoBehaviour
{
    public float rotationsPerMinute = 120f;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,rotationsPerMinute * Time.deltaTime, 0);
    }
}
