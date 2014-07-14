using UnityEngine;
using UnityEditor;
using System.Collections;
using Skill.Editor.UI;
using Skill.Framework;
using Skill.Framework.UI;

namespace Skill.Editor.Tools
{
    [CustomEditor(typeof(CRSpline3D))]
    public class CRSpline3DEditor : UnityEditor.Editor
    {
        private CRSpline3D _Path;

        private Skill.Framework.UI.Frame _Frame;
        private ChangeCheck _ChangeCheck;        
        
         
        private Button _BtnAdd;
        private Button _BtnRemove;
        private Skill.Framework.UI.ScrollView _PointsScrollView;
        private Skill.Framework.UI.SelectionGrid _GridPoints;

        private Skill.Framework.UI.Box _SelectedPointPropertiesBackground;
        private Skill.Framework.UI.StackPanel _PnlPoperties;
        private Vector3Field _VFValue;
        private FloatField _FFTime;

        private Skill.Framework.UI.Box _ToolsBackground;
        private Skill.Framework.UI.Grid _PnlTools;
        private LayerMaskField _LmGroundLayer;
        private Button _BtnGroundAll;
        private Button _BtnGroundSelected;

        private Button _BtnSetLinearTime;
        private FloatField _FFLinearTime;
        private LabelField _LblFLinearTime;


        private Vector3[] _CurvePoints;
        private Vector3[] _RenderPoints;
        private float _PreSmoothAmount;
        private bool _PreUseWorldSpace;

        private bool _SimulationStarted;
        private float _SimulationSpeed;
        private float _SimulationCurrectTime;

        void OnEnable()
        {
            _Path = serializedObject.targetObject as CRSpline3D;
            if (_Path.Keys == null || _Path.Keys.Length < 2)
                _Path.Keys = new Vector3[] { _Path.transform.position, _Path.transform.position + Vector3.forward };
            if (_Path.Times == null || _Path.Times.Length != _Path.Keys.Length)
            {
                _Path.Times = new float[_Path.Keys.Length];
                for (int i = 0; i < _Path.Keys.Length; i++)
                    _Path.Times[i] = i;                
            }

            _Path.ShowPath = false;
            _PreSmoothAmount = _Path.SmoothAmount = Mathf.Max(2, _Path.SmoothAmount);
            _PreUseWorldSpace = _Path.UseWorldSpace;
            RebuildPath();
            CreateUI();
            ValidateGrid();
            _BtnRemove.IsEnabled = _Path.Keys.Length > 2;
            _GridPoints.SelectedIndex = _Path.SelectedIndex;

            _SimulationStarted = false;
            _SimulationSpeed = 1.0f;
            _SimulationCurrectTime = 0;

            _Path.transform.hasChanged = false;
        }
        void OnDisable()
        {
            _Path.ShowPath = true;
        }
        private void CreateUI()
        {
            _ChangeCheck = new ChangeCheck();
            _ChangeCheck.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
            _ChangeCheck.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);

            _ChangeCheck.RowDefinitions.Add(24, Skill.Framework.UI.GridUnitType.Pixel);// buttons _BtnAdd/_BtnRemove
            _ChangeCheck.RowDefinitions.Add(164, Skill.Framework.UI.GridUnitType.Pixel);// _PointsScrollView
            _ChangeCheck.RowDefinitions.Add(44, Skill.Framework.UI.GridUnitType.Pixel); // _PnlPoperties
            _ChangeCheck.RowDefinitions.Add(94, Skill.Framework.UI.GridUnitType.Pixel); // _PnlTools

            _BtnAdd = new Button() { Row = 0, Column = 0 }; _BtnAdd.Content.text = "Add"; _BtnAdd.Content.tooltip = "Add new point the end of the path."; _BtnAdd.Content.image = Resources.UITextures.Plus;
            _BtnRemove = new Button() { Row = 0, Column = 1 }; _BtnRemove.Content.text = "Remove"; _BtnRemove.Content.tooltip = "Remove selected point."; _BtnRemove.Content.image = Resources.UITextures.Minus;
            _PointsScrollView = new Skill.Framework.UI.ScrollView() { Row = 1, Column = 0, ColumnSpan = 2, AlwayShowVertical = true, Padding = new Skill.Framework.UI.Thickness(0, 2) };
            _GridPoints = new Skill.Framework.UI.SelectionGrid() { XCount = 5 };
            _PointsScrollView.Controls.Add(_GridPoints);
            _PointsScrollView.RenderAreaChanged += _PointsScrollView_RenderAreaChanged;

