using UnityEngine;
using System.Collections;
namespace Skill.Framework.Audio
{
    public class TestAudioController : MonoBehaviour
    {

        public AudioController Controller;
        private Skill.Framework.UI.Frame _Frame;
        private Skill.Framework.UI.StackPanel _PnlButtons;

        void Start()
        {
            _Frame = new Skill.Framework.UI.Frame("Frame");
            _Frame.Grid.ColumnDefinitions.Add(160, Skill.Framework.UI.GridUnitType.Pixel);
            _Frame.Grid.ColumnDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star);

            _PnlButtons = new Skill.Framework.UI.StackPanel() { Column = 0 };
            _Frame.Controls.Add(_PnlButtons);

            if (Controller != null)
            {
                if (Controller.Triggers != null)
                {
                    foreach (var t in Controller.Triggers)
                    {
                        if (t != null)
                        {
                            Skill.Framework.UI.Button button = new Skill.Framework.UI.Button();
                            button.Height = 30;
                            button.Content.text = t;
                            button.Tag = t;
                            button.Margin = new Skill.Framework.UI.Thickness(2);
                            button.Click += button_Click;
                            _PnlButtons.Controls.Add(button);
                        }
                    }
                }
            }
        }

        void button_Click(object sender, System.EventArgs e)
        {
            Skill.Framework.UI.Button button = (Skill.Framework.UI.Button)sender;
            if (Controller != null)
                Controller[button.Tag].Set();
        }

        void Update()
        {
            if (Controller != null)
            {
                foreach (Skill.Framework.UI.Button button in _PnlButtons.Controls)
                    button.IsEnabled = !Controller[button.Tag].IsActive;
            }
        }

        void OnGUI()
        {
            _Frame.Position = Skill.Framework.Utility.ScreenRect;
            _Frame.OnGUI();
        }
    }
}