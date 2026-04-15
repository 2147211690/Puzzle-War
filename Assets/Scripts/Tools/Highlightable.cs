using System;
using System.Collections;
using UnityEngine;

namespace Tools
{
    public class Highlightable : MonoBehaviour
    {
        public HighlightableStyle style = null!;
        private RectTransform? _rectTransform;

        private Vector3 _originalScale;
        private Quaternion _originalRotation;
        private Coroutine _currentCoroutine;
        private bool _isHighlighted = false;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            if (style == null)
            {
                Debug.LogWarning($"[{nameof(Highlightable)}] Style is null on {gameObject.name}");
                return;
            }

            _originalScale = transform.localScale;
            _originalRotation = transform.rotation;
            _rectTransform?.SetAsLastSibling();
            StartHighlight();
        }

        private void OnDisable()
        {
            StopHighlight();
        }

        /// <summary>
        /// 开始高亮动画：先放大，然后间歇性抖动
        /// </summary>
        public void StartHighlight()
        {
            if (_isHighlighted || style == null) return;
            _isHighlighted = true;

            StopCurrentCoroutine();
            _currentCoroutine = StartCoroutine(HighlightRoutine());
        }

        /// <summary>
        /// 停止高亮，恢复原状
        /// </summary>
        public void StopHighlight()
        {
            _isHighlighted = false;
            StopCurrentCoroutine();
            RestoreOriginalState();
        }

        /// <summary>
        /// 手动触发一次抖动
        /// </summary>
        public void DoShakeOnce()
        {
            if (style == null) return;
            StopCurrentCoroutine();
            _currentCoroutine = StartCoroutine(ShakeOnceRoutine());
        }

        /// <summary>
        /// 手动触发一次心跳缩放
        /// </summary>
        public void DoPulseOnce()
        {
            if (style == null) return;
            StopCurrentCoroutine();
            _currentCoroutine = StartCoroutine(PulseOnceRoutine());
        }

        #region Coroutine Routines

        private IEnumerator HighlightRoutine()
        {
            // 1. 延迟启动
            yield return new WaitForSeconds(style.startDelay);

            // 2. 放大动画
            yield return StartCoroutine(ScaleRoutine(_originalScale, _originalScale * style.targetScale, style.scaleDuration));

            // 3. 间歇性抖动循环
            while (_isHighlighted && style.loopShake)
            {
                yield return new WaitForSeconds(style.shakeInterval);
                if (!_isHighlighted) yield break;
                
                yield return StartCoroutine(ShakeRoutine());
            }
        }

        private IEnumerator ScaleRoutine(Vector3 from, Vector3 to, float duration)
        {
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // OutBack 缓动效果模拟
                float easedT = OutBackEase(t);
                
                transform.localScale = Vector3.LerpUnclamped(from, to, easedT);
                yield return null;
            }
            
            transform.localScale = to;
        }

        private IEnumerator ShakeRoutine()
        {
            float elapsed = 0f;
            float halfDuration = style.shakeDuration * 0.5f;
            
            // 左摇
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                float angle = Mathf.Lerp(0, style.shakeStrength, Mathf.Sin(t * Mathf.PI));
                transform.rotation = _originalRotation * Quaternion.Euler(0, 0, angle);
                yield return null;
            }
            
            // 右摆
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                float angle = Mathf.Lerp(style.shakeStrength, -style.shakeStrength, Mathf.Sin(t * Mathf.PI));
                transform.rotation = _originalRotation * Quaternion.Euler(0, 0, angle);
                yield return null;
            }
            
            // 回正
            transform.rotation = _originalRotation;
        }

        private IEnumerator ShakeOnceRoutine()
        {
            yield return StartCoroutine(ShakeRoutine());
            RestoreRotation();
        }

        private IEnumerator PulseOnceRoutine()
        {
            float punchScale = (style.targetScale - 1f) * 0.5f;
            Vector3 punchSize = _originalScale * (1f + punchScale);
            
            // 放大
            yield return StartCoroutine(ScaleRoutine(transform.localScale, punchSize, 0.15f));
            // 回弹
            yield return StartCoroutine(ScaleRoutine(punchSize, _originalScale * style.targetScale, 0.15f));
        }

        #endregion

        #region Helper Methods

        private void StopCurrentCoroutine()
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
            }
        }

        private void RestoreOriginalState()
        {
            StopAllCoroutines();
            transform.localScale = _originalScale;
            transform.rotation = _originalRotation;
        }
        private void RestoreRotation()
        {
            transform.rotation = _originalRotation;
        }

        /// <summary>
        /// OutBack 缓动函数：超过目标后回弹
        /// </summary>
        private float OutBackEase(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            
            return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
        }

        #endregion

        #region Editor Test Methods

        [ContextMenu("测试高亮")]
        private void TestHighlight()
        {
            if (style == null)
            {
                Debug.LogError("Style is null!");
                return;
            }
            StartHighlight();
        }

        [ContextMenu("测试停止")]
        private void TestStop()
        {
            StopHighlight();
        }

        [ContextMenu("测试单次抖动")]
        private void TestShake()
        {
            DoShakeOnce();
        }

        #endregion
    }
}