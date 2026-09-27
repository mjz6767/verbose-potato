using System.Collections.Generic;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private readonly Dictionary<string, Texture2D> partySetupPortraitAtlases = new Dictionary<string, Texture2D>();

        private Texture2D PartySetupPortraitAtlas(string race, string classKey)
        {
            string fileName = CharacterCreationCatalog.PortraitAtlasFile(race, classKey);
            if (fileName == null) return null;
            if (partySetupPortraitAtlases.TryGetValue(fileName, out Texture2D cached)) return cached;

            Texture2D texture = LoadExternalPng(fileName);
            if (texture != null && !CharacterCreationCatalog.IsValidAtlas(texture))
            {
                Debug.LogWarning($"Rejected character portrait atlas '{fileName}': expected a 4x2 grid of square cells, each at least 256 pixels.");
                DestroyPortraitTexture(texture);
                texture = null;
            }
            if (texture != null) texture.filterMode = FilterMode.Bilinear;
            // Cache failures as well as successes, so a missing package cannot trigger disk reads every frame.
            partySetupPortraitAtlases[fileName] = texture;
            return texture;
        }

        private int PartySetupPortraitCell(string race, string classKey) => CharacterCreationCatalog.PortraitCell(race, classKey);

        private void ReleasePartySetupPortraitArt()
        {
            foreach (Texture2D texture in partySetupPortraitAtlases.Values) DestroyPortraitTexture(texture);
            partySetupPortraitAtlases.Clear();
        }

        private static void DestroyPortraitTexture(Texture2D texture)
        {
            if (texture == null) return;
            if (Application.isPlaying) Destroy(texture);
            else DestroyImmediate(texture);
        }
    }
}
