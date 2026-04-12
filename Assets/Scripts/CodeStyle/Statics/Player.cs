using UnityEngine;
using System.Collections;

namespace CodeStyle.Statics
{
    public class Player : MonoBehaviour
    {
        public static int playerCount = 0;

        void Start()
        {
            playerCount++;
        }
    }
}