using UnityEngine;
using System.Collections;
namespace Skill.Framework.ModernUI
{
    public class BrowserListItem : MonoBehaviour
    {
        public UnityEngine.UI.Toggle Checkmark;
        public UnityEngine.UI.Text Text;
        public UnityEngine.UI.Image ImageIcon;
        public UnityEngine.UI.Image Background;
        public Color EvenBackColor = Color.white;
        public Color OddBackColor = Color.white;


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

        private bool _IsDirectory;
        public bool IsDirectory
        {
            get { return _IsDirectory; }
            set
            {
                _IsDirectory = value;
                Checkmark.gameObject.SetActive(!_IsDirectory);
            }
        }
        public bool IsChecked
        {
            get { return Checkmark.isOn; }
            set
            {
                _IgnoreChange = true;
                Checkmark.isOn = value;
                _IgnoreChange = false;
            }
        }
        public string Title { get { return Text.text; } set { Text.text = value; } }
        public Sprite Icon { get { return ImageIcon.sprite; } set { ImageIcon.sprite = value; } }

        public string Path { get; set; }

        private bool _IgnoreChange;
        protected virtual void Awake()
        {
            Checkmark.onValueChanged.AddListener(CheckedChange);
        }

        private void CheckedChange(bool value)
        {
            if (_IgnoreChange) return;
            if (Browser != null && !IsDirectory)
            {
                Browser.ListItemSelected(this);
            }
        }
        public void Click()
        {
            if (Browser != null)
                Browser.ListItemClick(this);
        }
    }
}