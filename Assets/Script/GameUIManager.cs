using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseScreen;
    
    [SerializeField]
    private GameObject endScreen;

    [SerializeField]
    private Text inGameScoreText;

    [SerializeField]
    private Text pauseScoreText;

    [SerializeField]
    private Text endScoreText;

    [HideInInspector]
    public int score = 0;

    private void Start()
    {
        // All the pause and end UIs
        hidePauseScreen();
        hideEndScreen();
    }

    private void Update()
    {
        // print score to text elements
        inGameScoreText.text = score.ToString();
        pauseScoreText.text = score.ToString();
        endScoreText.text = score.ToString();
    }

    public void showPauseScreen()
    {
        pauseScreen.SetActive(true);
    }

    public void hidePauseScreen()
    {
        pauseScreen.SetActive(false);
    }

    public void quitApp()
    {
        Application.Quit();
    }

    public void showEndScreen()
    {
        endScreen.SetActive(true);
    }

    public void hideEndScreen()
    {
        endScreen.SetActive(false);
    }

}
