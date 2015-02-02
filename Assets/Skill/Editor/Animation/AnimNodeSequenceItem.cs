using UnityEngine;
using System.Collections;

namespace Skill.Editor.Animation
{
    public class AnimNodeSequenceItem : AnimNodeItem
    {
        private Skill.Framework.UI.Label _LblAnimName;
        public AnimNodeSequenceItem(AnimNodeSequenceData data)
            : base(data)
        {
            _LblAnimName = AddLabel();
            _LblAnimName.Text = data.AnimationName;
        }


        [Skill.Framework.ExposeProperty(20, "Speed", "Speed at which the animation will be played back.Default is 1.0")]
        public float Speed
        {
            get
            {
                return ((AnimNodeSequenceData)Data).Speed;
            }
            set
            {
                ((AnimNodeSequenceData)Data).Speed = value;
            }
        }

        [Skill.Framework.ExposeProperty(21, "WrapMode", "WrapMode")]
        public WrapMode WrapMode
        {
            get
            {
                return ((AnimNodeSequenceData)Data).WrapMode;
            }
            set
            {
                ((AnimNodeSequenceData)Data).WrapMode = value;
            }
        }

        [Skill.Framework.ExposeProperty(22, "UseProfile", "Whether use AnimationTree profiling method? (default is true)")]
        public bool UseTreeProfile
        {
            get
            {
                return ((AnimNodeSequenceData)Data).UseTreeProfile;
            }
            set
            {
                ((AnimNodeSequenceData)Data).UseTreeProfile = value;
            }
        }

        [Skill.Framework.ExposeProperty(23, "Sync", "synchronize animations with other animations in same blendnode")]
        public bool Synchronize
        {
            get
            {
                return ((AnimNodeSequenceData)Data).Sync;
            }
            set
            {
                ((AnimNodeSequenceData)Data).Sync = value;
            }
        }


        public string AnimationName
        {
            get
            {
                return ((AnimNodeSequenceData)Data).AnimationName;
            }
            set
            {
                if (value == null) value = string.Empty;
                ((AnimNodeSequenceData)Data).AnimationName = value;
                _LblAnimName.Text = value;
                Properties.Refresh();
            }
        }

        #region IProperties members

        protected override AnimNodeItem.ItemProperties CreateProperties()
        {
            return new SequenceItemProperties(this);
        }

        protected class SequenceItemProperties : ItemProperties
        {
            AnimNodeSequenceItem _SequenceItem;
            private Skill.Editor.UI.HelpBox _LblAnimation;
            private Skill.Framework.UI.Button _BtnAnimation;
            public SequenceItemProperties(AnimNodeSequenceItem item)
                : base(item)
            {
                _SequenceItem = item;
            }

            protected override void CreateCustomFileds()
            {
                _BtnAnimation = new Framework.UI.Button() { Height = 18, Margin = new Framework.UI.Thickness(0, 2) };
                _BtnAnimation.Content.text = "Select Animation";
                Controls.Add(_BtnAnimation);

                _LblAnimation = new UI.HelpBox() { Height = 20 };
                Controls.Add(_LblAnimation);
                base.CreateCustomFileds();

                _BtnAnimation.Click += _BtnAnimation_Click;
            }

            void _BtnAnimation_Click(object sender, System.EventArgs e)
            {
                Skill.Editor.UI.EditorFrame frame = _SequenceItem.OwnerFrame as Skill.Editor.UI.EditorFrame;
                if (frame != null)
                    ((AnimationTreeEditorWindow)frame.Owner).SelectAnimation(_SequenceItem);
            }

            protected override void RefreshData()
            {
                _LblAnimation.Message = string.Format("Animation Clip : {0}", _SequenceItem.AnimationName);
                base.RefreshData();
            }
        }

        #endregion
    }
}