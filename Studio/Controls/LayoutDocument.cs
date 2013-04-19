using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Studio.Controls
{
    public class LayoutDocument : AvalonDock.Layout.LayoutDocument
    {
        public LayoutDocument()
        {
        }

        public LayoutDocument(TabDocument document)
        {
            this.Document = document;
        }

        private TabDocument _Document;
        public TabDocument Document { get { return _Document; } private set { _Document = value; this.Content = _Document; } }
    }
}
