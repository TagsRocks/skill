using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;
using System.Windows.Controls;
using Skill.DataModels.Animation;
using System.Windows.Media;
using Skill.Studio.Animation.Editor;
using Skill.Studio.Diagram;
using System.Windows;
using System.Collections.ObjectModel;

namespace Skill.Studio.Animation
{

    public abstract class AnimNodeViewModel : INotifyPropertyChanged, ISelectable
    {

        #region Properties
        /// <summary> Whether out connector is visible or not (for Root node this value is false)</summary>
        [Browsable(false)]
        public virtual Visibility OutVisible { get { return Visibility.Visible; } }

        /// <summary> Whether input connectors are visible or not (for Sequence node this value is false)</summary>
        [Browsable(false)]
        public virtual Visibility InputsVisible { get { return Visibility.Visible; } }        

        /// <summary> Model </summary>
        [Browsable(false)]
        public AnimNode Model { get; private set; }


        /// <summary> Type of AnimNode </summary>
        [Browsable(false)]
        public AnimNodeType NodeType { get { return Model.NodeType; } }

        /// <summary> Id of Animation Node </summary>
        [Browsable(false)]
        public int Id { get { return Model.Id; } set { Model.Id = value; } }

        /// <summary> Canvas.Left </summary>        
        [Browsable(false)]
        public double X { get { return Model.X; } set { if (Model.X != value) { Model.X = value; OnPropertyChanged("X"); Tree.Editor.SetChanged(true); } } }

        /// <summary> Canvas.Top </summary>        
        [Browsable(false)]
        public double Y { get { return Model.Y; } set { if (Model.Y != value) { Model.Y = value; OnPropertyChanged("Y"); Tree.Editor.SetChanged(true); } } }

        [Browsable(false)]
        public AnimationTreeViewModel Tree { get; private set; }

