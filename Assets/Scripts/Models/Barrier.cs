using System;
using UnityEngine;

namespace Models
{
    public readonly struct Barrier : IEquatable<Barrier>
    {
        public Barrier(Vector2Int a, Vector2Int b)
        {
            // 一次性比较：先比x，x相同再比y
            bool aFirst = a.x < b.x || (a.x == b.x && a.y < b.y);
            Coords1 = aFirst ? a : b;
            Coords2 = aFirst ? b : a;
        }
        public Vector2Int Coords1 { get; }
        public Vector2Int Coords2 { get; }
        public override bool Equals(object? obj)
        {
            return obj is Barrier barrier && Coords1 == barrier.Coords1 && Coords2 == barrier.Coords2;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Coords1, Coords2);
        }

        public bool Equals(Barrier other)
        {
            return Coords1.Equals(other.Coords1) && Coords2.Equals(other.Coords2);
        }

        public Vector2Int NormalVec
        {
            get
            {
                var c = Coords2 - Coords1;
                c.y = Mathf.Abs(c.y);
                return c;
            }
        }
    }
}