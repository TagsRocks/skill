using UnityEngine;
using UnityEditor;
using System.Collections;
using Skill.Framework;
using Skill.Framework.Modules;

namespace Skill.Editor.Tools
{
    [CustomEditor(typeof(Meter))]

    public class MeterEditor : UnityEditor.Editor
    {
        private Meter _Meter;
        private GUIStyle _Style;

        void OnEnable()
        {
            _Style = new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = Color.black, background = Resources.Textures.WhiteTexture },
                padding = new RectOffset(2, 2, 2, 2)
            };
            _Meter = (Meter)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Detach lock points"))
                DetachLockPoints();
            EditorGUILayout.EndVertical();
        }

        private void DetachLockPoints()
        {
            if (_Meter.LockStartPoint != null)
            {
                if (_Meter.LockStartPoint.transform.parent != _Meter.transform)
                {
                    _Meter.LockStartPoint.transform.parent = null;
                    _Meter.LockStartPoint.transform.parent = _Meter.transform;
                }
            }
            if (_Meter.LockEndPoint != null)
            {
                if (_Meter.LockEndPoint.transform.parent != _Meter.transform)
                {
                    _Meter.LockEndPoint.transform.parent = null;
                    _Meter.LockEndPoint.transform.parent = _Meter.transform;
                }
            }
        }

        void OnSceneGUI()
        {
            Undo.RecordObject(_Meter, "Change meter positions");
            float distance = _Meter.Distance;
            float scalePerPixel = distance * _Meter.PixelPerUnit;

            Vector3 position = (_Meter.StartPosition + _Meter.EndPosition) * 0.5f;

            if (_Meter.ScaleToPixels)
                Handles.Label(position, string.Format("       Distance: {0} - Scale per pixel: {1}px", distance, scalePerPixel), _Style);
            else
                Handles.Label(position, string.Format("       Distance: {0}", distance), _Style);

            _Meter.StartPosition = Handles.PositionHandle(_Meter.StartPosition, Quaternion.identity);
            _Meter.EndPosition = Handles.PositionHandle(_Meter.EndPosition, Quaternion.identity);
        }
    }
}