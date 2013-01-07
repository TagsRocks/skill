using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Xml.Linq;

namespace Skill.Editor
{
    class Exporter : UnityEditor.EditorWindow
    {
        #region Variables
        private static Vector2 Size = new Vector2(320, 100);
        private static Exporter _Instance;
        #endregion

        #region Properties
        public static Exporter Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = EditorWindow.GetWindow<Exporter>();
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
            base.minSize = new Vector2(Size.x, Size.y);

            CreateUI();
        }


        #endregion

        #region Destroy
        public void OnDestroy()
        {
            _Instance = null;
        }
        #endregion


        private Skill.Editor.UI.EditorFrame _Frame;

        private Skill.Editor.UI.ObjectField<Transform> _RootBone;
        private Skill.Editor.UI.ObjectField<UnityEngine.Animation> _Animations;

        private Skill.Editor.UI.Button _BtnCopySkeleton;
        private Skill.Editor.UI.Button _BtnCopyAimations;

        private void CreateUI()
        {
            _RootBone = new UI.ObjectField<Transform>() { Margin = new Skill.Framework.UI.Thickness(4, 2, 4, 2), VerticalAlignment = Skill.Framework.UI.VerticalAlignment.Center, Row = 0, Column = 0 };
            _RootBone.Label.text = "Root Bone";
            _RootBone.ObjectChanged += new EventHandler(_RootBone_ObjectChanged);

            _Animations = new UI.ObjectField<UnityEngine.Animation>() { Margin = new Skill.Framework.UI.Thickness(4, 2, 4, 2), VerticalAlignment = Skill.Framework.UI.VerticalAlignment.Center, Row = 3, Column = 0 };
            _Animations.Label.text = "Animations";
            _Animations.ObjectChanged += new EventHandler(_Animations_ObjectChanged);

            _BtnCopySkeleton = new UI.Button() { Margin = new Skill.Framework.UI.Thickness(4, 2, 4, 2), VerticalAlignment = Skill.Framework.UI.VerticalAlignment.Center, IsEnabled = false, Row = 1, Column = 0 };
            _BtnCopySkeleton.Content.text = "Copy Skeleton to Clipboard";
            _BtnCopySkeleton.Click += new EventHandler(_BtnCopySkeleton_Click);

            _BtnCopyAimations = new UI.Button() { Margin = new Skill.Framework.UI.Thickness(4, 2, 4, 2), VerticalAlignment = Skill.Framework.UI.VerticalAlignment.Center, IsEnabled = false, Row = 4, Column = 0 };
            _BtnCopyAimations.Content.text = "Copy Animations to Clipboard";
            _BtnCopyAimations.Click += new EventHandler(_BtnCopyAimations_Click);

            _Frame = new UI.EditorFrame(this);

            _Frame.Grid.Margin = new Skill.Framework.UI.Thickness(0, 10, 0, 0);
            _Frame.Grid.RowDefinitions.Add(new Skill.Framework.UI.RowDefinition() { Height = new Skill.Framework.UI.GridLength(20, Skill.Framework.UI.GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new Skill.Framework.UI.RowDefinition() { Height = new Skill.Framework.UI.GridLength(20, Skill.Framework.UI.GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new Skill.Framework.UI.RowDefinition() { Height = new Skill.Framework.UI.GridLength(5, Skill.Framework.UI.GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new Skill.Framework.UI.RowDefinition() { Height = new Skill.Framework.UI.GridLength(20, Skill.Framework.UI.GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new Skill.Framework.UI.RowDefinition() { Height = new Skill.Framework.UI.GridLength(20, Skill.Framework.UI.GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new Skill.Framework.UI.RowDefinition() { Height = new Skill.Framework.UI.GridLength(1, Skill.Framework.UI.GridUnitType.Star) });

            _Frame.Grid.ColumnDefinitions.Add(new Skill.Framework.UI.ColumnDefinition() { Width = new Skill.Framework.UI.GridLength(312, Skill.Framework.UI.GridUnitType.Pixel) });
            _Frame.Grid.ColumnDefinitions.Add(new Skill.Framework.UI.ColumnDefinition() { Width = new Skill.Framework.UI.GridLength(1, Skill.Framework.UI.GridUnitType.Star) });

            _Frame.Grid.Controls.Add(_RootBone);
            _Frame.Grid.Controls.Add(_BtnCopySkeleton);
            _Frame.Grid.Controls.Add(_Animations);
            _Frame.Grid.Controls.Add(_BtnCopyAimations);
        }

        void _BtnCopyAimations_Click(object sender, EventArgs e)
        {
            CopyAnimations();
        }

        void _BtnCopySkeleton_Click(object sender, EventArgs e)
        {
            CopySkeleton();
        }

        void _Animations_ObjectChanged(object sender, EventArgs e)
        {
            _BtnCopyAimations.IsEnabled = _RootBone.Object != null && _Animations.Object != null;
        }

        void _RootBone_ObjectChanged(object sender, EventArgs e)
        {
            _BtnCopySkeleton.IsEnabled = _RootBone.Object != null;
            _BtnCopyAimations.IsEnabled = _RootBone.Object != null && _Animations.Object != null;
        }

        void OnGUI()
        {
            _Frame.OnGUI();
        }


        private void CopySkeleton()
        {
            if (_RootBone.Object == null)
            {
                Debug.LogError("Set valid Root");
            }
            else
            {
                try
                {
                    Skill.DataModels.Animation.Bone root = CreateBone(_RootBone.Object);
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
            if (_RootBone.Object == null)
            {
                Debug.LogError("Set valid Root");
            }
            else if (_Animations.Object == null)
            {
                Debug.LogError("Set valid Animation");
            }
            else
            {
                if (_Animations.Object != null)
                {
                    XDocument document = new XDocument();
                    XElement animationsElement = new XElement("Animations");
                    List<string> animNames = new List<string>();
                    foreach (AnimationState state in _Animations.Object)
                    {
                        if (animNames.Contains(state.name)) continue;
                        animNames.Add(state.name);
                        Skill.DataModels.Animation.AnimationClip clip = new DataModels.Animation.AnimationClip()
                        {
                            Name = state.name,
                            WrapMode = (DataModels.Animation.WrapMode)state.wrapMode,
                            Length = state.length
                        };


                        var xCurve = AnimationUtility.GetEditorCurve(state.clip, _RootBone.Object.name, typeof(Transform), "m_LocalPosition.x");
                        var yCurve = AnimationUtility.GetEditorCurve(state.clip, _RootBone.Object.name, typeof(Transform), "m_LocalPosition.y");
                        var zCurve = AnimationUtility.GetEditorCurve(state.clip, _RootBone.Object.name, typeof(Transform), "m_LocalPosition.z");

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
            //System.Windows.Forms.Clipboard.Clear();
            //System.Windows.Forms.Clipboard.SetText(document.ToString());
            EditorGUIUtility.systemCopyBuffer = document.ToString();
        }

    }
}
