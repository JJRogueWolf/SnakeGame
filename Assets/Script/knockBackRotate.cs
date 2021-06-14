using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class knockBackRotate : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, 180f * Time.deltaTime, 0f));
    }
}
