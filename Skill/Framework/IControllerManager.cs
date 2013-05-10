using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework
{
    public interface IControllerManager
    {
        void Register(Controller controller);
        bool UnRegister(Controller controller);
    }
}
