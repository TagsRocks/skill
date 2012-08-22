using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Skill.Studio.AI.Editor
{
    class BehaviorBrushes
    {
        private static Brush _SelectedBrush;
        public static Brush SelectedBrush
        {
            get
            {
                if (_SelectedBrush == null) _SelectedBrush = Brushes.Purple;
                return _SelectedBrush;
            }
        }

        private static Brush _EnableBrush;
        public static Brush EnableBrush
        {
            get
            {
                if (_EnableBrush == null) _EnableBrush = Brushes.LightGreen;
                return _EnableBrush;
            }
        }

        private static Brush _DisableBrush;
        public static Brush DisableBrush
        {
            get
            {
                if (_DisableBrush == null) _DisableBrush = Brushes.Salmon;
                return _DisableBrush;
            }
        }

        private static Brush _FailedBrush;
        public static Brush FailedBrush
        {
            get
            {
                if (_FailedBrush == null) _FailedBrush = Brushes.Red;
                return _FailedBrush;
            }
        }

        private static Brush _SuccessBrush;
        public static Brush SuccessBrush
        {
            get
            {
                if (_SuccessBrush == null) _SuccessBrush = Brushes.Green;
                return _SuccessBrush;
            }
        }

        private static Brush _RunningBrush;
        public static Brush RunningBrush
        {
            get
            {
                if (_RunningBrush == null) _RunningBrush = Brushes.Yellow;
                return _RunningBrush;
            }
        }

        private static Brush _DefaultBackBrush;
        public static Brush DefaultBackBrush
        {
            get
            {
                if (_DefaultBackBrush == null)
                {
                    LinearGradientBrush lg = new LinearGradientBrush()
                    {
                        StartPoint = new System.Windows.Point(0.5, 0),
                        EndPoint = new System.Windows.Point(0.5, 1)
                    };
                    lg.GradientStops.Add(new GradientStop(Color.FromRgb(200, 200, 200), 0));
                    lg.GradientStops.Add(new GradientStop(Colors.Gray, 1));
                    _DefaultBackBrush = lg;
                }
                return _DefaultBackBrush;
            }
        }

        private static Brush _DefaultBorderBrush;
        public static Brush DefaultBorderBrush
        {
            get
            {
                if (_DefaultBorderBrush == null)
                {
                    _DefaultBorderBrush = Brushes.Gray;
                }
                return _DefaultBorderBrush;
            }
        }
    }
}
