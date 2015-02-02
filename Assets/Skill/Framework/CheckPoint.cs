using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Skill.Framework
{
    /// <summary>
    /// Base class for checkPoint
    /// </summary>
    /// <remarks>
    /// usually checkpoint is activate and deactivate some objects and move some objects to correct place, but probably you have to inherite from this class
    /// and do some additional works that is specific for your game in Apply method.
    /// </remarks>
    public class CheckPoint : StaticBehaviour, IComparer<CheckPoint>
    {
        /// <summary> Data required to move an object </summary>
        [Serializable]
        public class MoveTo
        {
            /// <summary> Optional name </summary>
            public string Name;
            /// <summary> Object to move </summary>
            public GameObject Object;
            /// <summary> Destination of move</summary>
            public Transform Destination;
            /// <summary> apply rotation of destination to object? </summary>
            public bool UseRotation;

            /// <summary> Move object to destination </summary>
            public void Move()
            {
                if (this.Object != null && this.Destination != null)
                {
                    this.Object.transform.position = this.Destination.position;
                    if (this.UseRotation)
                        this.Object.transform.rotation = this.Destination.rotation;
                }
            }
        }

        /// <summary> Index of CheckPoint </summary>
        public int Index = 0;
        /// <summary> Objects to move </summary>
        public MoveTo[] Moves;
        /// <summary> Objects to deactivate </summary>
        public GameObject[] Deactivates;        
        /// <summary> Objects to activate </summary>
        public GameObject[] Activates;



        /// <summary> Apply Checkpoint changes</summary>
        public virtual void Apply()
        {
            if (Moves != null && Moves.Length > 0)
            {
                foreach (var m in Moves)
                {
                    if (m != null)
                        m.Move();
                }
            }
            if (Deactivates != null && Deactivates.Length > 0) Utility.Activate(Deactivates, false);
            if (Activates != null && Activates.Length > 0) Utility.Activate(Activates, true);            
            
        }

        /// <summary>
        /// Compare two CheckPoint by index
        /// </summary>
        /// <param name="x">CheckPoint x</param>
        /// <param name="y">CheckPoint y</param>
        /// <returns> compare result</returns>
        public int Compare(CheckPoint x, CheckPoint y)
        {
            return x.Index.CompareTo(y.Index);
        }

        /// <summary>
        /// Apply CheckPoint
        /// </summary>
        /// <param name="checkPointIndex">Index if CheckPoint to apply</param>
        public static void Apply(int checkPointIndex)
        {
            bool found = false;
            CheckPoint[] cps = FindObjectsOfType<CheckPoint>();
            if (cps != null)
            {
                if (cps.Length > 1)
                    Utility.QuickSort(cps, cps[0]);
                for (int i = 0; i < cps.Length; i++)
                {
                    if (cps[i].Index == checkPointIndex)
                    {
                        found = true;
                        cps[i].Apply();
                        break;
                    }
                }
            }
            if (!found)
                Debug.LogWarning(string.Format("Could not find CheckPoint with index of {0} to apply", checkPointIndex));
        }
    }
}
