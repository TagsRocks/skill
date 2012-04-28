using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Skill.Editor
{
    public enum CopyType
    {
        AI,
        Animation
    }
    public class CopyObject
    {
        public Rect CopyArea;
        public CopyType Type { get; private set; }
        public bool IsCut { get; set; }
        public List<object> Items { get; private set; }
        public object SingleItem { get; set; }

        public CopyObject(CopyType type)
        {
            this.Type = type;
            Items = new List<object>();
        }

        public static CopyObject Instance { get; set; }
    }
}
