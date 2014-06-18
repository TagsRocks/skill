using UnityEngine;
using System.Collections;
using Skill.Framework.UI;

namespace Skill.Editor.Curve
{
    public class CurveKeyProperties : Skill.Editor.UI.Extended.ExposeProperties
    {
        public CurveKeyProperties(CurveKey key)
            : base(key)
        {
        }

        protected override void Render()
        {
            base.Render();
            Refresh();
        }
        protected override void SetDirty()
        {
        }

    }
}