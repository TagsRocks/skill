using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;

namespace Skill.Studio.Controls
{
    public enum GifWrapMode
    {
        Loop,
        Once,
        ClampForever
    }

    public class GifImage : Image
    {

        private bool _ReachEnd;
        private bool _IsPlaying;
        public void Play()
        {
            if (!_IsPlaying)
            {
                _IsPlaying = true;
                _ReachEnd = false;
                FrameIndex = 0;
            }
        }
        public void Stop()
        {
            _IsPlaying = false;
            _ReachEnd = false;
            FrameIndex = 0;
        }

        public int FrameIndex
        {
            get { return (int)GetValue(FrameIndexProperty); }
            set { SetValue(FrameIndexProperty, value); }
        }

        public static readonly DependencyProperty FrameIndexProperty =
            DependencyProperty.Register("FrameIndex", typeof(int), typeof(GifImage), new UIPropertyMetadata(0, new PropertyChangedCallback(ChangingFrameIndex)));

        static void ChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            GifImage ob = obj as GifImage;
            if (ob._Gif != null)
            {
                int newIndex = (int)ev.NewValue;
                if (ob._IsPlaying)
                {
                    ob._Anim.SpeedRatio = 1;
                    if (ob.WrapMode == GifWrapMode.Once)
                    {
                        if (newIndex == ob._Gif.Frames.Count - 1)
                            ob._ReachEnd = true;
                        else if (newIndex == 0 && ob._ReachEnd)
                            ob._IsPlaying = false;
                    }
                    else if (ob.WrapMode == GifWrapMode.ClampForever)
                    {
                        if (newIndex == ob._Gif.Frames.Count - 1)
                        {
                            ob._ReachEnd = true;
                            ob._IsPlaying = false;
                        }
                    }
                    if (newIndex < ob._Gif.Frames.Count)
                    {
                        ob.Source = ob._Gif.Frames[newIndex];
                        ob.InvalidateVisual();
                    }
                }
            }
        }

        private string _GifSource;
        public string GifSource
        {
            get { return _GifSource; }
            set
            {
                if (_GifSource != value)
                {
                    _GifSource = value;
                    LoadGif();
                }
            }
        }

        private GifWrapMode _WrapMode = GifWrapMode.Loop;
        public GifWrapMode WrapMode
        {
            get { return _WrapMode; }
            set
            {
                if (_WrapMode != value)
                {
                    _WrapMode = value;
                    Play();
                }
            }
        }

        private System.IO.Stream _Stream;
        private GifBitmapDecoder _Gif;
        private Int32Animation _Anim;

        private int _FrameRate = 30;
        /// <summary> Number of frame per second (default is 30)</summary>
        public int FrameRate
        {
            get { return _FrameRate; }
            set
            {
                if (value <= 0)
                    value = 1;
                if (_FrameRate != value)
                {
                    _FrameRate = value;
                    SetFrameRate();
                }
            }
        }

        private void SetFrameRate()
        {
            if (_Anim != null && _Gif != null)
            {
                double duration = (double)_Gif.Frames.Count / FrameRate;
                int seconds = (int)duration;
                int miliseconds = (int)((duration - seconds) * 1000);
                _Anim.Duration = new Duration(new TimeSpan(0, 0, 0, seconds, miliseconds));
                _AnimationIsWorking = false;
            }
        }

        private void LoadGif()
        {
            Unload();
            if (System.IO.File.Exists(GifSource))
            {
                _Stream = new System.IO.FileStream(GifSource, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                _Gif = new GifBitmapDecoder(_Stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                _Anim.From = 0;
                _Anim.To = _Gif.Frames.Count - 1;
                SetFrameRate();
                Play();
                Source = _Gif.Frames[0];
            }            
        }

        public void Unload()
        {
            Source = null;
            _Gif = null;
            if (_Stream != null)
                _Stream.Close();
            _Stream = null;
        }

        public GifImage()
        {
            _Anim = new Int32Animation();
            _Anim.RepeatBehavior = RepeatBehavior.Forever;
        }

        bool _AnimationIsWorking = false;
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (!_AnimationIsWorking && Source != null)
            {
                BeginAnimation(FrameIndexProperty, _Anim);
                _AnimationIsWorking = true;
            }
        }
    }
}
