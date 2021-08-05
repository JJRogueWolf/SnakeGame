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

    public GameManager gameManager;

    private ContactPoint contactPoint;
    public bool isHit = false;
    private Vector3 reflectDirection;

    private List<GameObject> bodyParts = new List<GameObject>();
    private List<Vector3> lastPosition = new List<Vector3>();

    void Start()
    {
        bodySpeed = snakeSpeed;
    }
    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward);
        if (!gameManager.isGameOver && !gameManager.isGamePause)
        {
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
        GameObject body = Instantiate(snakeBody, transform.position, Quaternion.identity);
        bodyParts.Add(body);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wall")
        {
            contactPoint = other.contacts[0];
            reflectDirection = Vector3.Reflect(transform.forward, contactPoint.normal);
            isHit = true;
            StartCoroutine(SmoothRotate(reflectDirection));
        }

        if (other.gameObject.tag == "Food")
        {
            growBody();
            gameManager.PizzaCollect(other.transform.position, () =>
            {
                gameManager.isfoodSpawned = false;
                gameManager.uiManager.score += 1;
            });
            // Destroy collected Pizza
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Body")
        {
            gameManager.isGameOver = true;
        }
    }

    IEnumerator SmoothRotate(Vector3 reflectedDirection)
    {
        yield return new WaitForSeconds(1f);
        isHit = false;
    }
}
