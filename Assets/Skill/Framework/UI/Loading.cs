using System;
using UnityEngine;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Simple class to use in loading scene and load next level(work best in unity pro)
    /// </summary>
    [RequireComponent(typeof(FadeScreen))]
    public abstract class Loading : Skill.Framework.DynamicBehaviour
    {        
        /// <summary>
        /// Maximum allowed speed for progress
        /// </summary>
        public float MaxProgressSpeed = 0.2f;
        /// <summary>
        /// When AsyncOperation.progress reach this value we take it as load level process is completed
        /// </summary>
        /// <remarks>
        /// In my experience AsyncOperation.progress never reach 1.0f and i think this is a bug in unity,
        /// anyway if you think i am wrong or this bug is corrected set it to something more than 0.8f
        /// </remarks>
        public float MaxLoadProgress = 0.8f;

        /// <summary>
        /// No level loaded, just simulate progress based on MaxProgressSpeed
        /// </summary>
        public bool TestMode;

        private AsyncOperation _LoadingAsyncOperation;
        private FadeScreen _FadeScreen;
        private Skill.Framework.TimeWatch _ActivateSceneTW;
        private float _Progress;
        private bool _LoadLevelAfterFadeIn;

        /// <summary>
        /// Loading progress (0.0 - 1.0)
        /// </summary>
        public float Progress { get { return _Progress; } }

        /// <summary>
        /// Implament this to specify witch level to load
        /// </summary>
        protected abstract string NextLevel { get; }

        /// <summary> GetReferences </summary>
        protected override void GetReferences()
        {
            base.GetReferences();
            _FadeScreen = GetComponent<FadeScreen>();
        }

        /// <summary> Start </summary>
        protected override void Start()
        {
            base.Start();

            if (!Application.isEditor) TestMode = false;
            _FadeScreen.Fading.FadeIn();
            _LoadLevelAfterFadeIn = true;
            _Progress = 0;
        }

        private void LoadLevel()
        {
            if (!TestMode || !Application.isEditor)
            {
                if (Application.HasProLicense())
                {
                    _LoadingAsyncOperation = Application.LoadLevelAsync(NextLevel);
                    _LoadingAsyncOperation.allowSceneActivation = false;
                }
                else
                {
                    Application.LoadLevel(NextLevel);
                }
            }
        }

        /// <summary> Update </summary>
        protected override void Update()
        {
            if (_LoadLevelAfterFadeIn)
            {
                if (!_FadeScreen.Fading.IsFadeIn)
                {
                    LoadLevel();
                    _LoadLevelAfterFadeIn = false;
                }
            }
            else
            {
                float p;
                if (_LoadingAsyncOperation != null)
                {
                    p = _LoadingAsyncOperation.progress;
                    if (p > MaxLoadProgress)
                        p = 1.0f;
                }
                else
                {
                    p = _Progress + MaxProgressSpeed * Time.deltaTime;
                    if (p > 1.0f) p = 1.0f;
                }

                if (p > _Progress)
                {
                    _Progress += MaxProgressSpeed * Time.deltaTime;
                    if (_Progress > p)
                        _Progress = p;
                }
            }            

            if ((_LoadingAsyncOperation != null || TestMode) && !_ActivateSceneTW.IsEnabled)
            {
                if (_Progress > 0.99f)
                {
                    _ActivateSceneTW.Begin(_FadeScreen.Fading.FadeOutTime + 0.5f);
                    _FadeScreen.Fading.FadeOut();
                }
            }
            else if (_ActivateSceneTW.IsEnabledAndOver && _LoadingAsyncOperation != null)
            {
                _ActivateSceneTW.End();
                _LoadingAsyncOperation.allowSceneActivation = true;
            }
            base.Update();
        }        
    }
}
