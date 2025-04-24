using DefaultNamespace;
using UnityEngine;

namespace Weapons{
	public class Bullet : MonoBehaviour{
		public int damage;
		public float speed;
		public float maxDistance;
		private Vector3 startPosition;

		private void Start(){
			startPosition = transform.position;
			var rb = GetComponent<Rigidbody>();
			if (rb != null) rb.isKinematic = true; // Evita interacciones físicas
		}

		private void Update(){
			transform.Translate(Vector3.forward * speed * Time.deltaTime);
			if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
				Destroy(gameObject);
		}

		private void OnTriggerEnter(Collider other){
			var hitObject = other.gameObject;
			if (hitObject.CompareTag("Enemy")){
				var enemy = hitObject.GetComponent<Enemy>();
				if (enemy != null){
					// Calcular el daño restante basado en la distancia recorrida
					var distanceTraveled = Vector3.Distance(startPosition, transform.position);
					var currentDamage = Mathf.Max(0, Mathf.RoundToInt(damage * (1 - distanceTraveled / maxDistance)));

					Debug.Log($"Bullet hit {hitObject.name}, dealing {currentDamage} damage.");
					enemy.TakeDamage(currentDamage);
				}
				else{
					Debug.LogWarning("Enemy tag detected, but no Enemy component found on the GameObject.");
				}
			}
			else{
				if (GameManager.Instance.isDebug)
					Debug.Log($"Collision detected with object: {hitObject.name}, but it does not have the Enemy tag.");
			}
		}
	}
}