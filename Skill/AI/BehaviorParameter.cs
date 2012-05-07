using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Skill.AI
{
    /// <summary>
    /// Send custom parameter to behaviors
    /// </summary>
    public class BehaviorParameter
    {
        /// <summary>
        /// Name of parameter
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Value of parameter
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Create new instance of BehaviorParameter
        /// </summary>
        /// <param name="name">name of parameter</param>
        /// <param name="value">Value of parameter</param>
        public BehaviorParameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
    }

    /// <summary>
    /// Contains list of parameters
    /// </summary>
    public class BehaviorParameterCollection : ReadOnlyCollection<BehaviorParameter>
    {
        /// <summary>
        /// Create new instance of BehaviorParameterCollection
        /// </summary>
        /// <param name="parameters">array od parameters</param>
        public BehaviorParameterCollection(BehaviorParameter[] parameters)
            : base(parameters)
        {

        }

        /// <summary>
        /// Retrieves parameter by name
        /// </summary>
        /// <param name="name">Name of parameter</param>
        /// <returns>Parameter</returns>
        public object GetValue(string name)
        {
            foreach (var item in this)
            {
                if (item.Name == name)
                    return item.Value;
            }
            return null;
        }
    }
}
