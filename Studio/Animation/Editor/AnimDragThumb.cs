using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Skill.Studio.Animation.Editor
{
    public class AnimDragThumb : Thumb
    {
        public AnimNodeViewModel ViewModel
        {
            get { return (AnimNodeViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(AnimNodeViewModel), typeof(AnimDragThumb), new UIPropertyMetadata(null));


        public AnimationTreeCanvas ParentCanvas { get { return AnimationTreeCanvas.GetCanvas(this); } }

        public AnimDragThumb()
        {
            base.DragDelta += new DragDeltaEventHandler(DragThumb_DragDelta);
        }

        void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {

            if ((Keyboard.Modifiers & ModifierKeys.Alt) == 0)
            {
                if (ViewModel != null && ViewModel.IsSelected)
                {
                    AnimationTreeCanvas canvas = ParentCanvas;

                    //if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None)
                    //{
                    //    if (!canvas.Editor.IsDuplicating)
                    //        canvas.Editor.DuplicateSelection();
                    //}

                    //if (canvas.Editor.IsDuplicating)
                    //{
                    //    canvas.Editor.MoveDuplicated(e.HorizontalChange, e.VerticalChange);
                    //}


                    double minLeft = double.MaxValue;
                    double minTop = double.MaxValue;

                    foreach (var item in ViewModel.Tree.Editor.Selection.SelectedObjects)
                    {
                        if (item is AnimNodeViewModel)
                        {
                            AnimNodeViewModel vm = item as AnimNodeViewModel;

                            minLeft = double.IsNaN(vm.X) ? 0 : Math.Min(vm.X, minLeft);
                            minTop = double.IsNaN(vm.Y) ? 0 : Math.Min(vm.Y, minTop);
                        }
                    }

                    double deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
                    double deltaVertical = Math.Max(-minTop, e.VerticalChange);

                    foreach (var item in ViewModel.Tree.Editor.Selection.SelectedObjects)
                    {
                        if (item is AnimNodeViewModel)
                        {
                            AnimNodeViewModel vm = item as AnimNodeViewModel;
                            double left = vm.X;
                            double top = vm.Y;

                            if (double.IsNaN(left)) left = 0;
                            if (double.IsNaN(top)) top = 0;

                            vm.X = left + deltaHorizontal;
                            vm.Y = top + deltaVertical;
                        }
                    }


                    if (canvas != null)
                        canvas.InvalidateMeasure();
                    e.Handled = true;
                }
            }
        }
    }
}
