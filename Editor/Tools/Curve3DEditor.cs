using UnityEngine;
using UnityEditor;
using System.Collections;
using Skill.Editor.UI;
using Skill.Framework;
using Skill.Framework.UI;


namespace Skill.Editor.Tools
{
    [CustomEditor(typeof(Curve3D))]
    public class Curve3DEditor : UnityEditor.Editor
    {
        private Curve3D _Path;

        private Skill.Framework.UI.Frame _Frame;
        private ChangeCheck _ChangeCheck;

        private Button _BtnAdd;
        private Button _BtnRemove;
        private Skill.Framework.UI.ScrollView _PointsScrollView;
        private Skill.Framework.UI.SelectionGrid _GridPoints;

        private Button _BtnSmoothCurve;
        private Button _BtnSmoothPoint;
        private Button _BtnSetLinearTime;
        private FloatField _FFLinearTime;
        private LabelField _LblFLinearTime;
        private Skill.Editor.UI.Slider _SliSmoothValue;
        private Skill.Framework.UI.Grid _PnlTools;

        private LayerMaskField _LmGroundLayer;
        private Button _BtnGroundAll;
        private Button _BtnGroundSelected;

        private Skill.Framework.UI.Box _SelectedPointPropertiesBackground;
        private Skill.Framework.UI.StackPanel _PnlPoperties;
        private FloatField _FFTime;
        private Vector3Field _VFValue;
        private Vector3Field _VFInTangent;
        private Vector3Field _VFOutTangent;


        private Vector3[] _CurvePoints;
        private float _PreSmoothAmount;
        private bool _PreUseWorldSpace;
        private AnimationCurve3D _Curve3D;

        private bool _SimulationStarted;
        private float _SimulationSpeed;
        private float _SimulationCurrectTime;

        void OnEnable()
        {
            _Path = serializedObject.targetObject as Curve3D;
            if (_Path.Keys == null || _Path.Keys.Length < 2)
                _Path.Keys = new Keyframe3D[] { new Keyframe3D(0, _Path.transform.position), new Keyframe3D(1.0f, _Path.transform.position + Vector3.forward) };
            _PreSmoothAmount = _Path.SmoothAmount = Mathf.Max(2, _Path.SmoothAmount);
            _PreUseWorldSpace = _Path.UseWorldSpace;
            _Path.ShowPath = false;
            RebuildPath();
            CreateUI();
            ValidateGrid();
            _BtnRemove.IsEnabled = _Path.Keys.Length > 2;
            _GridPoints.SelectedIndex = _Path.SelectedIndex;

            _SimulationStarted = false;
            _SimulationSpeed = 1.0f;
            _SimulationCurrectTime = 0;

        }
        void OnDisable()
        {
            _Path.ShowPath = true;
        }
        private int CheckSelectedIndexByMouse(Vector3 mousePosition)
        {
            if (Camera.current != null)
            {
                Debug.Log("Mouse : " + mousePosition.ToString());
                Ray ray = Camera.current.ScreenPointToRay(mousePosition);
                Vector3 lineStart = ray.origin;
                Vector3 lineEnd = ray.origin + (ray.direction * 2000);
                int index = -1;
                float minDistance = float.MaxValue;
                for (int i = 0; i < _Path.Keys.Length; i++)
                {
                    float distance = HandleUtility.DistancePointLine(_Path.Keys[i].Value, lineStart, lineEnd);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        index = i;
                    }
                }
                if (minDistance < 4)
                    return index;
            }
            return _Path.SelectedIndex;
        }

        private void CreateUI()
        {
            _ChangeCheck = new ChangeCheck();
            _ChangeCheck.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
            _ChangeCheck.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);

            _ChangeCheck.RowDefinitions.Add(24, Skill.Framework.UI.GridUnitType.Pixel); // _BtnAdd , _BtnRemove
            _ChangeCheck.RowDefinitions.Add(164, Skill.Framework.UI.GridUnitType.Pixel); // _PointsScrollView
            _ChangeCheck.RowDefinitions.Add(86, Skill.Framework.UI.GridUnitType.Pixel); // _PnlPoperties
            _ChangeCheck.RowDefinitions.Add(130, Skill.Framework.UI.GridUnitType.Pixel);//_PnlTools

