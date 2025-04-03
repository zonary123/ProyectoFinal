using UnityEngine;

namespace Weapons
{
    public class Weapon : MonoBehaviour
    {
        public int damage;
        public GameObject bulletPrefab;
        public Transform bulletSpawn;
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
            if (canFire())
            {
                Debug.Log("Fire");
                Fire();
                _nextFireTime = Time.time + 1f / fireRate;
            }
        }

        protected virtual bool canFire()
        {
            return Input.GetButton("Fire1") && Time.time >= _nextFireTime;
        }

        protected virtual void Fire()
        {
            var fireDirection = bulletSpawn.forward;

            var bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.LookRotation(fireDirection));
            var bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.damage = damage;
            bulletScript.speed = bulletSpeed;
            bulletScript.maxDistance = range;

            Destroy(bullet, bulletLifeTime);
        }
    }
}