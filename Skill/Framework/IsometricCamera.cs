using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Isometric camera to view target from above. Add this component to 'UnityEngine.Camera' GameObject and set Target to player or a movable object.
    /// </summary>
    [AddComponentMenu("Skill/Camera/Isometric")]
    [RequireComponent(typeof(Camera))]
    public class IsometricCamera : DynamicBehaviour
    {
        /// <summary> Target to follow </summary>
        public Transform Target;
        /// <summary> How to smooth movement of camera when following target</summary>
        public SmoothingParameters MovementSmoothing;
        /// <summary> How to smooth offset value of camera relative to target</summary>
        public SmoothingParameters OffsetSmoothing;
        /// <summary> How to smooth zoom of camera</summary>
        public SmoothingParameters ZoomSmoothing;
        /// <summary> Camera moves by mouse when mouse position gets far from center of screen. </summary>
        public float CameraPreview = 2.0f;
        /// <summary> Rotation angle around target ( 0 - 360) </summary>
        public float AroundAngle = 0.0f;
        /// <summary> Rotation angle behind target( 0 - 90). 0 is completely horizontal to target and 90 is completely vertical to target. </summary>
        public float LookAngle = 45.0f;
        /// <summary> Minimum distance to target when PointOfIntrest is close to target</summary>
        public float ZoomIn = 8;
        /// <summary> Maximum distance to target when PointOfIntrest is far from target</summary>
        public float ZoomOut = 16;
        /// <summary> Maximum distance of PointOfIntrest from target</summary>
        public float MaxTargetOffset = 6;
        /// <summary> Maximum distance of camera position to desired camera position when smoothing follow motion </summary>
        /// <remarks>
        /// When target moves very fast it is important to keep target always in view. when distance of camera position to desired camera position is less than MinReachFactor
        /// then smoothing performed normal speed, but when this distance gets far from MinReachFactor and towards MaxReachFactor smoothing performed faster to reach desired camera position.
        /// by tweaking these factors you can get your desired result.
        /// </remarks>
        public float MaxReachFactor = 10;
        /// <summary> Minimum distance of camera position to desired camera position when smoothing follow motion </summary>
        /// <remarks>
        /// When target moves very fast it is important to keep target always in view. when distance of camera position to desired camera position is less than MinReachFactor
        /// then smoothing performed normal speed, but when this distance gets far from MinReachFactor and towards MaxReachFactor smoothing performed faster to reach desired camera position.
        /// by tweaking these factors you can get your desired result.
        /// </remarks>
        public float MinReachFactor = 4;
        /// <summary> Field of view </summary>
        public float Fov = 60;

        /// <summary> height of cursor plane above target </summary>
        public float CursorPlaneHeight = 0;

        /// <summary>
        /// Apply static offset to position of camera
        /// </summary>
        public Vector3 StaticOffset = Vector3.zero;

        /// <summary> Prepare a cursor point variable. This is the mouse position on PC and controlled by the thumbstick on mobiles. </summary>
        public Vector3 CursorScreenPosition { get; private set; }
        /// <summary> Position of cursur in world on CursorPlane </summary>
        public Vector3 CursorWorldPosition { get; private set; }
        /// <summary> Screen forward direction</summary>
        public Vector3 ScreenForward { get; private set; }
        /// <summary> Screen right direction</summary>
        public Vector3 ScreenRight { get; private set; }
        /// <summary> Camera </summary>
        public Camera Camera { get; private set; }
        /// <summary> Current zoom distance </summary>
        public float Zoom { get { return _Zoom.Current; } }
        /// <summary> It can be center of all enemies around player </summary>
        public virtual Vector3 PointOfInterest { get { return Target.position; } }


        // Private memeber data                
        private Transform _CameraTransform;
        private Vector3 _CameraVelocity;

        private Quaternion _ScreenMovementSpace;
        private Plane _MovementPlane;

        private Smoothing3 _Offset;
        private Smoothing _Zoom;
        private float _LengthOffset;

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _CameraVelocity = Vector3.zero;            
            _CameraTransform = Camera.transform;
            // Set the initial cursor position to the center of the screen
            CursorScreenPosition = new Vector3(0.5f * Screen.width, 0.5f * Screen.height, 0);
        }

        protected override void GetReferences()
        {
            base.GetReferences();
            // Set main camera
            Camera = GetComponent<Camera>();
            if (Camera == null)
            {
                Debug.LogError("Camera component not found. Isometric camera shold assigned to a camera GameOject.");
                return;
            }
        }

        /// <summary>
        /// Use this for initialization
        /// </summary>
        protected override void Start()
        {
            base.Start();
            // caching movement plane
            _MovementPlane = new Plane(Vector3.up, Target.position);

            // make sure zoom factors are in correct range
            if (ZoomIn < 0) ZoomIn = 0;
            if (ZoomOut < ZoomIn) ZoomOut = ZoomIn;

            this._Zoom.Reset((ZoomIn + ZoomOut) * 0.5f);

            UpdateCamera(true);
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (Camera == null) return;

            if (!Global.CutSceneEnable) // do not modify camera if game is in cutscene mode
            {
                Vector3 center = PointOfInterest;
                if (Target != null)
                {
                    Vector3 dir = Target.position - center;
                    _LengthOffset = dir.magnitude;
                    if (_LengthOffset > MaxTargetOffset)// if PointOfInterest is far than MaxTargetOffset
                    {
                        // keep distance of  PointOfInterest lower than MaxTargetOffset
                        center += dir.normalized * (_LengthOffset - MaxTargetOffset);
                        _LengthOffset = MaxTargetOffset;
                    }
                    // set offset
                    _Offset.Target = center - Target.position;
                }
                else
                {
                    _LengthOffset = 0;
                    _Offset.Target = Vector3.zero;
                }
                _Offset.Update(OffsetSmoothing);

                // make sure zoom factors are in correct range
                if (ZoomIn < 0) ZoomIn = 0;
                if (ZoomOut < ZoomIn) ZoomOut = ZoomIn;

                float zoomFactor = _LengthOffset / Mathf.Max(0.01f, MaxTargetOffset);
                _Zoom.Target = Mathf.Lerp(ZoomIn, ZoomOut, zoomFactor);
                _Zoom.Update(ZoomSmoothing);

                // update position of camera
                UpdateCamera();
            }

            base.Update();
        }

        /// <summary>
        /// Allow subclass to handle mouse position in different way
        /// </summary>
        /// <returns>Position of mouse on screen</returns>
        protected virtual Vector3 HandleMousePosition()
        {
            // On PC, the cursor point is the mouse position
            return Input.mousePosition;
        }

        private void UpdateCamera(bool force = false)
        {
            float radianLookAngle = LookAngle * Mathf.Deg2Rad;
            float zDistace = Mathf.Cos(radianLookAngle) * Zoom;
            float yDistace = Mathf.Sin(radianLookAngle) * Zoom;

            _ScreenMovementSpace = Quaternion.Euler(0, AroundAngle, 0);
            ScreenForward = _ScreenMovementSpace * Vector3.forward;
            ScreenRight = _ScreenMovementSpace * Vector3.right;

            //_MovementPlane.normal = Player.Character.up;
            _MovementPlane.distance = -Target.position.y + CursorPlaneHeight;

            CursorScreenPosition = HandleMousePosition();

            // Find out where the mouse ray intersects with the movement plane of the player
            CursorWorldPosition = ScreenPointToWorldPointOnPlane(CursorScreenPosition, _MovementPlane, Camera);

            Vector3 cameraAdjustmentVector = Vector3.zero;

            if (_LengthOffset < CameraPreview)
            {
                float halfWidth = Screen.width / 2.0f;
                float halfHeight = Screen.height / 2.0f;
                float maxHalf = Mathf.Max(halfWidth, halfHeight);

                // Acquire the relative screen position			
                Vector3 posRel = CursorScreenPosition - new Vector3(halfWidth, halfHeight, CursorScreenPosition.z);
                posRel.x /= maxHalf;
                posRel.y /= maxHalf;

                // used to adjust the camera based on cursor or joystick position
                cameraAdjustmentVector = posRel.x * ScreenRight + posRel.y * ScreenForward;
                cameraAdjustmentVector.y = 0.0f;
            }

            Camera.fov = Fov;

            Vector3 finalPosition = _CameraTransform.position;
            // HANDLE CAMERA POSITION
            Vector3 offsetToCenter = -zDistace * ScreenForward;
            offsetToCenter.y = yDistace;
            // Set the target position of the camera to point at the focus point

            Vector3 finalOffset = _Offset.Current + offsetToCenter + (cameraAdjustmentVector * CameraPreview);

            finalPosition = Target.position + finalOffset + StaticOffset;

            // speed up smoothing if target moves fast
            float moveDistance = Vector3.Distance(finalPosition, _CameraTransform.position);
            float speedTime = Mathf.Max(0.01f, 1.0f - Mathf.InverseLerp(MinReachFactor, MaxReachFactor, moveDistance));
            if (!force)
            {
                switch (MovementSmoothing.SmoothType)
                {
                    case SmoothType.Damp:
                        finalPosition = Vector3.SmoothDamp(_CameraTransform.position, finalPosition, ref _CameraVelocity, MovementSmoothing.SmoothTime);
                        break;
                    case SmoothType.DampSpeed:
                        finalPosition = Vector3.SmoothDamp(_CameraTransform.position, finalPosition, ref _CameraVelocity, MovementSmoothing.SmoothTime, MovementSmoothing.MaxSpeed);
                        break;
                    case SmoothType.DampSpeedAndTime:
                        finalPosition = Vector3.SmoothDamp(_CameraTransform.position, finalPosition, ref _CameraVelocity, MovementSmoothing.SmoothTime, MovementSmoothing.MaxSpeed, Time.deltaTime * MovementSmoothing.DeltaTimeFactor);
                        break;
                }

            }

            Vector3 look = -offsetToCenter.normalized;
            Quaternion finalRotation = Quaternion.LookRotation(look); // look at target

            // Apply some smoothing to the camera movement
            _CameraTransform.position = finalPosition;
            _CameraTransform.rotation = finalRotation;
        }


        /// <summary>
        /// Calculate intersect position of a ray from camera at screenPoint to a plane in world
        /// </summary>
        /// <param name="screenPoint">Position of point in screen space (mouse position)</param>
        /// <param name="plane">Plane in world space</param>
        /// <param name="camera">Camera to create ray</param>
        /// <returns> Intersection poitn of ray and plane </returns>
        public static Vector3 ScreenPointToWorldPointOnPlane(Vector3 screenPoint, Plane plane, Camera camera)
        {
            // Set up a ray corresponding to the screen position
            Ray ray = camera.ScreenPointToRay(screenPoint);

            // Find out where the ray intersects with the plane
            return PlaneRayIntersection(plane, ray);
        }

        /// <summary>
        /// Find out where the ray intersects with the plane
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="ray">Ray</param>
        /// <returns>Intersection poitn of ray and plane</returns>
        public static Vector3 PlaneRayIntersection(Plane plane, Ray ray)
        {
            float dist;
            plane.Raycast(ray, out dist);
            return ray.GetPoint(dist);
        }
    }
}
