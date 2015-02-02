using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

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

        private static Vector2 Size = new Vector2(220, 86);

        public MoveWindow()
        {
            hideFlags = HideFlags.DontSave;

            base.title = "Move";
            base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = Size;
            CreateUI();
        }

        void OnFocus()
        {
            if (_Frame != null)
            {
                _XYZStep.Value = _SerializedStep;
                _IntInstance.Value = _SerializedInstances;
                _TbDuplicate.IsChecked = _SerializedDuplicate;
            }
        }

        void OnLostFocus()
        {
            if (_Frame != null)
            {
                _SerializedStep = _XYZStep.Value;
                _SerializedInstances = _IntInstance.Value;
                _SerializedDuplicate = _TbDuplicate.IsChecked;
            }
        }

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
        private Skill.Editor.UI.Vector3Field _XYZStep;
        private Skill.Editor.UI.ToggleButton _TbDuplicate;
        private Skill.Editor.UI.IntField _IntInstance;
        private Skill.Framework.UI.Button _BtnMove;
        private Skill.Framework.UI.Button _BtnLocal;


        private void CreateUI()
        {
            _Frame = new Skill.Editor.UI.EditorFrame("Frame", this);
            _Frame.Grid.RowDefinitions.Add(80, Skill.Framework.UI.GridUnitType.Pixel); // Panel
            _Frame.Grid.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); // Empty        
            _Frame.Grid.Padding = new Skill.Framework.UI.Thickness(2, 4);


            _Panel = new Skill.Framework.UI.Grid();
            _Panel.RowDefinitions.Add(20, Skill.Framework.UI.GridUnitType.Pixel); // Axis step
            _Panel.RowDefinitions.Add(22, Skill.Framework.UI.GridUnitType.Pixel); // _BtnLocal
            _Panel.RowDefinitions.Add(22, Skill.Framework.UI.GridUnitType.Pixel); // Duplicate
            _Panel.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); // MoveButton        
            _Panel.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
            _Panel.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
            _Frame.Controls.Add(_Panel);

            var margin = new Skill.Framework.UI.Thickness(0, 4, 0, 0);
            var alignment = Skill.Framework.UI.VerticalAlignment.Top;

            _XYZStep = new Skill.Editor.UI.Vector3Field() { Row = 0, Column = 0, ColumnSpan = 2, Value = _SerializedStep };
            _Panel.Controls.Add(_XYZStep);

            _BtnLocal = new Framework.UI.Button() { Row = 1, Column = 0, ColumnSpan = 2, Margin = margin, VerticalAlignment = alignment };
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
            _XYZStep.ValueChanged += _XYZStep_ValueChanged;
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
            _SerializedStep = _XYZStep.Value;
        }

        void _IntInstance_ValueChanged(object sender, System.EventArgs e)
        {
            if (_IntInstance.Value < 1) _IntInstance.Value = 1;
            _SerializedInstances = _IntInstance.Value;
        }
        void _TbDuplicate_Changed(object sender, System.EventArgs e)
        {
            SetMoveText();
            _IntInstance.IsEnabled = _TbDuplicate.IsChecked;
            _SerializedDuplicate = _TbDuplicate.IsChecked;
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

        private void Move()
        {
            if (_XYZStep.Value == Vector3.zero) return;
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
                            t.localPosition += t.localRotation * _XYZStep.Value;
                        }
                        else
                            t.position += t.rotation * _XYZStep.Value;
                    }
                    else
                    {
                        t.position += _XYZStep.Value;
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
                        offset += _XYZStep.Value;

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
    }
}
