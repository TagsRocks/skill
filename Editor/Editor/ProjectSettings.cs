using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;

namespace Skill.Editor
{
    #region ProjectSettings
    /// <summary>
    /// Defines setting of a project
    /// </summary>
    public class ProjectSettings : IXElement
    {
        /// <summary> Location in unity project to generate code files </summary>
        public string UnityProjectLocaltion { get; set; }
        /// <summary> Postfix of user code file </summary>
        public string UserCodePostfix { get; set; }

        public ProjectSettings()
        {
            this.UnityProjectLocaltion = "c:\\Unity\\Assets\\Skill";
            this.UserCodePostfix = "_User";
        }

        public void Load(XElement e)
        {
            UnityProjectLocaltion = e.Attribute("UnityProjectLocaltion").Value;
            UserCodePostfix = e.Attribute("UserCodePostfix").Value;
        }

        public XElement ToXElement()
        {
            XElement settings = new XElement("Settings");
            settings.SetAttributeValue("UnityProjectLocaltion", UnityProjectLocaltion);
            settings.SetAttributeValue("UserCodePostfix", UserCodePostfix);
            return settings;
        }
    }
    #endregion

    #region ProjectSettingsViewModel
    public class ProjectSettingsViewModel : INotifyPropertyChanged
    {
        
        #region Properties
        /// <summary> ProjectSettings Model </summary>
        public ProjectSettings Model { get; private set; }
        #endregion

        #region Constructor
        public ProjectSettingsViewModel(ProjectSettings settings)
        {
            this.Model = settings;            
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, e);
        }

        #endregion // INotifyPropertyChanged Members

        public string UnityProjectLocaltion
        {
            get { return Model.UnityProjectLocaltion; }
            set
            {
                if (value != Model.UnityProjectLocaltion && !string.IsNullOrEmpty(value))
                {
                    if (!Validation.LocationValidator.IsValid(value))
                    {
                        System.Windows.MessageBox.Show("Invalid directory");
                        return;
                    }
                    Model.UnityProjectLocaltion = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("UnityProjectLocaltion"));
                }
            }
        }

        public string UserCodePostfix
        {
            get { return Model.UserCodePostfix; }
            set
            {
                if (value != Model.UserCodePostfix && !string.IsNullOrEmpty(value))
                {
                    Model.UserCodePostfix = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("UserCodePostfix"));
                }
            }
        }

        public void CopyFrom(ProjectSettingsViewModel other)
        {
            this.UnityProjectLocaltion = other.UnityProjectLocaltion;
            this.UserCodePostfix = other.UserCodePostfix;
        }
    }
    #endregion
}
