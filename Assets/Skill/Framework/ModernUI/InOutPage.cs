using UnityEngine;
using System.Collections;

namespace Skill.Framework.ModernUI
{
    public class InOutPage : MenuPage
    {        
        public float InDelay = 1;
        public float OutDelay = 1;

        private TimeWatch _InTW;
        private TimeWatch _OutTW;



        public event System.EventHandler In;
        protected virtual void OnIn()
        {
            if (In != null)
                In(this, System.EventArgs.Empty);
        }
        public event System.EventHandler Out;
        protected virtual void OnOut()
        {
            if (Out != null)
                Out(this, System.EventArgs.Empty);

        }

        protected virtual void OutEnd()
        {

        }

        public override void Show()
        {            
            this.gameObject.SetActive(true);
            _InTW.Begin(InDelay);
        }

        public override void Hide()
        {
            _OutTW.Begin(OutDelay);
            OnOut();
        }


        protected override void Update()
        {
            base.Update();
            if (_InTW.IsEnabledAndOver)
            {
                _InTW.End();
                base.Show();
                OnIn();

            }
            if (_OutTW.IsEnabledAndOver)
            {
                _OutTW.End();
                OutEnd();
                base.Hide();
            }
        }

    }
}