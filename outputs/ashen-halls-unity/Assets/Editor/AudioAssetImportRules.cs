using System;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public sealed class AudioAssetImportRules : AssetPostprocessor
    {
        internal const string AuthoredSfxPathPrefix = "Assets/Resources/Audio/Sfx/";
        internal const string OriginalMusicPathPrefix = "Assets/Resources/Audio/Music/";

        private void OnPreprocessAudio()
        {
            string normalizedPath = (assetPath ?? "").Replace('\\', '/');
            AudioImporter importer = (AudioImporter)assetImporter;
            AudioImporterSampleSettings settings = importer.defaultSampleSettings;
            if (normalizedPath.StartsWith(AuthoredSfxPathPrefix, StringComparison.OrdinalIgnoreCase))
            {
                importer.forceToMono = true;
                importer.loadInBackground = false;
                settings.loadType = AudioClipLoadType.DecompressOnLoad;
                settings.compressionFormat = AudioCompressionFormat.PCM;
                settings.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
                settings.preloadAudioData = true;
                importer.defaultSampleSettings = settings;
                return;
            }

            if (normalizedPath.StartsWith(OriginalMusicPathPrefix, StringComparison.OrdinalIgnoreCase))
            {
                importer.forceToMono = false;
                importer.loadInBackground = true;
                settings.loadType = AudioClipLoadType.CompressedInMemory;
                settings.compressionFormat = AudioCompressionFormat.Vorbis;
                settings.quality = 0.68f;
                settings.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
                settings.preloadAudioData = true;
                importer.defaultSampleSettings = settings;
            }
        }
    }
}
