using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snake : MonoBehaviour
{
    // to use the head of the snake as the pivot of the model
    [SerializeField]
    private GameObject snakePivot;

    [HideInInspector]
    public float snakeSpeed = 1f;

    [HideInInspector]
    public GameManager gameManager;

    private Animator animator;

    // Checks whether snake has hit wall
    [HideInInspector]
    public bool isHit = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        // Used a asset from asset store called "Simple Input"
        // Get the horizontal axis for the snake turn
        float hori = SimpleInput.GetAxis("Horizontal");
        // horizontal value is retrived by calculating the z rotation of the sprite

        if (!isHit)
        {
            // parameter in animator take float x and y to controll the blend space.
            animator.SetFloat("x", hori);
            animator.SetFloat("y", 0);
            // continuous forward movement of snake
            transform.Translate(Vector3.forward * snakeSpeed * Time.deltaTime);
            // turn the snake based on the horizontal value
            // got from the controller
            transform.RotateAround(snakePivot.transform.position, Vector3.up, hori);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            // boolean to confirm is hit and to stop snake
            isHit = true;
            // set animation value to 0
            animator.SetFloat("x", 0f);
            // Calculate the distance between collided objects for the direction
            Vector3 dir = collision.contacts[0].point - transform.position;
            // Get the opposite (-Vector3) and normalize it
            dir = -dir.normalized;
            // Adding force in the direction of dir and multiply it by force. 
            // This will push back the player
            GetComponent<Rigidbody>().AddForce(dir * 20f, ForceMode.Impulse);
        }

        if (collision.gameObject.tag == "Food")
        {
            gameManager.isfoodSpawned = false;
            // Add score of +1
            gameManager.uiManager.score += 1;
            // Destroy the food object
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            animator.SetFloat("y", 1f);
            StartCoroutine(continueMovement());   
        }
    }

    IEnumerator continueMovement()
    {
        yield return new WaitForSeconds(2);
        isHit = false;
    }
}
