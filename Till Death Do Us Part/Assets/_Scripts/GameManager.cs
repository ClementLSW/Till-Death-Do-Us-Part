using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int Score { get; private set; } = 0;

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
        
    }
}
