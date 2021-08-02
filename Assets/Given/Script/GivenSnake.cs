using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivenSnake : MonoBehaviour
{
    public float snakeSpeed;
    private Rigidbody rb;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float hori = SimpleInput.GetAxis("Horizontal") * 0.3f;
        animator.SetFloat("x", hori);
        transform.Translate(Vector3.forward * snakeSpeed * Time.deltaTime);
        //rb.velocity = transform.position;
        transform.RotateAround(transform.position, Vector3.up, hori);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Vector3 dir = collision.contacts[0].point - transform.position;
            //normalize and add a threshold
            dir = dir.normalized * 3f;
            Vector3 reflectDirection = Vector3.Reflect(dir, collision.contacts[0].normal).normalized;
            rb.velocity = reflectDirection;
        }
    }

}
