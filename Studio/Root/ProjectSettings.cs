using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;
using Skill.DataModels;

namespace Skill.Studio
{
    #region ProjectSettings
    /// <summary>
    /// Defines setting of a project
    /// </summary>
    public class ProjectSettings : IXElement
    {
        /// <summary> Location in unity project to generate code files </summary>
        public string OutputLocaltion { get; set; }

        public ProjectSettings()
        {
            this.OutputLocaltion = "c:\\Unity\\Assets\\Skill";
        }

        public void Load(XElement e)
        {
            OutputLocaltion = e.Attribute("OutputLocaltion").Value;
        }

        public XElement ToXElement()
        {
            XElement settings = new XElement("Settings");
            settings.SetAttributeValue("OutputLocaltion", OutputLocaltion);
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
        /// <summary> History </summary>
        public UnDoRedo History { get; set; }
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

        public string OutputLocaltion
        {
            get { return Model.OutputLocaltion; }
            set
            {
                if (value != Model.OutputLocaltion && !string.IsNullOrEmpty(value))
                {
                    if (!Validation.LocationValidator.IsValid(value))
                    {
                        System.Windows.MessageBox.Show("Invalid directory");
                        return;
                    }
                    if (History != null)
                        History.Insert(new ChangePropertyUnDoRedo(this, "OutputLocaltion", value, Model.OutputLocaltion));
                    Model.OutputLocaltion = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("OutputLocaltion"));
                }
            }
        }
        public void CopyFrom(ProjectSettingsViewModel other)
        {
            this.OutputLocaltion = other.OutputLocaltion;
        }
    }
    #endregion
}
