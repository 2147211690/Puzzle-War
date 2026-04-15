using System;
using Models;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class BarrierView : MonoBehaviour
    {
        public RectTransform rectTransform = null!;
        public Button button = null!;
        public Image image = null!;
        public Highlightable highlightable = null!;
        public Barrier Barrier { get; set; }
        public void AddClickEvent(Action<Barrier> onClick)
        {
            button.onClick.AddListener(() => onClick(Barrier));
        }
    }
}