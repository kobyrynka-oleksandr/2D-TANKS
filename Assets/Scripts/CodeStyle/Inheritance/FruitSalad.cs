using UnityEngine;
using System.Collections;

namespace CodeStyle.Inheritance
{
    public class FruitSalad : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("Creating the fruit");
            Fruit myFruit = new Fruit();
            Debug.Log("Creating the apple");
            Apple myApple = new Apple();

            myFruit.SayHello();
            myFruit.Chop();

            myApple.SayHello();
            myApple.Chop();

            Debug.Log("Creating the fruit");
            myFruit = new Fruit("yellow");
            Debug.Log("Creating the apple");
            myApple = new Apple("green");

            myFruit.SayHello();
            myFruit.Chop();

            myApple.SayHello();
            myApple.Chop();
        }
    }
}