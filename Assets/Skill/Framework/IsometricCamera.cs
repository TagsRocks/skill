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
        /// <summary> How to damp movement of camera</summary>
        public float Damping = 2.0f;
        /// <summary> How to damp offset of camera</summary>
        public float OffsetDamping = 1.0f;
        /// <summary> How to damp zooming of camera</summary>
        public float ZoomDamping = 3.0f;
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
        /// <summary> raycast world </summary>
        public LayerMask WorldRayastLayerMask;

        /// <summary>
        /// Apply relative custom offset to position of camera
        /// </summary>
        public Vector3 CustomOffset = Vector3.zero;

        /// <summary> Prepare a cursor point variable. This is the mouse position on PC and controlled by the thumbstick on mobiles. </summary>
        public Vector3 CursorScreenPosition { get; private set; }

        /// <summary> Position of cursur in world on CursorPlane </summary>
        public Vector3 CursorPlanePosition { get; private set; }

        /// <summary> Position of cursur in world </summary>
        public Vector3 RaycastWorldPosition { get; private set; }
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

        /// <summary> ray to world in cursor screen position </summary>
        public Ray CursorScreenRay { get; private set; }

        /// <summary> Temporarily ignore CustomOffset </summary>
        /// <remarks> you want ignore CustomOffset but do not lose previous value </remarks>
        public bool IgnoreCustomOffset { get; set; }

        // Private memeber data                
        private CameraLimit[] _Limits;
        private Transform _CameraTransform;
        private Transform _PreTarget;

        private Quaternion _ScreenMovementSpace;
        private Plane _MovementPlane;

        private Vector3? _PrePosition;
        private Vector3 _Offset;
        private Vector3 _SomeOFOffsets;
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

        /// <summary>
        /// Get required references
        /// </summary>
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

            _Limits = GetComponents<CameraLimit>();
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
            if (!Global.IsGamePaused) // do not modify camera if game is paused
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
                _Zoom = Mathf.Lerp(_Zoom, Mathf.Lerp(ZoomIn, ZoomOut, zoomFactor), ZoomDamping * Time.deltaTime);

                // update position of camera
                UpdateCamera();
            }
        }

        protected override void Update()
        {
            CursorScreenPosition = HandleMousePosition();
            if (!Global.IsGamePaused)
            {
                CursorScreenRay = this.Camera.ScreenPointToRay(CursorScreenPosition);
                // Set up a ray corresponding to the screen position            
                RaycastHit hit;
                if (Physics.Raycast(CursorScreenRay, out hit, float.MaxValue, this.WorldRayastLayerMask.value))
                    RaycastWorldPosition = hit.point;
                else
                    RaycastWorldPosition = CursorScreenRay.origin + (CursorScreenRay.direction * Camera.farClipPlane);
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

                Vector3 pos;
                // Find out where the mouse ray intersects with the movement plane of the player                
                if (PlaneRayIntersection(this._MovementPlane, CursorScreenRay, out pos))
                    CursorPlanePosition = pos;
                else
                    CursorPlanePosition = RaycastWorldPosition;

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

                Vector3 off = _Offset;
                if (!IgnoreCustomOffset)
                    off += (_ScreenMovementSpace * CustomOffset);
                _SomeOFOffsets = Vector3.Lerp(_SomeOFOffsets, off, OffsetDamping * Time.deltaTime);
                Vector3 finalOffset = _SomeOFOffsets + offsetToCenter + (cameraAdjustmentVector * CameraPreview);
                Vector3 finalPosition = Target.position + finalOffset;

                if (_PreTarget != Target)
                    _PrePosition = null;
                // apply camera limits
                if (_Limits != null && _Limits.Length > 0 && _PrePosition != null)
                {
                    for (int i = 0; i < _Limits.Length; i++)
                        _Limits[i].ApplyLimit(ref finalPosition, _PrePosition.Value);
                }

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

                _PrePosition = finalPosition;
            }
            _PreTarget = Target;
        }

        /// <summary>
        /// Find out where the ray intersects with the plane
        /// </summary>
        /// <param name="plane">Plane</param>
        /// <param name="ray">Ray</param>        
        /// <param name="intersectPosition">Intersection point of ray and plane</param>
        /// <returns>true if intersection occurs, otherwise false</returns>
        public static bool PlaneRayIntersection(Plane plane, Ray ray, out Vector3 intersectPosition)
        {
            float dist;
            if (plane.Raycast(ray, out dist))
            {
                intersectPosition = ray.GetPoint(dist);
                return true;
            }
            else
            {
                intersectPosition = Vector3.zero;
                return false;
            }
        }
    }
}
