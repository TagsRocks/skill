using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using Skill.Framework.UI;

namespace Skill.Editor
{
    class Distance : UnityEditor.EditorWindow
    {
        #region Variables
        private static Vector2 Size = new Vector2(340, 220);
        private static Distance _Instance;
        private Skill.Framework.Modules.Meter _Meter;
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
            if (_Instance != null)
            {
                Debug.LogError("Trying to create two instances of singleton. Self destruction in 3...");
                Destroy(this);
                return;
            }

            _Instance = this;

            titleContent = new GUIContent("Distance");
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
        private Skill.Editor.UI.HelpBox _Help;
        private Skill.Editor.UI.LayerMaskField _Layers;

        private Grid _PnlMeter;
        private Box _PnlMeterBg;
        private Skill.Editor.UI.TextField _TfMeterName;
        private Skill.Editor.UI.ToggleButton _TbCreateMeter;
        private Skill.Editor.UI.ToggleButton _TbAttachMeter;

        private Grid _PnlInfo;
        private Box _PnlInfoBg;
        private Skill.Editor.UI.LabelField _LblStartPoint;
        private Skill.Editor.UI.LabelField _LblEndPoint;
        private Skill.Editor.UI.LabelField _LblDistance;


        private Skill.Framework.UI.Button _BtnEnabled;
        private Skill.Editor.UI.HelpBox _InfoBox;
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
                normal = new GUIStyleState() { textColor = Color.black, background = Resources.UITextures.Colors.White },
                padding = new RectOffset(2, 2, 2, 2)
            };

