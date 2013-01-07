using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace Skill.Framework
{
    public interface ISettings
    {
        bool IsChanged { get; }
        void Write(BinaryWriter writer);
        void Read(BinaryReader reader);
        void SetDefault();
    }

    public static class GameSettings
    {
        public class AudioSettings : ISettings
        {

            private bool _Subtitle;
            public bool Subtitle
            {
                get
                {
                    return _Subtitle;
                }
                set
                {
                    if (_Subtitle != value)
                    {
                        _Subtitle = value;
                        IsChanged = true;
                    }
                }
            }

            private float _FxVolume;
            public float FxVolume
            {
                get
                {
                    return _FxVolume;
                }
                set
                {
                    if (value < 0.0f) value = 0.0f;
                    else if (value > 1.0f) value = 1.0f;
                    if (_FxVolume != value)
                    {
                        _FxVolume = value;
                        IsChanged = true;
                    }
                }
            }

            private float _DialogVolume;
            public float DialogVolume
            {
                get
                {
                    return _DialogVolume;
                }
                set
                {
                    if (value < 0.0f) value = 0.0f;
                    else if (value > 1.0f) value = 1.0f;
                    if (_DialogVolume != value)
                    {
                        _DialogVolume = value;
                        IsChanged = true;
                    }
                }
            }

            private float _MusicVolume;
            public float MusicVolume
            {
                get
                {
                    return _MusicVolume;
                }
                set
                {
                    if (value < 0.0f) value = 0.0f;
                    else if (value > 1.0f) value = 1.0f;
                    if (_MusicVolume != value)
                    {
                        _MusicVolume = value;
                        IsChanged = true;
                    }
                }
            }

            private float _MasterVolume;
            public float MasterVolume
            {
                get
                {
                    return _MasterVolume;
                }
                set
                {
                    if (value < 0.0f) value = 0.0f;
                    else if (value > 1.0f) value = 1.0f;
                    if (_MasterVolume != value)
                    {
                        _MasterVolume = value;
                        IsChanged = true;
                    }
                }
            }

            public bool IsChanged { get; private set; }

            public AudioSettings()
            {
                SetDefault();
            }

            public void Write(System.IO.BinaryWriter writer)
            {
                writer.Write(Subtitle);
                writer.Write(FxVolume);
                writer.Write(DialogVolume);
                writer.Write(MusicVolume);
                writer.Write(MasterVolume);
                IsChanged = false;
            }

            public void Read(System.IO.BinaryReader reader)
            {
                Subtitle = reader.ReadBoolean();
                FxVolume = reader.ReadSingle();
                DialogVolume = reader.ReadSingle();
                MusicVolume = reader.ReadSingle();
                MasterVolume = reader.ReadSingle();
                IsChanged = false;
            }

            public void SetDefault()
            {
                Subtitle = true;
                FxVolume = 1.0f;
                DialogVolume = 1.0f;
                MusicVolume = 1.0f;
                MasterVolume = 1.0f;
                IsChanged = false;
            }
        }

        public class GraphicsSettings : ISettings
        {
            public class ResolutionInfo
            {
                public string Name { get; private set; }
                public int Width { get; private set; }
                public int Height { get; private set; }

                public ResolutionInfo(string name, int width, int height)
                {
                    this.Name = name;
                    this.Width = width;
                    this.Height = height;
                }
            }

            public static ResolutionInfo[] Resolutions_4_3 = new ResolutionInfo[]
    {
        new ResolutionInfo("640x480",640,480),
        new ResolutionInfo("800x600",800,600),
        new ResolutionInfo("1024x768",1024,768),
        new ResolutionInfo("1152x864",1152,864)
    };

            public static ResolutionInfo[] Resolutions_16_10 = new ResolutionInfo[]
    {
        new ResolutionInfo("1280x800",1280,800),
        new ResolutionInfo("1440x900",1440,900),
    };

            public static ResolutionInfo[] Resolutions_16_9 = new ResolutionInfo[]
    {
        new ResolutionInfo("720x480",720,480),
        new ResolutionInfo("1280x720",1280,720),
        new ResolutionInfo("1360x768",1360,768),
        new ResolutionInfo("1600x600",1600,600),
    };

            private bool _Bloom;
            public bool Bloom { get { return _Bloom; } set { if (value != _Bloom) { _Bloom = value; IsChanged = true; } } }

            private bool _MotionBlur;
            public bool MotionBlur { get { return _MotionBlur; } set { if (value != _MotionBlur) { _MotionBlur = value; IsChanged = true; } } }

            private bool _FullScreen;
            public bool FullScreen { get { return _FullScreen; } set { if (value != _FullScreen) { _FullScreen = value; IsChanged = true; } } }

            private Resolution _Resolution;
            public Resolution Resolution { get { return _Resolution; } set { if (value.width != _Resolution.width || value.height != _Resolution.height || value.refreshRate != _Resolution.refreshRate) { _Resolution = value; IsChanged = true; } } }

            private int _QualityIndex;
            public int QualityIndex { get { return _QualityIndex; } set { if (value != _QualityIndex) { _QualityIndex = value; IsChanged = true; } } }

            public bool IsChanged { get; private set; }

            public void Write(System.IO.BinaryWriter writer)
            {
                writer.Write(Bloom);
                writer.Write(MotionBlur);
                writer.Write(FullScreen);
                writer.Write(Resolution.width);
                writer.Write(Resolution.height);
                writer.Write(Resolution.refreshRate);
                writer.Write(QualityIndex);

                IsChanged = false;
            }

            public void Read(System.IO.BinaryReader reader)
            {
                Bloom = reader.ReadBoolean();
                MotionBlur = reader.ReadBoolean();
                FullScreen = reader.ReadBoolean();
                Resolution = new Resolution() { width = reader.ReadInt32(), height = reader.ReadInt32(), refreshRate = reader.ReadInt32() };
                QualityIndex = reader.ReadInt32();
                IsChanged = false;
            }

            public void SetDefault()
            {
                Bloom = false;
                MotionBlur = false;
                FullScreen = true;
                Resolution = new Resolution() { width = 1024, height = 768, refreshRate = 60 };
                QualityIndex = 2;
                IsChanged = false;
            }
        }


        public static GraphicsSettings Graphics { get; private set; }
        public static AudioSettings Audio { get; private set; }

        static GameSettings()
        {
            Graphics = new GraphicsSettings();
            Audio = new AudioSettings();
        }

        private static string GetFilePath()
        {
            string gameDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games\\Gamename");
            return Path.Combine(gameDir, "Config.cfg");
        }

        private static void Load()
        {
            string filename = GetFilePath();
            if (File.Exists(filename))
            {
                try
                {
                    LoadFrom(filename);
                }
                catch (System.Exception)
                {
                    Graphics.SetDefault();
                    Audio.SetDefault();
                    File.Delete(filename);
                }
            }
            else
            {
                Save();
            }
        }

        private static void LoadFrom(string filename)
        {
            if (File.Exists(filename))
            {
                FileStream file = File.OpenRead(filename);
                BinaryReader reader = new BinaryReader(file);

                try
                {
                    Graphics.Read(reader);
                    Audio.Read(reader);
                }
                catch (Exception)
                {
                    reader.Close();
                    file.Close();
                    throw;
                }
            }
        }

        private static void Save()
        {
            string filename = GetFilePath();
            if (File.Exists(filename))
                File.Delete(filename);

            string dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            FileStream file = File.OpenWrite(filename);
            BinaryWriter writer = new BinaryWriter(file);

            try
            {
                Graphics.Write(writer);
                Audio.Write(writer);
            }
            catch (Exception)
            {
                writer.Close();
                file.Close();
                throw;
            }

        }
    }
}