using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Views
{
    public class PuzzleView : MonoBehaviour
    {
        public GameObject puzzlePieceViewPrefab = null!;
        public PuzzlePieceView?[,] PuzzlePieceViews { get; set; } = null!;
        public RectTransform rectTransform = null!;
        private Vector2Int _puzzleSize;
        private Vector2 _pieceSize = new(100, 100);
        private Vector2 _offset = new(0.5f, 0.5f);
        public void Init(Vector2Int puzzleSize, (int Id, Sprite? Sprite)[,] spritePairs)
        {
            _puzzleSize = puzzleSize;
            var pSize = rectTransform.rect.size;
            _pieceSize = pSize / puzzleSize;
            _offset = new(- pSize.x / 2 + _pieceSize.x / 2, pSize.y / 2 - _pieceSize.y / 2);
            PuzzlePieceViews = new PuzzlePieceView?[puzzleSize.x, puzzleSize.y];
            for (int i = 0; i < puzzleSize.x; i++)
            {
                for (int j = 0; j < puzzleSize.y; j++)
                {
                    //if (enumerator.Current.Sprite is null) continue;
                    if (spritePairs[i, j].Id == -1) continue;
                    PuzzlePieceViews[i, j] = Instantiate(puzzlePieceViewPrefab, transform).GetComponent<PuzzlePieceView>();
                    PuzzlePieceViews[i, j]!.Init(spritePairs[i, j].Id, spritePairs[i, j].Sprite, _pieceSize);
                    PuzzlePieceViews[i, j]!.rectTransform.localPosition = GetPosition(i, j);
                }
            }
        }

        public void MovePiece(Vector2Int from, Vector2Int to)
        {
            var piece = PuzzlePieceViews[from.x, from.y];
            if (piece is null) return;

            // 目标位置
            Vector2 targetPos = GetPosition(to.x, to.y);
            piece.PlayMove(targetPos, 0.25f);
            // 数组交换
            PuzzlePieceViews[to.x, to.y] = piece;
            PuzzlePieceViews[from.x, from.y] = null;
        }

        public void Wim()
        {
            foreach (var piece in PuzzlePieceViews)
                if (piece is not null)
                    piece.PlayComplete(0.2f);
        }
        private Vector2 GetPosition(int i, int j)
        {
            return _offset + new Vector2(i, - j) * _pieceSize;
        }
    }
}