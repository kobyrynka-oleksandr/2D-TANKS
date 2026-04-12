using UnityEngine;
using System.Collections;

namespace CodeStyle.ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static void ResetTransformation(this Transform trans)
        {
            trans.position = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = new Vector3(1, 1, 1);
        }
    }
}
