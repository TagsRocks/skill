using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Describes the thickness of a frame around a rectangle. 
    /// Four float values describe the Thickness.Left, Thickness.Top, Thickness.Right, and Thickness.Bottom sides of the rectangle, respectively.
    /// </summary>
    public struct Thickness : IEquatable<Thickness>
    {
        private float _Left;
        private float _Top;
        private float _Right;
        private float _Bottom;

        /// <summary>
        /// Initializes a new instance of the Thickness structure that has the specified uniform length on each side.
        /// </summary>
        /// <param name="uniformLength">The uniform length applied to all four sides of the bounding rectangle.</param>
        public Thickness(float uniformLength)
        {
            this._Left = this._Top = this._Right = this._Bottom = uniformLength;
        }

        /// <summary>
        /// Initializes a new instance of the Thickness structure that has specific lengths (supplied as a System.Double) applied to each side of the rectangle.
        /// </summary>
        /// <param name="left"> The thickness for the left side of the rectangle.</param>
        /// <param name="top">The thickness for the upper side of the rectangle.</param>
        /// <param name="right">The thickness for the right side of the rectangle</param>
        /// <param name="bottom">The thickness for the lower side of the rectangle.</param>
        public Thickness(float left, float top, float right, float bottom)
        {
            this._Left = left;
            this._Top = top;
            this._Right = right;
            this._Bottom = bottom;
        }

        /// <summary>
        /// Initializes a new instance of the Thickness structure that has specific lengths (supplied as a System.Double) applied to each side of the rectangle.
        /// </summary>
        /// <param name="leftAndRight"> The thickness for the left side and the right side of the rectangle.</param>
        /// <param name="topAndBottom"> The thickness for the upper side and the lower side of the rectangle.</param>        
        public Thickness(float leftAndRight, float topAndBottom)
            : this(leftAndRight, topAndBottom, leftAndRight, topAndBottom)
        {
        }

        /// <summary>
        /// Compares this Thickness structure to another System.Object for equality.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>true if the two objects are equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Thickness)
            {
                Thickness thickness = (Thickness)obj;
                return (this == thickness);
            }
            return false;
        }
        /// <summary>
        ///  Compares this Thickness structure to another Thickness structure for equality.
        /// </summary>
        /// <param name="thickness"> An instance of Thickness to compare for equality.</param>
        /// <returns>true if the two instances of Thickness are equal; otherwise, false.</returns>
        public bool Equals(Thickness thickness)
        {
            return (this == thickness);
        }
        /// <summary>
        /// Returns the hash code of the structure.
        /// </summary>
        /// <returns> A hash code for this instance of Thickness. </returns>
        public override int GetHashCode()
        {
            return (((this._Left.GetHashCode() ^ this._Top.GetHashCode()) ^ this._Right.GetHashCode()) ^ this._Bottom.GetHashCode());
        }
        /// <summary>
        /// Returns the string representation of the Thickness structure.
        /// </summary>
        /// <returns>A System.String that represents the Thickness value.</returns>
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", _Left, _Top, _Right, _Bottom);
        }
        /// <summary>
        /// Compares the value of two Thickness structures for equality.
        /// </summary>
        /// <param name="t1">The first structure to compare.</param>
        /// <param name="t2">The other structure to compare.</param>
        /// <returns>true if the two instances of Thickness are equal; otherwise, false.</returns>
        public static bool operator ==(Thickness t1, Thickness t2)
        {
            return ((t1._Left == t2._Left) && (t1._Top == t2._Top) && (t1._Right == t2._Right) && (t1._Bottom == t2._Bottom));
        }
        /// <summary>
        /// Compares two Thickness structures for inequality.
        /// </summary>
        /// <param name="t1">The first structure to compare.</param>
        /// <param name="t2">The other structure to compare.</param>
        /// <returns>true if the two instances of Thickness are not equal; otherwise, false.</returns>
        public static bool operator !=(Thickness t1, Thickness t2)
        {
            return !(t1 == t2);
        }

        /// <summary>
        /// Gets or sets the width, in pixels, of the left side of the bounding rectangle.
        /// </summary>
        public float Left
        {
            get
            {
                return this._Left;
            }
            set
            {
                this._Left = value;
            }
        }
        /// <summary>
        /// Gets or sets the width, in pixels, of the upper side of the bounding rectangle.
        /// </summary>
        public float Top
        {
            get
            {
                return this._Top;
            }
            set
            {
                this._Top = value;
            }
        }
        /// <summary>
        /// Gets or sets the width, in pixels, of the right side of the bounding rectangle.
        /// </summary>
        public float Right
        {
            get
            {
                return this._Right;
            }
            set
            {
                this._Right = value;
            }
        }
        /// <summary>
        ///  Gets or sets the width, in pixels, of the lower side of the bounding rectangle.
        /// </summary>
        public float Bottom
        {
            get
            {
                return this._Bottom;
            }
            set
            {
                this._Bottom = value;
            }
        }

        /// <summary>
        /// Left + Right
        /// </summary>
        public float Horizontal
        {
            get
            {
                return this._Left + this._Right;
            }
        }
        /// <summary>
        /// Top + Bottom
        /// </summary>
        public float Vertical
        {
            get
            {
                return this._Top + this._Bottom;
            }
        }

        private static Thickness _Empty = new Thickness(0);
        /// <summary>
        /// zero Thickness
        /// </summary>
        public static Thickness Empty { get { return _Empty; } }
    }

}