        private bool _IsSelected;
        [Browsable(false)]
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    if (value)
                        BorderBrush = SelectedBrush;
                    else
                        BorderBrush = ContentBrush;
                    OnSelectedChanged();
                    OnPropertyChanged("IsSelected");                    
                }
            }
        }

        public event EventHandler SelectedChanged;
        protected virtual void OnSelectedChanged()
        {
            if (SelectedChanged != null)
                SelectedChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Brush Properties

        private Brush _BorderBrush;
        [Browsable(false)]
        public Brush BorderBrush
        {
            get
            {
                if (_BorderBrush == null)
                    _BorderBrush = ContentBrush;
                return _BorderBrush;
            }
            set
            {
                if (_BorderBrush != value)
                {
                    _BorderBrush = value;
                    OnPropertyChanged("BorderBrush");
                }
            }
        }

        private Brush _SelectedBrush;
        [Browsable(false)]
        public virtual Brush SelectedBrush
        {
            get
            {
                if (_SelectedBrush == null)
                    _SelectedBrush = StaticBrushes.AnimSelectedBrush;
                return _SelectedBrush;
            }
            set
            {
                if (_SelectedBrush != value)
                {
                    _SelectedBrush = value;
                    OnPropertyChanged("SelectedBrush");
                }
            }
        }

        [Browsable(false)]
        public virtual Brush ContentBrush { get { return StaticBrushes.AnimOtherContnetBrush; } }

        [Browsable(false)]
        public abstract string ImageName { get; }

        #endregion

        #region Browsable Properties

        [DefaultValue(true)]
        [Description("Code generator generates a public property for this node.")]
        public virtual bool IsPublic
        {
            get { return Model.IsPublic; }
            set
            {
                if (Model.IsPublic != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "IsPublic", value, Model.IsPublic));
                    }
                    Model.IsPublic = value;
                    OnPropertyChanged("IsPublic");
                }

            }
        }

        [Description("Name of animation node and variable in code generation")]
        public virtual string Name
        {
            get { return Model.Name; }
            set
            {
                if (Model.Name != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "Name", value, Model.Name));
                    }
                    Model.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        [DefaultValue(0.3f)]
        [Description("Blend Time of animation node.")]
        public virtual float BlendTime
        {
            get
            {
                return Model.BlendTime;
            }
            set
            {
                if (Model.BlendTime != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "BlendTime", value, Model.BlendTime));
                    }
                    Model.BlendTime = value;
                    OnPropertyChanged("BlendTime");
                }
            }
        }

        [DefaultValue(false)]
        [Description("If true code generator create an method and hook it to BecameRelevant event")]
        [Category("Events")]
        public virtual bool BecameRelevant
        {
            get { return Model.BecameRelevant; }
            set
            {
                if (Model.BecameRelevant != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "BecameRelevant", value, Model.BecameRelevant));
                    }
                    Model.BecameRelevant = value;
                    OnPropertyChanged("BecameRelevant");
                }
            }
        }

        [DefaultValue(false)]
        [Description("If true code generator create an method and hook it to CeaseRelevant event")]
        [Category("Events")]
        public virtual bool CeaseRelevant
        {
            get { return Model.CeaseRelevant; }
            set
            {
                if (Model.CeaseRelevant != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "CeaseRelevant", value, Model.CeaseRelevant));
                    }
                    Model.CeaseRelevant = value;
                    OnPropertyChanged("CeaseRelevant");
                }
            }
        }

        [DefaultValue("")]
        [Description("User comment for this Animation Node")]
        public virtual string Comment
        {
            get { return Model.Comment; }
            set
            {
                if (Model.Comment != value)
                {
                    if (Tree.Editor.History != null)
                    {
                        Tree.Editor.History.Insert(new ChangePropertyUnDoRedo(this, "Comment", value, Model.Comment));
                    }
                    Model.Comment = value;
                    OnPropertyChanged("Comment");
                }
            }
        }


        #endregion

        #region Connectors
        [Browsable(false)]
        public ObservableCollection<ConnectorViewModel> Inputs { get; private set; }

        public void SetInputCount(int count)
        {
            if (count > Inputs.Count)
            {
                int index = Inputs.Count;
                for (int i = Inputs.Count; i < count; i++)
                {
                    Connector input = new Connector() { Index = index, Name = "Input" + index.ToString() };
                    this.Inputs.Add(new ConnectorViewModel(this, input));
                    index++;
                }
            }
            else if (count < Inputs.Count)
            {
                for (int i = Inputs.Count - 1; i >= count; i--)
                {
                    Inputs.RemoveAt(i);
                }
            }
        }

        public Editor.AnimConnector GetConnector(int index)
        {
            foreach (var c in Inputs)
            {
                if (c.Index == index)
                    return c.Connector;
            }
            return null;
        }

        [Browsable(false)]
        public ConnectorViewModel OutConnector { get; private set; }

        #endregion

        #region Constructor
        public AnimNodeViewModel(AnimationTreeViewModel tree, AnimNode model)
        {
            this.OutConnector = new ConnectorViewModel(this, new Connector() { Index = -1, Name = "Out", Type = ConnectorType.Output });
            this.Inputs = new ObservableCollection<ConnectorViewModel>();
            this.Tree = tree;
            this.Model = model;
            if (this.Model.Inputs != null)
            {
                foreach (var input in this.Model.Inputs)
                {
                    this.Inputs.Add(new ConnectorViewModel(this, input));
                }
            }
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            if (Model != null)
                return Model.Name;
            else
                return base.ToString();
        }
        #endregion

        #region CommiteChanges
        public virtual void CommiteChangesToModel()
        {
            Model.Inputs = new Connector[this.Inputs.Count];
            for (int i = 0; i < this.Inputs.Count; i++)
            {
                Model.Inputs[i] = this.Inputs[i].Model;
                Model.Inputs[i].Index = i;
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        // we could use DependencyProperties as well to inform others of property changes
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        #region Refresh
        /// <summary>
        /// Cause to refresh connections
        /// </summary>
        internal void RefreshConnections()
        {
            OnPropertyChanged("X");
        } 
        #endregion
    }

}
