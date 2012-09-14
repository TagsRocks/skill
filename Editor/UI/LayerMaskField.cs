using UnityEngine;
using System.Collections;
using Skill.Editor.UI;
using System;
using UnityEditor;
using System.Collections.Generic;
using Skill.UI;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a field for layer mask.
    /// </summary>
    public class LayerMaskField : EditorControl
    {
        /// <summary>
        /// Info about layer
        /// </summary>
        private class LayerInfo
        {
            /// <summary> Name of layer </summary>
            public string LayerName { get; set; }
            /// <summary> Number of bits to shift to gain layer ( 1 &lt;&lt; LayerBitIndex ) </summary>
            public int BitShift { get; set; }
        }

        private LayerInfo[] GetLayersInfo()
        {
            List<LayerInfo> LayersInfo = new List<LayerInfo>();

            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(layerName))
                {
                    LayersInfo.Add(new LayerInfo() { LayerName = layerName, BitShift = i + 1 });
                }
            }
            return LayersInfo.ToArray();
        }

        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }

        /// <summary>
        /// Occurs when Layer of LayerMaskField changed
        /// </summary>
        public event EventHandler LayersChanged;
        protected virtual void OnLayersChanged()
        {
            if (LayersChanged != null) LayersChanged(this, EventArgs.Empty);
        }

        private int _Layers;
        /// <summary>
        /// int - The Layers modified by the user.
        /// </summary>
        public int Layers
        {
            get { return _Layers; }
            set
            {
                if (_Layers != value)
                {
                    _Layers = value;
                    OnLayersChanged();
                }
            }
        }

        /// <summary>
        /// Create an instance of MaskField
        /// </summary>
        public LayerMaskField()
        {
            this.Label = new GUIContent() { text = "Layers" };
            this.Height = 16;
        }


        /// <summary>
        /// Paint LayerMaskField's content
        /// </summary>
        protected override void Paint(PaintParameters paintParams)
        {
            // get information about all available layers
            LayerInfo[] layers = GetLayersInfo();

            // because some layers are empty and we do not want to show them, 
            // convert layers to new format and remove empty layers and let user select from our format

            string[] layerNames = new string[layers.Length];
            int sequentialLayerMask = 0;
            for (int i = 0; i < layers.Length; i++)
            {
                layerNames[i] = layers[i].LayerName;

                // convert to a sequential format
                int mask = 1 << layers[i].BitShift;
                if ((_Layers & mask) != 0)
                    sequentialLayerMask |= 1 << (i + 1);
            }

            if (Style != null)
            {
                sequentialLayerMask = EditorGUI.MaskField(PaintArea, Label, sequentialLayerMask, layerNames, Style);
            }
            else
            {
                sequentialLayerMask = EditorGUI.MaskField(PaintArea, Label, sequentialLayerMask, layerNames);
            }
            // convert back from sequential format to standard format
            int result = 0;
            for (int i = 0; i < layers.Length; i++)
            {
                int mask = 1 << (i + 1);
                if ((sequentialLayerMask & mask) != 0)
                    result |= 1 << layers[i].BitShift;
            }

            Layers = result;
        }
    }
}
