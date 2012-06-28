using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Input;

namespace Skill.Studio.AI.Editor
{
    public class ParametersPropertyEditor : Xceed.Wpf.Toolkit.PropertyGrid.Editors.ITypeEditor
    {
        private BehaviorViewModel _Behavior;
        private TextBlock _TextBlock;

        public ParametersPropertyEditor()
        {
            _TextBlock = new TextBlock();
            _TextBlock.Cursor = Cursors.IBeam;
            _TextBlock.Text = "Parameters{...}";
            _TextBlock.Padding = new Thickness(5, 0, 5, 0);
            _TextBlock.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(_TextBlock_MouseLeftButtonUp);
        }

        void _TextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_Behavior != null)
            {
                ParameterCollectionViewModel parameters = null;

                if (_Behavior.Model.BehaviorType == DataModels.AI.BehaviorType.Action)
                    parameters = ((ActionViewModel)_Behavior).Parameters;
                else if (_Behavior.Model.BehaviorType == DataModels.AI.BehaviorType.Decorator)
                    parameters = ((DecoratorViewModel)_Behavior).Parameters;
                else if (_Behavior.Model.BehaviorType == DataModels.AI.BehaviorType.Condition)
                    parameters = ((ConditionViewModel)_Behavior).Parameters;

                if (parameters != null)
                {
                    ParametersEditorWindow window = new ParametersEditorWindow(parameters);
                    window.Owner = MainWindow.Instance;
                    window.ShowDialog();

                    if (_Behavior.Tree.Editor != null)
                        _Behavior.Tree.Editor.SetChanged(true);

                    _Behavior.RaiseChangeDisplayName();
                }
            }
        }

        public FrameworkElement ResolveEditor(Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem propertyItem)
        {
            _Behavior = propertyItem.Instance as BehaviorViewModel;
            return _TextBlock;
        }
    }
}
