using UnityEngine;
using System.Collections;

namespace CodeStyle.Overriding
{
    public class FruitSalad : MonoBehaviour
    {
        void Start()
        {
            Apple myApple = new Apple();

            myApple.SayHello();
            myApple.Chop();

            Fruit myFruit = new Apple();
            myFruit.SayHello();
            myFruit.Chop();
        }
    }
}