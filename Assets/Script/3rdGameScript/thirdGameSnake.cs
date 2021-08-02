using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thirdGameSnake : MonoBehaviour
{
    [SerializeField]
    private GameObject snakePivot;
    
    [SerializeField]
    private AnimationCurve shakeCurve;

    [SerializeField]
    private GameObject stars;

    public Camera snakeCamera;

    [HideInInspector]
    public float snakeSpeed = 1f;

    [HideInInspector]
    public thirdGameManager gameManager;

    private Vector3 reflectDirection;

    private Animator animator;
    private Rigidbody snakeBody;
    private ContactPoint contactPoint;

    [HideInInspector]
    public bool isHit = false;

    private AudioSource audioSource;

    [Header("Audio")]
    [SerializeField]
    private AudioClip hitAudio;
    [SerializeField]
    private AudioClip eatAudio;

    private void Start()
    {
        stars.SetActive(false);
        snakeBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        float hori = SimpleInput.GetAxis("Horizontal") * 0.3f;

        if (!gameManager.isGamePause)
        {
            animator.SetFloat("x", hori);
            animator.SetFloat("y", 0);
            transform.Translate(Vector3.forward * snakeSpeed * Time.deltaTime);
            transform.RotateAround(snakePivot.transform.position, Vector3.up, hori);
        }
        else{
            
            animator.SetFloat("x", 0);
            animator.SetFloat("y", 1);
        }
        if (!isHit)
        {
            transform.RotateAround(snakePivot.transform.position, Vector3.up, hori);
        }
        else
        {
            snakeBody.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(reflectDirection), Time.deltaTime * 15f);
            snakeBody.rotation = Quaternion.Euler(new Vector3(0, snakeBody.rotation.eulerAngles.y, 0));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            audioSource.clip = hitAudio;
            audioSource.Play();
            StartCoroutine(shake(1, 10));
            contactPoint = collision.contacts[0];
            Vector3 dir = contactPoint.point - transform.position;
            //normalize and add a threshold
            dir = dir.normalized * 3f;
            // Reflects snake off the plane defined by a normal.
            reflectDirection = Vector3.Reflect(dir, contactPoint.normal);
            isHit = true;
            StartCoroutine(SmoothRotate(reflectDirection));
            snakeBody.velocity = reflectDirection;
        }

        if (collision.gameObject.tag == "Food")
        {
            audioSource.clip = eatAudio;
            audioSource.Play();
            gameManager.isfoodSpawned = false;
            gameManager.uiManager.score += 1;
            Destroy(collision.gameObject);
        }
    }

    // private void OnCollisionExit(Collision collision)
    // {
    //     if (collision.gameObject.tag == "Wall")
    //     {
    //         animator.SetFloat("x", 0f);
    //         animator.SetFloat("y", 1f);
    //         stars.SetActive(true);
    //         StartCoroutine(knockBack());
    //     }
    // }

    // IEnumerator knockBack()
    // {
    //     yield return new WaitForSeconds(2);
    //     stars.SetActive(false);
    //     isHit = false;
    // }

    
    IEnumerator SmoothRotate(Vector3 reflectedDirection)
    {
        yield return new WaitForSeconds(1.5f);
        isHit = false;
    }

    public IEnumerator shake(float duration, float magnitude)
    {
        Vector3 originalPos = gameManager._camera.GetComponent<Camera>().transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float strength = shakeCurve.Evaluate(elapsed / duration);
            gameManager._camera.GetComponent<Camera>().transform.position = originalPos + Random.insideUnitSphere * strength;
            elapsed += Time.deltaTime;
            yield return null;
        }

        gameManager._camera.GetComponent<Camera>().transform.localPosition = originalPos;
    }
}
