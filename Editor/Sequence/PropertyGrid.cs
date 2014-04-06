using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    /// <summary>
    /// Show IProperties objects inside a scrollview
    /// </summary>
    public class PropertyGrid : Skill.Framework.UI.Grid
    {
        private Skill.Framework.UI.ScrollView _ScrollView;
        Skill.Framework.UI.Label _Title;

        private IProperties _SelectedObject;
        /// <summary>
        /// Selected object
        /// </summary>
        public IProperties SelectedObject
        {
            get { return _SelectedObject; }
            set
            {
                if (_SelectedObject != value)
                {
                    if (_SelectedObject != null)
                    {
                        _SelectedObject.IsSelectedProperties = false;
                        _ScrollView.Controls.Remove(_SelectedObject.Properties);
                        _SelectedObject = null;
                    }
                    _SelectedObject = value;
                    if (_SelectedObject != null)
                    {
                        _SelectedObject.IsSelectedProperties = true;
                        _Title.Content.text = string.Format("Properties - ({0})", _SelectedObject.Title);
                        _ScrollView.Controls.Add(_SelectedObject.Properties);
                        _SelectedObject.Properties.Refresh();
                    }
                    else
                    {
                        _Title.Content.text = "Properties";
                    }
                    Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(this);
                    OnLayoutChanged();
                    UpdatePropertiesWidth();
                }
            }
        }

        /// <summary>
        /// Create a PropertyGrid
        /// </summary>
        public PropertyGrid()
        {
            RowDefinitions.Add(20, Skill.Framework.UI.GridUnitType.Pixel);
            RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);

            _Title = new Skill.Framework.UI.Label() { Row = 0 };
            _Title.Content.text = "Properties";
            Controls.Add(_Title);

            _ScrollView = new Skill.Framework.UI.ScrollView() { Row = 1 };
            Controls.Add(_ScrollView);
        }

        protected override void OnRenderAreaChanged()
        {
            UpdatePropertiesWidth();
            base.OnRenderAreaChanged();

        }

        private void UpdatePropertiesWidth()
        {
            if (_SelectedObject != null)
                _SelectedObject.Properties.Width = RenderArea.width - Padding.Horizontal - 2;
        }

        /// <summary>
        /// Render
        /// </summary>
        protected override void Render()
        {
            if (_Title.Style == null)
                _Title.Style = Skill.Editor.Resources.Styles.Header;
            base.Render();
        }
    }

    /// <summary>
    /// Interface for object that have properties and can be edited by user
    /// </summary>
    public interface IProperties
    {
        /// <summary> Title to show in title area of PropertyGrid </summary> 
        string Title { get; }
        /// <summary> Properties to show in PropertyGrid </summary>
        Properties Properties { get; }
        /// <summary> Is selected in PropertyGrid </summary>
        bool IsSelectedProperties { get; set; }
    }

    /// <summary>
    /// Properties panel - subclass adds its own controls to handle properties
    /// </summary>
    public abstract class Properties : StackPanel
    {

        /// <summary>
        /// Ignore changes of edit controls because we are refreshing
        /// </summary>
        protected bool IgnoreChanges { get; private set; }

        /// <summary>
        /// Object to edit it's properties
        /// </summary>
        public object Object { get; private set; }

        /// <summary>
        /// Create a Properties panel
        /// </summary>
        /// <param name="obj"> Object to edit it's properties </param>
        public Properties(object obj)
        {
            if (obj == null) throw new System.ArgumentNullException("Invalid Owner");
            this.Object = obj;
        }

        /// <summary>
        /// Refresh properties of Object do to changes outside of editor 
        /// </summary>
        public void Refresh()
        {
            IgnoreChanges = true;
            RefreshData();
            IgnoreChanges = false;
        }
        /// <summary>
        /// Refresh properties of Object do to changes outside of editor 
        /// </summary>
        protected abstract void RefreshData();
        protected abstract void SetDirty();
    }

}