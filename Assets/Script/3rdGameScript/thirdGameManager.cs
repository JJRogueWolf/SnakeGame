using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class thirdGameManager : MonoBehaviour
{
    [SerializeField]
    private int rows = 5;
    [SerializeField]
    private int columns = 8;
    [HideInInspector]
    public int groundCount = 0;
    [HideInInspector]
    public List<Transform> nonGreenGround = new List<Transform>();

    [SerializeField]
    private float foodDisplaySecond = 8f;

    [Header("Snake")]
    [SerializeField]
    private int snakeSpeed = 1;

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

    private float positionOffset = 0.5f;

    private thirdGameSnake snakeObject;

    private bool isGamePause = false;
    [HideInInspector]
    public bool isfoodSpawned = false;

    private float timer = 0.0f;
    
    void Start()
    {
        groundCount = rows * columns;
        createGround();
        surroundWall();
        snake();
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
        print(seconds);
        if (!isfoodSpawned && nonGreenGround.Count > 0)
        {
            if (seconds % (foodDisplaySecond + Random.Range(5,10)) == 0)
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
                groundTile.GetComponent<thirdGameGround>().gameManager = this;
            }
        }
    }

    private void surroundWall()
    {
        GameObject parentWall = new GameObject();
        parentWall.name = "WallParent";
        parentWall.transform.position = Vector3.zero;

        GameObject leftWall = Instantiate(wallPrefab, parentWall.transform);
        leftWall.transform.localPosition = new Vector3(-positionOffset/2, positionOffset, (float)rows / 2);
        leftWall.transform.localScale = new Vector3(positionOffset, 1, rows + 1);

        GameObject rightWall = Instantiate(wallPrefab, parentWall.transform);
        rightWall.transform.localPosition = new Vector3(columns + (positionOffset/2) , positionOffset, (float)rows / 2);
        rightWall.transform.localScale = new Vector3(positionOffset, 1, rows + 1);

        GameObject topWall = Instantiate(wallPrefab, parentWall.transform);
        topWall.transform.localPosition = new Vector3((float)columns / 2, positionOffset, rows + (positionOffset / 2));
        topWall.transform.localScale = new Vector3(columns + 1, 1, positionOffset);

        GameObject bottomWall = Instantiate(wallPrefab, parentWall.transform);
        bottomWall.transform.localPosition = new Vector3((float)columns / 2, positionOffset, - (positionOffset / 2));
        bottomWall.transform.localScale = new Vector3(columns + 1, 1, positionOffset);
    }

    private void snake()
    {
        GameObject snake = Instantiate(snakePrefab, new Vector3((float)columns / 2 + positionOffset, 0, (float)rows / 2 + positionOffset), Quaternion.Euler(new Vector3(0f, 90f, 0f)));
        snake.name = "Snake";
        snakeObject = snake.GetComponent<thirdGameSnake>();
        snakeObject.gameManager = this;
        snakeObject.snakeSpeed = snakeSpeed;

        canvas.worldCamera = snakeObject.snakeCamera;
        canvas.planeDistance = 0.4f;
    }

    public void gamePaused()
    {
        isGamePause = true;

        if (snakeObject != null)
        {
            snakeObject.isHit = true;
        }
    }

    public void gameResumed()
    {
        isGamePause = false;
        if (snakeObject != null)
        {
            snakeObject.isHit = false;
        }
    }

    private GameObject spawnFood() {
        GameObject food = Instantiate(foodPrefab, nonGreenGround[Random.Range(0, nonGreenGround.Count)].transform.position, Quaternion.identity);
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
