using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Skill.Editor
{
    class AnimationCopy : UnityEditor.EditorWindow
    {
        #region Variables
        private static Vector2 Size = new Vector2(440, 100);
        private static AnimationCopy _Instance;
        #endregion

        #region Properties
        public static AnimationCopy Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = EditorWindow.GetWindow<AnimationCopy>();
                }
                return _Instance;
            }
        }
        #endregion

        #region Constructor
        public AnimationCopy()
        {
            hideFlags = HideFlags.DontSave;

            if (_Instance != null)
            {
                Debug.LogError("Trying to create two instances of singleton. Self destruction in 3...");
                Destroy(this);
                return;
            }

            _Instance = this;

            titleContent = new GUIContent("Animation Assigner");
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


        private static float FieldWidth = 300;
        private Skill.Editor.UI.EditorFrame _Frame;

        private Skill.Framework.UI.ScrollView _DestinationsView;
        private Skill.Editor.UI.ObjectField<UnityEngine.Animation> _SourceAnimation;

        private Skill.Framework.UI.Button _BtnAdd;
        private Skill.Framework.UI.Button _BtnCopy;

        private void CreateUI()
        {
            _SourceAnimation = new UI.ObjectField<UnityEngine.Animation>() { Margin = new Skill.Framework.UI.Thickness(2), VerticalAlignment = Skill.Framework.UI.VerticalAlignment.Center, Width = FieldWidth, HorizontalAlignment = Skill.Framework.UI.HorizontalAlignment.Left, Row = 0, Column = 0 };
            _SourceAnimation.Label.text = "Source Animation";
            _SourceAnimation.ObjectChanged += new EventHandler(_SourceAnimation_ObjectChanged);

            _BtnCopy = new Framework.UI.Button() { Margin = new Skill.Framework.UI.Thickness(4, 0, 4, 2), Row = 0, Column = 1, IsEnabled = false };
            _BtnCopy.Content.text = "Copy";
            _BtnCopy.Click += new EventHandler(_BtnAssign_Click);

            _BtnAdd = new Framework.UI.Button() { Margin = new Skill.Framework.UI.Thickness(4, 6, 4, 2), Row = 1, Column = 1 };
            _BtnAdd.Content.text = "Add";
            _BtnAdd.Click += new EventHandler(_BtnAdd_Click);


            _DestinationsView = new Skill.Framework.UI.ScrollView() { Margin = new Skill.Framework.UI.Thickness(0, 6, 0, 4), Row = 1, Column = 0, RowSpan = 2, Padding = new Skill.Framework.UI.Thickness(0, 0, 16, 0) };

            _Frame = new UI.EditorFrame("Frame", this);

            _Frame.Grid.Margin = new Skill.Framework.UI.Thickness(0, 10, 0, 0);
            _Frame.Grid.RowDefinitions.Add(new Skill.Framework.UI.RowDefinition() { Height = new Skill.Framework.UI.GridLength(20, Skill.Framework.UI.GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new Skill.Framework.UI.RowDefinition() { Height = new Skill.Framework.UI.GridLength(26, Skill.Framework.UI.GridUnitType.Pixel) });
            _Frame.Grid.RowDefinitions.Add(new Skill.Framework.UI.RowDefinition() { Height = new Skill.Framework.UI.GridLength(1, Skill.Framework.UI.GridUnitType.Star) });

            _Frame.Grid.ColumnDefinitions.Add(new Skill.Framework.UI.ColumnDefinition() { Width = new Skill.Framework.UI.GridLength(320, Skill.Framework.UI.GridUnitType.Pixel) });
            _Frame.Grid.ColumnDefinitions.Add(new Skill.Framework.UI.ColumnDefinition() { Width = new Skill.Framework.UI.GridLength(120, Skill.Framework.UI.GridUnitType.Pixel) });
            _Frame.Grid.ColumnDefinitions.Add(new Skill.Framework.UI.ColumnDefinition() { Width = new Skill.Framework.UI.GridLength(1, Skill.Framework.UI.GridUnitType.Star) });

            _Frame.Grid.Controls.Add(_SourceAnimation);
            _Frame.Grid.Controls.Add(_DestinationsView);
            _Frame.Grid.Controls.Add(_BtnCopy);
            _Frame.Grid.Controls.Add(_BtnAdd);

            AddDestination();
        }

        void _SourceAnimation_ObjectChanged(object sender, EventArgs e)
        {
            _BtnCopy.IsEnabled = _SourceAnimation.Object != null;
        }

        void _BtnAssign_Click(object sender, EventArgs e)
        {
            AssignAnimations();
        }

        private void AddDestination()
        {
            float x = 2;
            float y = 2;

            if (_DestinationsView.Controls.Count > 0)
                y += _DestinationsView.Controls[_DestinationsView.Controls.Count - 1].Position.yMax;

            Skill.Editor.UI.ObjectField<UnityEngine.Animation> newField = new UI.ObjectField<UnityEngine.Animation>()
            {
                Position = new Rect(x, y, FieldWidth, 16)
            };

            newField.Label.text = "Destination " + (_DestinationsView.Controls.Count + 1);
            _DestinationsView.Controls.Add(newField);
        }

        void _BtnAdd_Click(object sender, EventArgs e)
        {
            AddDestination();
        }

        void OnGUI()
        {
            _Frame.OnGUI();
        }

        private void AssignAnimations()
        {
            UnityEngine.Animation sourceAnimations = _SourceAnimation.Object;

            if (sourceAnimations == null)
            {
                Debug.LogError("Set valid Source Animation");
                return;
            }

            bool isAllNull = true;
            for (int i = 0; i < _DestinationsView.Controls.Count; i++)
            {
                if (((Skill.Editor.UI.ObjectField<UnityEngine.Animation>)_DestinationsView.Controls[i]).Object != null)
                {
                    isAllNull = false;
                    break;
                }
            }

            if (isAllNull)
            {
                Debug.LogError("Set valid Destination Animation");
                return;
            }

            for (int i = 0; i < _DestinationsView.Controls.Count; i++)
            {
                UnityEngine.Animation anim = ((Skill.Editor.UI.ObjectField<UnityEngine.Animation>)_DestinationsView.Controls[i]).Object;

                if (anim != null)
                {
                    foreach (AnimationState state in sourceAnimations)
                    {
                        AnimationState destState = anim[state.name];
                        if (destState == null)
                        {
                            anim.AddClip(state.clip, state.name);
                        }
                    }
                    EditorUtility.SetDirty(anim);
                }
            }
            Debug.Log("Animations Assigned");
        }

    }
}
