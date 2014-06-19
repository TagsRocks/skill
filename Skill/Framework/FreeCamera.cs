using UnityEngine;
using System.Collections;

namespace Skill.Framework
{
    /// <summary>
    /// WASD  : Movement
    /// Shift : Makes camera Accelerate
    /// Space : Moves camera on X and Z axis only.  So camera doesn't gain any height
    /// 1,2,3 : Switch between speeds
    /// </summary>
    public class FreeCamera : Skill.Framework.DynamicBehaviour
    {
        /// <summary> speed 1</summary>
        public float Speed1 = 10.0f;
        /// <summary> speed 2</summary>
        public float Speed2 = 25.0f;
        /// <summary> speed 3</summary>
        public float Speed3 = 50.0f;
        /// <summary> Multiplied by how long shift is held.  Basically running </summary>
        public float ShiftFactor = 1.5f;
        /// <summary> Maximum speed when holdin shift </summary>
        public float MaxSpeed = 100.0f;
        /// <summary> How sensitive it with mouse </summary>
        public float MouseSensitive = 0.2f;

        private Vector3 _LastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
        private float _TotalRun = 1.0f;
        private float _Speed;

        protected override void Start()
        {
            base.Start();
            _Speed = Speed1;
        }

        protected override void Update()
        {
            SelectSpeed();

            if (Input.GetMouseButtonDown(1))
                _LastMouse = Input.mousePosition;
            if (Input.GetMouseButton(1))
            {
                _LastMouse = Input.mousePosition - _LastMouse;
                _LastMouse = new Vector3(-_LastMouse.y * MouseSensitive, _LastMouse.x * MouseSensitive, 0);
                _LastMouse = new Vector3(transform.eulerAngles.x + _LastMouse.x, transform.eulerAngles.y + _LastMouse.y, 0);
                transform.eulerAngles = _LastMouse;
                _LastMouse = Input.mousePosition;
            }
            Vector3 velocity = GetInputVelocity();

            if (Input.GetKey(KeyCode.LeftShift))
            {
                _TotalRun += Time.deltaTime;
                velocity *= ShiftFactor;
            }
            else
            {
                _TotalRun = 1.0f;
            }
            velocity *= _TotalRun * _Speed * Time.deltaTime;
            velocity.x = Mathf.Clamp(velocity.x, -MaxSpeed, MaxSpeed);
            velocity.y = Mathf.Clamp(velocity.y, -MaxSpeed, MaxSpeed);
            velocity.z = Mathf.Clamp(velocity.z, -MaxSpeed, MaxSpeed);
            Vector3 newPosition = transform.position;
            if (Input.GetKey(KeyCode.Space)) //If player wants to move on X and Z axis only
            {
                transform.Translate(velocity);
                newPosition.x = transform.position.x;
                newPosition.z = transform.position.z;
                transform.position = newPosition;
            }
            else
            {
                transform.Translate(velocity);
            }
            base.Update();
        }

        private void SelectSpeed()
        {
            if (Input.GetKeyUp(KeyCode.Alpha1))
                _Speed = Speed1;
            else if (Input.GetKeyUp(KeyCode.Alpha2))
                _Speed = Speed2;
            else if (Input.GetKeyUp(KeyCode.Alpha3))
                _Speed = Speed3;
        }

        private Vector3 GetInputVelocity()
        {
            //returns the basic values, if it's 0 than it's not active.
            Vector3 velocity = new Vector3();
            if (Input.GetKey(KeyCode.W))
            {
                velocity += new Vector3(0, 0, 1);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                velocity += new Vector3(0, 0, -1);
            }
            if (Input.GetKey(KeyCode.A))
            {
                velocity += new Vector3(-1, 0, 0);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                velocity += new Vector3(1, 0, 0);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                velocity += new Vector3(0, -1, 0);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                velocity += new Vector3(0, 1, 0);
            }
            return velocity;
        }
    }

}