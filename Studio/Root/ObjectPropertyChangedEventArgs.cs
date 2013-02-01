using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Skill.Studio
{
    public class ObjectPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        public object PreviousValue { get; private set; }
        public object NewValue { get; private set; }

        public ObjectPropertyChangedEventArgs(string propertyName, object previousValue, object newValue)
            : base(propertyName)
        {
            this.PreviousValue = previousValue;
            this.NewValue = newValue;
        }
    }
}
