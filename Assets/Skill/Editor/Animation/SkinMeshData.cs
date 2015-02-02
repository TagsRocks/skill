using System;
using System.Collections.Generic;
using System.Text;
using Skill.Framework.IO;

namespace Skill.Editor.Animation
{
    /// <summary>
    /// Represent a skinmesh witch includes a hierarchy of bones
    /// </summary>
    public class SkinMeshData : IXmlElementSerializable
    {
        #region Properties
        /// <summary> Root of skinmesh </summary>
        public BoneData Root { get; set; }
        /// <summary> Name of skinmesh. this name is based on filename and will set after loading from file </summary>
        public string Name { get; set; }
        /// <summary> Animations of skinmesh </summary>
        public AnimationClipData[] Animations { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Create an instance of SkinMesh
        /// </summary>
        public SkinMeshData()
        {
            this.Name = "NewSkinMesh";
            this.Root = new BoneData();
            this.Root.Name = "Root";
            this.Root.RootPath = "Root";
        }
        #endregion

        #region Methods
        /// <summary>
        /// Check whether specyfied bone is in hierarchy or unused
        /// </summary>
        /// <param name="bone">Bone to check</param>
        /// <returns>True if is in hierarchy, otherwise false</returns>
        public bool IsInHierarchy(BoneData bone)
        {
            return IsInHierarchy(Root, bone);
        }

        private bool IsInHierarchy(BoneData parentBone, BoneData bone)
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
        /// Create a XmlElement containing SkinMesh data
        /// </summary>
        /// <returns>XmlElement</returns>
        public XmlElement ToXmlElement()
        {
            XmlElement skinmesh = new XmlElement("SkinMesh");
            skinmesh.SetAttributeValue("Name", Name);

            XmlElement bones = new XmlElement("Bones");
            bones.AppendChild(Root.ToXmlElement());
            skinmesh.AppendChild(bones);

            XmlElement animations = new XmlElement("Animations");
            if (Animations != null)
            {
                animations.SetAttributeValue("Count", Animations.Length);
                foreach (var clip in Animations)
                {
                    animations.AppendChild(clip.ToXmlElement());
                }
            }
            else
                animations.SetAttributeValue("Count", 0);
            skinmesh.AppendChild(animations);
            return skinmesh;
        }


        #endregion

        #region Load

        /// <summary>
        /// Load SkinMesh data from XmlElement
        /// </summary>
        /// <param name="e">XmlElement to load from</param>
        public void Load(XmlElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", this.Name);
            XmlElement bones = e["Bones"];
            if (bones != null)
            {
                XmlElement root = bones["Bone"];
                if (root != null)
                {
                    this.Root = new BoneData();
                    this.Root.Load(root);
                }
            }

            XmlElement animations = e["Animations"];
            if (animations != null)
            {
                int count = animations.GetAttributeValueAsInt("Count", 0);
                Animations = new AnimationClipData[count];
                int i = 0;

                foreach (var element in animations)
                {
                    AnimationClipData clip = new AnimationClipData();
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
    public class BoneData : IXmlElementSerializable, ICollection<BoneData>
    {
        #region Properties

        /// <summary> Name of bone </summary>
        public string Name { get; set; }

        /// <summary> Path of bone </summary>
        public string RootPath { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creatre an instance of bone
        /// </summary>        
        public BoneData()
        {
            _Children = new List<BoneData>();
        }
        #endregion

        #region Save

        /// <summary>
        /// Create a XmlElement that contains all data required to save in file
        /// </summary>
        /// <returns></returns>
        public XmlElement ToXmlElement()
        {
            XmlElement bone = new XmlElement("Bone");
            bone.SetAttributeValue("Name", Name);
            bone.SetAttributeValue("RootPath", this.RootPath);
            foreach (var b in this)
                bone.AppendChild(b.ToXmlElement());

            return bone;
        }
        #endregion

        #region Load

        protected static XmlElement FindChild(XmlElement e, string name)
        {
            foreach (var item in e)
            {
                if (e.Name == name)
                {
                    return item;
                }
            }
            return null;
        }


        /// <summary>
        /// Load bone data
        /// </summary>
        /// <param name="e">contains bone data</param>
        public void Load(XmlElement e)
        {
            this.Name = e.GetAttributeValueAsString("Name", this.Name);
            this.RootPath = e.GetAttributeValueAsString("RootPath", this.Name);
            Clear();
            foreach (var item in e)
            {
                BoneData childBone = new BoneData();
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
        public BoneData this[int index]
        {
            get { return _Children[index]; }
        }

        private List<BoneData> _Children; // list of children



        /// <summary>
        /// Insert a child bone at specified index.
        /// </summary>        
        /// <param name="index">the zero-based index at witch item should be inserted</param>
        /// <param name="item">bone to add</param>
        public void Insert(int index, BoneData item)
        {
            _Children.Insert(index, item);
        }

        /// <summary>
        /// Add a child bone.
        /// </summary>
        /// <param name="item">bone to add</param>
        /// <remarks>we don't control type of bone here since controlled in ViewModel</remarks>
        public void Add(BoneData item)
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
        public bool Contains(BoneData item)
        {
            return _Children.Contains(item);
        }

        /// <summary>
        /// Copy children to an array
        /// </summary>
        /// <param name="array">array to fill</param>
        /// <param name="arrayIndex">start index in array to fill</param>
        public void CopyTo(BoneData[] array, int arrayIndex)
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
        public bool Remove(BoneData item)
        {
            return _Children.Remove(item);
        }

        /// <summary>
        /// Enumerator for children
        /// </summary>
        /// <returns>Enumerator for children</returns>
        public IEnumerator<BoneData> GetEnumerator()
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
}
