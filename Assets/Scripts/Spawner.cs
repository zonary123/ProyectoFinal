using UnityEngine;

public class Spawner : MonoBehaviour{
	public float spawnCooldown = 2f; // Cooldown inicial
	private float cooldownTimer;
	private SpriteRenderer spriteRenderer;

	private void Start(){
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.enabled = GameManager.Instance.isDebug;
		cooldownTimer = spawnCooldown;
	}

	public void Tick(float deltaTime){
		if (cooldownTimer > 0) cooldownTimer -= deltaTime;
	}

	public bool ShouldSpawn(Transform playerTransform){
		// Verifica si el cooldown ha terminado
		return cooldownTimer <= 0;
	}

	public void ResetCooldown(){
		cooldownTimer = spawnCooldown;
	}
}