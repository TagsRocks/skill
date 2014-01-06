using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
namespace Skill.Framework
{
    /// <summary>
    /// Static utility class to work around lack of support for Keyframe.tangentMode
    /// This utility class mimics the functionality that happens behind the scenes in UnityEditor when you manipulate an AnimationCurve. All of this information
    /// was discovered via .net reflection, and thus relies on reflection to work
    /// --testure 09/05/2012
    /// source : http://answers.unity3d.com/questions/313276/undocumented-property-keyframetangentmode.html
    /// </summary>
    public static class AnimationCurveUtility : System.Object
    {
        public enum TangentMode
        {
            Editable,
            Smooth,
            Linear,
            Stepped
        }

        public enum TangentDirection
        {
            Left,
            Right
        }
        static FieldInfo _TangentModeField;
        static AnimationCurveUtility()
        {
            Type t = typeof(UnityEngine.Keyframe);
            _TangentModeField = t.GetField("m_TangentMode", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        }

        public static void SetLinear(ref AnimationCurve curve)
        {
            for (int i = 0; i < curve.length; i++)
            {
                object boxed = curve.keys[i]; // getting around the fact that Keyframe is a struct by pre-boxing
                _TangentModeField.SetValue(boxed, GetNewTangentKeyMode((int)_TangentModeField.GetValue(boxed), TangentDirection.Left, TangentMode.Linear));
                _TangentModeField.SetValue(boxed, GetNewTangentKeyMode((int)_TangentModeField.GetValue(boxed), TangentDirection.Right, TangentMode.Linear));
                curve.MoveKey(i, (Keyframe)boxed);
                curve.SmoothTangents(i, 0f);
            }
        }

        public static void Set(ref Keyframe key, TangentMode leftMode, TangentMode rightMode)
        {
            object boxed = key; // getting around the fact that Keyframe is a struct by pre-boxing
            _TangentModeField.SetValue(boxed, GetNewTangentKeyMode((int)_TangentModeField.GetValue(boxed), TangentDirection.Left, leftMode));
            _TangentModeField.SetValue(boxed, GetNewTangentKeyMode((int)_TangentModeField.GetValue(boxed), TangentDirection.Right, rightMode));
        }

        public static int GetNewTangentKeyMode(int currentTangentMode, TangentDirection leftRight, TangentMode mode)
        {
            int output = currentTangentMode;

            if (leftRight == TangentDirection.Left)
            {
                output &= -7;
                output |= ((int)mode) << 1;
            }
            else
            {
                output &= -25;
                output |= ((int)mode) << 3;
            }
            return output;
        }

    }
}