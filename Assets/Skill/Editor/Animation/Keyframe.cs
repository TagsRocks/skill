using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.Animation
{
    public struct KeyframeData : IXmlElementSerializable
    {
        public const string ElementName = "Keyframe";

        /// <summary> The time of the keyframe. </summary>
        public float Time;
        /// <summary> The value of the curve at keyframe. </summary>
        public float Value;
        /// <summary> Describes the tangent when approaching this point from the previous point in the curve. </summary>
        public float InTangent;
        /// <summary> Describes the tangent when leaving this point towards the next point in the curve. </summary>
        public float OutTangent;

        /// <summary>
        /// Create a keyframe.
        /// </summary>
        /// <param name="time"> The time of the keyframe. </param>
        /// <param name="value"> The value of the curve at keyframe. </param>
        public KeyframeData(float time, float value)
        {
            this.InTangent = 0;
            this.OutTangent = 0;
            this.Time = time;
            this.Value = value;
        }
        /// <summary>
        /// Create a keyframe.
        /// </summary>
        /// <param name="time"> The time of the keyframe. </param>
        /// <param name="value"> The value of the curve at keyframe. </param>
        /// <param name="inTangent"> Describes the tangent when approaching this point from the previous point in the curve. </param>
        /// <param name="outTangent"> Describes the tangent when leaving this point towards the next point in the curve. </param>
        public KeyframeData(float time, float value, float inTangent, float outTangent)
        {
            this.InTangent = inTangent;
            this.OutTangent = outTangent;
            this.Time = time;
            this.Value = value;
        }

        /// <summary>
        /// Create an XmlElement contains Keyframe data
        /// </summary>
        /// <returns>XmlElement</returns>
        public XmlElement ToXmlElement()
        {
            XmlElement kf = new XmlElement(ElementName);
            kf.SetAttributeValue("Time", this.Time);
            kf.SetAttributeValue("Value", this.Value);
            kf.SetAttributeValue("InTangent", this.InTangent);
            kf.SetAttributeValue("OutTangent", this.OutTangent);
            return kf;
        }

        /// <summary>
        /// Load Keyframe data from given XmlElement
        /// </summary>
        /// <param name="e"></param>
        public void Load(XmlElement e)
        {
            this.Time = e.GetAttributeValueAsFloat("Time", this.Time);
            this.Value = e.GetAttributeValueAsFloat("Value", this.Value);
            this.InTangent = e.GetAttributeValueAsFloat("InTangent", this.InTangent);
            this.OutTangent = e.GetAttributeValueAsFloat("OutTangent", this.OutTangent);
        }
    }

    /// <summary>
    /// Collection of Keyframe
    /// </summary>
    public class KeyframeDataCollection : ICollection<KeyframeData>, IXmlElementSerializable
    {
        public const string ElementName = "Keyframes";

        private List<KeyframeData> _Keys;


        public KeyframeData this[int index]
        {
            get { return _Keys[index]; }            
        }

        public KeyframeDataCollection()
        {
            _Keys = new List<KeyframeData>();
        }

        public KeyframeDataCollection(KeyframeData[] keys)
        {
            _Keys = new List<KeyframeData>(keys);
        }

        public XmlElement ToXmlElement()
        {
            XmlElement keyframes = new XmlElement(ElementName);

            keyframes.SetAttributeValue("Count", Count);
            for (int i = 0; i < _Keys.Count; i++)
            {
                keyframes.AppendChild(_Keys[i].ToXmlElement());
            }
            return keyframes;
        }

        public void Load(XmlElement e)
        {
            _Keys.Clear();
            int count = e.GetAttributeValueAsInt("Count", 0);
            _Keys.Capacity = count;

            foreach (var item in e)
            {
                if (item.Name == KeyframeData.ElementName)
                {
                    KeyframeData k = new KeyframeData();
                    k.Load(item);
                    _Keys.Add(k);
                }
            }
        }

        public void Add(KeyframeData item)
        {
            _Keys.Add(item);
        }

        public void Clear()
        {
            _Keys.Clear();
        }

        public bool Contains(KeyframeData item)
        {
            return _Keys.Contains(item);
        }

        public void CopyTo(KeyframeData[] array, int arrayIndex)
        {
            _Keys.CopyTo(array, arrayIndex);
        }

        public int Count { get { return _Keys.Count; } }

        public bool IsReadOnly { get { return false; } }

        public bool Remove(KeyframeData item)
        {
            return _Keys.Remove(item);
        }

        public void Remove(int index)
        {
            _Keys.RemoveAt(index);
        }

        public IEnumerator<KeyframeData> GetEnumerator()
        {
            return _Keys.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Keys as System.Collections.IEnumerable).GetEnumerator();
        }
    }

}
