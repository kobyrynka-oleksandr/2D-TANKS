using UnityEngine;

namespace CodeStyle.Interfaces
{
    public interface IDamageable
    {
        Vector3 Position { get; }
        void Damage(float damage);
    }
}