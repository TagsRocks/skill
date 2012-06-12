using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.IO
{
    public class SaveClass : IXElement, ICollection<SaveProperty>
    {
        public string Name { get; set; }

        public SaveClass()
        {
        }

        public XElement ToXElement()
        {
            XElement saveclass = new XElement("SaveClass");
            saveclass.SetAttributeValue("Name", Name);

            XElement properties = new XElement("Properties");

            foreach (var item in this)
            {
                properties.Add(item.ToXElement());
            }

            saveclass.Add(properties);

            WriteAttributes(saveclass);

            return saveclass;
        }
        /// <summary> subclasses can add aditional data to save in file </summary>
        /// <param name="e"></param>
        protected virtual void WriteAttributes(XElement e) { }

        public void Load(XElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", this.Name);

            this.Clear();

            XElement properties = e.FindChildByName("Properties");
            if (properties != null)
            {
                foreach (var element in properties.Elements())
                {
                    SaveProperty p = CreateProperty(element);
                    if (p != null)
                    {
                        p.Load(element);
                        this.Add(p);
                    }
                }
            }
            ReadAttributes(e);
        }

        /// <summary>
        /// subclass can load additional data here
        /// </summary>
        /// <param name="e">contains behavior data</param>
        protected virtual void ReadAttributes(XElement e) { }

        private static SaveProperty CreateProperty(XElement e)
        {
            SaveProperty result = null;

            int type = e.GetAttributeValueAsInt("PropertyType", -1);
            if (type > -1)
            {
                return CreateProperty((PropertyType)type);
            }

            return result;
        }

        public static SaveProperty CreateProperty(PropertyType type)
        {
            switch (type)
            {
                case PropertyType.Primitive:
                    return new PrimitiveProperty();
                case PropertyType.Class:
                    return new ClassProperty();
                default:
                    return null;
            }
        }


        #region ICollection<SaveGameProperty> member
        private List<SaveProperty> _Children = new List<SaveProperty>();
        public void Add(SaveProperty item)
        {
            _Children.Add(item);
        }

        public void Clear()
        {
            _Children.Clear();
        }

        public bool Contains(SaveProperty item)
        {
            return _Children.Contains(item);
        }

        public void CopyTo(SaveProperty[] array, int arrayIndex)
        {
            _Children.CopyTo(array, arrayIndex);
        }

        public int Count { get { return _Children.Count; } }

        public bool IsReadOnly { get { return false; } }

        public bool Remove(SaveProperty item)
        {
            return _Children.Remove(item);
        }

        public IEnumerator<SaveProperty> GetEnumerator()
        {
            return _Children.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Children as System.Collections.IEnumerable).GetEnumerator();
        }

        #endregion
    }
}
