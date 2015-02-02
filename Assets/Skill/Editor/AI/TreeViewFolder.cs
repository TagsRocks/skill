using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Skill.Editor.AI
{
    public class TreeViewFolder : Skill.Editor.UI.Extended.FolderView, IBehaviorItem, Skill.Editor.UI.Extended.IProperties
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

        public TreeViewFolder(BehaviorData data)
        {
            this.Data = data;
            this.Height = 20;
            LoadChildren();
            RefreshContent();
            base.Foldout.Content.image = Data.GetIcon();
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
            base.Foldout.Content.text = name;
            foreach (IBehaviorItem item in this.Controls)
                item.RefreshContent();
        }

        protected override void BeginRender()
        {
            if (Foldout.Style == null)
            {
                Foldout.Style = new GUIStyle(UnityEditor.EditorStyles.foldout);
                Foldout.Style.fixedWidth = 1000;
            }
            base.BeginRender();
        }


        #region Load Children
        /// <summary>
        /// create view models for all children
        /// </summary>
        private void LoadChildren()
        {
            //iterate throw children and create appropriate view model
            for (int i = 0; i < Data.Count; i++)
            {
                var child = CreateItem(Data[i]);
                base.Controls.Add(child);
            }
        }
        /// <summary>
        /// Create view model based on BehaviorType
        /// </summary>
        /// <param name="behavior">behavior data</param>
        /// <returns>Create view model</returns>
        public static Skill.Framework.UI.BaseControl CreateItem(BehaviorData behavior)
        {
            switch (behavior.BehaviorType)
            {
                case Skill.Framework.AI.BehaviorType.Action:
                    return new ActionItem((ActionData)behavior);
                case Skill.Framework.AI.BehaviorType.Condition:
                    return new ConditionItem((ConditionData)behavior);
                case Skill.Framework.AI.BehaviorType.ChangeState:
                    return new ChangeStateItem((ChangeStateData)behavior);
                case Skill.Framework.AI.BehaviorType.Decorator:
                    return CreateDecoratorItem((DecoratorData)behavior);
                case Skill.Framework.AI.BehaviorType.Composite:
                    return CreateCompositeItem((CompositeData)behavior);

            }
            return null;
        }
        /// <summary>
        /// Create view model based on CompositeType
        /// </summary>
        /// <param name="behavior">selector data</param>
        /// <returns>Create view model</returns>
        static DecoratorItem CreateDecoratorItem(DecoratorData decorator)
        {
            switch (decorator.Type)
            {
                case Skill.Framework.AI.DecoratorType.Default:
                    return new DecoratorItem(decorator);
                case Skill.Framework.AI.DecoratorType.AccessLimit:
                    return new AccessLimitDecoratorItem((AccessLimitDecoratorData)decorator);
                default:
                    throw new System.InvalidCastException("Invalid DecoratorType");
            }
        }
        /// <summary>
        /// Create view model based on CompositeType
        /// </summary>
        /// <param name="behavior">selector data</param>
        /// <returns>Create view model</returns>
        static CompositeItem CreateCompositeItem(CompositeData composite)
        {
            switch (composite.CompositeType)
            {
                case Skill.Framework.AI.CompositeType.Sequence:
                    return new SequenceSelectorItem((SequenceSelectorData)composite);
                case Skill.Framework.AI.CompositeType.Concurrent:
                    return new ConcurrentSelectorItem((ConcurrentSelectorData)composite);
                case Skill.Framework.AI.CompositeType.Random:
                    return new RandomSelectorItem((RandomSelectorData)composite);
                case Skill.Framework.AI.CompositeType.Priority:
                    return new PrioritySelectorItem((PrioritySelectorData)composite);
                case Skill.Framework.AI.CompositeType.Loop:
                    return new LoopSelectorItem((LoopSelectorData)composite);
                default:
                    throw new System.InvalidCastException("Invalid CompositeType");
            }
        }
        private IBehaviorItem Find(List<IBehaviorItem> controls, BehaviorData behavior)
        {
            foreach (var item in controls)
            {
                if (item.Data == behavior)
                    return item;
            }
            return null;
        }
        public void RefreshChildren()
        {
            // check if any changes happened
            bool change = false;
            if (Data.Count != this.Controls.Count)
            {
                change = true;
            }
            else
            {
                for (int i = 0; i < this.Controls.Count; i++)
                {
                    if (((IBehaviorItem)this.Controls[i]).Data != Data[i])
                    {
                        change = true;
                        break;
                    }
                }
            }

            if (change)
            {
                List<IBehaviorItem> controls = new List<IBehaviorItem>();
                for (int i = 0; i < this.Controls.Count; i++)
                    controls.Add(this.Controls[i] as IBehaviorItem);
                this.Controls.Clear();

                for (int i = 0; i < Data.Count; i++)
                {
                    IBehaviorItem item = Find(controls, Data[i]);
                    if (item == null)
                        item = CreateItem(Data[i]) as IBehaviorItem;

                    if (item != null)
                    {
                        controls.Remove(item);
                        this.Controls.Add(item as Skill.Framework.UI.BaseControl);
                    }
                }
            }

            foreach (var item in this.Controls)
            {
                if (item is TreeViewFolder)
                    ((TreeViewFolder)item).RefreshChildren();
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
                if (Parent != null && Parent is TreeViewFolder)
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
                if (Parent != null && Parent is TreeViewFolder)
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
                    if (!IsValidName(value))
                    {
                        UnityEditor.EditorUtility.DisplayDialog("Error", "Invalid behavior name", "Close");
                        return;
                    }

                    //Tree.History.Insert(new ChangePropertyUnDoRedo(this, "Name", value, Model.Name));
                    Data.Name = value;
                    Editor.RefreshContents();
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

        #region Name validation
        /// <summary>
        /// Check whether given name is valid
        /// </summary>
        /// <param name="name">name of variable</param>
        /// <returns>True if valid, otherwise false</returns>
        public static bool IsValidName(string name)
        {
            if (name == null) return false;
            return _Regex.Match(name).Success;
        }

        /// <summary> Regular expression to check names </summary>
        private static System.Text.RegularExpressions.Regex _Regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z_][a-zA-Z0-9_]*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        #endregion

        #region Util



        /// <summary>
        /// Check where is there a child that contains given behavior
        /// </summary>
        /// <param name="behavior">Behavior</param>
        /// <returns>true if contains, otherwise false</returns>
        public bool Contains(BehaviorData behavior)
        {
            foreach (IBehaviorItem item in this.Controls)
                if (item.Data == behavior) return true;
            return false;
        }

        private bool CheckAddCauseLoop(BehaviorData newBehavior)
        {
            TreeViewFolder parent = this;
            if (CheckAddCauseLoop(parent, newBehavior))
                return true;

            foreach (var item in newBehavior)
            {
                if (CheckAddCauseLoop(item))
                    return true;
            }
            return false;
        }
        private bool CheckAddCauseLoop(TreeViewFolder parent, BehaviorData newBehavior)
        {
            while (parent != null)
            {
                if (parent.Data == newBehavior)
                    return true;
                parent = parent.Parent as TreeViewFolder;
            }
            return false;
        }


        public bool CanAddBehavior(BehaviorData newBehavior, out string message)
        {
            // actions and conditions are leaves and can not have any child. also decorators can have only one child
            if (this.Data.BehaviorType != Skill.Framework.AI.BehaviorType.Composite && !(this.Data.BehaviorType == Skill.Framework.AI.BehaviorType.Decorator && Controls.Count == 0))
            {
                message = "Can not add child to this node anymore";
                return false;
            }

            // check to prevent loop in hierarchy. if a node be twise in hierarchy cause too loop in tree
            if (CheckAddCauseLoop(newBehavior))
            {
                message = "Adding this child cause to loop in tree";
                return false;
            }

            message = null;
            return true;
        }

        /// <summary>
        /// Move given child up (index - 1)
        /// </summary>
        /// <param name="child">child to move</param>
        public void MoveUp(IBehaviorItem child)
        {
            if (child.CanMoveUp)
            {
                int index = Controls.IndexOf((Skill.Framework.UI.BaseControl)child);
                if (index > 0 && index < this.Controls.Count)
                {
                    // decrease index one unit                
                    this.Controls.Remove((Skill.Framework.UI.BaseControl)child);
                    this.Controls.Insert(index - 1, (Skill.Framework.UI.BaseControl)child);
                    this.Data.Move(index, index - 1);
                    Editor.RefreshTree();
                    //Tree.History.Insert(new MoveUpBehaviorUnDoRedo(child, this));
                }
            }
        }

        /// <summary>
        /// Move given child down (index + 1)
        /// </summary>
        /// <param name="child">child to move</param>
        public void MoveDown(IBehaviorItem child)
        {
            if (child.CanMoveDown)
            {
                int index = Controls.IndexOf((Skill.Framework.UI.BaseControl)child);
                if (index >= 0)
                {
                    this.Controls.Remove((Skill.Framework.UI.BaseControl)child);
                    this.Controls.Insert(index + 1, (Skill.Framework.UI.BaseControl)child);
                    this.Data.Move(index, index + 1);
                    Editor.RefreshTree();
                    //Tree.History.Insert(new MoveUpBehaviorUnDoRedo(child, this, true));
                }
            }
        }


        /// <summary>
        /// Remove specyfied child
        /// </summary>
        /// <param name="child">child to remove</param>
        /// <returns>true if sucess, otherwise false</returns>
        public IBehaviorItem AddBehavior(BehaviorData newBehavior)
        {
            this.Data.Add(newBehavior);
            var item = CreateItem(newBehavior);
            this.Controls.Add(item);
            Editor.RefreshTree();
            return item as IBehaviorItem;
        }

        /// <summary>
        /// Remove specyfied child
        /// </summary>
        /// <param name="child">child to remove</param>
        /// <returns>true if sucess, otherwise false</returns>
        public bool RemoveBehavior(IBehaviorItem child)
        {
            //int index = this.Controls.IndexOf((Skill.Framework.UI.BaseControl)child);
            if (this.Controls.Remove((Skill.Framework.UI.BaseControl)child))
            {
                this.Data.Remove(child.Data);
                Editor.RefreshTree();
                //Tree.History.Insert(new AddBehaviorUnDoRedo(child, parameters, this, index, true));
                return true;
            }
            return false;
        }
        #endregion

        #region IProperties members
        public virtual bool IsSelectedProperties { get; set; }
        private ItemProperties _Properties;
        public virtual Skill.Editor.UI.Extended.PropertiesPanel Properties
        {
            get
            {
                if (_Properties == null) _Properties = new ItemProperties(this);
                return _Properties;
            }
        }
        public virtual string Title { get { return Data.BehaviorType.ToString(); } }
        protected class ItemProperties : Skill.Editor.UI.Extended.ExposeProperties
        {
            private TreeViewFolder _Item;
            private ParameterEditor _Editor;
            public ItemProperties(TreeViewFolder item)
                : base(item)
            {
                _Item = item;
            }

            protected override void CreateCustomFileds()
            {
                base.CreateCustomFileds();
                TreeViewFolder item = (TreeViewFolder)Object;
                if (item.Data is IParameterData)
                {
                    TreeViewFolder parent = item.Parent as TreeViewFolder;
                    if (parent != null)
                    {
                        var parameters = parent.Data.GetParameters(parent.Controls.IndexOf(item));
                        parameters.Match(((IParameterData)item.Data).ParameterDifinition);
                        _Editor = new ParameterEditor(item, parameters);
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
                _Item.Editor.Repaint();
            }
        }
        #endregion
    }
}
