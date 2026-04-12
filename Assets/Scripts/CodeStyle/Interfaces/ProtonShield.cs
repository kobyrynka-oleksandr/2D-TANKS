using UnityEngine;

namespace CodeStyle.Interfaces
{
    public class ProtonShield : IDamageable
    {
        public float hitPoints = 10f;
        public Vector3 Position { get; }
        public void Damage(float damage)
        {
            hitPoints -= damage;
        }
    }
}