using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "Create", menuName = "HighlightableStyle", order = 0)]
public class HighlightableStyle : ScriptableObject
{
    [Header("放大动画")]
    [Tooltip("目标缩放倍数")]
    public float targetScale = 1.2f;
    [Tooltip("放大动画时长(秒)")]
    public float scaleDuration = 0.3f;

    [Header("抖动动画")]
    [Tooltip("抖动幅度(角度)")]
    public float shakeStrength = 15f;
    [Tooltip("单次抖动时长")]
    public float shakeDuration = 0.4f;
    [Tooltip("抖动间隔时间")]
    public float shakeInterval = 2f;
    
    [Header("其他设置")]
    [Tooltip("启动延迟")]
    public float startDelay = 0.1f;
    [Tooltip("是否循环抖动")]
    public bool loopShake = true;
}