using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using Skill.Editor.UI;
using Skill.Framework.UI;

namespace Skill.Editor
{
    class BehaviorTreeDebugger : UnityEditor.EditorWindow, Skill.Framework.IControllerManager
    {
        #region Constructor
        private static Vector2 Size = new Vector2(340, 200);
        private static BehaviorTreeDebugger _Instance;

        public void OnDestroy()
        {
            _Instance = null;
            Skill.Framework.Global.UnRegister(this);
        }
        public static BehaviorTreeDebugger Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = EditorWindow.GetWindow<BehaviorTreeDebugger>();
                }
                return _Instance;
            }
        }

        public BehaviorTreeDebugger()
        {
            hideFlags = HideFlags.DontSave;
            if (_Instance != null)
            {
                Debug.LogError("Trying to create two instances of singleton. Self destruction in 3...");
                Destroy(this);
                return;
            }

            _Instance = this;

            title = "BehaviorTree Debugger";
            position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = Size;
            CreateGUI();
            Skill.Framework.Global.Register(this);
        }

        private void CreateGUI()
        {

        }
        #endregion

        void OnGUI()
        {
            //_Frame.OnGUI();
        }

        public void Register(Framework.Controller controller)
        {
            throw new NotImplementedException();
        }

        public bool UnRegister(Framework.Controller controller)
        {
            throw new NotImplementedException();
        }
    }
}
