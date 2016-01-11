using UnityEngine;
using System.Collections;
using Skill.Framework;
using Skill.Framework.IO;


public class JoyElementResizer : DynamicBehaviour
{
    public float MinSize = 16;
    public JoyElement[] Elements;

    private Skill.Framework.IO.ScaleGestureDetector _ScaleDetector;
    private Skill.Framework.IO.DragGestureDetector _DragDetector;
    private Skill.Framework.IO.TapGestureDetector _TapDetector;

    private TouchState _Touch;
    private Vector2 _StartDrag;
    private Rect _SelectedRenderArea;
    private JoyElement _SelectedElement;
    public JoyElement SelectedElement
    {
        get { return _SelectedElement; }
        private set
        {
            if (_SelectedElement != value)
            {
                _SelectedElement = value;
                _DragDetector.IsEnabled = _SelectedElement != null;

                if (_SelectedElement != null)
                    _SelectedRenderArea = _SelectedElement.RenderArea;
            }
        }
    }

    private void CreateGustures()
    {
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        if (Application.isPlaying && InputManager.Instance == null)
            Debug.LogWarning("You must place an instance of InputManager in scene to make JoyElementResizer works correctly");
#endif

        if (InputManager.Instance != null)
        {
            if (_ScaleDetector == null)
            {
                _ScaleDetector = new ScaleGestureDetector() { FingerCount = 2, LockTouches = true, Priority = ((uint)Elements.Length + 1) };
                _ScaleDetector.Scale += _ScaleDetector_Scale;
                InputManager.Add(_ScaleDetector);
            }
            if (_DragDetector == null)
            {
                _DragDetector = new DragGestureDetector() { FingerCount = 1, LockTouches = true, IsEnabled = false };
                InputManager.Add(_DragDetector);
                _DragDetector.Drag += JoyElementResizer_Drag;
            }

            if (_TapDetector == null)
            {
                _TapDetector = new TapGestureDetector() { FingerCount = 1, LockTouches = false };
                InputManager.Add(_TapDetector);
                _TapDetector.Tap += JoyElementResizer_Tap;
            }
        }
    }

    void _ScaleDetector_Scale(object sender, ScaleGestureEventArgs args)
    {
        if (SelectedElement != null)
        {
            float scale = args.TotalScale;

            Rect ra = _SelectedRenderArea;
            Vector2 cener = ra.center;
            ra.width *= scale;
            ra.height *= scale;

            if (ra.width < MinSize) ra.width = MinSize;
            if (ra.height < MinSize) ra.height = MinSize;

            ra.x = cener.x - ra.width * 0.5f;
            ra.y = cener.y - ra.height * 0.5f;
            SelectedElement.RenderArea = ra;
        }
    }


    void JoyElementResizer_Tap(object sender, TapGestureEventArgs args)
    {
        foreach (var item in Elements)
        {
            if (item != null)
            {
                if (item.RenderArea.Contains(args.Positions[0]))
                {
                    SelectedElement = item;
                    break;
                }
            }
        }
    }
    void JoyElementResizer_Drag(object sender, DragGestureEventArgs args)
    {
        if (SelectedElement != null)
        {
            Rect ra = SelectedElement.RenderArea;
            Vector2 cener = ra.center + args.DeltaTranslation;
            ra.x = cener.x - ra.width * 0.5f;
            ra.y = cener.y - ra.height * 0.5f;
            SelectedElement.RenderArea = ra;
            _SelectedRenderArea = ra;
        }
    }

    protected override void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            if (_ScaleDetector != null)
            {
                InputManager.Remove(_ScaleDetector);
                _ScaleDetector = null;
            }
            if (_DragDetector != null)
            {
                InputManager.Remove(_DragDetector);
                _DragDetector = null;
            }
            if (_TapDetector != null)
            {
                InputManager.Remove(_TapDetector);
                _TapDetector = null;
            }
        }

        base.OnDestroy();
    }


    protected override void OnEnable()
    {
        CreateGustures();
        if (_ScaleDetector != null)
            _ScaleDetector.IsEnabled = true;
        if (_DragDetector != null)
            _DragDetector.IsEnabled = SelectedElement != null;
        if (_TapDetector != null)
            _TapDetector.IsEnabled = true;

        if (Elements != null)
        {
            foreach (var e in Elements)
            {
                if (e != null)
                    e.enabled = false;
            }
        }
        if (SelectedElement == null && Elements != null)
            SelectedElement = Elements[0];
    }

    protected virtual void OnDisable()
    {
        SelectedElement = null;

        if (_ScaleDetector != null)
            _ScaleDetector.IsEnabled = false;
        if (_DragDetector != null)
            _DragDetector.IsEnabled = false;
        if (_TapDetector != null)
            _TapDetector.IsEnabled = false;

        if (Elements != null)
        {
            foreach (var e in Elements)
            {
                if (e != null)
                    e.enabled = true;
            }
        }
    }
}