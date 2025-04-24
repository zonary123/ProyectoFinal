using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapons{
	public class Weapon : MonoBehaviour{
		public enum TypeFire{
			Pulse,
			Single,
			Burst,
			Auto
		}

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
			if (Time.time < _nextFireTime) return;
			if (actualAmmo <= 0) return;

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

			var finalDirection = spread * fireDirection;

			var adjustedFirePosition =
				firePosition + fireDirection.normalized * 0.5f; // Desplaza 0.5 unidades hacia adelante

			var bullet = Instantiate(bulletPrefab, adjustedFirePosition, Quaternion.LookRotation(finalDirection));
			var bulletScript = bullet.GetComponent<Bullet>();
			if (bulletScript != null){
				bulletScript.damage = damage;
				bulletScript.speed = bulletSpeed;
				bulletScript.maxDistance = range;
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
}