using UnityEngine;
using System.Collections;

namespace CodeStyle.Generics
{
    public class GenericClass<T>
    {
        T item;

        public void UpdateItem(T newItem)
        {
            item = newItem;
        }
    }
}
