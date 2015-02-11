using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using System.Reflection;
using Skill.Editor.UI;
using System.Collections.Generic;
using Skill.Editor;

namespace Skill.Editor.Curve
{
    public class CurveEditor : Grid 
    {
        private TimeLine _TimeLine;
        private TimeLineCurveView _CurveView;
        private TimeLineGrid _Grid;        

        public event System.EventHandler Changed;
        private void OnChanged()
        {
            if (Changed != null)
                Changed(this, System.EventArgs.Empty);
        }


        public override void Invalidate()
        {
            base.Invalidate();
            if (_CurveView != null)
                _CurveView.Invalidate();
        }

        public Color Background { get; set; }

        public TimeLine TimeLine { get { return _TimeLine; } }
        public CurveEditor()
        {
            if (UnityEditor.EditorGUIUtility.isProSkin)
                Background = new Color(0.15f, 0.15f, 0.15f, 1.0f);
            else
                Background = new Color(0.55f, 0.55f, 0.55f, 1.0f);

            this.Selection = new UI.SelectableCollection<CurveKey>();
            _CurveView = new TimeLineCurveView(this);
            _TimeLine = new TimeLine(_CurveView) { SelectionEnable = false };
            _TimeLine.TimeBar.ShowTimePosition = false;
            _TimeLine.TimeBar.TimeStyle = false;
            _Grid = new TimeLineGrid(_TimeLine.TimeBar) { Row = _CurveView.Row, Column = _CurveView.Column, RowSpan = _CurveView.RowSpan, ColumnSpan = _CurveView.ColumnSpan };
            _Grid.Margin = new Thickness(0, 0, 0, _TimeLine.View.ScrollbarThickness);
            _TimeLine.Controls.Add(_Grid);
            _Grid.BringToBack();
            
            this.Selection.SelectionChanged += Selection_SelectionChanged;
            this.Controls.Add(_TimeLine);
        }
        protected override void Render()
        {
            _Grid.MaxValue = _CurveView.MaxVisibleValue;
            _Grid.MinValue = _CurveView.MinVisibleValue;
            UnityEditor.EditorGUI.DrawRect(RenderArea, Background);
            base.Render();
        }
        public CurveTrack AddCurve(AnimationCurve animationCurve, Color color)
        {
            CurveTrack track = _CurveView.AddCurve(animationCurve, color);
            track.Changed += Track_Changed;
            return track;
        }

        void Track_Changed(object sender, System.EventArgs e)
        {
            OnChanged();
        }
        public void RemoveAllCurves()
        {
            foreach (var c in _CurveView.Controls)
            {
                if (c is CurveTrack)
                    ((CurveTrack)c).Changed -= Track_Changed;
            }
            _CurveView.RemoveAllCurves();
        }

        public void RemoveCurve(CurveTrack track)
        {
            if (track != null)
            {
                track.Changed -= Track_Changed;
                _CurveView.Controls.Remove(track);
            }
        }

        //#region CurveKeyMultiSelector
        //internal class CurveKeyMultiSelector : Skill.Framework.UI.Panel
        //{
        //    private TimeLineCurveView _View;
        //    private bool _IsMouseDown;
        //    private Vector2 _StartSelection;
        //    private Vector2 _EndSelection;
        //    private Vector2 _DeltaSelection;

        //    private List<CurveKey> _KeepKeysSelected;
        //    private List<CurveKey> _RemoveKeys;
        //    private Rect _SelectionRect;
        //    private bool _Shift;
        //    public Rect SelectionRect { get { return _SelectionRect; } }

        //    public CurveKeyMultiSelector(TimeLineCurveView view)
        //    {
        //        _View = view;
        //        IsInScrollView = true;
        //        WantsMouseEvents = true;
        //        _RemoveKeys = new List<CurveKey>();
        //        _KeepKeysSelected = new List<CurveKey>();
        //    }