            _BtnAdd = new Button() { Row = 0, Column = 0 }; _BtnAdd.Content.text = "Add"; _BtnAdd.Content.tooltip = "Add new point the end of the path."; _BtnAdd.Content.image = Resources.UITextures.Plus;
            _BtnRemove = new Button() { Row = 0, Column = 1 }; _BtnRemove.Content.text = "Remove"; _BtnRemove.Content.tooltip = "Remove selected point."; _BtnRemove.Content.image = Resources.UITextures.Minus;
            _PointsScrollView = new Skill.Framework.UI.ScrollView() { Row = 1, Column = 0, ColumnSpan = 2, AlwayShowVertical = true, Padding = new Skill.Framework.UI.Thickness(0, 2) };
            _GridPoints = new Skill.Framework.UI.SelectionGrid() { XCount = 5 };
            _PointsScrollView.Controls.Add(_GridPoints);
            _PointsScrollView.RenderAreaChanged += _PointsScrollView_RenderAreaChanged;

            _SelectedPointPropertiesBackground = new Skill.Framework.UI.Box() { Row = 2, Column = 0, ColumnSpan = 2 };
            _PnlPoperties = new Skill.Framework.UI.StackPanel() { Row = 2, Column = 0, ColumnSpan = 2, Orientation = Skill.Framework.UI.Orientation.Vertical, Padding = new Skill.Framework.UI.Thickness(2) };

            _FFTime = new FloatField() { Height = 16, Margin = new Skill.Framework.UI.Thickness(0, 2, 0, 4) }; _FFTime.Label.text = "Time";
            _VFValue = new Vector3Field() { Height = 20 }; _VFValue.Label.text = "Position";
            _VFInTangent = new Vector3Field() { Height = 20 }; _VFInTangent.Label.text = "InTangent";
            _VFOutTangent = new Vector3Field() { Height = 20 }; _VFOutTangent.Label.text = "OutTangent";

            _PnlPoperties.Controls.Add(_FFTime);
            _PnlPoperties.Controls.Add(_VFValue);
            _PnlPoperties.Controls.Add(_VFInTangent);
            _PnlPoperties.Controls.Add(_VFOutTangent);

            _PnlTools = new Skill.Framework.UI.Grid() { Row = 5, Column = 0, ColumnSpan = 2, Padding = new Skill.Framework.UI.Thickness(2) };
            _PnlTools.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
            _PnlTools.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
            _PnlTools.ColumnDefinitions.Add(2, Skill.Framework.UI.GridUnitType.Star);