            _SelectedPointPropertiesBackground = new Skill.Framework.UI.Box() { Row = 2, Column = 0, ColumnSpan = 2 };
            _PnlPoperties = new Skill.Framework.UI.StackPanel() { Row = 2, Column = 0, ColumnSpan = 2, Orientation = Skill.Framework.UI.Orientation.Vertical, Padding = new Skill.Framework.UI.Thickness(2) };
            _VFValue = new Vector3Field() { Label = "Position", Height = 20 };
            _FFTime = new FloatField() { Height = 16, Margin = new Skill.Framework.UI.Thickness(0, 2, 0, 4) }; _FFTime.Label.text = "Time";

            _PnlPoperties.Controls.Add(_FFTime);
            _PnlPoperties.Controls.Add(_VFValue);

            _ToolsBackground = new Skill.Framework.UI.Box() { Row = 3, Column = 0, ColumnSpan = 2, Margin = new Framework.UI.Thickness(0, 2, 0, 0) };
            _PnlTools = new Skill.Framework.UI.Grid() { Row = 3, Column = 0, ColumnSpan = 2, Padding = new Skill.Framework.UI.Thickness(2), Margin = _ToolsBackground.Margin };
            _PnlTools.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
            _PnlTools.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);
            _PnlTools.ColumnDefinitions.Add(2, Skill.Framework.UI.GridUnitType.Star);

            _PnlTools.RowDefinitions.Add(24, Framework.UI.GridUnitType.Pixel);
            _PnlTools.RowDefinitions.Add(30, Framework.UI.GridUnitType.Pixel);
            _PnlTools.RowDefinitions.Add(30, Framework.UI.GridUnitType.Pixel);

            _LmGroundLayer = new LayerMaskField() { Row = 0, Column = 0, ColumnSpan = 3, Layers = _Path.GroundLayer, Margin = new Framework.UI.Thickness(2, 1) }; _LmGroundLayer.Label.text = "Ground Layer";
            _BtnGroundSelected = new Button() { Row = 1, Column = 0, ColumnSpan = 2, Margin = new Framework.UI.Thickness(0, 0, 0, 4) }; _BtnGroundSelected.Content.text = "Ground Selected"; _BtnGroundSelected.Content.tooltip = "put selected on ground";
            _BtnGroundAll = new Button() { Row = 1, Column = 2, Margin = new Framework.UI.Thickness(0, 0, 0, 4) }; _BtnGroundAll.Content.text = "Ground All"; _BtnGroundAll.Content.tooltip = "put all points on ground";
            _PnlTools.Controls.Add(_LmGroundLayer);
            _PnlTools.Controls.Add(_BtnGroundSelected);
            _PnlTools.Controls.Add(_BtnGroundAll);

            _LblFLinearTime = new LabelField() { Row = 2, Column = 0, Margin = new Skill.Framework.UI.Thickness(0, 4) }; _LblFLinearTime.Label.text = "Time";
            _FFLinearTime = new FloatField() { Row = 2, Column = 1, Margin = new Skill.Framework.UI.Thickness(0, 4), Value = _Path.PathTime };
            _BtnSetLinearTime = new Button() { Row = 2, Column = 2, Margin = new Skill.Framework.UI.Thickness(2, 2) }; _BtnSetLinearTime.Content.text = "Set Time by Distance"; _BtnSetLinearTime.Content.tooltip = "Set time of nodes by relative distance";
            _PnlTools.Controls.Add(_LblFLinearTime);
            _PnlTools.Controls.Add(_FFLinearTime);
            _PnlTools.Controls.Add(_BtnSetLinearTime);


            _ChangeCheck.Controls.Add(_BtnAdd);
            _ChangeCheck.Controls.Add(_BtnRemove);
            _ChangeCheck.Controls.Add(_PointsScrollView);
            _ChangeCheck.Controls.Add(_SelectedPointPropertiesBackground);
            _ChangeCheck.Controls.Add(_PnlPoperties);
            _ChangeCheck.Controls.Add(_ToolsBackground);
            _ChangeCheck.Controls.Add(_PnlTools);

            _Frame = new Skill.Framework.UI.Frame("EditorFrame");
            _Frame.Grid.Controls.Add(_ChangeCheck);

            _BtnAdd.Click += _BtnAdd_Click;
            _BtnRemove.Click += _BtnRemove_Click;
            _ChangeCheck.Changed += _ChangeCheck_Changed;
            _GridPoints.SelectedChanged += _GridPoints_SelectedChanged;

