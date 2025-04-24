using UnityEngine;
using UnityEngine.AI;

namespace DefaultNamespace{
	[RequireComponent(typeof(NavMeshAgent))]
	public class Enemy : MonoBehaviour{
		[SerializeField] private Animator animator;
		[SerializeField] private NavMeshAgent agent;
		[SerializeField] private float distanceDamage = 1.5f;
		[SerializeField] private TextMesh lifeText;
		private readonly float pathUpdateCooldown = 0.5f;

		private float health;
		private float lastDamageTime; // Tiempo del último ataque
		private float lastPathUpdate;

		private void Start(){
			health = GameManager.Instance.GetMaxLifeEnemy();
			agent = GetComponent<NavMeshAgent>();
			animator = GetComponent<Animator>();
			animator.applyRootMotion = false;

			lifeText = GetComponentInChildren<TextMesh>();
			if (lifeText != null)
				UpdateLifeText();
		}

		private void Update(){
			if (IsDead()) return;

			var distanceToPlayer = Vector3.Distance(transform.position, GameManager.Instance.player.transform.position);

			if (distanceToPlayer <= distanceDamage){
				agent.isStopped = true; // Detener al enemigo
				if (CanDamage()) DoDamage();
			}
			else{
				agent.isStopped = false; // Reanudar movimiento
				if (Time.time - lastPathUpdate > pathUpdateCooldown){
					UpdatePathToPlayer();
					lastPathUpdate = Time.time;
				}
			}
		}

		private void UpdatePathToPlayer(){
			var playerPos = GameManager.Instance.player.transform.position;
			var path = new NavMeshPath();
			if (agent.CalculatePath(playerPos, path) && path.status == NavMeshPathStatus.PathComplete)
				agent.SetDestination(playerPos);
		}

		public void TakeDamage(int damage){
			if (GameManager.Instance.isDebug)
				Debug.Log("Take damage: " + damage);

			if (IsDead()) return;

			health -= damage;
			if (health < 0) health = 0;
			GameManager.Instance.AddScore(10);
			UpdateLifeText();

			if (health <= 0) Die();
		}

		private void Die(){
			animator.SetTrigger("Dead");
			GameManager.Instance.OnEnemyKilled(60);
			agent.isStopped = true;
			Destroy(gameObject, 1f);
		}

		public bool CanDamage(){
			// Verifica si está dentro de la distancia y si el cooldown ha pasado
			return Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) < distanceDamage &&
			       Time.time - lastDamageTime >= GameManager.Instance.GetCooldownDamage();
		}

		public void DoDamage(){
			if (GameManager.Instance.isDebug)
				Debug.Log("Do damage");

			animator.SetTrigger("Attack");
			GameManager.Instance.player.TakeDamage(10);
			lastDamageTime = Time.time; // Actualiza el tiempo del último ataque
		}

		private bool IsDead(){
			return health <= 0;
		}

		private void UpdateLifeText(){
			if (lifeText != null)
				lifeText.text = $"{health} / {GameManager.Instance.GetMaxLifeEnemy()}";
		}
	}
}