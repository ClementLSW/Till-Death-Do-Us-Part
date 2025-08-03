using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    // TODO: Set Endings
    public int EvaluateEnding()
    {
        int tempScore = Score;
        ResetScore();
        return tempScore;
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
