using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    public class PuzzleModel
    {
        public Vector2Int Size { get; }
        public int StepCount { get; set; }
        public IReadOnlyCollection<Barrier> Barriers => _barriers;
        private PieceModel[,] _pieceModels;
        private Vector2Int[] _coords;
        private HashSet<Barrier> _barriers = new();
        public PuzzleModel(Vector2Int size)
        {
            Size = size;
            _pieceModels = new PieceModel[Size.x, Size.y];
            _coords = new Vector2Int[Size.x * Size.y];
            ClearPieceModels();
        }
        public void Swap(Vector2Int coords1, Vector2Int coords2)
        {
            var c1 = _pieceModels[coords1.x, coords1.y];
            var c2 = _pieceModels[coords2.x, coords2.y];
            _coords[c1.Id] = coords2;
            _coords[c2.Id] = coords1;
            SwapPieceModels(coords1, coords2);
        }
        public void Clear()
        {
            ClearPieceModels();
            ClearBarriers();
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
        //需要保证id 0-size.x*size.y-1,且每个id只出现一次
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
        public void SetPieceType(in Vector2Int coords, PieceTypeEnum type)
        {
            _pieceModels[coords.x, coords.y].Type = type;
        }
        public void AddBarrier(in Vector2Int coords1, in Vector2Int coords2) => _barriers.Add(new(coords1, coords2));
        public bool IsBarrier(in Vector2Int coords1, in Vector2Int coords2) => _barriers.Contains(new(coords1, coords2));
        public void RemoveBarrier(in Vector2Int coords1, in Vector2Int coords2) => _barriers.Remove(new(coords1, coords2));
        public void ClearBarriers() => _barriers.Clear();
        public Vector2Int GetCoords(int id) => _coords[id];
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
