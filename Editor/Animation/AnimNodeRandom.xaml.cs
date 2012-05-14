using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.ComponentModel;

namespace Skill.Studio.Animation
{

    public class AnimNodeRandom : AnimNode
    {
        public AnimNodeRandom()
            : base("AnimNodeRandom")
        {
            Chances = new Chances();
        }

        public override AnimNodeType NodeType { get { return AnimNodeType.Random; } }
        public Chances Chances { get; private set; }

        private string[] _ChildrenNames;
        public string[] ChildrenNames
        {
            get
            {
                if (this._ChildrenNames == null)
                    this._ChildrenNames = new string[0];
                return this._ChildrenNames;
            }
            set
            {
                this._ChildrenNames = value;
            }
        }

        protected override void WriteAttributes(XElement e)
        {

            XElement data = new XElement("RandomData");
            
            XElement children = new XElement("Children");
            children.SetAttributeValue("Count", ChildrenNames.Length);
            foreach (var item in ChildrenNames)
            {
                XElement name = new XElement("Name");
                name.Value = item;
                children.Add(name);
            }
            data.Add(children);


            XElement chances = new XElement("Chances");
            chances.SetAttributeValue("Count", Chances.Count);
            foreach (var item in Chances)
            {
                XElement f = new XElement("Float");
                f.Value = item.Property.ToString();
                chances.Add(f);
            }
            data.Add(chances);
            e.Add(data);

            base.WriteAttributes(e);
        }

        protected override void ReadAttributes(XElement e)
        {
            XElement data = e.FindChildByName("RandomData");
            if (data != null)
            {                
                XElement children = data.FindChildByName("Children");
                if (children != null)
                {
                    int count = children.GetAttributeValueAsInt("Count", 0);
                    ChildrenNames = new string[count];
                    int i = 0;
                    foreach (var element in children.Elements().Where(p => p.Name == "Name"))
                    {
                        ChildrenNames[i] = element.Value;
                        i++;
                    }
                }

                XElement constraints = data.FindChildByName("Chances");
                if (constraints != null)
                {
                    int count = constraints.GetAttributeValueAsInt("Count", 0);
                    Chances.SetCount(count);
                    int i = 0;
                    foreach (var element in constraints.Elements().Where(p => p.Name == "Float"))
                    {
                        Chances[i].Property = float.Parse(element.Value);
                        i++;
                    }
                }
            }
            base.ReadAttributes(e);
        }


        public override void CopyFrom(AnimNode other)
        {
            if (other.NodeType != this.NodeType) return;
            base.CopyFrom(other);
            AnimNodeRandom s = other as AnimNodeRandom;            
            this.ChildrenNames = s.ChildrenNames.Clone() as string[];
            this.Chances.SetCount(s.Chances.Count);
            for (int i = 0; i < s.Chances.Count; i++)
            {
                this.Chances[i].Property = s.Chances[i].Property;
            }
        }

        public override AnimNode Clone()
        {
            AnimNodeRandom node = new AnimNodeRandom();
            node.CopyFrom(this);
            return node;
        }
    }

    public class AnimNodeRandomProperties : AnimNodeProperties, IFloatCollectionContainer, IAnimNodeConnectorContainer
    {
        public AnimNodeRandomProperties(AnimNodeRandomViewModel viewmodel)
            : base(viewmodel)
        {

        }        

        [Description("Children")]
        public int ChildCount
        {
            get { return ((AnimNodeRandomViewModel)ViewModel).ChildCount; }
            set
            {
                if (((AnimNodeRandomViewModel)ViewModel).ChildCount != value)
                {
                    if (Editor != null)
                    {
                        Editor.History.Insert(new ChangePropertyUnDoRedo(this, "ChildCount", value, ((AnimNodeRandomViewModel)ViewModel).ChildCount));
                    }
                    ((AnimNodeRandomViewModel)ViewModel).ChildCount = value;
                }
            }
        }

        [Description("Chance of each child")]
        [System.ComponentModel.Editor(typeof(FloatCollectionEditor), typeof(FloatCollectionEditor))]
        public Chances Chances
        {
            get { return ((AnimNodeRandom)ViewModel.Model).Chances; }
        }

        [Description("Name of each input in diagram")]
        [DisplayName("Children")]
        [System.ComponentModel.Editor(typeof(AnimNodeConnectorsEditor), typeof(AnimNodeConnectorsEditor))]
        public ObservableCollection<AnimNodeConnectorProperties> Connectors
        {
            get { return ((AnimNodeRandomViewModel)ViewModel).AnimNodeConnectors; }
        }

        [Browsable(false)]
        public ObservableCollection<FloatContainer> FloatCollection
        {
            get { return Chances; }
        }
    }

    public class Chances : ObservableCollection<FloatContainer>
    {
        public Chances()
        {
            SetCount(2);
        }

        public void SetCount(int count)
        {
            if (count < 2) count = 2;
            if (Count < count)
            {
                while (Count < count)
                {
                    Add(new FloatContainer(1.0f));
                }
            }
            else if (Count > count)
            {
                while (Count > count)
                {
                    int index = Count - 1;
                    RemoveAt(index);
                }
            }
        }
    }

    /// <summary>
    /// Interaction logic for AnimNodeRandom.xaml
    /// </summary>
    public partial class AnimNodeRandomViewModel : AnimNodeViewModel
    {
        private ObservableCollection<AnimNodeConnectorProperties> _AnimNodeConnectors;
        public ObservableCollection<AnimNodeConnectorProperties> AnimNodeConnectors { get { return _AnimNodeConnectors; } }

        public override Skill.Studio.Diagram.Connector Out { get { return _Out; } }

        private int _ChildCount;
        public int ChildCount
        {
            get { return _ChildCount; }
            set
            {
                if (value < 2) value = 2;
                if (_ChildCount != value)
                {
                    _ChildCount = value;
                    ((AnimNodeRandom)Model).Chances.SetCount(_ChildCount);

                    if (_AnimNodeConnectors.Count < _ChildCount)
                    {
                        while (_AnimNodeConnectors.Count < _ChildCount)
                        {
                            int index = _AnimNodeConnectors.Count + 1; // before add item will be index of next added item
                            AnimNodeConnector ac = new AnimNodeConnector() { Text = "Input" + index.ToString() };
                            ac.Connector.Index = _AnimNodeConnectors.Count;
                            _AnimNodeConnectors.Add(ac.Properties);
                            _ConnectorPanel.Children.Add(ac);
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
                            _ConnectorPanel.Children.Remove(ac);
                        }
                    }
                }

            }
        }

        public AnimNodeRandomViewModel()
            : this(new AnimNodeRandom())
        {
        }

        public AnimNodeRandomViewModel(AnimNodeRandom model)
            : base(model)
        {
            InitializeComponent();
            _AnimNodeConnectors = new ObservableCollection<AnimNodeConnectorProperties>();
            ChildCount = ((AnimNodeRandom)Model).Chances.Count;


            string[] names = ((AnimNodeRandom)Model).ChildrenNames;
            if (names != null)
            {
                for (int i = 0; i < names.Length; i++)
                {
                    _AnimNodeConnectors[i].Text = names[i];
                }
            }
            this.Properties = new AnimNodeRandomProperties(this);
        }

        public override void CommiteChangesToModel()
        {
            base.CommiteChangesToModel();
            string[] names = new string[_ChildCount];
            for (int i = 0; i < ChildCount; i++)
            {
                names[i] = _AnimNodeConnectors[i].Text;
            }
            ((AnimNodeRandom)Model).ChildrenNames = names;
        }        
    }
}
