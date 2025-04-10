using System.Collections;
using UnityEngine;

namespace Weapons
{
    public class Weapon : MonoBehaviour
    {
        public GameObject bulletPrefab;
        public Transform bulletSpawn;

        [Header("Weapon Settings")] public float reloadTime;

        public TypeFire TypeFire;
        public int damage;
        public int currentMagazineAmmo;
        public int maxAmmo;
        public int magazineSize;
        public float fireRate;
        public float range;

        [Header("Bullet Settings")] public float bulletSpeed;
        public ParticleSystem particle;
        public float bulletLifeTime;
        public float bulletSpread;
        private bool _isReloading;

        private float _nextFireTime;

        private void Start()
        {
            _nextFireTime = 0f;
            currentMagazineAmmo = magazineSize;
        }

        private void Update()
        {
            if (_isReloading) return;

            if (Input.GetKeyDown(KeyCode.R) && currentMagazineAmmo < magazineSize && maxAmmo > 0)
            {
                StartCoroutine(Reload());
                return;
            }

            if (CanFire() && currentMagazineAmmo > 0)
            {
                Fire();
                _nextFireTime = Time.time + 1f / fireRate;
                currentMagazineAmmo--;
                Debug.Log($"Current magazine ammo: {currentMagazineAmmo} / {magazineSize}, Total ammo: {maxAmmo}");
            }
        }

        private IEnumerator Reload()
        {
            _isReloading = true;
            Debug.Log("Reloading...");
            yield return new WaitForSeconds(reloadTime);

            var ammoNeeded = magazineSize - currentMagazineAmmo;
            var ammoToReload = Mathf.Min(ammoNeeded, maxAmmo);
            currentMagazineAmmo += ammoToReload;
            maxAmmo -= ammoToReload;

            _isReloading = false;
            Debug.Log("Reload complete.");
        }

        protected virtual bool CanFire()
        {
            switch (TypeFire)
            {
                case TypeFire.Pulse:
                    return Input.GetButtonDown("Fire1");
                case TypeFire.Single:
                    return Input.GetButtonDown("Fire1") && Time.time >= _nextFireTime;
                case TypeFire.Burst:
                    return Input.GetButton("Fire1") && Time.time >= _nextFireTime;
                case TypeFire.Auto:
                    return Input.GetButton("Fire1");
                default:
                    return false;
            }
        }

        protected virtual void Fire()
        {
            var spread = Quaternion.Euler(
                Random.Range(-bulletSpread, bulletSpread),
                Random.Range(-bulletSpread, bulletSpread),
                0f
            );
            var fireDirection = spread * bulletSpawn.forward;

            var bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.LookRotation(fireDirection));
            var bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = damage;
                bulletScript.speed = bulletSpeed;
                bulletScript.maxDistance = range;
                Debug.Log(bulletScript.damage);
            }
            else
            {
                Debug.LogWarning("Bullet script not found");
            }

            Destroy(bullet, bulletLifeTime);
        }
    }

    public enum TypeFire
    {
        Pulse,
        Single,
        Burst,
        Auto
    }
}