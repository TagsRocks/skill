using Skill.Framework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Skill.Editor.UI;

namespace Skill.Editor
{
    [UnityEditor.CustomEditor(typeof(Skill.Framework.Dynamics.Explosion))]
    class ExplosionEditor : UnityEditor.Editor
    {

        private Skill.Framework.Dynamics.Explosion _Explosion;
        private const float FrameSize = 20; // height of frame        

        private Frame _Frame;
        private Skill.Editor.UI.LayerMaskField _LayerMaskField;

        void OnEnable()
        {
            _Explosion = serializedObject.targetObject as Skill.Framework.Dynamics.Explosion;
            // create ui
            _Frame = new Frame("Frame");
            _Frame.Position = new Rect(2, 400, 300, 20);
            _Frame.Grid.Padding = new Thickness(4, 0);

            _LayerMaskField = new Skill.Editor.UI.LayerMaskField() { Row = 0, Column = 0, Layers = _Explosion.LayerMask };
            _LayerMaskField.Label.text = "Layer Mask";
            _LayerMaskField.LayersChanged += _LayerMaskField_LayersChanged;
            _Frame.Grid.Controls.Add(_LayerMaskField);
        }

        void _LayerMaskField_LayersChanged(object sender, EventArgs e)
        {
            _Explosion.LayerMask = _LayerMaskField.Layers;
            UnityEditor.EditorUtility.SetDirty(_Explosion);
        }

        /// <summary>
        /// Implement this function to make a custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (_Explosion.UseRaycast)
            {
                _Frame.Update();
                _Frame.OnInspectorGUI(20);
            }
        }
    }
}
