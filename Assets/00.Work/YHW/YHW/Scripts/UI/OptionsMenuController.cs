using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace YHW.UI
{
    public class OptionsMenuController : MonoBehaviour
    {
        private const string PrefMaster = "settings.masterVolume";
        private const string PrefBgm = "settings.bgmVolume";
        private const string PrefSfx = "settings.sfxVolume";
        private const string PrefQuality = "settings.qualityIndex";
        private const string PrefFullscreen = "settings.fullscreen";
        private const string PrefResolutionIndex = "settings.resolutionIndex";

        [Header("Root")]
        [SerializeField] private CanvasGroup panelGroup;
        [SerializeField] private RectTransform panelCard;

        [Header("Display")]
        [SerializeField] private Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private Dropdown qualityDropdown;

        [Header("Audio")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider bgmVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Text masterVolumeLabel;
        [SerializeField] private Text bgmVolumeLabel;
        [SerializeField] private Text sfxVolumeLabel;

        [Header("Buttons")]
        [SerializeField] private Button applyButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button quitButton;

        public static float BgmVolume => PlayerPrefs.GetFloat(PrefBgm, 0.8f);
        public static float SfxVolume => PlayerPrefs.GetFloat(PrefSfx, 0.8f);

        private List<Resolution> _resolutions;
        private Action _onClose;

        private void Awake()
        {
            ApplySavedSettingsOnBoot();

            BuildResolutionOptions();
            BuildQualityOptions();

            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            bgmVolumeSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            qualityDropdown.onValueChanged.AddListener(OnQualityPreview);

            applyButton.onClick.AddListener(ApplyAndSave);
            resetButton.onClick.AddListener(ResetToDefault);
            backButton.onClick.AddListener(Close);
            quitButton.onClick.AddListener(TitleMenuController.QuitGame);

            panelGroup.alpha = 0f;
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
        }

        private void ApplySavedSettingsOnBoot()
        {
            AudioListener.volume = PlayerPrefs.GetFloat(PrefMaster, 1f);
            int savedQuality = PlayerPrefs.GetInt(PrefQuality, QualitySettings.GetQualityLevel());
            QualitySettings.SetQualityLevel(Mathf.Clamp(savedQuality, 0, QualitySettings.names.Length - 1), true);
        }

        private void BuildResolutionOptions()
        {
            _resolutions = new List<Resolution>();
            var options = new List<string>();
            int currentIndex = 0;
            var seen = new HashSet<string>();

            var allResolutions = Screen.resolutions;
            for (int i = 0; i < allResolutions.Length; i++)
            {
                var r = allResolutions[i];
                string key = r.width + "x" + r.height;
                if (!seen.Add(key)) continue;

                _resolutions.Add(r);
                options.Add(r.width + " x " + r.height);

                if (r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height)
                    currentIndex = _resolutions.Count - 1;
            }

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(options);

            int savedIndex = PlayerPrefs.GetInt(PrefResolutionIndex, currentIndex);
            if (savedIndex < 0 || savedIndex >= _resolutions.Count) savedIndex = currentIndex;
            resolutionDropdown.SetValueWithoutNotify(savedIndex);
        }

        private void BuildQualityOptions()
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
            int savedQuality = PlayerPrefs.GetInt(PrefQuality, QualitySettings.GetQualityLevel());
            qualityDropdown.SetValueWithoutNotify(Mathf.Clamp(savedQuality, 0, QualitySettings.names.Length - 1));
        }

        public void Open(Action onClose)
        {
            _onClose = onClose;
            LoadCurrentValues();

            gameObject.SetActive(true);
            panelGroup.interactable = true;
            panelGroup.blocksRaycasts = true;
            panelGroup.alpha = 0f;
            panelCard.localScale = Vector3.one * 0.9f;

            panelGroup.DOFade(1f, 0.25f);
            panelCard.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }

        private void Close()
        {
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
            panelGroup.DOFade(0f, 0.2f);
            panelCard.DOScale(0.9f, 0.2f).SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    _onClose?.Invoke();
                    _onClose = null;
                });
        }

        private void LoadCurrentValues()
        {
            float master = PlayerPrefs.GetFloat(PrefMaster, 1f);
            float bgm = PlayerPrefs.GetFloat(PrefBgm, 0.8f);
            float sfx = PlayerPrefs.GetFloat(PrefSfx, 0.8f);

            masterVolumeSlider.SetValueWithoutNotify(master);
            bgmVolumeSlider.SetValueWithoutNotify(bgm);
            sfxVolumeSlider.SetValueWithoutNotify(sfx);

            UpdateVolumeLabels(master, bgm, sfx);

            fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
        }

        private void OnMasterVolumeChanged(float v)
        {
            AudioListener.volume = v;
            UpdateVolumeLabels(v, bgmVolumeSlider.value, sfxVolumeSlider.value);
        }

        private void OnBgmVolumeChanged(float v) => UpdateVolumeLabels(masterVolumeSlider.value, v, sfxVolumeSlider.value);

        private void OnSfxVolumeChanged(float v) => UpdateVolumeLabels(masterVolumeSlider.value, bgmVolumeSlider.value, v);

        private void UpdateVolumeLabels(float m, float b, float s)
        {
            if (masterVolumeLabel) masterVolumeLabel.text = Mathf.RoundToInt(m * 100f) + "%";
            if (bgmVolumeLabel) bgmVolumeLabel.text = Mathf.RoundToInt(b * 100f) + "%";
            if (sfxVolumeLabel) sfxVolumeLabel.text = Mathf.RoundToInt(s * 100f) + "%";
        }

        private void OnQualityPreview(int index)
        {
            QualitySettings.SetQualityLevel(index, true);
        }

        private void ApplyAndSave()
        {
            int resIndex = resolutionDropdown.value;
            if (resIndex >= 0 && resIndex < _resolutions.Count)
            {
                var r = _resolutions[resIndex];
                Screen.SetResolution(r.width, r.height, fullscreenToggle.isOn);
            }
            else
            {
                Screen.fullScreen = fullscreenToggle.isOn;
            }

            QualitySettings.SetQualityLevel(qualityDropdown.value, true);

            PlayerPrefs.SetInt(PrefResolutionIndex, resIndex);
            PlayerPrefs.SetInt(PrefFullscreen, fullscreenToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt(PrefQuality, qualityDropdown.value);
            PlayerPrefs.SetFloat(PrefMaster, masterVolumeSlider.value);
            PlayerPrefs.SetFloat(PrefBgm, bgmVolumeSlider.value);
            PlayerPrefs.SetFloat(PrefSfx, sfxVolumeSlider.value);
            PlayerPrefs.Save();

            PulseButton(applyButton);
        }

        private void ResetToDefault()
        {
            masterVolumeSlider.value = 1f;
            bgmVolumeSlider.value = 0.8f;
            sfxVolumeSlider.value = 0.8f;
            qualityDropdown.value = QualitySettings.names.Length - 1;
            fullscreenToggle.isOn = true;

            int nativeIndex = _resolutions.FindIndex(r =>
                r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height);
            resolutionDropdown.value = Mathf.Max(0, nativeIndex);
        }

        private static void PulseButton(Button button)
        {
            button.transform.DOKill();
            button.transform.localScale = Vector3.one;
            button.transform.DOPunchScale(Vector3.one * 0.08f, 0.25f, 6, 0.6f);
        }
    }
}
