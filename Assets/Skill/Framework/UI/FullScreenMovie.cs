#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_XBOX360 || UNITY_PS3
#define SUPPORTMOVIE
#endif

using UnityEngine;
using System.Collections;
using Skill.Framework;
using Skill.Framework.UI;

namespace Skill.Framework.UI
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Fading))]
    public class FullScreenMovie : DynamicBehaviour
    {
        /// <summary> GUIDepth  </summary>
        public int GUIDepth = 9;
        /// <summary> ScaleMode of splash image</summary>
        public ScaleMode Scale = ScaleMode.ScaleToFit;
        /// <summary> Texture to use for fading texture between each splash image (black) </summary>
        public Texture2D FadeTexture;
        /// <summary> Texture to use for background (black) </summary>
        public Texture2D Background;
        /// <summary> Allow escape slapsh after this time </summary>
        public float AllowEscapeAfter = 2.0f;
        /// <summary> If true when user press escape immediately go to next splash and ignore fadeout </summary>
        public bool FastEscape = false;
        /// <summary> Is cutscene </summary>
        public bool CutSceneEnable = false;
        /// <summary> optional scene fading to fade rest of rendering </summary>
        public Fading SceneFading;

        private Frame _Frame;
        private Image _ImgMovie;
        private Image _ImgFadeTexture;
        private Image _ImgBackground;
        private TimeWatch _MovieTW;
        private TimeWatch _EndTW;
        private Fading _Fading;
        private bool _SavedCutSceneEnable;
#if SUPPORTMOVIE
        private AudioSource _Audio;
        private MovieTexture _LastMovie;
#endif


        /// <summary> Occurs when starts playing video</summary>
        public event System.EventHandler Begin;
        /// <summary> Occurs when starts playing video</summary>
        protected virtual void OnBegin() { if (Begin != null) Begin(this, System.EventArgs.Empty); }

        /// <summary> Occurs when played video ended of stoped</summary>
        public event System.EventHandler End;
        /// <summary> Occurs when played video ended of stoped</summary>
        protected virtual void OnEnd() { if (End != null) End(this, System.EventArgs.Empty); }

        /// <summary> Currently playing video (can be null)</summary>
        public Texture CurrentlyPlaying
        {
            get
            {
#if SUPPORTMOVIE
                return _LastMovie;
#else
                return null;
#endif
            }
        }

        /// <summary> Is playing a video </summary>
        public bool IsPlaying
        {
            get
            {
#if SUPPORTMOVIE
                return _LastMovie != null && _LastMovie.isPlaying;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Get required references
        /// </summary>
        protected override void GetReferences()
        {
            base.GetReferences();
            _Fading = GetComponent<Fading>();
#if SUPPORTMOVIE
            _Audio = GetComponent<AudioSource>();
#endif
            _Fading.Alpha = 1.0f;
        }

        /// <summary>
        /// Start
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            useGUILayout = false;
            _Frame = new Frame("MovieFrame");
            _Frame.Grid.RowDefinitions.Add(1, GridUnitType.Star);
            _Frame.Grid.ColumnDefinitions.Add(1, GridUnitType.Star);

            _ImgBackground = new Image() { Row = 0, Column = 0, RowSpan = 10, ColumnSpan = 10, Scale = ScaleMode.StretchToFill, Texture = Background };
            _ImgMovie = new Image() { Row = 0, Column = 0, Scale = this.Scale };
            _ImgFadeTexture = new Image() { Row = 0, Column = 0, Texture = FadeTexture, Scale = ScaleMode.StretchToFill, AlphaFading = _Fading };

            _Frame.Grid.Controls.Add(_ImgBackground);
            _Frame.Grid.Controls.Add(_ImgMovie);
            _Frame.Grid.Controls.Add(_ImgFadeTexture);

            _SavedCutSceneEnable = false;
        }

        public void Play(Texture movie)
        {

#if SUPPORTMOVIE
            if (_Fading != null) _Fading.Alpha = 1.0f;

            if (_LastMovie != null)
            {
                _LastMovie.Stop();
                if (_Audio != null && _LastMovie.audioClip != null)
                {
                    _Audio.Stop();
                    _Audio.clip = null;
                }
            }
            if (movie != null && movie is MovieTexture)
            {
                MovieTexture movieTexture = (MovieTexture)movie;

                _LastMovie = movieTexture;
                movieTexture.Play();
                if (_Audio != null && movieTexture.audioClip != null)
                {
                    _Audio.clip = movieTexture.audioClip;
                    _Audio.Play();
                }
                if (_Fading != null)
                {
                    _Fading.FadeToZero(true);
                    _MovieTW.Begin(Mathf.Max(movieTexture.duration - _Fading.FadeOutTime, _Fading.FadeOutTime + 0.1f));
                }
                else
                {
                    _MovieTW.Begin(movieTexture.duration);
                }

                _EndTW.End();
                _ImgMovie.Texture = movie;// change texture
                _SavedCutSceneEnable = Global.CutSceneEnable;
                Global.CutSceneEnable = CutSceneEnable;
                if (SceneFading != null)
                    SceneFading.FadeToOne();
                OnBegin();
            }
            else
            {
                _ImgMovie.Texture = null;
            }
            enabled = true;
#endif

        }


        public void Stop()
        {
#if SUPPORTMOVIE
            if (_LastMovie != null)
            {
                if (_LastMovie.isPlaying)
                {
                    EndMovie();
                }
            }
#endif
        }

        private void EndMovie()
        {
            if (FastEscape)
                _EndTW.Begin(0);
            else if (_Fading != null)
                _EndTW.Begin(_Fading.FadeOutTime + 0.1f);
            else
                _EndTW.Begin(0.1f);

            if (_Fading != null)
                _Fading.FadeToOne();
            _MovieTW.End();
        }

        /// <summary> Update </summary>
        protected override void Update()
        {
            if (_MovieTW.IsEnabled)
            {
                if (_MovieTW.IsOver)
                {
                    EndMovie();
                }
                else if (_MovieTW.ElapsedTime > AllowEscapeAfter)
                {
                    if (Escape())// if user press escape button                
                        EndMovie();
                }
            }
            else if (_EndTW.IsEnabled)
            {
                if (_EndTW.IsOver)
                {
#if SUPPORTMOVIE
                    if (_LastMovie != null)
                        _LastMovie.Stop();
#endif
                    _MovieTW.End();
                    _ImgMovie.Texture = null;
                    enabled = false;
                    if (CutSceneEnable)
                        Global.CutSceneEnable = _SavedCutSceneEnable;
                    if (SceneFading != null)
                        SceneFading.FadeToZero();
                    OnEnd();
#if SUPPORTMOVIE
                    _LastMovie = null;
#endif

                }
            }
            else
                enabled = false;
            _Frame.Update();
            base.Update();
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
            if (_MovieTW.IsEnabled || _EndTW.IsEnabled)
            {
                GUI.depth = GUIDepth;
                _Frame.Position = new Rect(0, 0, Screen.width, Screen.height);
                _Frame.OnGUI();
            }
        }
    }

}