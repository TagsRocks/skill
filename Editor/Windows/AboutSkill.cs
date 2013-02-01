using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Text;

namespace Skill.Editor
{
    class AboutSkill : UnityEditor.EditorWindow
    {
        private static Vector2 Size = new Vector2(300, 100);
        private static AboutSkill _Instance;


        private Skill.Editor.UI.EditorFrame _Frame;
        private Skill.Editor.UI.LabelField _LblAbout;
        private Skill.Editor.UI.Button _BtnClose;

        public void OnGUI()
        {
            _Frame.OnGUI();
        }

        public AboutSkill()
        {
            hideFlags = HideFlags.DontSave;

            if (_Instance != null)
            {
                Debug.LogError("Trying to create two instances of singleton. Self destruction in 3...");
                Destroy(this);
                return;
            }
            _Instance = this;

            title = "About Skill";
            base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
            base.minSize = new Vector2(Size.x, Size.y);

            CreateUI();
        }

        private void CreateUI()
        {
            _Frame = new UI.EditorFrame("Frame", this);

            _Frame.Grid.RowDefinitions.Add(new Skill.Framework.UI.RowDefinition() { Height = new Skill.Framework.UI.GridLength(1, Skill.Framework.UI.GridUnitType.Star) });
            _Frame.Grid.RowDefinitions.Add(new Skill.Framework.UI.RowDefinition() { Height = new Skill.Framework.UI.GridLength(20, Skill.Framework.UI.GridUnitType.Pixel) });

            _LblAbout = new UI.LabelField() { Row = 0, Column = 0 };
            _LblAbout.Label2.text = Skill.Editor.Properties.Resources.AppDescription;

            _BtnClose = new UI.Button() { Row = 1, Column = 0 };
            _BtnClose.Content.text = "Close";
            _BtnClose.Click += new EventHandler(_BtnClose_Click);

            _Frame.Grid.Controls.Add(_LblAbout);
            _Frame.Grid.Controls.Add(_BtnClose);
        }

        void _BtnClose_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        void OnDestroy()
        {
            _Instance = null;
        }


        public static AboutSkill Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = EditorWindow.GetWindow<AboutSkill>();
                }
                return _Instance;
            }
        }
    }
}
