using UnityEngine;
using System.Collections;

namespace CodeStyle.Statics
{
    public class Enemy
    {
        public static int enemyCount = 0;

        public Enemy()
        {
            enemyCount++;
        }
    }
}