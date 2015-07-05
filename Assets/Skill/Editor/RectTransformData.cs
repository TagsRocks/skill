using UnityEngine;
using System.Collections;
using Skill.Framework.IO;
using System.Collections.Generic;

namespace Skill.Editor
{
    public class RectTransformData : IXmlElementSerializable
    {

        // The position of the pivot of this RectTransform relative to the anchor reference point.
        public Vector2Data AnchoredPosition { get { return _AnchoredPosition; } set { _AnchoredPosition = value; } }
        // The 3D position of the pivot of this RectTransform relative to the anchor reference point.
        public Vector3Data AnchoredPosition3D { get { return _AnchoredPosition3D; } set { _AnchoredPosition3D = value; } }
        // The normalized position in the parent RectTransform that the upper right corner is anchored to.
        public Vector2Data AnchorMax { get { return _AnchorMax; } set { _AnchorMax = value; } }
        // The normalized position in the parent RectTransform that the lower left corner is anchored to.
        public Vector2Data AnchorMin { get { return _AnchorMin; } set { _AnchorMin = value; } }
        // The offset of the upper right corner of the rectangle relative to the upper right anchor.
        public Vector2Data OffsetMax { get { return _OffsetMax; } set { _OffsetMax = value; } }
        // The offset of the lower left corner of the rectangle relative to the lower left anchor.
        public Vector2Data OffsetMin { get { return _OffsetMin; } set { _OffsetMin = value; } }
        // The normalized position in this RectTransform that it rotates around.
        public Vector2Data Pivot { get { return _Pivot; } set { _Pivot = value; } }
        
        // The size of this RectTransform relative to the distances between the anchors.
        public Vector2Data SizeDelta { get { return _SizeDelta; } set { _SizeDelta = value; } }


        private Vector2Data _AnchoredPosition;
        private Vector3Data _AnchoredPosition3D;
        private Vector2Data _AnchorMax;
        private Vector2Data _AnchorMin;
        private Vector2Data _OffsetMax;
        private Vector2Data _OffsetMin;
        private Vector2Data _Pivot;        
        private Vector2Data _SizeDelta;        

        public TransformData Owner { get; set; }

        public RectTransformData(TransformData owner = null)
        {
            this.Owner = owner;
            this._AnchoredPosition = new Vector2Data();
            this._AnchoredPosition3D = new Vector3Data();
            this._AnchorMax = new Vector2Data();
            this._AnchorMin = new Vector2Data();
            this._OffsetMax = new Vector2Data();
            this._OffsetMin = new Vector2Data();
            this._Pivot = new Vector2Data();            
            this._SizeDelta = new Vector2Data();
        }

        public void Copy(RectTransform rt)
        {
            this._AnchoredPosition.Import(rt.anchoredPosition);
            this._AnchoredPosition3D.Import(rt.anchoredPosition3D);
            this._AnchorMax.Import(rt.anchorMax);
            this._AnchorMin.Import(rt.anchorMin);
            this._OffsetMax.Import(rt.offsetMax);
            this._OffsetMin.Import(rt.offsetMin);
            this._Pivot.Import(rt.pivot);            
            this._SizeDelta.Import(rt.sizeDelta);
        }

        public void Paste(RectTransform rt)
        {
            rt.anchoredPosition = this._AnchoredPosition.ToVector2();
            rt.anchoredPosition3D = this._AnchoredPosition3D.ToVector3();
            rt.anchorMax = this._AnchorMax.ToVector2();
            rt.anchorMin = this._AnchorMin.ToVector2();
            rt.offsetMax = this._OffsetMax.ToVector2();
            rt.offsetMin = this._OffsetMin.ToVector2();
            rt.pivot = this._Pivot.ToVector2();            
            rt.sizeDelta = this._SizeDelta.ToVector2();
        }


        public XmlElement ToXmlElement()
        {
            XmlElement t = new XmlElement("RectTransform");

            XmlElement p1 = this._AnchoredPosition.ToXmlElement(); p1.Name = "AnchoredPosition";
            XmlElement p2 = this._AnchoredPosition3D.ToXmlElement(); p2.Name = "AnchoredPosition3D";
            XmlElement p3 = this._AnchorMax.ToXmlElement(); p3.Name = "AnchorMax";
            XmlElement p4 = this._AnchorMin.ToXmlElement(); p4.Name = "AnchorMin";
            XmlElement p5 = this._OffsetMax.ToXmlElement(); p5.Name = "OffsetMax";
            XmlElement p6 = this._OffsetMin.ToXmlElement(); p6.Name = "OffsetMin";
            XmlElement p7 = this._Pivot.ToXmlElement(); p7.Name = "Pivot";            
            XmlElement p9 = this._SizeDelta.ToXmlElement(); p9.Name = "SizeDelta";

            t.AppendChild(p1);
            t.AppendChild(p2);
            t.AppendChild(p3);
            t.AppendChild(p4);
            t.AppendChild(p5);
            t.AppendChild(p6);
            t.AppendChild(p7);            
            t.AppendChild(p9);

            return t;
        }

        public void Load(XmlElement e)
        {

            XmlElement p1 = e["AnchoredPosition"];
            XmlElement p2 = e["AnchoredPosition3D"];
            XmlElement p3 = e["AnchorMax"];
            XmlElement p4 = e["AnchorMin"];
            XmlElement p5 = e["OffsetMax"];
            XmlElement p6 = e["OffsetMin"];
            XmlElement p7 = e["Pivot"];            
            XmlElement p9 = e["SizeDelta"];

            if (p1 != null) this._AnchoredPosition.Load(p1);
            if (p2 != null) this._AnchoredPosition3D.Load(p2);
            if (p3 != null) this._AnchorMax.Load(p3);
            if (p4 != null) this._AnchorMin.Load(p4);
            if (p5 != null) this._OffsetMax.Load(p5);
            if (p6 != null) this._OffsetMin.Load(p6);
            if (p7 != null) this._Pivot.Load(p7);            
            if (p9 != null) this._SizeDelta.Load(p9);
        }
    }
}
