using System;
using System.Collections.Generic;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Implements a structure that is used to describe the Size of an object.
    /// </summary>
    public struct Size
    {
        private float _Width;
        private float _Height;
        private static readonly Size _Empty;

        /// <summary>
        /// Compares two instances of Size for equality.
        /// </summary>
        /// <param name="size1">The first instance of Skill.UI.Size to compare.</param>
        /// <param name="size2"> The second instance of Skill.UI.Size to compare.</param>
        /// <returns>true if the two instances of Skill.UI.Size are equal; otherwise false.</returns>
        public static bool operator ==(Size size1, Size size2)
        {
            return ((size1.Width == size2.Width) && (size1.Height == size2.Height));
        }

        /// <summary>
        /// Compares two instances of Skill.UI.Size for inequality.
        /// </summary>
        /// <param name="size1">The first instance of Skill.UI.Size to compare.</param>
        /// <param name="size2">The second instance of Skill.UI.Size to compare.</param>
        /// <returns>true if the instances of Skill.UI.Size are not equal; otherwise false.</returns>
        public static bool operator !=(Size size1, Size size2)
        {
            return !(size1 == size2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size1"></param>
        /// <param name="size2"></param>
        /// <returns></returns>
        public static bool Equals(Size size1, Size size2)
        {
            if (size1.IsEmpty)
            {
                return size2.IsEmpty;
            }
            return (size1.Width.Equals(size2.Width) && size1.Height.Equals(size2.Height));
        }

        /// <summary>
        /// Compares an object to an instance of Skill.UI.Size for equality.
        /// </summary>
        /// <param name="o">The System.Object to compare.</param>
        /// <returns>true if the sizes are equal; otherwise, false.</returns>
        public override bool Equals(object o)
        {
            if ((o == null) || !(o is Size))
            {
                return false;
            }
            Size size = (Size)o;
            return Equals(this, size);
        }

        /// <summary>
        /// Compares a value to an instance of Skill.UI.Size for equality.
        /// </summary>
        /// <param name="value">The size to compare to this current instance of Skill.UI.Size.</param>
        /// <returns>true if the instances of Skill.UI.Size are equal; otherwise, false.</returns>
        public bool Equals(Size value)
        {
            return Equals(this, value);
        }

        /// <summary>
        /// Gets the hash code for this instance of Skill.UI.Size.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (this.IsEmpty)
            {
                return 0;
            }
            return (this.Width.GetHashCode() ^ this.Height.GetHashCode());
        }        

        /// <summary>
        ///  The hash code for this instance of Skill.UI.Size.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Width : {0}, Height : {1}", this._Width, this._Height);
        }                

        /// <summary>
        /// Initializes a new instance of the Skill.UI.Size structure and assigns it an initial width and height.
        /// </summary>
        /// <param name="width">The initial width of the instance of Skill.UI.Size.</param>
        /// <param name="height">The initial height of the instance of Skill.UI.Size.</param>
        public Size(float width, float height)
        {
            if ((width < 0.0) || (height < 0.0))
            {
                throw new ArgumentException("Width And Height of Size can not be negative");
            }
            this._Width = width;
            this._Height = height;
        }

        /// <summary>
        /// Empty size
        /// </summary>
        public static Size Empty
        {
            get
            {
                return _Empty;
            }
        }

        /// <summary>
        /// Gets a value that represents a static empty Skill.UI.Size.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (this._Width < 0.0);
            }
        }
        /// <summary>
        /// Gets or sets the Skill.UI.Size.Width of this instance of Skill.UI.Size.
        /// </summary>
        /// <returns>
        /// The Skill.UI.Size.Width of this instance of Skill.UI.Size. The default value is 0. The value cannot be negative.
        /// </returns>
        public float Width
        {
            get
            {
                return this._Width;
            }
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException("Can not modify empty size");
                }
                if (value < 0.0f)
                {
                    throw new ArgumentException("Width of Size can not be negative");
                }
                this._Width = value;
            }
        }
        /// <summary>
        /// Gets or sets the Skill.UI.Size.Height of this instance of Skill.UI.Size.
        /// </summary>
        /// <returns> The Skill.UI.Size.Height of this instance of Skill.UI.Size. The default is 0. The value cannot be negative. </returns>
        public float Height
        {
            get
            {
                return this._Height;
            }
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException("Can not modify empty size");
                }
                if (value < 0.0)
                {
                    throw new ArgumentException("Height of Size can not be negative");
                }
                this._Height = value;
            }
        }
        /// <summary>
        /// Convet a Size to a UnityEngine.Vector2
        /// </summary>
        /// <param name="size">size to convert</param>
        /// <returns>Size</returns>
        public static explicit operator UnityEngine.Vector2(Size size)
        {
            return new UnityEngine.Vector2(size._Width, size._Height);
        }

        /// <summary>
        /// Convet a UnityEngine.Vector2 to a Size
        /// </summary>
        /// <param name="vector"> vector to convert </param>
        /// <returns>Skill.UI.Size</returns>
        public static explicit operator Size(UnityEngine.Vector2 vector)
        {
            return new Size(vector.x, vector.y);
        }
                
        private static Size CreateEmptySize()
        {
            return new Size { _Width = float.NegativeInfinity, _Height = float.NegativeInfinity };
        }

        static Size()
        {
            _Empty = CreateEmptySize();
        }
    }

}
