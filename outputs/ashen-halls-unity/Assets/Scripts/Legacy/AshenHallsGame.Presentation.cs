using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private GUIStyle titleStyle;

        private GUIStyle h2Style;

        private GUIStyle labelStyle;

        private GUIStyle mutedStyle;

        private GUIStyle buttonStyle;

        private GUIStyle smallButtonStyle;

        private GUIStyle logStyle;

        private GUIStyle fieldStyle;

        private readonly Dictionary<string, GUIStyle> centerStyleCache = new Dictionary<string, GUIStyle>();

        private readonly Dictionary<string, GUIStyle> centerLeftStyleCache = new Dictionary<string, GUIStyle>();

        private readonly Dictionary<string, GUIStyle> centerRightStyleCache = new Dictionary<string, GUIStyle>();

        private void DrawStartupSplash(string status, bool overlay)
        {
            EnsurePixel();
            float elapsed = splashClockStarted ? Mathf.Max(0f, Time.realtimeSinceStartup - splashStartedAt) : 0f;
            float alpha = overlay ? Mathf.Clamp01(1f - Mathf.InverseLerp(5.1f, 6.0f, elapsed)) : 1f;
            DrawRect(new Rect(0, 0, Screen.width, Screen.height), Hex("050708", overlay ? 0.94f * alpha : 1f));

            if (splashArt != null)
            {
                Color old = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, alpha);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), splashArt, ScaleMode.ScaleAndCrop);
                GUI.color = old;

                DrawRect(new Rect(0, 0, Screen.width, Screen.height), Hex("050708", 0.18f * alpha));
                Rect title = new Rect(Screen.width * 0.18f, Screen.height * 0.08f, Screen.width * 0.64f, Screen.height * 0.25f);
                DrawRect(new Rect(title.x + title.width * 0.10f, title.y + title.height * 0.02f, title.width * 0.80f, title.height * 0.78f), Hex("030405", 0.46f * alpha));
                DrawBorder(new Rect(title.x + title.width * 0.12f, title.y + title.height * 0.08f, title.width * 0.76f, title.height * 0.58f), Hex("d7a84e", 0.54f * alpha), 2);
                GUI.Label(new Rect(title.x, title.y + title.height * 0.10f, title.width, title.height * 0.30f), GameTitle.ToUpperInvariant(), CenterStyle(Mathf.RoundToInt(Mathf.Clamp(Screen.width / 34f, 32f, 54f)), Hex("f3ead7", alpha)));
                GUI.Label(new Rect(title.x, title.y + title.height * 0.42f, title.width, title.height * 0.18f), GameSubtitle, CenterStyle(Mathf.RoundToInt(Mathf.Clamp(Screen.width / 82f, 14f, 22f)), Hex("d7a84e", alpha)));
                Rect betaBadge = new Rect(title.x + title.width * 0.32f, title.y + title.height * 0.62f, title.width * 0.36f, 30f);
                DrawRect(betaBadge, Hex("2a1112", 0.88f * alpha));
                DrawBorder(betaBadge, Hex("c65c3b", 0.92f * alpha), 1);
                GUI.Label(betaBadge, BuildStage.ToUpperInvariant(), CenterStyle(15, Hex("f3ead7", alpha)));

                Rect splashBar = new Rect(Screen.width * 0.33f, Screen.height * 0.79f, Screen.width * 0.34f, Mathf.Max(10f, Screen.height * 0.014f));
                DrawRect(Pad(splashBar, -4), Hex("030405", 0.76f * alpha));
                DrawBorder(Pad(splashBar, -4), Hex("d7a84e", 0.42f * alpha), 1);
                float splashPulse = 0.25f + Mathf.PingPong(elapsed * 0.65f, 0.75f);
                DrawRect(new Rect(splashBar.x, splashBar.y, Mathf.Max(10f, splashBar.width * splashPulse), splashBar.height), Hex("58b7a5", 0.92f * alpha));
                GUI.Label(new Rect(0, splashBar.yMax + 10f, Screen.width, 24f), string.IsNullOrEmpty(status) ? "Loading..." : status, CenterStyle(13, Hex("f3ead7", alpha)));
                GUI.Label(new Rect(0, splashBar.yMax + 34f, Screen.width, 22f), "click, Enter, or any key to continue", CenterStyle(12, Hex("d7a84e", 0.88f * alpha)));
                if (!string.IsNullOrEmpty(launchError))
                {
                    GUI.Label(new Rect(Screen.width * 0.18f, splashBar.yMax + 58f, Screen.width * 0.64f, 24f), "Startup note: " + launchError, CenterStyle(11, Hex("b94b56", alpha)));
                }
                return;
            }

            float scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
            Rect art = new Rect(Screen.width * 0.5f - 310f * scale, Screen.height * 0.5f - 185f * scale, 620f * scale, 370f * scale);
            DrawRect(art, Hex("101619", 0.94f * alpha));
            DrawBorder(art, Hex("d7a84e", 0.70f * alpha), Mathf.Max(1, Mathf.RoundToInt(2 * scale)));

            Rect sky = new Rect(art.x + 16 * scale, art.y + 16 * scale, art.width - 32 * scale, art.height * 0.44f);
            DrawRect(sky, Hex("171c20", 0.95f * alpha));
            for (int i = 0; i < 18; i++)
            {
                float sx = sky.x + ((i * 53) % 560) * scale;
                float sy = sky.y + (18 + (i * 29) % 96) * scale;
                DrawRect(new Rect(sx, sy, Mathf.Max(2, 3 * scale), Mathf.Max(2, 3 * scale)), Hex(i % 3 == 0 ? "d7a84e" : "8d6dcc", 0.65f * alpha));
            }

            DrawRect(new Rect(art.x + 60 * scale, art.y + 138 * scale, 500 * scale, 24 * scale), Hex("3a3329", alpha));
            for (int i = 0; i < 7; i++)
            {
                float x = art.x + (96 + i * 66) * scale;
                float h = (86 + (i % 3) * 24) * scale;
                DrawRect(new Rect(x, art.y + 72 * scale + (120 * scale - h), 34 * scale, h), Hex(i % 2 == 0 ? "46504d" : "3c4544", alpha));
                DrawRect(new Rect(x - 8 * scale, art.y + 70 * scale + (120 * scale - h), 50 * scale, 10 * scale), Hex("6b756e", alpha));
                DrawRect(new Rect(x + 9 * scale, art.y + 92 * scale + (120 * scale - h), 8 * scale, 28 * scale), Hex("050708", 0.82f * alpha));
            }

            for (int i = 0; i < 8; i++)
            {
                float x = art.x + (78 + i * 59) * scale;
                float y = art.y + (222 + (i % 2) * 7) * scale;
                DrawRect(new Rect(x, y, 20 * scale, 34 * scale), RoleColor(roleOrder[i % roleOrder.Length]).WithAlpha(alpha));
                DrawRect(new Rect(x + 4 * scale, y - 10 * scale, 12 * scale, 12 * scale), Hex("d9a67b", alpha));
                DrawRect(new Rect(x - 4 * scale, y + 30 * scale, 28 * scale, 6 * scale), Hex("020303", 0.72f * alpha));
            }

            GUI.Label(new Rect(art.x, art.y + 188 * scale, art.width, 50 * scale), GameTitle.ToUpperInvariant(), CenterStyle(Mathf.RoundToInt(40 * scale), Hex("f3ead7", alpha)));
            GUI.Label(new Rect(art.x, art.y + 236 * scale, art.width, 28 * scale), GameSubtitle + " / modern-pixel tactical party RPG", CenterStyle(Mathf.RoundToInt(15 * scale), Hex("b7aa90", alpha)));
            Rect fallbackBadge = new Rect(art.x + art.width * 0.33f, art.y + 266 * scale, art.width * 0.34f, 24 * scale);
            DrawRect(fallbackBadge, Hex("2a1112", 0.88f * alpha));
            DrawBorder(fallbackBadge, Hex("c65c3b", 0.88f * alpha), Mathf.Max(1, Mathf.RoundToInt(scale)));
            GUI.Label(fallbackBadge, BuildStage.ToUpperInvariant(), CenterStyle(Mathf.RoundToInt(12 * scale), Hex("f3ead7", alpha)));

            Rect bar = new Rect(art.x + 126 * scale, art.y + 296 * scale, art.width - 252 * scale, 12 * scale);
            DrawRect(bar, Hex("050708", 0.90f * alpha));
            float pulse = 0.25f + Mathf.PingPong(elapsed * 0.65f, 0.75f);
            DrawRect(new Rect(bar.x + 2 * scale, bar.y + 2 * scale, Mathf.Max(8 * scale, (bar.width - 4 * scale) * pulse), bar.height - 4 * scale), Hex("58b7a5", 0.88f * alpha));
            GUI.Label(new Rect(art.x, art.y + 314 * scale, art.width, 22 * scale), string.IsNullOrEmpty(status) ? "Loading..." : status, CenterStyle(Mathf.RoundToInt(12 * scale), Hex("d7a84e", alpha)));
            GUI.Label(new Rect(art.x, art.y + 336 * scale, art.width, 20 * scale), "click, Enter, or any key to continue", CenterStyle(Mathf.RoundToInt(11 * scale), Hex("b7aa90", 0.85f * alpha)));
            if (!string.IsNullOrEmpty(launchError))
            {
                GUI.Label(new Rect(art.x + 36 * scale, art.y + 354 * scale, art.width - 72 * scale, 24 * scale), "Startup note: " + launchError, CenterStyle(Mathf.RoundToInt(11 * scale), Hex("b94b56", alpha)));
            }
        }

        private void DrawLaunchError(Exception ex)
        {
            EnsurePixel();
            DrawRect(new Rect(0, 0, Screen.width, Screen.height), Hex("050708"));
            Rect rect = new Rect(Screen.width / 2f - 320, Screen.height / 2f - 120, 640, 240);
            DrawRect(rect, Hex("171c20"));
            DrawBorder(rect, blood, 2);
            GUI.Label(new Rect(rect.x + 24, rect.y + 24, rect.width - 48, 36), GameTitle + " startup recovered", CenterStyle(22, ink));
            GUI.Label(new Rect(rect.x + 32, rect.y + 76, rect.width - 64, 80), "The game caught a startup error instead of leaving a blank screen.\nTry closing and relaunching. The latest error was:", CenterStyle(14, muted));
            GUI.Label(new Rect(rect.x + 32, rect.y + 154, rect.width - 64, 42), ex.Message, CenterStyle(13, gold));
        }

        private void EnsureStyles()
        {
            EnsurePixel();

            if (titleStyle != null) return;

            centerStyleCache.Clear();
            centerLeftStyleCache.Clear();
            centerRightStyleCache.Clear();

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 31,
                fontStyle = FontStyle.Bold,
                normal = { textColor = ink }
            };
            h2Style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Hex("f7dfad") }
            };
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                normal = { textColor = ink },
                wordWrap = true
            };
            mutedStyle = new GUIStyle(labelStyle)
            {
                fontSize = 12,
                normal = { textColor = muted }
            };
            logStyle = new GUIStyle(labelStyle)
            {
                fontSize = 13,
                wordWrap = true,
                padding = new RectOffset(8, 8, 7, 7)
            };
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = ink },
                hover = { textColor = ink },
                active = { textColor = ink }
            };
            smallButtonStyle = new GUIStyle(buttonStyle) { fontSize = 12 };
            fieldStyle = new GUIStyle(GUI.skin.textField)
            {
                fontSize = 14,
                normal = { textColor = ink },
                focused = { textColor = ink }
            };
        }

        private void EnsurePixel()
        {
            if (pixel != null) return;
            pixel = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            pixel.SetPixel(0, 0, Color.white);
            pixel.Apply();
        }

        private void DrawGameChrome(string mode)
        {
            if (ExploreMapFocusActive())
            {
                DrawExploreFocusChrome(mode);
                return;
            }

            Rect top = new Rect(12, 10, Screen.width - 24, 58);
            DrawRect(top, Hex("10161b", 0.88f));
            DrawBorder(top, line.WithAlpha(0.72f), 1);
            DrawGameLogo(new Rect(top.x + 12, top.y + 9, 40, 40));
            float buttonW = Screen.width < 1180 ? 48f : 54f;
            float buttonGap = 6f;
            float actionsW = buttonW * 3f + buttonGap * 2f;
            float actionsX = top.xMax - actionsW - 10f;
            float resourceW = Screen.width < 1240 ? 62f : 76f;
            float resourceGap = Screen.width < 1240 ? 5f : 8f;
            float resourcesW = resourceW * 3f + resourceGap * 2f;
            float resourcesX = actionsX - resourcesW - 10f;
            float titleX = top.x + 62f;
            float titleW = Mathf.Max(180f, Mathf.Min(310f, resourcesX - titleX - 14f));
            Rect titlePlate = new Rect(titleX - 6f, top.y + 7f, titleW + 14f, 42f);
            TryDrawGameTitlePlate(titlePlate, 0.48f);
            GUI.Label(new Rect(titleX, top.y + 6, titleW, 27), GameTitle, CenterLeftStyle(23, ink));
            string modeLine = state.Mode == GameMode.Explore ? $"{StoryChapterTitle()} / Depth {state.Depth}" : $"{GameSubtitle} / Depth {state.Depth} / {mode}";
            GUI.Label(new Rect(titleX + 2, top.y + 34, titleW, 18), modeLine, CenterLeftStyle(10, muted));

            float preferenceX = titleX + titleW + 12f;
            float preferenceW = resourcesX - preferenceX - 12f;
            if (preferenceW >= 170f && state.Mode != GameMode.Explore && state.Mode != GameMode.Combat)
            {
                DrawPreferenceControls(preferenceX, top.y + 18, preferenceW, false);
            }

            DrawResource(new Rect(resourcesX, top.y + 8, resourceW, 42), "Gold", state.Gold.ToString());
            DrawResource(new Rect(resourcesX + resourceW + resourceGap, top.y + 8, resourceW, 42), "Supplies", state.Supplies.ToString());
            DrawResource(new Rect(resourcesX + (resourceW + resourceGap) * 2f, top.y + 8, resourceW, 42), "Elixirs", state.Elixirs.ToString());
            if (GUI.Button(new Rect(actionsX, top.y + 12, buttonW, 34), "Save", smallButtonStyle)) SaveGame();
            if (GUI.Button(new Rect(actionsX + buttonW + buttonGap, top.y + 12, buttonW, 34), "Load", smallButtonStyle)) LoadGame();
            if (GUI.Button(new Rect(actionsX + (buttonW + buttonGap) * 2f, top.y + 12, buttonW, 34), "New", smallButtonStyle)) NewMuster();
        }

        private bool ExploreMapFocusActive()
        {
            return state != null && state.Mode == GameMode.Explore;
        }

        private void DrawExploreFocusChrome(string mode)
        {
            Rect top = new Rect(8f, 8f, Screen.width - 16f, 40f);
            DrawRect(top, Hex("080d10", 0.78f));
            DrawBorder(top, line.WithAlpha(0.56f), 1);
            DrawGameLogo(new Rect(top.x + 8f, top.y + 6f, 28f, 28f));

            float buttonW = Screen.width < 1180 ? 42f : 48f;
            float buttonGap = 5f;
            float actionsW = buttonW * 3f + buttonGap * 2f;
            float actionsX = top.xMax - actionsW - 8f;
            float resourceW = Screen.width < 1240 ? 54f : 64f;
            float resourceGap = 6f;
            float resourcesW = resourceW * 3f + resourceGap * 2f;
            float resourcesX = actionsX - resourcesW - 8f;
            float titleX = top.x + 44f;
            float titleW = Mathf.Max(150f, resourcesX - titleX - 10f);

            GUI.Label(new Rect(titleX, top.y + 3f, titleW * 0.36f, 17f), FitText(GameTitle, titleW * 0.36f, CenterLeftStyle(15, ink)), CenterLeftStyle(15, ink));
            string route = $"{StoryChapterTitle()} / D{state.Depth}";
            GUI.Label(new Rect(titleX + titleW * 0.37f, top.y + 4f, titleW * 0.34f, 15f), FitText(route, titleW * 0.34f, CenterLeftStyle(10, muted)), CenterLeftStyle(10, muted));
            GUI.Label(new Rect(titleX + titleW * 0.72f, top.y + 4f, titleW * 0.28f, 15f), FitText(ExploreHudHint(), titleW * 0.28f, CenterRightStyle(9, teal)), CenterRightStyle(9, teal));

            DrawResourceCompact(new Rect(resourcesX, top.y + 6f, resourceW, 28f), "Gold", state.Gold.ToString());
            DrawResourceCompact(new Rect(resourcesX + resourceW + resourceGap, top.y + 6f, resourceW, 28f), "Supplies", state.Supplies.ToString());
            DrawResourceCompact(new Rect(resourcesX + (resourceW + resourceGap) * 2f, top.y + 6f, resourceW, 28f), "Elixirs", state.Elixirs.ToString());
            if (GUI.Button(new Rect(actionsX, top.y + 7f, buttonW, 26f), "Save", smallButtonStyle)) SaveGame();
            if (GUI.Button(new Rect(actionsX + buttonW + buttonGap, top.y + 7f, buttonW, 26f), "Load", smallButtonStyle)) LoadGame();
            if (GUI.Button(new Rect(actionsX + (buttonW + buttonGap) * 2f, top.y + 7f, buttonW, 26f), "New", smallButtonStyle)) NewMuster();
        }

        private void DrawPreferenceControls(float x, float y, float maxWidth = 300f, bool showPulse = true)
        {
            if (state == null) return;
            bool soundOn = !state.SfxMuted;
            bool compact = maxWidth < 340f;
            float sfxW = compact ? 68f : 82f;
            float volumeW = compact ? 54f : 64f;
            float testW = showPulse && maxWidth >= 330f ? 52f : 0f;
            bool nextSoundOn = GUI.Toggle(new Rect(x, y, sfxW, 24), soundOn, " Audio");
            if (nextSoundOn != soundOn)
            {
                ToggleSfxMute();
            }

            if (GUI.Button(new Rect(x + sfxW + 6f, y - 3, volumeW, 28), $"{state.SfxVolumePercent}%", smallButtonStyle))
            {
                CycleSfxVolume();
            }

            if (testW > 0f && GUI.Button(new Rect(x + sfxW + volumeW + 12f, y - 3, testW, 28), "Test", smallButtonStyle))
            {
                TestSfx();
            }

            float motionX = x + sfxW + volumeW + (testW > 0f ? testW + 20f : 14f);
            float motionW = maxWidth - (motionX - x);
            if (motionW >= 76f)
            {
                string motionLabel = motionW < 116f ? " Motion" : " Reduced Motion";
                state.ReducedMotion = GUI.Toggle(new Rect(motionX, y, motionW, 24), state.ReducedMotion, motionLabel);
            }
            if (showPulse) DrawSfxPulse(new Rect(x, y + 29f, Mathf.Min(maxWidth, 220f), 18f));
        }

        private void DrawSfxPulse(Rect rect)
        {
            if (rect.width < 120f) return;
            float age = Time.realtimeSinceStartup - lastSfxAt;
            bool live = !string.IsNullOrEmpty(lastSfxKey) && age >= 0f && age < 1.35f;
            Color color = live ? Color.Lerp(teal, gold, Mathf.Clamp01(1f - age / 1.35f)) : (state != null && state.SfxMuted ? blood : line);
            string label = state != null && state.SfxMuted ? "Audio muted" : live ? $"Audio: {lastSfxKey} {Mathf.RoundToInt(lastSfxVolume * 100f)}%" : "Audio ready";
            DrawRect(rect, Hex("080b0d", live ? 0.92f : 0.58f));
            DrawBorder(rect, color.WithAlpha(live ? 0.95f : 0.50f), 1);
            if (live)
            {
                DrawRect(new Rect(rect.x + 4, rect.y + 4, Mathf.Clamp(rect.width * (1f - age / 1.35f), 8f, rect.width - 8f), rect.height - 8), color.WithAlpha(0.28f));
            }
            GUI.Label(new Rect(rect.x + 7, rect.y + 1, rect.width - 14, rect.height), label, CenterLeftStyle(10, live ? cursorWhite : muted));
        }

        private void DrawResource(Rect rect, string label, string value)
        {
            DrawRect(rect, Hex("171c20"));
            DrawBorder(rect, line, 1);
            Color icon = label == "Gold" ? gold : label == "Supplies" ? moss : teal;
            Rect iconRect = new Rect(rect.x + 5, rect.y + 6, 15, 15);
            if (!TryDrawInventoryConsumableAtlasIcon(iconRect, ResourceConsumableIconIndex(label), Color.white.WithAlpha(0.94f)))
            {
                DrawRect(new Rect(rect.x + 8, rect.y + 10, 8, 8), icon);
            }
            GUI.Label(new Rect(rect.x + 23, rect.y + 4, rect.width - 27, 15), label == "Supplies" && rect.width < 78 ? "Sup" : label, CenterLeftStyle(9, muted));
            GUI.Label(new Rect(rect.x + 6, rect.y + 21, rect.width - 12, 18), value, CenterLeftStyle(13, ink));
        }

        private void DrawResourceCompact(Rect rect, string label, string value)
        {
            DrawRect(rect, Hex("11171b", 0.88f));
            DrawBorder(rect, line.WithAlpha(0.58f), 1);
            Color icon = label == "Gold" ? gold : label == "Supplies" ? moss : teal;
            Rect iconRect = new Rect(rect.x + 4f, rect.y + 7f, 13f, 13f);
            if (!TryDrawInventoryConsumableAtlasIcon(iconRect, ResourceConsumableIconIndex(label), Color.white.WithAlpha(0.90f)))
            {
                DrawRect(new Rect(rect.x + 7f, rect.y + 10f, 7f, 7f), icon);
            }
            string shortLabel = label == "Supplies" ? "Sup" : label == "Elixirs" ? "Elx" : "Gold";
            GUI.Label(new Rect(rect.x + 20f, rect.y + 1f, rect.width - 24f, 11f), shortLabel, CenterLeftStyle(8, muted));
            GUI.Label(new Rect(rect.x + 20f, rect.y + 13f, rect.width - 24f, 14f), value, CenterLeftStyle(11, ink));
        }

        private void DrawGameLogo(Rect rect)
        {
            DrawRect(rect, Hex("080b0d", 0.94f));
            DrawBorder(rect, teal, 2);
            DrawBorder(Pad(rect, 5f), gold.WithAlpha(0.74f), 1);
            if (gameIconArt != null)
            {
                Color old = GUI.color;
                GUI.color = Color.white.WithAlpha(0.96f);
                GUI.DrawTexture(Pad(rect, 2f), gameIconArt, ScaleMode.ScaleAndCrop, true);
                GUI.color = old;
                DrawBorder(Pad(rect, 1f), Hex("030405", 0.82f), 1);
                DrawBorder(rect, teal.WithAlpha(0.82f), 2);
                return;
            }
            Rect flame = new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.18f, rect.width * 0.32f, rect.height * 0.42f);
            DrawRect(new Rect(flame.x + flame.width * 0.34f, flame.y, flame.width * 0.32f, flame.height * 0.94f), ember);
            DrawRect(new Rect(flame.x + flame.width * 0.16f, flame.y + flame.height * 0.25f, flame.width * 0.26f, flame.height * 0.62f), gold);
            DrawRect(new Rect(flame.x + flame.width * 0.58f, flame.y + flame.height * 0.34f, flame.width * 0.28f, flame.height * 0.54f), blood);
            Rect hall = new Rect(rect.x + rect.width * 0.22f, rect.y + rect.height * 0.58f, rect.width * 0.56f, rect.height * 0.22f);
            DrawRect(hall, Hex("3c4544"));
            DrawRect(new Rect(hall.x + hall.width * 0.15f, hall.y - hall.height * 0.62f, hall.width * 0.16f, hall.height * 0.62f), Hex("a9b0a2"));
            DrawRect(new Rect(hall.x + hall.width * 0.68f, hall.y - hall.height * 0.62f, hall.width * 0.16f, hall.height * 0.62f), Hex("a9b0a2"));
            GUI.Label(new Rect(rect.x, rect.yMax - rect.height * 0.30f, rect.width, rect.height * 0.26f), "AH", CenterStyle(10, ink));
        }

        private bool TryDrawGameTitlePlate(Rect rect, float alpha)
        {
            if (titleCardArt == null || rect.width <= 4f || rect.height <= 4f) return false;
            Color old = GUI.color;
            GUI.color = Color.white.WithAlpha(Mathf.Clamp01(alpha));
            GUI.DrawTexture(rect, titleCardArt, ScaleMode.ScaleAndCrop, true);
            GUI.color = old;
            DrawRect(rect, Hex("030405", 0.18f));
            DrawBorder(rect, gold.WithAlpha(0.34f), 1);
            DrawRect(new Rect(rect.x + rect.width * 0.08f, rect.yMax - Mathf.Max(2f, rect.height * 0.05f), rect.width * 0.84f, Mathf.Max(2f, rect.height * 0.05f)), teal.WithAlpha(0.18f));
            return true;
        }

        private void DrawPanel(Rect rect)
        {
            DrawRect(rect, panel);
            DrawRect(new Rect(rect.x, rect.y, rect.width, Mathf.Min(18f, rect.height * 0.18f)), Hex("2a3233", 0.34f));
            DrawRect(new Rect(rect.x + 1, rect.y + 1, rect.width - 2, 1), Hex("f3ead7", 0.10f));
            DrawBorder(rect, line, 1);
        }

        private void DrawRpgPanel(Rect rect, Color accent)
        {
            DrawPanel(rect);
            DrawCombatUiPanelBackdrop(rect);
            DrawRect(new Rect(rect.x + 2, rect.y + 2, rect.width - 4, 2), accent.WithAlpha(0.20f));
            DrawBorder(Pad(rect, 4), accent.WithAlpha(0.26f), 1);
            float c = Mathf.Min(20f, rect.width * 0.08f);
            DrawRect(new Rect(rect.x + 5, rect.y + 5, c, 2), accent.WithAlpha(0.62f));
            DrawRect(new Rect(rect.x + 5, rect.y + 5, 2, c), accent.WithAlpha(0.62f));
            DrawRect(new Rect(rect.xMax - 5 - c, rect.y + 5, c, 2), accent.WithAlpha(0.62f));
            DrawRect(new Rect(rect.xMax - 7, rect.y + 5, 2, c), accent.WithAlpha(0.62f));
            DrawRect(new Rect(rect.x + 5, rect.yMax - 7, c, 2), accent.WithAlpha(0.44f));
            DrawRect(new Rect(rect.x + 5, rect.yMax - 5 - c, 2, c), accent.WithAlpha(0.44f));
            DrawRect(new Rect(rect.xMax - 5 - c, rect.yMax - 7, c, 2), accent.WithAlpha(0.44f));
            DrawRect(new Rect(rect.xMax - 7, rect.yMax - 5 - c, 2, c), accent.WithAlpha(0.44f));
            DrawCombatUiCornerTrim(rect, accent);
        }

        private void DrawCombatUiPanelBackdrop(Rect rect)
        {
            if (!IsCombatUiPanelAtlas()) return;
            int index = CombatUiPanelBackdropIndex(rect);
            if (index < 0) return;
            TryDrawCombatUiPanelAtlasIcon(new Rect(rect.x + 2f, rect.y + 2f, rect.width - 4f, rect.height - 4f), index, Color.white.WithAlpha(0.10f));
        }

        private int CombatUiPanelBackdropIndex(Rect rect)
        {
            if (rect.height <= 0f) return -1;
            float aspect = rect.width / rect.height;
            if (aspect > 3.1f) return 1;
            if (aspect > 1.35f) return 19;
            if (aspect < 0.78f) return 4;
            return 0;
        }

        private void DrawPanelHeader(Rect panelRect, string title, string icon, Color accent, string rightText)
        {
            Rect iconRect = new Rect(panelRect.x + 12, panelRect.y + 10, 22, 22);
            DrawTinyUiIcon(iconRect, icon, accent);
            float titleW = string.IsNullOrEmpty(rightText) ? panelRect.width - 58f : panelRect.width - 184f;
            GUI.Label(new Rect(panelRect.x + 42, panelRect.y + 9, titleW, 24), FitText(title, titleW, h2Style), h2Style);
            if (!string.IsNullOrEmpty(rightText))
            {
                GUI.Label(new Rect(panelRect.xMax - 126, panelRect.y + 13, 112, 18), rightText, CenterRightStyle(11, muted));
            }
            DrawRect(new Rect(panelRect.x + 12, panelRect.y + 38, panelRect.width - 24, 1), accent.WithAlpha(0.34f));
        }

        private void DrawTinyUiIcon(Rect rect, string icon, Color accent)
        {
            DrawRect(rect, Hex("050708", 0.74f));
            int hudIndex = CombatHudIconIndex(icon);
            if (hudIndex >= 0 && TryDrawCombatHudUiAtlasIcon(Pad(rect, 1f), hudIndex, Color.white.WithAlpha(0.94f)))
            {
                DrawBorder(rect, accent.WithAlpha(0.82f), 1);
                return;
            }
            int uiIndex = CombatUiIconIndex(icon);
            if (uiIndex >= 0 && TryDrawCombatUiAtlasIcon(Pad(rect, 1f), uiIndex, Color.white.WithAlpha(0.92f)))
            {
                DrawBorder(rect, accent.WithAlpha(0.82f), 1);
                return;
            }
            DrawBorder(rect, accent.WithAlpha(0.82f), 1);
            Rect inner = Pad(rect, rect.width * 0.22f);
            if (icon == "party")
            {
                DrawRect(new Rect(inner.x, inner.y + inner.height * 0.50f, inner.width, inner.height * 0.22f), accent);
                DrawRect(new Rect(inner.x + inner.width * 0.12f, inner.y, inner.width * 0.24f, inner.height * 0.44f), teal);
                DrawRect(new Rect(inner.x + inner.width * 0.60f, inner.y + inner.height * 0.08f, inner.width * 0.24f, inner.height * 0.40f), gold);
            }
            else if (icon == "enemy")
            {
                DrawRect(new Rect(inner.x + inner.width * 0.15f, inner.y + inner.height * 0.14f, inner.width * 0.70f, inner.height * 0.54f), blood);
                DrawRect(new Rect(inner.x + inner.width * 0.22f, inner.y, inner.width * 0.18f, inner.height * 0.28f), ink);
                DrawRect(new Rect(inner.x + inner.width * 0.60f, inner.y, inner.width * 0.18f, inner.height * 0.28f), ink);
                DrawRect(new Rect(inner.x + inner.width * 0.35f, inner.y + inner.height * 0.38f, inner.width * 0.10f, inner.height * 0.10f), retroBlack);
                DrawRect(new Rect(inner.x + inner.width * 0.56f, inner.y + inner.height * 0.38f, inner.width * 0.10f, inner.height * 0.10f), retroBlack);
            }
            else if (icon == "scroll")
            {
                DrawRect(new Rect(inner.x + inner.width * 0.10f, inner.y + inner.height * 0.10f, inner.width * 0.80f, inner.height * 0.72f), Hex("d9d3c4"));
                DrawRect(new Rect(inner.x + inner.width * 0.20f, inner.y + inner.height * 0.28f, inner.width * 0.58f, inner.height * 0.08f), accent);
                DrawRect(new Rect(inner.x + inner.width * 0.20f, inner.y + inner.height * 0.50f, inner.width * 0.42f, inner.height * 0.08f), accent.WithAlpha(0.75f));
            }
            else if (icon == "magic")
            {
                DrawPixelCross(inner, accent);
                DrawRect(new Rect(inner.center.x - inner.width * 0.10f, inner.y, inner.width * 0.20f, inner.height), accent.WithAlpha(0.70f));
            }
            else
            {
                DrawSigil(inner, icon, accent);
            }
        }

        private int CombatHudIconIndex(string icon)
        {
            switch ((icon ?? "").ToLowerInvariant())
            {
                case "queue": return 17;
                case "party": return 0;
                case "active": return 16;
                case "enemy": return 1;
                case "timeline":
                case "scroll": return 2;
                case "magic": return 11;
                case "music": return 19;
                default: return -1;
            }
        }

        private bool IsCombatUiAtlas()
        {
            return combatUiAtlas != null && Mathf.Abs(combatUiAtlas.width - combatUiAtlas.height) < 8 && combatUiAtlas.width >= 512;
        }

        private Rect CombatUiAtlasCell(int index)
        {
            return AtlasCell(combatUiAtlas, index, 4, 4);
        }

        private bool TryDrawCombatUiAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsCombatUiAtlas()) return false;
            return DrawTextureRegionTint(combatUiAtlas, rect, CombatUiAtlasCell(index), tint);
        }

        private bool IsCombatUiPanelAtlas()
        {
            return combatUiPanelAtlas != null && combatUiPanelAtlas.width >= 1000 && combatUiPanelAtlas.height >= 768;
        }

        private Rect CombatUiPanelAtlasCell(int index)
        {
            return AtlasCell(combatUiPanelAtlas, index, 5, 4);
        }

        private bool TryDrawCombatUiPanelAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsCombatUiPanelAtlas() || index < 0) return false;
            return DrawTextureRegionTint(combatUiPanelAtlas, rect, CombatUiPanelAtlasCell(index), tint);
        }

        private bool IsSpellbookUiAtlas()
        {
            return spellbookUiAtlas != null && Mathf.Abs(spellbookUiAtlas.width - spellbookUiAtlas.height) < 8 && spellbookUiAtlas.width >= 768;
        }

        private Rect SpellbookUiAtlasCell(int index)
        {
            return AtlasCell(spellbookUiAtlas, index, 5, 5);
        }

        private bool TryDrawSpellbookUiAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsSpellbookUiAtlas()) return false;
            return DrawTextureRegionTint(spellbookUiAtlas, rect, SpellbookUiAtlasCell(index), tint);
        }

        private bool IsSignatureSpellIconAtlas()
        {
            return signatureSpellIconAtlas != null
                && signatureSpellIconAtlas.width == CombatIconCatalog.SignatureSpellAtlasColumns * 256
                && signatureSpellIconAtlas.height == CombatIconCatalog.SignatureSpellAtlasRows * 256;
        }

        private Rect SignatureSpellIconAtlasCell(int index)
        {
            return AtlasCell(
                signatureSpellIconAtlas,
                index,
                CombatIconCatalog.SignatureSpellAtlasColumns,
                CombatIconCatalog.SignatureSpellAtlasRows);
        }

        private bool TryDrawSignatureSpellIconAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsSignatureSpellIconAtlas() || index < 0) return false;
            return DrawTextureRegionTint(signatureSpellIconAtlas, rect, SignatureSpellIconAtlasCell(index), tint);
        }

        private bool IsLightningSpellIconAtlas()
        {
            return lightningSpellIconAtlas != null
                && lightningSpellIconAtlas.width == 1024
                && lightningSpellIconAtlas.height == 512;
        }

        private Rect LightningSpellIconAtlasCell(int index)
        {
            return AtlasCell(
                lightningSpellIconAtlas,
                index,
                LightningSpellIconCatalog.AtlasColumns,
                LightningSpellIconCatalog.AtlasRows);
        }

        private bool TryDrawLightningSpellIconAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsLightningSpellIconAtlas() || index < 0) return false;
            return DrawTextureRegionTint(lightningSpellIconAtlas, rect, LightningSpellIconAtlasCell(index), tint);
        }

        private bool IsCombatSpellbookUiAtlas()
        {
            return combatSpellbookUiAtlas != null && combatSpellbookUiAtlas.width >= 768 && combatSpellbookUiAtlas.height >= 600;
        }

        private Rect CombatSpellbookUiAtlasCell(int index)
        {
            return AtlasCell(combatSpellbookUiAtlas, index, 5, 4);
        }

        private bool TryDrawCombatSpellbookUiAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsCombatSpellbookUiAtlas() || index < 0) return false;
            return DrawTextureRegionTint(combatSpellbookUiAtlas, rect, CombatSpellbookUiAtlasCell(index), tint);
        }

        private bool IsPactSpellbookAtlas()
        {
            return pactSpellbookAtlas != null && Mathf.Abs(pactSpellbookAtlas.width - pactSpellbookAtlas.height) < 8 && pactSpellbookAtlas.width >= 768;
        }

        private Rect PactSpellbookAtlasCell(int index)
        {
            return AtlasCell(pactSpellbookAtlas, index, 5, 4);
        }

        private bool TryDrawPactSpellbookAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsPactSpellbookAtlas() || index < 0) return false;
            return DrawTextureRegionTint(pactSpellbookAtlas, rect, PactSpellbookAtlasCell(index), tint);
        }

        private bool IsCombatHudUiAtlas()
        {
            return combatHudUiAtlas != null && combatHudUiAtlas.width >= 768 && combatHudUiAtlas.height >= 600;
        }

        private Rect CombatHudUiAtlasCell(int index)
        {
            return AtlasCell(combatHudUiAtlas, index, 5, 4);
        }

        private bool TryDrawCombatHudUiAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsCombatHudUiAtlas() || index < 0) return false;
            return DrawTextureRegionTint(combatHudUiAtlas, rect, CombatHudUiAtlasCell(index), tint);
        }

        private bool IsCombatSpellFloatAtlas()
        {
            return combatSpellFloatAtlas != null && combatSpellFloatAtlas.width >= 768 && combatSpellFloatAtlas.height >= 512;
        }

        private Rect CombatSpellFloatAtlasCell(int index)
        {
            bool square = Mathf.Abs(combatSpellFloatAtlas.width - combatSpellFloatAtlas.height) < 8;
            return square ? AtlasCell(combatSpellFloatAtlas, index, 4, 4) : AtlasCell(combatSpellFloatAtlas, index, 5, 4);
        }

        private bool TryDrawCombatSpellFloatAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsCombatSpellFloatAtlas() || index < 0) return false;
            return DrawTextureRegionTint(combatSpellFloatAtlas, rect, CombatSpellFloatAtlasCell(index), tint);
        }

        private bool IsEmberSpellAtlas()
        {
            return emberSpellAtlas != null && Mathf.Abs(emberSpellAtlas.width - emberSpellAtlas.height) < 8 && emberSpellAtlas.width >= 768;
        }

        private Rect EmberSpellAtlasCell(int index)
        {
            return AtlasCell(emberSpellAtlas, index, 4, 4);
        }

        private bool TryDrawEmberSpellAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsEmberSpellAtlas()) return false;
            return DrawTextureRegionTint(emberSpellAtlas, rect, EmberSpellAtlasCell(index), tint);
        }

        private bool IsEpicSpellEffectsAtlas()
        {
            return epicSpellEffectsAtlas != null && epicSpellEffectsAtlas.width >= 768 && epicSpellEffectsAtlas.height >= 600;
        }

        private Rect EpicSpellEffectsAtlasCell(int index)
        {
            bool square = Mathf.Abs(epicSpellEffectsAtlas.width - epicSpellEffectsAtlas.height) < 8;
            return square ? AtlasCell(epicSpellEffectsAtlas, index, 4, 4) : AtlasCell(epicSpellEffectsAtlas, index, 5, 4);
        }

        private bool TryDrawEpicSpellEffectsAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsEpicSpellEffectsAtlas() || index < 0) return false;
            return DrawTextureRegionTint(epicSpellEffectsAtlas, rect, EpicSpellEffectsAtlasCell(index), tint);
        }

        private bool IsSpellAnimationAtlas()
        {
            return spellAnimationAtlas != null && Mathf.Abs(spellAnimationAtlas.width - spellAnimationAtlas.height) < 8 && spellAnimationAtlas.width >= 768;
        }

        private Rect SpellAnimationAtlasCell(int index)
        {
            return AtlasCell(spellAnimationAtlas, index, 4, 4);
        }

        private bool TryDrawSpellAnimationAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsSpellAnimationAtlas() || index < 0) return false;
            return DrawTextureRegionTint(spellAnimationAtlas, rect, SpellAnimationAtlasCell(index), tint);
        }

        private bool IsBossEnemyAtlas()
        {
            return bossEnemyAtlas != null && bossEnemyAtlas.width >= 768 && bossEnemyAtlas.height >= 600;
        }

        private Rect BossEnemyAtlasCell(int index)
        {
            return AtlasCell(bossEnemyAtlas, index, 5, 4);
        }

        private bool TryDrawBossEnemyAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsBossEnemyAtlas() || index < 0) return false;
            return DrawTextureRegionTint(bossEnemyAtlas, rect, BossEnemyAtlasCell(index), tint);
        }

        private bool IsKoboldRouteAtlas()
        {
            return koboldRouteAtlas != null && Mathf.Abs(koboldRouteAtlas.width - koboldRouteAtlas.height) < 8 && koboldRouteAtlas.width >= 768;
        }

        private Rect KoboldRouteAtlasCell(int index)
        {
            return AtlasCell(koboldRouteAtlas, index, 4, 4);
        }

        private bool TryDrawKoboldRouteAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsKoboldRouteAtlas() || index < 0) return false;
            return DrawTextureRegionTint(koboldRouteAtlas, rect, KoboldRouteAtlasCell(index), tint);
        }

        private bool IsKoboldBossAtlas()
        {
            return koboldBossAtlas != null && Mathf.Abs(koboldBossAtlas.width - koboldBossAtlas.height) < 8 && koboldBossAtlas.width >= 768;
        }

        private Rect KoboldBossAtlasCell(int index)
        {
            return AtlasCell(koboldBossAtlas, index, 4, 4);
        }

        private bool TryDrawKoboldBossAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsKoboldBossAtlas() || index < 0) return false;
            return DrawTextureRegionTint(koboldBossAtlas, rect, KoboldBossAtlasCell(index), tint);
        }

        private bool IsKoboldCavePropAtlas()
        {
            return koboldCavePropAtlas != null && Mathf.Abs(koboldCavePropAtlas.width - koboldCavePropAtlas.height) < 8 && koboldCavePropAtlas.width >= 768;
        }

        private Rect KoboldCavePropAtlasCell(int index)
        {
            return AtlasCell(koboldCavePropAtlas, index, 4, 4);
        }

        private bool TryDrawKoboldCavePropAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsKoboldCavePropAtlas() || index < 0) return false;
            return DrawTextureRegionTint(koboldCavePropAtlas, rect, KoboldCavePropAtlasCell(index), tint);
        }

        private bool IsQuestWorldAtlas()
        {
            return questWorldAtlas != null && questWorldAtlas.width >= 768 && questWorldAtlas.height >= 600;
        }

        private Rect QuestWorldAtlasCell(int index)
        {
            return AtlasCell(questWorldAtlas, index, 5, 4);
        }

        private bool TryDrawQuestWorldAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsQuestWorldAtlas() || index < 0) return false;
            return DrawTextureRegionTint(questWorldAtlas, rect, QuestWorldAtlasCell(index), tint);
        }

        private bool IsWorldMapPropAtlas()
        {
            return worldMapPropAtlas != null && worldMapPropAtlas.width >= 768 && worldMapPropAtlas.height >= 600;
        }

        private Rect WorldMapPropAtlasCell(int index)
        {
            return AtlasCell(worldMapPropAtlas, index, 5, 4);
        }

        private bool TryDrawWorldMapPropAtlasIcon(Rect rect, int index, Color tint)
        {
            return TryDrawWorldMapPropAtlasIcon(rect, index, tint, DefaultWorldMapArtSpec());
        }

        private bool TryDrawWorldMapPropAtlasIcon(Rect rect, int index, Color tint, WorldMapArtSpec spec)
        {
            if (!IsWorldMapPropAtlas() || index < 0) return false;
            return TryDrawTrimmedExplorationAtlasCell(worldMapPropAtlas, rect, index, 5, 4, tint, "world map prop", 0.08f, 0.92f, spec);
        }

        private bool IsWorldMapBiomePropAtlas()
        {
            return worldMapBiomePropAtlas != null
                && worldMapBiomePropAtlas.width >= 768
                && worldMapBiomePropAtlas.height >= 600
                && AtlasHasSquareCells(worldMapBiomePropAtlas, 5, 4, 3f);
        }

        private Rect WorldMapBiomePropAtlasCell(int index)
        {
            return AtlasCell(worldMapBiomePropAtlas, index, 5, 4);
        }

        private bool TryDrawWorldMapBiomePropAtlasIcon(Rect rect, int index, Color tint)
        {
            return TryDrawWorldMapBiomePropAtlasIcon(rect, index, tint, DefaultWorldMapArtSpec());
        }

        private bool TryDrawWorldMapBiomePropAtlasIcon(Rect rect, int index, Color tint, WorldMapArtSpec spec)
        {
            if (!IsWorldMapBiomePropAtlas() || index < 0) return false;
            return TryDrawTrimmedExplorationAtlasCell(worldMapBiomePropAtlas, rect, index, 5, 4, tint, "world map biome prop", 0.04f, 0.88f, spec);
        }

        private bool IsWorldMapExplorationTileAtlas()
        {
            return worldMapExplorationTileAtlas != null && worldMapExplorationTileAtlas.width >= 768 && worldMapExplorationTileAtlas.height >= 600;
        }

        private Rect WorldMapExplorationTileAtlasCell(int index)
        {
            return AtlasCell(worldMapExplorationTileAtlas, index, 5, WorldMapExplorationTileAtlasRows());
        }

        private bool TryDrawWorldMapExplorationTileAtlasIcon(Rect rect, int index, Color tint)
        {
            return TryDrawWorldMapExplorationTileAtlasIcon(rect, index, tint, false, false);
        }

        private bool IsWorldMapMaterialAtlas()
        {
            if (worldMapMaterialAtlas == null
                || worldMapMaterialAtlas.width < 512
                || worldMapMaterialAtlas.height < 512)
            {
                return false;
            }

            int columns = WorldMapMaterialAtlasColumns();
            return AtlasHasSquareCells(worldMapMaterialAtlas, columns, columns, 3f);
        }

        private int WorldMapMaterialAtlasColumns()
        {
            // v1.68 expands the approved 4x4 contract to an 8x8 bank. Retaining
            // 4x4 detection keeps an older packaged atlas usable as a safe fallback.
            return worldMapMaterialAtlas != null
                && worldMapMaterialAtlas.width >= 1600
                && worldMapMaterialAtlas.height >= 1600
                ? 8
                : 4;
        }

        private bool TryDrawWorldMapMaterialAtlasTile(
            Rect rect,
            ExplorationMaterial material,
            Color tint,
            int mapX,
            int mapY)
        {
            if (!TryResolveWorldMapMaterialAtlasSample(
                material,
                mapX,
                mapY,
                HasStaticExploreObjectFootprint(mapX, mapY),
                out Rect source,
                out bool flipX,
                out bool flipY))
            {
                return false;
            }

            return DrawTextureRegionTintVariant(worldMapMaterialAtlas, rect, source, tint, flipX, flipY);
        }

        private bool TryResolveWorldMapMaterialAtlasSample(
            ExplorationMaterial material,
            int mapX,
            int mapY,
            bool quietCell,
            out Rect source,
            out bool flipX,
            out bool flipY)
        {
            source = Rect.zero;
            flipX = false;
            flipY = false;
            if (!IsWorldMapMaterialAtlas()) return false;
            int semanticIndex = ExplorationArtRules.MaterialAtlasIndex(material);
            if (semanticIndex < 0) return false;
            int variation = ExploreNoise(mapX, mapY, 257 + semanticIndex * 13);
            int columns = WorldMapMaterialAtlasColumns();
            int index = columns >= 8
                ? ExplorationArtRules.MaterialAtlasVariantIndex(
                    material,
                    ExplorationArtRules.MaterialAtlasVariant(material, quietCell ? 0 : variation))
                : semanticIndex;
            source = InsetAtlasSource(
                AtlasCell(worldMapMaterialAtlas, index, columns, columns),
                0.75f);
            flipX = (variation & 1) != 0;
            flipY = (variation & 2) != 0;
            return true;
        }

        private bool TryResolveWorldMapMaterialAtlasEdgeSample(
            ExplorationMaterial material,
            int mapX,
            int mapY,
            out Rect source,
            out bool flipX,
            out bool flipY)
        {
            return TryResolveWorldMapMaterialAtlasSample(
                material,
                mapX,
                mapY,
                HasStaticExploreObjectFootprint(mapX, mapY),
                out source,
                out flipX,
                out flipY);
        }

        private bool TryDrawWorldMapMaterialAtlasEdgeBand(
            Rect destination,
            Rect resolvedSource,
            bool sourceFlipX,
            bool sourceFlipY,
            int neighborDx,
            int neighborDy,
            int band,
            float bandFraction,
            float alpha)
        {
            Rect source = resolvedSource;
            bandFraction = Mathf.Clamp(bandFraction, 0.01f, 0.20f);
            band = Mathf.Max(0, band);
            if (neighborDx != 0)
            {
                float logicalStart = ExplorationArtRules.MaterialBlendSourceStart(
                    neighborDx < 0,
                    band,
                    bandFraction,
                    sourceFlipX);
                source = new Rect(
                    source.x + source.width * logicalStart,
                    source.y,
                    source.width * bandFraction,
                    source.height);
            }
            else if (neighborDy != 0)
            {
                float logicalStart = ExplorationArtRules.MaterialBlendSourceStart(
                    neighborDy < 0,
                    band,
                    bandFraction,
                    sourceFlipY);
                source = new Rect(
                    source.x,
                    source.y + source.height * logicalStart,
                    source.width,
                    source.height * bandFraction);
            }
            else
            {
                return false;
            }

            bool drawFlipX = neighborDx != 0
                ? ExplorationArtRules.MaterialBlendDrawFlip(sourceFlipX)
                : sourceFlipX;
            bool drawFlipY = neighborDy != 0
                ? ExplorationArtRules.MaterialBlendDrawFlip(sourceFlipY)
                : sourceFlipY;
            return DrawTextureRegionTintVariant(
                worldMapMaterialAtlas,
                destination,
                source,
                Color.white.WithAlpha(Mathf.Clamp01(alpha)),
                drawFlipX,
                drawFlipY);
        }

        private bool TryDrawWorldMapExplorationTileAtlasIcon(Rect rect, int index, Color tint, bool flipX, bool flipY)
        {
            return TryDrawWorldMapExplorationTileAtlasIcon(rect, index, tint, flipX, flipY, 1, 0, 0);
        }

        private bool TryDrawWorldMapExplorationTileAtlasIcon(Rect rect, int index, Color tint, bool flipX, bool flipY, int macroSize, int mapX, int mapY)
        {
            if (!IsWorldMapExplorationTileAtlas() || index < 0) return false;
            if (!WorldMapExplorationTileAtlasHasCell(index)) return false;
            Rect source = InsetAtlasSource(WorldMapExplorationTileAtlasCell(index), 0.75f);
            source = AtlasMacroSource(source, macroSize, mapX, mapY);
            return DrawTextureRegionTintVariant(worldMapExplorationTileAtlas, rect, source, tint, flipX, flipY);
        }

        private int WorldMapExplorationTileAtlasRows()
        {
            return worldMapExplorationTileAtlas == null ? 4 : Mathf.Max(4, AtlasRows(worldMapExplorationTileAtlas, 5));
        }

        private bool WorldMapExplorationTileAtlasHasCell(int index)
        {
            return index >= 0 && index < 5 * WorldMapExplorationTileAtlasRows();
        }

        private bool IsWorldMapLandmarkAtlas()
        {
            return worldMapLandmarkAtlas != null && worldMapLandmarkAtlas.width >= 768 && worldMapLandmarkAtlas.height >= 600;
        }

        private Rect WorldMapLandmarkAtlasCell(int index)
        {
            return AtlasCell(worldMapLandmarkAtlas, index, 5, 4);
        }

        private bool TryDrawWorldMapLandmarkAtlasIcon(Rect rect, int index, Color tint)
        {
            return TryDrawWorldMapLandmarkAtlasIcon(rect, index, tint, DefaultWorldMapArtSpec());
        }

        private bool TryDrawWorldMapLandmarkAtlasIcon(Rect rect, int index, Color tint, WorldMapArtSpec spec)
        {
            if (!IsWorldMapLandmarkAtlas() || index < 0) return false;
            return TryDrawTrimmedExplorationAtlasCell(worldMapLandmarkAtlas, rect, index, 5, 4, tint, "world map landmark", 0.08f, 0.92f, spec);
        }

        private bool IsWorldMapRegionLandmarkAtlas()
        {
            return worldMapRegionLandmarkAtlas != null
                && worldMapRegionLandmarkAtlas.width >= 1000
                && worldMapRegionLandmarkAtlas.height >= 800;
        }

        private bool TryDrawWorldMapRegionLandmarkAtlasIcon(Rect rect, int index, Color tint, WorldMapArtSpec spec)
        {
            if (!IsWorldMapRegionLandmarkAtlas() || index < 0) return false;
            return TryDrawTrimmedExplorationAtlasCell(
                worldMapRegionLandmarkAtlas,
                rect,
                index,
                WorldMapRegionLandmarkCatalog.Columns,
                WorldMapRegionLandmarkCatalog.Rows,
                tint,
                "world map regional landmark",
                0.08f,
                0.92f,
                spec);
        }

        private bool IsWorldMapOverlayAtlas()
        {
            return worldMapOverlayAtlas != null && worldMapOverlayAtlas.width >= 768 && worldMapOverlayAtlas.height >= 600;
        }

        private Rect WorldMapOverlayAtlasCell(int index)
        {
            return AtlasCell(worldMapOverlayAtlas, index, 5, 4);
        }

        private bool TryDrawWorldMapOverlayAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsWorldMapOverlayAtlas() || index < 0) return false;
            return DrawTextureRegionTint(worldMapOverlayAtlas, rect, WorldMapOverlayAtlasCell(index), tint);
        }

        private bool IsWorldMapProgressionOverlayAtlas()
        {
            return worldMapProgressionOverlayAtlas != null && worldMapProgressionOverlayAtlas.width >= 768 && worldMapProgressionOverlayAtlas.height >= 600;
        }

        private Rect WorldMapProgressionOverlayAtlasCell(int index)
        {
            return AtlasCell(worldMapProgressionOverlayAtlas, index, 5, 4);
        }

        private bool TryDrawWorldMapProgressionOverlayAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsWorldMapProgressionOverlayAtlas() || index < 0) return false;
            return DrawTextureRegionTint(worldMapProgressionOverlayAtlas, rect, WorldMapProgressionOverlayAtlasCell(index), tint);
        }

        private bool IsWorldMapUiAtlas()
        {
            return worldMapUiAtlas != null && worldMapUiAtlas.width >= 768 && worldMapUiAtlas.height >= 600;
        }

        private Rect WorldMapUiAtlasCell(int index)
        {
            return AtlasCell(worldMapUiAtlas, index, 5, 4);
        }

        private bool TryDrawWorldMapUiAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsWorldMapUiAtlas() || index < 0) return false;
            return TryDrawValidatedExplorationAtlasCell(worldMapUiAtlas, rect, index, 5, 4, tint, "world map UI icon", 0.04f, 0.86f);
        }

        private bool IsWorldMapTokenSpriteAtlas()
        {
            return worldMapTokenSpriteAtlas != null && worldMapTokenSpriteAtlas.width >= 768 && worldMapTokenSpriteAtlas.height >= 600;
        }

        private Rect WorldMapTokenSpriteAtlasCell(int index)
        {
            return AtlasCell(worldMapTokenSpriteAtlas, index, 5, 4);
        }

        private bool TryDrawWorldMapTokenSpriteAtlasIcon(Rect rect, int index, Color tint)
        {
            return TryDrawWorldMapTokenSpriteAtlasIcon(rect, index, tint, DefaultWorldMapArtSpec());
        }

        private bool TryDrawWorldMapTokenSpriteAtlasIcon(Rect rect, int index, Color tint, WorldMapArtSpec spec)
        {
            if (!IsWorldMapTokenSpriteAtlas() || index < 0) return false;
            return TryDrawTrimmedExplorationAtlasCell(worldMapTokenSpriteAtlas, rect, index, 5, 4, tint, "world map token", 0.10f, 0.92f, spec);
        }

        private bool IsMidgaardTownAtlas()
        {
            return midgaardTownAtlas != null && midgaardTownAtlas.width >= 768 && midgaardTownAtlas.height >= 600;
        }

        private Rect MidgaardTownAtlasCell(int index)
        {
            return AtlasCell(midgaardTownAtlas, index, 5, 4);
        }

        private bool TryDrawMidgaardTownAtlasIcon(Rect rect, int index, Color tint)
        {
            return TryDrawMidgaardTownAtlasIcon(rect, index, tint, DefaultWorldMapArtSpec());
        }

        private bool TryDrawMidgaardTownAtlasIcon(Rect rect, int index, Color tint, WorldMapArtSpec spec)
        {
            if (!IsMidgaardTownAtlas() || index < 0) return false;
            return TryDrawTrimmedExplorationAtlasCell(midgaardTownAtlas, rect, index, 5, 4, tint, "Midgaard town object", 0.08f, 0.92f, spec);
        }

        private bool IsMidgaardTileAtlas()
        {
            return midgaardTileAtlas != null && midgaardTileAtlas.width >= 768 && midgaardTileAtlas.height >= 600;
        }

        private Rect MidgaardTileAtlasCell(int index)
        {
            return AtlasCell(midgaardTileAtlas, index, 5, 4);
        }

        private bool TryDrawMidgaardTileAtlasIcon(Rect rect, int index, Color tint)
        {
            return TryDrawMidgaardTileAtlasIcon(rect, index, tint, false, false);
        }

        private bool TryDrawMidgaardTileAtlasIcon(Rect rect, int index, Color tint, bool flipX, bool flipY)
        {
            return TryDrawMidgaardTileAtlasIcon(rect, index, tint, flipX, flipY, 1, 0, 0);
        }

        private bool TryDrawMidgaardTileAtlasIcon(Rect rect, int index, Color tint, bool flipX, bool flipY, int macroSize, int mapX, int mapY)
        {
            if (!IsMidgaardTileAtlas() || index < 0) return false;
            Rect source = InsetAtlasSource(MidgaardTileAtlasCell(index), 0.75f);
            source = AtlasMacroSource(source, macroSize, mapX, mapY);
            return DrawTextureRegionTintVariant(midgaardTileAtlas, rect, source, tint, flipX, flipY);
        }

        private bool IsMidgaardInteriorTileAtlas()
        {
            return midgaardInteriorTileAtlas != null
                && midgaardInteriorTileAtlas.width >= 1000
                && midgaardInteriorTileAtlas.height >= 800
                && AtlasHasSquareCells(midgaardInteriorTileAtlas, 5, 4, 3f);
        }

        private bool TryDrawMidgaardInteriorTileAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsMidgaardInteriorTileAtlas() || index < 0 || index >= 20) return false;
            Rect source = InsetAtlasSource(AtlasCell(midgaardInteriorTileAtlas, index, 5, 4), 0.75f);
            return DrawTextureRegionTintVariant(midgaardInteriorTileAtlas, rect, source, tint, false, false);
        }

        private bool IsMidgaardWallAtlas()
        {
            return midgaardWallAtlas != null
                && midgaardWallAtlas.width >= 768
                && midgaardWallAtlas.height >= 600
                && AtlasHasSquareCells(midgaardWallAtlas, 5, 4, 3f);
        }

        private Rect MidgaardWallAtlasCell(int index)
        {
            return AtlasCell(midgaardWallAtlas, index, 5, 4);
        }

        private bool TryDrawMidgaardWallAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsMidgaardWallAtlas() || index < 0) return false;
            bool horizontal = index == 0 || index == 1 || index == 8;
            bool vertical = index == 2 || index == 3 || index == 9;
            Vector2 pivot = horizontal
                ? new Vector2(0.5f, 1f)
                : vertical ? new Vector2(0.5f, 0.5f) : new Vector2(0.5f, 1f);
            WorldMapArtSpec spec = new WorldMapArtSpec(horizontal || vertical ? 1.04f : 1.02f, pivot, Vector2.zero, true);
            return TryDrawTrimmedExplorationAtlasCell(midgaardWallAtlas, rect, index, 5, 4, tint, "Midgaard wall", 0.04f, 0.92f, spec);
        }

        private bool IsMidgaardGateAtlas()
        {
            return midgaardGateAtlas != null
                && midgaardGateAtlas.width >= 768
                && midgaardGateAtlas.height >= 600
                && AtlasHasSquareCells(midgaardGateAtlas, 5, 4, 3f);
        }

        private Rect MidgaardGateAtlasCell(int index)
        {
            return AtlasCell(midgaardGateAtlas, index, 5, 4);
        }

        private bool TryDrawMidgaardGateAtlasIcon(Rect rect, int index, Color tint)
        {
            return TryDrawMidgaardGateAtlasIcon(rect, index, tint, new WorldMapArtSpec(1.02f, new Vector2(0.5f, 1f), Vector2.zero, true));
        }

        private bool TryDrawMidgaardGateAtlasIcon(Rect rect, int index, Color tint, WorldMapArtSpec spec)
        {
            if (!IsMidgaardGateAtlas() || index < 0) return false;
            return TryDrawTrimmedExplorationAtlasCell(midgaardGateAtlas, rect, index, 5, 4, tint, "Midgaard gate", 0.08f, 0.92f, spec);
        }

        private bool IsMidgaardCityPropAtlas()
        {
            return midgaardCityPropAtlas != null && midgaardCityPropAtlas.width >= 768 && midgaardCityPropAtlas.height >= 600;
        }

        private Rect MidgaardCityPropAtlasCell(int index)
        {
            return AtlasCell(midgaardCityPropAtlas, index, 5, 4);
        }

        private bool TryDrawMidgaardCityPropAtlasIcon(Rect rect, int index, Color tint)
        {
            return TryDrawMidgaardCityPropAtlasIcon(rect, index, tint, DefaultWorldMapArtSpec());
        }

        private bool TryDrawMidgaardCityPropAtlasIcon(Rect rect, int index, Color tint, WorldMapArtSpec spec)
        {
            if (!IsMidgaardCityPropAtlas() || index < 0) return false;
            return TryDrawTrimmedExplorationAtlasCell(midgaardCityPropAtlas, rect, index, 5, 4, tint, "Midgaard prop", 0.06f, 0.92f, spec);
        }

        private bool IsMidgaardStreetLifeAtlas()
        {
            return midgaardStreetLifeAtlas != null
                && midgaardStreetLifeAtlas.width >= 768
                && midgaardStreetLifeAtlas.height >= 600
                && AtlasHasSquareCells(midgaardStreetLifeAtlas, 5, 4, 3f);
        }

        private Rect MidgaardStreetLifeAtlasCell(int index)
        {
            return AtlasCell(midgaardStreetLifeAtlas, index, 5, 4);
        }

        private bool TryDrawMidgaardStreetLifeAtlasIcon(Rect rect, int index, Color tint)
        {
            WorldMapArtSpec spec = new WorldMapArtSpec(0.98f, new Vector2(0.5f, 1f), Vector2.zero, false);
            if (!IsMidgaardStreetLifeAtlas() || index < 0) return false;
            return TryDrawTrimmedExplorationAtlasCell(midgaardStreetLifeAtlas, rect, index, 5, 4, tint, "Midgaard street life", 0.10f, 0.92f, spec);
        }

        private bool IsMidgaardPavingDecalAtlas()
        {
            return midgaardPavingDecalAtlas != null
                && midgaardPavingDecalAtlas.width >= 768
                && midgaardPavingDecalAtlas.height >= 768
                && AtlasHasSquareCells(midgaardPavingDecalAtlas, 4, 4, 3f);
        }

        private Rect MidgaardPavingDecalAtlasCell(int index)
        {
            return AtlasCell(midgaardPavingDecalAtlas, index, 4, 4);
        }

        private bool TryDrawMidgaardPavingDecalAtlasIcon(Rect rect, int index, Color tint)
        {
            WorldMapArtSpec spec = new WorldMapArtSpec(0.96f, new Vector2(0.5f, 0.5f), Vector2.zero, false);
            if (!IsMidgaardPavingDecalAtlas() || index < 0) return false;
            return TryDrawTrimmedExplorationAtlasCell(midgaardPavingDecalAtlas, rect, index, 4, 4, tint, "Midgaard paving decal", 0.08f, 0.92f, spec);
        }

        private bool IsMidgaardNpcAtlas()
        {
            return midgaardNpcAtlas != null
                && midgaardNpcAtlas.width == NpcPortraitCatalog.Columns * 256
                && midgaardNpcAtlas.height == NpcPortraitCatalog.Rows * 256;
        }

        private Rect MidgaardNpcAtlasCell(int index)
        {
            return AtlasCell(midgaardNpcAtlas, index, NpcPortraitCatalog.Columns, NpcPortraitCatalog.Rows);
        }

        private bool TryDrawMidgaardNpcAtlasIcon(Rect rect, int index, Color tint)
        {
            return TryDrawMidgaardNpcAtlasIcon(rect, index, tint, DefaultWorldMapArtSpec());
        }

        private bool TryDrawMidgaardNpcAtlasIcon(Rect rect, int index, Color tint, WorldMapArtSpec spec)
        {
            if (!IsMidgaardNpcAtlas() || index < 0) return false;
            return TryDrawTrimmedExplorationAtlasCell(
                midgaardNpcAtlas,
                rect,
                index,
                NpcPortraitCatalog.Columns,
                NpcPortraitCatalog.Rows,
                tint,
                "Midgaard NPC",
                0.10f,
                0.92f,
                spec);
        }

        private bool IsMidgaardInteriorPropAtlas()
        {
            return midgaardInteriorPropAtlas != null
                && midgaardInteriorPropAtlas.width >= 768
                && midgaardInteriorPropAtlas.height >= 600
                && AtlasHasSquareCells(midgaardInteriorPropAtlas, 5, 4, 3f);
        }

        private bool TryDrawMidgaardInteriorPropAtlasIcon(Rect rect, int index, Color tint, WorldMapArtSpec spec)
        {
            if (!IsMidgaardInteriorPropAtlas() || index < 0) return false;
            return TryDrawTrimmedExplorationAtlasCell(
                midgaardInteriorPropAtlas,
                rect,
                index,
                5,
                4,
                tint,
                "Midgaard interior prop",
                0.06f,
                0.94f,
                spec);
        }

        private bool IsMidgaardSewerAtlas()
        {
            return midgaardSewerAtlas != null && midgaardSewerAtlas.width >= 768 && midgaardSewerAtlas.height >= 600;
        }

        private Rect MidgaardSewerAtlasCell(int index)
        {
            return AtlasCell(midgaardSewerAtlas, index, 5, 4);
        }

        private bool TryDrawMidgaardSewerAtlasIcon(Rect rect, int index, Color tint)
        {
            return TryDrawMidgaardSewerAtlasIcon(rect, index, tint, DefaultWorldMapArtSpec());
        }

        private bool TryDrawMidgaardSewerAtlasIcon(Rect rect, int index, Color tint, WorldMapArtSpec spec)
        {
            if (!IsMidgaardSewerAtlas() || index < 0) return false;
            return TryDrawTrimmedExplorationAtlasCell(midgaardSewerAtlas, rect, index, 5, 4, tint, "Midgaard sewer", 0.08f, 0.92f, spec);
        }

        private bool IsStoryCardAtlas()
        {
            return storyCardAtlas != null && storyCardAtlas.width >= 768 && storyCardAtlas.height >= 600;
        }

        private Rect StoryCardAtlasCell(int index)
        {
            return AtlasCell(storyCardAtlas, index, 5, 4);
        }

        private bool TryDrawStoryCardAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsStoryCardAtlas() || index < 0) return false;
            return DrawTextureRegionTint(storyCardAtlas, rect, StoryCardAtlasCell(index), tint);
        }

        private bool IsNpcPortraitAtlas()
        {
            if (npcPortraitAtlas == null || npcPortraitAtlas.width < 1000 || npcPortraitAtlas.height < 800) return false;
            float expectedAspect = NpcPortraitCatalog.Columns / (float)NpcPortraitCatalog.Rows;
            return Mathf.Abs(npcPortraitAtlas.width / (float)npcPortraitAtlas.height - expectedAspect) < 0.03f;
        }

        private Rect NpcPortraitAtlasCell(int index)
        {
            return AtlasCell(npcPortraitAtlas, index, NpcPortraitCatalog.Columns, NpcPortraitCatalog.Rows);
        }

        private bool TryDrawNpcPortraitAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsNpcPortraitAtlas() || index < 0) return false;
            return DrawTextureRegionTint(npcPortraitAtlas, rect, NpcPortraitAtlasCell(index), tint);
        }

        private bool IsRouteScaffoldAtlas()
        {
            return routeScaffoldAtlas != null && routeScaffoldAtlas.width >= 768 && routeScaffoldAtlas.height >= 600;
        }

        private Rect RouteScaffoldAtlasCell(int index)
        {
            return AtlasCell(routeScaffoldAtlas, index, 5, 4);
        }

        private bool TryDrawRouteScaffoldAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsRouteScaffoldAtlas() || index < 0) return false;
            return TryDrawTrimmedExplorationAtlasCell(routeScaffoldAtlas, rect, index, 5, 4, tint, "route scaffold", 0.05f, 0.92f, DefaultWorldMapArtSpec());
        }

        private bool IsDungeonScaffoldAtlas()
        {
            return dungeonScaffoldAtlas != null && Mathf.Abs(dungeonScaffoldAtlas.width - dungeonScaffoldAtlas.height) < 8 && dungeonScaffoldAtlas.width >= 768;
        }

        private Rect DungeonScaffoldAtlasCell(int index)
        {
            return AtlasCell(dungeonScaffoldAtlas, index, 4, 4);
        }

        private bool TryDrawDungeonScaffoldAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsDungeonScaffoldAtlas() || index < 0) return false;
            return TryDrawTrimmedExplorationAtlasCell(dungeonScaffoldAtlas, rect, index, 4, 4, tint, "dungeon scaffold", 0.05f, 0.92f, DefaultWorldMapArtSpec());
        }

        private bool IsFactionBannerAtlas()
        {
            return factionBannerAtlas != null && factionBannerAtlas.width >= 768 && factionBannerAtlas.height >= 600;
        }

        private Rect FactionBannerAtlasCell(int index)
        {
            return AtlasCell(factionBannerAtlas, index, 5, 4);
        }

        private bool TryDrawFactionBannerAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsFactionBannerAtlas() || index < 0) return false;
            return TryDrawTrimmedExplorationAtlasCell(factionBannerAtlas, rect, index, 5, 4, tint, "faction banner", 0.04f, 0.92f, DefaultWorldMapArtSpec());
        }

        private bool IsServiceScaffoldAtlas()
        {
            return serviceScaffoldAtlas != null && serviceScaffoldAtlas.width >= 768 && serviceScaffoldAtlas.height >= 600;
        }

        private Rect ServiceScaffoldAtlasCell(int index)
        {
            return AtlasCell(serviceScaffoldAtlas, index, 5, 4);
        }

        private bool TryDrawServiceScaffoldAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsServiceScaffoldAtlas() || index < 0) return false;
            return TryDrawTrimmedExplorationAtlasCell(serviceScaffoldAtlas, rect, index, 5, 4, tint, "service scaffold", 0.05f, 0.92f, DefaultWorldMapArtSpec());
        }

        private bool IsCharacterInventoryUiAtlas()
        {
            return characterInventoryUiAtlas != null && characterInventoryUiAtlas.width >= 768 && characterInventoryUiAtlas.height >= 600;
        }

        private Rect CharacterInventoryUiAtlasCell(int index)
        {
            return AtlasCell(characterInventoryUiAtlas, index, 5, 4);
        }

        private bool TryDrawCharacterInventoryUiAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsCharacterInventoryUiAtlas() || index < 0) return false;
            return DrawTextureRegionTint(characterInventoryUiAtlas, rect, CharacterInventoryUiAtlasCell(index), tint);
        }

        private bool IsEnemyWorldObjectAtlas()
        {
            return enemyWorldObjectAtlas != null && enemyWorldObjectAtlas.width >= 768 && enemyWorldObjectAtlas.height >= 600;
        }

        private Rect EnemyWorldObjectAtlasCell(int index)
        {
            return AtlasCell(enemyWorldObjectAtlas, index, 5, 4);
        }

        private bool TryDrawEnemyWorldObjectAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsEnemyWorldObjectAtlas() || index < 0) return false;
            return DrawTextureRegionTint(enemyWorldObjectAtlas, rect, EnemyWorldObjectAtlasCell(index), tint);
        }

        private bool IsRoamingThreatAtlas()
        {
            return roamingThreatAtlas != null
                && roamingThreatAtlas.width == 1400
                && roamingThreatAtlas.height == 1120;
        }

        private bool TryDrawRoamingThreatAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsRoamingThreatAtlas() || index < 0 || index >= 20) return false;
            return DrawTextureRegionTint(roamingThreatAtlas, rect, AtlasCell(roamingThreatAtlas, index, 5, 4), tint);
        }

        private bool IsTavernUiAtlas()
        {
            return tavernUiAtlas != null && tavernUiAtlas.width >= 768 && tavernUiAtlas.height >= 600;
        }

        private Rect TavernUiAtlasCell(int index)
        {
            return AtlasCell(tavernUiAtlas, index, 5, 4);
        }

        private bool TryDrawTavernUiAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsTavernUiAtlas() || index < 0) return false;
            return DrawTextureRegionTint(tavernUiAtlas, rect, TavernUiAtlasCell(index), tint);
        }

        private bool IsInventoryConsumableAtlas()
        {
            return inventoryConsumableAtlas != null && inventoryConsumableAtlas.width >= 768 && inventoryConsumableAtlas.height >= 600;
        }

        private Rect InventoryConsumableAtlasCell(int index)
        {
            return AtlasCell(inventoryConsumableAtlas, index, 5, 4);
        }

        private bool TryDrawInventoryConsumableAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsInventoryConsumableAtlas() || index < 0) return false;
            return DrawTextureRegionTint(inventoryConsumableAtlas, rect, InventoryConsumableAtlasCell(index), tint);
        }

        private bool IsCombatCommandIconAtlas()
        {
            return combatCommandIconAtlas != null && combatCommandIconAtlas.width >= 768 && combatCommandIconAtlas.height >= 600;
        }

        private Rect CombatCommandIconAtlasCell(int index)
        {
            return AtlasCell(combatCommandIconAtlas, index, 5, 4);
        }

        private bool TryDrawCombatCommandIconAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsCombatCommandIconAtlas() || index < 0) return false;
            return DrawTextureRegionTint(combatCommandIconAtlas, rect, CombatCommandIconAtlasCell(index), tint);
        }

        private bool IsAbilityIconAtlas()
        {
            return abilityIconAtlas != null
                && CombatIconCatalog.IsAbilityAtlasDimensions(
                    abilityIconAtlas.width,
                    abilityIconAtlas.height);
        }

        private Rect AbilityIconAtlasCell(int index)
        {
            if (index < 0
                || index >= CombatIconCatalog.AbilityAtlasColumns * CombatIconCatalog.ExpandedAbilityAtlasRows)
            {
                return Rect.zero;
            }
            return AtlasCell(
                abilityIconAtlas,
                index,
                CombatIconCatalog.AbilityAtlasColumns,
                CombatIconCatalog.ExpandedAbilityAtlasRows);
        }

        private bool TryDrawAbilityIconAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsAbilityIconAtlas()
                || index < 0
                || index >= CombatIconCatalog.AbilityAtlasColumns * CombatIconCatalog.ExpandedAbilityAtlasRows)
            {
                return false;
            }
            return DrawTextureRegionTint(abilityIconAtlas, rect, AbilityIconAtlasCell(index), tint);
        }

        private int AbilityIconAtlasRows()
        {
            return CombatIconCatalog.ExpandedAbilityAtlasRows;
        }

        private bool IsRangerAbilityEffectAtlas()
        {
            return rangerAbilityEffectAtlas != null && rangerAbilityEffectAtlas.width >= 768 && rangerAbilityEffectAtlas.height >= 768;
        }

        private Rect RangerAbilityEffectAtlasCell(int index)
        {
            return AtlasCell(rangerAbilityEffectAtlas, index, 4, 4);
        }

        private bool TryDrawRangerAbilityEffectAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsRangerAbilityEffectAtlas() || index < 0) return false;
            return DrawTextureRegionTint(rangerAbilityEffectAtlas, rect, RangerAbilityEffectAtlasCell(index), tint);
        }

        private bool IsEnemySpriteAtlas()
        {
            return enemySpriteAtlas != null && Mathf.Abs(enemySpriteAtlas.width - enemySpriteAtlas.height) < 8 && enemySpriteAtlas.width >= 768;
        }

        private Rect EnemySpriteAtlasCell(int index)
        {
            return AtlasCell(enemySpriteAtlas, index, 4, 4);
        }

        private bool TryDrawEnemySpriteAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsEnemySpriteAtlas() || index < 0) return false;
            return DrawTextureRegionTint(enemySpriteAtlas, rect, EnemySpriteAtlasCell(index), tint);
        }

        private bool IsCharacterCombatAtlas()
        {
            if (characterCombatAtlas == null
                || characterCombatAtlas.width < PlayerSpriteCatalog.Columns * 128
                || characterCombatAtlas.height < PlayerSpriteCatalog.Rows * 128)
            {
                return false;
            }

            float cellWidth = characterCombatAtlas.width / (float)PlayerSpriteCatalog.Columns;
            float cellHeight = characterCombatAtlas.height / (float)PlayerSpriteCatalog.Rows;
            return Mathf.Abs(cellWidth - cellHeight) < 2f;
        }

        private Rect CharacterCombatAtlasCell(int index)
        {
            return AtlasCell(characterCombatAtlas, index, PlayerSpriteCatalog.Columns, PlayerSpriteCatalog.Rows);
        }

        private bool TryDrawCharacterCombatAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsCharacterCombatAtlas() || index < 0) return false;
            return DrawTextureRegionTint(characterCombatAtlas, rect, CharacterCombatAtlasCell(index), tint);
        }

        private bool IsCreatureSpriteAtlas()
        {
            return creatureSpriteAtlas != null && Mathf.Abs(creatureSpriteAtlas.width - creatureSpriteAtlas.height) < 8 && creatureSpriteAtlas.width >= 768;
        }

        private Rect CreatureSpriteAtlasCell(int index)
        {
            return AtlasCell(creatureSpriteAtlas, index, 4, 4);
        }

        private bool TryDrawCreatureSpriteAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsCreatureSpriteAtlas() || index < 0) return false;
            return DrawTextureRegionTint(creatureSpriteAtlas, rect, CreatureSpriteAtlasCell(index), tint);
        }

        private bool IsDemonSummonAtlas()
        {
            return demonSummonAtlas != null && Mathf.Abs(demonSummonAtlas.width - demonSummonAtlas.height) < 8 && demonSummonAtlas.width >= 768;
        }

        private Rect DemonSummonAtlasCell(int index)
        {
            return AtlasCell(demonSummonAtlas, index, 4, 4);
        }

        private bool TryDrawDemonSummonAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsDemonSummonAtlas() || index < 0) return false;
            return DrawTextureRegionTint(demonSummonAtlas, rect, DemonSummonAtlasCell(index), tint);
        }

        private bool IsCombatTerrainAtlas()
        {
            return combatTerrainAtlas != null && Mathf.Abs(combatTerrainAtlas.width - combatTerrainAtlas.height) < 8 && combatTerrainAtlas.width >= 768;
        }

        private Rect CombatTerrainAtlasCell(int index)
        {
            return AtlasCell(combatTerrainAtlas, index, 4, 4);
        }

        private bool TryDrawCombatTerrainAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsCombatTerrainAtlas() || index < 0) return false;
            return DrawTextureRegionTint(combatTerrainAtlas, rect, CombatTerrainAtlasCell(index), tint);
        }

        private bool IsKoboldCombatTerrainAtlas()
        {
            return koboldCombatTerrainAtlas != null && Mathf.Abs(koboldCombatTerrainAtlas.width - koboldCombatTerrainAtlas.height) < 8 && koboldCombatTerrainAtlas.width >= 768;
        }

        private Rect KoboldCombatTerrainAtlasCell(int index)
        {
            return AtlasCell(koboldCombatTerrainAtlas, index, 4, 4);
        }

        private bool TryDrawKoboldCombatTerrainAtlasIcon(Rect rect, int index, Color tint)
        {
            if (!IsKoboldCombatTerrainAtlas() || index < 0) return false;
            return DrawTextureRegionTint(koboldCombatTerrainAtlas, rect, KoboldCombatTerrainAtlasCell(index), tint);
        }

        private int CombatUiIconIndex(string icon)
        {
            switch ((icon ?? "").ToLowerInvariant())
            {
                case "timeline":
                case "scroll": return 0;
                case "queue": return 1;
                case "party":
                case "active": return 2;
                case "target": return 3;
                case "move": return 4;
                case "ready": return 5;
                case "spent": return 6;
                case "magic": return 7;
                case "blade":
                case "skill":
                case "attack": return 8;
                case "range": return 8;
                case "blocked": return 9;
                case "guard": return 10;
                case "danger": return 11;
                case "hp": return 12;
                case "mp": return 13;
                case "status": return 14;
                case "trim": return 15;
                default: return -1;
            }
        }

        private void DrawCombatUiCornerTrim(Rect rect, Color accent)
        {
            if (!IsCombatUiAtlas()) return;
            float size = Mathf.Clamp(Mathf.Min(rect.width, rect.height) * 0.12f, 18f, 34f);
            Color tint = Color.Lerp(Color.white, accent, 0.28f).WithAlpha(0.36f);
            TryDrawCombatUiAtlasIcon(new Rect(rect.x + 5f, rect.y + 5f, size, size), 15, tint);
            TryDrawCombatUiAtlasIcon(new Rect(rect.xMax - size - 5f, rect.y + 5f, size, size), 15, tint);
        }

        private void DrawLabeledMeter(Rect rect, string label, int value, int max, Color color)
        {
            GUI.Label(new Rect(rect.x, rect.y - 2, 20, rect.height + 4), label, CenterLeftStyle(Mathf.RoundToInt(Mathf.Clamp(rect.height + 3, 8, 11)), muted));
            Rect bar = new Rect(rect.x + 20, rect.y, rect.width - 20, rect.height);
            DrawMeter(bar, value, max, color);
            DrawBorder(bar, Hex("030405", 0.70f), 1);
        }

        private string FitText(string text, float width, GUIStyle style)
        {
            if (string.IsNullOrEmpty(text) || width <= 12f || style == null) return "";
            if (style.CalcSize(new GUIContent(text)).x <= width) return text;
            const string ellipsis = "...";
            int lo = 0;
            int hi = text.Length;
            while (lo < hi)
            {
                int mid = (lo + hi + 1) / 2;
                string sample = text.Substring(0, mid) + ellipsis;
                if (style.CalcSize(new GUIContent(sample)).x <= width) lo = mid;
                else hi = mid - 1;
            }
            return text.Substring(0, Mathf.Clamp(lo, 0, text.Length)) + ellipsis;
        }

        private void DrawRect(Rect rect, Color color)
        {
            Color old = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, pixel);
            GUI.color = old;
        }

        private void DrawBorder(Rect rect, Color color, int thickness)
        {
            DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
            DrawRect(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color);
            DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
            DrawRect(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), color);
        }

        private void DrawMeter(Rect rect, int value, int max, Color color)
        {
            DrawRect(rect, Hex("111619"));
            DrawRect(new Rect(rect.x, rect.y, rect.width * Mathf.Clamp01(max <= 0 ? 0 : (float)value / max), rect.height), color);
        }

        private bool DrawAtlasRegion(Rect destination, Rect sourcePixels)
        {
            return DrawTextureRegion(betaCombatArt, destination, sourcePixels);
        }

        private bool DrawFormulaLabRegion(Rect destination, Rect sourcePixels)
        {
            return DrawTextureRegion(formulaLabArt, destination, sourcePixels);
        }

        private bool DrawClassIconRegion(Rect destination, Rect sourcePixels)
        {
            return DrawTextureRegion(classIconAtlas, destination, sourcePixels);
        }

        private bool DrawWorldObjectRegion(Rect destination, Rect sourcePixels)
        {
            return DrawTextureRegion(worldObjectAtlas, destination, sourcePixels);
        }

        private bool DrawCombatSpriteSheetRegion(Rect destination, Rect sourcePixels)
        {
            return DrawTextureRegion(combatSpriteSheet, destination, sourcePixels);
        }

        private bool DrawTextureRegion(Texture2D texture, Rect destination, Rect sourcePixels)
        {
            return DrawTextureRegionTint(texture, destination, sourcePixels, Color.white);
        }

        private Rect InsetAtlasSource(Rect source, float insetPixels)
        {
            float inset = Mathf.Clamp(insetPixels, 0f, Mathf.Min(source.width, source.height) * 0.20f);
            return new Rect(
                source.x + inset,
                source.y + inset,
                Mathf.Max(1f, source.width - inset * 2f),
                Mathf.Max(1f, source.height - inset * 2f));
        }

        private Rect AtlasMacroSource(Rect source, int macroSize, int mapX, int mapY)
        {
            macroSize = Mathf.Clamp(macroSize, 1, 4);
            if (macroSize <= 1) return source;
            int sx = ((mapX % macroSize) + macroSize) % macroSize;
            int sy = ((mapY % macroSize) + macroSize) % macroSize;
            float width = source.width / macroSize;
            float height = source.height / macroSize;
            return new Rect(source.x + sx * width, source.y + sy * height, width, height);
        }

        private bool DrawTextureRegionTint(Texture2D texture, Rect destination, Rect sourcePixels, Color tint)
        {
            return DrawTextureRegionTintVariant(texture, destination, sourcePixels, tint, false, false);
        }

        private bool DrawTextureRegionTintVariant(Texture2D texture, Rect destination, Rect sourcePixels, Color tint, bool flipX, bool flipY)
        {
            if (texture == null || sourcePixels.width <= 0f || sourcePixels.height <= 0f) return false;
            Rect fit = AspectFit(destination, sourcePixels.width, sourcePixels.height);
            Rect tex = new Rect(
                sourcePixels.x / texture.width,
                1f - (sourcePixels.y + sourcePixels.height) / texture.height,
                sourcePixels.width / texture.width,
                sourcePixels.height / texture.height);
            if (flipX)
            {
                tex.x += tex.width;
                tex.width = -tex.width;
            }
            if (flipY)
            {
                tex.y += tex.height;
                tex.height = -tex.height;
            }

            Color old = GUI.color;
            GUI.color = tint;
            GUI.DrawTextureWithTexCoords(fit, texture, tex, true);
            GUI.color = old;
            return true;
        }

        private void DrawBanner()
        {
            if (Time.time > bannerUntil || string.IsNullOrEmpty(bannerText)) return;
            Rect rect = new Rect(Screen.width / 2f - 210, 114, 420, 48);
            DrawRect(rect, Hex("171c20", 0.96f));
            DrawBorder(rect, gold, 1);
            GUI.Label(rect, bannerText, CenterStyle(18, ink));
        }

        private void ShowLootPanel(InventoryItem item, int goldFound, int suppliesFound, int elixirsFound, string equipNote, string title = "Cache opened")
        {
            if (item == null) return;
            showArmory = false;
            showDialogue = false;
            showSpellbook = false;
            showAbilityPanel = false;
            lootPanelTitle = string.IsNullOrEmpty(title) ? "Loot recovered" : title;
            lootPanelTraitLine = ItemTraitLine(item);
            lootPanelEquipNote = string.IsNullOrWhiteSpace(equipNote) ? "" : equipNote;
            lootPanelBody = $"{item.DisplayName}\n{lootPanelTraitLine}\n{lootPanelEquipNote}";
            lootPanelItem = item;
            lootPanelGold = goldFound;
            lootPanelSupplies = suppliesFound;
            lootPanelElixirs = elixirsFound;
            // Rewards contain comparison and auto-equip information that should not
            // disappear while the player is reading it. Explicit dismissal also
            // gives the modal/input layer one clear owner.
            lootPanelUntil = 0f;
            lootPanelRequiresDismissal = true;
            MarkUiDirty();
            SyncLootPopupScreen();
            QueueSfx("itemtake", 0.06f, 0.38f);
        }

        private void ShowDialogue(string title, string speaker, string body, ObjectType focus, Color accent)
        {
            if (state == null) return;
            ClearQueuedDialogueLoot();
            dialogueChoices = Array.Empty<DialogueChoiceView>();
            dialogueChoiceHandler = null;
            dialogueTopicChoices = Array.Empty<DialogueChoiceView>();
            dialogueTopicChoiceHandler = null;
            dialogueReturnToTopics = null;
            dialogueShowingResponse = false;
            dialogueTitle = string.IsNullOrWhiteSpace(title) ? "Midgaard" : title;
            dialogueSpeaker = string.IsNullOrWhiteSpace(speaker) ? ObjectName(focus) : speaker;
            dialogueBody = string.IsNullOrWhiteSpace(body) ? ObjectHint(focus) : body;
            dialoguePages = DialoguePagingRules.Paginate(dialogueBody);
            dialoguePageIndex = 0;
            dialogueFocus = focus;
            dialogueAccentColor = accent;
            dialogueFallbackScroll = Vector2.zero;
            dialogueOpenedFrame = Time.frameCount;
            SuppressBoardPointer();
            showDialogue = true;
            showArmory = false;
            showSpellbook = false;
            showAbilityPanel = false;
            DismissLootPopupSilently();
            MarkUiDirty();
            SyncDialogueScreen();
            PlaySfx("dialogueopen", 0.52f);
        }

        private void ShowDialogueChoices(
            string title,
            string speaker,
            string body,
            ObjectType focus,
            Color accent,
            DialogueChoiceView[] choices,
            Action<string> onChoice)
        {
            if (!showDialogue)
            {
                ShowDialogue(title, speaker, body, focus, accent);
            }
            else
            {
                dialogueTitle = string.IsNullOrWhiteSpace(title) ? "Midgaard" : title;
                dialogueSpeaker = string.IsNullOrWhiteSpace(speaker) ? ObjectName(focus) : speaker;
                dialogueBody = string.IsNullOrWhiteSpace(body) ? ObjectHint(focus) : body;
                dialoguePages = DialoguePagingRules.Paginate(dialogueBody);
                dialoguePageIndex = 0;
                dialogueFocus = focus;
                dialogueAccentColor = accent;
                dialogueFallbackScroll = Vector2.zero;
                dialogueOpenedFrame = Time.frameCount;
                SuppressBoardPointer();
            }
            dialogueChoices = choices ?? Array.Empty<DialogueChoiceView>();
            dialogueChoiceHandler = onChoice;
            dialogueTopicChoices = dialogueChoices;
            dialogueTopicChoiceHandler = dialogueChoiceHandler;
            dialogueReturnToTopics = null;
            dialogueShowingResponse = false;
            MarkUiDirty();
            SyncDialogueScreen();
        }

        private void ShowDialogueResponse(
            string title,
            string speaker,
            string body,
            ObjectType focus,
            Color accent,
            Action returnToTopics)
        {
            dialogueTitle = string.IsNullOrWhiteSpace(title) ? "Midgaard" : title;
            dialogueSpeaker = string.IsNullOrWhiteSpace(speaker) ? ObjectName(focus) : speaker;
            dialogueBody = string.IsNullOrWhiteSpace(body) ? ObjectHint(focus) : body;
            dialoguePages = DialoguePagingRules.Paginate(dialogueBody);
            dialoguePageIndex = 0;
            dialogueFocus = focus;
            dialogueAccentColor = accent;
            dialogueFallbackScroll = Vector2.zero;
            dialogueChoices = Array.Empty<DialogueChoiceView>();
            dialogueChoiceHandler = null;
            dialogueReturnToTopics = returnToTopics;
            dialogueShowingResponse = true;
            dialogueOpenedFrame = Time.frameCount;
            SuppressBoardPointer();
            MarkUiDirty();
            SyncDialogueScreen();
        }

        private void AdvanceDialogue()
        {
            if (!showDialogue) return;
            int pageCount = dialoguePages == null ? 0 : dialoguePages.Length;
            if (dialoguePageIndex + 1 < pageCount)
            {
                dialoguePageIndex++;
                dialogueFallbackScroll = Vector2.zero;
                SuppressBoardPointer();
                MarkUiDirty();
                SyncDialogueScreen();
                PlaySfx("dialoguepage", 0.44f);
                return;
            }

            if (dialogueShowingResponse && ReturnDialogueToTopics()) return;
            CloseDialogue();
        }

        private void CloseDialogue()
        {
            if (!showDialogue) return;
            SuppressBoardPointer();
            showDialogue = false;
            dialogueChoices = Array.Empty<DialogueChoiceView>();
            dialogueChoiceHandler = null;
            dialogueTopicChoices = Array.Empty<DialogueChoiceView>();
            dialogueTopicChoiceHandler = null;
            dialogueReturnToTopics = null;
            dialogueShowingResponse = false;
            MarkUiDirty();
            SyncDialogueScreen();
            PlaySfx("dialogueclose", 0.38f);
            OpenQueuedDialogueLoot();
        }

        private string CurrentDialoguePage()
        {
            if (dialoguePages == null || dialoguePages.Length == 0)
            {
                dialoguePages = DialoguePagingRules.Paginate(dialogueBody);
                dialoguePageIndex = 0;
            }
            dialoguePageIndex = Mathf.Clamp(dialoguePageIndex, 0, dialoguePages.Length - 1);
            return dialoguePages[dialoguePageIndex];
        }

        private string DialoguePageLabel()
        {
            int count = dialoguePages == null ? 0 : dialoguePages.Length;
            return count > 1 ? $"{dialoguePageIndex + 1} / {count}" : "";
        }

        private string DialogueContinueLabel()
        {
            int count = dialoguePages == null ? 0 : dialoguePages.Length;
            if (dialoguePageIndex + 1 < count) return "Next";
            if (dialogueShowingResponse && dialogueReturnToTopics != null) return "Back to topics";
            return IsDialogueChoicePage() ? "Leave" : "Continue";
        }

        private bool IsDialogueChoicePage()
        {
            int pageCount = dialoguePages == null ? 0 : dialoguePages.Length;
            return dialogueChoices != null
                && dialogueChoices.Length > 0
                && dialoguePageIndex >= Mathf.Max(0, pageCount - 1);
        }

        private bool ReturnDialogueToTopics()
        {
            if (!showDialogue || !dialogueShowingResponse || dialogueReturnToTopics == null) return false;
            Action returnToTopics = dialogueReturnToTopics;
            dialogueReturnToTopics = null;
            dialogueShowingResponse = false;
            SuppressBoardPointer();
            returnToTopics();
            PlaySfx("dialoguepage", 0.42f);
            return true;
        }

        private void ChooseDialogueChoice(string id)
        {
            if (!IsDialogueChoicePage() || string.IsNullOrWhiteSpace(id)) return;
            for (int i = 0; i < dialogueChoices.Length; i++)
            {
                DialogueChoiceView choice = dialogueChoices[i];
                if (choice == null || !choice.Enabled || !string.Equals(choice.Id, id, StringComparison.Ordinal)) continue;
                Action<string> handler = dialogueChoiceHandler;
                dialogueChoices = Array.Empty<DialogueChoiceView>();
                dialogueChoiceHandler = null;
                SuppressBoardPointer();
                PlaySfx("dialoguepage", 0.50f);
                MarkUiDirty();
                if (handler != null)
                {
                    handler(id);
                    if (showDialogue) SyncDialogueScreen();
                }
                else CloseDialogue();
                return;
            }
        }

        private void ChooseDialogueChoice(int index)
        {
            if (!IsDialogueChoicePage() || index < 0 || index >= dialogueChoices.Length) return;
            DialogueChoiceView choice = dialogueChoices[index];
            if (choice == null || !choice.Enabled) return;
            ChooseDialogueChoice(choice.Id);
        }

        private void ShowDialogueThenLoot(
            string title,
            string speaker,
            string body,
            ObjectType focus,
            Color accent,
            InventoryItem item,
            int goldFound,
            int suppliesFound,
            int elixirsFound,
            string equipNote,
            string lootTitle)
        {
            ShowDialogue(title, speaker, body, focus, accent);
            queuedDialogueLootItem = item;
            queuedDialogueLootTitle = lootTitle ?? "Loot recovered";
            queuedDialogueLootEquipNote = equipNote ?? "";
            queuedDialogueLootGold = goldFound;
            queuedDialogueLootSupplies = suppliesFound;
            queuedDialogueLootElixirs = elixirsFound;
            MarkUiDirty();
        }

        private void OpenQueuedDialogueLoot()
        {
            if (queuedDialogueLootItem == null) return;
            InventoryItem item = queuedDialogueLootItem;
            string title = queuedDialogueLootTitle;
            string equipNote = queuedDialogueLootEquipNote;
            int goldFound = queuedDialogueLootGold;
            int suppliesFound = queuedDialogueLootSupplies;
            int elixirsFound = queuedDialogueLootElixirs;
            ClearQueuedDialogueLoot();
            ShowLootPanel(item, goldFound, suppliesFound, elixirsFound, equipNote, title);
        }

        private void ClearQueuedDialogueLoot()
        {
            queuedDialogueLootItem = null;
            queuedDialogueLootTitle = "";
            queuedDialogueLootEquipNote = "";
            queuedDialogueLootGold = 0;
            queuedDialogueLootSupplies = 0;
            queuedDialogueLootElixirs = 0;
        }

        private string CacheSupplyLine(int suppliesFound, int elixirsFound)
        {
            List<string> parts = new List<string>();
            if (suppliesFound > 0) parts.Add(suppliesFound == 1 ? "1 supply" : $"{suppliesFound} supplies");
            if (elixirsFound > 0) parts.Add(elixirsFound == 1 ? "1 elixir" : $"{elixirsFound} elixirs");
            return parts.Count == 0 ? "" : ", " + string.Join(", ", parts.ToArray());
        }

        private string ItemTraitLine(InventoryItem item)
        {
            if (item == null) return "";
            List<string> parts = new List<string>();
            if (InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form))
            {
                int range = WeaponRange(item, state?.Party?.FirstOrDefault() ?? new PartyMember { Role = "" });
                parts.Add(range > 1 ? $"range {range}" : "melee");
                if (item.DamageMin > 0 && item.DamageMax > 0) parts.Add($"{item.DamageMin}-{item.DamageMax} dmg");
                if (item.AttackSpeed > 0) parts.Add($"spd {item.AttackSpeed}");
                if (!string.IsNullOrEmpty(item.DamageType) && item.DamageType != "physical") parts.Add(item.DamageType);
                string enchantment = WeaponEnchantmentRules.StatusText(item);
                if (!string.IsNullOrEmpty(enchantment)) parts.Add(enchantment);
                string status = GearOnHitStatus(item.DisplayName);
                if (!string.IsNullOrEmpty(status)) parts.Add(status + " chance");
                if (GearLifeDrainAmount(item.DisplayName, Mathf.Max(1, item.DamageMax)) > 0) parts.Add("life drain");
                if ((item.DisplayName ?? "").ToLowerInvariant().Contains("unfathomable darkness")) parts.Add("mild vorpal");
            }
            else
            {
                parts.Add($"armor {ArmorDefenseBonus(item)}");
                if (ArmorAgilityModifier(item.DisplayName) > 0) parts.Add("light");
                if (ArmorAgilityModifier(item.DisplayName) < 0) parts.Add("heavy");
                if ((item.DisplayName ?? "").ToLowerInvariant().Contains("ward")) parts.Add("warding");
            }
            string stats = ItemStatBonusLine(item);
            if (!string.IsNullOrEmpty(stats)) parts.Add(stats);
            if (!string.IsNullOrEmpty(item.Rarity) && item.Rarity != "starter") parts.Add(item.Rarity);
            return parts.Count == 0 ? "Plain but serviceable." : "Traits: " + string.Join(" / ", parts);
        }

        private string WeaponSummaryLine(PartyMember member)
        {
            if (member == null) return "";
            List<string> parts = new List<string>
            {
                $"range {member.Range}",
                $"{member.DamageMin}-{member.DamageMax} dmg",
                $"spd {member.AttackSpeed}",
                $"scales {WeaponPrimaryStatLabel(member)}",
                string.IsNullOrEmpty(member.WeaponDamageType) ? "physical" : member.WeaponDamageType
            };
            int hit = WeaponHitBonus(member.WeaponName);
            int power = WeaponPowerBonus(member.WeaponName);
            if (hit != 0) parts.Add("hit " + Signed(hit));
            if (power != 0) parts.Add("power " + Signed(power));
            string status = GearOnHitStatus(member.WeaponName);
            if (!string.IsNullOrEmpty(status)) parts.Add(status + " chance");
            if (GearLifeDrainAmount(member.WeaponName, Mathf.Max(1, member.DamageMax)) > 0) parts.Add("life drain");
            return string.Join(" / ", parts);
        }

        private string ItemStatBonusLine(InventoryItem item)
        {
            if (item == null) return "";
            List<string> stats = new List<string>();
            if (item.StrengthBonus != 0) stats.Add("STR " + Signed(item.StrengthBonus));
            if (item.IntelligenceBonus != 0) stats.Add("INT " + Signed(item.IntelligenceBonus));
            if (item.AgilityBonus != 0) stats.Add("AGI " + Signed(item.AgilityBonus));
            if (item.HealthBonus != 0) stats.Add("HP " + Signed(item.HealthBonus));
            return string.Join(" ", stats);
        }

        private string ArmorSummaryLine(PartyMember member)
        {
            if (member == null) return "";
            int agility = ArmorAgilityModifier(member.ArmorName);
            int guard = GearGuardBonus(new CombatUnit { ArmorName = member.ArmorName, WeaponName = member.WeaponName, Spell = member.Spell });
            List<string> parts = new List<string> { $"armor {member.ArmorBonus}" };
            if (guard > 0) parts.Add("guard +" + guard);
            if (agility != 0) parts.Add("agi " + Signed(agility));
            string text = (member.ArmorName ?? "").ToLowerInvariant();
            if (text.Contains("ward") || text.Contains("anti-magic") || text.Contains("moonstone")) parts.Add("wards magic");
            return string.Join(" / ", parts);
        }

        private string BestFitLine(InventoryItem item)
        {
            if (item == null || state?.Party == null || state.Party.Count == 0) return "";
            if (InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form))
            {
                PartyMember target = state.Party.OrderByDescending(p => WeaponRoleFit(item, p)).ThenBy(p => p.WeaponBonus).FirstOrDefault();
                if (target == null) return "";
                string type = string.IsNullOrEmpty(item.DamageType) ? "physical" : item.DamageType;
                return $"Best fit: {target.Name}\nrange {WeaponRange(item, target)} / {type} / bonus {Signed(item.Bonus)}";
            }
            else
            {
                PartyMember target = state.Party.OrderBy(p => ArmorRolePenalty(item, p)).ThenBy(p => p.ArmorBonus).FirstOrDefault();
                if (target == null) return "";
                int agility = ArmorAgilityModifier(item.DisplayName);
                string weight = agility > 0 ? "light" : agility < 0 ? "heavy" : "steady";
                return $"Best fit: {target.Name}\narmor {ArmorDefenseBonus(item)} / {weight} / bonus {Signed(item.Bonus)}";
            }
        }

        private string FormulaCasterSummary()
        {
            int mend = state?.Party?.Count(p => CasterKnowsSchool(p.Spell, "mend")) ?? 0;
            int emberCount = state?.Party?.Count(p => CasterKnowsSchool(p.Spell, "ember")) ?? 0;
            int hexCount = state?.Party?.Count(p => CasterKnowsSchool(p.Spell, "hex")) ?? 0;
            int pactCount = state?.Party?.Count(p => CasterKnowsSchool(p.Spell, "pact")) ?? 0;
            return $"Crafts: priest {mend}, ember {emberCount}, hex {hexCount}, pact {pactCount}. Spell rows show required level; combat Spellbook only shows learned formulas.";
        }

        private string FormulaRuleLine(FormulaDef formula)
        {
            if (formula == null) return "";
            string splash = formula.Splash ? " / splash" : "";
            string sight = FormulaRequiresLineOfSight(formula) ? " / sight" : FormulaArcsOverCover(formula) ? " / arc" : "";
            string pact = formula.Effect == "summon" ? $" / pact {SummonBurden(formula.SummonRole)}" : "";
            return $"L{FormulaRequiredLevel(formula)} {FormulaTierLabel(formula)} / {formula.Mana} MP / r{formula.Range} / {formula.Target}{splash}{sight}{pact}";
        }

        private string FormulaEffectLine(FormulaDef formula)
        {
            if (formula == null) return "";
            if (formula.Effect == "terrain") return formula.Terrain + (formula.Duration > 0 ? $" {formula.Duration}t" : " block");
            if (formula.Effect == "summon") return $"{SummonDisplayName(formula.SummonRole)} {Mathf.Max(1, formula.Duration)}t";
            if (formula.Effect == "damage" || formula.Effect == "drain")
            {
                string type = string.IsNullOrEmpty(formula.DamageType) ? "magic" : formula.DamageType;
                string extra = formula.Splash ? " splash" : "";
                if (!string.IsNullOrEmpty(formula.Status)) extra += " " + StatusLabel(formula.Status);
                return $"{type}{extra}";
            }
            if (formula.Effect == "status") return StatusLabel(formula.Status);
            return formula.Effect;
        }

        private string SummonDisplayName(string role)
        {
            if (string.Equals(role, "boundimp", StringComparison.OrdinalIgnoreCase)) return "Bound Imp";
            if (string.Equals(role, "lesserdemon", StringComparison.OrdinalIgnoreCase)) return "Lesser Demon";
            if (string.Equals(role, "greaterdemon", StringComparison.OrdinalIgnoreCase)) return "Greater Demon";
            if (string.IsNullOrWhiteSpace(role)) return "Summon";
            return DisplayRace(role);
        }

        private string Signed(int value)
        {
            return value > 0 ? "+" + value : value.ToString();
        }
    }
}
