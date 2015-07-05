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
        public Color Color = Color.white;
        public int LayerMask = 0xffffff;
        public float Radius = 32;
        public bool UV2 = false;        
        public float Strength = 50f;
        public float Falloff = 0.8f;
        public bool ChannelR = true, ChannelG = false, ChannelB = false, ChannelA = false;
        public bool EnableBrushFlow = false;
        public float FlowAmount = 0.1f;
        public bool SeamPainting = false;
        public bool Normalize = false;
        public bool Brush = false;
    }
}