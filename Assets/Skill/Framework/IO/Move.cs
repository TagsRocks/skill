using UnityEngine;
using System.Collections;

namespace Skill.Framework.IO
{
    /// <summary>
    /// Describes a sequences of buttons which must be pressed to active the move.
    /// </summary>
    public class Move
    {
        /// <summary> Name of move </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The sequence of buttons presses required to activate this move.
        /// </summary>
        public int[] Sequence { get; private set; }

        /// <summary>
        /// Set this to true if the input used to activate this move may
        /// be reused as a component of longer moves.
        /// </summary>
        public bool IsSubMove { get; set; }

        /// <summary>
        /// Create a Move
        /// </summary>
        /// <param name="name">Name of move</param>
        /// <param name="sequence"> The sequence of buttons presses required to activate this move. </param>
        public Move(string name, params int[] sequence)
        {
            Name = name;
            Sequence = sequence;
        }

        /// <summary> Occurs when move sequence detected </summary>
        public event System.EventHandler Perform;

        internal void RaisePerform()
        {
            if (Perform != null) Perform(this, System.EventArgs.Empty);
        }
    }
}