using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.AI
{

    #region Behavior
    /// <summary>
    /// Base class for all behaviors
    /// </summary>
    public abstract class BehaviorData : IXmlElementSerializable, ICollection<BehaviorData>
    {
        #region Properties
        /// <summary> Returns type of behavior. all subclass must implement this properties </summary>
        public abstract Skill.Framework.AI.BehaviorType BehaviorType { get; }

        /// <summary> Name of behavior </summary>
        public virtual string Name { get; set; }

        /// <summary> Id of behavior </summary>
        public int Id { get; set; }

        /// <summary> Weight of node when behavior is child of a random selector </summary>
        public float Weight { get; set; }

        /// <summary> User comment for this behavior </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Behavior of node when is child of a ConcurrentSelector
        /// </summary>
        public Skill.Framework.AI.ConcurrencyMode Concurrency { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Creatre an instance of behavior
        /// </summary>
        /// <param name="name">Name of behavior</param>
        public BehaviorData(string name)
        {
            _Behaviors = new List<BehaviorData>();
            _Parameters = new List<ParameterDataCollection>();
            this.Name = name;
            this.Weight = 1;
        }
        #endregion

        #region Helper save methods
        /// <summary>
        /// convert back children index string to array of int. used in saving and loading behavior
        /// </summary>
        public static int[] ConvertToIndices(string childrenString)
        {
            List<int> list = new List<int>();
            if (!string.IsNullOrEmpty(childrenString))
            {
                string[] splip = childrenString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < splip.Length; i++)
                {
                    list.Add(int.Parse(splip[i]));
                }
            }
            return list.ToArray();
        }
        /// <summary>
        /// convert index of children to string that seperated with ,
        /// </summary>
        /// <returns>Index of chilren</returns>
        public string GetChildrenString()
        {
            string childrenString = "";
            for (int i = 0; i < Count; i++)
            {
                childrenString += this[i].Id.ToString("D");
                if (i < Count - 1)
                    childrenString += ",";
            }
            return childrenString;
        }
        #endregion

        #region Save
        /// <summary> subclasses can add aditional data to save in file </summary>
        /// <param name="e"></param>
        protected virtual void WriteAttributes(XmlElement e) { }

        /// <summary>
        /// Create a XmlElement that contains all data required to save in file
        /// </summary>
        /// <returns></returns>
        public XmlElement ToXmlElement()
        {
            XmlElement behavior = new XmlElement("Behavior");
            behavior.SetAttributeValue("BehaviorType", BehaviorType.ToString());
            behavior.SetAttributeValue("Name", Name);
            behavior.SetAttributeValue("Id", Id);
            behavior.SetAttributeValue("Weight", Weight);
            behavior.SetAttributeValue("Concurrency", Concurrency.ToString());

            //if (_Parameters != null && _Parameters.Count > 0)
            //{
            //    XmlElement childParameters = new XmlElement("ChildParameters");
            //    foreach (var p in _Parameters)
            //        childParameters.AppendChild(p.ToXmlElement());
            //    behavior.AppendChild(childParameters);
            //}

            if (!string.IsNullOrEmpty(Comment))
            {
                XmlElement comment = new XmlElement("Comment");
                comment.Value = Comment;
                behavior.AppendChild(comment);
            }


            WriteAttributes(behavior); // allow subclass to add additional data
            return behavior;
        }
        #endregion

        #region Load

        /// <summary>
        /// subclass can load additional data here
        /// </summary>
        /// <param name="e">contains behavior data</param>
        protected virtual void ReadAttributes(XmlElement e) { }

        /// <summary>
        /// Load behavior data
        /// </summary>
        /// <param name="e">contains behavior data</param>
        public void Load(XmlElement e)
        {
            Name = e.GetAttributeValueAsString("Name", Name);
            Id = int.Parse(e.GetAttributeValueAsString("Id", "-1"));
            Weight = e.GetAttributeValueAsFloat("Weight", 1);
            Concurrency = e.GetAttributeValueAsEnum<Skill.Framework.AI.ConcurrencyMode>("Concurrency", Skill.Framework.AI.ConcurrencyMode.Unlimit);

            //_Parameters.Clear();
            //XmlElement childParameters = e["ChildParameters"];
            //if (childParameters != null)
            //{
            //    foreach (var p in childParameters)
            //    {
            //        ParameterDataCollection pdc = new ParameterDataCollection();
            //        pdc.Load(p);
            //        _Parameters.Add(pdc);
            //    }
            //}            

            XmlElement comment = e["Comment"];
            if (comment != null)
            {
                Comment = comment.Value;
            }

            ReadAttributes(e);// allow subclass to read additional data
        }

        public void FixParameters()
        {
            if (_Parameters.Count < Count)
            {
                for (int i = _Parameters.Count; i < Count; i++)
                    _Parameters.Add(new ParameterDataCollection());
            }
            else if (_Parameters.Count > Count)
            {
                _Parameters.RemoveRange(Count, _Parameters.Count - Count);
            }
        }
        #endregion

        #region ICollection<Behavior> methods

        public ParameterDataCollection GetParameters(BehaviorData b) { return GetParameters(IndexOf(b)); }

        public ParameterDataCollection GetParameters(int childIndex)
        {
            if (childIndex >= 0 && childIndex < _Parameters.Count)
                return _Parameters[childIndex];
            return null;
        }


        /// <summary>
        /// Moves the item at the specified index to a new location in the collection.
        /// </summary>
        /// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
        /// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
        public void Move(int oldIndex, int newIndex)
        {
            var item = this._Behaviors[oldIndex];
            this._Behaviors.RemoveAt(oldIndex);
            this._Behaviors.Insert(newIndex, item);

            ParameterDataCollection parmeters = _Parameters[oldIndex];
            this._Parameters.RemoveAt(oldIndex);
            this._Parameters.Insert(newIndex, parmeters);
        }

        /// <summary>
        /// Retrieves child by index
        /// </summary>
        /// <param name="index">index of child</param>
        /// <returns>child at given index</returns>
        public BehaviorData this[int index]
        {
            get { return _Behaviors[index]; }
        }
        private List<BehaviorData> _Behaviors; // list of children
        private List<ParameterDataCollection> _Parameters;



        /// <summary>
        /// Insert a child Behavior at specified index.
        /// </summary>        
        /// <param name="index">the zero-based index at witch item should be inserted</param>
        /// <param name="item">Behavior to add</param>                        
        /// <param name="parameters">parameters</param>
        public void Insert(int index, BehaviorData item, ParameterDataCollection parameters)
        {
            _Behaviors.Insert(index, item);
            if (item is IParameterData)
                parameters.Match(((IParameterData)item).ParameterDifinition);
            _Parameters.Insert(index, parameters);
        }

        /// <summary>
        /// Insert a child Behavior at specified index.
        /// </summary>        
        /// <param name="index">the zero-based index at witch item should be inserted</param>
        /// <param name="item">Behavior to add</param>                
        public void Insert(int index, BehaviorData item)
        {
            Insert(index, item, new ParameterDataCollection());
        }

        /// <summary>
        /// Add a child behavior.
        /// </summary>
        /// <param name="item">behavior to add</param>                
        /// <remarks>we don't control type of behavior here since controlled in ViewModel</remarks>
        public void Add(BehaviorData item)
        {
            Add(item, new ParameterDataCollection());
        }

        /// <summary>
        /// Add a child behavior.
        /// </summary>
        /// <param name="item">behavior to add</param>                
        /// <remarks>we don't control type of behavior here since controlled in ViewModel</remarks>
        public void Add(BehaviorData item, ParameterDataCollection parameters)
        {
            _Behaviors.Add(item);
            if (item is IParameterData)
                parameters.Match(((IParameterData)item).ParameterDifinition);
            _Parameters.Add(parameters);
        }


        /// <summary>
        /// Remove all children
        /// </summary>
        public void Clear()
        {
            _Behaviors.Clear();
            _Parameters.Clear();
        }

        /// <summary>
        /// Return true if contains given child
        /// </summary>
        /// <param name="item">child to check</param>
        /// <returns>true if contains, otherwise false</returns>
        public bool Contains(BehaviorData item)
        {
            return _Behaviors.Contains(item);
        }


        /// <summary>
        /// Searches for the specified Behavior and returns the zero-based index of the
        /// first occurrence within the entire childeren.
        /// </summary>
        /// <param name="item"> The Behavior to locate in the children. The value can be null for reference types.</param>
        /// <returns> The zero-based index of the first occurrence of item within the children, if found; otherwise, –1. </returns>
        public int IndexOf(BehaviorData item)
        {
            return _Behaviors.IndexOf(item);
        }


        /// <summary>
        /// Copy children to an array
        /// </summary>
        /// <param name="array">array to fill</param>
        /// <param name="arrayIndex">start index in array to fill</param>
        public void CopyTo(BehaviorData[] array, int arrayIndex)
        {
            _Behaviors.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Count of children
        /// </summary>
        public int Count
        {
            get { return _Behaviors.Count; }
        }

        /// <summary>
        /// Collection is not readonly
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Remove given child
        /// </summary>
        /// <param name="item">child to remove</param>
        /// <returns></returns>
        public bool Remove(BehaviorData item)
        {
            int index = _Behaviors.IndexOf(item);
            if (index >= 0)
            {
                _Behaviors.RemoveAt(index);
                _Parameters.RemoveAt(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Replace specified child
        /// </summary>
        /// <param name="oldItem">old child to replace</param>
        /// <param name="newItem">new child to place</param>
        /// <returns>True if sucess, otherwise false</returns>
        public bool Replace(BehaviorData oldItem, BehaviorData newItem)
        {
            int index = _Behaviors.IndexOf(oldItem);
            if (index >= 0)
            {
                _Behaviors.RemoveAt(index);
                _Behaviors.Insert(index, newItem);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Enumerator for children
        /// </summary>
        /// <returns>Enumerator for children</returns>
        public IEnumerator<BehaviorData> GetEnumerator()
        {
            return _Behaviors.GetEnumerator();
        }

        /// <summary>
        /// Enumerator for children
        /// </summary>
        /// <returns>Enumerator for children</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Behaviors as System.Collections.IEnumerable).GetEnumerator();
        }
        #endregion

        public override string ToString()
        {
            return Name;
        }
        public UnityEngine.Texture2D GetIcon()
        {
            switch (BehaviorType)
            {
                case Skill.Framework.AI.BehaviorType.Action:
                    return Skill.Editor.Resources.UITextures.BTree.Action;
                case Skill.Framework.AI.BehaviorType.Condition:
                    return Skill.Editor.Resources.UITextures.BTree.Condition;
                case Skill.Framework.AI.BehaviorType.Decorator:

                    switch (((DecoratorData)this).Type)
                    {
                        case Skill.Framework.AI.DecoratorType.Default:
                            return Skill.Editor.Resources.UITextures.BTree.Decorator;
                        case Skill.Framework.AI.DecoratorType.AccessLimit:
                            return Skill.Editor.Resources.UITextures.BTree.AccessLimitDecorator;
                    }

                    break;
                case Skill.Framework.AI.BehaviorType.Composite:

                    switch (((CompositeData)this).CompositeType)
                    {
                        case Skill.Framework.AI.CompositeType.Sequence:
                            return Skill.Editor.Resources.UITextures.BTree.Sequence;
                        case Skill.Framework.AI.CompositeType.Concurrent:
                            return Skill.Editor.Resources.UITextures.BTree.Concurrent;
                        case Skill.Framework.AI.CompositeType.Random:
                            return Skill.Editor.Resources.UITextures.BTree.Random;
                        case Skill.Framework.AI.CompositeType.Priority:
                            return Skill.Editor.Resources.UITextures.BTree.Priority;
                        case Skill.Framework.AI.CompositeType.Loop:
                            return Skill.Editor.Resources.UITextures.BTree.Loop;
                        case Skill.Framework.AI.CompositeType.State:
                            return Skill.Editor.Resources.UITextures.BTree.Priority;
                        default:
                            break;
                    }
                    break;
                case Skill.Framework.AI.BehaviorType.ChangeState:
                    return Skill.Editor.Resources.UITextures.BTree.ChangeState;
            }
            return null;
        }

    }
    #endregion
}
