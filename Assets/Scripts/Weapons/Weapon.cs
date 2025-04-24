using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapons{
	public class Weapon : MonoBehaviour{
		public GameObject bulletPrefab;
		public Transform bulletSpawn;

		[Header("Weapon Settings")] public float reloadTime;
		public int damage;
		public int actualAmmo;
		public int maxAmmo;
		public int magazineSize;
		public float fireRate;
		public float range;

		[Header("Bullet Settings")] public float bulletSpeed;
		public ParticleSystem particle;
		public float bulletSpread;
		private readonly float bulletLifeTime = 3f;
		private bool _isReloading;

		private float _nextFireTime;

		private void Awake(){
			if (bulletPrefab == null) Debug.LogError("La bala no está asignada en el inspector.");

			if (particle == null) Debug.LogError("El sistema de partículas no está asignado en el inspector.");

			_nextFireTime = 0f;
			actualAmmo = magazineSize;
		}

		private void Update(){
			if (_isReloading) return;

			if (Input.GetKeyDown(KeyCode.R) && actualAmmo < magazineSize && maxAmmo > 0) StartCoroutine(Reload());
		}

		public void Fire(Vector3 firePosition, Vector3 fireDirection){
			// Verificar si el arma está lista para disparar según el fireRate
			if (Time.time < _nextFireTime){
				Debug.Log($"No puedes disparar aún. Tiempo restante: {_nextFireTime - Time.time} segundos.");
				return;
			}

			// Verificar si hay munición en el cargador
			if (actualAmmo <= 0){
				Debug.Log("Sin munición en el cargador. Recarga para continuar disparando.");
				return;
			}

			// Aplicar propagación al disparo
			var spread = Quaternion.Euler(
				Random.Range(-bulletSpread, bulletSpread),
				Random.Range(-bulletSpread, bulletSpread),
				0f
			);

			if (particle != null){
				particle.transform.position = bulletSpawn.position;
				particle.transform.rotation = Quaternion.LookRotation(fireDirection);
				particle.Play();
			}
			else{
				Debug.LogWarning("Sistema de partículas no encontrado");
			}

			var finalDirection = spread * fireDirection;

			var adjustedFirePosition = firePosition + fireDirection.normalized * 0.5f; // Desplaza 0.5 unidades hacia adelante

			var bullet = Instantiate(bulletPrefab, adjustedFirePosition, Quaternion.LookRotation(finalDirection));
			var bulletScript = bullet.GetComponent<Bullet>();
			if (bulletScript != null){
				bulletScript.damage = damage;
				bulletScript.speed = bulletSpeed;
				bulletScript.maxDistance = range;
			}
			else{
				Debug.LogWarning("El prefab de la bala no tiene el script Bullet adjunto.");
			}

			Destroy(bullet, bulletLifeTime);

			// Actualizar tiempo del próximo disparo y reducir munición
			_nextFireTime = Time.time + 1f / fireRate;
			actualAmmo--;
		}

		public IEnumerator Reload(){
			_isReloading = true;
			Debug.Log("Recargando...");
			yield return new WaitForSeconds(reloadTime);

			var ammoNeeded = magazineSize - actualAmmo;
			var ammoToReload = Mathf.Min(ammoNeeded, maxAmmo);
			actualAmmo += ammoToReload;
			maxAmmo -= ammoToReload;

			_isReloading = false;
			GameManager.Instance.UpdateAmmoText(this);
			Debug.Log("Recarga completa");
		}
	}

	public enum TypeFire{
		Pulse,
		Single,
		Burst,
		Auto
	}
}