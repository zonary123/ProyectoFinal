using UnityEngine;

namespace DefaultNamespace
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private float health;
        private TextMesh lifeText;

        private void Start()
        {
            health = GameManager.Instance.maxLifeEnemy;
            lifeText = GetComponentInChildren<TextMesh>();
            if (lifeText != null)
                UpdateLifeText();
            else
                Debug.LogError("TextMesh component not found in children.");
        }

        private void Update()
        {
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                GameManager.Instance.enemiesAlive--;
                GameManager.Instance.AddScore(1);
                Destroy(gameObject);
            }
            else
            {
                UpdateLifeText();
            }
        }

        private void UpdateLifeText()
        {
            lifeText.text = health + " / " + GameManager.Instance.maxLifeEnemy;
        }
    }
}