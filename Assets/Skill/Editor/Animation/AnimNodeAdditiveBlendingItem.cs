using UnityEngine;
using System.Collections;

namespace Skill.Editor.Animation
{
    public class AnimNodeAdditiveBlendingItem : AnimNodeBlendBaseItem
    {
        public AnimNodeAdditiveBlendingItem(AnimNodeAdditiveBlendingData data)
            : base(data)
        {

        }

        public override void ChangeParameterName(string oldParamName, string newParamName)
        {
            if (((AnimNodeAdditiveBlendingData)Data).Parameter == oldParamName)
                ((AnimNodeAdditiveBlendingData)Data).Parameter = newParamName;
            base.ChangeParameterName(oldParamName, newParamName);
        }


        #region IProperties members

        protected override AnimNodeItem.ItemProperties CreateProperties() { return new AdditiveBlendingItemProperties(this); }

        protected class AdditiveBlendingItemProperties : ItemProperties
        {
            private AdditiveBlendingParameterSelector _ParameterSelector;
            public AdditiveBlendingItemProperties(AnimNodeAdditiveBlendingItem item)
                : base(item)
            {
            }

            protected override void CreateCustomFileds()
            {
                AnimNodeAdditiveBlendingItem item = base.Object as AnimNodeAdditiveBlendingItem;
                _ParameterSelector = new AdditiveBlendingParameterSelector(item);
                Controls.Add(_ParameterSelector);

                base.CreateCustomFileds();
            }

            protected override void RefreshData()
            {
                base.RefreshData();
                AnimNodeItem item = base.Object as AnimNodeItem;
                Skill.Editor.Animation.AnimationTreeEditorWindow window = (Skill.Editor.Animation.AnimationTreeEditorWindow)((Skill.Editor.UI.EditorFrame)item.OwnerFrame).Owner;
                _ParameterSelector.Rebuild(window.Tree.Parameters);
            }

            class AdditiveBlendingParameterSelector : ParameterSelector
            {
                public AdditiveBlendingParameterSelector(AnimNodeAdditiveBlendingItem item)
                    : base(item, AnimationTreeParameterType.Float, 1)
                {
                }

                protected override void SetParameter(int parameterIndex, string value)
                {
                    ((AnimNodeAdditiveBlendingData)((AnimNodeAdditiveBlendingItem)Item).Data).Parameter = value;
                }

                protected override string GetParameter(int parameterIndex)
                {
                    return ((AnimNodeAdditiveBlendingData)((AnimNodeAdditiveBlendingItem)Item).Data).Parameter;
                }
            }
        }

        #endregion
    }
}