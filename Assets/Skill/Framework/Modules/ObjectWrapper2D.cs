using UnityEngine;
using System.Collections;
namespace Skill.Framework.Modules
{
    public class ObjectWrapper2D : DynamicBehaviour
    {
        /// <summary> Distance to camera if camera is perspective</summary>
        public float Distance = 5.0f;
        /// <summary> Speed </summary>
        public float Speed = 0;
        /// <summary> lenght of single object</summary>
        public float Width = 10;


        private Transform[] _ObjectsToWrap;
        private float _Left;
        private float _Right;
        private Camera _Camera;
        private Transform _CameraTransform;
        private ScreenSizeChange _ScreenSizeChange;
        private float _FrustumHalfWidth;

        protected override void GetReferences()
        {
            base.GetReferences();
            _Camera = Camera.main;
            _CameraTransform = _Camera.transform;

            _ObjectsToWrap = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
                _ObjectsToWrap[i] = transform.GetChild(i);
        }

        protected override void Start()
        {
            base.Start();
            _Left = (Width * _ObjectsToWrap.Length) * -0.5f;
            _Right = (Width * _ObjectsToWrap.Length) * 0.5f;

            Vector3 pos = transform.position;
            pos.x = _CameraTransform.position.x;
            transform.position = pos;

            AlignObjects();
        }
        private void AlignObjects()
        {
            for (int i = 0; i < _ObjectsToWrap.Length; i++)
            {
                Vector3 pos = _ObjectsToWrap[i].localPosition;
                pos.x = _Left + (Width * i) + (Width * 0.5f);
                _ObjectsToWrap[i].localPosition = pos;
            }
        }

        private void ShiftRight()
        {
            this._Left += Width;
            this._Right += Width;

            Transform temp = _ObjectsToWrap[0];
            for (int i = 1; i < _ObjectsToWrap.Length; i++)
                _ObjectsToWrap[i - 1] = _ObjectsToWrap[i];
            _ObjectsToWrap[_ObjectsToWrap.Length - 1] = temp;

            AlignObjects();
        }

        private void ShiftLeft()
        {
            this._Left -= Width;
            this._Right -= Width;

            Transform temp = _ObjectsToWrap[_ObjectsToWrap.Length - 1];
            for (int i = _ObjectsToWrap.Length - 2; i >= 0; i--)
                _ObjectsToWrap[i + 1] = _ObjectsToWrap[i];
            _ObjectsToWrap[0] = temp;

            AlignObjects();
        }

        protected override void Update()
        {
            if (Global.IsGamePaused) return;

            if (_ScreenSizeChange.IsChanged)
            {
                if (_Camera.orthographic)
                    _FrustumHalfWidth = MathHelper.OrthographicWidth(_Camera) * 0.5f;
                else
                    _FrustumHalfWidth = MathHelper.FrustumWidthAtDistance(_Camera, Distance) * 0.5f;
            }

            float cameraX = _CameraTransform.position.x;
            float screenLeft = cameraX - _FrustumHalfWidth;
            float screenRight = cameraX + _FrustumHalfWidth;

            Vector3 pos = transform.position;
            if (Speed != 0)
            {
                float deltaMove = Time.deltaTime * Speed;
                pos.x += deltaMove;
                transform.position = pos;
            }

            while (screenLeft < (pos.x + this._Left + Width))
                ShiftLeft();
            while (screenRight > (pos.x + this._Right - Width))
                ShiftRight();
            base.Update();
        }
    }
}