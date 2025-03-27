using System;
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
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            GameObject hitObject = other.gameObject;
            if (hitObject.CompareTag(Tags.ENEMY))
            {
                Enemy enemy = hitObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.takeDamage(damage);
                }
            }
            Destroy(gameObject);
        }
    }
}