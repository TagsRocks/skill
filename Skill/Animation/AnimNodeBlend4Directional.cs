using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    #region MoveDirection
    /// <summary>
    /// Defines 4 main direction
    /// </summary>
    public enum MoveDirection
    {
        /// <summary> Forward direction </summary>
        Forward = 0,
        /// <summary> Backward direction </summary>
        Backward = 1,
        /// <summary> Left direction </summary>
        Left = 2,
        /// <summary> Right direction  </summary>
        Right = 3
    } 
    #endregion

    /// <summary>
    /// This blend node allows the Anim Tree to automatically blend between four inputs which represent the owning actor moving forwards, backwards, strafing left and strafing right.
    /// The blend node compares the direction of velocity or acceleration to the direction of the owning actor. 
    /// Depending on the differences between the two, it will switch between the inputs.
    /// http://udn.epicgames.com/Three/AnimationNodes.html#AnimNodeBlendDirectional
    /// </summary>
    public class AnimNodeBlend4Directional : AnimNodeBlendByIndex
    {
        /// <summary> Forward child node </summary>
        public AnimNode Forward { get { return base[0]; } set { base[0] = value; } }
        /// <summary> Backward child node </summary>
        public AnimNode Backward { get { return base[1]; } set { base[1] = value; } }
        /// <summary> Left child node </summary>
        public AnimNode Left { get { return base[2]; } set { base[2] = value; } }
        /// <summary> Right child node </summary>
        public AnimNode Right { get { return base[3]; } set { base[3] = value; } }
        
        private float _Angle; // current angle of actor in local space
        /// <summary>
        /// Angle of actor in local space
        /// </summary>
        public float Angle { get { return _Angle; } set { _Angle = value % 360; Refresh(); } }

        private MoveDirection _Direction;// direction of actor

        /// <summary>
        /// current direction of actor based of angle
        /// </summary>
        public MoveDirection Direction { get { return _Direction; } }


        private void Refresh()
        {
            if (_Angle > 45 && _Angle < 135) _Direction = MoveDirection.Right;
            else if (_Angle >= 135 && _Angle <= 225) _Direction = MoveDirection.Backward;
            else if (_Angle > 225 && _Angle < 315) _Direction = MoveDirection.Left;
            else _Direction = MoveDirection.Forward;

            SelectedChildIndex = (int)_Direction;
        }        

        /// <summary>
        /// Create an instance of AnimNodeBlend4Directional
        /// </summary>
        public AnimNodeBlend4Directional()
            : base(4)
        {

        }        
    }
}
