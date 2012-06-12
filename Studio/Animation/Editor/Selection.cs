using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Studio.Diagram;

namespace Skill.Studio.Animation.Editor
{
    public class Selection
    {
        public event EventHandler Change;
        private void OnChange()
        {
            if (Change != null)
                Change(this, EventArgs.Empty);
        }

        public List<ISelectable> SelectedObjects { get; private set; }

        public Selection()
        {
            SelectedObjects = new List<ISelectable>();
        }

        public void Select(ISelectable item)
        {
            if (SelectedObjects.Count == 1 && SelectedObjects[0] == item) return;
            this.ClearNoChangeEvent();
            this.Add(item);
        }

        public void Add(ISelectable item)
        {
            if (item.IsSelected && SelectedObjects.Contains(item)) return;
            item.IsSelected = true;
            SelectedObjects.Add(item);
            OnChange();
        }

        public void Remove(ISelectable item)
        {
            if (item.IsSelected && SelectedObjects.Contains(item))
            {
                item.IsSelected = false;
                SelectedObjects.Remove(item);
                OnChange();
            }
        }

        public void Clear()
        {
            if (SelectedObjects.Count > 0)
            {
                ClearNoChangeEvent();
                OnChange();
            }
        }

        public void ClearNoChangeEvent()
        {
            SelectedObjects.ForEach(item => item.IsSelected = false);
            SelectedObjects.Clear();
        }
    }
}
