using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Studio.Diagram;
using System.Windows;

namespace Skill.Studio.Animation
{
    public class AnimationTreeCanvas : DiagramCanvas
    {
        public AnimationTreeCanvas()
        {
            base.Selection.Change += new EventHandler(Selection_Change);
        }

        void Selection_Change(object sender, EventArgs e)
        {
            if (Selection.CurrentSelection.Count == 1)
            {
                AnimNodeViewModel vm = Selection.CurrentSelection[0] as AnimNodeViewModel;
                if (vm != null)
                    System.Windows.Input.ApplicationCommands.Properties.Execute(vm.Properties, null);
                else
                    System.Windows.Input.ApplicationCommands.Properties.Execute(null, null);
            }
            else
            {
                //System.Windows.Input.ApplicationCommands.Properties.Execute(Selection.CurrentSelection, null);
                System.Windows.Input.ApplicationCommands.Properties.Execute(null, null);
            }
        }


        public AnimNodeViewModel FindById(int id)
        {
            foreach (var item in Children)
            {
                if (item is AnimNodeViewModel)
                {
                    AnimNodeViewModel vm = (AnimNodeViewModel)item;
                    if (vm.Model.Id == id) return vm;
                }
            }
            return null;
        }

        public AnimationTreeEditor Editor
        {
            get { return (AnimationTreeEditor)GetValue(EditorProperty); }
            set { SetValue(EditorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Editor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditorProperty =
            DependencyProperty.Register("Editor", typeof(AnimationTreeEditor), typeof(AnimationTreeCanvas), new UIPropertyMetadata(null));        
    }
}
