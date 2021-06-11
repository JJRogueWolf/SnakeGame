using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Width of the gound to be created
    [SerializeField]
    private int rows = 5;
    //Height of the gound to be created
    [SerializeField]
    private int columns = 8;

    [HideInInspector]
    public int groundCount = 0;
    [HideInInspector]
    public List<Transform> nonGreenGround = new List<Transform>();

    // Time to make food available in the ground
    [SerializeField]
    private float foodDisplaySecond = 8f;

    // Snake Movement speed
    [Header("Snake")]
    [SerializeField]
    private int snakeSpeed = 1;

    [Header("Camera")]
    public GameObject camera;

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

    private snake snakeObject;

    private bool isGamePause = false;
    [HideInInspector]
    public bool isfoodSpawned = false;

    private float timer = 0.0f;
    
    void Start()
    {
        // Total number of tiles needed
        groundCount = rows * columns;
        createGround();
        surroundWall();
        snake();
    }

    private void Update()
    {
        if(groundCount < 1)
        {
            // All the tile are now green
            gamePaused();
            uiManager.showEndScreen();
        }

        // get seconds value
        timer += Time.deltaTime;
        int seconds = (int)timer % 60;
        if (!isfoodSpawned && nonGreenGround.Count > 0)
        {
            // spawn food after 5 to 10 seconds for the previous food destroyed
            if (seconds % (foodDisplaySecond + Random.Range(5,10)) == 0)
            {
                //Destroy food after sometime
                StartCoroutine(destroyFood(spawnFood()));
            }
        }
    }

    private void createGround()
    {
        // Create an empty gameObject as holder 
        // for all the tiles created
        GameObject parentGround = new GameObject();
        parentGround.name = "GroundParent";
        parentGround.transform.position = Vector3.zero;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                // Creating tile as the a child initalised empty gameObject
                GameObject groundTile = Instantiate(groundPrefab, parentGround.transform);
                // placing the tile one after other in grid pattern.
                // size of tile is 1X1, so 0.5(1/2) is the position offset
                groundTile.transform.localPosition = new Vector3(column + positionOffset, 0, row + positionOffset);
                // Add the created tile to the list to make food spawn on it
                nonGreenGround.Add(groundTile.transform);
                groundTile.GetComponent<Ground>().gameManager = this;
            }
        }
        // calculate the height of the camera for grid to appear inside camera
        // properly without edge cutting off

        //Height is calculated by taking the longest side of the grid
        float cameraHeight = rows < columns ? columns : rows;
        if (cameraHeight < 4)
        {
            cameraHeight = 4;
        }
        camera.transform.localPosition = new Vector3(((float)columns / 2), cameraHeight, ((float)rows / 2));
    }

    private void surroundWall()
    {
        // Create an empty gameObject as holder 
        // for all the walls created
        GameObject parentWall = new GameObject();
        parentWall.name = "WallParent";
        parentWall.transform.position = Vector3.zero;

        // Creating left side wall 
        GameObject leftWall = Instantiate(wallPrefab, parentWall.transform);
        leftWall.transform.localPosition = new Vector3(-positionOffset/2, positionOffset, (float)rows / 2);
        leftWall.transform.localScale = new Vector3(positionOffset, 1, rows + 1);

        // Creating right side wall 
        GameObject rightWall = Instantiate(wallPrefab, parentWall.transform);
        rightWall.transform.localPosition = new Vector3(columns + (positionOffset/2) , positionOffset, (float)rows / 2);
        rightWall.transform.localScale = new Vector3(positionOffset, 1, rows + 1);


        // Creating top side wall 
        GameObject topWall = Instantiate(wallPrefab, parentWall.transform);
        topWall.transform.localPosition = new Vector3((float)columns / 2, positionOffset, rows + (positionOffset / 2));
        topWall.transform.localScale = new Vector3(columns + 1, 1, positionOffset);


        // Creating bottom side wall 
        GameObject bottomWall = Instantiate(wallPrefab, parentWall.transform);
        bottomWall.transform.localPosition = new Vector3((float)columns / 2, positionOffset, - (positionOffset / 2));
        bottomWall.transform.localScale = new Vector3(columns + 1, 1, positionOffset);
    }

    private void snake()
    {
        // spawn snake inside the grid
        GameObject snake = Instantiate(snakePrefab, new Vector3((float)columns / 2 + positionOffset, 0, (float)rows / 2 + positionOffset), Quaternion.Euler(new Vector3(0f, 90f, 0f)));
        snake.name = "Snake";
        snakeObject = snake.GetComponent<snake>();
        snakeObject.gameManager = this;
        // Set snake speed
        snakeObject.snakeSpeed = snakeSpeed;
        //snake.GetComponent<Snake>().joystick = controller;
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
        //spawn food in random places inside grid which is not green
        GameObject food = Instantiate(foodPrefab, nonGreenGround[Random.Range(0, nonGreenGround.Count)].transform.position, snakeObject.gameObject.transform.rotation);
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
