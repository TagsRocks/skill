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
        /// <summary> Texture : it could be Texture2D or MovieTexture(Unity Pro) </summary>
        public Texture[] SplashImages;
        /// <summary> ScaleMode of splash image</summary>
        public ScaleMode Scale = ScaleMode.ScaleToFit;
        /// <summary> Texture to use for fading texture between each splash image (black) </summary>
        public Texture2D FadeTexture;
        /// <summary> Time to show each splash </summary>
        public float SplashTime = 2.0f;
        /// <summary> Size of splash relative to screen (0.0f- 1.0f)</summary>
        public float Size = 0.7f;
        /// <summary> Allow escape slapsh after this time </summary>
        public float AllowEscapeAfter = 20.0f;
        /// <summary> Level to load in background.(Unity Pro) </summary>
        public int LevelToLoad = -1;
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

        private Frame _Frame;
        private Image _ImgSplash;
        private Image _ImgFadeTexture;
        private AsyncOperation _LoadingAsyncOperation;
        private TimeWatch _NextSplashTW;
        private TimeWatch _SplashTW;
        private int _CurrentSplashIndex;
        private Fading _Fading;
        private bool _IsCompleted;

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
            _Fading = GetComponent<Fading>();
        }

        /// <summary>
        /// Start
        /// </summary>
        protected override void Start()
        {
            base.Start();
            useGUILayout = false;
            _Frame = new Frame("SplashFrame");

            for (int i = 0; i < 3; i++)
            {
                _Frame.Grid.RowDefinitions.Add(1, GridUnitType.Star);
                _Frame.Grid.ColumnDefinitions.Add(1, GridUnitType.Star);
            }

            _ImgSplash = new Image() { Row = 1, Column = 1, Scale = this.Scale };
            _ImgFadeTexture = new Image() { Row = 0, Column = 0, ColumnSpan = 3, RowSpan = 3, Texture = FadeTexture, Scale = ScaleMode.StretchToFill, AlphaFading = _Fading };

            _Frame.Grid.Controls.Add(_ImgSplash);
            _Frame.Grid.Controls.Add(_ImgFadeTexture);

            _CurrentSplashIndex = -1;
            ShowNext();
            LoadLevel();
        }



        private void SetSize(float size)
        {
            float clampedSize = Mathf.Clamp01(size) * 100;
            _Frame.Grid.RowDefinitions[0].Height = _Frame.Grid.RowDefinitions[2].Height = _Frame.Grid.ColumnDefinitions[0].Width = _Frame.Grid.ColumnDefinitions[2].Width = new GridLength((100 - clampedSize) * 0.5f, GridUnitType.Star);
            _Frame.Grid.RowDefinitions[1].Height = _Frame.Grid.ColumnDefinitions[1].Width = new GridLength(clampedSize, GridUnitType.Star);
        }

        private void LoadLevel()
        {
            if (LevelToLoad >= 0 && Application.HasProLicense())
            {
                _LoadingAsyncOperation = Application.LoadLevelAsync(LevelToLoad);
                _LoadingAsyncOperation.allowSceneActivation = false;
            }
        }


        private void ShowNext()
        {
            if (_Fading != null)
            {
                // fadeout
                _Fading.FadeOut();

                //prepare to change texture after fadeout                
                _NextSplashTW.Begin(_Fading.FadeOutTime);
            }
            else
                _NextSplashTW.Begin(0.1f);
        }


        /// <summary> Update </summary>
        protected override void Update()
        {
            if (_CurrentSplashIndex >= SplashImages.Length) // all splash showed
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
                }
                if (_SplashTW.ElapsedTime > AllowEscapeAfter)
                {
                    if (Escape())// if user press escape button
                    {
                        if (FastEscape)
                            MoveNext();
                        else
                            ShowNext();
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
            if (_CurrentSplashIndex >= 0)
            {
                if (SplashImages[_CurrentSplashIndex] is MovieTexture)
                {
                    MovieTexture movie = (MovieTexture)SplashImages[_CurrentSplashIndex];
                    movie.Stop();
                    if (audio != null && movie.audioClip != null)
                    {
                        audio.Stop();
                        audio.clip = null;
                    }
                }
            }

            _CurrentSplashIndex++;// go next splash
            if (_CurrentSplashIndex < SplashImages.Length) // if another splash exist
            {
                if (SplashImages[_CurrentSplashIndex] is MovieTexture)
                {
                    MovieTexture movie = (MovieTexture)SplashImages[_CurrentSplashIndex];
                    movie.Play();
                    if (audio != null && movie.audioClip != null)
                    {
                        audio.clip = movie.audioClip;
                        audio.Play();
                    }
                    if (FullScreenMovies)
                        SetSize(1.0f);
                    else
                        SetSize(this.Size);

                    if (_Fading != null)
                    {
                        _Fading.FadeIn(true);
                        _SplashTW.Begin(Mathf.Max(movie.duration - _Fading.FadeOutTime, SplashTime - _Fading.FadeOutTime, _Fading.FadeOutTime + 0.1f));
                    }
                    else
                        _SplashTW.Begin(Mathf.Max(movie.duration - 0.1f, SplashTime - 0.1f, 0.1f));
                }
                else
                {
                    SetSize(this.Size);
                    if (_Fading != null)
                    {
                        _Fading.FadeIn(true);
                        _SplashTW.Begin(Mathf.Max(SplashTime - _Fading.FadeOutTime, _Fading.FadeOutTime + 0.1f));
                    }
                    else
                        _SplashTW.Begin(Mathf.Max(SplashTime - 0.1f, 0.1f));
                }

                _ImgSplash.Texture = SplashImages[_CurrentSplashIndex];// change texture
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
            _Frame.Position = new Rect(0, 0, Screen.width, Screen.height);
            _Frame.OnGUI();
        }
    }
}
