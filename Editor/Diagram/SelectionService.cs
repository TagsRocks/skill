using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Studio.Diagram
{
    public class SelectionService
    {
        private DiagramCanvas _DiagramCanvas;

        public event EventHandler Change;
        private void OnChange()
        {
            if (Change != null)
                Change(this, EventArgs.Empty);
        }

        public List<ISelectable> CurrentSelection { get; private set; }

        private int count;
        private void BeginCheckForChange() { count = CurrentSelection.Count; }
        private void EndCheckForChange()
        {
            if (count != CurrentSelection.Count)
            {
                OnChange();
            }
            count = CurrentSelection.Count;
        }

        public SelectionService(DiagramCanvas canvas)
        {
            this._DiagramCanvas = canvas;
            CurrentSelection = new List<ISelectable>();
        }

        public void Select(ISelectable item)
        {
            this.ClearSelectionNoChangeEvent();
            this.Add(item);
        }

        public void Add(ISelectable item)
        {
            item.IsSelected = true;
            CurrentSelection.Add(item);
            OnChange();
        }

        public void RemoveFromSelection(ISelectable item)
        {
            item.IsSelected = false;
            CurrentSelection.Remove(item);
            OnChange();
        }

        public void Clear()
        {
            BeginCheckForChange();
            ClearSelectionNoChangeEvent();
            EndCheckForChange();
        }
        public void ClearSelectionNoChangeEvent()
        {
            CurrentSelection.ForEach(item => item.IsSelected = false);
            CurrentSelection.Clear();
        }

        public void SelectAll()
        {
            ClearSelectionNoChangeEvent();
            BeginCheckForChange();
            CurrentSelection.AddRange(_DiagramCanvas.Children.OfType<ISelectable>());
            CurrentSelection.ForEach(item => item.IsSelected = true);
            EndCheckForChange();
        }
    }
}
