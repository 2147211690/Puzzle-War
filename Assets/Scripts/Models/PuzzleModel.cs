using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    public class PuzzleModel
    {
        public Vector2Int Size { get; }
        public int StepCount { get; set; }
        private int[,] _puzzleIds;
        private Dictionary<int, Vector2Int> _coords = new();
        
        public PuzzleModel(Vector2Int size)
        {
            Size = size;
            _puzzleIds = new int[Size.x, Size.y];
            ClearIds();
        }
        public int GetId(Vector2Int coords) => _puzzleIds[coords.x, coords.y];
        public void SetId(Vector2Int coords, int id)
        {
            if (id == -1)
            {
                _coords.Remove(id);
            }
            else
            {
                if (_coords.TryGetValue(id, out var oldCoords)) _puzzleIds[oldCoords.x, oldCoords.y] = -1;
                _coords[id] = coords;
            }
            _puzzleIds[coords.x, coords.y] = id;
        }

        public void Clear()
        {
            _coords.Clear();
            ClearIds();
        }
        
        public Vector2Int GetCoords(int id) => _coords.TryGetValue(id, out var coords) ? coords : Vector2Int.zero;
        public int this[int x, int y]
        {
            get => GetId(new(x, y));
            set => SetId(new(x, y), value);
        }

        public int this[Vector2Int coords]
        {
            get => GetId(coords);
            set => SetId(coords, value);
        }

        public Vector2Int this[int id] => GetCoords(id);
        
        private void ClearIds()
        {
            for (int i = 0; i < Size.x; i++)
            {
                for (int j = 0; j < Size.y; j++)
                {
                    _puzzleIds[i, j] = -1;
                }
            }
        }
    }
}
