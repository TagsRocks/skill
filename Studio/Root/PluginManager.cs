using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reflection;
using Skill.CodeGeneration;

namespace Skill.Studio
{
    #region PluginInfo
    /// <summary>
    /// Defines information about available plugin
    /// </summary>
    public class PluginInfo
    {
        /// <summary> Full path of dll file </summary>
        public string AssemblyPath { get; set; }
        /// <summary> Full path of class ( inherited from ICodeGenerator) </summary>
        public string ClassName { get; set; }
        /// <summary> Name of plugin provided by DisplayNameAttribute </summary>
        public string DisplayName { get; set; }

        /// <summary> Convert information to single string and can be save in project settings </summary>
        public string SaveString { get { return string.Format("{0}||{1}||{2}", AssemblyPath, ClassName, DisplayName); } }

        public override string ToString() { return DisplayName; }

        /// <summary>
        /// Load information from saved string
        /// </summary>
        /// <param name="saveString">SaveString</param>
        public PluginInfo(string saveString)
        {
            string[] split = saveString.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
            if (split != null && split.Length == 3)
            {
                this.AssemblyPath = split[0];
                this.ClassName = split[1];
                this.DisplayName = split[2];
            }
            else
            {
                this.AssemblyPath = "";
                this.ClassName = "";
                this.DisplayName = "";
            }
        }

        /// <summary>
        /// Create empty plugin information
        /// </summary>
        public PluginInfo()
        {
            this.AssemblyPath = "";
            this.ClassName = "";
            this.DisplayName = "";
        }


        public override bool Equals(object obj)
        {
            PluginInfo info = obj as PluginInfo;
            if (info == null) return false;
            return this.AssemblyPath.Equals(info.AssemblyPath, StringComparison.OrdinalIgnoreCase) &&
                this.ClassName.Equals(info.ClassName, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
    #endregion

    /// <summary>
    /// Manage plugin stuff
    /// </summary>
    internal static class PluginManager
    {
        #region IgnoredAssemblies
        /// <summary>
        /// list of assemblies that we sure do not have plugin
        /// </summary>
        private static string[] IgnoredAssemblies = new string[]
        {
            "Skill.dll",
            "Skill.CodeGeneration.dll",
            "Skill.UnityCSharpCodeGeneration.dll",
            "Skill.DataModels.dll",
            "Skill.Editor.dll",
            "nunit.framework.dll",
            "UnityEditor.dll",
            "UnityEngine.dll",
            "AvalonDock.dll",
            "AvalonDock.Themes.dll",
            "WPFToolkit.Extended.dll"
        };

        private static bool IsIgnored(string dllFilePath)
        {
            string fileName = Path.GetFileName(dllFilePath);
            foreach (var item in IgnoredAssemblies)
            {
                if (item.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
        #endregion

        #region Default Plugin
        /// <summary>
        /// Create plugin infomation for default code generator privided by skill developer
        /// </summary>
        public static PluginInfo DefaultSkillCodeGenerator
        {
            get
            {
                return new PluginInfo()
                {
                    AssemblyPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Skill.UnityCSharpCodeGeneration.dll"),
                    ClassName = "Skill.CodeGeneration.CSharp.CSharpCodeGenerator",
                    DisplayName = "Skill-Unity-CSharp"
                };
            }
        }
        #endregion

        private static PluginInfo _SelectedPlugin;
        public static PluginInfo SelectedPlugin
        {
            get
            {
                if (_SelectedPlugin == null)
                {
                    if (!string.IsNullOrEmpty(Properties.Settings.Default.Plugin))
                    {
                        _SelectedPlugin = new PluginInfo(Properties.Settings.Default.Plugin);
                    }
                    if (_SelectedPlugin == null || string.IsNullOrEmpty(_SelectedPlugin.AssemblyPath) || !File.Exists(_SelectedPlugin.AssemblyPath))
                    {
                        SelectedPlugin = DefaultSkillCodeGenerator;
                    }
                }
                return _SelectedPlugin;
            }

            set
            {
                if (value == null) return;
                _SelectedPlugin = value;
                _Plugin = null;
                Properties.Settings.Default.Plugin = _SelectedPlugin.SaveString;
                Properties.Settings.Default.Save();
            }
        }

        private static ICodeGenerator _Plugin;
        /// <summary> Curent active plugin </summary>
        public static ICodeGenerator Plugin
        {
            get
            {
                if (_Plugin == null)
                {
                    try
                    {
                        _Plugin = LoadPlugin(SelectedPlugin);
                        if (_Plugin == null)
                            MainWindow.Instance.ShowError("Invalid plugin. Please select valid plugin then try again.");
                    }
                    catch (Exception ex)
                    {
                        MainWindow.Instance.ShowError(ex.ToString());
                        _Plugin = null;
                    }
                }
                return _Plugin;
            }
        }

        public static PluginInfo[] FindPlugins()
        {
            List<PluginInfo> plugins = new List<PluginInfo>();
            plugins.Add(DefaultSkillCodeGenerator);

            string[] dLLs = Directory.GetFileSystemEntries(AppDomain.CurrentDomain.BaseDirectory, "*.dll");


            foreach (var dll in dLLs)
            {
                if (IsIgnored(dll)) continue;
                try
                {
                    ExamineAssembly(Assembly.LoadFrom(dll), "Skill.CodeGeneration.ICodeGenerator", plugins);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
            return plugins.ToArray();
        }

        // Methods
        public static ICodeGenerator LoadPlugin(PluginInfo Plugin)
        {
            object pluginInstance;
            try
            {
                pluginInstance = RuntimeHelpers.GetObjectValue(Assembly.LoadFrom(Plugin.AssemblyPath).CreateInstance(Plugin.ClassName));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                pluginInstance = null;
            }
            return pluginInstance as ICodeGenerator;
        }

        /// <summary>
        /// Check whether specified assembly containes any plugin
        /// </summary>
        /// <param name="assembly">Assembly to check</param>
        /// <param name="interfaceName">Full name of plugin interface</param>
        /// <param name="plugins">List of plugins to fill</param>
        private static void ExamineAssembly(Assembly assembly, string interfaceName, List<PluginInfo> plugins)
        {
            foreach (Type objType in assembly.GetTypes())
            {
                if ((objType.IsPublic && ((objType.Attributes & TypeAttributes.Abstract) != TypeAttributes.Abstract)) && (objType.GetInterface(interfaceName, true) != null))
                {
                    PluginInfo newPlugin = new PluginInfo()
                    {
                        AssemblyPath = assembly.Location,
                        ClassName = objType.FullName
                    };

                    // try find DisplayName 
                    object[] displayNames = objType.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    if (displayNames != null && displayNames.Length > 0)
                        newPlugin.DisplayName = (typeof(DisplayNameAttribute)).GetProperty("DisplayName", BindingFlags.Public | BindingFlags.Instance).GetValue(displayNames[0], null) as string;
                    if (string.IsNullOrEmpty(newPlugin.DisplayName))
                    {
                        int dotIndex = newPlugin.ClassName.LastIndexOf('.');
                        if (dotIndex < 0)
                            newPlugin.DisplayName = newPlugin.ClassName;
                        else
                            newPlugin.DisplayName = newPlugin.ClassName.Substring(dotIndex + 1);
                    }

                    plugins.Add(newPlugin);
                }
            }
        }
    }
}
