using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Skill.Framework.UI
{

    /// <summary>
    /// A helper class to simulate zoom in ImageWithTexCoords
    /// </summary>
    public class ImageZoom : DynamicBehaviour
    {
        class ZoomData
        {
            public ImageWithTexCoords Image;
            public Rect SourceRect;
            public Rect DestRect;
            public TimeWatch Timer;
        }

        private List<ZoomData> _Zooms;
        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _Zooms = new List<ZoomData>();
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (Global.IsGamePaused) return;

            if (_Zooms.Count > 0)
            {
                int index = 0;
                while (index < _Zooms.Count)
                {
                    ZoomData zd = _Zooms[index];
                    float percent = zd.Timer.Percent;
                    if (percent >= 1.0f)
                    {
                        zd.Image.TextureCoordinate = zd.DestRect;
                        _Zooms.RemoveAt(index);
                        continue;
                    }
                    else
                    {
                        Rect textcoords = new Rect();
                        textcoords.x = Mathf.Lerp(zd.SourceRect.x, zd.DestRect.x, percent);
                        textcoords.y = Mathf.Lerp(zd.SourceRect.y, zd.DestRect.y, percent);
                        textcoords.width = Mathf.Lerp(zd.SourceRect.width, zd.DestRect.width, percent);
                        textcoords.height = Mathf.Lerp(zd.SourceRect.height, zd.DestRect.height, percent);
                        zd.Image.TextureCoordinate = textcoords;
                    }
                    index++;
                }
            }
            base.Update();
        }

        /// <summary>
        /// Zoom from startTextCoords to destTextCoords
        /// </summary>
        /// <param name="image">Image to apply zoom effect</param>
        /// <param name="lenght">lenght of zoom animation</param>
        /// <param name="startTextCoords">start TextCoords</param>
        /// <param name="destTextCoords">destination TextCoords</param>
        public void Zoom(ImageWithTexCoords image, float lenght, Rect startTextCoords, Rect destTextCoords)
        {
            if (image == null) throw new System.ArgumentNullException("Invalid image");
            ZoomData zd = new ZoomData();
            zd.Image = image;
            zd.SourceRect = startTextCoords;
            zd.DestRect = destTextCoords;
            zd.Timer.Begin(Mathf.Max(0, lenght));
            _Zooms.Add(zd);
        }

        /// <summary>
        /// Zoom to center of image 
        /// </summary>
        /// <param name="image">Image to apply zoom effect</param>
        /// <param name="lenght">lenght of zoom animation</param>
        /// <param name="zoomInX">zoom percent in x</param>
        /// <param name="zoomInY"> zoom percent in y </param>
        public void ZoomTo(ImageWithTexCoords image, float lenght, float zoomInX, float zoomInY)
        {
            if (image == null) throw new System.ArgumentNullException("Invalid image");
            if (zoomInX < Mathf.Epsilon) zoomInX = Mathf.Epsilon;
            if (zoomInY < Mathf.Epsilon) zoomInY = Mathf.Epsilon;

            Rect startTextCoords = image.TextureCoordinate;

            Rect destTextCoords = new Rect();
            destTextCoords.width = 1.0f / zoomInX;
            destTextCoords.height = 1.0f / zoomInY;

            destTextCoords.x = (1.0f - destTextCoords.width) * 0.5f;
            destTextCoords.y = (1.0f - destTextCoords.height) * 0.5f;

            Zoom(image, lenght, startTextCoords, destTextCoords);
        }

        /// <summary>
        /// Zoom from currect TextCoords to destTextCoords
        /// </summary>
        /// <param name="image">Image to apply zoom effect</param>
        /// <param name="lenght">lenght of zoom animation</param>
        /// <param name="destTextCoords">destination TextCoords</param>
        public void ZoomTo(ImageWithTexCoords image, float lenght, Rect destTextCoords)
        {
            if (image == null) throw new System.ArgumentNullException("Invalid image");
            Rect startTextCoords = image.TextureCoordinate;
            Zoom(image, lenght, startTextCoords, destTextCoords);
        }

        /// <summary>
        /// zoom from currect TextCoords to rect defined in pixels
        /// </summary>
        /// <param name="image">Image to apply zoom effect</param>
        /// <param name="lenght">lenght of zoom animation</param>
        /// <param name="x"> pixel x (0 - image.Texture.width)</param>
        /// <param name="y"> pixel y (0 - image.Texture.height) </param>
        /// <param name="width">with of destination in pixel</param>
        /// <param name="height">height of destination in pixel</param>
        /// <param name="inverseY">Inverse y</param>
        public void ZoomTo(ImageWithTexCoords image, float lenght, int x, int y, int width, int height, bool inverseY = false)
        {
            if (image == null) throw new System.ArgumentNullException("Invalid image");
            if (image.Texture == null) throw new System.ArgumentNullException("Invalid image.Texture");
            Rect startTextCoords = image.TextureCoordinate;

            Rect destTextCoords = new Rect();

            destTextCoords.width = ((float)width / (float)image.Texture.width);
            destTextCoords.height = ((float)height / (float)image.Texture.height);
            destTextCoords.x = ((float)x / (float)image.Texture.width);
            destTextCoords.y = ((float)y / (float)image.Texture.height);

            if (inverseY) destTextCoords.y = 1.0f - destTextCoords.y;

            Zoom(image, lenght, startTextCoords, destTextCoords);
        }
    }
}