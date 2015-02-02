using UnityEngine;
using System.Collections;
namespace Skill.Editor.Animation
{

    public class AnimNodeOverrideItem : AnimNodeBlendBaseItem
    {

        public AnimNodeOverrideItem(AnimNodeOverrideData data)
            : base(data)
        {

        }

        public override void ChangeParameterName(string oldParamName, string newParamName)
        {
            if (((AnimNodeOverrideData)Data).Parameter == oldParamName)
                ((AnimNodeOverrideData)Data).Parameter = newParamName;
            base.ChangeParameterName(oldParamName, newParamName);
        }


        #region IProperties members

        protected override AnimNodeItem.ItemProperties CreateProperties() { return new OverrideItemProperties(this); }

        protected class OverrideItemProperties : ItemProperties
        {
            private OverrideBlendingParameterSelector _ParameterSelector;
            public OverrideItemProperties(AnimNodeOverrideItem item)
                : base(item)
            {
            }

            protected override void CreateCustomFileds()
            {
                AnimNodeOverrideItem item = base.Object as AnimNodeOverrideItem;                
                _ParameterSelector = new OverrideBlendingParameterSelector(item);
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

            class OverrideBlendingParameterSelector : ParameterSelector
            {
                public OverrideBlendingParameterSelector(AnimNodeOverrideItem item)
                    : base(item, AnimationTreeParameterType.Float, 1)
                {
                }

                protected override void SetParameter(int parameterIndex, string value)
                {
                    ((AnimNodeOverrideData)((AnimNodeOverrideItem)Item).Data).Parameter = value;
                }

                protected override string GetParameter(int parameterIndex)
                {
                    return ((AnimNodeOverrideData)((AnimNodeOverrideItem)Item).Data).Parameter;
                }
            }
        }

        #endregion
    }
}