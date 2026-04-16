using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public struct Barrier : IEquatable<Barrier>, ISerializationCallbackReceiver
    {
        [SerializeField] private Vector2Int _coords1;
        [SerializeField] private Vector2Int _coords2;
        public Barrier(Vector2Int a, Vector2Int b)
        {
            // 一次性比较：先比x，x相同再比y
            bool aFirst = a.x < b.x || (a.x == b.x && a.y < b.y);
            _coords1 = aFirst ? a : b;
            _coords2 = aFirst ? b : a;
        }
        public Vector2Int Coords1 => _coords1;
        public Vector2Int Coords2 => _coords2;

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

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            var a = _coords1;
            var b = _coords2;
            bool aFirst = a.x < b.x || (a.x == b.x && a.y < b.y);
            _coords1 = aFirst ? a : b;
            _coords2 = aFirst ? b : a;
        }
    }
}