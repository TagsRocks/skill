using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Skill.Framework.UI;

namespace Skill.Editor
{
    public class MoveWindow : EditorWindow
    {
        #region EditorWindow
        private static MoveWindow _Instance;
        public static MoveWindow Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = ScriptableObject.CreateInstance<MoveWindow>();
                return _Instance;
            }
        }

        private static Vector2 Size = new Vector2(220, 120);

        public MoveWindow()
        {
            hideFlags = HideFlags.DontSave;

            base.titleContent = new GUIContent("Move");
            base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = Size;
            CreateUI();
        }

        void OnFocus()
        {
            if (_Frame != null)
            {
                _XStep.Value = _SerializedStep.x;
                _YStep.Value = _SerializedStep.y;
                _ZStep.Value = _SerializedStep.z;
                _IntInstance.Value = _SerializedInstances;
                _TbDuplicate.IsChecked = _SerializedDuplicate;
                _BtnLocal.Content.text = _SerializedLocal ? "Local" : "Global";
                _RefreshStyles = true;
            }
        }

        //void OnLostFocus()
        //{
        //    if (_Frame != null)
        //    {
        //        _SerializedStep = _XYZStep.Value;
        //        _SerializedInstances = _IntInstance.Value;
        //        _SerializedDuplicate = _TbDuplicate.IsChecked;
        //    }
        //}

        #endregion

        #region Serialized Variables

        [SerializeField]
        private Vector3 _SerializedStep = new Vector3(1, 0, 0);
        [SerializeField]
        private int _SerializedInstances = 1;
        [SerializeField]
        private bool _SerializedDuplicate = false;
        [SerializeField]
        private bool _SerializedLocal = true;

        #endregion

        #region UI
        private bool _RefreshStyles = true;
        private Skill.Editor.UI.EditorFrame _Frame;
        private Skill.Framework.UI.Grid _Panel;
        private FieldComponent _XStep;
        private FieldComponent _YStep;
        private FieldComponent _ZStep;
        private Skill.Editor.UI.ToggleButton _TbDuplicate;
        private Skill.Editor.UI.IntField _IntInstance;
        private Skill.Framework.UI.Button _BtnMove;
        private Skill.Framework.UI.Button _BtnLocal;


        private void CreateUI()
        {
            _Frame = new Skill.Editor.UI.EditorFrame("Frame", this);
            _Frame.Grid.RowDefinitions.Add(120, Skill.Framework.UI.GridUnitType.Pixel); // Panel
            _Frame.Grid.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); // Empty        
            _Frame.Grid.Padding = new Skill.Framework.UI.Thickness(2, 4);


            _Panel = new Skill.Framework.UI.Grid();
            _Panel.RowDefinitions.Add(36, Skill.Framework.UI.GridUnitType.Pixel); // Axis step
            _Panel.RowDefinitions.Add(24, Skill.Framework.UI.GridUnitType.Pixel); // _BtnLocal
            _Panel.RowDefinitions.Add(22, Skill.Framework.UI.GridUnitType.Pixel); // Duplicate
            _Panel.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); // MoveButton        
            _Panel.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
            _Panel.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
            _Frame.Controls.Add(_Panel);

            var margin = new Skill.Framework.UI.Thickness(0, 4, 0, 0);
            var alignment = Skill.Framework.UI.VerticalAlignment.Top;

            Grid xyzStep = new Grid() { Row = 0, Column = 0, ColumnSpan = 2 };
            xyzStep.ColumnDefinitions.Add(1, GridUnitType.Star);
            xyzStep.ColumnDefinitions.Add(1, GridUnitType.Star);
            xyzStep.ColumnDefinitions.Add(1, GridUnitType.Star);
            _Panel.Controls.Add(xyzStep);

            _XStep = new FieldComponent() { Column = 0, Value = _SerializedStep.x, Label = "X", Margin = new Thickness() };
            xyzStep.Controls.Add(_XStep);

            _YStep = new FieldComponent() { Column = 1, Value = _SerializedStep.y, Label = "Y", Margin = new Thickness(4, 0, 0, 0) };
            xyzStep.Controls.Add(_YStep);

            _ZStep = new FieldComponent() { Column = 2, Value = _SerializedStep.z, Label = "Z", Margin = new Thickness(4, 0, 0, 0) };
            xyzStep.Controls.Add(_ZStep);


            _BtnLocal = new Framework.UI.Button() { Row = 1, Column = 0, ColumnSpan = 2, Margin = margin };
            _BtnLocal.Content.text = _SerializedLocal ? "Local" : "Global";
            _BtnLocal.Content.tooltip = "local or global space";
            _Panel.Controls.Add(_BtnLocal);

            _TbDuplicate = new Skill.Editor.UI.ToggleButton() { Row = 2, Column = 0, Left = true, IsChecked = _SerializedDuplicate, Margin = margin, VerticalAlignment = alignment };
            _TbDuplicate.Label.text = "Duplicate";
            _TbDuplicate.Label.tooltip = "duplicate instance";
            _Panel.Controls.Add(_TbDuplicate);

            _IntInstance = new Skill.Editor.UI.IntField() { Row = 2, Column = 1, Value = _SerializedInstances, IsEnabled = false, Margin = margin, VerticalAlignment = alignment, ChangeOnReturn = false };
            _IntInstance.Label.tooltip = "duplicate instance";
            _Panel.Controls.Add(_IntInstance);

            _BtnMove = new Skill.Framework.UI.Button() { Row = 3, Column = 0, ColumnSpan = 2 };
            SetMoveText();
            _Panel.Controls.Add(_BtnMove);

            _TbDuplicate.Changed += _TbDuplicate_Changed;
            _IntInstance.ValueChanged += _IntInstance_ValueChanged;
            _XStep.ValueChanged += _XYZStep_ValueChanged;
            _YStep.ValueChanged += _XYZStep_ValueChanged;
            _ZStep.ValueChanged += _XYZStep_ValueChanged;
            _BtnMove.Click += _BtnMove_Click;
            _BtnLocal.Click += _BtnLocal_Click;
        }

        void _BtnLocal_Click(object sender, System.EventArgs e)
        {
            _SerializedLocal = !_SerializedLocal;
            _BtnLocal.Content.text = _SerializedLocal ? "Local" : "Global";
            _RefreshStyles = true;
        }

        void _XYZStep_ValueChanged(object sender, System.EventArgs e)
        {
            _SerializedStep.x = _XStep.Value;
            _SerializedStep.y = _YStep.Value;
            _SerializedStep.z = _ZStep.Value;
            UnityEditor.EditorUtility.SetDirty(this);
        }

        void _IntInstance_ValueChanged(object sender, System.EventArgs e)
        {
            if (_IntInstance.Value < 1) _IntInstance.Value = 1;
            _SerializedInstances = _IntInstance.Value;
            UnityEditor.EditorUtility.SetDirty(this);
        }
        void _TbDuplicate_Changed(object sender, System.EventArgs e)
        {
            SetMoveText();
            _IntInstance.IsEnabled = _TbDuplicate.IsChecked;
            _SerializedDuplicate = _TbDuplicate.IsChecked;
            UnityEditor.EditorUtility.SetDirty(this);
        }

        private void SetMoveText()
        {
            if (_TbDuplicate.IsChecked)
            {
                _BtnMove.Content.text = "Duplicate & Move";
                _BtnMove.Content.tooltip = "duplicate selected and move it";
            }
            else
            {
                _BtnMove.Content.text = "Move";
                _BtnMove.Content.tooltip = "move selected";
            }
        }



        void _BtnMove_Click(object sender, System.EventArgs e)
        {
            if (_TbDuplicate.IsChecked)
                DuplicateAndMove();
            else
                Move();
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
                _BtnMove.Content.image = EditorGUIUtility.FindTexture("d_MoveTool on");
                _BtnLocal.Content.image = EditorGUIUtility.FindTexture(_SerializedLocal ? "d_ToolHandleLocal" : "d_ToolHandleGlobal");
            }
        }

        #endregion

        private Vector3 XYZStepValue { get { return new Vector3(_XStep.Value, _YStep.Value, _ZStep.Value); } }

        private void Move()
        {
            if (_XStep.Value == 0 && _YStep.Value == 0 && _ZStep.Value == 0) return;
            var transforms = Selection.transforms;
            if (transforms != null && transforms.Length > 0)
            {
                foreach (var t in transforms)
                {
                    Undo.RecordObject(t, "Move");
                    if (_SerializedLocal)
                    {
                        if (t.parent != null)
                        {
                            t.localPosition += t.localRotation * XYZStepValue;
                        }
                        else
                            t.position += t.rotation * XYZStepValue;
                    }
                    else
                    {
                        t.position += XYZStepValue;
                    }
                }
            }
        }

        private void DuplicateAndMove()
        {
            var transforms = Selection.transforms;
            if (transforms != null && transforms.Length > 0 && _IntInstance.Value > 0)
            {
                List<GameObject> newObjects = new List<GameObject>();
                foreach (var t in transforms)
                {
                    Vector3 offset = Vector3.zero;
                    for (int i = 0; i < _IntInstance.Value; i++)
                    {
                        GameObject obj = null;
                        offset += XYZStepValue;

                        var pt = PrefabUtility.GetPrefabType(t.gameObject);
                        if (pt == PrefabType.Prefab || pt == PrefabType.ModelPrefab || pt == PrefabType.ModelPrefabInstance || pt == PrefabType.PrefabInstance)
                        {
                            Object prefab = PrefabUtility.GetPrefabParent(t.gameObject);
                            if (prefab != null)
                                obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                            if (obj == null)
                                obj = (GameObject)GameObject.Instantiate(t.gameObject, t.position + offset, t.rotation);
                        }
                        else
                        {
                            obj = (GameObject)GameObject.Instantiate(t.gameObject, t.position + offset, t.rotation);
                        }

                        obj.name = t.gameObject.name;
                        if (t.parent != null)
                        {
                            obj.transform.parent = t.parent;
                            obj.transform.localRotation = t.localRotation;
                            if (_SerializedLocal)
                                obj.transform.localPosition = t.localPosition + (t.localRotation * offset);
                            else
                                obj.transform.position = t.position + offset;
                        }
                        else
                        {
                            obj.transform.rotation = t.rotation;
                            if (_SerializedLocal)
                                obj.transform.position = t.position + (t.rotation * offset);
                            else
                                obj.transform.position = t.position + offset;

                        }

                        Undo.RegisterCreatedObjectUndo(obj, "Duplicate");
                        newObjects.Add(obj);
                    }
                }
                if (newObjects.Count > 0)
                    Selection.objects = newObjects.ToArray();
            }
        }





        class FieldComponent : Grid
        {
            private Skill.Editor.UI.FloatField _ValueField;
            private Skill.Framework.UI.Label _Label;
            private Skill.Framework.UI.Button _BtnSign;
            private Skill.Framework.UI.Button _BtnIncrease;
            private Skill.Framework.UI.Button _BtnDecrease;
            private bool _RefreshStyles;

            public float Value { get { return _ValueField.Value; } set { _ValueField.Value = value; } }
            public string Label { get { return _Label.Text; } set { _Label.Text = value; } }

            /// <summary>
            /// Occurs when value of FloatField changed
            /// </summary>
            public event System.EventHandler ValueChanged;
            /// <summary>
            /// when value of FloatField changed
            /// </summary>
            protected virtual void OnValueChanged()
            {
                if (ValueChanged != null) ValueChanged(this, System.EventArgs.Empty);
            }

            public FieldComponent()
            {

                this.ColumnDefinitions.Add(16, GridUnitType.Pixel);
                this.ColumnDefinitions.Add(1, GridUnitType.Star);
                this.ColumnDefinitions.Add(1, GridUnitType.Star);
                this.ColumnDefinitions.Add(1, GridUnitType.Star);

                this.RowDefinitions.Add(20, GridUnitType.Pixel);
                this.RowDefinitions.Add(1, GridUnitType.Star);

                _ValueField = new UI.FloatField() { Row = 0, Column = 1, ColumnSpan = 3 };
                this.Controls.Add(_ValueField);

                _Label = new Label() { Row = 0, Column = 0 };
                this.Controls.Add(_Label);

                _BtnSign = new Button() { Row = 1, Column = 1 };
                this.Controls.Add(_BtnSign);

                _BtnIncrease = new Button() { Row = 1, Column = 2 };
                this.Controls.Add(_BtnIncrease);

                _BtnDecrease = new Button() { Row = 1, Column = 3 };
                this.Controls.Add(_BtnDecrease);

                _BtnSign.Click += _BtnSign_Click;
                _BtnIncrease.Click += _BtnIncrease_Click;
                _BtnDecrease.Click += _BtnDecrease_Click;
                _ValueField.ValueChanged += _ValueField_ValueChanged;
                _RefreshStyles = true;
            }

            void _ValueField_ValueChanged(object sender, System.EventArgs e)
            {
                OnValueChanged();
            }

            void _BtnSign_Click(object sender, System.EventArgs e) { Value *= -1; }
            void _BtnIncrease_Click(object sender, System.EventArgs e) { Value++; }
            void _BtnDecrease_Click(object sender, System.EventArgs e) { Value--; }


            protected override void BeginRender()
            {
                base.BeginRender();
                if (_RefreshStyles)
                {
                    _RefreshStyles = false;
                    RefreshStyles();
                }
            }


            private void RefreshStyles()
            {

                _BtnSign.Content.image = Skill.Editor.Resources.UITextures.Remove;
                _BtnIncrease.Content.image = Skill.Editor.Resources.UITextures.Plus;
                _BtnDecrease.Content.image = Skill.Editor.Resources.UITextures.Minus;

                GUIStyle style = new GUIStyle(Skill.Editor.Resources.Styles.SmallButton);
                style.alignment = TextAnchor.MiddleCenter;

                _BtnSign.Style = style;
                _BtnIncrease.Style = style;
                _BtnDecrease.Style = style;
            }
        }
    }
}
