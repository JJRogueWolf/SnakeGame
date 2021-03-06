using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleInputNamespace;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int rows = 5;
    [SerializeField]
    private int columns = 8;
    [HideInInspector]
    public int groundCount = 0;
    [HideInInspector]
    public List<Transform> nonGreenGround = new List<Transform>();
    [HideInInspector]
    public List<Transform> foodSpawnableArea = new List<Transform>(); 
    
    [Header("Food")]
    [SerializeField]
    private float foodDisplaySecond = 8f;

    [SerializeField]
    public GameObject pizzaIconTarget;
    [HideInInspector]
    public Animator pizzaArtAnimator;

    [Header("Snake")]
    [SerializeField]
    private int snakeSpeed = 1;

    [Header("Controller")]
    [SerializeField]
    private bool shouldRotateSmoothToZero = false;
    [SerializeField]
    private GameObject controller;

    [Header("Camera")]
    public GameObject _camera;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject groundPrefab;
    [SerializeField]
    private GameObject wallPrefab;
    [SerializeField]
    private GameObject snakePrefab;
    [SerializeField]
    private GameObject foodPrefab;

    [Header("UI")]
    public GameUIManager uiManager;

    [Space]
    [Header("Audio")]
    public AudioSource mainBgm;

    private float positionOffset = 0.5f;
    private float scaleOffset = 0.5f;

    private PixelSnakeControlls snakeObject;

    [HideInInspector]
    public bool isGamePause = false;
    [HideInInspector]
    public bool isfoodSpawned = false;

    [HideInInspector]
    public bool isGameOver = false;

    private float timer = 0.0f;
    
    
    void Start()
    {
        // Total count of grid
        groundCount = rows * columns;
        // Generate Ground
        createGround();
        // Generate Surrounding Wall
        surroundWall();
        // Spawn snake
        snake();

        pizzaArtAnimator = pizzaIconTarget.GetComponent<Animator>();

        controller.GetComponent<SteeringWheel>().shouldWheelResetClip = shouldRotateSmoothToZero;
    }

    private void Update()
    {
        if (isGameOver)
        {
            // Game Ended
            gamePaused();
            uiManager.showEndScreen();
        }

        // Spawing food
        timer += Time.deltaTime;
        int seconds = (int)timer % 60;
        if (!isfoodSpawned && nonGreenGround.Count > 2)
        {
            if (seconds % (foodDisplaySecond + UnityEngine.Random.Range(5, 10)) == 0)
            {
                if (foodSpawnableArea.Count > 0)
                {
                    // Coroutine to destroy the food after the given time
                    StartCoroutine(destroyFood(spawnFood()));
                }
            }
        }
    }

    private void createGround()
    {
        // Create an empty Gameobject to hold tiles created 
        GameObject parentGround = new GameObject();
        parentGround.name = "GroundParent";
        parentGround.transform.position = Vector3.zero;

        // Loop to spawn each title
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                // Spawning tile prefab under empty ground object
                GameObject groundTile = Instantiate(groundPrefab, parentGround.transform);
                // positioning the spawned tile with the row and column with an offset since the tile pivot is at the center of the object
                groundTile.transform.localPosition = new Vector3(column + positionOffset, 0, row + positionOffset);
                // Adding the the spawned title into a list to check and confirm the wether the generated tile is turned green or not
                nonGreenGround.Add(groundTile.transform);
                if (row != 0 && row != rows - 1)
                {
                    if(column != 0 && column != columns - 1)
                    {
                        // Creating a list as a referncing for food to spawn
                        // adding the tiles which is not on the edges to avoid the collision with the wall
                        foodSpawnableArea.Add(groundTile.transform);
                    }
                }
                groundTile.GetComponent<Ground>().gameManager = this;
            }
        }

        // Adjusting the Height of the camera to fit the grid
        // Gets the greatest between row and height
        float cameraHeight = Mathf.Max(rows,columns);
        if (cameraHeight < 4)
        {
            // For height below 4, the camera height is kept to 4
            cameraHeight = 4;
            // adjusting the plane distance for UI canvas
            
        }
        uiManager.gameObject.GetComponent<Canvas>().planeDistance = (cameraHeight / 2);
        _camera.transform.localPosition = new Vector3(((float)columns / 2), cameraHeight, ((float)rows / 2f));

    }
    
    //  spawning walls around the grid with offset 
    private void surroundWall()
    {
        GameObject parentWall = new GameObject();
        parentWall.name = "WallParent";
        parentWall.transform.position = Vector3.zero;

        GameObject leftWall = Instantiate(wallPrefab, parentWall.transform);
        leftWall.transform.localPosition = new Vector3(0, positionOffset / 2, ((float)rows/2) + (scaleOffset / 2));
        leftWall.transform.localScale = new Vector3(scaleOffset, scaleOffset, rows + scaleOffset);

        GameObject rightWall = Instantiate(wallPrefab, parentWall.transform);
        rightWall.transform.localPosition = new Vector3(columns + (scaleOffset/2), positionOffset / 2, ((float)rows / 2));
        rightWall.transform.localScale = new Vector3(scaleOffset, scaleOffset, rows + scaleOffset);

        GameObject topWall = Instantiate(wallPrefab, parentWall.transform);
        topWall.transform.localPosition = new Vector3(((float)columns / 2) + (scaleOffset / 2), positionOffset / 2, rows + (scaleOffset / 2));
        topWall.transform.localScale = new Vector3(columns + scaleOffset, scaleOffset, scaleOffset);

        GameObject bottomWall = Instantiate(wallPrefab, parentWall.transform);
        bottomWall.transform.localPosition = new Vector3(((float) columns/2), positionOffset / 2, 0);
        bottomWall.transform.localScale = new Vector3(columns + scaleOffset, scaleOffset, scaleOffset);
    }

    // Spawning Snake to the center of the grid
    private void snake()
    {
        GameObject snake = Instantiate(snakePrefab, new Vector3((float)columns / 2 + positionOffset, 0.5f, (float)rows / 2 + positionOffset), Quaternion.Euler(new Vector3(0f, 90f, 0f)));
        snake.name = "Snake";
        snakeObject = snake.GetComponent<PixelSnakeControlls>();
        snakeObject.gameManager = this;
        snakeObject.snakeSpeed = snakeSpeed;
    }

    public void gamePaused()
    {
        isGamePause = true;
    }

    public void gameResumed()
    {
        isGamePause = false;
    }

    // Spawn food at random position based on the the lsit created which does not have edge tiles. 
    private GameObject spawnFood() {
        GameObject food = Instantiate(foodPrefab, foodSpawnableArea[UnityEngine.Random.Range(0, foodSpawnableArea.Count)].transform.position, Quaternion.identity);
        isfoodSpawned = true;
        return food;
    }

    // Destroy the spawned food
    IEnumerator destroyFood(GameObject objectToDestroy)
    {
        yield return new WaitForSeconds(foodDisplaySecond);
        if (objectToDestroy != null)
        {
            Destroy(objectToDestroy);
            isfoodSpawned = false;
        }
    }

    // Spawn dummy food prefab for the collecting animation 
    public void PizzaCollect(Vector3 currentposition, Action onComplete)
    {
        Vector3 targetPostion = pizzaIconTarget.transform.position;
        GameObject _pizza = Instantiate(foodPrefab, transform);
        _pizza.GetComponent<BoxCollider>().enabled = false;
        _pizza.GetComponent<Animator>().enabled = false;

        StartCoroutine(ScalePizza(_pizza.transform, currentposition, targetPostion, onComplete));
    }

    // Scaling the pizza
    IEnumerator ScalePizza(Transform pizza, Vector3 startPosition, Vector3 endPosition, Action onComplete)
    {
        float height = startPosition.y + 5f;
        Vector3 heightVector = new Vector3(startPosition.x, height, startPosition.z);
        float time = 0;
        while (time < 0.3f)
        {
            time += 0.5f * Time.deltaTime;
            pizza.position = startPosition;            
            pizza.position = Vector3.Lerp(startPosition, heightVector, time);
            pizza.localScale = Vector3.Lerp(pizza.localScale, new Vector3(3f, 3f, 3f), time);
            yield return new WaitForEndOfFrame();

        }
        yield return null;
        startPosition = heightVector;
        StartCoroutine(MovePizza(pizza, startPosition, endPosition, onComplete));
    }
    // Moving the pizza to the point section animation
    IEnumerator MovePizza(Transform pizza, Vector3 startPosition, Vector3 endPosition, Action onComplete)
    {
        float time = 0;
        while ( time < 1)
        {
            time += 2 * Time.deltaTime;
            pizza.position = Vector3.Lerp(startPosition, endPosition, time);
            pizza.localScale = Vector3.Lerp(pizza.localScale, new Vector3(1f, 1f, 1f), time * 0.3f);
            yield return new WaitForEndOfFrame();
        }
        onComplete.Invoke();
        Destroy(pizza.gameObject);
    }

    public IEnumerator shake(float duration)
    {
        Vector3 originalPos = pizzaIconTarget.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        pizzaIconTarget.transform.localPosition = originalPos;
    }
}
