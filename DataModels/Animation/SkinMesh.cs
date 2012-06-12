using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Skill.DataModels.Animation
{
    /// <summary>
    /// Represent a skinmesh witch includes a hierarchy of bones
    /// </summary>
    public class SkinMesh : IXElement
    {
        #region Properties
        /// <summary> Root of skinmesh </summary>
        public Bone Root { get; set; }
        /// <summary> Name of skinmesh. this name is based on filename and will set after loading from file </summary>
        public string Name { get; set; }
        /// <summary> Animations of skinmesh </summary>
        public AnimationClip[] Animations { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Create an instance of SkinMesh
        /// </summary>
        public SkinMesh()
        {
            this.Name = "NewSkinMesh";
            this.Root = new Bone("Root");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Check whether specyfied bone is in hierarchy or unused
        /// </summary>
        /// <param name="bone">Bone to check</param>
        /// <returns>True if is in hierarchy, otherwise false</returns>
        public bool IsInHierarchy(Bone bone)
        {
            return IsInHierarchy(Root, bone);
        }

        private bool IsInHierarchy(Bone parentBone, Bone bone)
        {
            if (bone == parentBone) return true;
            foreach (var item in parentBone)
            {
                if (IsInHierarchy(item, bone))
                    return true;
            }
            return false;
        }
        #endregion

        #region Save
        /// <summary>
        /// Create a XElement containing SkinMesh data
        /// </summary>
        /// <returns>XElement</returns>
        public XElement ToXElement()
        {
            XElement skinmesh = new XElement("SkinMesh");
            skinmesh.SetAttributeValue("Name", Name);

            XElement bones = new XElement("Bones");
            bones.Add(Root.ToXElement());
            skinmesh.Add(bones);

            XElement animations = new XElement("Animations");
            if (Animations != null)
            {
                animations.SetAttributeValue("Count", Animations.Length);
                foreach (var clip in Animations)
                {
                    animations.Add(clip.ToXElement());
                }
            }
            else
                animations.SetAttributeValue("Count", 0);
            skinmesh.Add(animations);
            return skinmesh;
        }


        #endregion

        #region Load

        /// <summary>
        /// Load SkinMesh data from XElement
        /// </summary>
        /// <param name="e">XElement to load from</param>
        public void Load(XElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", this.Name);
            XElement bones = e.FindChildByName("Bones");
            if (bones != null)
            {
                XElement root = bones.FindChildByName("Bone");
                if (root != null)
                {
                    this.Root = new Bone();
                    this.Root.Load(root);
                }
            }

            XElement animations = e.FindChildByName("Animations");
            if (animations != null)
            {
                int count = animations.GetAttributeValueAsInt("Count", 0);
                Animations = new AnimationClip[count];
                int i = 0;

                foreach (var element in animations.Elements())
                {
                    AnimationClip clip = new AnimationClip();
                    clip.Load(element);
                    Animations[i++] = clip;
                }
            }
        }
        #endregion

    }

    #region bone
    /// <summary>
    /// Represent a bone in hierarchy of SkinMesh
    /// </summary>
    public class Bone : IXElement, ICollection<Bone>
    {
        #region Properties

        /// <summary> Name of bone </summary>
        public virtual string Name { get; set; }


        #endregion

        #region Constructor
        /// <summary>
        /// Creatre an instance of bone
        /// </summary>
        /// <param name="name">Name of bone</param>
        public Bone(string name)
        {
            _Children = new List<Bone>();
            this.Name = name;
        }

        /// <summary>
        /// Creatre an instance of bone
        /// </summary>        
        public Bone()
            : this("NewBone")
        {
        }
        #endregion

        #region Save

        /// <summary>
        /// Create a XElement that contains all data required to save in file
        /// </summary>
        /// <returns></returns>
        public XElement ToXElement()
        {
            XElement bone = new XElement("Bone");
            bone.SetAttributeValue("Name", Name);
            foreach (var b in this)
                bone.Add(b.ToXElement());

            return bone;
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
        /// Load bone data
        /// </summary>
        /// <param name="e">contains bone data</param>
        public void Load(XElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", this.Name);
            Clear();
            foreach (var item in e.Elements())
            {
                Bone childBone = new Bone();
                childBone.Load(item);
                this.Add(childBone);
            }
        }
        #endregion

        #region ICollection<Bone> methods

        /// <summary>
        /// Moves the item at the specified index to a new location in the collection.
        /// </summary>
        /// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
        /// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
        public void Move(int oldIndex, int newIndex)
        {
            var item = this._Children[oldIndex];
            this._Children.RemoveAt(oldIndex);
            this._Children.Insert(newIndex, item);
        }

        /// <summary>
        /// Retrieves child by index
        /// </summary>
        /// <param name="index">index of child</param>
        /// <returns>child at given index</returns>
        public Bone this[int index]
        {
            get { return _Children[index]; }
        }

        private List<Bone> _Children; // list of children



        /// <summary>
        /// Insert a child bone at specified index.
        /// </summary>        
        /// <param name="index">the zero-based index at witch item should be inserted</param>
        /// <param name="item">bone to add</param>
        public void Insert(int index, Bone item)
        {
            _Children.Insert(index, item);
        }

        /// <summary>
        /// Add a child bone.
        /// </summary>
        /// <param name="item">bone to add</param>
        /// <remarks>we don't control type of bone here since controlled in ViewModel</remarks>
        public void Add(Bone item)
        {
            _Children.Add(item);
        }

        /// <summary>
        /// Remove all children
        /// </summary>
        public void Clear()
        {
            _Children.Clear();
        }

        /// <summary>
        /// Return true if contains given child
        /// </summary>
        /// <param name="item">child to check</param>
        /// <returns>true if contains, otherwise false</returns>
        public bool Contains(Bone item)
        {
            return _Children.Contains(item);
        }

        /// <summary>
        /// Copy children to an array
        /// </summary>
        /// <param name="array">array to fill</param>
        /// <param name="arrayIndex">start index in array to fill</param>
        public void CopyTo(Bone[] array, int arrayIndex)
        {
            _Children.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Count of children
        /// </summary>
        public int Count
        {
            get { return _Children.Count; }
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
        public bool Remove(Bone item)
        {
            return _Children.Remove(item);
        }

        /// <summary>
        /// Enumerator for children
        /// </summary>
        /// <returns>Enumerator for children</returns>
        public IEnumerator<Bone> GetEnumerator()
        {
            return _Children.GetEnumerator();
        }

        /// <summary>
        /// Enumerator for children
        /// </summary>
        /// <returns>Enumerator for children</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Children as System.Collections.IEnumerable).GetEnumerator();
        }
        #endregion
    }
    #endregion

    #region AnimationClip
    /// <summary>
    /// Represent an AnimationClip data
    /// </summary>
    public class AnimationClip : IXElement
    {
        /// <summary> Name of AnimationClip </summary>
        public string Name { get; set; }
        /// <summary> WrapMode of AnimationClip </summary>
        public WrapMode WrapMode { get; set; }
        /// <summary> Length of AnimationClip </summary>
        public float Length { get; set; }

        /// <summary>
        /// Convert AnimationClip data to a XElement
        /// </summary>
        /// <returns>XElement containing data</returns>
        public XElement ToXElement()
        {
            XElement clip = new XElement("AnimationClip");
            clip.SetAttributeValue("Name", Name);
            clip.SetAttributeValue("WrapMode", (int)WrapMode);
            clip.SetAttributeValue("Length", Length);
            return clip;
        }

        /// <summary>
        /// Load AnimationClip data from XElement
        /// </summary>
        /// <param name="e">XElement to load</param>
        public void Load(XElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", Name);
            this.WrapMode = (WrapMode)e.GetAttributeValueAsInt("WrapMode", 0);
            this.Length = e.GetAttributeValueAsFloat("Length", 0);
        }
    }
    #endregion
}
