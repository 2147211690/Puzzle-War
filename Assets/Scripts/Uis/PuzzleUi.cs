using System;
using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Uis
{
    public class PuzzleUi : MonoBehaviour
    {
        public PuzzleController puzzleController;
        public Vector2Int puzzleSize;
        public Texture2D puzzleTexture;

        public GameObject startUi;
        public GameObject gammingUi;
        
        public GameObject settingPopup;
        public GameObject winPopup;
        public GameObject confirmPopup;
        public LevelPopup levelPopup;

        public GameObject popup;
        [Header("设置")]
        public Scrollbar sfxVolumeSlider;
        public Scrollbar bgmVolumeSlider;
        [Header("证明")]
        public TMP_Text confirmTipText;
               
        public event EventHandler? Home;
        public event EventHandler? Replay;
        public event EventHandler<(float SfxVolume, float BgmVolume)>? SettingChanged;
        public event EventHandler? NextLevel;
        public event EventHandler<int>? SelectLevel;
        
        private string _currentPopupName = string.Empty;
        public event EventHandler? StartGame;
        public void OnStartGame()
        {
            StartGame?.Invoke(this, EventArgs.Empty);
        }

        public void OpenPopup(string args)
        {
            popup.SetActive(true);
            string popupName = args;
            if (popupName == "Setting")
            {
                settingPopup.SetActive(true);
            }
            else if (popupName == "Win")
            {
                winPopup.SetActive(true);
            }
            else if (popupName == "Replay")
            {
                confirmPopup.SetActive(true);
                confirmTipText.text = $"你要重试吗?";
            }
            else if (popupName == "Home")
            {
                confirmPopup.SetActive(true);
                confirmTipText.text = $"你要返回主页吗?";
            }
            else if (popupName == "Level")
            {
                levelPopup.gameObject.SetActive(true);
            }
            _currentPopupName = popupName;
        }

        public void ClosePopup(bool confirm)
        {
            popup.SetActive(false);
            if (_currentPopupName == "Setting")
            {
                settingPopup.SetActive(false);
                if (confirm) 
                {
                    OnSettingChanged();
                }
            }
            else if (_currentPopupName == "Win")
            {
                winPopup.SetActive(false);
            }
            else if (_currentPopupName == "Replay")
            {
                confirmPopup.SetActive(false);
                if (confirm)
                {
                    OnReplay();
                }
            }
            else if (_currentPopupName == "Home")
            {
                confirmPopup.SetActive(false);
                if (confirm)
                {
                    OnHome();
                }
            }
            else if (_currentPopupName == "Level")
            {
                levelPopup.gameObject.SetActive(false);
                if (confirm)
                {
                    OnSelectLevel(levelPopup.SelectedLevel);
                }
            }
            _currentPopupName = string.Empty;
        }
        public void OnSettingChanged()
        {
            SettingChanged?.Invoke(this, (sfxVolumeSlider.value, bgmVolumeSlider.value));
        }
        public void OnHome()
        {
            Home?.Invoke(this, EventArgs.Empty);
        }
        public void OnReplay()
        {
            Replay?.Invoke(this, EventArgs.Empty);
        }
        public void OnNextLevel()
        {
            NextLevel?.Invoke(this, EventArgs.Empty);
        }
        public void OnSelectLevel(int level)
        {
            SelectLevel?.Invoke(this, level);
        }
    }
}