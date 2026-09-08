using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// YHW OptionsMenuController와 동일한 Prefs 키.
    /// 타이틀 설정 ↔ 인게임 설정 값을 공유한다.
    /// </summary>
    public static class GameSettingsStore
    {
        public const string PrefMaster = "settings.masterVolume";
        public const string PrefBgm = "settings.bgmVolume";
        public const string PrefSfx = "settings.sfxVolume";
        public const string PrefQuality = "settings.qualityIndex";
        public const string PrefFullscreen = "settings.fullscreen";
        public const string PrefResolutionIndex = "settings.resolutionIndex";

        public static float MasterVolume => PlayerPrefs.GetFloat(PrefMaster, 1f);
        public static float BgmVolume => PlayerPrefs.GetFloat(PrefBgm, 0.8f);
        public static float SfxVolume => PlayerPrefs.GetFloat(PrefSfx, 0.8f);

        public static void ApplyBootSettings()
        {
            AudioListener.volume = MasterVolume;

            int quality = PlayerPrefs.GetInt(PrefQuality, QualitySettings.GetQualityLevel());
            quality = Mathf.Clamp(quality, 0, QualitySettings.names.Length - 1);
            QualitySettings.SetQualityLevel(quality, true);

            if (PlayerPrefs.HasKey(PrefFullscreen))
                Screen.fullScreen = PlayerPrefs.GetInt(PrefFullscreen, 1) == 1;
        }

        public static void SaveAudio(float master, float bgm, float sfx)
        {
            PlayerPrefs.SetFloat(PrefMaster, Mathf.Clamp01(master));
            PlayerPrefs.SetFloat(PrefBgm, Mathf.Clamp01(bgm));
            PlayerPrefs.SetFloat(PrefSfx, Mathf.Clamp01(sfx));
            AudioListener.volume = Mathf.Clamp01(master);
            PlayerPrefs.Save();
        }

        public static void SaveDisplay(int qualityIndex, bool fullscreen, int resolutionIndex)
        {
            qualityIndex = Mathf.Clamp(qualityIndex, 0, QualitySettings.names.Length - 1);
            QualitySettings.SetQualityLevel(qualityIndex, true);
            Screen.fullScreen = fullscreen;

            PlayerPrefs.SetInt(PrefQuality, qualityIndex);
            PlayerPrefs.SetInt(PrefFullscreen, fullscreen ? 1 : 0);
            PlayerPrefs.SetInt(PrefResolutionIndex, Mathf.Max(0, resolutionIndex));
            PlayerPrefs.Save();
        }
    }
}
