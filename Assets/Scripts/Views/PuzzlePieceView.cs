
using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Views
{
    public class PuzzlePieceView : MonoBehaviour
    {
        public Button button = null!;
        public Image image = null!;
        public RectTransform rectTransform = null!;
        public TMP_Text idText = null!;

        public Image broad = null!;
        public RectMask2D mask = null!;
        public int PuzzleId { get; private set; }

        public void Init(int puzzleId, Sprite? sprite, Vector2 renderSize)
        {
            PuzzleId = puzzleId;
            idText.text = PuzzleId.ToString();
            if (sprite is not null) image.sprite = sprite;
            rectTransform.sizeDelta = renderSize + new Vector2Int(1, 1); //加1,避免误差
        }
        
        public void AddClickEvent(Action<int> onClick)
        {
            button.onClick.AddListener(() => onClick(PuzzleId));
        }
        
        public void PlayComplete(float animTime)
        {
            //使用DOTween,mask边距逐渐为0,broad和idText逐渐为透明
            DOTween.To(
                () => mask.padding,
                value => mask.padding = value,
                new Vector4(0, 0, 0, 0),
                animTime
            ).SetEase(Ease.OutQuad);
            // 边框消失
            broad.DOFade(0, animTime).SetEase(Ease.OutQuad);
            // 文字消失
            idText.DOFade(0, animTime).SetEase(Ease.OutQuad);
        }
        
        public void PlayMove(Vector2 localTargetPos, float animTime)
        {
            // 先干掉旧动画，防止冲突
            rectTransform.DOKill(true);

            // 力度感滑动：起步快、滑行稳、停止干脆
            rectTransform
                .DOLocalMove(localTargetPos, animTime)
                .SetEase(Ease.OutSine)
                .SetUpdate(true);
        }
    }
}