using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Uis
{
    public class LevelPopup : MonoBehaviour
    {
        public int SelectedLevel { get; set; } = 0;
        public GameObject levelButtonPrefab = null!;
        public RectTransform boxRectTransform = null!;
        public GridLayoutGroup boxGridLayout = null!;

        public Vector2 cellSize;
        public Vector2Int cellCount;
        public float spacing;
        public UnityEvent selectComplete = new();
        public int PageCount { get; private set; }

        public int CurrentPageIndex
        {
            get => _currentPageIndexIndex;
            set
            {
                if (value == _currentPageIndexIndex) return;
                _currentPageIndexIndex = value;
            }
        }

        public int LevelCount
        {
            get => _levelCount;
            set
            {
                if (value == _levelCount) return;
                _levelCount = value;
                PageCount = (int)Math.Ceiling((double)_levelCount / (cellCount.x * cellCount.y));
                _currentPageIndexIndex = Mathf.Clamp(CurrentPageIndex, 0, PageCount - 1);
            }
        }

        public int MaxUnlockedLevel
        {
            get => _maxUnlockedLevel;
            set
            { 
                if (value == _maxUnlockedLevel) return;
                _maxUnlockedLevel = value;
            }
        }
        
        public Button nextPageButton = null!;
        public Button prevPageButton = null!;
        
        private LevelButton[,] levelButtons;
        private int _levelCount = 0;
        private int _currentPageIndexIndex = 0;
        private int _maxUnlockedLevel = 10;

        public void Awake()
        {
            nextPageButton.onClick.AddListener(OnNextPage);
            prevPageButton.onClick.AddListener(OnPrevPage);
            
            boxRectTransform.sizeDelta =
                new Vector2(cellSize.x * cellCount.x + spacing * (cellCount.x - 1) + 1f, 
                    cellSize.y * cellCount.y + spacing * (cellCount.y - 1) + 1f);
            boxGridLayout.cellSize = cellSize;
            boxGridLayout.spacing = new Vector2(spacing, spacing);
            levelButtons = new LevelButton[cellCount.x, cellCount.y];
            for (int i = 0; i < cellCount.x * cellCount.y; i++)
            {
                var cell = Instantiate(levelButtonPrefab, boxRectTransform).GetComponent<LevelButton>();
                var id = i;
                cell.button.onClick.AddListener(() =>
                {
                    OnClick(id);
                });
                levelButtons[i % cellCount.x, i / cellCount.x] = cell;
            }
        }

        private void OnEnable()
        {
            UpdateLevelButtons();
        }

        private void OnClick(int index)
        {
            var level = GetRealLevel(index);
            if (level > MaxUnlockedLevel) return;
            SelectedLevel = level;
            selectComplete.Invoke();
        }
        private void UpdateLevelButtons()
        {
            for (int i = 0; i < cellCount.x * cellCount.y; i++)
            {
                var cell = levelButtons[i % cellCount.x, i / cellCount.x];
                var level = GetRealLevel(i);
                cell.Level = level;
                cell.IsSelected = SelectedLevel == level;
                cell.IsUnlocked = level <= MaxUnlockedLevel;
                cell.gameObject.SetActive(level < LevelCount);
            }
            nextPageButton.interactable = CurrentPageIndex < PageCount - 1;
            prevPageButton.interactable = CurrentPageIndex > 0;
        }

        public void OnNextPage()
        {
            CurrentPageIndex++;
            UpdateLevelButtons();
        }

        public void OnPrevPage()
        {
            CurrentPageIndex--;
            UpdateLevelButtons();
        }
        private int GetRealLevel(int index) => index + cellCount.x * cellCount.y * CurrentPageIndex;
    }
}