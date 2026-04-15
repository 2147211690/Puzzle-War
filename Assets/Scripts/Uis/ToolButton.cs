using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Uis
{
    public class ToolButton : MonoBehaviour
    {
        public Button button = null!;
        public Image toolImage = null!;
        public TMP_Text countText = null!;
        private bool _isUsing;

        public bool IsUsing
        {
            get => _isUsing;
            set
            {
                if (_isUsing == value) return;
                _isUsing = value;
                if (_isUsing)
                {
                    
                }
                else
                {
                    
                }
            }
        }
    }
}