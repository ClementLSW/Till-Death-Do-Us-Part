using UnityEngine;

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
        PlayerPrefs.SetInt("Score", gm.Score);
        PlayerPrefs.SetInt("CurrentDialogID", FindObjectOfType<DialogManager>().CurrentDialogID);
        // Implement save logic here
        Debug.Log("Game saved.");
    }

    public void LoadGame()
    {
        gm.LoadScore(PlayerPrefs.GetInt("Score", 0));
        dm.SetDialogue(PlayerPrefs.GetInt("CurrentDialogID", 1));
    }
}
