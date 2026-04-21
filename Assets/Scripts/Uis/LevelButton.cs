using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Uis
{
    public class LevelButton : MonoBehaviour
    {
        public int Level
        {
            get => _level;
            set
            {
                if (_level == value) return;
                _level = value;
                levelText.text = $"{_level + 1}";
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            { 
                if (_isSelected == value) return;
                _isSelected = value;
                selectObject.SetActive(value);
            }
        }

        public bool IsUnlocked
        {
            get => _isUnlocked;
            set
            {
                if (_isUnlocked == value) return;
                _isUnlocked = value;
                background.color = value ? Color.white : Color.grey;
                button.interactable = value;
            }
        }

        public GameObject selectObject;
        public TMP_Text levelText;
        public Button button;
        public Image background;
        private int _level = 0;
        private bool _isUnlocked = true;
        private bool _isSelected = false;

        private void Awake()
        {
            levelText.text = $"{_level + 1}";
        }
    }
}