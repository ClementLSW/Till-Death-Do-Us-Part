using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoad : MonoBehaviour
{
    private DialogManager dm;
    private GameManager gm;

    private void Awake()
    {
        dm = GetComponentInChildren<DialogManager>();
        gm = GetComponent<GameManager>();

        if (dm == null)
        {
            Debug.LogError("DialogManager not found in the scene.");
        }
        if (gm == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }
    }

    public void SaveGame()
    {
        if (gm == null || dm == null)
        {
            Debug.LogError("Unable to save, GM or DM is null. Are you trying to save the game on the menu screen?");
            return;
        }
        PlayerPrefs.SetInt("Score", gm.Score);
        PlayerPrefs.SetString("CurrentDialogID", dm.CurrentDialogID);
        PlayerPrefs.SetString("CurrentDay", gm.CurrentDay.ToString());
        Debug.Log("Game saved.");
    }

    public void LoadGame()
    {
        /*gm.LoadScore(PlayerPrefs.GetInt("Score", 0));
        dm.SetDialogue(PlayerPrefs.GetString("CurrentDialogID", "mon001"));
        if (System.Enum.TryParse(PlayerPrefs.GetString("CurrentDay", "Monday"), out DayOfWeek.Day day))
        {
            gm.CurrentDay = day;
        }
        else
        {
            gm.CurrentDay = DayOfWeek.Day.Monday; // Default to Monday if parsing fails
        }*/
        SceneManager.LoadScene("GameplayScene");
    }

    public void ResetGame()
    {
        PlayerPrefs.DeleteKey("Score");
        PlayerPrefs.DeleteKey("CurrentDialogID");
        PlayerPrefs.DeleteKey("CurrentDay");
    }

    public void StartNewGame()
    {
        ResetGame();
        SceneManager.LoadScene("GameplayScene");
    }
}
