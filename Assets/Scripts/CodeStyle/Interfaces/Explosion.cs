using System.Collections.Generic;
using UnityEngine;

namespace CodeStyle.Interfaces
{
    public class Explosion : MonoBehaviour
    {
        public float range = 10f;
        public float damage = 35f;
        List<IDamageable> m_AllDamageables = new List<IDamageable>();
        void Start()
        {
            MonoBehaviour[] allScripts = Object.FindObjectsByType<MonoBehaviour>(UnityEngine.FindObjectsSortMode.None);
            for (int i = 0; i < allScripts.Length; i++)
            {
                if (allScripts[i] is IDamageable)
                    m_AllDamageables.Add(allScripts[i] as IDamageable);
            }
        }
        public void Explode()
        {
            for (int i = 0; i < m_AllDamageables.Count; i++)
            {
                if (Vector3.Distance(m_AllDamageables[i].Position, transform.position) < range)
                    m_AllDamageables[i].Damage(damage);
            }
        }
    }
}