using UnityEngine;

namespace CodeStyle.PolymorphismAndCasting
{
    public class PetShop : MonoBehaviour
    {
        [SerializeReference]
        public Mammal mammal = new Cat();
    }
}