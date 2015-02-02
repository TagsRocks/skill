using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;

namespace Skill.Framework
{
    /// <summary>
    /// A general settings class that contains all variables needed for games, that provides some methods to ease process of save/load settings.
    /// is is possible to save/load settings in binary and xml formats. New variables will be added in future versions.
    /// Override virtual methods to fit your default settings.
    /// </summary>
    public class Settings : Skill.Framework.IO.ISavable
    {
        /// <summary>The AA Filtering option. </summary>
        public enum AntiAliasing
        {
            None = 0,
            TwoXSampling = 2,
            FourXSampling = 4,
            EightXSampling = 8,
        }

        /// <summary> Number of cascades to use for directional light shadows. </summary>
        public enum ShadowCascades
        {
            None = 0,
            TowCascades = 2,
            FourCascades = 4
        }

        /// <summary> The VSync Count. </summary>
        public enum VSyncCount
        {
            None = 0,
            EveryVBlank = 1,
            EverySecondVBlank = 2,
        }

        private static E LoadEnum<E>(string str, E defaultValue)
        {
            try
            {
                return (E)Enum.Parse(typeof(E), str, true);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        private static int FilterValue(int value, int defaultValue, Array filters)
        {
            if (filters != null)
            {
                foreach (var item in filters)
                {
                    if ((int)item == value) return value;
                }
            }
            return defaultValue;
        }

        public sealed class AudioSettings : Skill.Framework.IO.ISavable
        {

            // Variables
            private bool _Subtitle;
            private float _FxVolume;
            private float _VoiceVolume;
            private float _MusicVolume;
            private float _CinematicVolume;
            private float _MasterVolume;
            private int _OutputSampleRate;
            private AudioSpeakerMode _SpeakerMode;

            // Properties
            public bool Subtitle { get { return _Subtitle; } set { _Subtitle = value; } }
            public float FxVolume { get { return _FxVolume; } set { _FxVolume = Mathf.Clamp01(value); } }
            public float VoiceVolume { get { return _VoiceVolume; } set { _VoiceVolume = Mathf.Clamp01(value); } }
            public float MusicVolume { get { return _MusicVolume; } set { _MusicVolume = Mathf.Clamp01(value); } }
            public float CinematicVolume { get { return _CinematicVolume; } set { _CinematicVolume = Mathf.Clamp01(value); } }
            public float MasterVolume { get { return _MasterVolume; } set { _MasterVolume = Mathf.Clamp01(value); } }
            public int OutputSampleRate { get { return _OutputSampleRate; } set { _OutputSampleRate = ValidateSampleRate(value); } }
            public AudioSpeakerMode SpeakerMode { get { return _SpeakerMode; } set { _SpeakerMode = value; } }

            private int ValidateSampleRate(int sampleRate)
            {
                if (sampleRate != 8000 &&
                    sampleRate != 11025 &&
                    sampleRate != 12000 &&
                    sampleRate != 16000 &&
                    sampleRate != 22050 &&
                    sampleRate != 32000 &&
                    sampleRate != 44100 &&
                    sampleRate != 48000 &&
                    sampleRate != 96000)
                    sampleRate = 44100;
                return sampleRate;
            }

            public float GetVolume(Skill.Framework.Audio.SoundCategory category)
            {
                float volume = 1.0f;

                switch (category)
                {
                    case Skill.Framework.Audio.SoundCategory.FX:
                        volume = FxVolume;
                        break;
                    case Skill.Framework.Audio.SoundCategory.Music:
                        volume = MusicVolume;
                        break;
                    case Skill.Framework.Audio.SoundCategory.Voice:
                        volume = VoiceVolume;
                        break;
                    case Skill.Framework.Audio.SoundCategory.Cinematic:
                        volume = CinematicVolume;
                        break;
                    default:
                        volume = 1.0f;
                        break;
                }
                return volume * MasterVolume;
            }

            public void ApplyChanges()
            {
                if (UnityEngine.AudioSettings.outputSampleRate != _OutputSampleRate)
                    UnityEngine.AudioSettings.outputSampleRate = _OutputSampleRate;
                if (UnityEngine.AudioSettings.speakerMode != _SpeakerMode)
                    UnityEngine.AudioSettings.speakerMode = _SpeakerMode;
            }

            /// <summary>
            /// read from UnityEngine.AudioSettings. this is useful when Game started for first time
            /// </summary>
            public void SetAsCurrentSetting()
            {
                _OutputSampleRate = UnityEngine.AudioSettings.outputSampleRate;
                _SpeakerMode = UnityEngine.AudioSettings.speakerMode;
            }

            // Methods
            public AudioSettings()
            {
            }

            public static AudioSettings CreateAudioSettings()
            {
                return new AudioSettings();
            }

            public void Save(IO.XmlElement e, Skill.Framework.IO.XmlSaveStream stream)
            {
                IO.XmlElement _SubtitleElement = stream.Create("Subtitle", _Subtitle);
                e.AppendChild(_SubtitleElement);
                IO.XmlElement _FxVolumeElement = stream.Create("FxVolume", _FxVolume);
                e.AppendChild(_FxVolumeElement);
                IO.XmlElement _VoiceVolumeElement = stream.Create("VoiceVolume", _VoiceVolume);
                e.AppendChild(_VoiceVolumeElement);
                IO.XmlElement _MusicVolumeElement = stream.Create("MusicVolume", _MusicVolume);
                e.AppendChild(_MusicVolumeElement);
                IO.XmlElement _CinematicVolumeElement = stream.Create("CinematicVolume", _CinematicVolume);
                e.AppendChild(_CinematicVolumeElement);
                IO.XmlElement _MasterVolumeElement = stream.Create("MasterVolume", _MasterVolume);
                e.AppendChild(_MasterVolumeElement);
                IO.XmlElement _OutputSampleRateElement = stream.Create("OutputSampleRate", _OutputSampleRate);
                e.AppendChild(_OutputSampleRateElement);
                IO.XmlElement _SpeakerModeElement = stream.Create("SpeakerMode", _SpeakerMode.ToString());
                e.AppendChild(_SpeakerModeElement);

            }
            public void Save(Skill.Framework.IO.BinarySaveStream stream)
            {
                stream.Write(_Subtitle);
                stream.Write(_FxVolume);
                stream.Write(_VoiceVolume);
                stream.Write(_MusicVolume);
                stream.Write(_CinematicVolume);
                stream.Write(_MasterVolume);
                stream.Write(_OutputSampleRate);
                stream.Write((int)_SpeakerMode);

            }
            public void Load(IO.XmlElement e, Skill.Framework.IO.XmlLoadStream stream)
            {
                IO.XmlElement element = e.FirstChild as IO.XmlElement;
                while (element != null)
                {
                    switch (element.Name)
                    {
                        case "Subtitle":
                            this.Subtitle = stream.ReadBoolean(element);
                            break;
                        case "FxVolume":
                            this.FxVolume = stream.ReadFloat(element);
                            break;
                        case "VoiceVolume":
                            this.VoiceVolume = stream.ReadFloat(element);
                            break;
                        case "MusicVolume":
                            this.MusicVolume = stream.ReadFloat(element);
                            break;
                        case "CinematicVolume":
                            this.CinematicVolume = stream.ReadFloat(element);
                            break;
                        case "MasterVolume":
                            this.MasterVolume = stream.ReadFloat(element);
                            break;
                        case "OutputSampleRate":
                            this.OutputSampleRate = stream.ReadInt(element);
                            break;
                        case "SpeakerMode":
                            this.SpeakerMode = LoadEnum<AudioSpeakerMode>(stream.ReadString(element), AudioSpeakerMode.Stereo);
                            break;
                    }
                    element = e.GetNextSibling(element);
                }

            }
            public void Load(Skill.Framework.IO.BinaryLoadStream stream)
            {
                this.Subtitle = stream.ReadBoolean();
                this.FxVolume = stream.ReadFloat();
                this.VoiceVolume = stream.ReadFloat();
                this.MusicVolume = stream.ReadFloat();
                this.CinematicVolume = stream.ReadFloat();
                this.MasterVolume = stream.ReadFloat();
                this.OutputSampleRate = stream.ReadInt();
                this.SpeakerMode = (AudioSpeakerMode)FilterValue(stream.ReadInt(), (int)AudioSpeakerMode.Stereo, Enum.GetValues(typeof(AudioSpeakerMode)));
            }

        }
        public sealed class QualitySettings : Skill.Framework.IO.ISavable
        {

            // saved variables
            private AnisotropicFiltering _PreAnisotropicFiltering;
            private AntiAliasing _PreAntiAliasing;
            private BlendWeights _PreBlendWeights;
            private float _PreLodBias;
            private int _PreMasterTextureLimit;
            private int _PreMaximumLODLevel;
            private int _PreMaxQueuedFrames;
            private int _PreParticleRaycastBudget;
            private int _PrePixelLightCount;
            private ShadowCascades _PreShadowCascades;
            private float _PreShadowDistance;
            private ShadowProjection _PreShadowProjection;
            private bool _PreSoftVegetation;
            private VSyncCount _PreVSyncCount;
            private bool _PreBloom;
            private bool _PreHDR;
            private bool _PreAmbientOcclusion;
            private int _PrePostprocessQuality;
            private int _PreShadowQuality;
            private int _PreResolutionWidth;
            private int _PreResolutionHeight;
            private int _PreRefreshRate;
            private bool _PreFullScreen;
            private int _PreQualityLevel;

            // Variables
            private AnisotropicFiltering _AnisotropicFiltering;
            private AntiAliasing _AntiAliasing;
            private BlendWeights _BlendWeights;
            private float _LodBias;
            private int _MasterTextureLimit;
            private int _MaximumLODLevel;
            private int _MaxQueuedFrames;
            private int _ParticleRaycastBudget;
            private int _PixelLightCount;
            private ShadowCascades _ShadowCascades;
            private float _ShadowDistance;
            private ShadowProjection _ShadowProjection;
            private bool _SoftVegetation;
            private VSyncCount _VSyncCount;
            private bool _Bloom;
            private bool _HDR;
            private bool _AmbientOcclusion;
            private int _PostprocessQuality;
            private int _ShadowQuality;
            private int _ResolutionWidth;
            private int _ResolutionHeight;
            private int _RefreshRate;
            private bool _FullScreen;
            private int _QualityLevel;

            // Properties
            /// <summary>   Global anisotropic filtering mode. </summary>
            public AnisotropicFiltering AnisotropicFiltering { get { return _AnisotropicFiltering; } set { _AnisotropicFiltering = value; } }
            /// <summary> Set The AA Filtering option. </summary>
            public AntiAliasing AntiAliasing { get { return _AntiAliasing; } set { _AntiAliasing = value; } }
            /// <summary>   Blend weights. </summary>
            public BlendWeights BlendWeights { get { return _BlendWeights; } set { _BlendWeights = value; } }
            /// <summary>   Global multiplier for the LOD's switching distance. </summary>
            public float LodBias { get { return _LodBias; } set { _LodBias = value; } }
            /// <summary>   A texture size limit applied to all textures. </summary>
            public int MasterTextureLimit { get { return _MasterTextureLimit; } set { if (value < 0) value = 0; _MasterTextureLimit = value; } }
            /// <summary>   A maximum LOD level. All LOD groups </summary>
            public int MaximumLODLevel { get { return _MaximumLODLevel; } set { if (value < 0) value = 0; _MaximumLODLevel = value; } }
            /// <summary> Maximum number of frames queued up by graphics driver. </summary>
            public int MaxQueuedFrames { get { return _MaxQueuedFrames; } set { if (value < 0) value = 0; _MaxQueuedFrames = value; } }
            /// <summary> Budget for how many ray casts can be performed per frame for approximate collision testing. </summary>
            public int ParticleRaycastBudget { get { return _ParticleRaycastBudget; } set { if (value < 0) value = 0; _ParticleRaycastBudget = value; } }
            /// <summary> The maximum number of pixel lights that should affect any object. </summary>
            public int PixelLightCount { get { return _PixelLightCount; } set { if (value < 0) value = 0; _PixelLightCount = value; } }
            /// <summary>   Number of cascades to use for directional light shadows. </summary>
            public ShadowCascades ShadowCascades { get { return _ShadowCascades; } set { _ShadowCascades = value; } }
            /// <summary>   Shadow drawing distance. </summary>
            public float ShadowDistance { get { return _ShadowDistance; } set { if (value < 0) value = 0; _ShadowDistance = value; } }
            /// <summary> Directional light shadow projection. </summary>
            public ShadowProjection ShadowProjection { get { return _ShadowProjection; } set { _ShadowProjection = value; } }
            /// <summary> Use a two-pass shader for the vegetation in the terrain engine. </summary>
            public bool SoftVegetation { get { return _SoftVegetation; } set { _SoftVegetation = value; } }
            /// <summary> The VSync Count. </summary>
            public VSyncCount VSyncCount { get { return _VSyncCount; } set { _VSyncCount = value; } }
            /// <summary> Enable or disable bloom effect </summary>
            public bool Bloom { get { return _Bloom; } set { _Bloom = value; } }
            /// <summary> Enable or disable HDR effect </summary>
            public bool HDR { get { return _HDR; } set { _HDR = value; } }
            /// <summary> Enable or disable AmbientOcclusion </summary>
            public bool AmbientOcclusion { get { return _AmbientOcclusion; } set { _AmbientOcclusion = value; } }
            /// <summary> Gets of sets postprocess quality </summary>
            public int PostprocessQuality { get { return _PostprocessQuality; } set { _PostprocessQuality = value; } }
            /// <summary> Gets of sets shadow quality </summary>
            public int ShadowQuality { get { return _ShadowQuality; } set { _ShadowQuality = value; } }
            /// <summary> Gets of sets resolution </summary>
            public Resolution Resolution
            {
                get { return new UnityEngine.Resolution() { width = _ResolutionWidth, height = _ResolutionHeight, refreshRate = _RefreshRate }; }
                set
                {
                    _ResolutionWidth = value.width;
                    _ResolutionHeight = value.height;
                    _RefreshRate = value.refreshRate;
                }
            }
            /// <summary> Gets of sets fullscreen </summary>
            public bool FullScreen { get { return _FullScreen; } set { _FullScreen = value; } }
            /// <summary> Graphics quality level. </summary>
            public int QualityLevel
            {
                get { return _QualityLevel; }
                set
                {
                    if (value < 0) value = 0;
                    else if (value >= UnityEngine.QualitySettings.names.Length)
                        value = UnityEngine.QualitySettings.names.Length - 1;
                    _QualityLevel = value;
                }
            }

            /// <summary> if true, ApplyChanges() will override 'UnityEngine.QualitySettings.anisotropicFiltering'.This value is will saved to file </summary>
            public bool OverrideAnisotropicFiltering { get; set; }
            /// <summary> if true, ApplyChanges() will override 'UnityEngine.QualitySettings.antiAliasing'.This value is will saved to file </summary>
            public bool OverrideAntiAliasing { get; set; }
            /// <summary> if true, ApplyChanges() will override 'UnityEngine.QualitySettings.blendWeights'.This value is will saved to file </summary>
            public bool OverrideBlendWeights { get; set; }
            /// <summary> if true, ApplyChanges() will override 'UnityEngine.QualitySettings.lodBias'.This value is will saved to file </summary>
            public bool OverrideLodBias { get; set; }
            /// <summary> if true, ApplyChanges() will override 'UnityEngine.QualitySettings.masterTextureLimit'.This value is will saved to file </summary>
            public bool OverrideMasterTextureLimit { get; set; }
            /// <summary> if true, ApplyChanges() will override 'UnityEngine.QualitySettings.maximumLODLevel'.This value is will saved to file </summary>
            public bool OverrideMaximumLODLevel { get; set; }
            /// <summary> if true, ApplyChanges() will override 'UnityEngine.QualitySettings.maxQueuedFrames'.This value is will saved to file </summary>
            public bool OverrideMaxQueuedFrames { get; set; }
            /// <summary> if true, ApplyChanges() will override 'UnityEngine.QualitySettings.particleRaycastBudget'.This value is will saved to file </summary>
            public bool OverrideParticleRaycastBudget { get; set; }
            /// <summary> if true, ApplyChanges() will override 'UnityEngine.QualitySettings.pixelLightCount'.This value is will saved to file </summary>
            public bool OverridePixelLightCount { get; set; }
            /// <summary> if true, ApplyChanges() will override 'UnityEngine.QualitySettings.shadowCascades'.This value is will saved to file </summary>
            public bool OverrideShadowCascades { get; set; }
            /// <summary> if true, ApplyChanges() will override 'UnityEngine.QualitySettings.shadowDistance'.This value is will saved to file </summary>
            public bool OverrideShadowDistance { get; set; }
            /// <summary> if true, ApplyChanges() will override 'UnityEngine.QualitySettings.shadowProjection'.This value is will saved to file </summary>
            public bool OverrideShadowProjection { get; set; }
            /// <summary> if true, ApplyChanges() will override 'UnityEngine.QualitySettings.softVegetation'.This value is will saved to file </summary>
            public bool OverrideSoftVegetation { get; set; }
            /// <summary> if true, ApplyChanges() will override 'UnityEngine.QualitySettings.vSyncCount'.This value is will saved to file </summary>
            public bool OverrideVSyncCount { get; set; }

            internal void CopyNewValues()
            {
                _PreAnisotropicFiltering = _AnisotropicFiltering;
                _PreAntiAliasing = _AntiAliasing;
                _PreBlendWeights = _BlendWeights;
                _PreLodBias = _LodBias;
                _PreMasterTextureLimit = _MasterTextureLimit;
                _PreMaximumLODLevel = _MaximumLODLevel;
                _PreMaxQueuedFrames = _MaxQueuedFrames;
                _PreParticleRaycastBudget = _ParticleRaycastBudget;
                _PrePixelLightCount = _PixelLightCount;
                _PreShadowCascades = _ShadowCascades;
                _PreShadowDistance = _ShadowDistance;
                _PreShadowProjection = _ShadowProjection;
                _PreSoftVegetation = _SoftVegetation;
                _PreVSyncCount = _VSyncCount;
                _PreBloom = _Bloom;
                _PreHDR = _HDR;
                _PreAmbientOcclusion = _AmbientOcclusion;
                _PrePostprocessQuality = _PostprocessQuality;
                _PreShadowQuality = _ShadowQuality;
                _PreResolutionWidth = _ResolutionWidth;
                _PreResolutionHeight = _ResolutionHeight;
                _PreRefreshRate = _RefreshRate;
                _PreFullScreen = _FullScreen;
                _PreQualityLevel = _QualityLevel;
            }
            private void CopyOldValues()
            {
                _AnisotropicFiltering = _PreAnisotropicFiltering;
                _AntiAliasing = _PreAntiAliasing;
                _BlendWeights = _PreBlendWeights;
                _LodBias = _PreLodBias;
                _MasterTextureLimit = _PreMasterTextureLimit;
                _MaximumLODLevel = _PreMaximumLODLevel;
                _MaxQueuedFrames = _PreMaxQueuedFrames;
                _ParticleRaycastBudget = _PreParticleRaycastBudget;
                _PixelLightCount = _PrePixelLightCount;
                _ShadowCascades = _PreShadowCascades;
                _ShadowDistance = _PreShadowDistance;
                _ShadowProjection = _PreShadowProjection;
                _SoftVegetation = _PreSoftVegetation;
                _VSyncCount = _PreVSyncCount;
                _Bloom = _PreBloom;
                _HDR = _PreHDR;
                _AmbientOcclusion = _PreAmbientOcclusion;
                _PostprocessQuality = _PrePostprocessQuality;
                _ShadowQuality = _PreShadowQuality;
                _ResolutionWidth = _PreResolutionWidth;
                _ResolutionHeight = _PreResolutionHeight;
                _RefreshRate = _PreRefreshRate;
                _FullScreen = _PreFullScreen;
                _QualityLevel = _PreQualityLevel;
            }



            public void AcceptChanges(bool applyExpensiveChanges = true)
            {
                CopyNewValues();
                ApplyChanges();
            }

            public void RevertChanges(bool applyExpensiveChanges = true)
            {
                CopyOldValues();
                ApplyChanges();
            }

            public void ApplyChanges(bool applyExpensiveChanges = true)
            {

                if (QualityLevel != UnityEngine.QualitySettings.GetQualityLevel())
                    UnityEngine.QualitySettings.SetQualityLevel(QualityLevel, applyExpensiveChanges);

                if (applyExpensiveChanges)
                {
                    if (OverrideAnisotropicFiltering) UnityEngine.QualitySettings.anisotropicFiltering = AnisotropicFiltering;
                    if (OverrideAntiAliasing) UnityEngine.QualitySettings.antiAliasing = (int)AntiAliasing;
                }
                if (OverrideBlendWeights) UnityEngine.QualitySettings.blendWeights = BlendWeights;
                if (OverrideLodBias) UnityEngine.QualitySettings.lodBias = LodBias;
                if (OverrideMasterTextureLimit) UnityEngine.QualitySettings.masterTextureLimit = MasterTextureLimit;
                if (OverrideMaximumLODLevel) UnityEngine.QualitySettings.maximumLODLevel = MaximumLODLevel;
                if (OverrideMaxQueuedFrames) UnityEngine.QualitySettings.maxQueuedFrames = MaxQueuedFrames;
                if (OverrideParticleRaycastBudget) UnityEngine.QualitySettings.particleRaycastBudget = ParticleRaycastBudget;
                if (OverridePixelLightCount) UnityEngine.QualitySettings.pixelLightCount = PixelLightCount;
                if (OverrideShadowCascades) UnityEngine.QualitySettings.shadowCascades = (int)ShadowCascades;
                if (OverrideShadowDistance) UnityEngine.QualitySettings.shadowDistance = ShadowDistance;
                if (OverrideShadowProjection) UnityEngine.QualitySettings.shadowProjection = ShadowProjection;
                if (OverrideSoftVegetation) UnityEngine.QualitySettings.softVegetation = SoftVegetation;
                if (OverrideVSyncCount) UnityEngine.QualitySettings.vSyncCount = (int)VSyncCount;

                Resolution currRes = Screen.currentResolution;
                Resolution newRes = Resolution;
                if (!newRes.Equals(currRes) || FullScreen != Screen.fullScreen)
                    UnityEngine.Screen.SetResolution(newRes.width, newRes.height, FullScreen, newRes.refreshRate);
            }

            // Methods
            public QualitySettings()
            {
            }

            public static QualitySettings CreateQualitySettings()
            {
                return new QualitySettings();
            }

            /// <summary>
            /// read from UnityEngine.QualitySettings. this is useful when Game started for first time
            /// </summary>
            public void SetAsCurrentSetting()
            {
                _AnisotropicFiltering = UnityEngine.QualitySettings.anisotropicFiltering;
                _AntiAliasing = (Settings.AntiAliasing)UnityEngine.QualitySettings.antiAliasing;
                _BlendWeights = UnityEngine.QualitySettings.blendWeights;
                _LodBias = UnityEngine.QualitySettings.lodBias;
                _MasterTextureLimit = UnityEngine.QualitySettings.masterTextureLimit;
                _MaximumLODLevel = UnityEngine.QualitySettings.maximumLODLevel;
                _MaxQueuedFrames = UnityEngine.QualitySettings.maxQueuedFrames;
                _ParticleRaycastBudget = UnityEngine.QualitySettings.particleRaycastBudget;
                _PixelLightCount = UnityEngine.QualitySettings.pixelLightCount;
                _ShadowCascades = (Settings.ShadowCascades)UnityEngine.QualitySettings.shadowCascades;
                _ShadowDistance = UnityEngine.QualitySettings.shadowDistance;
                _ShadowProjection = UnityEngine.QualitySettings.shadowProjection;
                _SoftVegetation = UnityEngine.QualitySettings.softVegetation;
                _VSyncCount = (Settings.VSyncCount)UnityEngine.QualitySettings.vSyncCount;
                _ResolutionWidth = Screen.width;
                _ResolutionHeight = Screen.height;
                _RefreshRate = Screen.currentResolution.refreshRate;
                _QualityLevel = UnityEngine.QualitySettings.GetQualityLevel();
                _FullScreen = Screen.fullScreen;
            }

            public void Save(IO.XmlElement e, Skill.Framework.IO.XmlSaveStream stream)
            {
                IO.XmlElement _AnisotropicFilteringElement = stream.Create("AnisotropicFiltering", _AnisotropicFiltering.ToString());
                e.AppendChild(_AnisotropicFilteringElement);
                IO.XmlElement _AntiAliasingElement = stream.Create("AntiAliasing", _AntiAliasing.ToString());
                e.AppendChild(_AntiAliasingElement);
                IO.XmlElement _BlendWeightsElement = stream.Create("BlendWeights", _BlendWeights.ToString());
                e.AppendChild(_BlendWeightsElement);
                IO.XmlElement _LodBiasElement = stream.Create("LodBias", _LodBias);
                e.AppendChild(_LodBiasElement);
                IO.XmlElement _MasterTextureLimitElement = stream.Create("MasterTextureLimit", _MasterTextureLimit);
                e.AppendChild(_MasterTextureLimitElement);
                IO.XmlElement _MaximumLODLevelElement = stream.Create("MaximumLODLevel", _MaximumLODLevel);
                e.AppendChild(_MaximumLODLevelElement);
                IO.XmlElement _MaxQueuedFramesElement = stream.Create("MaxQueuedFrames", _MaxQueuedFrames);
                e.AppendChild(_MaxQueuedFramesElement);
                IO.XmlElement _ParticleRaycastBudgetElement = stream.Create("ParticleRaycastBudget", _ParticleRaycastBudget);
                e.AppendChild(_ParticleRaycastBudgetElement);
                IO.XmlElement _PixelLightCountElement = stream.Create("PixelLightCount", _PixelLightCount);
                e.AppendChild(_PixelLightCountElement);
                IO.XmlElement _ShadowCascadesElement = stream.Create("ShadowCascades", _ShadowCascades.ToString());
                e.AppendChild(_ShadowCascadesElement);
                IO.XmlElement _ShadowDistanceElement = stream.Create("ShadowDistance", _ShadowDistance);
                e.AppendChild(_ShadowDistanceElement);
                IO.XmlElement _ShadowProjectionElement = stream.Create("ShadowProjection", _ShadowProjection.ToString());
                e.AppendChild(_ShadowProjectionElement);
                IO.XmlElement _SoftVegetationElement = stream.Create("SoftVegetation", _SoftVegetation);
                e.AppendChild(_SoftVegetationElement);
                IO.XmlElement _VSyncCountElement = stream.Create("VSyncCount", _VSyncCount.ToString());
                e.AppendChild(_VSyncCountElement);
                IO.XmlElement _BloomElement = stream.Create("Bloom", _Bloom);
                e.AppendChild(_BloomElement);
                IO.XmlElement _HDRElement = stream.Create("HDR", _HDR);
                e.AppendChild(_HDRElement);
                IO.XmlElement _AmbientOcclusionElement = stream.Create("AmbientOcclusion", _AmbientOcclusion);
                e.AppendChild(_AmbientOcclusionElement);
                IO.XmlElement _PostprocessQualityElement = stream.Create("PostprocessQuality", _PostprocessQuality);
                e.AppendChild(_PostprocessQualityElement);
                IO.XmlElement _ShadowQualityElement = stream.Create("ShadowQuality", _ShadowQuality);
                e.AppendChild(_ShadowQualityElement);
                IO.XmlElement _ResolutionWidthElement = stream.Create("ResolutionWidth", _ResolutionWidth);
                e.AppendChild(_ResolutionWidthElement);
                IO.XmlElement _ResolutionHeightElement = stream.Create("ResolutionHeight", _ResolutionHeight);
                e.AppendChild(_ResolutionHeightElement);
                IO.XmlElement _RefreshRateElement = stream.Create("RefreshRate", _RefreshRate);
                e.AppendChild(_RefreshRateElement);
                IO.XmlElement _FullScreenElement = stream.Create("FullScreen", _FullScreen);
                e.AppendChild(_FullScreenElement);
                IO.XmlElement _QualityLevelElement = stream.Create("QualityLevel", _QualityLevel);
                e.AppendChild(_QualityLevelElement);

            }
            public void Save(Skill.Framework.IO.BinarySaveStream stream)
            {
                stream.Write((int)_AnisotropicFiltering);
                stream.Write((int)_AntiAliasing);
                stream.Write((int)_BlendWeights);
                stream.Write(_LodBias);
                stream.Write(_MasterTextureLimit);
                stream.Write(_MaximumLODLevel);
                stream.Write(_MaxQueuedFrames);
                stream.Write(_ParticleRaycastBudget);
                stream.Write(_PixelLightCount);
                stream.Write((int)_ShadowCascades);
                stream.Write(_ShadowDistance);
                stream.Write((int)_ShadowProjection);
                stream.Write(_SoftVegetation);
                stream.Write((int)_VSyncCount);
                stream.Write(_Bloom);
                stream.Write(_HDR);
                stream.Write(_AmbientOcclusion);
                stream.Write(_PostprocessQuality);
                stream.Write(_ShadowQuality);
                stream.Write(_ResolutionWidth);
                stream.Write(_ResolutionHeight);
                stream.Write(_RefreshRate);
                stream.Write(_FullScreen);
                stream.Write(_QualityLevel);
            }

            public void Load(IO.XmlElement e, Skill.Framework.IO.XmlLoadStream stream)
            {
                IO.XmlElement element = e.FirstChild as IO.XmlElement;
                while (element != null)
                {
                    switch (element.Name)
                    {
                        case "AnisotropicFiltering":
                            this._AnisotropicFiltering = LoadEnum<AnisotropicFiltering>(stream.ReadString(element), UnityEngine.AnisotropicFiltering.Disable);
                            break;
                        case "AntiAliasing":
                            this._AntiAliasing = LoadEnum<AntiAliasing>(stream.ReadString(element), Settings.AntiAliasing.None);
                            break;
                        case "BlendWeights":
                            this._BlendWeights = LoadEnum<BlendWeights>(stream.ReadString(element), BlendWeights.TwoBones);
                            break;
                        case "LodBias":
                            this._LodBias = stream.ReadFloat(element);
                            break;
                        case "MasterTextureLimit":
                            this._MasterTextureLimit = stream.ReadInt(element);
                            break;
                        case "MaximumLODLevel":
                            this._MaximumLODLevel = stream.ReadInt(element);
                            break;
                        case "MaxQueuedFrames":
                            this._MaxQueuedFrames = stream.ReadInt(element);
                            break;
                        case "ParticleRaycastBudget":
                            this._ParticleRaycastBudget = stream.ReadInt(element);
                            break;
                        case "PixelLightCount":
                            this._PixelLightCount = stream.ReadInt(element);
                            break;
                        case "ShadowCascades":
                            this._ShadowCascades = LoadEnum<ShadowCascades>(stream.ReadString(element), ShadowCascades.None);
                            break;
                        case "ShadowDistance":
                            this._ShadowDistance = stream.ReadFloat(element);
                            break;
                        case "ShadowProjection":
                            this._ShadowProjection = LoadEnum<ShadowProjection>(stream.ReadString(element), ShadowProjection.StableFit);
                            break;
                        case "SoftVegetation":
                            this._SoftVegetation = stream.ReadBoolean(element);
                            break;
                        case "VSyncCount":
                            this._VSyncCount = LoadEnum<VSyncCount>(stream.ReadString(element), VSyncCount.EveryVBlank);
                            break;
                        case "Bloom":
                            this._Bloom = stream.ReadBoolean(element);
                            break;
                        case "HDR":
                            this._HDR = stream.ReadBoolean(element);
                            break;
                        case "AmbientOcclusion":
                            this._AmbientOcclusion = stream.ReadBoolean(element);
                            break;
                        case "PostprocessQuality":
                            this._PostprocessQuality = stream.ReadInt(element);
                            break;
                        case "ShadowQuality":
                            this._ShadowQuality = stream.ReadInt(element);
                            break;
                        case "ResolutionWidth":
                            this._ResolutionWidth = stream.ReadInt(element);
                            break;
                        case "ResolutionHeight":
                            this._ResolutionHeight = stream.ReadInt(element);
                            break;
                        case "RefreshRate":
                            this._RefreshRate = stream.ReadInt(element);
                            break;
                        case "FullScreen":
                            this._FullScreen = stream.ReadBoolean(element);
                            break;
                        case "QualityLevel":
                            this._QualityLevel = stream.ReadInt(element);
                            break;
                    }
                    element = e.GetNextSibling(element);
                }
                CopyNewValues();
            }
            public void Load(Skill.Framework.IO.BinaryLoadStream stream)
            {
                this.AnisotropicFiltering = (AnisotropicFiltering)FilterValue(stream.ReadInt(), (int)AnisotropicFiltering.Disable, Enum.GetValues(typeof(AnisotropicFiltering)));
                this.AntiAliasing = (AntiAliasing)FilterValue(stream.ReadInt(), (int)AntiAliasing.None, Enum.GetValues(typeof(AntiAliasing)));
                this.BlendWeights = (BlendWeights)FilterValue(stream.ReadInt(), (int)BlendWeights.TwoBones, Enum.GetValues(typeof(BlendWeights)));
                this.LodBias = stream.ReadFloat();
                this.MasterTextureLimit = stream.ReadInt();
                this.MaximumLODLevel = stream.ReadInt();
                this.MaxQueuedFrames = stream.ReadInt();
                this.ParticleRaycastBudget = stream.ReadInt();
                this.PixelLightCount = stream.ReadInt();
                this.ShadowCascades = (ShadowCascades)FilterValue(stream.ReadInt(), (int)ShadowCascades.None, Enum.GetValues(typeof(ShadowCascades)));
                this.ShadowDistance = stream.ReadFloat();
                this.ShadowProjection = (ShadowProjection)FilterValue(stream.ReadInt(), (int)ShadowProjection.StableFit, Enum.GetValues(typeof(ShadowProjection)));
                this.SoftVegetation = stream.ReadBoolean();
                this.VSyncCount = (VSyncCount)FilterValue(stream.ReadInt(), (int)VSyncCount.EveryVBlank, Enum.GetValues(typeof(VSyncCount)));
                this.Bloom = stream.ReadBoolean();
                this.HDR = stream.ReadBoolean();
                this.AmbientOcclusion = stream.ReadBoolean();
                this.PostprocessQuality = stream.ReadInt();
                this.ShadowQuality = stream.ReadInt();
                this._ResolutionWidth = stream.ReadInt();
                this._ResolutionHeight = stream.ReadInt();
                this._RefreshRate = stream.ReadInt();
                this.FullScreen = stream.ReadBoolean();
                this.QualityLevel = stream.ReadInt();
                CopyNewValues();
            }

        }
        public sealed class KeyMap : Skill.Framework.IO.ISavable
        {
            private static Array KeyCodeValues = Enum.GetValues(typeof(KeyCode));

            // Variables
            private KeyCode _PrimaryKey;
            private KeyCode _SecondaryKey;
            private KeyCode _GamepadKey;
            private string _Name;

            // Properties
            public KeyCode PrimaryKey { get { return _PrimaryKey; } set { _PrimaryKey = value; } }
            public KeyCode SecondaryKey { get { return _SecondaryKey; } set { _SecondaryKey = value; } }
            public KeyCode GamepadKey { get { return _GamepadKey; } set { _GamepadKey = value; } }
            public string Name { get { return _Name; } set { _Name = value; } }
            /// <summary>
            /// Index of key
            /// </summary>
            public int Index { get; internal set; }

            // Methods
            public KeyMap()
            {
            }
            public static KeyMap CreateKeyMap()
            {
                return new KeyMap();
            }
            public void Save(IO.XmlElement e, Skill.Framework.IO.XmlSaveStream stream)
            {
                IO.XmlElement _NameElement = stream.Create("Name", _Name);
                e.AppendChild(_NameElement);
                IO.XmlElement _PrimaryKeyElement = stream.Create("PrimaryKey", _PrimaryKey.ToString());
                e.AppendChild(_PrimaryKeyElement);
                IO.XmlElement _SecondaryKeyElement = stream.Create("SecondaryKey", _SecondaryKey.ToString());
                e.AppendChild(_SecondaryKeyElement);
                IO.XmlElement _GamepadKeyKeyElement = stream.Create("GamepadKey", _GamepadKey.ToString());
                e.AppendChild(_GamepadKeyKeyElement);

            }
            public void Save(Skill.Framework.IO.BinarySaveStream stream)
            {
                stream.Write(_Name);
                stream.Write((int)_PrimaryKey);
                stream.Write((int)_SecondaryKey);
                stream.Write((int)_GamepadKey);

            }
            public void Load(IO.XmlElement e, Skill.Framework.IO.XmlLoadStream stream)
            {
                IO.XmlElement element = e.FirstChild as IO.XmlElement;
                while (element != null)
                {
                    switch (element.Name)
                    {
                        case "Name":
                            this.Name = stream.ReadString(element);
                            break;
                        case "PrimaryKey":
                            this.PrimaryKey = LoadEnum<KeyCode>(stream.ReadString(element), KeyCode.None);
                            break;
                        case "SecondaryKey":
                            this.SecondaryKey = LoadEnum<KeyCode>(stream.ReadString(element), KeyCode.None);
                            break;
                        case "GamepadKey":
                            this.GamepadKey = LoadEnum<KeyCode>(stream.ReadString(element), KeyCode.None);
                            break;
                    }
                    element = e.GetNextSibling(element);
                }

            }
            public void Load(Skill.Framework.IO.BinaryLoadStream stream)
            {
                this.Name = stream.ReadString();
                this.PrimaryKey = (KeyCode)FilterValue(stream.ReadInt(), (int)KeyCode.None, KeyCodeValues);
                this.SecondaryKey = (KeyCode)FilterValue(stream.ReadInt(), (int)KeyCode.None, KeyCodeValues);
                this.GamepadKey = (KeyCode)FilterValue(stream.ReadInt(), (int)KeyCode.None, KeyCodeValues);
            }

        }
        public sealed class InputSettings : Skill.Framework.IO.ISavable, IEnumerable<KeyMap>
        {

            // Variables
            private bool _InvertMouseX;
            private bool _InvertMouseY;
            private float _MouseSensitivity;
            private bool _ToggleCrouch;
            private bool _ToggleRun;
            private bool _ToggleAim;
            private bool _Vibration;
            private KeyMap[] _Keys = null;
            private Dictionary<string, KeyMap> _KeyDictionary;

            // Properties
            public bool InvertMouseX { get { return _InvertMouseX; } set { _InvertMouseX = value; } }
            public bool InvertMouseY { get { return _InvertMouseY; } set { _InvertMouseY = value; } }
            public float MouseSensitivity { get { return _MouseSensitivity; } set { _MouseSensitivity = Mathf.Clamp01(value); } }
            public bool ToggleCrouch { get { return _ToggleCrouch; } set { _ToggleCrouch = value; } }
            public bool ToggleRun { get { return _ToggleRun; } set { _ToggleRun = value; } }
            public bool ToggleAim { get { return _ToggleAim; } set { _ToggleAim = value; } }
            public bool Vibration { get { return _Vibration; } set { _Vibration = value; } }

            /// <summary>
            /// Get key by name
            /// </summary>
            /// <param name="keyName">Name of key</param>
            /// <returns>KeyMap</returns>
            public KeyMap this[string keyName]
            {
                get
                {
                    KeyMap result = null;
                    _KeyDictionary.TryGetValue(keyName, out result);
                    return result;
                }
            }

            /// <summary>
            /// Get key by index
            /// </summary>
            /// <param name="keyIndex">Index of key</param>
            /// <returns>KeyMap</returns>
            public KeyMap this[int keyIndex] { get { return _Keys[keyIndex]; } }

            /// <summary>
            /// Retrieves number of keys
            /// </summary>
            public int KeyCount { get { return (_Keys != null) ? _Keys.Length : 0; } }

            /// <summary>
            /// Find index of keyname.(used to access faster later)
            /// </summary>
            /// <param name="keyName">Name of key</param>
            /// <returns>Index of KeyMap</returns>
            public int GetIndex(string keyName)
            {
                if (_Keys != null && _Keys.Length > 0)
                {
                    for (int i = 0; i < _Keys.Length; i++)
                    {
                        if (_Keys[i].Name == keyName)
                            return i;
                    }
                }
                return -1;
            }

            // Methods
            public InputSettings()
            {
                _KeyDictionary = new Dictionary<string, KeyMap>();
            }

            public void SetKeys(KeyMap[] keys)
            {
                if (keys == null || keys.Length == 0)
                    throw new Exception("Invalid Keys");
                _Keys = keys;                
            }

            internal void Build(KeyMap[] defaultKeys)
            {
                if (_Keys == null || _Keys.Length == 0)
                {
                    _Keys = defaultKeys;
                }
                else
                {
                    List<KeyMap> keyList = new List<KeyMap>();
                    for (int i = 0; i < defaultKeys.Length; i++)
                    {
                        KeyMap keymap = defaultKeys[i];
                        if (keymap != null)
                        {
                            foreach (var k in _Keys)
                            {
                                if (k.Name == keymap.Name)
                                {
                                    keymap = k;
                                    break;
                                }
                            }
                        }
                        keyList.Add(keymap);
                    }
                    _Keys = keyList.ToArray();
                }

                if (_Keys == null || _Keys.Length == 0)
                    throw new Exception("Invalid Keys to build");
                _KeyDictionary.Clear();
                for (int i = 0; i < _Keys.Length; i++)
                {
                    KeyMap k = _Keys[i];
                    k.Index = i;
                    _KeyDictionary.Add(k.Name, k);
                }
            }

            public static InputSettings CreateInputSettings()
            {
                return new InputSettings();
            }
            public void Save(IO.XmlElement e, Skill.Framework.IO.XmlSaveStream stream)
            {
                IO.XmlElement _InvertMouseXElement = stream.Create("InvertMouseX", _InvertMouseX);
                e.AppendChild(_InvertMouseXElement);
                IO.XmlElement _InvertMouseYElement = stream.Create("InvertMouseY", _InvertMouseY);
                e.AppendChild(_InvertMouseYElement);
                IO.XmlElement _MouseSensitivityElement = stream.Create("MouseSensitivity", _MouseSensitivity);
                e.AppendChild(_MouseSensitivityElement);
                IO.XmlElement _ToggleCrouchElement = stream.Create("ToggleCrouch", _ToggleCrouch);
                e.AppendChild(_ToggleCrouchElement);
                IO.XmlElement _ToggleRunElement = stream.Create("ToggleRun", _ToggleRun);
                e.AppendChild(_ToggleRunElement);
                IO.XmlElement _ToggleAimElement = stream.Create("ToggleAim", _ToggleAim);
                e.AppendChild(_ToggleAimElement);
                IO.XmlElement _VibrationElement = stream.Create("Vibration", _Vibration);
                e.AppendChild(_VibrationElement);
                IO.XmlElement _KeysElement = stream.Create<KeyMap>("Keys", _Keys);
                e.AppendChild(_KeysElement);

            }
            public void Save(Skill.Framework.IO.BinarySaveStream stream)
            {
                stream.Write(_InvertMouseX);
                stream.Write(_InvertMouseY);
                stream.Write(_MouseSensitivity);
                stream.Write(_ToggleCrouch);
                stream.Write(_ToggleRun);
                stream.Write(_ToggleAim);
                stream.Write(_Vibration);
                stream.Write<KeyMap>(_Keys);

            }
            public void Load(IO.XmlElement e, Skill.Framework.IO.XmlLoadStream stream)
            {
                IO.XmlElement element = e.FirstChild as IO.XmlElement;
                while (element != null)
                {
                    switch (element.Name)
                    {
                        case "InvertMouseX":
                            this.InvertMouseX = stream.ReadBoolean(element);
                            break;
                        case "InvertMouseY":
                            this.InvertMouseY = stream.ReadBoolean(element);
                            break;
                        case "MouseSensitivity":
                            this.MouseSensitivity = stream.ReadFloat(element);
                            break;
                        case "ToggleCrouch":
                            this.ToggleCrouch = stream.ReadBoolean(element);
                            break;
                        case "ToggleRun":
                            this.ToggleRun = stream.ReadBoolean(element);
                            break;
                        case "ToggleAim":
                            this.ToggleAim = stream.ReadBoolean(element);
                            break;
                        case "Vibration":
                            this.Vibration = stream.ReadBoolean(element);
                            break;
                        case "Keys":
                            this._Keys = stream.ReadSavableArray<KeyMap>(element, KeyMap.CreateKeyMap);
                            break;
                    }
                    element = e.GetNextSibling(element);
                }

            }
            public void Load(Skill.Framework.IO.BinaryLoadStream stream)
            {
                this.InvertMouseX = stream.ReadBoolean();
                this.InvertMouseY = stream.ReadBoolean();
                this.MouseSensitivity = stream.ReadFloat();
                this.ToggleCrouch = stream.ReadBoolean();
                this.ToggleRun = stream.ReadBoolean();
                this.ToggleAim = stream.ReadBoolean();
                this.Vibration = stream.ReadBoolean();
                this._Keys = stream.ReadSavableArray<KeyMap>(KeyMap.CreateKeyMap);

            }


            public IEnumerator<KeyMap> GetEnumerator()
            {
                if (_Keys != null) return _Keys.GetEnumerator() as IEnumerator<KeyMap>;
                throw new InvalidOperationException("Keys are empty");
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                if (_Keys != null) return _Keys.GetEnumerator() as IEnumerator<KeyMap>;
                throw new InvalidOperationException("Keys are empty");
            }

            public bool ChangePrimaryKey(string keyName, KeyCode primaryKey, bool ignoreConflict = false)
            {
                KeyMap k = this[keyName];
                if (k != null)
                {
                    k.PrimaryKey = primaryKey;
                    if (!ignoreConflict)
                    {
                        if (k.SecondaryKey == primaryKey) k.SecondaryKey = KeyCode.None;
                        if (k.GamepadKey == primaryKey) k.GamepadKey = KeyCode.None;
                        foreach (var key in _Keys)
                        {
                            if (k == key) continue;
                            if (key.PrimaryKey == primaryKey) key.PrimaryKey = KeyCode.None;
                            if (key.SecondaryKey == primaryKey) key.SecondaryKey = KeyCode.None;
                            if (key.GamepadKey == primaryKey) key.GamepadKey = KeyCode.None;
                        }
                    }
                    return true;
                }
                return false;
            }

            public bool ChangeSecondaryKey(string keyName, KeyCode secondaryKey, bool ignoreConflict = false)
            {
                KeyMap k = this[keyName];
                if (k != null)
                {
                    k.SecondaryKey = secondaryKey;
                    if (!ignoreConflict)
                    {
                        if (k.PrimaryKey == secondaryKey) k.PrimaryKey = KeyCode.None;
                        if (k.GamepadKey == secondaryKey) k.GamepadKey = KeyCode.None;
                        foreach (var key in _Keys)
                        {
                            if (k == key) continue;
                            if (key.PrimaryKey == secondaryKey) key.PrimaryKey = KeyCode.None;
                            if (key.SecondaryKey == secondaryKey) key.SecondaryKey = KeyCode.None;
                            if (key.GamepadKey == secondaryKey) key.GamepadKey = KeyCode.None;
                        }
                    }
                    return true;
                }
                return false;
            }
            public bool ChangeGamepadKey(string keyName, KeyCode gamepadKey, bool ignoreConflict = false)
            {
                KeyMap k = this[keyName];
                if (k != null)
                {
                    k.GamepadKey = gamepadKey;
                    if (!ignoreConflict)
                    {
                        if (k.PrimaryKey == gamepadKey) k.PrimaryKey = KeyCode.None;
                        if (k.SecondaryKey == gamepadKey) k.SecondaryKey = KeyCode.None;
                        foreach (var key in _Keys)
                        {
                            if (k == key) continue;
                            if (key.PrimaryKey == gamepadKey) key.PrimaryKey = KeyCode.None;
                            if (key.SecondaryKey == gamepadKey) key.SecondaryKey = KeyCode.None;
                            if (key.GamepadKey == gamepadKey) key.GamepadKey = KeyCode.None;
                        }
                    }
                    return true;
                }
                return false;
            }

            #region GetKey
            /// <summary>
            /// Returns true while the user holds down the key identified by name. Think auto fire. 
            /// </summary>
            /// <param name="keyName">Name of key</param>
            /// <returns>Ttrue while the user holds down the key, otherwise false</returns>
            public bool GetKey(string keyName)
            {
                return GetKey(_KeyDictionary[keyName]);
            }
            /// <summary>
            /// Returns true while the user holds down the key identified by name. Think auto fire. 
            /// </summary>
            /// <param name="keyIndex">Index of key</param>
            /// <returns>Ttrue while the user holds down the key, otherwise false</returns>
            public bool GetKey(int keyIndex)
            {
                return GetKey(_Keys[keyIndex]);
            }
            private bool GetKey(KeyMap map)
            {
                return UnityEngine.Input.GetKey(map.PrimaryKey) || UnityEngine.Input.GetKey(map.SecondaryKey) || UnityEngine.Input.GetKey(map.GamepadKey);
            } 
            #endregion

            #region GetKeyDown
            /// <summary>
            /// Returns true during the frame the user starts pressing down the key identified by name. 
            /// </summary>
            /// <param name="keyName">Name of key</param>
            /// <returns>True if the user starts pressing down the key, otherwise false</returns>
            public bool GetKeyDown(string keyName)
            {
                return GetKeyDown(_KeyDictionary[keyName]);
            }
            /// <summary>
            /// Returns true during the frame the user starts pressing down the key identified by name. 
            /// </summary>
            /// <param name="keyIndex">Index of key</param>
            /// <returns>True if the user starts pressing down the key, otherwise false</returns>
            public bool GetKeyDown(int keyIndex)
            {
                return GetKeyDown(_Keys[keyIndex]);
            }
            private bool GetKeyDown(KeyMap map)
            {
                return UnityEngine.Input.GetKeyDown(map.PrimaryKey) || UnityEngine.Input.GetKeyDown(map.SecondaryKey) || UnityEngine.Input.GetKeyDown(map.GamepadKey);
            } 
            #endregion

            #region GetKeyUp
            /// <summary>
            /// Returns true during the frame the user releases the key identified by name.
            /// </summary>
            /// <param name="keyName">Name of key</param>
            /// <returns>True if the user releases the key, otherwise false</returns>
            public bool GetKeyUp(string keyName)
            {
                return GetKeyUp(_KeyDictionary[keyName]);
            }
            /// <summary>
            /// Returns true during the frame the user releases the key identified by name.
            /// </summary>
            /// <param name="keyIndex">Index of key</param>
            /// <returns>True if the user releases the key, otherwise false</returns>
            public bool GetKeyUp(int keyIndex)
            {
                return GetKeyUp(_Keys[keyIndex]);
            }
            private bool GetKeyUp(KeyMap map)
            {
                return UnityEngine.Input.GetKeyUp(map.PrimaryKey) || UnityEngine.Input.GetKeyUp(map.SecondaryKey) || UnityEngine.Input.GetKeyUp(map.GamepadKey);
            } 
            #endregion
        }

        // Variables
        private AudioSettings _Audio = null;
        private QualitySettings _Quality = null;
        private InputSettings _Input = null;

        // Properties
        public AudioSettings Audio { get { return _Audio; } set { _Audio = value; } }
        public QualitySettings Quality { get { return _Quality; } set { _Quality = value; } }
        public InputSettings Input { get { return _Input; } set { _Input = value; } }

        // Methods
        public Settings()
        {
            _Audio = new AudioSettings();
            _Quality = new QualitySettings();
            _Input = new InputSettings();

            SetDefaultAudioSettings();
            SetDefaultQualitySettings();
            SetDefaultInputSettings();

            Build();
        }

        private void Build()
        {
            _Quality.CopyNewValues();
            _Input.Build(CreateInputKeys());
        }

        public static Settings CreateSettings()
        {
            return new Settings();
        }
        public void Save(IO.XmlElement e, Skill.Framework.IO.XmlSaveStream stream)
        {
            IO.XmlElement _AudioElement = stream.Create<AudioSettings>("Audio", _Audio);
            e.AppendChild(_AudioElement);
            IO.XmlElement _QualityElement = stream.Create<QualitySettings>("Quality", _Quality);
            e.AppendChild(_QualityElement);
            IO.XmlElement _InputElement = stream.Create<InputSettings>("Input", _Input);
            e.AppendChild(_InputElement);

        }
        public void Save(Skill.Framework.IO.BinarySaveStream stream)
        {
            stream.Write<AudioSettings>(_Audio);
            stream.Write<QualitySettings>(_Quality);
            stream.Write<InputSettings>(_Input);

        }
        public void Load(IO.XmlElement e, Skill.Framework.IO.XmlLoadStream stream)
        {
            IO.XmlElement element = e.FirstChild as IO.XmlElement;
            while (element != null)
            {
                switch (element.Name)
                {
                    case "Audio":
                        this._Audio = stream.ReadSavable<AudioSettings>(element, AudioSettings.CreateAudioSettings);
                        break;
                    case "Quality":
                        this._Quality = stream.ReadSavable<QualitySettings>(element, QualitySettings.CreateQualitySettings);
                        break;
                    case "Input":
                        this._Input = stream.ReadSavable<InputSettings>(element, InputSettings.CreateInputSettings);
                        break;
                }
                element = e.GetNextSibling(element);
            }
            Build();
        }
        public void Load(Skill.Framework.IO.BinaryLoadStream stream)
        {
            this._Audio = stream.ReadSavable<AudioSettings>(AudioSettings.CreateAudioSettings);
            this._Quality = stream.ReadSavable<QualitySettings>(QualitySettings.CreateQualitySettings);
            this._Input = stream.ReadSavable<InputSettings>(InputSettings.CreateInputSettings);
            Build();
        }

        public virtual void SetDefaultAudioSettings()
        {
            _Audio.Subtitle = false;
            _Audio.FxVolume = 1.0f;
            _Audio.VoiceVolume = 1.0f;
            _Audio.MusicVolume = 1.0f;
            _Audio.CinematicVolume = 1.0f;
            _Audio.MasterVolume = 1.0f;
            _Audio.OutputSampleRate = 44100;
            _Audio.SpeakerMode = AudioSpeakerMode.Stereo;
        }

        public virtual void SetDefaultQualitySettings()
        {
            _Quality.AnisotropicFiltering = UnityEngine.AnisotropicFiltering.Enable;
            _Quality.AntiAliasing = Settings.AntiAliasing.None;
            _Quality.BlendWeights = UnityEngine.BlendWeights.TwoBones;
            _Quality.LodBias = 1;
            _Quality.MasterTextureLimit = 0;
            _Quality.MaximumLODLevel = 0;
            _Quality.MaxQueuedFrames = -1;
            _Quality.ParticleRaycastBudget = 256;
            _Quality.PixelLightCount = 2;
            _Quality.ShadowCascades = Settings.ShadowCascades.TowCascades;
            _Quality.ShadowDistance = 40;
            _Quality.ShadowProjection = UnityEngine.ShadowProjection.StableFit;
            _Quality.SoftVegetation = false;
            _Quality.VSyncCount = Settings.VSyncCount.EveryVBlank;
            _Quality.Bloom = true;
            _Quality.HDR = false;
            _Quality.AmbientOcclusion = false;
            _Quality.PostprocessQuality = 1;
            _Quality.ShadowQuality = 1;
            _Quality.Resolution = new Resolution() { width = 1024, height = 768, refreshRate = 60 };
            _Quality.QualityLevel = 0;
            _Quality.FullScreen = false;
        }

        public virtual void SetDefaultInputSettings()
        {
            _Input.InvertMouseX = false;
            _Input.InvertMouseY = false;
            _Input.MouseSensitivity = 0.5f;
            _Input.ToggleCrouch = false;
            _Input.ToggleRun = false;
            _Input.ToggleAim = false;
            _Input.SetKeys(CreateInputKeys());
        }

        protected virtual KeyMap[] CreateInputKeys()
        {
            return new KeyMap[]
                {
                    new KeyMap(){ Name = "Horizontal+" , PrimaryKey = KeyCode.A },
                    new KeyMap(){ Name = "Horizontal-" , PrimaryKey = KeyCode.D },
                    new KeyMap(){ Name = "Vertical+" , PrimaryKey = KeyCode.W  },
                    new KeyMap(){ Name = "Vertical-" , PrimaryKey = KeyCode.S },
                    new KeyMap(){ Name = "Fire1" , PrimaryKey = KeyCode.Mouse0 , SecondaryKey = KeyCode.LeftControl  },
                    new KeyMap(){ Name = "Fire2" , PrimaryKey = KeyCode.Mouse1 , SecondaryKey = KeyCode.LeftAlt},
                    new KeyMap(){ Name = "Fire3" , PrimaryKey = KeyCode.Mouse2 , SecondaryKey = KeyCode.LeftCommand},
                    new KeyMap(){ Name = "Jump" , PrimaryKey = KeyCode.Space},
                    new KeyMap(){ Name = "Action" , PrimaryKey = KeyCode.E },
                    new KeyMap(){ Name = "Crouch" , PrimaryKey = KeyCode.C},                    
                };
        }
    }
}