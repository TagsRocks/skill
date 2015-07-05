using UnityEngine;
using System.Collections;

namespace Skill.Editor.AI
{
    public class TreeViewItem : Skill.Framework.UI.Label, IBehaviorItem, Skill.Editor.UI.IProperties
    {
        public BehaviorTreeEditorWindow Editor
        {
            get
            {
                Skill.Framework.UI.Frame owner = OwnerFrame;
                if (owner != null)
                    return (BehaviorTreeEditorWindow)((Skill.Editor.UI.EditorFrame)owner).Owner;
                return null;
            }
        }

        public BehaviorData Data { get; private set; }

        public TreeViewItem(BehaviorData data)
        {
            this.Data = data;
            this.Height = 20;
            RefreshContent();
            base.Content.image = Data.GetIcon();
        }

        public virtual void RefreshContent()
        {
            string name = Data.Name;
            if (Data is IParameterData)
            {
                if (Parent != null)
                {
                    TreeViewFolder folder = Parent as TreeViewFolder;
                    if (folder != null)
                    {
                        var parameters = folder.Data.GetParameters(folder.Controls.IndexOf(this));
                        if (parameters != null)
                        {
                            parameters.Match(((IParameterData)Data).ParameterDifinition);
                            if (parameters.Count > 0)
                                name += parameters.ToString();
                        }
                    }
                }
            }
            base.Content.text = name;
        }

        #region Expose Properties
        [Skill.Framework.ExposeProperty(0, "Name", "name of behavior")]
        public virtual string Name2
        {
            get
            {
                return Data.Name;
            }
            set
            {
                if (value != Data.Name && !string.IsNullOrEmpty(value))
                {
                    if (!TreeViewFolder.IsValidName(value))
                    {
                        UnityEditor.EditorUtility.DisplayDialog("Error", "Invalid behavior name", "Close");
                        return;
                    }
                    //Tree.History.Insert(new ChangePropertyUnDoRedo(this, "Name", value, Model.Name));
                    Data.Name = value;
                    RefreshContent();
                    Editor.BehaviorNameChanged(this.Data);
                    Editor.RefreshSameContent(this);
                }
            }
        }

        [Skill.Framework.ExposeProperty(100, "Comment", "user comment for behavior")]
        public string Comment
        {
            get { return Data.Comment; }
            set
            {
                if (value != Data.Comment)
                {
                    //Tree.History.Insert(new ChangePropertyUnDoRedo(this, "Comment", value, Model.Comment));
                    Data.Comment = value;
                }
            }
        }

        [Skill.Framework.ExposeProperty(91, "Weight", "Weight of node when behavior is child of a random selecto")]
        public float Weight
        {
            get { return Data.Weight; }
            set
            {
                if (value != Data.Weight)
                {
                    if (value < 0.1f) { value = 0.1f; }
                    //Tree.History.Insert(new ChangePropertyUnDoRedo(this, "Weight", value, Model.Weight));
                    Data.Weight = value;
                }
            }
        }

        [Skill.Framework.ExposeProperty(92, "Concurrency", "execution time of behavior when behavior is child of a concurent selector")]
        public Skill.Framework.AI.ConcurrencyMode Concurrency
        {
            get { return Data.Concurrency; }
            set
            {
                if (value != Data.Concurrency)
                {
                    //Tree.History.Insert(new ChangePropertyUnDoRedo(this, "Concurrency", value, Model.Concurrency));
                    Data.Concurrency = value;
                }
            }
        }
        #endregion

        #region IBehaviorItem members

        /// <summary>
        /// Check whether this node can move up inside parent children
        /// </summary>    
        public bool CanMoveUp
        {
            get
            {
                if (Parent != null)
                {
                    TreeViewFolder folder = (TreeViewFolder)Parent;
                    if (folder.Controls.Count > 0 && folder.Controls[0] != this)
                        return true;
                    return false;

                }
                return false;
            }
        }

        /// <summary>
        /// Check whether this node can move down inside parent children
        /// </summary>    
        public bool CanMoveDown
        {
            get
            {
                if (Parent != null)
                {
                    TreeViewFolder folder = (TreeViewFolder)Parent;
                    if (folder.Controls.Count > 0 && folder.Controls[folder.Controls.Count - 1] != this)
                        return true;
                    return false;

                }
                return false;
            }
        }
        #endregion

        #region IProperties members
        public bool IsSelectedProperties { get; set; }
        private ItemProperties _Properties;
        public Skill.Editor.UI.PropertiesPanel Properties
        {
            get
            {
                if (_Properties == null) _Properties = new ItemProperties(this);
                return _Properties;
            }
        }
        public string Title { get { return Data.BehaviorType.ToString(); } }
        protected class ItemProperties : Skill.Editor.UI.ExposeProperties
        {
            private TreeViewItem _Item;
            private ParameterEditor _Editor;

            public ItemProperties(TreeViewItem item)
                : base(item)
            {
                _Item = item;
            }

            protected override void CreateCustomFileds()
            {
                base.CreateCustomFileds();
                TreeViewItem item = (TreeViewItem)Object;
                if (item.Data is IParameterData)
                {
                    TreeViewFolder parent = item.Parent as TreeViewFolder;
                    if (parent != null)
                    {
                        var parameters = parent.Data.GetParameters(parent.Controls.IndexOf(item));
                        var difinition = ((IParameterData)item.Data).ParameterDifinition;
                        parameters.Match(difinition);
                        _Editor = new ParameterEditor(item, difinition, parameters);                        
                        this.Controls.Add(_Editor);
                    }

                }
            }

            protected override void RefreshData()
            {
                base.RefreshData();
                if (_Editor != null)
                    _Editor.Rebuild();
            }

            protected override void SetDirty()
            {
                Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(_Item);
            }
        }
        #endregion
    }
}
