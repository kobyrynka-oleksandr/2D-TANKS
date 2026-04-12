using UnityEngine;
using System.Collections;

namespace CodeStyle.CreatingProperties
{
    public class Game : MonoBehaviour
    {
        void Start()
        {
            Player myPlayer = new Player();

            myPlayer.Experience = 5;
            int x = myPlayer.Experience;
        }
    }
}
