using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Editor.UI
{
    public class InspectorProperties : UnityEngine.ScriptableObject
    {
        private IProperties _SelectedObject;
        public IProperties SelectedObject
        {
            get
            {
                return _SelectedObject;
            }
        }

        private static InspectorProperties _Instance;
        public static void Select(IProperties p)
        {
            if (_Instance == null)
                _Instance = UnityEngine.ScriptableObject.CreateInstance<InspectorProperties>();

            if (_Instance._SelectedObject != p)
            {
                if (_Instance._SelectedObject != null)
                    _Instance._SelectedObject.IsSelectedProperties = false;
            }

            _Instance._SelectedObject = p;

            if (p != null)
                UnityEditor.Selection.activeObject = _Instance;
        }

        public static bool IsSelected(IProperties p)
        {
            if (_Instance != null)
                return _Instance._SelectedObject == p;

            return false;
        }

        public static IProperties GetSelected()
        {
            if (_Instance != null)
                return _Instance._SelectedObject;
            else return null;
        }
    }

    [UnityEditor.CustomEditor(typeof(InspectorProperties))]
    public class InspectorPropertiesEditor : UnityEditor.Editor
    {
        private Skill.Framework.UI.Frame _Frame;
        private Skill.Editor.UI.PropertyGrid _PropertyGrid;
        private InspectorProperties _Properties;

        void OnEnable()
        {
            _Properties = target as InspectorProperties;
            _Frame = new Framework.UI.Frame("Frame");
            _PropertyGrid = new PropertyGrid(false);
            _Frame.Controls.Add(_PropertyGrid);
        }

        public override void OnInspectorGUI()
        {
            _PropertyGrid.SelectedObject = _Properties.SelectedObject;
            _Frame.OnInspectorGUI(500);
            Repaint();
        }
    }
}
