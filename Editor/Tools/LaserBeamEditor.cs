using UnityEngine;
using UnityEditor;
using System.Collections;
using Skill.Framework.UI;
using Skill.Editor.UI;
using Skill.Framework.Rendering;

namespace Skill.Editor.Tools
{
    /// <summary>
    /// Editor of LaserBeam 
    /// </summary>
    [CustomEditor(typeof(LaserBeam))]
    public class LaserBeamEditor : UnityEditor.Editor
    {
        private const float FrameSize = 20; // height of frame
        private LaserBeam _LaserBeam;

        private Frame _Frame;
        private Skill.Editor.UI.LayerMaskField _LayerMaskField;

        void OnEnable()
        {
            // get LaserBeam
            _LaserBeam = serializedObject.targetObject as LaserBeam;

            // create ui
            _Frame = new Frame("Frame");
            _Frame.Position = new Rect(2, 400, 300, 20);
            _Frame.Grid.Padding = new Thickness(4, 0);

            _LayerMaskField = new Skill.Editor.UI.LayerMaskField() { Row = 0, Column = 0, Layers = _LaserBeam.CollisionLayerMask };
            _LayerMaskField.Label.text = "Collision Mask";
            _LayerMaskField.LayersChanged += _LayerMaskField_LayersChanged;

            _Frame.Grid.Controls.Add(_LayerMaskField);
        }

        void _LayerMaskField_LayersChanged(object sender, System.EventArgs e)
        {
            _LaserBeam.CollisionLayerMask = _LayerMaskField.Layers;
            UnityEditor.EditorUtility.SetDirty(_LaserBeam);
        }

        /// <summary>
        /// Implement this function to make a custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _Frame.Update();
            _Frame.OnInspectorGUI(FrameSize);
        }        
    }
}