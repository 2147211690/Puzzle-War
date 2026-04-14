
using System;
using DefaultNamespace;
using DG.Tweening;
using Models;
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
        public PuzzlePieceStyle style = null!;
        private PieceTypeEnum _type = PieceTypeEnum.Free;
        public int PuzzleId { get; private set; }

        public PieceTypeEnum Type
        {
            get => _type;
            set
            {
                if (_type == value) return;
                _type = value;
                broad.sprite = _type switch
                {
                    PieceTypeEnum.Free => style.moveSprite,
                    PieceTypeEnum.Fixed => style.fixedSprite,
                    PieceTypeEnum.UpDown => style.moveUpDownSprite,
                    PieceTypeEnum.LeftRight => style.moveLeftRightSprite,
                };
            }
        }

        public void Init(int puzzleId, Sprite? sprite, PieceTypeEnum type, Vector2 renderSize)
        {
            PuzzleId = puzzleId;
            idText.text = PuzzleId.ToString();
            if (sprite is not null) image.sprite = sprite;
            rectTransform.sizeDelta = renderSize + new Vector2Int(2, 2); //加2,避免误差
            Type = type;
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

        public void PlaySummon(float animTime)
        {
            // 先干掉旧动画，防止冲突
            rectTransform.DOKill(true);
            // 初始状态：缩放到0
            rectTransform.localScale = Vector3.zero;
            
            // 灵动缩放：从0弹性放大到1，带有回弹效果
            rectTransform
                .DOScale(1f, animTime)
                .SetEase(Ease.OutBack, overshoot: 1.5f) // 回弹效果，overshoot控制回弹幅度
                .SetUpdate(true);
    
            // 可选：同时淡入（如果初始透明）
            image.DOFade(1f, animTime * 0.5f).From(0f).SetEase(Ease.OutQuad);
    
            // 可选：边框和文字也跟随淡入
            broad.DOFade(1f, animTime * 0.6f).From(0f).SetEase(Ease.OutQuad);
            idText.DOFade(1f, animTime * 0.6f).From(0f).SetEase(Ease.OutQuad);
        }
        public void PlayCompleteSummon(float animTime)
        {
            // 先干掉旧动画，防止冲突
            rectTransform.DOKill(true);
            // 初始状态：缩放到0
            rectTransform.localScale = Vector3.zero;
            mask.padding = Vector4.zero;
            broad.color = Color.clear;
            idText.color = Color.clear;
            // 灵动缩放：从0弹性放大到1，带有回弹效果
            rectTransform
                .DOScale(1f, animTime)
                .SetEase(Ease.OutBack, overshoot: 1.5f) // 回弹效果，overshoot控制回弹幅度
                .SetUpdate(true);
        }
    }
}