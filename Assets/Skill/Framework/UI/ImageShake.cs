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

        public bool UseInitialTexCoords = true;
        public bool NegativeTexCoords = false;

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

                        Rect textcoord;
                        if (UseInitialTexCoords)
                            textcoord = sd.InitialTexCoords;
                        else
                            textcoord = sd.Image.TextureCoordinate;

                        float factor = 1.0f;
                        float timeLeft = sd.Timer.TimeLeft;
                        if (timeLeft < sd.Fadeout)
                            factor = Mathf.Max(1.0f, timeLeft / sd.Fadeout);

                        textcoord.x += Random.Range(-sd.Amount.x, sd.Amount.x) * factor;
                        textcoord.y += Random.Range(-sd.Amount.y, sd.Amount.y) * factor;

                        if (!NegativeTexCoords)
                        {
                            if (textcoord.x < 0) textcoord.x = 0;
                            if (textcoord.y < 0) textcoord.y = 0;
                        }

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
        /// <param name="duration">lenght of shake animation</param>
        /// <param name="shakeAmount">amount of shake in x,y (0.0f - 1.0f)</param>
        /// <param name="saveTexCoords">Whether to return to initial TexCoords after shake </param>
        /// <param name="fadeout">Fade out shake at specified time left of shake</param>
        public void Shake(ImageWithTexCoords image, float duration, Vector2 shakeAmount, bool saveTexCoords = false, float fadeout = 0)
        {
            if (image == null) throw new System.ArgumentNullException("Invalid image");
            ShakeData sd = new ShakeData();
            sd.Image = image;
            sd.Amount = shakeAmount;
            sd.Timer.Begin(Mathf.Max(0, duration));
            sd.InitialTexCoords = image.TextureCoordinate;
            sd.SaveTexCoords = saveTexCoords;
            sd.Fadeout = fadeout;
            _Shakes.Add(sd);
        }
    }
}
