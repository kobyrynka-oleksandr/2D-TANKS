using UnityEngine;
using System.Collections;

namespace CodeStyle.MemberHiding
{
    public class Orc : Enemy
    {
        new public void Yell()
        {
            Debug.Log("Orc version of the Yell() method");
        }
    }
}