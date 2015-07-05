using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Skill.Framework;

namespace Skill.Framework.Audio
{
    public class AudioController : Skill.Framework.DynamicBehaviour
    {
        [HideInInspector]
        [SerializeField]
        public AudioState[] States = null;
        [HideInInspector]
        [SerializeField]
        public string[] Triggers = null;
        [HideInInspector]
        [SerializeField]
        public string DefaultState = null;
        [HideInInspector]
        [SerializeField]
        public float Zoom = 1.0f;
        [HideInInspector]
        [SerializeField]
        public float PanX = 0;
        [HideInInspector]
        [SerializeField]
        public float PanY = 0;


        public float VolumeFactor = 1.0f;

        public class Trigger
        {
            public string Name { get; private set; }
            public bool IsActive { get; private set; }
            public void Set() { IsActive = true; }
            public void Reset() { IsActive = false; }

            internal Trigger(string name)
            {
                this.Name = name;
            }
        }

        public Trigger this[string triggerName]
        {
            get
            {
                Trigger trigger;
                if (_TriggersMap.TryGetValue(triggerName, out trigger))
                    return trigger;
                return null;
            }
        }

        private Dictionary<string, Trigger> _TriggersMap;
        private AudioState _CurrentState;
        private AudioState _NextState;
        private TimeWatch _NextStateTW;
        private bool _InTransition;
        private AudioTransition _ActiveTransition;
        private TimeWatch _UpdateNextStateTW;
        private TimeWatch _ScheduleNextStateTW;

        public AudioState CurrentState { get { return _CurrentState; } }
        protected override void Awake()
        {
            base.Awake();
            _TriggersMap = new Dictionary<string, Trigger>();
            if (Triggers == null) Triggers = new string[0];
            foreach (var t in Triggers)
                _TriggersMap.Add(t, new Trigger(t));

            foreach (var s in States)
            {
                s.Validate(gameObject.AddComponent<AudioSource>());
                if (s.Transitions != null)
                {
                    foreach (var t in s.Transitions)
                        t.TriggerRef = this[t.Trigger];
                }
            }

            _CurrentState = FindState(DefaultState);
            if (_CurrentState == null)
                Debug.LogError("Default state not found");
        }

        private AudioState FindState(string stateName)
        {
            if (stateName == null) return null;
            if (States != null)
            {
                foreach (var s in States)
                {
                    if (s.Name == stateName)
                        return s;
                }
            }
            return null;
        }

        protected override void Start()
        {
            base.Start();
            Skill.Framework.Audio.PitchListener listener = GetComponent<Skill.Framework.Audio.PitchListener>();
            if (listener != null)
                listener.RelinkAudios();

            _InTransition = false;
            _NextState = null;
            _ScheduleNextStateTW.End();
            if (_CurrentState != null)
            {
                if (_CurrentState.Clip != null)
                {
                    _CurrentState.Initialize(false);
                    _CurrentState.StartTime(0, 0);
                }

                if (_CurrentState.NextState != null)
                {
                    float deltaTime = _CurrentState.End - _CurrentState.Begin;
                    if (deltaTime > 1)
                        _ScheduleNextStateTW.Begin(deltaTime);
                    else
                        ScheduleNextState(deltaTime);
                }
            }
        }

