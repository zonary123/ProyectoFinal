using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Weapons;

public class GameManager : MonoBehaviour{
	public Player player;

	[Header("Level Settings")] public int round;
	public int enemiesAlive;
	public int enemiesSpawned;
	public int totalEnemiesToKill = 10;

	[Header("Level Enemy")] public float minLifeEnemy = 100f;
	public float maxLifeEnemy = 500f;
	public float minCooldownDamage = 10f;
	public float maxCooldownDamage = 20f;
	public float minCooldownSpawn = 2f;
	public float maxCooldownSpawn = 5f;
	public float spawnCooldown = 2f;

	[Header("Start Settings")] public Camera playerCamera;
	public Camera startCamera;
	public GameObject startPanel;

	[Header("UI Settings")] public GameObject gameOverPanel;
	public GameObject canvas;
	public GameObject statsUI;
	public GameObject pauseUI;
	public TMP_Text scoreText;
	public TMP_Text roundText;
	public TMP_Text ammoText;
	public TMP_Text relaodAmmoText;
	public int score;
	public int highScore;

	[Header("Debug Settings")] public bool isDebug = true;

	[Header("Spawner Settings")] public List<Spawner> spawners; // Lista de spawners para los zombies

	private float cachedCooldownDamage;
	private float cachedCooldownSpawn;
	private float cachedMaxLifeEnemy;

	private float roundProgress;

	public static GameManager Instance{ get; private set; }

	private void Awake(){
		if (Instance == null){
			Instance = this;
			DontDestroyOnLoad(gameObject);
			DontDestroyOnLoad(canvas);
		}
		else{
			Destroy(gameObject);
		}

		UpdateRoundParameters();
		Time.timeScale = 0f;
	}

	private void FixedUpdate(){
		if (Input.GetKeyDown(KeyCode.Escape)){
			if (Time.timeScale == 1f)
				Pause();
			else
				Resume();
		}

		if (CanSpawnEnemy() && spawners.Count > 0)
			foreach (var spawner in spawners){
				spawner.Tick(Time.deltaTime);

				if (spawner.ShouldSpawn(player.transform)){
					Instantiate(Resources.Load("Zombis/Zombi_Normal"), spawner.transform.position, Quaternion.identity);
					OnEnemySpawned();
					spawner.ResetCooldown();
					break; // Importante: solo permitimos 1 spawn por frame
				}
			}
	}

	public void StartGame(){
		startCamera.gameObject.SetActive(false);
		playerCamera.gameObject.SetActive(true);
		startPanel.SetActive(false);
		statsUI.SetActive(true);
		Cursor.lockState = CursorLockMode.Locked;
		Time.timeScale = 1f;
	}

	private void Pause(){
		if (isDebug) Debug.Log("Juego en pausa.");
		Time.timeScale = 0f; // Pausa el juego
	}

	private void Resume(){
		if (isDebug) Debug.Log("Juego reanudado.");
		Time.timeScale = 1f; // Reanuda el juego
	}

	private void NextRound(){
		round++;
		enemiesAlive = 0;
		enemiesSpawned = 0;
		totalEnemiesToKill = Mathf.FloorToInt(10 * round); // Escala la dificultad
		UpdateRoundParameters();
		UpdateRoundText();
		if (round > 5) Winning();
	}

	private void Winning(){
		if (isDebug) Debug.Log("You won!");
		Time.timeScale = 0f;
		GameOver();
	}

	public void GameOver(){
		gameOverPanel.SetActive(true);
		StartCoroutine(nameof(ReloadGame));
	}

	private IEnumerator ReloadGame(){
		yield return new WaitForSeconds(4f);

		// Buscar todos los objetos en la escena DontDestroyOnLoad
		var dontDestroyObjects = new List<GameObject>();
		var allObjects = FindObjectsOfType<GameObject>();
		foreach (var obj in allObjects)
			if (obj.scene.name == "DontDestroyOnLoad")
				dontDestroyObjects.Add(obj);

		// Destruir los objetos encontrados
		foreach (var obj in dontDestroyObjects) Destroy(obj);

		// Recargar la escena
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		Cursor.lockState = CursorLockMode.None;
	}

	private void UpdateRoundParameters(){
		roundProgress = Mathf.Clamp01((float)round / 100);
		cachedMaxLifeEnemy = Mathf.RoundToInt(Mathf.Lerp(minLifeEnemy, maxLifeEnemy, roundProgress));
		cachedCooldownDamage = Mathf.RoundToInt(Mathf.Lerp(maxCooldownDamage, minCooldownDamage, roundProgress));
		cachedCooldownSpawn = Mathf.RoundToInt(Mathf.Lerp(maxCooldownSpawn, minCooldownSpawn, roundProgress));
	}

	private void UpdateRoundText(){
		if (roundText != null)
			roundText.text = "Round: " + round;
		else
			Debug.LogError("Texto de ronda no asignado en el inspector.");
	}

	private void UpdateScoreText(){
		if (scoreText != null)
			scoreText.text = "Score: " + score;
		else
			Debug.LogError("Texto de puntuación no asignado en el inspector.");
	}

	public void UpdateAmmoText(Weapon weapon){
		if (ammoText != null)
			ammoText.text = weapon.actualAmmo + " / " + weapon.maxAmmo;
		else
			Debug.LogError("Texto de munición no asignado en el inspector.");
	}

	public float GetCooldownDamage(){
		return cachedCooldownDamage;
	}

	public float GetCooldownSpawn(){
		return cachedCooldownSpawn;
	}

	public float GetMaxLifeEnemy(){
		return cachedMaxLifeEnemy;
	}

	public void AddScore(int points){
		score += points;
		if (score > highScore) highScore = score;
		UpdateScoreText();
	}

	public void ResetScore(){
		score = 0;
		UpdateScoreText();
	}

	public void OnEnemySpawned(){
		enemiesSpawned++;
		enemiesAlive++;
	}

	public void OnEnemyKilled(int reward){
		enemiesAlive--;
		AddScore(reward);

		if (enemiesSpawned >= totalEnemiesToKill && enemiesAlive <= 0){
			if (isDebug) Debug.Log("All enemies defeated. Proceeding to next round.");
			NextRound();
		}
	}

	public bool CanSpawnEnemy(){
		return enemiesSpawned < totalEnemiesToKill;
	}
}