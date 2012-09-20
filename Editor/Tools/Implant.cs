using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Skill.UI;

namespace Skill.Editor.Tools
{
    public class Implant : UnityEditor.EditorWindow
    {
        #region Variables
        private static Vector2 Size = new Vector2(320, 140);
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
        private Skill.Editor.UI.ObjectField<ImplantAsset> _AssetField;
        private Skill.Editor.UI.SelectionField _RotationSF;
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
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(316, GridUnitType.Pixel) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });


            _AssetField = new UI.ObjectField<ImplantAsset>() { Row = 0, Column = 0, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2) };
            _AssetField.Label.text = "ImplantAsset";

            _Layers = new Skill.Editor.UI.LayerMaskField() { Layers = 1, Row = 1, Column = 0, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2) };
            _Layers.Label.text = "Raycast layers";

            _RotationSF = new Skill.Editor.UI.SelectionField() { Row = 2, Column = 0, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2) };
            _RotationSF.Label.Width = 110;

            _RandomRotation = new Skill.Editor.UI.XYZComponent();
            _CustomRotation = new Skill.Editor.UI.Vector3Field();
            _RaycastNormalRotation = new Skill.UI.ToggleButton() { HorizontalAlignment = Skill.UI.HorizontalAlignment.Left, Margin = new Thickness(20, 0, 0, 0) };
            _RaycastNormalRotation.Content.text = "Random Y";

            _RotationSF.AddField(_RandomRotation, "Random :");
            _RotationSF.AddField(_CustomRotation, "Custom :");
            _RotationSF.AddField(_RaycastNormalRotation, "Raycast normal :");

            _BtnEnabled = new Skill.Editor.UI.Button() { Margin = new Thickness(2), Row = 3, Column = 0 };
            _BtnEnabled.Content.text = "Enable";
            _BtnEnabled.Click += new System.EventHandler(_BtnEnabled_Click);

            _InfoLabel = new Skill.UI.Label() { Row = 4, Column = 0 };

            _MainGrid.Controls.Add(_AssetField);
            _MainGrid.Controls.Add(_Layers);
            _MainGrid.Controls.Add(_RotationSF);
            _MainGrid.Controls.Add(_BtnEnabled);
            _MainGrid.Controls.Add(_InfoLabel);
            _Frame.Controls.Add(_MainGrid);

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


        private ImplantObject GetRandomImplantObject()
        {
            if (_AssetField.Object == null) return null;
            if (_AssetField.Object.Objects == null || _AssetField.Object.Objects.Length == 0) return null;

            float totalWeight = 0;
            foreach (ImplantObject item in _AssetField.Object.Objects)
                totalWeight += item.Chance;

            float rnd = UnityEngine.Random.Range(0.0f, totalWeight);
            float sum = 0;
            foreach (ImplantObject item in _AssetField.Object.Objects)
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
                    ImplantObject randomObj = GetRandomImplantObject();

                    if (randomObj != null && randomObj.Prefab != null)
                    {
                        Ray r = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
                        RaycastHit hit;
                        if (Physics.Raycast(r, out hit, 1000, _Layers.Layers))
                        {
                            GameObject obj = PrefabUtility.InstantiatePrefab(randomObj.Prefab) as GameObject;
                            if (obj != null)
                            {

                                // scale                                
                                obj.transform.localScale *= UnityEngine.Random.Range(randomObj.MinScalePercent, randomObj.MaxScalePercent);

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
