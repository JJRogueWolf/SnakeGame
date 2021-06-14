using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thirdGameSnake : MonoBehaviour
{
    [SerializeField]
    private GameObject snakePivot;

    [SerializeField]
    private GameObject stars;

    public Camera snakeCamera;

    [HideInInspector]
    public float snakeSpeed = 1f;

    [HideInInspector]
    public thirdGameManager gameManager;

    private Animator animator;
    private Rigidbody snakeBody;
    private ContactPoint contactPoint;

    [HideInInspector]
    public bool isHit = false;

    private void Start()
    {
        stars.SetActive(false);
        snakeBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        float hori = SimpleInput.GetAxis("Horizontal");

        if (!isHit)
        {
            animator.SetFloat("x", hori);
            animator.SetFloat("y", 0);
            transform.Translate(Vector3.forward * snakeSpeed * Time.deltaTime);
            transform.RotateAround(snakePivot.transform.position, Vector3.up, hori);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            isHit = true;
            contactPoint = collision.contacts[0];
            Vector3 dir = contactPoint.point - transform.position;
            //normalize and add a threshold
            dir = dir.normalized;
            // Reflects snake off the plane defined by a normal.
            snakeBody.velocity = Vector3.Reflect(dir * 5f, contactPoint.normal);
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
            animator.SetFloat("x", 0f);
            animator.SetFloat("y", 1f);
            stars.SetActive(true);
            StartCoroutine(knockBack());
        }
    }

    IEnumerator knockBack()
    {
        yield return new WaitForSeconds(2);
        stars.SetActive(false);
        isHit = false;
    }
}
