using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Skill.Framework.Managers;

namespace Skill.Framework
{
    [AddComponentMenu("Skill/Base/SpawnQueue")]
    public class SpawnQueue : DynamicBehaviour
    {
        public float Interval = 3.0f;
        public int Count;

        private Queue<GameObject> _Queue;
        private TimeWatch _IntervalTW;

        protected override void Awake()
        {
            base.Awake();
            _Queue = new Queue<GameObject>();
        }

        public void Enqueue(GameObject spawnedObject)
        {
            if (spawnedObject != null)
            {
                spawnedObject.SetActive(false);
                _Queue.Enqueue(spawnedObject);

                Count = _Queue.Count;
            }
        }


        protected override void Update()
        {
            if (_Queue.Count > 0)
            {
                if (_IntervalTW.IsOver)
                {
                    GameObject obj = _Queue.Dequeue();
                    obj.SetActive(true);
                    _IntervalTW.Begin(Interval);
                }
                Count = _Queue.Count;
            }
            base.Update();
        }
    }
}
