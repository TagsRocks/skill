using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Editor
{
    /// <summary>
    /// Defines base class for all editor tabs
    /// </summary>
    public class TabDocument : AvalonDock.DocumentContent
    {
        public TabDocument()
        {
            History = new UnDoRedo();
        }
        /// <summary> Take care of undo and redo </summary>
        public UnDoRedo History { get; private set; }

        /// <summary> Unload content </summary>
        public virtual void UnLoad() { }
        /// <summary> content filename </summary>
        public virtual string FileName { get { return ""; } }
        /// <summary> Save content </summary>
        public virtual void Save() { }
        /// <summary> Whether is changed or not </summary>
        public virtual bool IsChanged { get { return true; } }
        /// <summary> notify that filename changed elsewhere  </summary>
        /// <param name="newName">New name of file without directory</param>
        public virtual void OnChangeName(string newName) { }

        /// <summary> is cut possible now</summary>
        public virtual bool CanCut { get { return false; } }
        /// <summary> is copy possible now</summary>
        public virtual bool CanCopy { get { return false; } }
        /// <summary> is paste possible now </summary>
        public virtual bool CanPaste { get { return false; } }
        /// <summary> is delete possible now</summary>
        public virtual bool CanDelete { get { return false; } }

        /// <summary> Do cut </summary>
        public virtual void Cut() { }
        /// <summary> Do copy </summary>
        public virtual void Copy() { }
        /// <summary> Do paste </summary>
        public virtual void Paste() { }
        /// <summary> Do delete </summary>
        public virtual void Delete() { }
    }
}
