using DefaultNamespace;
using UnityEngine;

namespace Weapons
{
    public class Bullet : MonoBehaviour
    {
        public int damage;
        public float speed;
        public float maxDistance;
        private Vector3 startPosition;

        private void Start()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
                Destroy(gameObject);
        }


        private void OnTriggerEnter(Collider other)
        {
            var hitObject = other.gameObject;
            if (hitObject.CompareTag("Enemy"))
            {
                var enemy = hitObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Debug.Log($"Bullet hit {hitObject.name}, dealing {damage} damage.");
                    enemy.TakeDamage(damage);
                }
                else
                {
                    Debug.LogWarning("Enemy tag detected, but no Enemy component found on the GameObject.");
                }
            }
            else
            {
                Debug.Log($"Collision detected with object: {hitObject.name}, but it does not have the Enemy tag.");
            }

            Destroy(gameObject);
        }
    }
}