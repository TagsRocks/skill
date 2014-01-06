using System;
using UnityEngine;

namespace Skill.Framework.IO
{
    public class InputButton
    {
        private static int _ValueGenerator = 0;

        /// <summary>
        /// value (power of 2)
        /// </summary>
        public int Value { get; private set; }

        /// <summary> Name of Button </summary>
        public string Name { get; private set; }

        /// <summary>
        /// keys that atleast one press required to activate this button.
        /// </summary>
        public KeyCode[] Keys { get; private set; }

        /// <summary> Is direction button </summary>
        public bool IsDirection { get; private set; }

        /// <summary>
        /// Create Button
        /// </summary>
        /// <param name="name"> Name of Button </param>
        /// <param name="key"> keys that atleast one press required to activate this button. </param>
        internal InputButton(string name, params KeyCode[] keys)
        {
            if (_ValueGenerator > 31)
                throw new InvalidOperationException("Can not create more than 32 buttons");
            this.Value = 1 << _ValueGenerator++;
            this.Name = name;
            this.Keys = keys;
        }

        /// <summary> Is button pressed </summary>
        public bool IsPressed
        {
            get
            {
                for (int i = 0; i < Keys.Length; i++)
                {
                    if (UnityEngine.Input.GetKeyDown(this.Keys[i]))
                        return true;
                }
                return false;
            }
        }

        public static implicit operator int(InputButton b)
        {
            return b.Value;
        }
    }

}