            _PnlTools.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); // _LmGroundLayer
            _PnlTools.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); // _BtnGroundAll , _BtnGroundSelected
            _PnlTools.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); // _LblFLinearTime
            _PnlTools.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
            _PnlTools.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);

            _LmGroundLayer = new LayerMaskField() { Row = 0, Column = 0, ColumnSpan = 3, Layers = _Path.GroundLayer, Margin = new Framework.UI.Thickness(2) }; _LmGroundLayer.Label.text = "Ground Layer";
            _BtnGroundSelected = new Button() { Row = 1, Column = 0, ColumnSpan = 2, Margin = new Skill.Framework.UI.Thickness(2, 2) }; _BtnGroundSelected.Content.text = "Ground Selected"; _BtnGroundSelected.Content.tooltip = "put selected on ground";
            _BtnGroundAll = new Button() { Row = 1, Column = 2, Margin = new Skill.Framework.UI.Thickness(2, 2) }; _BtnGroundAll.Content.text = "Ground All"; _BtnGroundAll.Content.tooltip = "put all points on ground";

            _LblFLinearTime = new LabelField() { Row = 2, Column = 0, Margin = new Skill.Framework.UI.Thickness(0, 2) }; _LblFLinearTime.Label.text = "Time";
            _FFLinearTime = new FloatField() { Row = 2, Column = 1, Margin = new Skill.Framework.UI.Thickness(0, 2), Value = _Path.PathTime };
            _BtnSetLinearTime = new Button() { Row = 2, Column = 2, Margin = new Skill.Framework.UI.Thickness(2, 2) }; _BtnSetLinearTime.Content.text = "Set Time by Distance"; _BtnSetLinearTime.Content.tooltip = "Set time of nodes by relative distance";

            _SliSmoothValue = new Skill.Editor.UI.Slider() { Row = 3, Column = 0, ColumnSpan = 3, MinValue = 0.0f, MaxValue = 1.0f, Value = _Path.SmoothValue }; _SliSmoothValue.Label.text = "Smooth Value";
            _BtnSmoothCurve = new Button() { Row = 4, Column = 0, ColumnSpan = 2, Margin = new Skill.Framework.UI.Thickness(2, 2) }; _BtnSmoothCurve.Content.text = "Smooth Curve"; _BtnSmoothCurve.Content.tooltip = "Smooth the in and out tangents of the keys.";
            _BtnSmoothPoint = new Button() { Row = 4, Column = 2, Margin = new Skill.Framework.UI.Thickness(2, 2) }; _BtnSmoothPoint.Content.text = "Smooth Point"; _BtnSmoothPoint.Content.tooltip = "Smooth the in and out tangents of the selected key.";

            _PnlTools.Controls.Add(_LmGroundLayer);
            _PnlTools.Controls.Add(_BtnGroundSelected);
            _PnlTools.Controls.Add(_BtnGroundAll);
            _PnlTools.Controls.Add(_LblFLinearTime);
            _PnlTools.Controls.Add(_FFLinearTime);
            _PnlTools.Controls.Add(_BtnSetLinearTime);
            _PnlTools.Controls.Add(_SliSmoothValue);
            _PnlTools.Controls.Add(_BtnSmoothPoint);
            _PnlTools.Controls.Add(_BtnSmoothCurve);


            _ChangeCheck.Controls.Add(_BtnAdd);
            _ChangeCheck.Controls.Add(_BtnRemove);
            _ChangeCheck.Controls.Add(_PointsScrollView);
            _ChangeCheck.Controls.Add(_SelectedPointPropertiesBackground);
            _ChangeCheck.Controls.Add(_PnlPoperties);
            _ChangeCheck.Controls.Add(_PnlTools);

            _Frame = new Skill.Framework.UI.Frame("EditorFrame");
            _Frame.Grid.Controls.Add(_ChangeCheck);

            _BtnAdd.Click += _BtnAdd_Click;
            _BtnRemove.Click += _BtnRemove_Click;
            _ChangeCheck.Changed += _ChangeCheck_Changed;
            _GridPoints.SelectedChanged += _GridPoints_SelectedChanged;

            _FFTime.ValueChanged += SelectedPoint_ValueChanged;
            _VFValue.ValueChanged += SelectedPoint_ValueChanged;
            _VFInTangent.ValueChanged += SelectedPoint_ValueChanged;
            _VFOutTangent.ValueChanged += SelectedPoint_ValueChanged;

            _FFLinearTime.ValueChanged += _FFLinearTime_ValueChanged;
            _BtnSmoothPoint.Click += _BtnSmoothPoint_Click;
            _BtnSmoothCurve.Click += _BtnSmoothCurve_Click;
            _BtnSetLinearTime.Click += _BtnSetLinearTime_Click;

            _SliSmoothValue.ValueChanged += _SliSmoothValue_ValueChanged;

            _LmGroundLayer.LayersChanged += _LmGroundLayer_LayersChanged;
            _BtnGroundSelected.Click += _BtnGroundSelected_Click;
            _BtnGroundAll.Click += _BtnGroundAll_Click;
        }

        void _BtnGroundAll_Click(object sender, System.EventArgs e)
        {
            if (_Path.Keys != null)
            {
                for (int i = 0; i < _Path.Keys.Length; i++)
                    Ground(i);
                RebuildPath();
                EditorUtility.SetDirty(_Path);
            }
        }

        void _BtnGroundSelected_Click(object sender, System.EventArgs e)
        {
            if (_Path.SelectedIndex >= 0)
            {
                Ground(_Path.SelectedIndex);
                RebuildPath();
                EditorUtility.SetDirty(_Path);
            }
        }

        private void Ground(int pointIndex)
        {
            Ray ray = new Ray() { origin = _Path.Keys[pointIndex].Value + new Vector3(0, 99999, 0), direction = Vector3.down };
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 999999, _Path.GroundLayer))
                _Path.Keys[pointIndex].Value = hit.point;
        }

        void _LmGroundLayer_LayersChanged(object sender, System.EventArgs e)
        {
            _Path.GroundLayer = _LmGroundLayer.Layers;
            EditorUtility.SetDirty(_Path);
        }

        private void ReloadProperties()
        {
            _IgnoreChanges = true;
            Keyframe3D key = _Path.Keys[_Path.SelectedIndex];
            _FFTime.Value = key.Time;
            _VFValue.Value = key.Value;
            _VFInTangent.Value = key.InTangent;
            _VFOutTangent.Value = key.OutTangent;
            _IgnoreChanges = false;
        }

        void _SliSmoothValue_ValueChanged(object sender, System.EventArgs e)
        {
            _Path.SmoothValue = _SliSmoothValue.Value;
            EditorUtility.SetDirty(_Path);
        }

        void _FFLinearTime_ValueChanged(object sender, System.EventArgs e)
        {
            _Path.PathTime = _FFLinearTime.Value;
            EditorUtility.SetDirty(_Path);
        }

        void _BtnSetLinearTime_Click(object sender, System.EventArgs e)
        {
            SetTimeByDistance();
            ReloadProperties();
        }

        void _BtnSmoothCurve_Click(object sender, System.EventArgs e)
        {
            Undo.RecordObject(_Path, "Smooth Curve");
            SmoothPath();
            ReloadProperties();
        }

        void _BtnSmoothPoint_Click(object sender, System.EventArgs e)
        {
            if (_Curve3D != null && _Path.SelectedIndex >= 0)
            {
                Undo.RecordObject(_Path, string.Format("Smooth Point : {0}", _Path.SelectedIndex));
                _Curve3D.SmoothTangents(_Path.SelectedIndex, _Path.SmoothValue);
                Keyframe3D smoothedKey = _Curve3D[_Path.SelectedIndex];
                _Path.Keys[_Path.SelectedIndex] = smoothedKey;
                ReloadProperties();
                RebuildPoints();
            }
        }

        private bool _IgnoreChanges;
        void SelectedPoint_ValueChanged(object sender, System.EventArgs e)
        {
            if (_Path.SelectedIndex >= 0 && !_IgnoreChanges)
            {
                _IgnoreChanges = true;
                Keyframe3D key = _Path.Keys[_Path.SelectedIndex];
                Keyframe3D prekey = _Path.Keys[Mathf.Max(0, _Path.SelectedIndex - 1)];
                Keyframe3D nextkey = _Path.Keys[Mathf.Min(_Path.Keys.Length - 1, _Path.SelectedIndex + 1)];

                if (prekey != key && _FFTime.Value <= prekey.Time) _FFTime.Value = prekey.Time + 0.0001f;
                if (nextkey != key && _FFTime.Value >= nextkey.Time) _FFTime.Value = nextkey.Time - 0.0001f;
                if (_FFTime.Value < 0) _FFTime.Value = 0;

                if (_FFTime.Value > _Path.PathTime)
                    _FFLinearTime.Value = _Path.PathTime = _FFTime.Value;
                else if (_Path.SelectedIndex == _Path.Keys.Length - 1 && _FFTime.Value < _Path.PathTime)
                    _FFLinearTime.Value = _Path.PathTime = _FFTime.Value;

                key.Time = _FFTime.Value;
                key.Value = _VFValue.Value;
                key.InTangent = _VFInTangent.Value;
                key.OutTangent = _VFOutTangent.Value;
                _IgnoreChanges = false;
            }
        }

        void _GridPoints_SelectedChanged(object sender, System.EventArgs e)
        {
            if (_GridPoints.SelectedIndex >= 0)
            {
                _Path.SelectedIndex = _GridPoints.SelectedIndex;
                ReloadProperties();

            }
        }

        void _ChangeCheck_Changed(object sender, System.EventArgs e)
        {
            EditorUtility.SetDirty(_Path);
            RebuildPath();
        }

        void _BtnRemove_Click(object sender, System.EventArgs e)
        {
            if (_Path.SelectedIndex >= 0 && _Path.Keys.Length > 2)
            {
                int index = _Path.SelectedIndex;
                Keyframe3D[] preKeys = _Path.Keys;
                _Path.Keys = new Keyframe3D[preKeys.Length - 1];
                for (int i = 0; i < index; i++) _Path.Keys[i] = preKeys[i];
                for (int i = index; i < _Path.Keys.Length; i++) _Path.Keys[i] = preKeys[i + 1];
                ValidateGrid();
                _GridPoints.SelectedIndex = _Path.SelectedIndex = Mathf.Max(0, _Path.Length - 1);
                _BtnRemove.IsEnabled = _Path.Keys.Length > 2;
            }
        }

        void _BtnAdd_Click(object sender, System.EventArgs e)
        {
            if (_Path.SelectedIndex < 0 || _Path.SelectedIndex == _Path.Keys.Length - 1)
                _Path.SelectedIndex = _Path.Keys.Length - 1;
            _Path.SelectedIndex++;
            Keyframe3D[] preKeys = _Path.Keys;
            _Path.Keys = new Keyframe3D[preKeys.Length + 1];
            for (int i = 0; i < _Path.SelectedIndex; i++) _Path.Keys[i] = preKeys[i];
            for (int i = _Path.SelectedIndex + 1; i < _Path.Keys.Length; i++) _Path.Keys[i] = preKeys[i - 1];

            _Path.Keys[_Path.SelectedIndex] = new Keyframe3D(_Path.Keys[_Path.SelectedIndex - 1]);
            Keyframe3D preKey = _Path.Keys[Mathf.Max(_Path.SelectedIndex - 1, 0)];
            Keyframe3D nextKey = _Path.Keys[Mathf.Min(_Path.SelectedIndex + 1, _Path.Length - 1)];
            _Path.Keys[_Path.SelectedIndex].Value = (preKey.Value + nextKey.Value) * 0.5f;
            _Path.Keys[_Path.SelectedIndex].Time = (preKey.Time + nextKey.Time) * 0.5f;

            if (_Path.SelectedIndex == _Path.Length - 1) _Path.Keys[_Path.SelectedIndex].Time++;

            ValidateGrid();
            _BtnRemove.IsEnabled = _Path.Keys.Length > 2;
            _GridPoints.SelectedIndex = _Path.SelectedIndex;
            ReloadProperties();
            RebuildPath();
        }

        void _PointsScrollView_RenderAreaChanged(object sender, System.EventArgs e)
        {
            _GridPoints.Width = _PointsScrollView.RenderArea.width - _PointsScrollView.ScrollbarThickness;
            _GridPoints.XCount = Mathf.Max(5, (int)_GridPoints.Width / 50);
            ValidateGridPointsSize();
        }

        private void ValidateGridPointsSize()
        {
            Rect position = _GridPoints.Position;
            position.x = position.y = 0;
            position.height = (int)((Mathf.Max(0, _GridPoints.Items.Count - 1) / _GridPoints.XCount) + 1) * 20.0f;
            _GridPoints.Position = position;
        }

        private void ValidateGrid()
        {
            if (_GridPoints.Items.Count != _Path.Keys.Length)
            {
                for (int i = 0; i < Mathf.Min(_GridPoints.Items.Count, _Path.Keys.Length); i++)
                    _GridPoints.Items[i].Content.text = i.ToString();
                if (_GridPoints.Items.Count > _Path.Keys.Length)
                {
                    while (_GridPoints.Items.Count > _Path.Keys.Length)
                        _GridPoints.Items.Remove(_GridPoints.Items[_GridPoints.Items.Count - 1]);
                }
                else if (_Path.Keys.Length > _GridPoints.Items.Count)
                {
                    for (int i = _GridPoints.Items.Count; i < _Path.Keys.Length; i++)
                    {
                        Skill.Framework.UI.SelectionGridItem item = new Skill.Framework.UI.SelectionGridItem();
                        item.Content.text = i.ToString();
                        _GridPoints.Items.Add(item);
                    }
                }
                ValidateGridPointsSize();
            }
        }

        private void RebuildPath()
        {
            _Curve3D = new AnimationCurve3D(_Path.Keys);
            RebuildPoints();
            _Path.transform.hasChanged = false;
        }

        private void RebuildPoints()
        {
            _CurvePoints = new Vector3[_Path.Length * _Path.SmoothAmount + 1];
            float timeStep = _Path.TimeLength / (_CurvePoints.Length - 1);
            float timer = 0;
            for (int i = 0; i < _CurvePoints.Length; i++)
            {
                _CurvePoints[i] = _Curve3D.Evaluate(timer);
                timer += timeStep;
            }

            if (!_Path.UseWorldSpace)
            {
                for (int i = 0; i < _CurvePoints.Length; i++)
                    _CurvePoints[i] = _Path.transform.TransformPoint(_CurvePoints[i]);
            }
        }

        void OnSceneGUI()
        {
            //if (Event.current != null)
            //{
            //    Event e = Event.current;
            //    if (e.isMouse && e.type == EventType.mouseDown && e.button == 0)
            //        _GridPoints.SelectedIndex = CheckSelectedIndexByMouse(e.mousePosition);
            //}


            if (_CurvePoints != null && _Path != null)
            {
                Handles.color = _Path.Color;
                Handles.DrawAAPolyLine(3, _CurvePoints);
            }

            int index = _Path.SelectedIndex;
            if (index >= 0)
            {
                bool changed = _Path.transform.hasChanged;
                Keyframe3D key = _Path.Keys[index];

                Vector3 prePos = key.Value;
                if (!_Path.UseWorldSpace)
                    prePos = _Path.transform.TransformPoint(prePos);
                Vector3 newPos = Handles.PositionHandle(prePos, Quaternion.identity);
                if (key.Value != newPos)
                {
                    if (!_Path.UseWorldSpace)
                        newPos = _Path.transform.InverseTransformPoint(newPos);
                    _VFValue.Value = key.Value = newPos;
                    changed = true;
                }

                if (_Path.ShowInTangent)
                    changed |= DrawTangentHandle(key.Value, Color.magenta, "In", ref key.InTangent);
                if (_Path.ShowOutTangent)
                    changed |= DrawTangentHandle(key.Value, Color.magenta, "Out", ref key.OutTangent);

                if (changed)
                {
                    EditorUtility.SetDirty(_Path);
                    RebuildPath();
                    ReloadProperties();
                }
            }

            // ********************************* Simulation *************************************

            if (_SimulationStarted)
            {
                if (_Curve3D != null)
                {
                    _SimulationCurrectTime += _SimulationSpeed / 120.0f;
                    if (_SimulationCurrectTime >= _Path.TimeLength)
                    {
                        StopSimulation();
                    }
                    else
                    {

                        Vector3 pos;
                        if (_Path.UseWorldSpace)
                            pos = _Curve3D.Evaluate(_SimulationCurrectTime);
                        else
                            pos = _Path.transform.TransformPoint(_Curve3D.Evaluate(_SimulationCurrectTime));
                        Handles.color = Color.red;
                        Handles.SphereCap(0, pos, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 0.2f);
                        SceneView.focusedWindow.Repaint();
                    }
                }
            }

            // ********************************* GUI *************************************
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(Screen.width - 200, Screen.height - 90, 190, 120));

            GUILayout.BeginVertical();
            if (_SimulationStarted)
            {
                if (GUILayout.Button("Stop Simulation"))
                    StopSimulation();
            }
            else
            {
                if (GUILayout.Button("Start Simulation"))
                    StartSimulation();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Format("Speed : {0:F2}", _SimulationSpeed), GUILayout.MaxWidth(80));
            _SimulationSpeed = GUILayout.HorizontalSlider(_SimulationSpeed, 0.1f, 2.0f);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();


            GUILayout.EndArea();
            Handles.EndGUI();
        }

        private void StopSimulation()
        {
            _SimulationStarted = false;
        }

        private void StartSimulation()
        {
            if (_Path.SelectedIndex >= 0)
            {
                _SimulationStarted = true;
                if (_Path.SelectedIndex == _Path.Keys.Length - 1)
                    _SimulationCurrectTime = _Path.Keys[_Path.SelectedIndex - 1].Time;
                else
                    _SimulationCurrectTime = _Path.Keys[_Path.SelectedIndex].Time;
            }
        }

        private bool DrawTangentHandle(Vector3 position, Color color, string text, ref Vector3 tangent)
        {
            if (!_Path.UseWorldSpace)
                position = _Path.transform.TransformPoint(position);
            Vector3 p = position + tangent;
            float size = HandleUtility.GetHandleSize(p);
            Handles.color = color;
            Handles.DrawLine(position, p);
            Handles.Label(p + new Vector3(0, size * 0.4f, 0), text);
            Handles.SphereCap(0, p, Quaternion.identity, size * 0.2f);
            p = Handles.PositionHandle(p, Quaternion.identity);
            p -= position;
            if (p != tangent)
            {
                tangent = p;
                return true;
            }
            return false;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _Frame.Update();
            _Frame.OnInspectorGUI(404);

            if (_PreSmoothAmount != _Path.SmoothAmount || _PreUseWorldSpace != _Path.UseWorldSpace)
            {
                if (_PreUseWorldSpace != _Path.UseWorldSpace)
                    TransformPoints(_Path.UseWorldSpace ? _Path.transform.localToWorldMatrix : _Path.transform.worldToLocalMatrix);
                _PreUseWorldSpace = _Path.UseWorldSpace;
                _PreSmoothAmount = _Path.SmoothAmount = Mathf.Max(2, _Path.SmoothAmount);
                RebuildPath();
                ReloadProperties();
            }
        }


        private void TransformPoints(Matrix4x4 matrix)
        {
            for (int i = 0; i < _Path.Length; i++)
                _Path.Keys[i].Value = matrix.MultiplyPoint(_Path.Keys[i].Value);
        }

        private void SmoothPath()
        {
            if (_Curve3D != null)
            {
                for (int i = 0; i < _Curve3D.Length; i++)
                    _Curve3D.SmoothTangents(i, _Path.SmoothValue);
                _Path.Keys = _Curve3D.GetKeys();
            }
        }

        private void SetTimeByDistance()
        {
            if (_Path != null)
            {
                float totalTime = -1;
                foreach (var k in _Path.Keys)
                    k.Time = ++totalTime;

                float maxDistance = 0;
                float[] distances = new float[_Path.Keys.Length];
                distances[0] = 0;

                _Curve3D = new AnimationCurve3D(_Path.Keys);
                for (int i = 1; i < _Path.Keys.Length; i++)
                {
                    maxDistance += CalcDistance(_Curve3D, i - 1, i);
                    distances[i] = maxDistance;
                }

                for (int i = 0; i < _Path.Length; i++)
                {
                    _Path.Keys[i].Time = (distances[i] / maxDistance) * _Path.PathTime;
                    EditorUtility.SetDirty(_Path);
                }
            }
        }
        private float CalcDistance(AnimationCurve3D curve, float startTime, float endTime, int resolution = 60)
        {
            float timeStep = (endTime - startTime) / resolution;
            float distance = 0;
            float time = startTime;
            for (int i = 0; i < resolution; i++)
            {
                distance += Vector3.Distance(curve.Evaluate(time), curve.Evaluate(time + timeStep));
                time += timeStep;
            }

            return distance;
        }

    }
}