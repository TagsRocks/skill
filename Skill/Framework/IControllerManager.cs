using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework
{
    /// <summary>
    /// Manage all controllers in game
    /// </summary>
    public interface IControllerManager
    {
        /// <summary>
        /// Notify a Controller started
        /// </summary>
        /// <param name="controller">Controller</param>
        void Register(Controller controller);
        /// <summary>
        /// Notify a Controller destroyed
        /// </summary>
        /// <param name="controller">Controller</param>
        bool UnRegister(Controller controller);
    }
}
