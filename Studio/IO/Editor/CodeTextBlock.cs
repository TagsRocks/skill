using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Skill.Studio.IO.Editor
{
    public class CodeTextBlock : TextBlock
    {

        public string Code
        {
            get { return (string)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Code.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(string), typeof(CodeTextBlock), new UIPropertyMetadata(null, OnCodeChanged));


        private static void OnCodeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CodeTextBlock textBlock = sender as CodeTextBlock;
            textBlock.Inlines.Clear();
            List<Inline> lines = ConvertToCode(e.NewValue as string);
            foreach (var line in lines)
            {
                textBlock.Inlines.Add(line);
            }
        }

        private static List<Inline> ConvertToCode(string code)
        {
            if (code == null)
                code = "";

            string[] parts = code.Split(new char[] { ' ', '<', '>' ,'[',']' }, StringSplitOptions.RemoveEmptyEntries);

            List<Inline> lines = new List<Inline>();

            bool afterClass = false;
            foreach (var str in parts)
            {
                if (str == "class")
                {
                    lines.Add(new Run(str) { Foreground = Brushes.Blue });
                    afterClass = true;
                }
                else if (afterClass)
                {
                    lines.Add(new Run(str) { Foreground = Brushes.DarkGreen });
                    afterClass = false;
                }
                else if (str == "public" || str == "int" || str == "float" || str == "bool" || str == "string")
                    lines.Add(new Run(str) { Foreground = Brushes.Blue });
                else if (str == "Bounds" || str == "Color" || str == "Matrix4x4" || str == "Plane" || str == "Quaternion" || str == "Ray" || str == "Rect" || str == "Vector2" || str == "Vector3" || str == "Vector4")
                    lines.Add(new Run(str) { Foreground = Brushes.DarkGreen });
                else
                    lines.Add(new Run(str));                

                lines.Add(new Run(" "));
            }

            return lines;
        }
    }
}
