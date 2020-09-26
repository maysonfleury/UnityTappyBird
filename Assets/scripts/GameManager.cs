using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance;

    public GameObject StartPage;
    public GameObject GameOverPage;
    public GameObject CountdownPage;
    public Text scoreText;

    enum PageState
    {
        None,
        Start,
        GameOver,
        Countdown
    }

    int score = 0;
    bool gameOver = true;

    public bool GameOver { get { return gameOver; } }
    public int Score { get { return score; } }

	void Awake () {
        Instance = this;
	}

    // Subscribe to Countdonwext and TapController events
    void OnEnable()
    {
        CountdownText.OnCountdownFinished += OnCountdownFinished;
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
    }

    // Unsubscribe to Countdonwext and TapController events
    void OnDisable()
    {
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
        TapController.OnPlayerDied -= OnPlayerDied;
        TapController.OnPlayerScored -= OnPlayerScored;
    }

    // When game start countdown is complete, set values for game start
    void OnCountdownFinished()
    {
        SetPageState(PageState.None);
        OnGameStarted();
        score = 0;
        gameOver = false;
    }

    // When the bird enters a DeadZone finish the game (and add new highscore if applicable)
    void OnPlayerDied()
    {
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("Highscore");
        if (score > savedScore)
        {
            PlayerPrefs.SetInt("Highscore", score);
        }
        SetPageState(PageState.GameOver);
    }

    // When the bird enters a ScoreZone, increase score by 1
    void OnPlayerScored()
    {
        score++;
        scoreText.text = score.ToString();
    }

    // Changes page state depending on curring state
    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                StartPage.SetActive(false);
                GameOverPage.SetActive(false);
                CountdownPage.SetActive(false);
                break;
            case PageState.Start:
                StartPage.SetActive(true);
                GameOverPage.SetActive(false);
                CountdownPage.SetActive(false);
                break;
            case PageState.GameOver:
                StartPage.SetActive(false);
                GameOverPage.SetActive(true);
                CountdownPage.SetActive(false);
                break;
            case PageState.Countdown:
                StartPage.SetActive(false);
                GameOverPage.SetActive(false);
                CountdownPage.SetActive(true);
                break;
        }
    }

    public void ConfirmGameOver()
    {
        // Activated when replay button is hit
        OnGameOverConfirmed(); // Event, resets game
        scoreText.text = "0";
        SetPageState(PageState.Start);
    }

    public void StartGame()
    {
        // Activated when playbutton is hit
        SetPageState(PageState.Countdown);
    }
}
