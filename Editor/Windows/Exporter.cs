using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Xml.Linq;

namespace Skill.Editor
{
    public class Exporter : UnityEditor.EditorWindow
    {
        #region Variables
        private static Vector2 Size = new Vector2(250, 200);
        private static Exporter _Instance;
        #endregion

        #region Properties
        public static Exporter Instance
        {
            get
            {
                if (_Instance == null)
                {
                   _Instance=  EditorWindow.GetWindow<Exporter>();
                }
                return _Instance;
            }
        }
        #endregion

        #region Constructor
        public Exporter()
        {
            hideFlags = HideFlags.DontSave;

            if (_Instance != null)
            {
                Debug.LogError("Trying to create two instances of singleton. Self destruction in 3...");
                Destroy(this);
                return;
            }

            _Instance = this;

            title = "Export to Skill Studio";
            position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
        }
        #endregion

        #region Destroy
        public void OnDestroy()
        {
            _Instance = null;
        }
        #endregion


        private UnityEngine.Object _RootBone;
        private UnityEngine.Object _Animations;

        public void OnGUI()
        {
            float btnYOffset = 22;
            float btnX = 150;
            float btnMarginRight = 18;
            float btnHeight = 22;
            float rowHeight = 45;

            EditorGUILayout.BeginVertical();

            // ****************   Copy Skeleton   ****************

            Rect rowRect = EditorGUILayout.BeginHorizontal(GUILayout.Height(rowHeight));
            _RootBone = EditorGUILayout.ObjectField("Root", _RootBone, typeof(Transform), true);

            Rect buttonRect = rowRect;
            buttonRect.y += btnYOffset;
            buttonRect.x = Mathf.Min(btnX, rowRect.width * 0.46f);
            buttonRect.width = rowRect.width - buttonRect.x - btnMarginRight;
            buttonRect.height = btnHeight;

            if (GUI.Button(buttonRect, "Copy Skeleton to Clipboard"))
            {
                CopySkeleton();
            }

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space();

            // ****************   Copy Animations   ****************            

            rowRect = EditorGUILayout.BeginHorizontal(GUILayout.Height(rowHeight));
            _Animations = EditorGUILayout.ObjectField("Animations", _Animations, typeof(UnityEngine.Animation), true);

            buttonRect = rowRect;
            buttonRect.y += btnYOffset;
            buttonRect.x = Mathf.Min(btnX, rowRect.width * 0.46f);
            buttonRect.width = rowRect.width - buttonRect.x - btnMarginRight;
            buttonRect.height = btnHeight;

            if (GUI.Button(buttonRect, "Copy Animations to Clipboard"))
            {
                CopyAnimations();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }


        private void CopySkeleton()
        {
            if (_RootBone == null)
            {
                Debug.LogError("Set valid Root");
            }
            else
            {
                try
                {
                    Skill.DataModels.Animation.Bone root = CreateBone(_RootBone as Transform);
                    XDocument document = new XDocument();
                    document.Add(root.ToXElement());
                    CopyToClipboard(document);
                    Debug.Log("Skeleton copied to clipboard");
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }

        }

        private DataModels.Animation.Bone CreateBone(UnityEngine.Transform transform)
        {
            DataModels.Animation.Bone bone = new DataModels.Animation.Bone();
            bone.Name = transform.name;
            foreach (Transform t in transform)
            {
                bone.Add(CreateBone(t));
            }
            return bone;
        }

        private void CopyAnimations()
        {
            if (_RootBone == null)
            {
                Debug.LogError("Set valid Root");
            }
            else if (_Animations == null)
            {
                Debug.LogError("Set valid Animation");
            }
            else
            {
                UnityEngine.Animation animations = _Animations as UnityEngine.Animation;
                if (animations != null)
                {
                    XDocument document = new XDocument();
                    XElement animationsElement = new XElement("Animations");
                    List<string> animNames = new List<string>();
                    foreach (AnimationState state in animations)
                    {
                        if (animNames.Contains(state.name)) continue;
                        animNames.Add(state.name);
                        Skill.DataModels.Animation.AnimationClip clip = new DataModels.Animation.AnimationClip()
                        {
                            Name = state.name,
                            WrapMode = (DataModels.Animation.WrapMode)state.wrapMode,
                            Length = state.length
                        };


                        var xCurve = AnimationUtility.GetEditorCurve(state.clip, _RootBone.name, typeof(Transform), "m_LocalPosition.x");
                        var yCurve = AnimationUtility.GetEditorCurve(state.clip, _RootBone.name, typeof(Transform), "m_LocalPosition.y");
                        var zCurve = AnimationUtility.GetEditorCurve(state.clip, _RootBone.name, typeof(Transform), "m_LocalPosition.z");

                        AddKeys(xCurve, clip.RootMotion.XKeys);
                        AddKeys(yCurve, clip.RootMotion.YKeys);
                        AddKeys(zCurve, clip.RootMotion.ZKeys);

                        animationsElement.Add(clip.ToXElement());
                    }
                    if (animNames.Count > 0)
                    {
                        document.Add(animationsElement);
                        CopyToClipboard(document);
                        Debug.Log("AnimationClips copied to clipboard");
                    }
                    else
                        Debug.LogWarning("can not find any AnimationClip");
                }
            }
        }

        private void AddKeys(AnimationCurve curve, Skill.DataModels.Animation.KeyframeCollection collection)
        {
            if (curve == null) return;
            if (curve.keys == null || curve.keys.Length == 0) return;

            for (int i = 0; i < curve.keys.Length; i++)
            {
                var k = curve.keys[i];
                collection.Add(new DataModels.Animation.Keyframe(k.time, k.value, k.inTangent, k.outTangent));
            }
        }

        private void CopyToClipboard(XDocument document)
        {
            System.Windows.Forms.Clipboard.Clear();
            System.Windows.Forms.Clipboard.SetText(document.ToString());
        }

    }
}
