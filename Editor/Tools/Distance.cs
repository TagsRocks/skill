using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using Skill.UI;

namespace Skill.Editor.Tools
{
    public class Distance : UnityEditor.EditorWindow
    {
        #region Variables
        private static Vector2 Size = new Vector2(320, 180);
        private static Distance _Instance;
        #endregion

        #region Properties
        public static Distance Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = EditorWindow.GetWindow<Distance>();
                }
                return _Instance;
            }
        }
        #endregion

        #region Constructor
        public Distance()
        {
            hideFlags = HideFlags.DontSave;

            if (_Instance != null)
            {
                Debug.LogError("Trying to create two instances of singleton. Self destruction in 3...");
                Destroy(this);
                return;
            }

            _Instance = this;

            title = "Distance";
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
        private Skill.UI.Box _Title;
        private Skill.Editor.UI.LayerMaskField _Layers;
        private Skill.Editor.UI.LabelField _LblStartPoint;
        private Skill.Editor.UI.LabelField _LblEndPoint;
        private Skill.Editor.UI.LabelField _LblDistance;
        private Skill.Editor.UI.Button _BtnEnabled;
        private Skill.UI.Label _InfoLabel;
        private bool _IsDistanceEnable;

        private Vector3 _StartPoint;
        private Vector3 _EndPoint;
        private bool _IsStartPointPicked;
        private bool _IsEndPointPicked;


        private void CreateUI()
        {
            _Frame = new Skill.Editor.UI.EditorFrame(this);

            _MainGrid = new Skill.UI.Grid();
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(316, GridUnitType.Pixel) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            _Title = new Box() { Row = 0, Column = 0, Margin = new Thickness(2) };
            _Title.Content.text = "Measure distance of two points";

            _Layers = new Skill.Editor.UI.LayerMaskField() { Layers = 0xFFFFFFF, Row = 1, Column = 0, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2) };
            _Layers.Label.text = "Raycast layers";

            _LblStartPoint = new Skill.Editor.UI.LabelField() { Row = 2, Column = 0, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2) };
            _LblStartPoint.Label.text = "Start Point";

            _LblEndPoint = new Skill.Editor.UI.LabelField() { Row = 3, Column = 0, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2) };
            _LblEndPoint.Label.text = "End Point";

            _LblDistance = new Skill.Editor.UI.LabelField() { Row = 4, Column = 0, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2) };
            _LblDistance.Label.text = "Distance";

            _BtnEnabled = new Skill.Editor.UI.Button() { Margin = new Thickness(2), Row = 5, Column = 0 };
            _BtnEnabled.Content.text = "Enable";
            _BtnEnabled.Click += new System.EventHandler(_BtnEnabled_Click);

            _InfoLabel = new Skill.UI.Label() { Row = 6, Column = 0 };

            _MainGrid.Controls.Add(_Title);
            _MainGrid.Controls.Add(_Layers);
            _MainGrid.Controls.Add(_LblStartPoint);
            _MainGrid.Controls.Add(_LblEndPoint);
            _MainGrid.Controls.Add(_LblDistance);
            _MainGrid.Controls.Add(_BtnEnabled);
            _MainGrid.Controls.Add(_InfoLabel);
            _Frame.Controls.Add(_MainGrid);

        }

        void _BtnEnabled_Click(object sender, System.EventArgs e)
        {
            _IsDistanceEnable = !_IsDistanceEnable;
            if (_IsDistanceEnable)
            {
                _BtnEnabled.Content.text = "Disable";
                _InfoLabel.Content.text = "Hold Ctrl and Right Click";
            }
            else
            {
                _BtnEnabled.Content.text = "Enable";
                _InfoLabel.Content.text = "";
                _IsStartPointPicked = _IsEndPointPicked = false;
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

        void UpdateScene(SceneView sceneview)
        {
            if (_IsDistanceEnable)
            {
                Event e = Event.current;

                Ray r = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
                RaycastHit hit;
                if (Physics.Raycast(r, out hit, 1000, _Layers.Layers))
                {
                    if (e.isMouse && e.type == EventType.MouseDown && e.button == 1 && e.modifiers == EventModifiers.Control)
                    {
                        if (_IsStartPointPicked)
                        {
                            _EndPoint = hit.point;
                            _LblEndPoint.Label2.text = _EndPoint.ToString();                            
                            _IsStartPointPicked = false;
                            _IsEndPointPicked = true;
                        }
                        else
                        {
                            _EndPoint = _StartPoint = hit.point;
                            _LblStartPoint.Label2.text = _StartPoint.ToString();
                            _LblDistance.Label2.text = "0";
                            _IsStartPointPicked = true;
                            _IsEndPointPicked = false;
                            Repaint();
                        }
                    }
                    if (_IsEndPointPicked)
                    {
                        _LblDistance.Label2.text = Vector3.Distance(_StartPoint, _EndPoint).ToString("F4");
                        Debug.DrawLine(_StartPoint, _EndPoint, Color.red, 0.1f);
                        Repaint();
                        _IsEndPointPicked = false;
                    }
                }
            }
        }
    }
}
