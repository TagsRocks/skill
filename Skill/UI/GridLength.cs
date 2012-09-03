using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.UI
{
    /// <summary>
    /// Represents the length of elements that explicitly support Skill.UI.GridUnitType.Star unit types.
    /// </summary>
    public struct GridLength : IEquatable<GridLength>
    {
        private float _UnitValue;
        private GridUnitType _UnitType;
        private static readonly GridLength _Auto;
        
        /// <summary>
        /// Initializes a new instance of the GridLength structure using the specified absolute value in pixels.
        /// </summary>
        /// <param name="pixels"> The number of  pixels.</param>
        /// <exception cref="System.ArgumentException">Pixels is equal to float.NegativeInfinity, float.PositiveInfinity, or float.NaN </exception>
        public GridLength(float pixels)
            : this(pixels, GridUnitType.Pixel)
        {
        }       
        
        /// <summary>
        /// Initializes a new instance of the GridLength structure and specifies what kind of value it holds.
        /// </summary>
        /// <param name="value"> The initial value of this instance of GridLength. </param>
        /// <param name="type"> The GridUnitType held by this instance of GridLength. </param>
        /// <exception cref="System.ArgumentException">Pixels is equal to float.NegativeInfinity, float.PositiveInfinity, or float.NaN </exception>
        public GridLength(float value, GridUnitType type)
        {
            if (float.IsNaN(value))
            {
                throw new ArgumentException("Invalid constructor parameter (value is NaN)");
            }
            if (float.IsInfinity(value))
            {
                throw new ArgumentException("Invalid constructor parameter (value is Infinity)");
            }
            if (((type != GridUnitType.Auto) && (type != GridUnitType.Pixel)) && (type != GridUnitType.Star))
            {
                throw new ArgumentException("Invalid constructor parameter (Unknown GridUnitType)");
            }
            this._UnitValue = (type == GridUnitType.Auto) ? 0.0f : value;
            this._UnitType = type;
        }

        /// <summary>
        ///  Compares two GridLength structures for equality.
        /// </summary>
        /// <param name="gl1"> The first instance of GridLength to compare. </param>
        /// <param name="gl2"> The second instance of GridLength to compare. </param>
        /// <returns> true if the two instances of GridLength have the same value and GridUnitType; otherwise, false. </returns>
        public static bool operator ==(GridLength gl1, GridLength gl2)
        {
            return ((gl1.GridUnitType == gl2.GridUnitType) && (gl1.Value == gl2.Value));
        }
        
        /// <summary>
        ///  Compares two GridLength structures to determine if they are not equal.
        /// </summary>
        /// <param name="gl1"> The first instance of GridLength to compare. </param>
        /// <param name="gl2"> The second instance of GridLength to compare. </param>
        /// <returns>true if the two instances of GridLength do not have the same value and GridUnitType; otherwise, false. </returns>
        public static bool operator !=(GridLength gl1, GridLength gl2)
        {
            if (gl1.GridUnitType == gl2.GridUnitType)
            {
                return !(gl1.Value == gl2.Value);
            }
            return true;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current GridLength instance.
        /// </summary>
        /// <param name="oCompare"> The object to compare with the current instance. </param>
        /// <returns> true if the specified object has the same value and GridUnitType as the current instance; otherwise, false. </returns>
        public override bool Equals(object oCompare)
        {
            if (oCompare is GridLength)
            {
                GridLength length = (GridLength)oCompare;
                return (this == length);
            }
            return false;
        }

        /// <summary>
        /// Determines whether the GridLength is equal to the current GridLength.
        /// </summary>
        /// <param name="gridLength"> The GridLength structure to compare with the current instance. </param>
        /// <returns> true if the specified GridLength has the same value and GridUnitType as the current instance; otherwise, false. </returns>
        public bool Equals(GridLength gridLength)
        {
            return (this == gridLength);
        }

        /// <summary>
        /// Gets a hash code for the GridLength.
        /// </summary>
        /// <returns>
        ///  A hash code for the current GridLength structure.
        /// </returns>
        public override int GetHashCode()
        {
            return (((int)this._UnitValue) + (int)this._UnitType);
        }

        /// <summary>
        /// Gets a value that indicates whether the GridLength holds a value that is expressed in pixels.
        /// </summary>
        public bool IsAbsolute
        {
            get
            {
                return (this._UnitType == GridUnitType.Pixel);
            }
        }
        /// <summary>
        /// Gets a value that indicates whether the GridLength holds a value whose size is determined by the size properties of the content object.
        /// </summary>
        public bool IsAuto
        {
            get
            {
                return (this._UnitType == GridUnitType.Auto);
            }
        }

        /// <summary>
        ///  Gets a value that indicates whether the GridLength holds a value that is expressed as a weighted proportion of available space.
        /// </summary>
        public bool IsStar
        {
            get
            {
                return (this._UnitType == GridUnitType.Star);
            }
        }

        /// <summary>
        /// Gets a float that represents the value of the GridLength.
        /// </summary>
        public float Value
        {
            get
            {
                if (this._UnitType != GridUnitType.Auto)
                {
                    return this._UnitValue;
                }
                return 1.0f;
            }
        }
        /// <summary>
        /// Gets the associated GridUnitType for the GridLength.
        /// </summary>
        public GridUnitType GridUnitType
        {
            get
            {
                return this._UnitType;
            }
        }
        /// <summary>
        /// Returns a System.String representation of the GridLength.
        /// </summary>
        /// <returns> A System.String representation of the current GridLength structure.</returns>
        public override string ToString()
        {
            return string.Format("Type : {0}, Value : {1}", _UnitType, _UnitValue);
        }

        /// <summary>
        /// Gets an instance of GridLength that holds a value whose size is determined by the size properties of the content object.
        /// </summary>
        public static GridLength Auto
        {
            get
            {
                return _Auto;
            }
        }


        static GridLength()
        {
            _Auto = new GridLength(1.0f, GridUnitType.Auto);
        }
    }

}
