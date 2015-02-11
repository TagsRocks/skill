using UnityEngine;
using System.Collections;

namespace Skill.Framework.Modules
{
    /// <summary> Defines data needed to paint on texture </summary>
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshFilter))]
    [ExecuteInEditMode]
    public class PaintColor : MonoBehaviour
    {

        public Texture2D Texture = null;
        public Color Paint = Color.white;
        public Color Erase = Color.black;
        public float Radius = 32;
        public bool UV2 = false;
        public bool EraseEnable = false;
        public float Strength = 50f;
        public float Falloff = 0.8f;
        public bool ChannelR = true, ChannelG = false, ChannelB = false, ChannelA = false;
        public bool EnableBrushFlow = false;
        public float FlowAmount = 0.1f;
        public bool SeamPainting = false;
        public bool Normalize = false;
    }
}