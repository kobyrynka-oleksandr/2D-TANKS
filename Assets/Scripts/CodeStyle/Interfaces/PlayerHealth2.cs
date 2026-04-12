using UnityEngine;

namespace CodeStyle.Interfaces
{
    public class PlayerHealth2 : MonoBehaviour
    {
        [SerializeReference]
        public IDamageable shield = new ProtonShield();
    }
}