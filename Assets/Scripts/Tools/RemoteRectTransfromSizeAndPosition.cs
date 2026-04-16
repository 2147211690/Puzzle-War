using UnityEngine;

namespace Tools
{
    [RequireComponent(typeof(RectTransform)), ExecuteAlways]
    public class RemoteRectTransfromSizeAndPosition : MonoBehaviour
    {
        private RectTransform _rectTransform;
        public RectTransform? targetRectTransform;
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }
        private void Update()
        {
            if (targetRectTransform == null) return;
            _rectTransform.sizeDelta = targetRectTransform.sizeDelta;
            _rectTransform.SetPositionAndRotation(targetRectTransform.position, targetRectTransform.rotation);
        }
    }
}