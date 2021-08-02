using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snake : MonoBehaviour
{
    [SerializeField]
    private GameObject snakePivot;

    [SerializeField]
    private AnimationCurve shakeCurve;

    [HideInInspector]
    public float snakeSpeed = 1f;

    [HideInInspector]
    public GameManager gameManager;

    private Animator animator;
    private Rigidbody snakeBody;
    private ContactPoint contactPoint;

    private Transform collidedTransform;

    private Vector3 reflectDirection;

    [HideInInspector]
    public bool isHit = false;

    private Vector3 lastVelocity;

    private AudioSource audioSource;

    [Header("Audio")]
    [SerializeField]
    private AudioClip hitAudio;
    [SerializeField]
    private AudioClip eatAudio;
   
    private void Start()
    {
        snakeBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
   
    void Update()
    {
        // Steering control
        float hori = SimpleInput.GetAxis("Horizontal") * 0.3f;
        // Animation for snake
        animator.SetFloat("x", hori);
        animator.SetFloat("y", 0);
        if (!gameManager.isGamePause)
        {
            // Move snake if game is not paused
            transform.Translate(Vector3.forward * snakeSpeed * Time.deltaTime);
        }
        else {
            // stop the animation if the game is paused
            animator.SetFloat("x", 0);
            animator.SetFloat("y", 1);
        }
        
        if (!isHit)
        {
            // Rotate snake based on the custom pivot
            transform.RotateAround(snakePivot.transform.position, Vector3.up, hori);
        } else
        {
            // Rotate snake away on hit with wall
            snakeBody.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(reflectDirection), Time.deltaTime * 15f);
            // Keeping x and z axis of the rotation Zero
            snakeBody.rotation = Quaternion.Euler(new Vector3(0, snakeBody.rotation.eulerAngles.y, 0));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            // Play Hit audio
            audioSource.clip = hitAudio;
            audioSource.Play();
            // Camera shake animation
            StartCoroutine(shake(1, 10));
            contactPoint = collision.contacts[0];
            // Calculate the contact direction
            Vector3 dir = contactPoint.point - transform.position;
            //normalize
            dir = dir.normalized;
            // Reflects snake off the plane defined by a normal.
            reflectDirection = Vector3.Reflect(dir, contactPoint.normal);
            isHit = true;
            StartCoroutine(SmoothRotate(reflectDirection));
            snakeBody.velocity = reflectDirection;
        }

        if (collision.gameObject.tag == "Food")
        {
            // Play eat audio
            audioSource.clip = eatAudio;
            audioSource.Play();
            // Pizza collection animation
            gameManager.PizzaCollect(collision.transform.position, () => {
                gameManager.isfoodSpawned = false;
                gameManager.uiManager.score += 1;
            });
            // Destroy collected Pizza
            Destroy(collision.gameObject);
        }
    }

    IEnumerator SmoothRotate(Vector3 reflectedDirection)
    {
        yield return new WaitForSeconds(1.5f);
        isHit = false;
    }

    public IEnumerator shake(float duration, float magnitude)
    {
        // Backing up position of the camera
        Vector3 originalPos = gameManager._camera.GetComponent<Camera>().transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Geting the strength using the animation curve
            float strength = shakeCurve.Evaluate(elapsed / duration);
            gameManager._camera.GetComponent<Camera>().transform.position = originalPos + Random.insideUnitSphere * strength;
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Assigning the default camera position
        gameManager._camera.GetComponent<Camera>().transform.localPosition = originalPos;
    }
}
