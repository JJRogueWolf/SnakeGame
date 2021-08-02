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
    private Text inGameScoreTextShadow;

    [SerializeField]
    private Text pauseScoreText;
    [SerializeField]
    private Text pauseScoreShadow;

    [SerializeField]
    private Text endScoreText;
    [SerializeField]
    private Text endScoreTextShadow;

    [HideInInspector]
    public int score = 0;

    private void Start()
    {
        hidePauseScreen();
        hideEndScreen();
    }

    private void Update()
    {
        // Displaying the scores on the Text and its shadow
        inGameScoreText.text = score.ToString();
        inGameScoreTextShadow.text = score.ToString();
        pauseScoreText.text = score.ToString() + " Pizza\nCollected";
        pauseScoreShadow.text = score.ToString() + " Pizza\nCollected";
        endScoreText.text = score.ToString();
        endScoreTextShadow.text = score.ToString();
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
