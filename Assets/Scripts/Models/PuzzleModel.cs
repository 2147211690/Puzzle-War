using System.Collections.Generic;
using System.Linq;
using Tools;
using UnityEngine;

namespace Models
{
    public class PuzzleModel
    {
        public Vector2Int Size { get; }
        public int StepCount { get; set; }
        public IReadOnlyCollection<Barrier> Barriers => _barriers;
        public Texture2D Texture2D { get; private set; }
        public IReadOnlyList<Sprite> Sprites => _sprites;
        private PieceModel[,] _pieceModels;
        private Vector2Int[] _coords;
        private HashSet<Barrier> _barriers = new();
        private Sprite[] _sprites;
        public PuzzleModel(Vector2Int size)
        {
            Size = size;
            _pieceModels = new PieceModel[Size.x, Size.y];
            _coords = new Vector2Int[Size.x * Size.y];
            ClearPieceModels();
        }
        public PuzzleModel(PuzzleData puzzleData)
        {
            Size = puzzleData.size;
            _pieceModels = new PieceModel[Size.x, Size.y];
            _coords = new Vector2Int[Size.x * Size.y];
            foreach (var barrier in puzzleData._barriers) _barriers.Add(barrier);
            _sprites = PuzzleTools.GetTextureSprite(puzzleData.texture, Size);
            Texture2D = puzzleData.texture;
            for (int i = 0; i < puzzleData._pieceModels.Length; i++)
            {
                var x = i % Size.x;
                var y = i / Size.x;
                _pieceModels[x, y] = puzzleData._pieceModels[i];
                _coords[_pieceModels[x, y].Id] = new Vector2Int(x, y);
            }
        }

        public PuzzleData ToPuzzleData()
        {
            var data = ScriptableObject.CreateInstance<PuzzleData>();
            data.size = Size;
            data.texture = Texture2D;
            var pieces = new PieceModel[Size.x * Size.y];
            for (int i = 0; i < pieces.Length; i++)
            {
                var x = i % Size.x;
                var y = i / Size.x;
                pieces[i] = _pieceModels[x, y];
            }
            data._pieceModels = pieces;
            data._barriers = _barriers.ToArray();
            return data;
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

        public void SetTexture(Texture2D texture2D)
        {
            _sprites = PuzzleTools.GetTextureSprite(texture2D, Size);
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
        public void AddBarrier(in Barrier barrier) => _barriers.Add(barrier);
        public bool IsBarrier(in Barrier barrier) => _barriers.Contains(barrier);
        public void RemoveBarrier(in Barrier barrier) => _barriers.Remove(barrier);
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
