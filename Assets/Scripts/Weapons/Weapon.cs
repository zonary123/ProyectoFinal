using UnityEngine;

namespace Weapons
{
    public class Weapon : MonoBehaviour
    {
        public int damage;
        public GameObject bulletPrefab;
        public float fireRate;
        public float range;
        public float bulletSpeed;
        public float bulletLifeTime;
        public float bulletSpread;

        private float _nextFireTime;

        private void Start()
        {
            _nextFireTime = 0f;
        }

        private void Update()
        {
            Debug.Log("Update method called");

            if (Input.GetKeyDown(KeyCode.F) && Time.time >= _nextFireTime)
            {
                Debug.Log("Fire");
                Fire();
                _nextFireTime = Time.time + 1f / fireRate;
            }

            Debug.Log("Update Weapon");
        }

        protected virtual void Fire()
        {
            var fireDirection = transform.forward;
            fireDirection.x += Random.Range(-bulletSpread, bulletSpread);
            fireDirection.y += Random.Range(-bulletSpread, bulletSpread);

            var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.LookRotation(fireDirection));
            var bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.damage = damage;
            bulletScript.speed = bulletSpeed;
            bulletScript.maxDistance = range;

            Destroy(bullet, bulletLifeTime);
        }
    }
}