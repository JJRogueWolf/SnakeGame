using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleInputNamespace;

public class thirdGameManager : MonoBehaviour
{

    public static thirdGameManager Instance { get; private set; }

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

    [SerializeField]
    private float foodDisplaySecond = 8f;

    [SerializeField]
    public GameObject pizzaIconTarget;

    [Header("Camera")]
    [HideInInspector]
    public GameObject _camera;
    public GameObject mapCamera;

    [Header("Snake")]
    [SerializeField]
    private int snakeSpeed = 1;

    [Header("Controller")]
    [SerializeField]
    private bool shouldRotateSmoothToZero = false;
    [SerializeField]
    private GameObject controller;

    [Header("Canvas")]
    [SerializeField]
    private Canvas canvas;

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

    private float positionOffset = 0.2f;

    private thirdGameSnake snakeObject;

    [HideInInspector]
    public bool isGamePause = false;
    [HideInInspector]
    public bool isfoodSpawned = false;

    private float timer = 0.0f;
    
    void Start()
    {
        groundCount = rows * columns;
        createGround();
        surroundWall();
        snake();
        controller.GetComponent<SteeringWheel>().shouldWheelResetClip = shouldRotateSmoothToZero;
    }

    private void Update()
    {
        if(groundCount < 1)
        {
            gamePaused();
            uiManager.showEndScreen();
        }
        timer += Time.deltaTime;
        int seconds = (int)timer % 60;
        if (!isfoodSpawned && nonGreenGround.Count > 2)
        {
            if (seconds % (foodDisplaySecond + UnityEngine.Random.Range(5,10)) == 0)
            {
                StartCoroutine(destroyFood(spawnFood()));
            }
        }
    }

    private void createGround()
    {
        GameObject parentGround = new GameObject();
        parentGround.name = "GroundParent";
        parentGround.transform.position = Vector3.zero;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GameObject groundTile = Instantiate(groundPrefab, parentGround.transform);
                groundTile.transform.localPosition = new Vector3(column + positionOffset, 0, row + positionOffset);
                nonGreenGround.Add(groundTile.transform);
                if (row != 0 || row != rows - 1)
                {
                    if (column != 0 || column != columns - 1)
                    {
                        foodSpawnableArea.Add(groundTile.transform);
                    }
                }
                groundTile.GetComponent<thirdGameGround>().gameManager = this;
            }
        }
        
        float cameraHeight = Mathf.Max(rows,columns) * 1.6f;
        if (cameraHeight < 4)
        {
            cameraHeight = 4;
        }
        
        mapCamera.transform.localPosition = new Vector3(((float)columns / 2) - positionOffset * 1.5f, cameraHeight, (float)rows / 2f);

    }

    private void surroundWall()
    {
        GameObject parentWall = new GameObject();
        parentWall.name = "WallParent";
        parentWall.transform.position = Vector3.zero;

        GameObject leftWall = Instantiate(wallPrefab, parentWall.transform);
        leftWall.transform.localPosition = new Vector3(-positionOffset * 1.5f, positionOffset / 2, ((float)rows / 2) - positionOffset * 1.5f);
        leftWall.transform.localScale = new Vector3(positionOffset, 0.2f, rows + 0.2f);

        GameObject rightWall = Instantiate(wallPrefab, parentWall.transform);
        rightWall.transform.localPosition = new Vector3(columns - positionOffset * 1.5f, positionOffset / 2, ((float)rows / 2) - positionOffset * 1.5f);
        rightWall.transform.localScale = new Vector3(positionOffset, 0.2f, rows + 0.2f);

        GameObject topWall = Instantiate(wallPrefab, parentWall.transform);
        topWall.transform.localPosition = new Vector3(((float)columns / 2) - positionOffset * 1.5f, positionOffset / 2, rows - positionOffset * 1.5f);
        topWall.transform.localScale = new Vector3(columns + 0.2f, 0.2f, positionOffset);

        GameObject bottomWall = Instantiate(wallPrefab, parentWall.transform);
        bottomWall.transform.localPosition = new Vector3(((float)columns / 2) - positionOffset * 1.5f, positionOffset / 2, -(positionOffset * 1.5f));
        bottomWall.transform.localScale = new Vector3(columns + 0.2f, 0.2f, positionOffset);
    }

    private void snake()
    {
        GameObject snake = Instantiate(snakePrefab, new Vector3((float)columns / 2 + positionOffset, 0, (float)rows / 2 + positionOffset), Quaternion.Euler(new Vector3(0f, 90f, 0f)));
        snake.name = "Snake";
        snakeObject = snake.GetComponent<thirdGameSnake>();
        snakeObject.gameManager = this;
        snakeObject.snakeSpeed = snakeSpeed;

        canvas.worldCamera = snakeObject.snakeCamera;
        _camera = snakeObject.snakeCamera.gameObject;
        canvas.planeDistance = 0.4f;
        //snake.GetComponent<Snake>().joystick = controller;
    }

    public void gamePaused()
    {
        isGamePause = true;
    }

    public void gameResumed()
    {
        isGamePause = false;
    }

    private GameObject spawnFood() {
        GameObject food = Instantiate(foodPrefab, foodSpawnableArea[UnityEngine.Random.Range(0, foodSpawnableArea.Count)].transform.position, Quaternion.identity);
        isfoodSpawned = true;
        return food;
    }

    IEnumerator destroyFood(GameObject objectToDestroy)
    {
        yield return new WaitForSeconds(foodDisplaySecond);
        if (objectToDestroy != null)
        {
            Destroy(objectToDestroy);
            isfoodSpawned = false;
        }
    }
}
