using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Level Settings")] public int round;
    public int enemiesAlive;
    public int enemiesSpawned;
    [Header("Level Enemy")] public int playerNumber;
    public float minLifeEnemy = 100f;
    public float maxLifeEnemy = 500f;
    public float minCooldownDamage = 10f;
    public float maxCooldownDamage = 20f;
    public float minCooldownSpawn = 0.5f;
    public float maxCooldownSpawn = 2f;

    public int score;
    public int highScore;
    public bool isDebug = true;
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

    public float GetCooldownDamage()
    {
        return Mathf.Lerp(maxCooldownDamage, minCooldownDamage, GetRoundProgress());
    }

    public float GetCooldownSpawn()
    {
        return Mathf.Lerp(maxCooldownSpawn, minCooldownSpawn, GetRoundProgress());
    }

    public float GetMaxLifeEnemy()
    {
        return Mathf.Lerp(maxLifeEnemy, minLifeEnemy, GetRoundProgress());
    }

    private float GetRoundProgress()
    {
        // Assuming round starts at 1 and progresses indefinitely
        // Adjust the divisor to control how quickly the values approach the minimum
        return Mathf.Clamp01((float)round / 100);
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