        //    /// <summary>
        //    /// OnMouse down
        //    /// </summary>
        //    /// <param name="args">args</param>
        //    protected override void OnMouseDown(MouseClickEventArgs args)
        //    {
        //        if (args.Button == MouseButton.Left)
        //        {
        //            Frame of = OwnerFrame;
        //            if (of != null)
        //            {
        //                _IsMouseDown = OwnerFrame.RegisterPrecedenceEvent(this);
        //                _StartSelection = _EndSelection = args.MousePosition;
        //                _DeltaSelection = Vector2.zero;
        //                _Shift = args.Shift;
        //                _KeepKeysSelected.Clear();
        //                if (!_Shift)
        //                {
        //                    _View.Editor.Selection.Clear();
        //                }
        //                else
        //                {
        //                    foreach (var ki in _View.Editor.Selection)
        //                        _KeepKeysSelected.Add(ki);
        //                }
        //                args.Handled = true;
        //            }
        //        }
        //        base.OnMouseDown(args);
        //    }

        //    /// <summary>
        //    /// Handle event
        //    /// </summary>
        //    /// <param name="e">event to handle</param>
        //    public override void HandleEvent(Event e)
        //    {
        //        if (_IsMouseDown && e != null)
        //        {
        //            if (e.type == EventType.MouseDrag)
        //            {
        //                //_EndSelection = e.mousePosition + _View.ScrollPosition;

        //                _DeltaSelection += e.delta;
        //                _EndSelection = _StartSelection + _DeltaSelection;
        //                UpdateSelection();
        //                e.Use();
        //            }
        //            else if ((e.type == EventType.MouseUp || e.rawType == EventType.MouseUp) && e.button == 0)
        //            {
        //                Frame of = OwnerFrame;
        //                if (of != null)
        //                {
        //                    of.UnregisterPrecedenceEvent(this);
        //                    _IsMouseDown = false;
        //                    _SelectionRect = new Rect();
        //                    e.Use();
        //                }
        //            }
        //        }
        //        else
        //            base.HandleEvent(e);
        //    }

        //    private void UpdateSelection()
        //    {
        //        Rect ra = RenderArea;
        //        _SelectionRect = new Rect();
        //        _SelectionRect.xMin = Mathf.Max(ra.xMin, Mathf.Min(_StartSelection.x, _EndSelection.x));
        //        _SelectionRect.yMin = Mathf.Max(ra.yMin, Mathf.Min(_StartSelection.y, _EndSelection.y));

        //        _SelectionRect.xMax = Mathf.Min(ra.xMax, Mathf.Max(_StartSelection.x, _EndSelection.x));
        //        _SelectionRect.yMax = Mathf.Min(ra.yMax, Mathf.Max(_StartSelection.y, _EndSelection.y));

        //        // Add keys inside _SelectionRect
                

        //        // Remove keys that was inside _SelectionRect but not now
        //        foreach (var key in _View.Editor.Selection)
        //        {
        //            if (!_KeepKeysSelected.Contains(key))
        //            {
        //                if (!_SelectionRect.Contains(key.RenderArea.center))
        //                {
        //                    _RemoveKeys.Add(key);
        //                }
        //            }
        //        }
        //        if (_RemoveKeys.Count > 0)
        //        {
        //            foreach (var key in _RemoveKeys)
        //                _View.Editor.Selection.Remove(key);
        //            _RemoveKeys.Clear();
        //        }
        //    }

        //    protected override void UpdateLayout() { }
        //    protected override void Render()
        //    {
        //        base.Render();
        //        if (_IsMouseDown && _SelectionRect.width > 0 && _SelectionRect.height > 0)
        //        {
        //            GUI.Box(_SelectionRect, "", Skill.Editor.Resources.Styles.SelectedItem);
        //        }
        //    }
        //}
        //#endregion

        #region Selection

        public Skill.Editor.UI.SelectableCollection<CurveKey> Selection { get; private set; }

        void Selection_SelectionChanged(object sender, System.EventArgs e)
        {
            _DragStarted = false;
            Skill.Editor.UI.ISelectable selected = this.Selection.SelectedItem;
            if (selected != null)
            {
                Skill.Editor.UI.InspectorProperties.Select((CurveKey)selected);
            }
            else
            {
                var s = Skill.Editor.UI.InspectorProperties.GetSelected();
                if (s != null && s is CurveKey)
                    Skill.Editor.UI.InspectorProperties.Select(null);
            }
        }
        internal void RemoveSelection()
        {
            if (Selection.Count > 0)
            {
                foreach (var key in Selection)
                {
                    key.Track.RegisterForRemove(key.Index);
                }
                foreach (var c in _TimeLine.View.Controls)
                {
                    if (c is CurveTrack)
                        ((CurveTrack)c).RemoveKeys();
                }
                Selection.Clear();
            }
        }

