using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Skill.Editor.Animation
{

    public abstract class AnimNodeChildrenEditable : AnimNode
    {
        public AnimNodeChildrenEditable(string name)
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


        public override void CopyFrom(AnimNode other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimNodeChildrenEditable s = other as AnimNodeChildrenEditable;
            this.ChildrenNames = s.ChildrenNames.Clone() as string[];
        }
    }

    public class AnimNodeChildrenEditableProperties : AnimNodeProperties, IAnimNodeConnectorContainer
    {
        public AnimNodeChildrenEditableProperties(AnimNodeChildrenEditableViewModel viewmodel)
            : base(viewmodel)
        {

        }

        [Description("Children")]
        public int ChildCount
        {
            get { return ((AnimNodeChildrenEditableViewModel)ViewModel).ChildCount; }
            set
            {
                if (((AnimNodeChildrenEditableViewModel)ViewModel).ChildCount != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "ChildCount", value, ((AnimNodeChildrenEditableViewModel)ViewModel).ChildCount));
                    }
                    ((AnimNodeChildrenEditableViewModel)ViewModel).ChildCount = value;
                }
            }
        }

        [Description("Name of each input in diagram")]
        [DisplayName("Children")]
        [System.ComponentModel.Editor(typeof(AnimNodeConnectorsEditor), typeof(AnimNodeConnectorsEditor))]
        public ObservableCollection<AnimNodeConnectorProperties> Connectors
        {
            get { return ((AnimNodeChildrenEditableViewModel)ViewModel).AnimNodeConnectors; }
        }
    }


    /// <summary>
    /// Interaction logic for AnimNodeBlendByIndex.xaml
    /// </summary>
    public class AnimNodeChildrenEditableViewModel : AnimNodeViewModel
    {
        private ObservableCollection<AnimNodeConnectorProperties> _AnimNodeConnectors;
        public ObservableCollection<AnimNodeConnectorProperties> AnimNodeConnectors { get { return _AnimNodeConnectors; } }

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
                    if (_AnimNodeConnectors.Count < _ChildCount)
                    {
                        while (_AnimNodeConnectors.Count < _ChildCount)
                        {
                            int index = _AnimNodeConnectors.Count + 1; // before add item will be index of next added item
                            AnimNodeConnector ac = new AnimNodeConnector() { Text = "Input" + index.ToString() };
                            ac.Connector.Index = _AnimNodeConnectors.Count;
                            _AnimNodeConnectors.Add(ac.Properties);
                            ConnectorPanel.Children.Add(ac);
                        }
                    }
                    else if (_AnimNodeConnectors.Count > _ChildCount)
                    {
                        while (_AnimNodeConnectors.Count > _ChildCount)
                        {
                            int index = _AnimNodeConnectors.Count - 1;
                            AnimNodeConnector ac = _AnimNodeConnectors[index].Connector;
                            ac.Destroy();
                            _AnimNodeConnectors.Remove(ac.Properties);
                            ConnectorPanel.Children.Remove(ac);
                        }
                    }
                }

            }
        }

        public AnimNodeChildrenEditableViewModel()
        {
        }

        public AnimNodeChildrenEditableViewModel(AnimNodeChildrenEditable model)
            : base(model)
        {
            _AnimNodeConnectors = new ObservableCollection<AnimNodeConnectorProperties>();
        }

        protected virtual void InitializeChildNames()
        {
            string[] names = ((AnimNodeChildrenEditable)Model).ChildrenNames;
            if (names != null)
            {
                ChildCount = names.Length;
                for (int i = 0; i < names.Length; i++)
                {
                    _AnimNodeConnectors[i].Text = names[i];
                }
            }
        }

        public override void CommiteChangesToModel()
        {
            base.CommiteChangesToModel();
            string[] names = new string[ChildCount];
            for (int i = 0; i < ChildCount; i++)
            {
                names[i] = _AnimNodeConnectors[i].Text;
            }
            ((AnimNodeChildrenEditable)Model).ChildrenNames = names;
        }
    }
}
