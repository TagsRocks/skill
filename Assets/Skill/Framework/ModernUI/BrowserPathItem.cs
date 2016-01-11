using UnityEngine;
using System.Collections;
namespace Skill.Framework.ModernUI
{
    public class BrowserPathItem : MonoBehaviour
    {
        public UnityEngine.UI.Text Text;
        public UnityEngine.UI.Image Background;
        public Color EvenBackColor = Color.white;
        public Color OddBackColor = Color.white;

        public string Path { get { return Text.text; } set { Text.text = value; } }
        public FileBrowser Browser { get; set; }

        private int _Index;

        public int Index
        {
            get { return _Index; }
            set
            {
                _Index = value;
                if (Background != null)
                {
                    if (_Index % 2 == 0)
                        Background.color = EvenBackColor;
                    else
                        Background.color = OddBackColor;
                }
            }
        }

        public void Click()
        {
            if (Browser != null)
                Browser.PathButtonClick(this);
        }
    }
}