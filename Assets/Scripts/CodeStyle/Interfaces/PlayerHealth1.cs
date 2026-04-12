using UnityEngine;

namespace CodeStyle.Interfaces
{
    public class PlayerHealth1 : MonoBehaviour, IDamageable
    {
        public float startingHealth = 100f;
        float m_CurrentHealth;
        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
        }
        void Start()
        {
            m_CurrentHealth = startingHealth;
        }
        public void Damage(float damage)
        {
            m_CurrentHealth -= damage;
        }
    }
}