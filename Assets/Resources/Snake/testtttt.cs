using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testtttt : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        print("Hello");
        transform.Rotate(0, Time.deltaTime * 6, 0);
    }
}
