using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AshenHalls
{
    public readonly struct PartySetupFolioSnapshot
    {
        public readonly bool Visible;
        public readonly bool ReducedMotion;
        public readonly bool TransitionActive;
        public readonly float PreviousPortraitAlpha;
        public readonly float AccentAlpha;
        public readonly float AmbientTime;
        public readonly float FirelightAlpha;
        public readonly float SelectionGlowAlpha;
        public readonly int DecorativeGraphicCount;

        public PartySetupFolioSnapshot(bool visible, bool reducedMotion, bool transitionActive,
            float previousPortraitAlpha, float accentAlpha, float ambientTime, float firelightAlpha,
            float selectionGlowAlpha, int decorativeGraphicCount)
        {
            Visible = visible;
            ReducedMotion = reducedMotion;
            TransitionActive = transitionActive;
            PreviousPortraitAlpha = previousPortraitAlpha;
            AccentAlpha = accentAlpha;
            AmbientTime = ambientTime;
            FirelightAlpha = firelightAlpha;
            SelectionGlowAlpha = selectionGlowAlpha;
            DecorativeGraphicCount = decorativeGraphicCount;
        }
    }

    public static class PartySetupFolioAnimationRules
    {
        public const float CrossfadeSeconds = 0.28f;
        public const float AccentSeconds = 0.75f;
        public const int EmberCount = 8;

        public static PartySetupFolioSnapshot Evaluate(float transitionTime, float ambientTime,
            bool visible, bool reducedMotion, bool hasPreviousPortrait, int graphicCount)
        {
            if (!visible) return new PartySetupFolioSnapshot(false, reducedMotion, false, 0f, 0f, ambientTime, 0f, 0f, graphicCount);
            if (reducedMotion) return new PartySetupFolioSnapshot(true, true, false, 0f, 0f, 0f, 0.10f, 0.16f, graphicCount);
            float fade = Mathf.Clamp01(transitionTime / CrossfadeSeconds);
            fade = fade * fade * (3f - 2f * fade);
            float previous = hasPreviousPortrait ? 1f - fade : 0f;
            float accent = Mathf.Clamp01(1f - transitionTime / AccentSeconds);
            accent = accent * accent * 0.68f;
            float fire = 0.105f + 0.018f * Mathf.Sin(ambientTime * 1.35f) + 0.008f * Mathf.Sin(ambientTime * 3.1f);
            float glow = 0.16f + 0.045f * Mathf.Sin(ambientTime * 1.05f);
            return new PartySetupFolioSnapshot(true, false, transitionTime < AccentSeconds,
                previous, accent, ambientTime, fire, glow, graphicCount);
        }

        public static Vector2 EmberPosition(int index, float time)
        {
            float progress = Mathf.Repeat(time / (6f + index * 0.37f) + index * 0.173f, 1f);
            float x = index % 2 == 0 ? 11f : 1269f;
            return new Vector2(x + Mathf.Sin(time * 0.7f + index) * 3f, 665f - progress * 550f);
        }

        public static float EmberAlpha(int index, float time)
        {
            float progress = Mathf.Repeat(time / (6f + index * 0.37f) + index * 0.173f, 1f);
            return Mathf.Sin(progress * Mathf.PI) * 0.55f;
        }

        public static Color PortraitAccent(string race, string characterClass)
        {
            Color ancestry;
            switch (race)
            {
                case "dusk elf": ancestry = new Color32(157, 145, 219, 255); break;
                case "stoneborn": ancestry = new Color32(175, 183, 181, 255); break;
                case "fenkin": ancestry = new Color32(153, 186, 103, 255); break;
                case "ashling": ancestry = new Color32(235, 141, 66, 255); break;
                default: ancestry = new Color32(218, 183, 118, 255); break;
            }
            Color calling;
            switch (characterClass)
            {
                case "rogue": calling = new Color32(161, 176, 198, 255); break;
                case "warrior": calling = new Color32(215, 155, 103, 255); break;
                case "ranger": calling = new Color32(157, 196, 115, 255); break;
                case "wizard": calling = new Color32(112, 170, 235, 255); break;
                case "mage": calling = new Color32(246, 148, 66, 255); break;
                case "warlock": calling = new Color32(183, 124, 223, 255); break;
                case "priest": calling = new Color32(241, 225, 158, 255); break;
                default: calling = new Color32(236, 195, 113, 255); break;
            }
            return Color.Lerp(ancestry, calling, 0.65f);
        }
    }

    /// <summary>Reusable decoration only. The current portrait and every control remain in their final layout.</summary>
    public sealed class PartySetupFolioEffects : MonoBehaviour
    {
        private readonly List<Button> buttons = new List<Button>();
        private readonly Image[] embers = new Image[PartySetupFolioAnimationRules.EmberCount];
        private readonly Image[] corners = new Image[8];
        private readonly FolioLightGraphic[] selectionGlows = new FolioLightGraphic[4];
        private readonly Transform[] selectionParents = new Transform[4];
        private RawImage portrait;
        private RawImage previousPortrait;
        private FolioLightGraphic portraitGlow;
        private FolioLightGraphic hearthGlow;
        private FolioLightGraphic windowGlow;
        private Func<bool> reducedMotion;
        private Color themeColor = new Color32(218, 183, 118, 255);
        private float transitionTime = PartySetupFolioAnimationRules.AccentSeconds;
        private float ambientTime;
        private bool visible;
        private bool reduced;
        private bool hasPrevious;
        private bool hasIdentity;
        private int identityMember = -1;
        private string identityRace;
        private string identityClass;
        private int graphicCount;
        private PartySetupFolioSnapshot snapshot;

        public PartySetupFolioSnapshot Snapshot => snapshot;

        public void Initialize(RawImage selectedPortrait, Func<bool> reducedMotionProvider)
        {
            portrait = selectedPortrait;
            reducedMotion = reducedMotionProvider;
            hearthGlow = Light("Hearth Firelight", transform, true, new Rect(-115f, 125f, 340f, 630f));
            windowGlow = Light("Window Firelight", transform, true, new Rect(1130f, 90f, 240f, 640f));
            for (int i = 0; i < embers.Length; i++)
            {
                embers[i] = DecorationImage("Margin Ember " + i, transform);
                embers[i].rectTransform.localRotation = Quaternion.Euler(0f, 0f, 45f);
                float size = i % 3 == 0 ? 3f : 2f;
                SetRect(embers[i].rectTransform, new Rect(0, 0, size, size));
            }
            GameObject ghost = new GameObject("Previous Portrait Reveal", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            ghost.transform.SetParent(portrait.transform.parent, false);
            previousPortrait = ghost.GetComponent<RawImage>();
            previousPortrait.raycastTarget = false;
            previousPortrait.enabled = false;
            SetRect(previousPortrait.rectTransform, new Rect(3, 3, 270, 270));
            graphicCount++;
            portraitGlow = Light("Portrait Calling Accent", portrait.transform.parent, false, new Rect(2, 2, 272, 272));
            portraitGlow.Spread = 8f;
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = DecorationImage("Portrait Corner " + i, portrait.transform.parent);
                bool right = (i / 2) % 2 == 1;
                bool bottom = i / 2 >= 2;
                bool vertical = i % 2 == 1;
                SetRect(corners[i].rectTransform, new Rect(right ? (vertical ? 267 : 254) : 7,
                    bottom ? (vertical ? 254 : 267) : 7, vertical ? 2 : 15, vertical ? 15 : 2));
            }
            for (int i = 0; i < selectionGlows.Length; i++)
            {
                selectionGlows[i] = Light("Selected Folio Glow " + i, transform, false, new Rect());
                selectionGlows[i].Spread = 5f;
                selectionGlows[i].gameObject.SetActive(false);
            }
            SynchronizePreference();
            RenderFrame();
        }

        public void RegisterButton(Button button)
        {
            if (button == null || buttons.Contains(button)) return;
            buttons.Add(button);
            SetButtonDuration(button);
        }

        public void SetVisible(bool value)
        {
            visible = value;
            SynchronizePreference();
            if (value)
            {
                ambientTime = 0f;
                transitionTime = reduced ? PartySetupFolioAnimationRules.AccentSeconds : 0f;
            }
            else FinishReveal();
            RenderFrame();
        }

        public void ChoosePortrait(Texture2D atlas, Rect uv, int member, string race, string characterClass)
        {
            bool changed = !hasIdentity || identityMember != member || identityRace != race || identityClass != characterClass;
            if (!changed && portrait.texture == atlas && portrait.uvRect == uv) return;
            SynchronizePreference();
            hasPrevious = visible && !reduced && portrait.enabled && portrait.texture != null && atlas != null
                && (portrait.texture != atlas || portrait.uvRect != uv);
            previousPortrait.texture = hasPrevious ? portrait.texture : null;
            previousPortrait.uvRect = portrait.uvRect;
            portrait.texture = atlas;
            portrait.uvRect = uv;
            portrait.color = Color.white;
            portrait.enabled = atlas != null;
            hasIdentity = true;
            identityMember = member;
            identityRace = race;
            identityClass = characterClass;
            themeColor = PartySetupFolioAnimationRules.PortraitAccent(race, characterClass);
            transitionTime = visible && !reduced ? 0f : PartySetupFolioAnimationRules.AccentSeconds;
            RenderFrame();
        }

        public void SetSelection(Button roster, Button race, Button characterClass, Button tab)
        {
            SetSelectionGlow(0, roster);
            SetSelectionGlow(1, race);
            SetSelectionGlow(2, characterClass);
            SetSelectionGlow(3, tab);
            RenderFrame();
        }

        public void Advance(float deltaSeconds)
        {
            if (!visible || !gameObject.activeInHierarchy) return;
            SynchronizePreference();
            if (!reduced && !float.IsNaN(deltaSeconds) && !float.IsInfinity(deltaSeconds))
            {
                float delta = Mathf.Max(0f, deltaSeconds);
                transitionTime = Mathf.Min(PartySetupFolioAnimationRules.AccentSeconds, transitionTime + delta);
                ambientTime += delta;
            }
            if (transitionTime >= PartySetupFolioAnimationRules.CrossfadeSeconds && hasPrevious)
            {
                hasPrevious = false;
                previousPortrait.texture = null;
            }
            RenderFrame();
        }

        private void Update()
        {
            Advance(Time.unscaledDeltaTime);
        }

        private void OnDisable()
        {
            visible = false;
            FinishReveal();
            RenderFrame();
        }

        private void FinishReveal()
        {
            transitionTime = PartySetupFolioAnimationRules.AccentSeconds;
            hasPrevious = false;
            if (previousPortrait != null)
            {
                previousPortrait.texture = null;
                previousPortrait.enabled = false;
            }
        }

        private void SynchronizePreference()
        {
            bool current = reducedMotion?.Invoke() ?? false;
            if (current == reduced) return;
            reduced = current;
            if (reduced)
            {
                ambientTime = 0f;
                FinishReveal();
            }
            for (int i = 0; i < buttons.Count; i++) SetButtonDuration(buttons[i]);
        }

        private void SetButtonDuration(Button button)
        {
            ColorBlock colors = button.colors;
            colors.fadeDuration = reduced ? 0f : 0.07f;
            button.colors = colors;
        }

        private void SetSelectionGlow(int index, Button button)
        {
            Transform target = button == null ? null : button.transform;
            if (selectionParents[index] == target) return;
            selectionParents[index] = target;
            FolioLightGraphic glow = selectionGlows[index];
            glow.gameObject.SetActive(target != null);
            if (target == null) return;
            glow.transform.SetParent(target, false);
            RectTransform rect = glow.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private void RenderFrame()
        {
            snapshot = PartySetupFolioAnimationRules.Evaluate(transitionTime, ambientTime, visible, reduced, hasPrevious, graphicCount);
            if (previousPortrait == null) return;
            previousPortrait.enabled = snapshot.PreviousPortraitAlpha > 0f && previousPortrait.texture != null;
            previousPortrait.color = WithAlpha(Color.white, snapshot.PreviousPortraitAlpha);
            portraitGlow.color = WithAlpha(themeColor, snapshot.AccentAlpha);
            for (int i = 0; i < corners.Length; i++) corners[i].color = WithAlpha(themeColor, snapshot.AccentAlpha);
            hearthGlow.color = new Color(1f, 0.41f, 0.10f, snapshot.FirelightAlpha);
            windowGlow.color = new Color(0.52f, 0.60f, 0.82f, snapshot.FirelightAlpha * 0.55f);
            for (int i = 0; i < embers.Length; i++)
            {
                Vector2 position = PartySetupFolioAnimationRules.EmberPosition(i, ambientTime);
                embers[i].rectTransform.anchoredPosition = new Vector2(position.x, -position.y);
                embers[i].color = new Color(1f, 0.65f, 0.23f, visible && !reduced ? PartySetupFolioAnimationRules.EmberAlpha(i, ambientTime) : 0f);
            }
            for (int i = 0; i < selectionGlows.Length; i++)
                selectionGlows[i].color = WithAlpha(i == 0 ? themeColor : new Color32(204, 153, 67, 255), snapshot.SelectionGlowAlpha);
        }

        private FolioLightGraphic Light(string name, Transform parent, bool radial, Rect area)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
            go.transform.SetParent(parent, false);
            FolioLightGraphic graphic = go.AddComponent<FolioLightGraphic>();
            graphic.Radial = radial;
            graphic.raycastTarget = false;
            SetRect(graphic.rectTransform, area);
            graphicCount++;
            return graphic;
        }

        private Image DecorationImage(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            Image graphic = go.GetComponent<Image>();
            graphic.raycastTarget = false;
            graphicCount++;
            return graphic;
        }

        private static Color WithAlpha(Color value, float alpha)
        {
            value.a = alpha;
            return value;
        }

        private static void SetRect(RectTransform rect, Rect area)
        {
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(area.x, -area.y);
            rect.sizeDelta = new Vector2(area.width, area.height);
        }
    }

    /// <summary>Small procedural gradients; no generated textures, materials, or runtime sprite ownership.</summary>
    public sealed class FolioLightGraphic : MaskableGraphic
    {
        public bool Radial;
        public float Spread = 5f;

        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            Rect rect = GetPixelAdjustedRect();
            Color edge = color;
            edge.a = 0f;
            if (Radial)
            {
                const int segments = 24;
                mesh.AddVert(rect.center, color, Vector2.zero);
                for (int i = 0; i <= segments; i++)
                {
                    float angle = i * Mathf.PI * 2f / segments;
                    mesh.AddVert(rect.center + new Vector2(Mathf.Cos(angle) * rect.width * 0.5f, Mathf.Sin(angle) * rect.height * 0.5f), edge, Vector2.zero);
                    if (i > 0) mesh.AddTriangle(0, i, i + 1);
                }
                return;
            }
            Vector2 bottomLeft = new Vector2(rect.xMin, rect.yMin);
            Vector2 bottomRight = new Vector2(rect.xMax, rect.yMin);
            Vector2 topRight = new Vector2(rect.xMax, rect.yMax);
            Vector2 topLeft = new Vector2(rect.xMin, rect.yMax);
            Strip(mesh, bottomLeft, bottomRight, bottomRight + new Vector2(Spread, -Spread), bottomLeft + new Vector2(-Spread, -Spread), color, edge);
            Strip(mesh, bottomRight, topRight, topRight + new Vector2(Spread, Spread), bottomRight + new Vector2(Spread, -Spread), color, edge);
            Strip(mesh, topRight, topLeft, topLeft + new Vector2(-Spread, Spread), topRight + new Vector2(Spread, Spread), color, edge);
            Strip(mesh, topLeft, bottomLeft, bottomLeft + new Vector2(-Spread, -Spread), topLeft + new Vector2(-Spread, Spread), color, edge);
        }

        private static void Strip(VertexHelper mesh, Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color inner, Color outer)
        {
            int start = mesh.currentVertCount;
            mesh.AddVert(a, inner, Vector2.zero);
            mesh.AddVert(b, inner, Vector2.zero);
            mesh.AddVert(c, outer, Vector2.zero);
            mesh.AddVert(d, outer, Vector2.zero);
            mesh.AddTriangle(start, start + 1, start + 2);
            mesh.AddTriangle(start + 2, start + 3, start);
        }
    }
}
