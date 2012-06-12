using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Skill.Studio.Animation.Editor
{
    public static class StaticBrushes
    {

        private static Brush _AnimSequenceContnetBrush;
        public static Brush AnimSequenceContnetBrush
        {
            get
            {
                if (_AnimSequenceContnetBrush == null)
                {
                    _AnimSequenceContnetBrush = FindBrush("AnimSequenceContnetBrush");
                    if (_AnimSequenceContnetBrush == null)
                        _AnimSequenceContnetBrush = new SolidColorBrush(Color.FromRgb(0x61, 0x32, 0x31));
                }
                return _AnimSequenceContnetBrush;
            }
        }


        private static Brush _AnimDirectionContnetBrush;
        public static Brush AnimDirectionContnetBrush
        {
            get
            {
                if (_AnimDirectionContnetBrush == null)
                {
                    _AnimDirectionContnetBrush = FindBrush("AnimDirectionContnetBrush");
                    if (_AnimDirectionContnetBrush == null)
                        _AnimDirectionContnetBrush = new SolidColorBrush(Color.FromRgb(0x00, 0x54, 0xC6));
                }
                return _AnimDirectionContnetBrush;
            }
        }


        private static Brush _AnimOtherContnetBrush;
        public static Brush AnimOtherContnetBrush
        {
            get
            {
                if (_AnimOtherContnetBrush == null)
                {
                    _AnimOtherContnetBrush = FindBrush("AnimOtherContnetBrush");
                    if (_AnimOtherContnetBrush == null)
                        _AnimOtherContnetBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0x03, 0xA3));
                }
                return _AnimOtherContnetBrush;
            }
        }


        private static Brush _AnimBlendContnetBrush;
        public static Brush AnimBlendContnetBrush
        {
            get
            {
                if (_AnimBlendContnetBrush == null)
                {
                    _AnimBlendContnetBrush = FindBrush("AnimBlendContnetBrush");
                    if (_AnimBlendContnetBrush == null)
                        _AnimBlendContnetBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0x42, 0x00));
                }
                return _AnimBlendContnetBrush;
            }
        }


        private static Brush _AnimRandomContnetBrush;
        public static Brush AnimRandomContnetBrush
        {
            get
            {
                if (_AnimRandomContnetBrush == null)
                {
                    _AnimRandomContnetBrush = FindBrush("AnimRandomContnetBrush");
                    if (_AnimRandomContnetBrush == null)
                        _AnimRandomContnetBrush = new SolidColorBrush(Color.FromRgb(0x2F, 0x00, 0x8F));
                }
                return _AnimRandomContnetBrush;
            }
        }


        private static Brush _AnimAdditiveContnetBrush;
        public static Brush AnimAdditiveContnetBrush
        {
            get
            {
                if (_AnimAdditiveContnetBrush == null)
                {
                    _AnimAdditiveContnetBrush = FindBrush("AnimAdditiveContnetBrush");
                    if (_AnimAdditiveContnetBrush == null)
                        _AnimAdditiveContnetBrush = new SolidColorBrush(Color.FromRgb(0x08, 0x73, 0x00));
                }
                return _AnimAdditiveContnetBrush;
            }
        }


        private static Brush _AnimRootContnetBrush;
        public static Brush AnimRootContnetBrush
        {
            get
            {
                if (_AnimRootContnetBrush == null)
                {
                    _AnimRootContnetBrush = FindBrush("AnimRootContnetBrush");
                    if (_AnimRootContnetBrush == null)
                        _AnimRootContnetBrush = new SolidColorBrush(Color.FromRgb(0x43, 0x43, 0x43));
                }
                return _AnimRootContnetBrush;
            }
        }


        private static Brush _AnimSelectedBrush;
        public static Brush AnimSelectedBrush
        {
            get
            {
                if (_AnimSelectedBrush == null)
                {
                    _AnimSelectedBrush = FindBrush("AnimSelectedBrush");
                    if (_AnimSelectedBrush == null)
                        _AnimSelectedBrush = Brushes.Yellow;
                }
                return _AnimSelectedBrush;
            }
        }


        private static Brush _AnimBorderBrush;
        public static Brush AnimBorderBrush
        {
            get
            {
                if (_AnimBorderBrush == null)
                {
                    _AnimBorderBrush = FindBrush("AnimBorderBrush");
                    if (_AnimBorderBrush == null)
                        _AnimBorderBrush = Brushes.Gray;
                }
                return _AnimBorderBrush;
            }
        }

        private static Brush FindBrush(string key)
        {
            return System.Windows.Application.Current.TryFindResource(key) as Brush;
        }
    }
}
