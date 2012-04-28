using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    public abstract class BehaviorTree
    {
        private float _LastUpdateTime;


        public Behavior Root { get; private set; }
        public Exception Exception { get; internal set; }
        public Action RunningAction { get; internal set; }
        /// <summary> to enable update time interval set this to more than zero (default is 0)</summary>
        public float UpdateTimeInterval { get; set; }

        public Skill.Controllers.IController Controller { get; private set; }
        public object UserData { get; set; }
        public BehaviorState State { get; private set; }

        protected abstract Behavior CreateTree();

        public BehaviorTree(Skill.Controllers.IController controller)
        {
            if (controller == null)
                throw new ArgumentNullException("Controller is null");
            this.Controller = controller;
            this.State = new BehaviorState(this);
            this._LastUpdateTime = 0;
            this.UpdateTimeInterval = 0;
            Root = CreateTree();
        }

        public void Update()
        {
            if (UpdateTimeInterval > 0)
            {
                if (UnityEngine.Time.time < (_LastUpdateTime + UpdateTimeInterval))
                    return;
            }
            ForceUpdate();
        }

        public void ForceUpdate()
        {
            this._LastUpdateTime = UnityEngine.Time.time;
            Root.Trace(State);
        }
    }
}
