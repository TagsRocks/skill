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

            string[] parts = code.Split(new char[] { ' ', '<', '>' }, StringSplitOptions.RemoveEmptyEntries);

            List<Inline> lines = new List<Inline>();

            bool afterClassOrPublic = false;
            foreach (var str in parts)
            {
                Brush foreground = Brushes.Black;

                if (str == "class" || str == "public")
                {
                    foreground = Brushes.Blue;
                    afterClassOrPublic = true;
                }
                else if (afterClassOrPublic)
                {
                    if (str == ClassPropertyViewModel.InvalidClass || str == ClassPropertyViewModel.InvalidClassArray)
                    {
                        foreground = Brushes.Red;
                    }
                    else if (str == "int" || str == "float" || str == "bool" || str == "string" ||
                             str == "int[]" || str == "float[]" || str == "bool[]" || str == "string[]")
                    {
                        foreground = Brushes.Blue;
                    }
                    else
                    {
                        foreground = Brushes.DarkGreen;
                    }
                    afterClassOrPublic = false;
                }

                int bIndex = str.IndexOf('[');
                if (bIndex > 0)
                {
                    lines.Add(new Run(str.Substring(0, bIndex)) { Foreground = foreground });
                    lines.Add(new Run(str.Substring(bIndex)) { Foreground = Brushes.Black });
                }
                else
                {
                    lines.Add(new Run(str) { Foreground = foreground });
                }

                lines.Add(new Run(" "));
            }

            return lines;
        }
    }
}
