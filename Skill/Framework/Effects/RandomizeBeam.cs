using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Skill.Framework.Effects
{
    /// <summary>
    /// A beam that use random between min and max amplitude each frame.
    /// </summary>    
    public class RandomizeBeam : Beam
    {
        /// <summary>
        /// Update beam renderer
        /// </summary>
        /// <param name="renderer">The LineRenderer use to render beam</param>
        /// <param name="vertexCount">Number of vertex</param>
        protected override void UpdateBeamRenderer(LineRenderer renderer, int vertexCount)
        {

            // calc step between points
            Vector3 step = Direction * (Length / (vertexCount-1));
            Vector3 pos = StartPosition; // start width StartPosition
            Vector3 offsetDirection = base.OffsetDirection;// create local variable for better performance

            int sectionIndex = 0;
            int vertexIndex = 0;
            BeamSection section = Sections[sectionIndex];
            for (int i = 0; i < vertexCount; i++)
            {
                if (vertexIndex >= section.VertexCount) // go to next section
                {
                    vertexIndex = 0;
                    sectionIndex++;
                    if (sectionIndex >= Sections.Length)
                        sectionIndex = 0;
                    section = Sections[sectionIndex];
                }

                renderer.SetPosition(i, pos + (Random.Range(MinAmplitude, MaxAmplitude) * section.Scale) * offsetDirection);
                vertexIndex++;
                pos += step;// advance position
            }
        }
    }

}