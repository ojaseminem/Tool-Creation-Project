using UnityEngine;
using UnityEngine.UI;

namespace HealthBar_Scene.Scripts
{
    public class PlayerHealth : MonoBehaviour
    {
        #region Variables

        public float health;

        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float chipSpeed = 2f;
        [SerializeField] private Image frontHealthBar;
        [SerializeField] private Image backHealthBar;
        private float m_LerpTimer;

        #endregion

        #region Private Functions

        private void Start() => health = maxHealth;

        private void Update()
        {
            health = Mathf.Clamp(health, 0, maxHealth);

            UpdateHealthUI();
        }

        private void UpdateHealthUI()
        {
            float fillFront = frontHealthBar.fillAmount;
            float fillBack = backHealthBar.fillAmount;
            float healthFraction = health / maxHealth;

            if (fillBack > healthFraction)
            {
                frontHealthBar.fillAmount = healthFraction;
                backHealthBar.color = Color.red;
                m_LerpTimer += Time.deltaTime;
                float percentComplete = m_LerpTimer / chipSpeed;
                percentComplete *= percentComplete;
                backHealthBar.fillAmount = Mathf.Lerp(fillBack, healthFraction, percentComplete);
            }

            if (fillFront < healthFraction)
            {
                backHealthBar.color = Color.green;
                backHealthBar.fillAmount = healthFraction;
                m_LerpTimer += Time.deltaTime;
                float percentComplete = m_LerpTimer / chipSpeed;
                percentComplete *= percentComplete;
                frontHealthBar.fillAmount = Mathf.Lerp(fillFront, backHealthBar.fillAmount, percentComplete);
            }
        }

        #endregion

        #region Public Functions

        //Call this function to take damage
        public void TakeDamage(float damage)
        {
            health -= damage;
            m_LerpTimer = 0f;
        }

        //Call this function to heal the player
        public void RestoreHealth(float healAmount)
        {
            health += healAmount;
            m_LerpTimer = 0f;
        }

        #endregion
    }
}