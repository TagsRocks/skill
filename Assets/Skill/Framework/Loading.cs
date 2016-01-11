using System;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Simple class to use in loading scene and load next level(work best in unity pro)
    /// </summary>
    [RequireComponent(typeof(Fading))]
    public abstract class Loading : DynamicBehaviour
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
        private Fading _Fading;
        private Skill.Framework.TimeWatch _ActivateSceneTW;
        private float _Progress = 0;

        /// <summary>
        /// Loading progress (0.0 - 1.0)
        /// </summary>
        public float Progress
        {
            get { return _Progress; }
            private set
            {
                if (_Progress != value)
                {
                    _Progress = value;
                    OnProgressChanged();
                }
            }
        }

        /// <summary> Occurs when progress changed </summary>
        protected abstract void OnProgressChanged();

        /// <summary>
        /// Implament this to specify witch level to load
        /// </summary>
        protected abstract string NextLevel { get; }

        /// <summary> GetReferences </summary>
        protected override void GetReferences()
        {
            base.GetReferences();
            _Fading = GetComponent<Fading>();
        }

        /// <summary> Start </summary>
        protected override void Start()
        {
            base.Start();

            if (!Application.isEditor) TestMode = false;
            _Fading.FadeToZero(true);
            LoadLevel();
            Progress = 0;
        }

        private void LoadLevel()
        {
            if (!TestMode || !Application.isEditor)
            {
                _LoadingAsyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(NextLevel);
                _LoadingAsyncOperation.allowSceneActivation = false;
            }
        }

        /// <summary> Update </summary>
        protected override void Update()
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
                Progress = _Progress + MaxProgressSpeed * Time.deltaTime;
                if (Progress > p)
                    Progress = p;
            }


            if ((_LoadingAsyncOperation != null || TestMode) && !_ActivateSceneTW.IsEnabled)
            {
                if (_Progress > 0.99f)
                {
                    _ActivateSceneTW.Begin(_Fading.FadeOutTime + 0.5f);
                    _Fading.FadeToOne();
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
