using System;

namespace Models
{
    [Flags]
    public enum PieceTypeEnum
    {
        Free,
        Fixed,
        UpDown,
        LeftRight,
    }
}