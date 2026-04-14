using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "PuzzlePieceStyle", menuName = "Create/PuzzlePieceStyle", order = 0)]
    public class PuzzlePieceStyle : ScriptableObject
    {
        public Sprite? moveSprite;
        public Sprite? moveUpDownSprite;
        public Sprite? moveLeftRightSprite;
        public Sprite? fixedSprite;
    }
}