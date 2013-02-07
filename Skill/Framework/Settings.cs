using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;

namespace Skill.Framework
{
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

        public class AudioSettings : Skill.Framework.IO.ISavable
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
            public float FxVolume { get { return _FxVolume; } set { SetValidVolume(ref _FxVolume, value); } }
            public float VoiceVolume { get { return _VoiceVolume; } set { SetValidVolume(ref _VoiceVolume, value); } }
            public float MusicVolume { get { return _MusicVolume; } set { SetValidVolume(ref _MusicVolume, value); } }
            public float CinematicVolume { get { return _CinematicVolume; } set { SetValidVolume(ref _CinematicVolume, value); } }
            public float MasterVolume { get { return _MasterVolume; } set { SetValidVolume(ref _MasterVolume, value); } }
            public int OutputSampleRate { get { return _OutputSampleRate; } set { _OutputSampleRate = value; } }
            public AudioSpeakerMode SpeakerMode { get { return _SpeakerMode; } set { _SpeakerMode = value; } }

            public float GetVolume(Skill.Framework.Sounds.SoundCategory category)
            {
                float volume = 1.0f;

                switch (category)
                {
                    case Skill.Framework.Sounds.SoundCategory.FX:
                        volume = FxVolume;
                        break;
                    case Skill.Framework.Sounds.SoundCategory.Music:
                        volume = MusicVolume;
                        break;
                    case Skill.Framework.Sounds.SoundCategory.Voice:
                        volume = VoiceVolume;
                        break;
                    case Skill.Framework.Sounds.SoundCategory.Cinematic:
                        volume = CinematicVolume;
                        break;
                    default:
                        volume = 1.0f;
                        break;
                }
                return volume * MasterVolume;
            }

            private void SetValidVolume(ref float volume, float value)
            {
                if (value < 0) value = 0;
                else if (value > 1.0f) value = 1.0f;
                volume = value;
            }

            public virtual void SetDefault()
            {
                _Subtitle = false;
                _FxVolume = 1.0f;
                _VoiceVolume = 1.0f;
                _MusicVolume = 1.0f;
                _CinematicVolume = 1.0f;
                _MasterVolume = 1.0f;
                _OutputSampleRate = 44100;
                _SpeakerMode = AudioSpeakerMode.Stereo;
            }

            public virtual void ApplyChanges()
            {
                UnityEngine.AudioSettings.outputSampleRate = _OutputSampleRate;
                UnityEngine.AudioSettings.speakerMode = _SpeakerMode;
            }

