using UnityEngine;
using System.Collections;


namespace Skill.Framework.ModernUI
{
    public class ColorKey : MonoBehaviour
    {
        public string NameInBank = "Text";


        void Start()
        {
            ColorBank.Apply(this);
        }

#if UNITY_EDITOR

        [ContextMenu("Update from Bank")]
        public void UpdateFromBank()
        {
            ColorBank.Apply(this);
        }

#endif
    }
}