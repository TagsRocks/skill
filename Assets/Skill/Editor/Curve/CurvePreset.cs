using UnityEngine;
using System.Collections;
using Skill.Editor.UI;
using Skill.Framework.UI;

namespace Skill.Editor.Curve
{
    class CurvePreset : EditorControl
    {
        public event System.EventHandler Click;
        void OnClick()
        {
            if (Click != null)
                Click(this, System.EventArgs.Empty);
        }
        public Keyframe[] Keys { get; private set; }

        private AnimationCurve _Curve;
        public CurvePreset(Keyframe[] keys)
        {
            this.Keys = keys;
            _Curve = new AnimationCurve(keys);
            WantsMouseEvents = true;
            Width = 40;
            Height = 26;
            Margin = new Thickness(4, 4, 0, 0);
        }

        static Color CurveColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        static Color CurveBGColor = new Color(0.337f, 0.337f, 0.337f, 1);

        protected override void Render()
        {
            UnityEditor.EditorGUIUtility.DrawCurveSwatch(RenderArea, _Curve, null, CurveColor, CurveBGColor);
        }

        protected override void MouseDownEvent(MouseClickEventArgs args)
        {
            if (args.Button == MouseButton.Left)
                OnClick();
            base.MouseDownEvent(args);
        }
    }

    class CurvePresetLibrary : UniformGrid
    {
        public CurvePreset Preset { get; private set; }

        public event System.EventHandler PresetSelected;
        private void OnPresetSelected()
        {
            if (PresetSelected != null)
                PresetSelected(this, System.EventArgs.Empty);
        }

        public CurvePresetLibrary()
        {

            CurvePreset[] presets = new CurvePreset[]
        {
            new CurvePreset(GetConstantKeys(1.0f)),
            new CurvePreset(GetLinearKeys()),
            new CurvePreset(GetLinearMirrorKeys()),
            new CurvePreset(GetEaseInKeys()),
            new CurvePreset(GetEaseInMirrorKeys()),
            new CurvePreset(GetEaseInOutKeys()),
            new CurvePreset(GetEaseInOutMirrorKeys()),
            new CurvePreset(GetEaseOutKeys()),
            new CurvePreset(GetEaseOutMirrorKeys()),                        
        };

            base.Columns = presets.Length;
            base.Rows = 1;

            for (int i = 0; i < presets.Length; i++)
            {
                presets[i].Column = i;
                this.Controls.Add(presets[i]);
                presets[i].Click += Preset_Click;
            }
        }

        void Preset_Click(object sender, System.EventArgs e)
        {
            Preset = (CurvePreset)sender;
            OnPresetSelected();
        }


        private static Keyframe[] GetConstantKeys(float value)
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, value, 0f, 0f), new Keyframe(1f, value, 0f, 0f) };
            SetSmoothEditable(ref keys);
            return keys;
        }
        private static Keyframe[] GetEaseInKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f, 0f, 0f), new Keyframe(1f, 1f, 2f, 2f) };
            SetSmoothEditable(ref keys);
            return keys;
        }
        private static Keyframe[] GetEaseInMirrorKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 1f, -2f, -2f), new Keyframe(1f, 0f, 0f, 0f) };
            SetSmoothEditable(ref keys);
            return keys;
        }
        private static Keyframe[] GetEaseInOutKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f, 0f, 0f), new Keyframe(1f, 1f, 0f, 0f) };
            SetSmoothEditable(ref keys);
            return keys;
        }
        private static Keyframe[] GetEaseInOutMirrorKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 1f, 0f, 0f), new Keyframe(1f, 0f, 0f, 0f) };
            SetSmoothEditable(ref keys);
            return keys;
        }
        private static Keyframe[] GetEaseOutKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f, 2f, 2f), new Keyframe(1f, 1f, 0f, 0f) };
            SetSmoothEditable(ref keys);
            return keys;
        }
        private static Keyframe[] GetEaseOutMirrorKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 1f, 0f, 0f), new Keyframe(1f, 0f, -2f, -2f) };
            SetSmoothEditable(ref keys);
            return keys;
        }
        private static Keyframe[] GetLinearKeys()
        {
            Keyframe[] keyframeArray = new Keyframe[] { new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, 1f, 1f, 1f) };
            for (int i = 0; i < 2; i++)
            {
                Skill.Editor.CurveUtility.SetKeyBroken(ref keyframeArray[i], false);
                Skill.Editor.CurveUtility.SetKeyTangentMode(ref keyframeArray[i], 0, Skill.Editor.CurveUtility.TangentMode.Smooth);
                Skill.Editor.CurveUtility.SetKeyTangentMode(ref keyframeArray[i], 1, Skill.Editor.CurveUtility.TangentMode.Smooth);
            }
            return keyframeArray;
        }
        private static Keyframe[] GetLinearMirrorKeys()
        {
            Keyframe[] keyframeArray = new Keyframe[] { new Keyframe(0f, 1f, -1f, -1f), new Keyframe(1f, 0f, -1f, -1f) };
            for (int i = 0; i < 2; i++)
            {
                Skill.Editor.CurveUtility.SetKeyBroken(ref keyframeArray[i], false);
                Skill.Editor.CurveUtility.SetKeyTangentMode(ref keyframeArray[i], 0, Skill.Editor.CurveUtility.TangentMode.Smooth);
                Skill.Editor.CurveUtility.SetKeyTangentMode(ref keyframeArray[i], 1, Skill.Editor.CurveUtility.TangentMode.Smooth);
            }
            return keyframeArray;
        }
        private static void SetSmoothEditable(ref Keyframe[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                Skill.Editor.CurveUtility.SetKeyBroken(ref keys[i], false);
                Skill.Editor.CurveUtility.SetKeyTangentMode(ref keys[i], 0, Skill.Editor.CurveUtility.TangentMode.Editable);
                Skill.Editor.CurveUtility.SetKeyTangentMode(ref keys[i], 1, Skill.Editor.CurveUtility.TangentMode.Editable);
            }
        }
    }

}