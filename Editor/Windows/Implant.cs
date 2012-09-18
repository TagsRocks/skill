using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Skill.UI;

namespace Skill.Editor
{
    public class Implant : UnityEditor.EditorWindow
    {
        #region Variables
        private static Vector2 Size = new Vector2(605, 200);
        private static Implant _Instance;
        #endregion

        #region Properties
        public static Implant Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = EditorWindow.GetWindow<Implant>();
                }
                return _Instance;
            }
        }
        #endregion

        #region Constructor
        public Implant()
        {
            hideFlags = HideFlags.DontSave;

            if (_Instance != null)
            {
                Debug.LogError("Trying to create two instances of singleton. Self destruction in 3...");
                Destroy(this);
                return;
            }

            _Instance = this;

            title = "Implant";
            position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            this.minSize = Size;

            CreateUI();
        }
        #endregion

        #region Destroy
        public void OnDestroy()
        {
            _Instance = null;
        }
        #endregion


        #region UI

        private Skill.Editor.UI.EditorFrame _Frame;
        private Skill.UI.Grid _MainGrid;
        private Skill.UI.ListBox _PrefabsLB;
        private Skill.Editor.UI.SelectionField _RotationSF;
        private Skill.UI.StackPanel _ButtonsPanel;
        private Skill.Editor.UI.Button _BtnAdd;
        private Skill.Editor.UI.Button _BtnRemove;
        private Skill.Editor.UI.Button _BtnClear;
        private Skill.Editor.UI.LayerMaskField _Layers;
        private Skill.Editor.UI.XYZComponent _RandomRotation;
        private Skill.Editor.UI.Vector3Field _CustomRotation;
        private Skill.UI.ToggleButton _RaycastNormalRotation;
        private Skill.UI.Label _InfoLabel;

        private bool _IsImplantEnable;
        private Skill.Editor.UI.Button _BtnEnabled;

        private void CreateUI()
        {
            _Frame = new Skill.Editor.UI.EditorFrame(this);

            _MainGrid = new Skill.UI.Grid();
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(304, GridUnitType.Pixel) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300, GridUnitType.Pixel) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            _PrefabsLB = new ListBox() { Row = 0, Column = 0, RowSpan = 5, Margin = new Thickness(2), SelectionMode = Skill.UI.SelectionMode.Single };
            _PrefabsLB.Background.Visibility = Visibility.Visible;
            _PrefabsLB.SelectionChanged += new System.EventHandler(_PrefabsLB_SelectionChanged);

            _Layers = new Skill.Editor.UI.LayerMaskField() { Layers = 1, Row = 0, Column = 1, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2) };
            _Layers.Label.text = "Raycast layers";

            _RotationSF = new Skill.Editor.UI.SelectionField() { Row = 1, Column = 1 };
            _RotationSF.Label.Width = 110;

            _RandomRotation = new Skill.Editor.UI.XYZComponent();
            _CustomRotation = new Skill.Editor.UI.Vector3Field();
            _RaycastNormalRotation = new Skill.UI.ToggleButton() { HorizontalAlignment = Skill.UI.HorizontalAlignment.Left, Margin = new Thickness(20, 0, 0, 0) };
            _RaycastNormalRotation.Content.text = "Random Y";

            _RotationSF.AddField(_RandomRotation, "Random :");
            _RotationSF.AddField(_CustomRotation, "Custom :");
            _RotationSF.AddField(_RaycastNormalRotation, "Raycast normal :");


            _ButtonsPanel = new StackPanel() { Row = 2, Column = 1, HorizontalAlignment = HorizontalAlignment.Left, Width = 100 };

            _BtnAdd = new Skill.Editor.UI.Button() { Margin = new Thickness(2, 2, 2, 0) };
            _BtnAdd.Content.text = "Add";
            _BtnAdd.Click += new System.EventHandler(_BtnAdd_Click);

            _BtnRemove = new Skill.Editor.UI.Button() { Margin = new Thickness(2, 2, 2, 0), IsEnabled = false };
            _BtnRemove.Content.text = "Remove";
            _BtnRemove.Click += new System.EventHandler(_BtnRemove_Click);

            _BtnClear = new Skill.Editor.UI.Button() { Margin = new Thickness(2, 2, 2, 0) };
            _BtnClear.Content.text = "Clear";
            _BtnClear.Click += new System.EventHandler(_BtnClear_Click);

            _ButtonsPanel.Controls.Add(_BtnAdd);
            _ButtonsPanel.Controls.Add(_BtnRemove);
            _ButtonsPanel.Controls.Add(_BtnClear);


            _BtnEnabled = new Skill.Editor.UI.Button() { Margin = new Thickness(2), Row = 4, Column = 1 };
            _BtnEnabled.Content.text = "Enable";
            _BtnEnabled.Click += new System.EventHandler(_BtnEnabled_Click);

            _InfoLabel = new Skill.UI.Label() { Row = 3, Column = 1 };

            _MainGrid.Controls.Add(_PrefabsLB);
            _MainGrid.Controls.Add(_Layers);
            _MainGrid.Controls.Add(_RotationSF);
            _MainGrid.Controls.Add(_ButtonsPanel);
            _MainGrid.Controls.Add(_BtnEnabled);
            _MainGrid.Controls.Add(_InfoLabel);
            _Frame.Controls.Add(_MainGrid);

        }

        void _BtnRemove_Click(object sender, System.EventArgs e)
        {
            if (_PrefabsLB.SelectedItem != null)
                _PrefabsLB.Controls.Remove(_PrefabsLB.SelectedItem);
        }

        void _BtnEnabled_Click(object sender, System.EventArgs e)
        {
            _IsImplantEnable = !_IsImplantEnable;
            if (_IsImplantEnable)
            {
                _BtnEnabled.Content.text = "Disable";
                _InfoLabel.Content.text = "Hold Ctrl and Right Click";
            }
            else
            {
                _BtnEnabled.Content.text = "Enable";
                _InfoLabel.Content.text = "";
            }
        }

        void _PrefabsLB_SelectionChanged(object sender, System.EventArgs e)
        {
            _BtnRemove.IsEnabled = _PrefabsLB.SelectedItem != null;
        }

        void _BtnClear_Click(object sender, System.EventArgs e)
        {
            _PrefabsLB.Controls.Clear();
        }

        void _BtnAdd_Click(object sender, System.EventArgs e)
        {
            _PrefabsLB.Controls.Add(new ImplantObjectField());
        }

        #endregion

        public void OnEnable()
        {
            SceneView.onSceneGUIDelegate += UpdateScene;
        }

        public void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= UpdateScene;
        }

        public void OnGUI()
        {
            Rect rect = position;
            rect.x = 0;
            rect.y = 0;
            _MainGrid.Position = rect;
            _Frame.OnGUI();
        }


        private ImplantObjectField GetRandomField()
        {
            if (_PrefabsLB.Controls.Count == 0) return null;

            float totalWeight = 0;
            foreach (ImplantObjectField item in _PrefabsLB.Controls)
                totalWeight += item.Chance;

            float rnd = UnityEngine.Random.Range(0.0f, totalWeight);
            float sum = 0;
            foreach (ImplantObjectField item in _PrefabsLB.Controls)
            {
                sum += item.Chance;
                if (sum >= rnd) return item;
            }
            return null;

        }

        void UpdateScene(SceneView sceneview)
        {
            if (_IsImplantEnable)
            {
                Event e = Event.current;

                if (e.isMouse && e.type == EventType.MouseDown && e.button == 1 && e.modifiers == EventModifiers.Control)
                {
                    ImplantObjectField randomField = GetRandomField();

                    if (randomField != null && randomField.Prefab != null)
                    {
                        Ray r = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
                        RaycastHit hit;
                        if (Physics.Raycast(r, out hit, 1000, _Layers.Layers))
                        {
                            GameObject obj = PrefabUtility.InstantiatePrefab(randomField.Prefab) as GameObject;
                            if (obj != null)
                            {

                                // scale                                
                                obj.transform.localScale *= UnityEngine.Random.Range(randomField.MinScale, randomField.MaxScale);

                                // position
                                obj.transform.position = hit.point;

                                // rotation
                                Quaternion rotation = obj.transform.rotation;
                                if (_RotationSF.SelectedField == _RandomRotation)
                                {
                                    Vector3 euler = obj.transform.eulerAngles;
                                    if (_RandomRotation.IsXChecked) euler.x = UnityEngine.Random.Range(0.0f, 360.0f);
                                    if (_RandomRotation.IsYChecked) euler.y = UnityEngine.Random.Range(0.0f, 360.0f);
                                    if (_RandomRotation.IszChecked) euler.z = UnityEngine.Random.Range(0.0f, 360.0f);
                                    rotation = Quaternion.Euler(euler);
                                }
                                else if (_RotationSF.SelectedField == _CustomRotation)
                                {
                                    rotation = Quaternion.Euler(_CustomRotation.Value);
                                }
                                else
                                {
                                    rotation *= Quaternion.FromToRotation(obj.transform.up, hit.normal);

                                    if (_RaycastNormalRotation.IsChecked)
                                    {
                                        rotation *= Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0);
                                    }

                                }
                                obj.transform.rotation = rotation;
                                Undo.RegisterCreatedObjectUndo(obj, "Create " + obj.name);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Please set valid prefab to implant");
                    }
                }
            }
        }
    }
}
