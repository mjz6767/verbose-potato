# Ashen Halls Tool Downloads

These files are staged in `outputs/tools/` for manual install and release hygiene. They were downloaded as helper tools for packaging, sound editing, and safer project history. Installers are not run automatically.

## Staged Files

| Tool | Version | Local file | Purpose | SHA-256 |
| --- | --- | --- | --- | --- |
| Git for Windows | 2.54.0 | `Git-2.54.0-64-bit.exe` | Project history, diffs, branches, tags, and release safety. | `2B96E7854F0520F0F6B709C21041D9801B1BE44D5E1A0D9FA621B2FBC40F1983` |
| 7-Zip | 26.02 | `7z2602-x64.exe` | Package inspection and clean extraction checks. | `6745FA76DC2EA031596D8678F6F6B99C3C1B435B4164A63485ADBBC7B8D82EF0` |
| Audacity | 3.7.8 | `audacity-win-3.7.8-64bit.zip` | Trimming and exporting small one-shot WAV sound effects. | `900620F6E9BB6A9F6D1C0A1A10B58EAF480C0C8A4BFCE134F89E80EDDC83979F` |
| LibreSprite | 1.2 | `libresprite-development-windows-x86_64-v1.2.zip` | Free pixel-art editor for original sprites, tile sheets, enemy variants, and UI icons. | `D3D04642DF395FA90DBE38590880C9F3301A0075BD1E7C243FA37B364BBDF7A4` |

## Source Links

- Git for Windows: https://git-scm.com/downloads/win
- Git release asset: https://github.com/git-for-windows/git/releases/download/v2.54.0.windows.1/Git-2.54.0-64-bit.exe
- 7-Zip downloads: https://www.7-zip.org/download.html
- 7-Zip release asset: https://github.com/ip7z/7zip/releases/download/26.02/7z2602-x64.exe
- Audacity Windows downloads: https://www.audacityteam.org/download/windows/
- Audacity release asset: https://github.com/audacity/audacity/releases/download/Audacity-3.7.8/audacity-win-3.7.8-64bit.zip
- LibreSprite releases: https://github.com/LibreSprite/LibreSprite/releases
- LibreSprite release asset: https://github.com/LibreSprite/LibreSprite/releases/download/v1.2/libresprite-development-windows-x86_64.zip

## Verification

Run `outputs/tools/Verify-ToolDownloads.ps1` from PowerShell to confirm the local files still match the checksums above.

## Install Notes

- Git and 7-Zip are installers and may need normal Windows approval.
- Audacity is staged as a zip archive.
- LibreSprite is staged as a zip archive and can be extracted when we are ready to draw final sprite sheets.
- Aseprite remains optional and paid, so it has not been downloaded automatically.
- Bfxr, sfxr, and ChipTone are still good candidates for later sound passes, but should be selected deliberately.
