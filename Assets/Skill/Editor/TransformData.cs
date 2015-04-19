using UnityEngine;
using System.Collections;
using Skill.Framework.IO;
using System.Collections.Generic;

namespace Skill.Editor
{
    public class TransformData : IXmlElementSerializable
    {


        public string Name { get; set; }
        public Vector3Data Position { get { return _Position; } set { _Position = value; } }
        public Vector3Data LocalPosition { get { return _LocalPosition; } set { _LocalPosition = value; } }
        public Vector3Data LocalScale { get { return _LocalScale; } set { _LocalScale = value; } }
        public QuaternionData Rotation { get { return _Rotation; } set { _Rotation = value; } }
        public QuaternionData LocalRotation { get { return _LocalRotation; } set { _LocalRotation = value; } }

        private Vector3Data _Position;
        private Vector3Data _LocalPosition;
        private Vector3Data _LocalScale;
        private QuaternionData _Rotation;
        private QuaternionData _LocalRotation;
        private List<TransformData> _Children;

        public void Add(TransformData t)
        {
            _Children.Add(t);
        }
        public bool Remove(TransformData t)
        {
            return _Children.Remove(t);
        }
        public int Count { get { return _Children.Count; } }

        public TransformData this[int index] { get { return _Children[index]; } }

        public TransformData()
        {
            _Children = new List<TransformData>();
            Name = "Transform";
            Position = new Vector3Data();
            LocalPosition = new Vector3Data();
            LocalScale = new Vector3Data();
            Rotation = new QuaternionData();
            LocalRotation = new QuaternionData();
        }

        public void Copy(Transform t)
        {
            this.Name = t.name;
            this._Position.Import(t.position);
            this._LocalPosition.Import(t.localPosition);
            this._LocalScale.Import(t.localScale);
            this._Rotation.Import(t.rotation);
            this._LocalRotation.Import(t.localRotation);

            _Children.Clear();
            for (int i = 0; i < t.childCount; i++)
            {
                TransformData tdc = new TransformData();
                tdc.Copy(t.GetChild(i));
                Add(tdc);
            }
        }

        public void Paste(Transform t)
        {
            t.localScale = _LocalScale.ToVector3();
            if (t.parent == null)
            {
                t.position = _Position.ToVector3();
                t.rotation = _Rotation.ToQuaternion();
            }
            else
            {
                t.localPosition = _LocalPosition.ToVector3();
                t.localRotation = _LocalRotation.ToQuaternion();
            }

            for (int i = 0; i < Count; i++)
            {
                TransformData c = _Children[i];
                Transform child = t.FindChild(c.Name);
                if (child != null)
                    c.Paste(child);
            }
        }


        public XmlElement ToXmlElement()
        {
            XmlElement t = new XmlElement("Transform");
            if (Name == null) Name = string.Empty;
            t.SetAttributeValue("TName", Name);

            XmlElement p = _Position.ToXmlElement(); p.Name = "Position";
            XmlElement lp = _LocalPosition.ToXmlElement(); lp.Name = "LocalPosition";
            XmlElement ls = _LocalScale.ToXmlElement(); ls.Name = "LocalScale";
            XmlElement r = _Rotation.ToXmlElement(); r.Name = "Rotation";
            XmlElement lr = _LocalRotation.ToXmlElement(); lr.Name = "LocalRotation";

            t.AppendChild(p);
            t.AppendChild(lp);
            t.AppendChild(ls);
            t.AppendChild(r);
            t.AppendChild(lr);



            if (this._Children.Count > 0)
            {
                XmlElement children = new XmlElement("Children");
                children.SetAttributeValue("Count", this._Children.Count);
                foreach (var item in this._Children)
                {
                    children.AppendChild(item.ToXmlElement());
                }
                t.AppendChild(children);
            }


            return t;
        }

        public void Load(XmlElement e)
        {
            this.Name = e.GetAttributeValueAsString("TName", "Transform");

            XmlElement p = e["Position"];
            XmlElement lp = e["LocalPosition"];
            XmlElement ls = e["LocalScale"];
            XmlElement r = e["Rotation"];
            XmlElement lr = e["LocalRotation"];

            if (p != null) _Position.Load(p);
            if (lp != null) _LocalPosition.Load(lp);
            if (ls != null) _LocalScale.Load(ls);
            if (r != null) _Rotation.Load(r);
            if (lr != null) _LocalRotation.Load(lr);


            _Children.Clear();
            XmlElement children = e["Children"];
            if (children != null)
            {
                int count = children.GetAttributeValueAsInt("Count", 0);
                if (count > 0)
                {
                    this._Children.Capacity = count;
                    foreach (var element in children)
                    {
                        TransformData t = new TransformData();
                        t.Load(element);
                        _Children.Add(t);
                    }
                }
            }
        }
    }
}
