using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Skill.Editor
{
    class BehaviorTreeDebugger : UnityEditor.EditorWindow
    {
        private static Vector2 Size = new Vector2(500, 400);

        private static BehaviorTreeDebugger _Instance;

        private LineDrawer _RedLine;

        public void OnDestroy()
        {
            _Instance = null;
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

            _RedLine = new LineDrawer(Color.red);
        }

        void OnGUI()
        {
            _RedLine.DrawLine(new Vector2(10, 10), new Vector2(200, 200), 5);
        }
    }
}
