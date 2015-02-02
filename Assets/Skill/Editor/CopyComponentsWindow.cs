using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Skill.Editor
{
    public class CopyPaseComponentsWindow : EditorWindow
    {
        #region EditorWindow
        private static CopyPaseComponentsWindow _Instance;
        public static CopyPaseComponentsWindow Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = ScriptableObject.CreateInstance<CopyPaseComponentsWindow>();
                return _Instance;
            }
        }

        private static Vector2 Size = new Vector2(500, 400);

        public CopyPaseComponentsWindow()
        {
            hideFlags = HideFlags.DontSave;

            base.title = "Copy/Paste";
            base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = new Vector2(Size.x * 0.5f, Size.y * 0.5f);

            CreateUI();
        }

        void OnFocus()
        {
            if (_Frame != null)
            {
                _RefreshStyles = true;
            }
        }
        #endregion

        #region UI
        private bool _RefreshStyles = true;
        private Skill.Editor.UI.EditorFrame _Frame;
        private Skill.Framework.UI.Label _SourceLabel;
        private Skill.Framework.UI.Label _DestinationLabel;
        private Skill.Editor.UI.ObjectField<GameObject> _Source;
        private Skill.Editor.UI.ObjectField<GameObject> _Destination;
        private Skill.Framework.UI.StackPanel _SourceComponents;
        private Skill.Framework.UI.StackPanel _DestinationComponents;
        private Skill.Framework.UI.Button _BtnCopy;
        private Skill.Framework.UI.Label _Splitter;
        private Skill.Framework.UI.Label _ComponentsBackground;

        private void CreateUI()
        {
            _Frame = new Skill.Editor.UI.EditorFrame("Frame", this);

            _Frame.Grid.ColumnDefinitions.Add(58, Skill.Framework.UI.GridUnitType.Pixel); // SourceLabel
            _Frame.Grid.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); // Source        
            _Frame.Grid.ColumnDefinitions.Add(2, Skill.Framework.UI.GridUnitType.Pixel); // Splitter            
            _Frame.Grid.ColumnDefinitions.Add(80, Skill.Framework.UI.GridUnitType.Pixel); // DestinationLabel
            _Frame.Grid.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); // Destination

            _Frame.Grid.RowDefinitions.Add(16, Skill.Framework.UI.GridUnitType.Pixel); // ObjectFields
            _Frame.Grid.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); // Components
            _Frame.Grid.RowDefinitions.Add(36, Skill.Framework.UI.GridUnitType.Pixel); // _BtnCopy    

            _Splitter = new Framework.UI.Label() { Row = 0, Column = 2, RowSpan = 2 };
            _Frame.Controls.Add(_Splitter);

            _SourceLabel = new Framework.UI.Label() { Row = 0, Column = 0, Text = "Source", Margin = new Framework.UI.Thickness(2, 0) };
            _Frame.Controls.Add(_SourceLabel);

            _Source = new Skill.Editor.UI.ObjectField<GameObject>() { Row = 0, Column = 1, AllowSceneObjects = true };
            _Frame.Controls.Add(_Source);

            _DestinationLabel = new Framework.UI.Label() { Row = 0, Column = 3, Text = "Destination", Margin = new Framework.UI.Thickness(2, 0) };
            _Frame.Controls.Add(_DestinationLabel);

            _Destination = new Skill.Editor.UI.ObjectField<GameObject>() { Row = 0, Column = 4, AllowSceneObjects = true };
            _Frame.Controls.Add(_Destination);

            _ComponentsBackground = new Skill.Framework.UI.Label() { Row = 1, Column = 0, ColumnSpan = 5 };
            _Frame.Controls.Add(_ComponentsBackground);

            _SourceComponents = new Skill.Framework.UI.StackPanel() { Row = 1, Column = 0, ColumnSpan = 2 };
            _Frame.Controls.Add(_SourceComponents);

            _DestinationComponents = new Skill.Framework.UI.StackPanel() { Row = 1, Column = 3, ColumnSpan = 2 };
            _Frame.Controls.Add(_DestinationComponents);

            _BtnCopy = new Skill.Framework.UI.Button() { Row = 2, Column = 0, ColumnSpan = 5 };
            _BtnCopy.Content.text = "Copy and Paste";
            _Frame.Controls.Add(_BtnCopy);

            _BtnCopy.Click += _BtnCopy_Click;
            _Source.ObjectChanged += _Source_ObjectChanged;
            _Destination.ObjectChanged += _Destination_ObjectChanged;

            EnableControls(false);

            _Splitter.BringToFront();
        }

        private void EnableControls(bool enable)
        {
            _BtnCopy.IsEnabled = enable;
            _SourceComponents.IsEnabled = enable;
            _DestinationComponents.IsEnabled = enable;
        }

        void _Destination_ObjectChanged(object sender, System.EventArgs e)
        {
            Rebuild();
        }

        void _Source_ObjectChanged(object sender, System.EventArgs e)
        {
            Rebuild();
        }

        void _BtnCopy_Click(object sender, System.EventArgs e)
        {
            CopyComponents();
        }

        void OnGUI()
        {
            if (_Frame != null)
            {
                RefreshStyles();
                _Frame.OnGUI();
            }
        }

        private void RefreshStyles()
        {
            if (_RefreshStyles)
            {
                _RefreshStyles = false;
                _Splitter.Style = Skill.Editor.Resources.Styles.VerticalSplitter;
                _ComponentsBackground.Style = Skill.Editor.Resources.Styles.BackgroundShadow;
            }
        }

        #endregion

        #region Copy Components
        class MatchComponent
        {
            public Component Source;
            public Component Destination;
            public bool Copy;
        }
        private List<MatchComponent> _MatchedComponents = new List<MatchComponent>();

        private bool IsMatchedBefore(Component c)
        {
            for (int i = 0; i < _MatchedComponents.Count; i++)
            {
                if (_MatchedComponents[i].Destination == c)
                    return true;
            }
            return false;
        }

        private Component FindMatch(Component[] components, System.Type type)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].GetType().Equals(type))
                {
                    if (!IsMatchedBefore(components[i]))
                        return components[i];
                }
            }
            return null;
        }

        private void Rebuild()
        {
            if (_Source.Object != null && _Destination.Object != null)
            {
                EnableControls(true);

                Component[] sourceComponents = _Source.Object.GetComponents<Component>();
                Component[] destComponents = _Destination.Object.GetComponents<Component>();

                _MatchedComponents.Clear();
                for (int i = 0; i < sourceComponents.Length; i++)
                {
                    MatchComponent match = new MatchComponent();
                    match.Source = sourceComponents[i];
                    match.Destination = FindMatch(destComponents, match.Source.GetType());
                    match.Copy = true;
                    if (match.Source is Transform)
                        match.Copy = false;
                    _MatchedComponents.Add(match);
                }

                Skill.Framework.UI.Thickness margin = new Skill.Framework.UI.Thickness(2, 2, 2, 6);
                foreach (var item in _MatchedComponents)
                {
                    Skill.Editor.UI.ToggleButton tb = new Skill.Editor.UI.ToggleButton() { Margin = margin, Left = true };
                    tb.IsChecked = item.Copy;
                    tb.UserData = item;
                    tb.Label.text = item.Source.GetType().Name;
                    _SourceComponents.Controls.Add(tb);

                    Skill.Framework.UI.Label lbl = new Skill.Framework.UI.Label() { Margin = margin };
                    if (item.Destination != null)
                        lbl.Text = item.Destination.GetType().Name;
                    _DestinationComponents.Controls.Add(lbl);

                    tb.Changed += ToggleButton_Changed;
                }
            }
            else
            {
                _SourceComponents.Controls.Clear();
                _DestinationComponents.Controls.Clear();
                EnableControls(false);
            }
        }

        void ToggleButton_Changed(object sender, System.EventArgs e)
        {
            Skill.Editor.UI.ToggleButton tb = (Skill.Editor.UI.ToggleButton)sender;
            MatchComponent match = (MatchComponent)tb.UserData;
            match.Copy = tb.IsChecked;
        }

        private void CopyComponents()
        {
            if (_MatchedComponents.Count > 0 && _Destination.Object != null)
            {
                Undo.RecordObject(_Destination.Object, "Add Components");
                foreach (var item in _MatchedComponents)
                {
                    if (item.Copy)
                    {
                        UnityEditorInternal.ComponentUtility.CopyComponent(item.Source);
                        if (item.Destination == null)
                            item.Destination = _Destination.Object.AddComponent(item.Source.GetType());
                        if (item.Destination != null)
                            UnityEditorInternal.ComponentUtility.PasteComponentValues(item.Destination);
                    }
                }
                foreach (var item in _MatchedComponents)
                {
                    if (item.Copy)
                    {
                        if (item.Destination != null)
                            MatchLocalProperties(item.Source, item.Destination);
                    }
                }
            }
        }

        private void MatchLocalProperties(Component source, Component dest)
        {
            Transform sourceRoot = source.transform.root;
            Transform destRoot = dest.transform.root;
            MatchLocalProperties(sourceRoot, destRoot, source, dest);
        }
        private void MatchLocalProperties(Transform sourceRoot, Transform destRoot, object sourceVariable, object destVariable)
        {
            System.Type componentType = typeof(Component);
            System.Reflection.FieldInfo[] fieldInfos = sourceVariable.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var field in fieldInfos)
            {
                object sourceValue = field.GetValue(sourceVariable);
                if (sourceValue != null)
                {
                    if (field.FieldType.IsSubclassOf(componentType))
                    {
                        Transform sourceChild = ((Component)sourceValue).transform;
                        if (Skill.Framework.Utility.IsInHierarchy(sourceRoot, sourceChild))
                        {
                            string path = Skill.Framework.Utility.GetPath(sourceRoot, sourceChild);
                            Transform destChild = destRoot.Find(path);
                            if (destChild != null)
                            {
                                Component destValue = destChild.GetComponent(sourceValue.GetType());
                                if (destValue != null)
                                    field.SetValue(destVariable, destValue);
                                else
                                    field.SetValue(destVariable, null);
                            }
                            else
                            {
                                field.SetValue(destVariable, null);
                            }
                        }
                    }
                    else if (field.FieldType.IsClass && field.FieldType.IsSerializable && field.FieldType.IsByRef &&
                            !field.FieldType.IsPrimitive && !field.FieldType.IsArray && !field.FieldType.Equals(typeof(System.String)))
                    {
                        object destValue = field.GetValue(destVariable);
                        if (destValue != null)
                        {
                            MatchLocalProperties(sourceRoot, destRoot, sourceValue, destValue);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
