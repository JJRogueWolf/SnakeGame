using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelSnakeControlls : MonoBehaviour
{
    [SerializeField]
    public float snakeSpeed = 1f;
    public float bodySpeed = 1f;
    public float steeringSpeed = 90f;
    public int gap = 100;

    public GameObject snakeBody;
    [HideInInspector]
    public GameManager gameManager;

    private ContactPoint contactPoint;
    [HideInInspector]
    public bool isHit = false;
    private Vector3 reflectDirection;

    private List<GameObject> bodyParts = new List<GameObject>();
    private List<Vector3> lastPosition = new List<Vector3>();

    [SerializeField]
    private AnimationCurve shakeCurve;

    private AudioSource audioSource;

    [Header("Audio")]
    [SerializeField]
    private AudioClip hitAudio;
    [SerializeField]
    private AudioClip eatAudio;
    [SerializeField]
    private AudioClip loseAudio;

    void Start()
    {
        bodySpeed = snakeSpeed;
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward);
        if (!gameManager.isGameOver && !gameManager.isGamePause)
        {
            //Character movement
            transform.Translate(Vector3.forward * snakeSpeed * Time.deltaTime);
            float hori = SimpleInput.GetAxis("Horizontal");
            transform.Rotate(Vector3.up * hori * Time.deltaTime);

            if (!isHit)
            {
                // Rotate snake based on the custom pivot
                transform.Rotate(Vector3.up * steeringSpeed * hori * Time.deltaTime);
            }
            else
            {
                // Rotate snake away on hit with wall
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(reflectDirection), Time.deltaTime * 5f);
                // Keeping x and z axis of the rotation Zero
                transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
            }

            lastPosition.Insert(0, transform.position);
            //Body to follow the head
            int index = 0;
            foreach (var body in bodyParts)
            {
                Vector3 pos = lastPosition[Mathf.Clamp(index * gap, 0, lastPosition.Count - 1)];
                Vector3 moveDirection = pos - body.transform.position;
                body.transform.position += moveDirection * bodySpeed * Time.deltaTime;
                body.transform.LookAt(pos);
                index++;
            }
        }
    }

    void growBody(){
        //add new Body 
        GameObject body = Instantiate(snakeBody, transform.position, Quaternion.identity);
        bodyParts.Add(body);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wall")
        {
            // Play Hit audio
            audioSource.clip = hitAudio;
            audioSource.Play();
            // Camera shake animation
            StartCoroutine(shake(1, 10));
            contactPoint = other.contacts[0];
            //get reflect direction
            reflectDirection = Vector3.Reflect(transform.forward, contactPoint.normal);
            isHit = true;
            StartCoroutine(SmoothRotate(reflectDirection));
        }

        if (other.gameObject.tag == "Food")
        {
            // Play eat audio
            Destroy(other.gameObject);
            growBody();
            audioSource.clip = eatAudio;
            audioSource.Play();
            //boolean to enable next food spawn
            gameManager.isfoodSpawned = false;
            //increment score
            gameManager.uiManager.score += 1;
            //Scored collection effect
            gameManager.pizzaArtAnimator.SetBool("scored", true);
            StartCoroutine(Scored());
        }
        if (other.gameObject.tag == "Body")
        {
            gameManager.mainBgm.Stop();
            audioSource.clip = loseAudio;
            audioSource.Play();
            // Camera shake animation
            StartCoroutine(shake(1, 10));
            //ending game
            StartCoroutine(EndGame());

            gameManager.isGamePause = true;
        }
    }

    IEnumerator Scored()
    {
        yield return new WaitForSeconds(1);
        gameManager.pizzaArtAnimator.SetBool("scored", false);

    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(1);
        gameManager.isGameOver = true;

    }

    IEnumerator SmoothRotate(Vector3 reflectedDirection)
    {
        yield return new WaitForSeconds(1f);
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
