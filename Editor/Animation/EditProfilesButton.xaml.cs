﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using System.Collections.ObjectModel;

namespace Skill.Editor.Animation
{
    /// <summary>
    /// Interaction logic for EditProfilesButton.xaml
    /// </summary>
    public partial class EditProfilesButton : UserControl, ITypeEditor
    {
        private AnimNodeProperties _Properties = null;

        public EditProfilesButton()
        {
            InitializeComponent();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (this._Properties == null) return;

            if (this._Properties is Skill.Editor.Animation.AnimNodeAimOffsetProperties)
            {
                Skill.Editor.Animation.AnimNodeAimOffsetProperties properties = this._Properties as Skill.Editor.Animation.AnimNodeAimOffsetProperties;
                if (properties != null)
                {
                    AnimNodeAimOffsetProfileEditor editor = new AnimNodeAimOffsetProfileEditor(properties.Profiles);
                    editor.ShowDialog();
                    this._Properties.Editor.SetChanged(true);
                }
            }
            else if (this._Properties is Skill.Editor.Animation.AnimationTreeRootProperties)
            {
                Skill.Editor.Animation.AnimationTreeRootProperties properties = this._Properties as Skill.Editor.Animation.AnimationTreeRootProperties;
                if (properties != null)
                {
                    AnimationTreeProfileEditor editor = new AnimationTreeProfileEditor(properties.Profiles);
                    editor.ShowDialog();
                    this._Properties.Editor.SetChanged(true);
                }
            }
        }

        public FrameworkElement ResolveEditor(Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem propertyItem)
        {
            if (propertyItem.Instance is Skill.Editor.Animation.AnimNodeProperties)
            {
                this._Properties = propertyItem.Instance as Skill.Editor.Animation.AnimNodeProperties;
            }
            return this;
        }
    }
}
