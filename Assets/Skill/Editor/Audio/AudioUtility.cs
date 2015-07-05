using UnityEngine;
using UnityEditor;
using System.Collections;
namespace Skill.Editor.Audio
{
    public static class AudioUtility
    {
        private static string GetCurrentPlatformString()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.OSXDashboardPlayer:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "Standalone";
                case RuntimePlatform.OSXWebPlayer:
                case RuntimePlatform.WindowsWebPlayer:
                    return "WebPlayer";
                case RuntimePlatform.PS3:
                    return "PS3";
                case RuntimePlatform.PS4:
                    return "PS4";
                case RuntimePlatform.PSP2:
                    return "PSP2";
                case RuntimePlatform.WP8Player:
                    return "WP8";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.XBOX360:
                    return "XBox360";
                case RuntimePlatform.XboxOne:
                    return "XBoxOne";

                case RuntimePlatform.WSAPlayerARM:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.WSAPlayerX86:
                    //i don't know what is the string
                    return "Standalone";
                default:
                    return "default";
            }

        }

        public static float[] GetAudioClipData(AudioClip audio)
        {
            float[] samples = new float[audio.samples * audio.channels];

            string path = AssetDatabase.GetAssetPath(audio);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;            

            //AudioImporterSampleSettings sampleSettingsBackup = audioImporter.GetOverrideSampleSettings(GetCurrentPlatformString());
            AudioImporterSampleSettings sampleSettingsBackup = audioImporter.defaultSampleSettings;
            //workaround to prevent the error in the function getData
            //when Audio Importer loadType is "compressed in memory"
            if (sampleSettingsBackup.loadType != AudioClipLoadType.DecompressOnLoad)
            {
                AudioImporterSampleSettings newSampleSettings = sampleSettingsBackup;
                newSampleSettings.loadType = AudioClipLoadType.DecompressOnLoad;
                //audioImporter.SetOverrideSampleSettings(GetCurrentPlatformString(), newSampleSettings);
                audioImporter.defaultSampleSettings = newSampleSettings;
                AssetDatabase.ImportAsset(path);

                //getData after the loadType changed
                audio.GetData(samples, 0);

                //restore the loadType (end of workaround)
                //audioImporter.SetOverrideSampleSettings(GetCurrentPlatformString(), sampleSettingsBackup);
                audioImporter.defaultSampleSettings = sampleSettingsBackup;
                AssetDatabase.ImportAsset(path);
            }
            else
            {
                //getData after the loadType changed
                audio.GetData(samples, 0);
            }

            return samples;
        }
    }
}