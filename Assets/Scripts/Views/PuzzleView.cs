using System.Collections.Generic;
using Controllers;
using DG.Tweening;
using Models;
using UnityEngine;

namespace Views
{
    public class PuzzleView : MonoBehaviour
    {
        public GameObject puzzlePieceViewPrefab = null!;
        public PuzzlePieceView[,] PuzzlePieceViews { get; set; } = null!;
        public RectTransform rectTransform = null!;
        private Vector2Int _puzzleSize;
        private Vector2 _pieceSize = new(100, 100);
        private Vector2 _offset = new(0.5f, 0.5f);
        public void Init(Vector2Int puzzleSize, PieceModel[,] pieces)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            _puzzleSize = puzzleSize;
            var pSize = rectTransform.rect.size;
            _pieceSize = pSize / puzzleSize;
            _offset = new(- pSize.x / 2 + _pieceSize.x / 2, pSize.y / 2 - _pieceSize.y / 2);
            PuzzlePieceViews = new PuzzlePieceView[puzzleSize.x, puzzleSize.y];
            for (int i = 0; i < puzzleSize.x; i++)
            {
                for (int j = 0; j < puzzleSize.y; j++)
                {
                    //if (enumerator.Current.Sprite is null) continue;
                    CreatePiece(pieces[i, j], new Vector2Int(i, j));
                }
            }
        }

        private PuzzlePieceView CreatePiece(in PieceModel piece, Vector2Int coords)
        {
            var pieceView = PuzzlePieceViews[coords.x, coords.y] = Instantiate(puzzlePieceViewPrefab, transform).GetComponent<PuzzlePieceView>();
            pieceView.Init(piece.Id, piece.Sprite, piece.IsCanMove, _pieceSize);
            pieceView.rectTransform.localPosition = GetPosition(coords.x, coords.y);
            pieceView.gameObject.SetActive(!piece.IsEmpty);
            return pieceView;
        }

        public void SwapPiece(Vector2Int from, Vector2Int to)
        {
            var fromPiece = PuzzlePieceViews[from.x, from.y];
            var toPiece = PuzzlePieceViews[to.x, to.y];
            toPiece.rectTransform.localPosition = GetPosition(from.x, from.y);
            // 目标位置
            Vector2 targetPos = GetPosition(to.x, to.y);
            fromPiece.PlayMove(targetPos, 0.25f);
            // 数组交换
            (PuzzlePieceViews[from.x, from.y], PuzzlePieceViews[to.x, to.y]) = (toPiece, fromPiece);
        }

        public void Wim()
        {
            foreach (var piece in PuzzlePieceViews)
            {
                if (!piece.gameObject.activeSelf)
                {
                    piece.gameObject.SetActive(true);
                    piece.PlayCompleteSummon(0.2f);
                }
                else
                {
                    piece.PlayComplete(0.2f);
                }
            }
            
        }
        private Vector2 GetPosition(int i, int j)
        {
            return _offset + new Vector2(i, - j) * _pieceSize;
        }
    }
}