        private Vector2 _DeltaDrag;
        private bool _DragStarted;
        internal void KeyDrag(CurveKey key, Vector2 delta, bool snap)
        {
            if (Selection.Contains(key))
            {
                if (!_DragStarted)
                {
                    foreach (var item in Selection)
                    {
                        item.StartDrag.x = item.X;
                        item.StartDrag.y = item.Y;
                    }
                    _DeltaDrag = Vector3.zero;
                    _DragStarted = true;
                }
                _DeltaDrag += delta;

                delta = _DeltaDrag;
                if (snap)
                {
                    float x = key.StartDrag.x + delta.x;
                    float time = GetSnappedValue(key.Track.GetTime(x, true), _TimeLine.TimeBar.SmallStep);
                    x = key.Track.GetX(time, true);
                    delta.x = x - key.StartDrag.x - (key.Width * 0.5f);

                    float y = key.StartDrag.y + delta.y;
                    float value = GetSnappedValue(key.Track.GetValue(y, true), _Grid.SmallStep);
                    y = key.Track.GetY(value, true);
                    delta.y = y - key.StartDrag.y - (key.Height * 0.5f);
                }
                foreach (var item in Selection)
                {
                    item.X = item.StartDrag.x + delta.x;
                    item.Y = item.StartDrag.y + delta.y;
                }

                OnChanged();
            }
            else
            {
                Selection.Select(key);
            }
        }
        private static float GetSnappedValue(float value, double snap)
        {
            if (snap <= 0) return value;

            long factor = 1000000;
            long longSnapTime = (long)(snap * factor);
            if (longSnapTime <= 0) return value;

            long longTime = (long)((double)value * factor);
            long lowerBound = (longTime / longSnapTime) * longSnapTime;
            long upperBound = lowerBound + longSnapTime;

            if (upperBound - longTime < longTime - lowerBound)
                longTime = upperBound;
            else
                longTime = lowerBound;
            return Mathf.Max(0, (float)((double)longTime / factor));
        }



        #endregion

        #region Curve Edit Methods
        private void Flatten()
        {
            if (Selection.Count > 0)
            {
                foreach (CurveKey ki in Selection)
                {
                    Keyframe key = ki.Keyframe;
                    key.inTangent = 0f;
                    key.outTangent = 0f;
                    ki.Curve.MoveKey(ki.Index, key);
                    Skill.Editor.CurveUtility.UpdateTangentsFromModeSurrounding(ki.Curve, ki.Index);
                }
            }
        }

        private void SetBoth(Skill.Editor.CurveUtility.TangentMode mode)
        {
            if (Selection.Count > 0)
            {
                foreach (CurveKey ki in Selection)
                {
                    Keyframe key = ki.Keyframe;
                    Skill.Editor.CurveUtility.SetKeyBroken(ref key, false);
                    Skill.Editor.CurveUtility.SetKeyTangentMode(ref key, 1, mode);
                    Skill.Editor.CurveUtility.SetKeyTangentMode(ref key, 0, mode);
                    if (mode == Skill.Editor.CurveUtility.TangentMode.Editable)
                    {
                        float tangent = Skill.Editor.CurveUtility.CalculateSmoothTangent(key);
                        key.inTangent = tangent;
                        key.outTangent = tangent;
                    }
                    ki.Curve.MoveKey(ki.Index, key);
                    Skill.Editor.CurveUtility.UpdateTangentsFromModeSurrounding(ki.Curve, ki.Index);
                }
            }
        }

