using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snake : MonoBehaviour
{
    [SerializeField]
    private GameObject snakePivot;

    [HideInInspector]
    public float snakeSpeed = 1f;

    [HideInInspector]
    public GameManager gameManager;

    private Animator animator;
    private Rigidbody snakeBody;
    private ContactPoint contactPoint;

    [HideInInspector]
    public bool isHit = false;

    private void Start()
    {
        snakeBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        float hori = SimpleInput.GetAxis("Horizontal");

        if (!isHit)
        {
            animator.SetFloat("x", hori);
            transform.Translate(Vector3.forward * snakeSpeed * Time.deltaTime);
            transform.RotateAround(snakePivot.transform.position, Vector3.up, hori);
        }
        else
        {
            Vector3 newDirection = Vector3.Reflect(transform.forward, contactPoint.normal);
            snakeBody.AddTorque(newDirection);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            isHit = true;
            animator.SetFloat("x", 0f);
            contactPoint = collision.contacts[0];
            Vector3 dir = contactPoint.point - transform.position;
            //normalize and add a threshold
            dir = dir.normalized;
            // Reflects snake off the plane defined by a normal.
            snakeBody.velocity = Vector3.Reflect(dir * 5f, contactPoint.normal);
            snakeBody.AddTorque(Vector3.forward);
        }

        if (collision.gameObject.tag == "Food")
        {
            gameManager.isfoodSpawned = false;
            gameManager.uiManager.score += 1;
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            isHit = false;
        }
    }
}
