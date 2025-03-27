using UnityEngine;

namespace Weapons
{
    public class WeaponShotGun : Weapon
    {
        public int pelletsCount; // Number of pellets for shotgun

        protected override void Fire()
        {
            for (var i = 0; i < pelletsCount; i++)
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
}