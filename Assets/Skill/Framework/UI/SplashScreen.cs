#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_XBOX360 || UNITY_PS3
#define SUPPORTMOVIE
#endif

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Fading))]
    public class SplashScreen : DynamicBehaviour
    {
        [System.Serializable]
        public class SplashImage
        {
            /// <summary> Optional Name </summary>
            public string Name = "Image";
            /// <summary> Texture : it could be Texture2D or MovieTexture(Unity Pro) </summary>
            public Texture Image = null;
            /// <summary> ScaleMode of splash image</summary>
            public ScaleMode Scale = ScaleMode.ScaleToFit;
            /// <summary> Size of splash relative to screen (0.0f- 1.0f)</summary>
            public float WidthPercent = 0.7f;
            /// <summary> Size of splash relative to screen (0.0f- 1.0f)</summary>
            public float HeightPercent = 0.7f;
            /// <summary> Duration of image </summary>
            public float MaxDuration = 4.0f;
            /// <summary> Allow escape slapsh after this time </summary>
            public float MinDuration = 4.0f;
        }


        /// <summary> Texture : it could be Texture2D or MovieTexture(Unity Pro) </summary>
        public SplashImage[] Images;
        /// <summary> Texture to use for fading texture between each splash image (black) </summary>
        public Texture2D FadeTexture;
        /// <summary> Level to load in background.(Unity Pro) </summary>
        public string LevelToLoad = null;
        /// <summary> Show movies in fullscreen size</summary>
        public bool FullScreenMovies = true;
        /// <summary>
        /// If true when user press escape immediately go to next splash and ignore fadeout
        /// </summary>
        public bool FastEscape = false;
        /// <summary>
        /// When AsyncOperation.progress reach this value we take it as load level process is completed
        /// </summary>
        /// <remarks>
        /// In my experience AsyncOperation.progress never reach 1.0f and i think this is a bug in unity,
        /// anyway if you think i am wrong or this bug is corrected set it to something more than 0.8f
        /// </remarks>
        public float MaxLoadProgress = 0.8f;

        public int GUIDepth = 0;

        private Frame _Frame;
        private Image _ImgSplash;
        private Image _ImgFadeTexture;
        private AsyncOperation _LoadingAsyncOperation;
        private TimeWatch _NextSplashTW;
        private TimeWatch _SplashTW;
        private int _CurrentSplashIndex;
        private Fading _Fading;
        private bool _IsCompleted;
        private AudioSource _Audio;

        /// <summary> Occurs when SplashScreen is completed </summary>
        public event EventHandler Completed;
        /// <summary> Occurs when SplashScreen is completed </summary>
        protected virtual void OnCompleted()
        {
            if (_LoadingAsyncOperation != null && _LoadingAsyncOperation.progress >= MaxLoadProgress)
                _LoadingAsyncOperation.allowSceneActivation = true;
            if (Completed != null) Completed(this, EventArgs.Empty);
        }

        /// <summary>
        /// Get required references
        /// </summary>
        protected override void GetReferences()
        {
            base.GetReferences();
            _Audio = GetComponent<AudioSource>();
            _Fading = GetComponent<Fading>();
            _Fading.Alpha = 1.0f;
        }

        /// <summary>
        /// Start
        /// </summary>
        protected override void Start()
        {
            base.Start();
            Time.timeScale = 1;
            _Frame = new Frame("SplashFrame");

            for (int i = 0; i < 3; i++)
            {
                _Frame.Grid.RowDefinitions.Add(1, GridUnitType.Star);
                _Frame.Grid.ColumnDefinitions.Add(1, GridUnitType.Star);
            }

            _ImgSplash = new Image() { Row = 1, Column = 1 };
            _ImgFadeTexture = new Image() { Row = 0, Column = 0, ColumnSpan = 3, RowSpan = 3, Texture = FadeTexture, Scale = ScaleMode.StretchToFill, AlphaFading = _Fading };

            _Frame.Grid.Controls.Add(_ImgSplash);
            _Frame.Grid.Controls.Add(_ImgFadeTexture);

            _CurrentSplashIndex = -1;
            ShowNext();
            LoadLevel();
        }



        private void SetSize(float width, float height, ScaleMode scale)
        {
            float clampedSizeW = Mathf.Clamp01(width) * 100;
            _Frame.Grid.ColumnDefinitions[0].Width = _Frame.Grid.ColumnDefinitions[2].Width = new GridLength((100 - clampedSizeW) * 0.5f, GridUnitType.Star);
            _Frame.Grid.ColumnDefinitions[1].Width = new GridLength(clampedSizeW, GridUnitType.Star);

            float clampedSizeH = Mathf.Clamp01(height) * 100;
            _Frame.Grid.RowDefinitions[0].Height = _Frame.Grid.RowDefinitions[2].Height = new GridLength((100 - clampedSizeH) * 0.5f, GridUnitType.Star);
            _Frame.Grid.RowDefinitions[1].Height = new GridLength(clampedSizeH, GridUnitType.Star);

            _ImgSplash.Scale = scale;
        }

        private void LoadLevel()
        {
            if (!string.IsNullOrEmpty(LevelToLoad) && Application.HasProLicense())
            {
                _LoadingAsyncOperation = Application.LoadLevelAsync(LevelToLoad);
                _LoadingAsyncOperation.allowSceneActivation = false;
            }
        }


        private void ShowNext()
        {
            if (_CurrentSplashIndex >= 0 && _Fading != null)
            {
                // fadeout
                _Fading.FadeOut();

                //prepare to change texture after fadeout                
                _NextSplashTW.Begin(_Fading.FadeOutTime + 0.2f);
            }
            else
                _NextSplashTW.Begin(0.2f);
        }


        /// <summary> Update </summary>
        protected override void Update()
        {
            if (_CurrentSplashIndex >= Images.Length) // all splash showed
            {
                if (!_IsCompleted)
                {
                    _IsCompleted = true;
                    OnCompleted();// raise Completed event
                }
            }
            else
            {
                if (_NextSplashTW.IsEnabled)
                {
                    if (_NextSplashTW.IsOver) // fadeout is complete and times to change splash
                    {
                        _NextSplashTW.End();
                        MoveNext();// show next and fadein
                    }
                }
                else if (_SplashTW.IsEnabled)
                {
                    if (_SplashTW.IsOver) // times to fadeout and prepare to show next splash
                    {
                        _SplashTW.End();
                        ShowNext();
                    }
                    else if (_SplashTW.ElapsedTime > Images[_CurrentSplashIndex].MinDuration)
                    {
                        if (Escape())// if user press escape button
                        {
                            if (FastEscape)
                                MoveNext();
                            else
                                ShowNext();
                        }

                    }
                }
                _Frame.Update();
            }
            base.Update();
        }

        private void MoveNext()
        {
            _NextSplashTW.End();
            // stop previous movie
#if SUPPORTMOVIE
            if (_CurrentSplashIndex >= 0)
            {
                if (Images[_CurrentSplashIndex].Image is MovieTexture)
                {
                    MovieTexture movie = (MovieTexture)Images[_CurrentSplashIndex].Image;
                    movie.Stop();
                    if (_Audio != null && movie.audioClip != null)
                    {
                        _Audio.Stop();
                        _Audio.clip = null;
                    }
                }
            }
#endif

            _CurrentSplashIndex++;// go next splash
            if (_CurrentSplashIndex < Images.Length) // if another splash exist
            {
#if SUPPORTMOVIE
                if (Images[_CurrentSplashIndex].Image is MovieTexture)
                {
                    MovieTexture movie = (MovieTexture)Images[_CurrentSplashIndex].Image;
                    movie.Play();
                    if (_Audio != null && movie.audioClip != null)
                    {
                        _Audio.clip = movie.audioClip;
                        _Audio.Play();
                    }
                    if (FullScreenMovies)
                        SetSize(1.0f, 1.0f, Images[_CurrentSplashIndex].Scale);
                    else
                        SetSize(Images[_CurrentSplashIndex].WidthPercent, Images[_CurrentSplashIndex].HeightPercent, Images[_CurrentSplashIndex].Scale);

                    if (_Fading != null)
                    {
                        _Fading.FadeIn(true);
                        _SplashTW.Begin(Mathf.Max(movie.duration - _Fading.FadeOutTime, Images[_CurrentSplashIndex].MaxDuration - _Fading.FadeOutTime, _Fading.FadeOutTime + 0.1f));
                    }
                    else
                        _SplashTW.Begin(Mathf.Max(movie.duration - 0.1f, Images[_CurrentSplashIndex].MaxDuration - 0.1f, 0.1f));
                }
                else
                {
#endif
                    SetSize(Images[_CurrentSplashIndex].WidthPercent, Images[_CurrentSplashIndex].HeightPercent, Images[_CurrentSplashIndex].Scale);
                    if (_Fading != null)
                    {
                        _Fading.FadeIn(true);
                        _SplashTW.Begin(Mathf.Max(Images[_CurrentSplashIndex].MaxDuration - _Fading.FadeOutTime, _Fading.FadeOutTime + 0.1f));
                    }
                    else
                        _SplashTW.Begin(Mathf.Max(Images[_CurrentSplashIndex].MaxDuration - 0.1f, 0.1f));
#if SUPPORTMOVIE
                }
#endif

                _ImgSplash.Texture = Images[_CurrentSplashIndex].Image;// change texture
            }
            else
            {
                if (_Fading != null) _Fading.Alpha = 1.0f;
                _ImgSplash.Texture = null;
            }
        }

        /// <summary>
        /// Allow subclass to define when user press escape 
        /// by defalut : Input.GetKeyDown(KeyCode.Escape)
        /// </summary>
        /// <returns></returns>
        protected virtual bool Escape()
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }

        /// <summary>
        /// OnGUI
        /// </summary>
        protected virtual void OnGUI()
        {
            GUI.depth = GUIDepth;
            _Frame.Position = new Rect(0, 0, Screen.width, Screen.height);
            _Frame.OnGUI();
        }
    }
}