        private void SetTangent(int leftRight, Skill.Editor.CurveUtility.TangentMode mode)
        {
            if (Selection.Count > 0)
            {
                foreach (CurveKey ki in Selection)
                {
                    Keyframe key = ki.Keyframe;
                    CurveUtility.SetKeyBroken(ref key, true);
                    if (leftRight == 2)
                    {
                        CurveUtility.SetKeyTangentMode(ref key, 0, mode);
                        CurveUtility.SetKeyTangentMode(ref key, 1, mode);
                    }
                    else
                    {
                        CurveUtility.SetKeyTangentMode(ref key, leftRight, mode);
                        if (CurveUtility.GetKeyTangentMode(key, 1 - leftRight) == CurveUtility.TangentMode.Smooth)
                        {
                            CurveUtility.SetKeyTangentMode(ref key, 1 - leftRight, CurveUtility.TangentMode.Editable);
                        }
                    }
                    if ((mode == CurveUtility.TangentMode.Stepped) && ((leftRight == 0) || (leftRight == 2)))
                    {
                        key.inTangent = float.PositiveInfinity;
                    }
                    if ((mode == CurveUtility.TangentMode.Stepped) && ((leftRight == 1) || (leftRight == 2)))
                    {
                        key.outTangent = float.PositiveInfinity;
                    }
                    ki.Curve.MoveKey(ki.Index, key);
                    CurveUtility.UpdateTangentsFromModeSurrounding(ki.Curve, ki.Index);
                }
            }
        }

        public void SetBothConstant()
        {
            SetTangent(2, CurveUtility.TangentMode.Stepped);
        }

        public void SetBothEditable()
        {
            SetTangent(2, CurveUtility.TangentMode.Editable);
        }

        public void SetBothLinear()
        {
            SetTangent(2, CurveUtility.TangentMode.Linear);
        }

        public void SetBroken()
        {
            if (Selection.Count > 0)
            {
                foreach (CurveKey ki in Selection)
                {
                    Keyframe key = ki.Keyframe;
                    CurveUtility.SetKeyBroken(ref key, true);
                    if (CurveUtility.GetKeyTangentMode(key, 1) == CurveUtility.TangentMode.Smooth)
                    {
                        CurveUtility.SetKeyTangentMode(ref key, 1, CurveUtility.TangentMode.Editable);
                    }
                    if (CurveUtility.GetKeyTangentMode(key, 0) == CurveUtility.TangentMode.Smooth)
                    {
                        CurveUtility.SetKeyTangentMode(ref key, 0, CurveUtility.TangentMode.Editable);
                    }
                    ki.Curve.MoveKey(ki.Index, key);
                    CurveUtility.UpdateTangentsFromModeSurrounding(ki.Curve, ki.Index);
                }
            }
        }

        public void SetEditable()
        {
            SetBoth(CurveUtility.TangentMode.Editable);
        }

        public void SetFlat()
        {
            SetBoth(CurveUtility.TangentMode.Editable);
            Flatten();
        }

        public void SetLeftConstant()
        {
            SetTangent(0, CurveUtility.TangentMode.Stepped);
        }

        public void SetLeftEditable()
        {
            SetTangent(0, CurveUtility.TangentMode.Editable);
        }

        public void SetLeftLinear()
        {
            SetTangent(0, CurveUtility.TangentMode.Linear);
        }

        public void SetRightConstant()
        {
            SetTangent(1, CurveUtility.TangentMode.Stepped);
        }

        public void SetRightEditable()
        {
            SetTangent(1, CurveUtility.TangentMode.Editable);
        }

        public void SetRightLinear()
        {
            SetTangent(1, CurveUtility.TangentMode.Linear);
        }

        public void SetSmooth()
        {
            SetBoth(CurveUtility.TangentMode.Smooth);
        }


        public struct SelectionState
        {
            public int SelectionCount;
            public bool Auto;
            public bool FreeSmooth;
            public bool Flat;
            public bool Broken;
            public bool LeftFree;
            public bool LeftLinear;
            public bool LeftConstant;
            public bool RightFree;
            public bool RightLinear;
            public bool RightConstant;
        }

