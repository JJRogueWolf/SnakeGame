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

    [HideInInspector]
    public bool isHit = false;

    private void Start()
    {
        stars.SetActive(false);
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
