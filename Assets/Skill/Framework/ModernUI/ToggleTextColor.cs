using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Skill.Framework.ModernUI
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleTextColor : MonoBehaviour
    {
        public UnityEngine.UI.Text Label;
        public Color On = Color.white;
        public Color Off = Color.white;

        private Toggle _Toggle;

        void Awake()
        {
            _Toggle = GetComponent<Toggle>();
            _Toggle.onValueChanged.AddListener(ToggleChanged);
            UpdateColor();
        }

        void ToggleChanged(bool value)
        {
            UpdateColor();
        }

        private void UpdateColor()
        {
            if (_Toggle.isOn)
                Label.color = On;
            else
                Label.color = Off;
        }
    }
}