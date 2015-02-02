using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Skill.Framework.UI;

namespace Skill.Editor.UI
{
    public interface ISelectable
    {
        Rect RenderArea { get; }
        bool IsSelected { get; set; }
    }

    public class SelectableCollection<T> : ICollection<T> where T : ISelectable
    {
        private List<T> _SelectedItems = new List<T>();

#pragma warning disable 0067 // this warning is only inside unity (???)

        public event System.EventHandler SelectionChanged;

#pragma warning restore 0067

        private void OnSelectionChanged()
        {
            if (SelectionChanged != null) SelectionChanged(this, System.EventArgs.Empty);
        }
        public ISelectable SelectedItem
        {
            get
            {
                if (_SelectedItems.Count == 1)
                    return _SelectedItems[0];
                return null;
            }
        }

        public void Select(T item)
        {
            if (!Contains(item))
            {
                foreach (var it in _SelectedItems)
                    it.IsSelected = false;

                _SelectedItems.Clear();
                item.IsSelected = true;
                _SelectedItems.Add(item);
                OnSelectionChanged();
            }
        }

        public void Add(T item)
        {
            if (!Contains(item))
            {
                item.IsSelected = true;
                _SelectedItems.Add(item);
                OnSelectionChanged();
            }
        }

        public void Clear()
        {
            if (_SelectedItems.Count > 0)
            {
                foreach (var Key in _SelectedItems)
                    Key.IsSelected = false;
                _SelectedItems.Clear();
                OnSelectionChanged();
            }
        }

        public bool Contains(T item)
        {
            return _SelectedItems.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _SelectedItems.CopyTo(array, arrayIndex);
        }

        public int Count { get { return _SelectedItems.Count; } }

        public bool IsReadOnly { get { return false; } }

        public bool Remove(T item)
        {
            bool result = false;
            if (Contains(item))
            {
                item.IsSelected = false;
                result = _SelectedItems.Remove(item);
                OnSelectionChanged();
                return true;
            }
            return result;
        }

        public IEnumerator<T> GetEnumerator() { return ((IEnumerable<T>)_SelectedItems).GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return _SelectedItems.GetEnumerator(); }
    }

    public class MultiSelector<T> : Skill.Framework.UI.Panel where T : ISelectable
    {
        private IEnumerable<T> _SelectableHost;
        private Skill.Editor.UI.SelectableCollection<T> _Selection;
        private bool _IsMouseDown;
        private Vector2 _StartSelection;
        private Vector2 _EndSelection;
        private Vector2 _DeltaSelection;

        private List<T> _KeepKeysSelected;
        private List<T> _RemoveKeys;
        private Rect _SelectionRect;
        private bool _Shift;
        public Rect SelectionRect { get { return _SelectionRect; } }

        public MultiSelector(IEnumerable<T> host, Skill.Editor.UI.SelectableCollection<T> selection)
        {
            this._SelectableHost = host;
            this._Selection = selection;
            IsInScrollView = true;
            WantsMouseEvents = true;
            _RemoveKeys = new List<T>();
            _KeepKeysSelected = new List<T>();
        }


        /// <summary>
        /// OnMouse down
        /// </summary>
        /// <param name="args">args</param>
        protected override void OnMouseDown(MouseClickEventArgs args)
        {
            if (args.Button == MouseButton.Left)
            {
                Frame of = OwnerFrame;
                if (of != null)
                {
                    _IsMouseDown = OwnerFrame.RegisterPrecedenceEvent(this);
                    _StartSelection = _EndSelection = args.MousePosition;
                    _DeltaSelection = Vector2.zero;
                    _Shift = args.Shift;
                    _KeepKeysSelected.Clear();
                    if (!_Shift)
                    {
                        _Selection.Clear();
                    }
                    else
                    {
                        foreach (var ki in _Selection)
                            _KeepKeysSelected.Add(ki);
                    }
                    args.Handled = true;
                }
            }
            base.OnMouseDown(args);
        }

        /// <summary>
        /// Handle event
        /// </summary>
        /// <param name="e">event to handle</param>
        public override void HandleEvent(Event e)
        {
            if (_IsMouseDown && e != null)
            {
                if (e.type == EventType.MouseDrag)
                {
                    //_EndSelection = e.mousePosition + _View.ScrollPosition;

                    _DeltaSelection += e.delta;
                    _EndSelection = _StartSelection + _DeltaSelection;
                    UpdateSelection();
                    e.Use();
                }
                else if ((e.type == EventType.MouseUp || e.rawType == EventType.MouseUp) && e.button == 0)
                {
                    Frame of = OwnerFrame;
                    if (of != null)
                    {
                        of.UnregisterPrecedenceEvent(this);
                        _IsMouseDown = false;
                        _SelectionRect = new Rect();
                        e.Use();
                    }
                }
            }
            else
                base.HandleEvent(e);
        }

        private void UpdateSelection()
        {
            Rect ra = RenderArea;
            _SelectionRect = new Rect();
            _SelectionRect.xMin = Mathf.Max(ra.xMin, Mathf.Min(_StartSelection.x, _EndSelection.x));
            _SelectionRect.yMin = Mathf.Max(ra.yMin, Mathf.Min(_StartSelection.y, _EndSelection.y));

            _SelectionRect.xMax = Mathf.Min(ra.xMax, Mathf.Max(_StartSelection.x, _EndSelection.x));
            _SelectionRect.yMax = Mathf.Min(ra.yMax, Mathf.Max(_StartSelection.y, _EndSelection.y));

            // Add keys inside _SelectionRect
            foreach (T c in _SelectableHost)
            {
                if (_SelectionRect.Contains(c.RenderArea.center))
                {
                    _Selection.Add(c);
                }
            }

            // Remove keys that was inside _SelectionRect but not now
            foreach (var key in _Selection)
            {
                if (!_KeepKeysSelected.Contains(key))
                {
                    if (!_SelectionRect.Contains(key.RenderArea.center))
                    {
                        _RemoveKeys.Add(key);
                    }
                }
            }
            if (_RemoveKeys.Count > 0)
            {
                foreach (var key in _RemoveKeys)
                    _Selection.Remove(key);
                _RemoveKeys.Clear();
            }
        }

        protected override void UpdateLayout() { }
        protected override void Render()
        {
            base.Render();
            if (_IsMouseDown && _SelectionRect.width > 0 && _SelectionRect.height > 0)
            {
                GUI.Box(_SelectionRect, "", Skill.Editor.Resources.Styles.SelectedItem);
            }
        }
    }
}