using System;
using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Uis
{
    public class PuzzleUi : MonoBehaviour
    {
        public PuzzleController puzzleController = null!;
        public Vector2Int puzzleSize;
        public Texture2D puzzleTexture = null!;
        public TMP_Text scoreText = null!;
        public TMP_Text titleText = null!;

        public GameObject homeUi = null!;
        public GameObject gammingUi = null!;
        public GameObject normalUi = null!;
        public GameObject homeCore = null!;
        [Header("弹窗")]
        public GameObject settingPopup = null!;
        public GameObject winPopup = null!;
        public TMP_Text winScoreText = null!;
        public GameObject confirmPopup = null!;
        public LevelPopup levelPopup = null!;
        public GameObject enterSidePopup = null!;

        public GameObject popup = null!;
        [Header("设置")]
        public Scrollbar sfxVolumeSlider = null!;
        public Scrollbar bgmVolumeSlider = null!;
        [Header("证明")]
        public TMP_Text confirmTipText = null!;
        [Header("按钮")]
        public Button settingButton = null!;
        public Button replayButton = null!;
        public Button homeButton = null!;
        public Button levelButton = null!;
        public Button enterSideButton = null!;
        public Button enterSideGetAwardButton = null!;
        public Button unlockLevelButton = null!;
        [Header("道具")]
        public TMP_Text hammerText = null!;
        public TMP_Text scissorsText = null!;
        
        private Action<WimPopupResult>? _home;
        private Action<(float SfxVolume, float BgmVolume)>? _settingChanged;
        private Action<bool>? _confirm;
        private Action<int>? _selectLevel;
        private Action<EnterSidePopupResult>? _enterSideGetAward;
        private bool _isViewing = true;
        public event EventHandler? StartGame;
        public void OnStartGame()
        {
            StartGame?.Invoke(this, EventArgs.Empty);
        }

        public void OpenSettingPopup(Action<(float SfxVolume, float BgmVolume)> settingChanged)
        {
            _settingChanged += settingChanged;
            sfxVolumeSlider.value = AudioManager.Instance.sfxVolume;
            bgmVolumeSlider.value = AudioManager.Instance.bgmVolume;
            popup.SetActive(true);
            settingPopup.SetActive(true);
        }
        public void OnCloseSettingPopup()
        {
            settingPopup.SetActive(false);
            popup.SetActive(false);
            _settingChanged?.Invoke((sfxVolumeSlider.value, bgmVolumeSlider.value));
            _settingChanged = null;
        }

        public void OpenWinPopup(int score, Action<WimPopupResult> home)
        {
            _home += home;
            winPopup.SetActive(true);
            winScoreText.text = $"你赢了\n得分: {score}";
            popup.SetActive(true);
        }
        public void OnCloseWinPopupHome()
        {
            winPopup.SetActive(false);
            popup.SetActive(false);
            _home?.Invoke(WimPopupResult.Home);
            _home = null;
        }
        public void OnCloseWinPopupNextLevel()
        {
            winPopup.SetActive(false);
            popup.SetActive(false);
            _home?.Invoke(WimPopupResult.NextLevel);
            _home = null;
        }
        
        public enum EnterSidePopupResult
        {
            Cancel,
            Ok,
            GetAward
        }
        public void OpenEnterSideBarAwardPopup(bool hasAward, Action<EnterSidePopupResult> getAward)
        {
            _enterSideGetAward += getAward;
            enterSideGetAwardButton.interactable = hasAward;
            popup.SetActive(true);
            enterSidePopup.SetActive(true);
        }
        public void OnCloseEnterSideBarAwardPopupOk()
        {
            OnCloseEnterSiderBar(EnterSidePopupResult.Ok);
        }
        public void OnCloseEnterSideBarAwardPopupCancel()
        {
            OnCloseEnterSiderBar(EnterSidePopupResult.Cancel);
        }
        public void OnCloseEnterSideBarAwardPopupGetAward()
        {
            OnCloseEnterSiderBar(EnterSidePopupResult.GetAward);
        }
        private void OnCloseEnterSiderBar(EnterSidePopupResult r)
        {
            popup.SetActive(false);
            enterSidePopup.SetActive(false);
            _enterSideGetAward?.Invoke(r);
            _enterSideGetAward = null;
        }

        public enum WimPopupResult
        {
            Home,
            NextLevel
        }
        
        public void OpenComfirmPopup(string tip, Action<bool> confirm)
        {
            confirmPopup.SetActive(true);
            popup.SetActive(true);
            confirmTipText.text = tip;
            _confirm += confirm;
        }
        
        public void OnCloseComfirmPopup(bool confirm)
        {
            confirmPopup.SetActive(false);
            popup.SetActive(false);
            _confirm?.Invoke(confirm);
            _confirm = null;
        }

        public void OpenLevelPopup(int levelCount, int maxUnlockLevel, int currentLevel, Action<int> selectLevel)
        {
            _selectLevel += selectLevel;
            levelPopup.LevelCount = levelCount;
            levelPopup.MaxUnlockedLevel = maxUnlockLevel;
            levelPopup.SelectedLevel = currentLevel;
            levelPopup.gameObject.SetActive(true);
            popup.SetActive(true);
        }

        public void OnCloseLevelPopup()
        {
            levelPopup.gameObject.SetActive(false);
            popup.SetActive(false);
            _selectLevel?.Invoke(levelPopup.SelectedLevel);
            _selectLevel = null;
        }

        public void OnViewClicked()
        {
            _isViewing = !_isViewing;
            homeCore.SetActive(_isViewing);
        }
    }
}