using UnityEngine;
using System.Collections;

namespace CodeStyle.Statics
{
    public class Game
    {
        void Start()
        {
            Enemy enemy1 = new Enemy();
            Enemy enemy2 = new Enemy();
            Enemy enemy3 = new Enemy();

            int x = Enemy.enemyCount;
        }
    }
}