            _VFValue.ValueChanged += SelectedPoint_ValueChanged;
            _LmGroundLayer.LayersChanged += _LmGroundLayer_LayersChanged;
            _BtnGroundSelected.Click += _BtnGroundSelected_Click;
            _BtnGroundAll.Click += _BtnGroundAll_Click;

            _FFLinearTime.ValueChanged += _FFLinearTime_ValueChanged;
            _BtnSetLinearTime.Click += _BtnSetLinearTime_Click;
            _FFTime.ValueChanged += SelectedPoint_ValueChanged;
        }

        void _BtnSetLinearTime_Click(object sender, System.EventArgs e)
        {
            SetTimeByDistance();
            ReloadProperties();
        }

        void _FFLinearTime_ValueChanged(object sender, System.EventArgs e)
        {
            _Path.PathTime = _FFLinearTime.Value;
            EditorUtility.SetDirty(_Path);
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
            Ray ray = new Ray() { origin = _Path.Keys[pointIndex] + new Vector3(0, 99999, 0), direction = Vector3.down };
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 999999, _Path.GroundLayer))
                _Path.Keys[pointIndex] = hit.point;
        }

        void _LmGroundLayer_LayersChanged(object sender, System.EventArgs e)
        {
            _Path.GroundLayer = _LmGroundLayer.Layers;
            EditorUtility.SetDirty(_Path);
        }

        private bool _IgnoreChanges;
        void SelectedPoint_ValueChanged(object sender, System.EventArgs e)
        {
            if (_Path.SelectedIndex >= 0 && !_IgnoreChanges)
            {
                _IgnoreChanges = true;
                int key = _Path.SelectedIndex;
                int prekey = Mathf.Max(0, _Path.SelectedIndex - 1);
                int nextkey = Mathf.Min(_Path.Keys.Length - 1, _Path.SelectedIndex + 1);

                if (prekey != key && _FFTime.Value <= _Path.Times[prekey]) _FFTime.Value = _Path.Times[prekey] + 0.0001f;
                if (nextkey != key && _FFTime.Value >= _Path.Times[nextkey]) _FFTime.Value = _Path.Times[nextkey] - 0.0001f;
                if (_FFTime.Value < 0) _FFTime.Value = 0;

                if (_FFTime.Value > _Path.PathTime)
                    _FFLinearTime.Value = _Path.PathTime = _FFTime.Value;
                else if (_Path.SelectedIndex == _Path.Keys.Length - 1 && _FFTime.Value < _Path.PathTime)
                    _FFLinearTime.Value = _Path.PathTime = _FFTime.Value;

                _Path.Times[key] = _FFTime.Value;
                _Path.Keys[key] = _VFValue.Value;
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

        private void ReloadProperties()
        {
            _IgnoreChanges = true;
            _VFValue.Value = _Path.Keys[_Path.SelectedIndex];
            _FFTime.Value = _Path.Times[_Path.SelectedIndex];
            _IgnoreChanges = false;
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
                Vector3[] prePoints = _Path.Keys;
                float[] preTimes = _Path.Times;
                _Path.Keys = new Vector3[prePoints.Length - 1];
                _Path.Times = new float[prePoints.Length - 1];
                for (int i = 0; i < index; i++)
                {
                    _Path.Keys[i] = prePoints[i];
                    _Path.Times[i] = preTimes[i];
                }
                for (int i = index; i < _Path.Keys.Length; i++)
                {
                    _Path.Keys[i] = prePoints[i + 1];
                    _Path.Times[i] = preTimes[i + 1];
                }


                ValidateGrid();
                _Path.SelectedIndex = _GridPoints.SelectedIndex = Mathf.Max(0, index - 1);
                _BtnRemove.IsEnabled = _Path.Keys.Length > 2;
            }
        }

        void _BtnAdd_Click(object sender, System.EventArgs e)
        {
            if (_Path.SelectedIndex < 0 || _Path.SelectedIndex == _Path.Keys.Length - 1)
                _Path.SelectedIndex = _Path.Keys.Length - 1;
            _Path.SelectedIndex++;
            Vector3[] preKeys = _Path.Keys;
            float[] preTimes = _Path.Times;
            _Path.Keys = new Vector3[preKeys.Length + 1];
            _Path.Times = new float[preTimes.Length + 1];
            for (int i = 0; i < _Path.SelectedIndex; i++)
            {
                _Path.Keys[i] = preKeys[i];
                _Path.Times[i] = preTimes[i];
            }
            for (int i = _Path.SelectedIndex + 1; i < _Path.Keys.Length; i++)
            {
                _Path.Keys[i] = preKeys[i - 1];
                _Path.Times[i] = preTimes[i - 1];
            }

            _Path.Keys[_Path.SelectedIndex] = _Path.Keys[_Path.SelectedIndex - 1];
            _Path.Times[_Path.SelectedIndex] = _Path.Times[_Path.SelectedIndex - 1];

            int preKey = Mathf.Max(_Path.SelectedIndex - 1, 0);
            int nextKey = Mathf.Min(_Path.SelectedIndex + 1, _Path.Length - 1);

            _Path.Keys[_Path.SelectedIndex] = (_Path.Keys[preKey] + _Path.Keys[nextKey]) * 0.5f;
            _Path.Times[_Path.SelectedIndex] = (_Path.Times[preKey] + _Path.Times[nextKey]) * 0.5f;

            ValidateGrid();
            _BtnRemove.IsEnabled = _Path.Keys.Length > 2;
            _GridPoints.SelectedIndex = _Path.SelectedIndex;
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
            _CurvePoints = CRSpline3D.GeneratorPathControlPoints(_Path.Keys);
            if (!_Path.UseWorldSpace)
            {
                for (int i = 0; i < _CurvePoints.Length; i++)
                    _CurvePoints[i] = _Path.transform.TransformPoint(_CurvePoints[i]);
            }
            _Path.transform.hasChanged = false;

            // rebuild render points
            int count = _Path.Length * _Path.SmoothAmount;
            if (_RenderPoints == null || _RenderPoints.Length != count + 1)
                _RenderPoints = new Vector3[count + 1];

            _RenderPoints[0] = CRSpline3D.Interpolate(_CurvePoints, 0);
            for (int i = 1; i <= count; i++)
            {
                float pm = (float)i / count;
                _RenderPoints[i] = CRSpline3D.Interpolate(_CurvePoints, pm);
            }
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
                    float distance = HandleUtility.DistancePointLine(_Path.Keys[i], lineStart, lineEnd);
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
                if (_RenderPoints != null)
                    Handles.DrawAAPolyLine(3, _RenderPoints);
            }

            int index = _Path.SelectedIndex;
            if (index >= 0)
            {
                bool changed = _Path.transform.hasChanged;
                Vector3 point = _Path.Keys[index];
                if (!_Path.UseWorldSpace)
                    point = _Path.transform.TransformPoint(point);
                Vector3 newPos = Handles.PositionHandle(point, Quaternion.identity);
                if (point != newPos)
                {
                    if (!_Path.UseWorldSpace)
                        newPos = _Path.transform.InverseTransformPoint(newPos);
                    _VFValue.Value = _Path.Keys[index] = newPos;
                    changed = true;
                }

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
                if (_CurvePoints != null)
                {
                    _SimulationCurrectTime += _SimulationSpeed / 120.0f;
                    if (_SimulationCurrectTime >= _Path.TimeLength)
                    {
                        StopSimulation();
                    }
                    else
                    {
                        Vector3 pos = CRSpline3D.Interpolate(_CurvePoints, _Path.ConvertToInterpolationTime(_SimulationCurrectTime));
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
                if (_Path.SelectedIndex == _Path.Times.Length - 1)
                    _SimulationCurrectTime = _Path.Times[_Path.SelectedIndex - 1];
                else
                    _SimulationCurrectTime = _Path.Times[_Path.SelectedIndex];

            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _Frame.Update();
            _Frame.OnInspectorGUI(330);

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
                _Path.Keys[i] = matrix.MultiplyPoint(_Path.Keys[i]);
        }

        private void SetTimeByDistance()
        {
            if (_Path != null)
            {                
                float maxDistance = 0;
                float[] distances = new float[_Path.Length];
                distances[0] = 0;

                float len = _Path.Length - 1;

                for (int i = 1; i < _Path.Length; i++)
                {
                    maxDistance += CalcDistance((float)(i - 1) / len, (float)i / len);
                    distances[i] = maxDistance;
                }

                for (int i = 0; i < _Path.Length; i++)
                {
                    _Path.Times[i] = (distances[i] / maxDistance) * _Path.PathTime;
                    EditorUtility.SetDirty(_Path);
                }
            }
        }

        private float CalcDistance(float startTime, float endTime, int resolution = 30)
        {
            float timeStep = (endTime - startTime) / resolution;
            float distance = 0;
            float time = startTime;
            for (int i = 0; i < resolution; i++)
            {
                distance += Vector3.Distance(CRSpline3D.Interpolate(_CurvePoints, time), CRSpline3D.Interpolate(_CurvePoints, time + timeStep));
                time += timeStep;
            }

            return distance;
        }
    }
}