            _Frame = new Skill.Editor.UI.EditorFrame("Frame", this) { Location = FrameLocation.Fill };

            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(44, GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(64, GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            _Help = new UI.HelpBox() { Row = 0, Column = 0, Margin = new Thickness(2) };
            _Help.Message = "Measure distance of two points";

            _Layers = new Skill.Editor.UI.LayerMaskField() { Layers = 0xFFFFFFF, Row = 1, Column = 0, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2) };
            _Layers.Label.text = "Raycast layers";

            _PnlMeter = new Grid() { Row = 2, Padding = new Thickness(2) };
            _PnlMeter.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _PnlMeter.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _PnlMeter.ColumnDefinitions.Add(1, GridUnitType.Star);
            _PnlMeter.ColumnDefinitions.Add(1, GridUnitType.Star);
            _PnlMeterBg = new Box() { Row = 0, Column = 0, RowSpan = 2, ColumnSpan = 2 };
            _TfMeterName = new UI.TextField() { Row = 0, Column = 1, Text = "New Meter", IsEnabled = false };
            _TbCreateMeter = new UI.ToggleButton() { Row = 0, Column = 0, Margin = new Thickness(2, 1) }; _TbCreateMeter.Label.text = "Create Meter";
            _TbAttachMeter = new UI.ToggleButton() { Row = 1, Column = 0, ColumnSpan = 2, Margin = new Thickness(2, 0, 2, 1), IsEnabled = false }; _TbAttachMeter.Label.text = "Attached Meter"; _TbAttachMeter.Label.tooltip = "Attach created meter to picked colliders";
            _PnlMeter.Controls.Add(_PnlMeterBg);
            _PnlMeter.Controls.Add(_TfMeterName);
            _PnlMeter.Controls.Add(_TbCreateMeter);
            _PnlMeter.Controls.Add(_TbAttachMeter);

            _PnlInfo = new Grid() { Row = 3, Padding = new Thickness(2) };
            _PnlInfo.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _PnlInfo.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _PnlInfo.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) });
            _PnlInfo.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(316, GridUnitType.Pixel) });
            _PnlInfo.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            _PnlInfoBg = new Box() { Row = 0, Column = 0, RowSpan = 3, ColumnSpan = 2 };

            _LblStartPoint = new Skill.Editor.UI.LabelField() { Row = 0, Column = 0, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2) };
            _LblStartPoint.Label.text = "Start Point";

            _LblEndPoint = new Skill.Editor.UI.LabelField() { Row = 1, Column = 0, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2) };
            _LblEndPoint.Label.text = "End Point";

            _LblDistance = new Skill.Editor.UI.LabelField() { Row = 2, Column = 0, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2) };
            _LblDistance.Label.text = "Distance";

            _PnlInfo.Controls.Add(_PnlInfoBg);
            _PnlInfo.Controls.Add(_LblStartPoint);
            _PnlInfo.Controls.Add(_LblEndPoint);
            _PnlInfo.Controls.Add(_LblDistance);


            _BtnEnabled = new Skill.Framework.UI.Button() { Margin = new Thickness(2), Row = 4, Column = 0 };
            _BtnEnabled.Content.text = "Enable";
            _BtnEnabled.Click += new System.EventHandler(_BtnEnabled_Click);

            _InfoBox = new UI.HelpBox() { Row = 5, Column = 0 };

            _Frame.Grid.Controls.Add(_Help);
            _Frame.Grid.Controls.Add(_Layers);
            _Frame.Grid.Controls.Add(_PnlMeter);
            _Frame.Grid.Controls.Add(_PnlInfo);
            _Frame.Grid.Controls.Add(_BtnEnabled);
            _Frame.Grid.Controls.Add(_InfoBox);

            _TbCreateMeter.Changed += _TbCreateMeter_Changed;

        }

        void _TbCreateMeter_Changed(object sender, EventArgs e)
        {
            _TbAttachMeter.IsEnabled = _TbCreateMeter.IsChecked;
            _TfMeterName.IsEnabled = _TbCreateMeter.IsChecked;
        }

        void _BtnEnabled_Click(object sender, System.EventArgs e)
        {
            _IsDistanceEnable = !_IsDistanceEnable;
            if (_IsDistanceEnable)
            {
                _BtnEnabled.Content.text = "Disable";
                _InfoBox.Message = "Hold Ctrl and Right Click";
            }
            else
            {
                _BtnEnabled.Content.text = "Enable";
                _InfoBox.Message = "";
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
                    float size = HandleUtility.GetHandleSize(hit.point);
                    colide = true;
                    Handles.color = Color.red;
                    Handles.ArrowCap(0, hit.point, Quaternion.LookRotation(hit.normal), size * 0.5f);
                    Handles.SphereCap(0, hit.point, Quaternion.LookRotation(hit.normal), size * 0.1f);
                    if (SceneView.lastActiveSceneView != null)
                        SceneView.lastActiveSceneView.Repaint();
                }

                if (colide && e.isMouse && e.type == EventType.MouseDown && e.button == 1 && e.modifiers == EventModifiers.Control)
                {
                    if (_IsStartPointPicked)
                    {
                        if (_IsEndPointPicked)
                        {
                            SetMeterStart(hit);
                        }
                        else
                        {
                            SetMeterEnd(hit);
                        }
                    }
                    else
                    {
                        SetMeterStart(hit);
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

                if (!_TbCreateMeter.IsChecked)
                {
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

        void CreateMeter()
        {
            if (string.IsNullOrEmpty(_TfMeterName.Text))
                _TfMeterName.Text = "New Meter";
            GameObject obj = new GameObject(_TfMeterName.Text);
            Undo.RegisterCreatedObjectUndo(obj, "Creaet Meter");
            _Meter = obj.AddComponent<Skill.Framework.Modules.Meter>();
        }

        Transform CreatePoint(Transform parent, Vector3 position, int index)
        {
            GameObject obj = new GameObject("Point" + index.ToString());
            obj.AddComponent<Skill.Framework.Point3D>();
            obj.transform.position = position;
            obj.transform.parent = parent;

            Undo.RegisterCreatedObjectUndo(obj, "Creaet Point3D");
            return obj.transform;
        }

        void SetMeterStart(RaycastHit hit)
        {
            _StartPoint = hit.point;
            _IsEndPointPicked = false;
            _IsStartPointPicked = true;
            if (_TbCreateMeter.IsChecked)
            {
                if (_Meter == null)
                    CreateMeter();
                if (_TbAttachMeter.IsChecked)
                    _Meter.LockStartPoint = CreatePoint(hit.collider.transform, hit.point, 0);
                else
                    _Meter.LockStartPoint = CreatePoint(_Meter.transform, hit.point, 0);
                _Meter.FreeEndPoint = hit.point;
            }
        }

        void SetMeterEnd(RaycastHit hit)
        {
            _EndPoint = hit.point;
            _IsEndPointPicked = true;
            if (_TbCreateMeter.IsChecked)
            {
                if (_Meter != null)
                {
                    if (_TbAttachMeter.IsChecked)
                        _Meter.LockEndPoint = CreatePoint(hit.collider.transform, hit.point, 1);
                    else
                        _Meter.LockEndPoint = CreatePoint(_Meter.transform, hit.point, 1);

                    _Meter = null;
                }
            }
        }
    }
}
