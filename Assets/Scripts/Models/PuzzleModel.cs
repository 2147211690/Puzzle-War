using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    public class PuzzleModel
    {
        public Vector2Int Size { get; }
        public int StepCount { get; private set; }
        private PieceModel[,] _pieceModels;
        private Dictionary<int, Vector2Int> _coords = new();
        
        public PuzzleModel(Vector2Int size)
        {
            Size = size;
            _pieceModels = new PieceModel[Size.x, Size.y];
            ClearPieceModels();
        }
        public void Swap(Vector2Int coords1, Vector2Int coords2)
        {
            var c1 = _pieceModels[coords1.x, coords1.y];
            var c2 = _pieceModels[coords2.x, coords2.y];
            _coords[c1.Id] = coords2;
            _coords[c2.Id] = coords1;
            SwapPieceModels(coords1, coords2);
            StepCount++;
        }
        public void Clear()
        {
            _coords.Clear();
            ClearPieceModels();
        }

        public void FillAll()
        {
            for (int i = 0; i < Size.x; i++)
            {
                for (int j = 0; j < Size.y; j++)
                {
                    if (_pieceModels[i, j].IsEmpty)
                        _pieceModels[i, j].IsEmpty = false;
                }
            }
        }
        public void SetPuzzle(PieceModel[,] pieces)
        {
            StepCount = 0;
            for (int i = 0; i < Size.x; i++)
            {
                for (int j = 0; j < Size.y; j++)
                {
                    _pieceModels[i, j] = pieces[i, j];
                    _coords[pieces[i, j].Id] = new Vector2Int(i, j);
                }
            }
        }
        public Vector2Int GetCoords(int id) => _coords.TryGetValue(id, out var coords) ? coords : Vector2Int.zero;
        public PieceModel this[int x, int y] => GetPieceModel(x, y);
        public PieceModel this[Vector2Int coords] => GetPieceModel(coords.x, coords.y);
        public Vector2Int this[int id] => GetCoords(id);
        public PieceModel GetPieceModel(int x, int y) => _pieceModels[x, y];
        private void ClearPieceModels()
        {
            for (int i = 0; i < Size.x; i++)
            {
                for (int j = 0; j < Size.y; j++)
                {
                    _pieceModels[i, j] = new PieceModel() {Id = i + j * Size.x, IsEmpty = true, Type = PieceTypeEnum.Fixed};
                }
            }
        }
        private void SwapPieceModels(Vector2Int coords1, Vector2Int coords2)
        {
            (_pieceModels[coords1.x, coords1.y], _pieceModels[coords2.x, coords2.y]) = (_pieceModels[coords2.x, coords2.y], _pieceModels[coords1.x, coords1.y]);
        }
    }
}
