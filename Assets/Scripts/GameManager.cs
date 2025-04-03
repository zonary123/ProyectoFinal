using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Level Settings")] public int round;

    public int enemiesAlive;
    public int enemiesSpawned;
    public float maxLifeEnemy;

    public int highScore;

    public int score;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        if (score > highScore) highScore = score;
    }

    public void ResetScore()
    {
        score = 0;
    }
}