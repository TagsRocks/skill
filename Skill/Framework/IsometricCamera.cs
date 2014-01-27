using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Isometric camera to view target from above. Add this component to 'UnityEngine.Camera' GameObject and set Target to player or a movable object.
    /// </summary>    
    [RequireComponent(typeof(Camera))]
    public class IsometricCamera : DynamicBehaviour
    {
        /// <summary> Target to follow </summary>
        public Transform Target;
        /// <summary> How to damp movement of camera when following target</summary>
        public float Damping = 2.0f;
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
        public float MaxOffset = 6;
        /// <summary> Field of view </summary>
        public float Fov = 60;
        /// <summary> height of cursor plane above target </summary>
        public float CursorPlaneHeight = 0;
        /// <summary> If target can move very fast do not allow camera to loose it</summary>
        public bool FastTarget;

        /// <summary>
        /// Apply relative custom offset to position of camera
        /// </summary>
        public Vector3 CustomOffset = Vector3.zero;

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
        public float Zoom { get { return _Zoom; } }
        /// <summary> It can be center of all enemies around player </summary>
        public Vector3? PointOfInterest { get; set; }
        /// <summary>  Camera screen space </summary>
        public Quaternion ScreenSpace { get { return _ScreenMovementSpace; } }


        // Private memeber data                
        private Transform _CameraTransform;

        private Quaternion _ScreenMovementSpace;
        private Plane _MovementPlane;

        private Vector3 _Offset;
        private float _Zoom;
        private float _LengthOffset;

        private float _PreAroundAngle, _PreZoom, _PreLookAngle;
        private float _ZDistace, _YDistace;

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _PreZoom = -1;
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
            _MovementPlane = new Plane(Vector3.up, Vector3.zero);

            // make sure zoom factors are in correct range
            if (ZoomIn < 0) ZoomIn = 0;
            if (ZoomOut < ZoomIn) ZoomOut = ZoomIn;

            this._Zoom = (ZoomIn + ZoomOut) * 0.5f;

            UpdateCamera(true);
        }

        /// <summary>
        /// LateUpdate
        /// </summary>
        protected virtual void LateUpdate()
        {
            if (!Global.IsGamePaused && !Global.CutSceneEnable) // do not modify camera if game is in cutscene mode
            {
                if (Target != null)
                {
                    if (PointOfInterest != null)
                    {
                        Vector3 center = PointOfInterest.Value;
                        Vector3 dir = Target.position - center;
                        _LengthOffset = dir.magnitude;
                        if (_LengthOffset > MaxOffset)// if PointOfInterest is far than MaxTargetOffset
                        {
                            // keep distance of  PointOfInterest lower than MaxTargetOffset
                            center += dir.normalized * (_LengthOffset - MaxOffset);
                            _LengthOffset = MaxOffset;
                        }
                        // set offset
                        _Offset = center - Target.position;
                    }
                    else
                    {
                        _Offset = Vector3.zero;
                        _LengthOffset = 0;
                    }
                }
                else
                {
                    _LengthOffset = 0;
                    _Offset = Vector3.zero;
                }

                // make sure zoom factors are in correct range
                if (ZoomIn < 0) ZoomIn = 0;
                if (ZoomOut < ZoomIn) ZoomOut = ZoomIn;

                float zoomFactor = _LengthOffset / Mathf.Max(0.01f, MaxOffset);
                _Zoom = Mathf.Lerp(ZoomIn, ZoomOut, zoomFactor);

                // update position of camera
                UpdateCamera();
            }
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



        private void UpdateScreenSpace()
        {
            if (_PreLookAngle != LookAngle || _PreZoom != Zoom || _PreAroundAngle != AroundAngle)
            {
                _PreLookAngle = LookAngle;
                _PreZoom = Zoom;
                _PreAroundAngle = AroundAngle;

                float radianLookAngle = LookAngle * Mathf.Deg2Rad;
                _ZDistace = Mathf.Cos(radianLookAngle) * Zoom;
                _YDistace = Mathf.Sin(radianLookAngle) * Zoom;

                _ScreenMovementSpace = Quaternion.Euler(0, AroundAngle, 0);
                ScreenForward = _ScreenMovementSpace * Vector3.forward;
                ScreenRight = _ScreenMovementSpace * Vector3.right;
            }
        }

        private void UpdateCamera(bool force = false)
        {
            if (Target != null)
            {
                UpdateScreenSpace();

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

                Camera.fieldOfView = Fov;

                // HANDLE CAMERA POSITION
                Vector3 offsetToCenter = -_ZDistace * ScreenForward;
                offsetToCenter.y = _YDistace;
                // Set the target position of the camera to point at the focus point

                Vector3 finalOffset = _Offset + offsetToCenter + (cameraAdjustmentVector * CameraPreview) + (_ScreenMovementSpace * CustomOffset);
                Vector3 finalPosition = Target.position + finalOffset;

                if (!force)
                    finalPosition = Vector3.Lerp(_CameraTransform.position, finalPosition, Damping * Time.deltaTime);

                if (FastTarget)
                {
                    // speed up smoothing if target moves fast
                    Vector3 dir = _CameraTransform.position - finalPosition;
                    float distance = dir.magnitude;
                    if (distance > MaxOffset)
                    {
                        finalPosition += dir.normalized * MaxOffset;
                        _CameraTransform.position = finalPosition;
                    }
                }

                Vector3 look = -offsetToCenter.normalized;
                Quaternion finalRotation = Quaternion.LookRotation(look); // look at target

                // Apply some smoothing to the camera movement
                _CameraTransform.position = finalPosition;
                _CameraTransform.rotation = finalRotation;
            }
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
