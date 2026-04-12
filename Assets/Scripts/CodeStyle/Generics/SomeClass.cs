using UnityEngine;
using System.Collections;

namespace CodeStyle.Generics
{
    public class SomeClass
    {
        public T GenericMethod<T>(T param)
        {
            return param;
        }
    }
}
