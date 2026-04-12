using UnityEngine;
using System.Collections;

namespace CodeStyle.Generics
{
    public class GenericClassExample : MonoBehaviour
    {
        void Start()
        {
            GenericClass<int> myClass = new GenericClass<int>();

            myClass.UpdateItem(5);
        }
    }
}