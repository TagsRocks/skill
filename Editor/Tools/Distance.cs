using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using Skill.Framework.UI;

namespace Skill.Editor.Tools
{
    class Distance : UnityEditor.EditorWindow
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

        private static readonly Vector3 LabelOffset = new Vector3(0, 0.5f, 0);

        private Skill.Editor.UI.EditorFrame _Frame;
        private Skill.Framework.UI.Box _Title;
        private Skill.Editor.UI.LayerMaskField _Layers;
        private Skill.Editor.UI.LabelField _LblStartPoint;
        private Skill.Editor.UI.LabelField _LblEndPoint;
        private Skill.Editor.UI.LabelField _LblDistance;
        private Skill.Editor.UI.Button _BtnEnabled;
        private Skill.Framework.UI.Label _InfoLabel;
        private bool _IsDistanceEnable;

        private Vector3 _StartPoint;
        private Vector3 _EndPoint;
        private bool _IsStartPointPicked;
        private bool _IsEndPointPicked;

        private GUIStyle _LableStyle;

        private void CreateUI()
        {
            _LableStyle = new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = Color.black, background = Resources.WhiteTexture },
                padding = new RectOffset(2, 2, 2, 2)
            };

            _Frame = new Skill.Editor.UI.EditorFrame("Frame", this);

            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            _Frame.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(316, GridUnitType.Pixel) });
            _Frame.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

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

            _InfoLabel = new Skill.Framework.UI.Label() { Row = 6, Column = 0 };

            _Frame.Grid.Controls.Add(_Title);
            _Frame.Grid.Controls.Add(_Layers);
            _Frame.Grid.Controls.Add(_LblStartPoint);
            _Frame.Grid.Controls.Add(_LblEndPoint);
            _Frame.Grid.Controls.Add(_LblDistance);
            _Frame.Grid.Controls.Add(_BtnEnabled);
            _Frame.Grid.Controls.Add(_InfoLabel);

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

        void OnEnable()
        {
            SceneView.onSceneGUIDelegate += UpdateScene;
        }

        void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= UpdateScene;
        }

        void OnGUI()
        {
            _Frame.OnGUI();
        }

        void UpdateScene(SceneView sceneview)
        {
            if (_IsDistanceEnable)
            {
                Event e = Event.current;

                bool colide = false;
                Ray r = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
                RaycastHit hit;
                if (Physics.Raycast(r, out hit, 1000, _Layers.Layers))
                {
                    colide = true;
                    Handles.color = Color.red;
                    Handles.ArrowCap(0, hit.point, Quaternion.LookRotation(hit.normal), 0.5f);
                    Handles.SphereCap(0, hit.point, Quaternion.LookRotation(hit.normal), 0.1f);
                    SceneView.currentDrawingSceneView.Repaint();
                }

                if (colide && e.isMouse && e.type == EventType.MouseDown && e.button == 1 && e.modifiers == EventModifiers.Control)
                {
                    if (_IsStartPointPicked)
                    {
                        if (_IsEndPointPicked)
                        {
                            _StartPoint = hit.point;
                            _IsEndPointPicked = false;
                            _IsStartPointPicked = true;
                        }
                        else
                        {
                            _EndPoint = hit.point;
                            _IsEndPointPicked = true;
                        }
                    }
                    else
                    {
                        _StartPoint = hit.point;
                        _IsEndPointPicked = false;
                        _IsStartPointPicked = true;
                    }

                    if (_IsStartPointPicked)
                        _LblStartPoint.Label2.text = _StartPoint.ToString();
                    else
                        _LblStartPoint.Label2.text = "";

                    if (_IsEndPointPicked)
                        _LblEndPoint.Label2.text = _EndPoint.ToString();
                    else
                        _LblEndPoint.Label2.text = "";

                    if (_IsEndPointPicked && _IsStartPointPicked)
                        _LblDistance.Label2.text = Vector3.Distance(_StartPoint, _EndPoint).ToString("F4");
                    else
                        _LblDistance.Label2.text = "---";
                    Repaint();
                }

                if (_IsStartPointPicked)
                {
                    Handles.color = Color.grey;
                    Handles.SphereCap(0, _StartPoint, Quaternion.identity, 0.1f);
                }
                if (_IsEndPointPicked)
                {
                    Handles.color = Color.grey;
                    Handles.SphereCap(0, _EndPoint, Quaternion.identity, 0.1f);
                }

                if (_IsEndPointPicked && _IsStartPointPicked)
                {
                    Handles.color = Color.red;
                    Handles.DrawLine(_StartPoint, _EndPoint);
                    Handles.color = Color.red;
                    Handles.Label((_StartPoint + _EndPoint) / 2 + LabelOffset,
                     string.Format("Distance = {0}", Vector3.Distance(_StartPoint, _EndPoint).ToString("F4")), _LableStyle);
                }
            }
        }
    }
}
