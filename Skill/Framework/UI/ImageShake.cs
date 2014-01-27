using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Helper class to simulate shake ImageWithTexCoords
    /// </summary>
    public class ImageShake : DynamicBehaviour
    {
        class ShakeData
        {
            public bool SaveTexCoords;
            public Rect InitialTexCoords;
            public ImageWithTexCoords Image;
            public Vector2 Amount;
            public TimeWatch Timer;
            public float Fadeout;
        }

        private List<ShakeData> _Shakes;
        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _Shakes = new List<ShakeData>();
        }

        /// <summary>
        /// LateUpdate
        /// </summary>
        protected virtual void LateUpdate()
        {
            if (Global.IsGamePaused) return;

            if (_Shakes.Count > 0)
            {
                int index = 0;
                while (index < _Shakes.Count)
                {
                    ShakeData sd = _Shakes[index];
                    float percent = sd.Timer.Percent;
                    if (percent >= 1.0f)
                    {
                        _Shakes.RemoveAt(index);
                        if (sd.SaveTexCoords)
                            sd.Image.TextureCoordinate = sd.InitialTexCoords;
                        continue;
                    }
                    else
                    {
                        Rect textcoord = sd.Image.TextureCoordinate;

                        float factor = 1.0f;
                        float timeLeft = sd.Timer.TimeLeft;
                        if (timeLeft < sd.Fadeout)
                            factor = timeLeft / sd.Fadeout;

                        textcoord.x += Random.Range(-sd.Amount.x, sd.Amount.x) * factor;
                        textcoord.y += Random.Range(-sd.Amount.y, sd.Amount.y) * factor;
                        sd.Image.TextureCoordinate = textcoord;
                    }
                    index++;
                }
            }
        }

        /// <summary>
        /// Shake image for specific time
        /// </summary>
        /// <param name="image">Image to apply shake </param>
        /// <param name="lenght">lenght of shake animation</param>
        /// <param name="shakeAmount">amount of shake in x,y</param>
        /// <param name="saveTexCoords">Whether to return to initial TexCoords after shake </param>
        /// <param name="fadeout">Fade out shake at specified time left of shake</param>
        public void Shake(ImageWithTexCoords image, float lenght, Vector2 shakeAmount, bool saveTexCoords = false, float fadeout = 0)
        {
            if (image == null) throw new System.ArgumentNullException("Invalid image");
            ShakeData sd = new ShakeData();
            sd.Image = image;
            sd.Amount = shakeAmount;
            sd.Timer.Begin(Mathf.Max(0, lenght));
            sd.InitialTexCoords = image.TextureCoordinate;
            sd.SaveTexCoords = saveTexCoords;
            sd.Fadeout = fadeout;
            _Shakes.Add(sd);
        }
    }
}
