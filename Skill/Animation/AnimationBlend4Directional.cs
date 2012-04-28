using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    public enum MoveDirection
    {
        Forward = 0,
        Backward = 1,
        Left = 2,
        Right = 3
    }

    public class AnimationBlend4Directional : AnimationSwitchBase
    {
        public AnimationNode Forward { get { return base[0]; } set { base[0] = value; } }
        public AnimationNode Backward { get { return base[1]; } set { base[1] = value; } }
        public AnimationNode Left { get { return base[2]; } set { base[2] = value; } }
        public AnimationNode Right { get { return base[3]; } set { base[3] = value; } }

        private float _Angle;
        public float Angle { get { return _Angle; } set { _Angle = value % 360; Refresh(); } }

        private MoveDirection _Direction;
        public MoveDirection Direction { get { return _Direction; } }


        private void Refresh()
        {
            if (_Angle > 45 && _Angle < 135) _Direction = MoveDirection.Right;
            else if (_Angle >= 135 && _Angle <= 225) _Direction = MoveDirection.Backward;
            else if (_Angle > 225 && _Angle < 315) _Direction = MoveDirection.Left;
            else _Direction = MoveDirection.Forward;
        }

        protected override int SelectActiveChildIndex()
        {
            return (int)_Direction;
        }

        public AnimationBlend4Directional()
            : base(4)
        {

        }
    }
}
