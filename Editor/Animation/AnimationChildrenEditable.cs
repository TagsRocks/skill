using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Skill.Editor.Animation
{

    public abstract class AnimationChildrenEditable : AnimationNode
    {
        public AnimationChildrenEditable(string name)
            : base(name)
        {
        }

        private string[] _ChildrenNames;
        public string[] ChildrenNames
        {
            get
            {
                if (this._ChildrenNames == null)
                    this._ChildrenNames = new string[] { "Input1", "Input2" };
                return this._ChildrenNames;
            }
            set
            {
                this._ChildrenNames = value;
            }
        }

        protected override void WriteAttributes(XElement e)
        {

            XElement children = new XElement("Children");
            children.SetAttributeValue("Count", ChildrenNames.Length);
            foreach (var item in ChildrenNames)
            {
                XElement name = new XElement("ChildName");
                name.Value = item;
                children.Add(name);
            }
            e.Add(children);

            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XElement e)
        {
            XElement children = e.FindChildByName("Children");
            if (children != null)
            {
                int count = children.GetAttributeValueAsInt("Count", 0);
                ChildrenNames = new string[count];
                int i = 0;
                foreach (var element in children.Elements().Where(p => p.Name == "ChildName"))
                {
                    ChildrenNames[i] = element.Value;
                    i++;
                }
            }
            base.ReadAttributes(e);
        }


        public override void CopyFrom(AnimationNode other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimationChildrenEditable s = other as AnimationChildrenEditable;
            this.ChildrenNames = s.ChildrenNames.Clone() as string[];
        }
    }

    public class AnimationChildrenEditableProperties : AnimationNodeProperties, IAnimConnectorContainer
    {
        public AnimationChildrenEditableProperties(AnimationChildrenEditableViewModel viewmodel)
            : base(viewmodel)
        {

        }

        [Description("Children")]
        public int ChildCount
        {
            get { return ((AnimationChildrenEditableViewModel)ViewModel).ChildCount; }
            set
            {
                if (((AnimationChildrenEditableViewModel)ViewModel).ChildCount != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "ChildCount", value, ((AnimationChildrenEditableViewModel)ViewModel).ChildCount));
                    }
                    ((AnimationChildrenEditableViewModel)ViewModel).ChildCount = value;
                }
            }
        }

        [Description("Name of each input in diagram")]
        [DisplayName("Children")]
        [System.ComponentModel.Editor(typeof(AnimConnectorsEditor), typeof(AnimConnectorsEditor))]
        public ObservableCollection<AnimConnectorProperties> Connectors
        {
            get { return ((AnimationChildrenEditableViewModel)ViewModel).AnimConnectors; }
        }
    }


    /// <summary>
    /// Interaction logic for AnimationSwitchByIndex.xaml
    /// </summary>
    public class AnimationChildrenEditableViewModel : AnimationNodeViewModel
    {
        private ObservableCollection<AnimConnectorProperties> _AnimConnectors;
        public ObservableCollection<AnimConnectorProperties> AnimConnectors { get { return _AnimConnectors; } }

        protected virtual System.Windows.Controls.StackPanel ConnectorPanel { get { return null; } }

        private int _ChildCount;
        public virtual int ChildCount
        {
            get { return _ChildCount; }
            set
            {
                if (value < 2) value = 2;
                if (_ChildCount != value)
                {
                    _ChildCount = value;
                    if (_AnimConnectors.Count < _ChildCount)
                    {
                        while (_AnimConnectors.Count < _ChildCount)
                        {
                            int index = _AnimConnectors.Count + 1; // before add item will be index of next added item
                            AnimConnector ac = new AnimConnector() { Text = "Input" + index.ToString() };
                            ac.Connector.Index = _AnimConnectors.Count;
                            _AnimConnectors.Add(ac.Properties);
                            ConnectorPanel.Children.Add(ac);
                        }
                    }
                    else if (_AnimConnectors.Count > _ChildCount)
                    {
                        while (_AnimConnectors.Count > _ChildCount)
                        {
                            int index = _AnimConnectors.Count - 1;
                            AnimConnector ac = _AnimConnectors[index].Connector;
                            ac.Destroy();
                            _AnimConnectors.Remove(ac.Properties);
                            ConnectorPanel.Children.Remove(ac);
                        }
                    }
                }

            }
        }

        public AnimationChildrenEditableViewModel()
        {
        }

        public AnimationChildrenEditableViewModel(AnimationChildrenEditable model)
            : base(model)
        {
            _AnimConnectors = new ObservableCollection<AnimConnectorProperties>();
        }

        protected virtual void InitializeChildNames()
        {
            string[] names = ((AnimationChildrenEditable)Model).ChildrenNames;
            if (names != null)
            {
                ChildCount = names.Length;
                for (int i = 0; i < names.Length; i++)
                {
                    _AnimConnectors[i].Text = names[i];
                }
            }
        }

        public override void CommiteChangesToModel()
        {
            base.CommiteChangesToModel();
            string[] names = new string[ChildCount];
            for (int i = 0; i < ChildCount; i++)
            {
                names[i] = _AnimConnectors[i].Text;
            }
            ((AnimationChildrenEditable)Model).ChildrenNames = names;
        }
    }
}
