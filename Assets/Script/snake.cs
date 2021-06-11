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

    [HideInInspector]
    public bool isHit = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        float hori = SimpleInput.GetAxis("Horizontal");

        //if(Mathf.Abs(hori)< 0.1f)
        //{
        //    hori = 0f;
        //}

        if (!isHit)
        {
            animator.SetFloat("x", hori);
            transform.Translate(Vector3.forward * snakeSpeed * Time.deltaTime);
            transform.RotateAround(snakePivot.transform.position, Vector3.up, hori);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            isHit = true;
            animator.SetFloat("x", 0f);
            Vector3 dir = collision.contacts[0].point - transform.position;
            // Get the opposite (-Vector3) and normalize it
            dir = -dir.normalized;
            // Adding force in the direction of dir and multiply it by force. 
            // This will push back the player
            GetComponent<Rigidbody>().AddForce(dir * 50f, ForceMode.Impulse);
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
