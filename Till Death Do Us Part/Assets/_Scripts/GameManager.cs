using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused
    }

    public GameState CurrentGameState { get; private set; } = GameState.MainMenu;
    public int Score { get; private set; } = 0;
    public DayOfWeek.Day CurrentDay { get; set; } = DayOfWeek.Day.Monday;
    SaveLoad saveLoad;

    public void AutoSave()
    {
        saveLoad.SaveGame();
    }

    public void AddScore(int amount)
    {
        Score += amount;
        Debug.Log($"Score updated: {Score}");
    }

    public void ResetScore()
    {
        Score = 0;
        Debug.Log("Score reset to 0");
    }

    public void LoadScore(int value)
    {
        Score = value;
    }

    private void Awake()
    {
        saveLoad = GetComponent<SaveLoad>();
    }

    private void Start()
    {
        LoadScore(PlayerPrefs.GetInt("Score", 0));
        GetComponentInChildren<DialogManager>().SetDialogue(PlayerPrefs.GetString("CurrentDialogID", "mon001"));
        if (System.Enum.TryParse(PlayerPrefs.GetString("CurrentDay", "Monday"), out DayOfWeek.Day day))
        {
            CurrentDay = day;
        }
        else
        {
            CurrentDay = DayOfWeek.Day.Monday; // Default to Monday if parsing fails
        }
    }

    // TODO: Set Endings
    public int EvaluateEnding()
    {
        int tempScore = Score;
        ResetScore();
        return tempScore;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && CurrentGameState != GameState.MainMenu)
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (CurrentGameState == GameState.Playing)
        {
            CurrentGameState = GameState.Paused;
            Time.timeScale = 0f; // Pause the game
            Debug.Log("Game Paused");
        }
        else if (CurrentGameState == GameState.Paused)
        {
            CurrentGameState = GameState.Playing;
            Time.timeScale = 1f; // Resume the game
            Debug.Log("Game Resumed");
        }
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