            // Methods
            public AudioSettings()
            {
                SetDefault();
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
                IO.XmlElement _SpeakerModeElement = stream.Create("SpeakerMode", (int)_SpeakerMode);
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
                            this._Subtitle = stream.ReadBoolean(element);
                            break;
                        case "FxVolume":
                            this._FxVolume = stream.ReadFloat(element);
                            break;
                        case "VoiceVolume":
                            this._VoiceVolume = stream.ReadFloat(element);
                            break;
                        case "MusicVolume":
                            this._MusicVolume = stream.ReadFloat(element);
                            break;
                        case "CinematicVolume":
                            this._CinematicVolume = stream.ReadFloat(element);
                            break;
                        case "MasterVolume":
                            this._MasterVolume = stream.ReadFloat(element);
                            break;
                        case "OutputSampleRate":
                            this._OutputSampleRate = stream.ReadInt(element);
                            break;
                        case "SpeakerMode":
                            this._SpeakerMode = (AudioSpeakerMode)stream.ReadInt(element);
                            break;
                    }
                    element = element.NextSibling as IO.XmlElement;
                }

            }
            public void Load(Skill.Framework.IO.BinaryLoadStream stream)
            {
                this._Subtitle = stream.ReadBoolean();
                this._FxVolume = stream.ReadFloat();
                this._VoiceVolume = stream.ReadFloat();
                this._MusicVolume = stream.ReadFloat();
                this._CinematicVolume = stream.ReadFloat();
                this._MasterVolume = stream.ReadFloat();
                this._OutputSampleRate = stream.ReadInt();
                this._SpeakerMode = (AudioSpeakerMode)stream.ReadInt();

            }

        }
        public class QualitySettings : Skill.Framework.IO.ISavable
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
            public int MasterTextureLimit { get { return _MasterTextureLimit; } set { _MasterTextureLimit = value; } }
            /// <summary>   A maximum LOD level. All LOD groups </summary>
            public int MaximumLODLevel { get { return _MaximumLODLevel; } set { _MaximumLODLevel = value; } }
            /// <summary> Maximum number of frames queued up by graphics driver. </summary>
            public int MaxQueuedFrames { get { return _MaxQueuedFrames; } set { _MaxQueuedFrames = value; } }
            /// <summary> Budget for how many ray casts can be performed per frame for approximate collision testing. </summary>
            public int ParticleRaycastBudget { get { return _ParticleRaycastBudget; } set { _ParticleRaycastBudget = value; } }
            /// <summary> The maximum number of pixel lights that should affect any object. </summary>
            public int PixelLightCount { get { return _PixelLightCount; } set { _PixelLightCount = value; } }
            /// <summary>   Number of cascades to use for directional light shadows. </summary>
            public ShadowCascades ShadowCascades { get { return _ShadowCascades; } set { _ShadowCascades = value; } }
            /// <summary>   Shadow drawing distance. </summary>
            public float ShadowDistance { get { return _ShadowDistance; } set { _ShadowDistance = value; } }
            /// <summary> Directional light shadow projection. </summary>
            public ShadowProjection ShadowProjection { get { return _ShadowProjection; } set { _ShadowProjection = value; } }
            /// <summary> Use a two-pass shader for the vegetation in the terrain engine. </summary>
            public bool SoftVegetation { get { return _SoftVegetation; } set { _SoftVegetation = value; } }
            /// <summary> The VSync Count. </summary>
            public VSyncCount VSyncCount { get { return _VSyncCount; } set { _VSyncCount = value; } }
            /// <summary> </summary>
            public bool Bloom { get { return _Bloom; } set { _Bloom = value; } }
            /// <summary> </summary>
            public bool HDR { get { return _HDR; } set { _HDR = value; } }
            /// <summary> </summary>
            public bool AmbientOcclusion { get { return _AmbientOcclusion; } set { _AmbientOcclusion = value; } }
            /// <summary> </summary>
            public int PostprocessQuality { get { return _PostprocessQuality; } set { _PostprocessQuality = value; } }
            /// <summary> </summary>
            public int ShadowQuality { get { return _ShadowQuality; } set { _ShadowQuality = value; } }
            /// <summary> </summary>
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
            /// <summary> </summary>
            public bool FullScreen { get { return _FullScreen; } set { _FullScreen = value; } }
            /// <summary> Graphics quality level. </summary>
            public int QualityLevel { get { return _QualityLevel; } set { if (value < 0)value = 0; _QualityLevel = value; } }

            /// <summary>
            /// if true, ApplyChanges() will override settings that defined in ProjectSettings->Quality.
            /// This property will not saved to file
            /// </summary>
            public bool OverrideUnityQualitySettings { get; set; }

            private void CopyNewValues()
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

            public virtual void SetDefault()
            {
                _AnisotropicFiltering = UnityEngine.AnisotropicFiltering.Enable;
                _AntiAliasing = Settings.AntiAliasing.None;
                _BlendWeights = UnityEngine.BlendWeights.TwoBones;
                _LodBias = 1;
                _MasterTextureLimit = 0;
                _MaximumLODLevel = 0;
                _MaxQueuedFrames = -1;
                _ParticleRaycastBudget = 256;
                _PixelLightCount = 2;
                _ShadowCascades = Settings.ShadowCascades.TowCascades;
                _ShadowDistance = 40;
                _ShadowProjection = UnityEngine.ShadowProjection.StableFit;
                _SoftVegetation = false;
                _VSyncCount = Settings.VSyncCount.EveryVBlank;
                _Bloom = true;
                _HDR = false;
                _AmbientOcclusion = false;
                _PostprocessQuality = 1;
                _ShadowQuality = 1;
                _ResolutionWidth = 1024;
                _ResolutionHeight = 768;
                _RefreshRate = 60;
                _QualityLevel = 0;
            }

            public virtual void AcceptChanges(bool applyExpensiveChanges = true)
            {
                CopyNewValues();
                ApplyChanges();
            }

            public virtual void RevertChanges(bool applyExpensiveChanges = true)
            {
                CopyOldValues();
                ApplyChanges();
            }

            public virtual void ApplyChanges(bool applyExpensiveChanges = true)
            {
                if (QualityLevel > UnityEngine.QualitySettings.names.Length - 1)
                    QualityLevel = UnityEngine.QualitySettings.names.Length - 1;
                UnityEngine.QualitySettings.SetQualityLevel(QualityLevel, applyExpensiveChanges);

                if (OverrideUnityQualitySettings)
                {
                    if (applyExpensiveChanges)
                    {
                        UnityEngine.QualitySettings.anisotropicFiltering = AnisotropicFiltering;
                        UnityEngine.QualitySettings.antiAliasing = (int)AntiAliasing;
                    }
                    
                    UnityEngine.QualitySettings.blendWeights = BlendWeights;
                    UnityEngine.QualitySettings.lodBias = LodBias;
                    UnityEngine.QualitySettings.masterTextureLimit = MasterTextureLimit;
                    UnityEngine.QualitySettings.maximumLODLevel = MaximumLODLevel;
                    UnityEngine.QualitySettings.maxQueuedFrames = MaxQueuedFrames;
                    UnityEngine.QualitySettings.particleRaycastBudget = ParticleRaycastBudget;
                    UnityEngine.QualitySettings.pixelLightCount = PixelLightCount;
                    UnityEngine.QualitySettings.shadowCascades = (int)ShadowCascades;
                    UnityEngine.QualitySettings.shadowDistance = ShadowDistance;
                    UnityEngine.QualitySettings.shadowProjection = ShadowProjection;
                    UnityEngine.QualitySettings.softVegetation = SoftVegetation;
                    UnityEngine.QualitySettings.vSyncCount = (int)VSyncCount;
                }

                Resolution res = Resolution;
                UnityEngine.Screen.SetResolution(res.width, res.height, FullScreen, res.refreshRate);
            }

            // Methods
            public QualitySettings()
            {
                SetDefault();
                CopyNewValues();
            }
            public static QualitySettings CreateQualitySettings()
            {
                return new QualitySettings();
            }
            public void Save(IO.XmlElement e, Skill.Framework.IO.XmlSaveStream stream)
            {
                IO.XmlElement _AnisotropicFilteringElement = stream.Create("AnisotropicFiltering", (int)_AnisotropicFiltering);
                e.AppendChild(_AnisotropicFilteringElement);
                IO.XmlElement _AntiAliasingElement = stream.Create("AntiAliasing", (int)_AntiAliasing);
                e.AppendChild(_AntiAliasingElement);
                IO.XmlElement _BlendWeightsElement = stream.Create("BlendWeights", (int)_BlendWeights);
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
                IO.XmlElement _ShadowCascadesElement = stream.Create("ShadowCascades", (int)_ShadowCascades);
                e.AppendChild(_ShadowCascadesElement);
                IO.XmlElement _ShadowDistanceElement = stream.Create("ShadowDistance", _ShadowDistance);
                e.AppendChild(_ShadowDistanceElement);
                IO.XmlElement _ShadowProjectionElement = stream.Create("ShadowProjection", (int)_ShadowProjection);
                e.AppendChild(_ShadowProjectionElement);
                IO.XmlElement _SoftVegetationElement = stream.Create("SoftVegetation", _SoftVegetation);
                e.AppendChild(_SoftVegetationElement);
                IO.XmlElement _VSyncCountElement = stream.Create("VSyncCount", (int)_VSyncCount);
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
                            this._AnisotropicFiltering = (AnisotropicFiltering)stream.ReadInt(element);
                            break;
                        case "AntiAliasing":
                            this._AntiAliasing = (AntiAliasing)stream.ReadInt(element);
                            break;
                        case "BlendWeights":
                            this._BlendWeights = (BlendWeights)stream.ReadInt(element);
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
                            this._ShadowCascades = (ShadowCascades)stream.ReadInt(element);
                            break;
                        case "ShadowDistance":
                            this._ShadowDistance = stream.ReadFloat(element);
                            break;
                        case "ShadowProjection":
                            this._ShadowProjection = (ShadowProjection)stream.ReadInt(element);
                            break;
                        case "SoftVegetation":
                            this._SoftVegetation = stream.ReadBoolean(element);
                            break;
                        case "VSyncCount":
                            this._VSyncCount = (VSyncCount)stream.ReadInt(element);
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
                    element = element.NextSibling as IO.XmlElement;
                }
                CopyNewValues();
            }
            public void Load(Skill.Framework.IO.BinaryLoadStream stream)
            {
                this._AnisotropicFiltering = (AnisotropicFiltering)stream.ReadInt();
                this._AntiAliasing = (AntiAliasing)stream.ReadInt();
                this._BlendWeights = (BlendWeights)stream.ReadInt();
                this._LodBias = stream.ReadFloat();
                this._MasterTextureLimit = stream.ReadInt();
                this._MaximumLODLevel = stream.ReadInt();
                this._MaxQueuedFrames = stream.ReadInt();
                this._ParticleRaycastBudget = stream.ReadInt();
                this._PixelLightCount = stream.ReadInt();
                this._ShadowCascades = (ShadowCascades)stream.ReadInt();
                this._ShadowDistance = stream.ReadFloat();
                this._ShadowProjection = (ShadowProjection)stream.ReadInt();
                this._SoftVegetation = stream.ReadBoolean();
                this._VSyncCount = (VSyncCount)stream.ReadInt();
                this._Bloom = stream.ReadBoolean();
                this._HDR = stream.ReadBoolean();
                this._AmbientOcclusion = stream.ReadBoolean();
                this._PostprocessQuality = stream.ReadInt();
                this._ShadowQuality = stream.ReadInt();
                this._ResolutionWidth = stream.ReadInt();
                this._ResolutionHeight = stream.ReadInt();
                this._RefreshRate = stream.ReadInt();
                this._FullScreen = stream.ReadBoolean();
                this._QualityLevel = stream.ReadInt();
                CopyNewValues();
            }

        }
        public class KeyMap : Skill.Framework.IO.ISavable
        {

            // Variables
            private KeyCode _PrimaryKey;
            private KeyCode _SecondaryKey;
            private string _Name;

            // Properties
            public KeyCode PrimaryKey { get { return _PrimaryKey; } set { _PrimaryKey = value; } }
            public KeyCode SecondaryKey { get { return _SecondaryKey; } set { _SecondaryKey = value; } }
            public string Name { get { return _Name; } set { _Name = value; } }


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
                IO.XmlElement _PrimaryKeyElement = stream.Create("PrimaryKey", (int)_PrimaryKey);
                e.AppendChild(_PrimaryKeyElement);
                IO.XmlElement _SecondaryKeyElement = stream.Create("SecondaryKey", (int)_SecondaryKey);
                e.AppendChild(_SecondaryKeyElement);

            }
            public void Save(Skill.Framework.IO.BinarySaveStream stream)
            {
                stream.Write(_Name);
                stream.Write((int)_PrimaryKey);
                stream.Write((int)_SecondaryKey);

            }
            public void Load(IO.XmlElement e, Skill.Framework.IO.XmlLoadStream stream)
            {
                IO.XmlElement element = e.FirstChild as IO.XmlElement;
                while (element != null)
                {
                    switch (element.Name)
                    {
                        case "Name":
                            this._Name = stream.ReadString(element);
                            break;
                        case "PrimaryKey":
                            this._PrimaryKey = (KeyCode)stream.ReadInt(element);
                            break;
                        case "SecondaryKey":
                            this._SecondaryKey = (KeyCode)stream.ReadInt(element);
                            break;
                    }
                    element = element.NextSibling as IO.XmlElement;
                }

            }
            public void Load(Skill.Framework.IO.BinaryLoadStream stream)
            {
                this._Name = stream.ReadString();
                this._PrimaryKey = (KeyCode)stream.ReadInt();
                this._SecondaryKey = (KeyCode)stream.ReadInt();

            }

        }
        public class InputSettings : Skill.Framework.IO.ISavable
        {

            // Variables
            private bool _InvertMouseX;
            private bool _InvertMouseY;
            private float _MouseSensitivity;
            private KeyMap[] _Keys = null;
            private Dictionary<string, KeyMap> _KeyDictionary;

            // Properties
            public bool InvertMouseX { get { return _InvertMouseX; } set { _InvertMouseX = value; } }
            public bool InvertMouseY { get { return _InvertMouseY; } set { _InvertMouseY = value; } }
            public float MouseSensitivity { get { return _MouseSensitivity; } set { _MouseSensitivity = value; } }
            public KeyMap[] Keys
            {
                get { return _Keys; }
                internal set
                {
                    _Keys = value;
                    _KeyDictionary.Clear();
                    if (_Keys != null)
                    {
                        foreach (var k in _Keys)
                        {
                            _KeyDictionary.Add(k.Name, k);
                        }
                    }
                }
            }

            /// <summary>
            /// Get key by name
            /// </summary>
            /// <param name="keyName">Name of key</param>
            /// <returns></returns>
            public KeyMap GetKey(string keyName)
            {
                return _KeyDictionary[keyName];
            }

            /// <summary>
            /// Find index of keyname.(used to access faster later)
            /// </summary>
            /// <param name="keyName">Name of key</param>
            /// <returns></returns>
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
                Keys = CreateKeys();
            }

            protected virtual KeyMap[] CreateKeys()
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
                    new KeyMap(){ Name = "Junp" , PrimaryKey = KeyCode.Space},
                    new KeyMap(){ Name = "Action" , PrimaryKey = KeyCode.E },
                    new KeyMap(){ Name = "Crouch" , PrimaryKey = KeyCode.C},                    
                };
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
                IO.XmlElement _KeysElement = stream.Create<KeyMap>("Keys", _Keys);
                e.AppendChild(_KeysElement);

            }
            public void Save(Skill.Framework.IO.BinarySaveStream stream)
            {
                stream.Write(_InvertMouseX);
                stream.Write(_InvertMouseY);
                stream.Write(_MouseSensitivity);
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
                            this._InvertMouseX = stream.ReadBoolean(element);
                            break;
                        case "InvertMouseY":
                            this._InvertMouseY = stream.ReadBoolean(element);
                            break;
                        case "MouseSensitivity":
                            this._MouseSensitivity = stream.ReadFloat(element);
                            break;
                        case "Keys":
                            this._Keys = stream.ReadSavableArray<KeyMap>(element, KeyMap.CreateKeyMap);
                            break;
                    }
                    element = element.NextSibling as IO.XmlElement;
                }

            }
            public void Load(Skill.Framework.IO.BinaryLoadStream stream)
            {
                this._InvertMouseX = stream.ReadBoolean();
                this._InvertMouseY = stream.ReadBoolean();
                this._MouseSensitivity = stream.ReadFloat();
                this._Keys = stream.ReadSavableArray<KeyMap>(KeyMap.CreateKeyMap);

            }

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
            _Audio = CreateAudioSettings();
            _Quality = CreateQualitySettings();
            _Input = CreateInputSettings();
        }

        protected virtual AudioSettings CreateAudioSettings()
        {
            return new AudioSettings();
        }
        protected virtual QualitySettings CreateQualitySettings()
        {
            return new QualitySettings();
        }
        protected virtual InputSettings CreateInputSettings()
        {
            return new InputSettings();
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
                element = element.NextSibling as IO.XmlElement;
            }

        }
        public void Load(Skill.Framework.IO.BinaryLoadStream stream)
        {
            this._Audio = stream.ReadSavable<AudioSettings>(AudioSettings.CreateAudioSettings);
            this._Quality = stream.ReadSavable<QualitySettings>(QualitySettings.CreateQualitySettings);
            this._Input = stream.ReadSavable<InputSettings>(InputSettings.CreateInputSettings);

        }

    }
}