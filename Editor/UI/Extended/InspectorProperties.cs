using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Editor.UI.Extended
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

            _Instance._SelectedObject = p;
            UnityEditor.Selection.activeObject = _Instance;
        }
    }

    [UnityEditor.CustomEditor(typeof(InspectorProperties))]
    public class InspectorPropertiesEditor : UnityEditor.Editor
    {
        private Skill.Framework.UI.Frame _Frame;
        private Skill.Editor.UI.Extended.PropertyGrid _PropertyGrid;
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
