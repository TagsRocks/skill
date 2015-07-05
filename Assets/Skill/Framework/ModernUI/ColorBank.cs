using UnityEngine;
using System.Collections;
namespace Skill.Framework.ModernUI
{
    public class ColorBank : MonoBehaviour
    {
        public ColorData[] Colors;

        [System.Serializable]
        public class ColorData
        {
            public string Name;
            public UnityEngine.UI.ColorBlock Color;
            public Material Material;
        }

        // The only instance of TextColorBank object in scene
        private static ColorBank _Instance;
        private static ColorBank GetInstance()
        {
            if (_Instance == null)
                _Instance = FindObjectOfType<ColorBank>();
            return _Instance;
        }

        /// <summary> Awake </summary>
        void Awake()
        {
            if (_Instance != null)
            {
                if (_Instance != this)
                    Destroy(this.gameObject);
            }
            else
            {
                _Instance = this;
            }
        }


#if UNITY_EDITOR
        [ContextMenu("Update all keys")]
        public void UpdateAll()
        {
            UpdateAllKeys();
        }
#endif

        public static void UpdateAllKeys()
        {
            ColorBank bank = GetInstance();
            if (bank == null)
            {
                Debug.LogWarning("Can not find any instance of ColorBank in scene");
            }
            else
            {
                ColorKey[] colorKeys = FindObjectsOfType<ColorKey>();
                foreach (var ck in colorKeys)
                    Apply(ck);
            }
        }

        public static void Apply(ColorKey ck)
        {
            ColorBank bank = GetInstance();
            if (bank == null)
            {
                Debug.LogWarning("Can not find any instance of ColorBank in scene");
            }
            else
            {
                if (ck.NameInBank == null) ck.NameInBank = string.Empty;
                if (bank.Colors != null)
                {
                    ColorData cd = FindColor(bank.Colors, ck.NameInBank);
                    if (cd != null)
                    {
                        UnityEngine.UI.Text text = ck.GetComponent<UnityEngine.UI.Text>();
                        if (text != null)
                        {
                            text.color = cd.Color.normalColor;
                            text.material = cd.Material;
                            return;
                        }

                        UnityEngine.UI.Button button = ck.GetComponent<UnityEngine.UI.Button>();
                        if (button != null)
                        {
                            button.colors = cd.Color;
                            return;
                        }

                        UnityEngine.UI.Image image = ck.GetComponent<UnityEngine.UI.Image>();
                        if (image != null)
                        {
                            image.color = cd.Color.normalColor;
                            return;
                        }

                        ToggleTextColor ttc = ck.GetComponent<ToggleTextColor>();
                        if (ttc != null)
                        {
                            ttc.On = cd.Color.normalColor;
                            ttc.Off = cd.Color.disabledColor;
                            return;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("can not find color with name : " + ck.NameInBank + "in ColorBank");
                    }
                }
            }
        }
        private static ColorData FindColor(ColorData[] colors, string name)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i].Name == name)
                    return colors[i];
            }
            return null;
        }
    }
}