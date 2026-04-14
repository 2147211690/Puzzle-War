using System;

namespace Models
{
    [Flags]
    public enum DirMask
    {
        None = 0,
        Up = 1 << 0,
        Down = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
    }
}