        public SelectionState GetSelectionState()
        {
            SelectionState state = new SelectionState();

            state.SelectionCount = Selection.Count;
            bool hasSelected = Selection.Count > 0;
            state.Auto = hasSelected;
            state.FreeSmooth = hasSelected;
            state.Flat = hasSelected;
            state.Broken = hasSelected;
            state.LeftFree = hasSelected;
            state.LeftLinear = hasSelected;
            state.LeftConstant = hasSelected;
            state.RightFree = hasSelected;
            state.RightLinear = hasSelected;
            state.RightConstant = hasSelected;

            foreach (CurveKey ki in Selection)
            {
                Keyframe key = ki.Keyframe;
                CurveUtility.TangentMode keyTangentMode = CurveUtility.GetKeyTangentMode(key, 0);
                CurveUtility.TangentMode mode2 = CurveUtility.GetKeyTangentMode(key, 1);
                bool keyBroken = CurveUtility.GetKeyBroken(key);
                if ((keyTangentMode != CurveUtility.TangentMode.Smooth) || (mode2 != CurveUtility.TangentMode.Smooth))
                {
                    state.Auto = false;
                }
                if ((keyBroken || (keyTangentMode != CurveUtility.TangentMode.Editable)) || (mode2 != CurveUtility.TangentMode.Editable))
                {
                    state.FreeSmooth = false;
                }
                if ((keyBroken || (keyTangentMode != CurveUtility.TangentMode.Editable)) || (((key.inTangent != 0f) || (mode2 != CurveUtility.TangentMode.Editable)) || (key.outTangent != 0f)))
                {
                    state.Flat = false;
                }
                if (!keyBroken)
                {
                    state.Broken = false;
                }
                if (!keyBroken || (keyTangentMode != CurveUtility.TangentMode.Editable))
                {
                    state.LeftFree = false;
                }
                if (!keyBroken || (keyTangentMode != CurveUtility.TangentMode.Linear))
                {
                    state.LeftLinear = false;
                }
                if (!keyBroken || (keyTangentMode != CurveUtility.TangentMode.Stepped))
                {
                    state.LeftConstant = false;
                }
                if (!keyBroken || (mode2 != CurveUtility.TangentMode.Editable))
                {
                    state.RightFree = false;
                }
                if (!keyBroken || (mode2 != CurveUtility.TangentMode.Linear))
                {
                    state.RightLinear = false;
                }
                if (!keyBroken || (mode2 != CurveUtility.TangentMode.Stepped))
                {
                    state.RightConstant = false;
                }
            }

            return state;
        }
        #endregion

        internal static EditCurveInfo[] GetCurves(object obj)
        {
            List<EditCurveInfo> curves = new List<EditCurveInfo>();

            FieldInfo[] fields = obj.GetType().GetFields();
            System.Type attType = typeof(Skill.Framework.CurveEditorAttribute);

            foreach (FieldInfo info in fields)
            {
                if (info.FieldType == typeof(AnimationCurve))
                {
                    object[] attributes = info.GetCustomAttributes(true);
                    Skill.Framework.CurveEditorAttribute curveEditorAttribute = null;
                    foreach (object att in attributes)
                    {
                        if (att.GetType() == attType)
                        {
                            curveEditorAttribute = (Skill.Framework.CurveEditorAttribute)att;
                            break;
                        }
                    }

                    if (curveEditorAttribute == null)
                        curveEditorAttribute = new Skill.Framework.CurveEditorAttribute(1.0f, 0.8f, 0.8f, info.Name);

                    EditCurveInfo ci = new EditCurveInfo(obj, info, curveEditorAttribute);
                    curves.Add(ci);
                }
            }
            return curves.ToArray();
        }
        internal class EditCurveInfo
        {
            public object Component { get; private set; }
            public Skill.Framework.CurveEditorAttribute Attribute { get; private set; }
            public FieldInfo Field { get; private set; }

            public EditCurveInfo(object component, FieldInfo field, Skill.Framework.CurveEditorAttribute attribute)
            {
                this.Component = component;
                this.Field = field;
                this.Attribute = attribute;
            }

            public AnimationCurve GetCurve()
            {
                AnimationCurve curve = Field.GetValue(Component) as AnimationCurve;
                if (curve == null)
                {
                    curve = new AnimationCurve();
                    Field.SetValue(Component, curve);
                }
                return curve;
            }

            public void SetCurve(AnimationCurve curve)
            {
                Field.SetValue(Component, curve);
            }
        }
        
    }
}