        protected override void Update()
        {
            base.Update();
            if (_NextStateTW.IsEnabled)
            {
                if (_NextState == null)
                {
                    _NextStateTW.End();
                    if (_ActiveTransition != null)
                    {
                        if (_ActiveTransition.TriggerRef != null)
                            _ActiveTransition.TriggerRef.Reset();
                        _ActiveTransition = null;
                    }
                }
                if (_NextStateTW.IsEnabledAndOver)
                {
                    _UpdateNextStateTW.End();
                    _ScheduleNextStateTW.End();
                    _NextStateTW.End();
                    if (_ActiveTransition != null)
                    {
                        if (_ActiveTransition.TriggerRef != null)
                            _ActiveTransition.TriggerRef.Reset();
                        _ActiveTransition = null;
                    }

                    _CurrentState = _NextState;
                    _InTransition = false;
                    _NextState = null;

                    if (_CurrentState.NextState != null)
                    {
                        float deltaTime = _CurrentState.End - _CurrentState.Begin;
                        if (deltaTime > 1)
                            _ScheduleNextStateTW.Begin(deltaTime);
                        else
                            ScheduleNextState(deltaTime);
                    }
                }
            }
            else if (_ScheduleNextStateTW.IsEnabled)
            {
                if (_ScheduleNextStateTW.TimeLeft < 2)
                {
                    ScheduleNextState(_ScheduleNextStateTW.TimeLeft);
                    _ScheduleNextStateTW.End();
                }
            }

            if (_CurrentState != null)
            {
                _CurrentState.Update(this.VolumeFactor);
                if (!_InTransition)
                {
                    if (_CurrentState.Transitions != null)
                    {
                        foreach (var t in _CurrentState.Transitions)
                        {
                            if (t.TriggerRef != null)
                            {
                                if (t.TriggerRef.IsActive)
                                {
                                    TransitBy(t);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (_UpdateNextStateTW.IsEnabledAndOver && _NextState != null)
                _NextState.Update(this.VolumeFactor);
        }

        private void ScheduleNextState(float deltaTime)
        {
            if (_CurrentState != null)
            {
                _CurrentState.EndTime(deltaTime, 0);
                _NextState = FindState(_CurrentState.NextState);
                if (_NextState != null)
                {
                    _NextStateTW.Begin(deltaTime);
                    if (_NextState.Clip != null)
                    {
                        _NextState.Initialize(false);
                        _NextState.StartTime(deltaTime, 0);
                    }
                }
            }
        }
        private void TransitBy(AudioTransition transition)
        {
            if (transition.Destination != null)
            {
                _ActiveTransition = transition;

                float deltaTime = 0;
                bool isPlaying = _CurrentState.IsPlaying;
                if (isPlaying)
                {
                    float time = _CurrentState.AudioTime;
                    if (_ActiveTransition.WaitForBreakPoint)
                    {
                        float breakPoint = FindNextBreakPoint(time + _ActiveTransition.FadeOut, _CurrentState.BreakPoints, _CurrentState.End);
                        deltaTime = breakPoint - time;
                    }
                    else
                    {
                        deltaTime = Mathf.Min(_ActiveTransition.FadeOut, _CurrentState.End - time);
                    }
                }

                if (_NextState != null)
                    _NextState.Stop();

                _InTransition = true;
                _NextState = FindState(transition.Destination);
                _NextStateTW.Begin(deltaTime);

                if (isPlaying)
                    _CurrentState.EndTime(deltaTime, _ActiveTransition.FadeOut);

                if (_NextState.Clip != null)
                {
                    _NextState.Initialize(_ActiveTransition.FadeIn > AudioState.MINFADETIME);
                    if (_ActiveTransition.CrossFade)
                    {
                        _UpdateNextStateTW.Begin(Mathf.Max(0, deltaTime - _ActiveTransition.FadeIn));
                        _NextState.StartTime(_UpdateNextStateTW.Length, _ActiveTransition.FadeIn);
                    }
                    else
                        _NextState.StartTime(deltaTime, _ActiveTransition.FadeIn);
                }
            }
        }
        private float FindNextBreakPoint(float time, float[] breakPoints, float end)
        {
            if (breakPoints != null)
            {
                for (int i = 0; i < breakPoints.Length; i++)
                {
                    if (breakPoints[i] >= time)
                        return breakPoints[i];
                }
            }
            return end;
        }
        public void Stop()
        {
            if (_NextState != null)
                _NextState.Stop();
            if (_CurrentState != null)
                _CurrentState.Stop();

            foreach (var t in _TriggersMap.Values)
                t.Reset();

            _NextStateTW.End();
            _ActiveTransition = null;
            _NextState = null;
            _CurrentState = FindState(DefaultState);
        }
        public void ResetAllTriggers()
        {
            foreach (var t in _TriggersMap.Values)
                t.Reset();
        }

        public void FadeTo(string stateName, float fadeIn, float fadeOut, bool crossFade = true)
        {
            AudioState state = FindState(stateName);
            if (state != null && state != _CurrentState)
            {
                AudioTransition at = new AudioTransition();
                at.Destination = stateName;
                at.FadeIn = Mathf.Max(fadeIn, 0);
                at.FadeOut = Mathf.Max(fadeOut, 0);
                at.WaitForBreakPoint = false;
                at.CrossFade = crossFade;
                TransitBy(at);
            }
        }
    }
}