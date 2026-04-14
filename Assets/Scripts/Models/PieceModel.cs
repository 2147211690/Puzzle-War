using UnityEngine;

namespace Models
{
    public struct PieceModel
    {
        public int Id { get; set; }
        public bool IsEmpty { get; set; }
        public Sprite? Sprite { get; set; }
        public bool IsCanMove { get; set; }
    }
}