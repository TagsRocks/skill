using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Skill.Framework.Effects
{
    /// <summary> control all specular materials in scene </summary>
    public class SpecularController : MonoBehaviour
    {
        public static SpecularController Instance { get; private set; }

        public Color DefaultColor;

        private List<Material> _Materials;
        private int _SpecId;

        private Color _SpecColor;
        public Color SpecularColor
        {
            get
            {
                return _SpecColor;
            }
            set
            {
                _SpecColor = value;
                SetColor(_SpecColor);
            }
        }

        void Awake()
        {
            Instance = this;
            _SpecId = Shader.PropertyToID("_SpecColor");
        }

        // Use this for initialization
        void Start()
        {

            _Materials = new List<Material>();
            Renderer[] renderers = FindObjectsOfType<Renderer>();

            for (int i = 0; i < renderers.Length; i++)
            {
                foreach (var m in renderers[i].sharedMaterials)
                {
                    if (m.HasProperty(_SpecId))
                    {
                        if (!_Materials.Contains(m))
                            _Materials.Add(m);
                    }
                }
            }
            SetColor(DefaultColor);
        }

        private void SetColor(Color color)
        {
            foreach (var m in _Materials)
                m.SetColor(_SpecId, color);
        }

        void OnDestroy()
        {
            SetColor(DefaultColor);
        }
    }
}