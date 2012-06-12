using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.AI
{
    #region BehaviorType
    /// <summary>
    /// Defines types of behaviors
    /// </summary>
    public enum BehaviorType
    {
        Action,
        Condition,
        Decorator,
        Composite,
    }
    #endregion

    #region Behavior
    /// <summary>
    /// Base class for all behaviors
    /// </summary>
    public abstract class Behavior : IXElement, ICollection<Behavior>
    {
        #region Properties
        /// <summary> Returns type of behavior. all subclass must implement this properties </summary>
        public abstract BehaviorType BehaviorType { get; }

        /// <summary> Name of behavior </summary>
        public virtual string Name { get; set; }

        /// <summary> Id of behavior </summary>
        public int Id { get; set; }

        /// <summary> If true code generator create an method and hook it to success event </summary>
        public bool SuccessEvent { get; set; }

        /// <summary> If true code generator create an method and hook it to failure event </summary>
        public bool FailureEvent { get; set; }

        /// <summary> If true code generator create an method and hook it to running event </summary>
        public bool RunningEvent { get; set; }

        /// <summary> If true code generator create an method and hook it to reset event </summary>
        public bool ResetEvent { get; set; }

        /// <summary> Weight of node when behavior is child of a random selector </summary>
        public float Weight { get; set; }

        /// <summary> User comment for this behavior </summary>
        public string Comment { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Creatre an instance of behavior
        /// </summary>
        /// <param name="name">Name of behavior</param>
        public Behavior(string name)
        {
            _Behaviors = new List<Behavior>();
            _Parameters = new List<ParameterCollection>();
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
        protected virtual void WriteAttributes(XElement e) { }

        /// <summary>
        /// Create a XElement that contains all data required to save in file
        /// </summary>
        /// <returns></returns>
        public XElement ToXElement()
        {
            XElement behavior = new XElement("Behavior");
            behavior.SetAttributeValue("BehaviorType", BehaviorType.ToString());
            behavior.SetAttributeValue("Name", Name);
            behavior.SetAttributeValue("Id", Id);
            XElement events = new XElement("Events");
            events.SetAttributeValue("Failure", FailureEvent);
            events.SetAttributeValue("Success", SuccessEvent);
            events.SetAttributeValue("Running", RunningEvent);
            events.SetAttributeValue("Reset", ResetEvent);

            if (!string.IsNullOrEmpty(Comment))
            {
                XElement comment = new XElement("Comment");
                comment.SetValue(Comment);
                behavior.Add(comment);
            }
            behavior.Add(events);
            WriteAttributes(behavior); // allow subclass to add additional data
            return behavior;
        }
        #endregion

        #region Load

        protected static XElement FindChild(XElement e, string name)
        {
            foreach (var item in e.Elements().Where(p => p.Name == name))
                return item;
            return null;
        }

        /// <summary>
        /// subclass can load additional data here
        /// </summary>
        /// <param name="e">contains behavior data</param>
        protected virtual void ReadAttributes(XElement e) { }

        /// <summary>
        /// Load behavior data
        /// </summary>
        /// <param name="e">contains behavior data</param>
        public void Load(XElement e)
        {
            Name = e.Attribute("Name").Value;
            Id = int.Parse(e.Attribute("Id").Value);

            XElement events = FindChild(e, "Events");
            if (events != null)
            {
                this.FailureEvent = events.GetAttributeValueAsBoolean("Failure", false);
                this.SuccessEvent = events.GetAttributeValueAsBoolean("Success", false);
                this.RunningEvent = events.GetAttributeValueAsBoolean("Running", false);
                this.ResetEvent = events.GetAttributeValueAsBoolean("Reset", false);
            }

            XElement comment = FindChild(e, "Comment");
            if (comment != null)
            {
                Comment = comment.Value;
            }

            ReadAttributes(e);// allow subclass to read additional data
        }
        #endregion

        #region ICollection<Behavior> methods

        /// <summary>
        /// Moves the item at the specified index to a new location in the collection.
        /// </summary>
        /// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
        /// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
        public void Move(int oldIndex, int newIndex)
        {
            //if (oldIndex < 0 || oldIndex >= Count) throw new IndexOutOfRangeException("Invalid index of child behavior");
            //if (newIndex < 0 || newIndex >= Count) throw new IndexOutOfRangeException("Invalid index of child behavior");
            var item = this._Behaviors[oldIndex];
            this._Behaviors.RemoveAt(oldIndex);
            this._Behaviors.Insert(newIndex, item);

            var ps = this._Parameters[oldIndex];
            this._Parameters.RemoveAt(oldIndex);
            this._Parameters.Insert(newIndex, ps);
        }

        /// <summary>
        /// Retrieves child by index
        /// </summary>
        /// <param name="index">index of child</param>
        /// <returns>child at given index</returns>
        public Behavior this[int index]
        {
            get { return _Behaviors[index]; }
        }

        /// <summary>
        /// Retrieves parameters of child at specified index
        /// </summary>
        /// <param name="index">index of child</param>
        /// <returns>parameters of child</returns>
        public ParameterCollection GetParameters(int index)
        {
            return _Parameters[index];
        }

        private List<Behavior> _Behaviors; // list of children
        private List<ParameterCollection> _Parameters; // Parameters of children

        /// <summary>
        /// Insert a child Behavior at specified index.
        /// </summary>        
        /// <param name="index">the zero-based index at witch item should be inserted</param>
        /// <param name="item">Behavior to add</param>
        public void Insert(int index, Behavior item)
        {
            this.Insert(index, item, new ParameterCollection());
        }


        /// <summary>
        /// Insert a child Behavior at specified index.
        /// </summary>        
        /// <param name="index">the zero-based index at witch item should be inserted</param>
        /// <param name="item">Behavior to add</param>        
        /// <param name="parameters">Parameters for behavior</param>
        public void Insert(int index, Behavior item, ParameterCollection parameters)
        {
            _Behaviors.Insert(index, item);
            _Parameters.Insert(index, parameters);
        }

        /// <summary>
        /// Add a child behavior.
        /// </summary>
        /// <param name="item">behavior to add</param>
        /// <remarks>we don't control type of behavior here since controlled in ViewModel</remarks>
        public void Add(Behavior item)
        {
            this.Add(item, new ParameterCollection());
        }

        /// <summary>
        /// Add a child behavior.
        /// </summary>
        /// <param name="item">behavior to add</param>        
        /// <param name="parameters">Parameters of behavior</param>
        /// <remarks>we don't control type of behavior here since controlled in ViewModel</remarks>
        public void Add(Behavior item, ParameterCollection parameters)
        {
            _Behaviors.Add(item);
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
        public bool Contains(Behavior item)
        {
            return _Behaviors.Count(b => b == item) > 0;
        }

        /// <summary>
        /// Copy children to an array
        /// </summary>
        /// <param name="array">array to fill</param>
        /// <param name="arrayIndex">start index in array to fill</param>
        public void CopyTo(Behavior[] array, int arrayIndex)
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
        public bool Remove(Behavior item)
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
        /// Enumerator for children
        /// </summary>
        /// <returns>Enumerator for children</returns>
        public IEnumerator<Behavior> GetEnumerator()
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
    }
    #endregion
}
