using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Views
{
    public class PuzzleView : MonoBehaviour
    {
        public GameObject puzzlePieceViewPrefab = null!;
        public GameObject backNumberPrefab = null!;
        public GameObject barrierPrefab = null!;
        public Material blurMaterial = null!;
        
        public PuzzlePieceView[,] PuzzlePieceViews { get; set; } = null!;
        public Image previewImage = null!;
        public Button sideBarButton = null!;
        public Dictionary<Barrier, BarrierView> BarrierViews { get; set; } = new();
        public RectTransform rectTransform = null!;
        public GameObject numbers = null!;
        public GameObject pieces = null!;
        public GameObject barriers = null!;
        public EventHandler? WinComplete;
        public event EventHandler<ToolTypeEnum>? ToolClicked;
        private Vector2Int _puzzleSize;
        private Vector2 _pieceSize = new(100, 100);
        private Vector2 _offset = new(0.5f, 0.5f);
        public void SetPuzzleGame(PuzzleModel puzzleModel)
        {
            Clear();
            _puzzleSize = puzzleModel.Size;
            var pSize = rectTransform.rect.size;
            _pieceSize = pSize / _puzzleSize;
            _offset = new(- pSize.x / 2 + _pieceSize.x / 2, pSize.y / 2 - _pieceSize.y / 2);
            PuzzlePieceViews = new PuzzlePieceView[_puzzleSize.x, _puzzleSize.y];
            for (int i = 0; i < _puzzleSize.x; i++)
            {
                for (int j = 0; j < _puzzleSize.y; j++)
                {
                    //if (enumerator.Current.Sprite is null) continue;
                    var piece = puzzleModel[i, j];
                    CreatePiece(piece, new Vector2Int(i, j), puzzleModel.GetSprite(piece.Id));
                    CreateBackNumber(i + j * _puzzleSize.x + 1, new Vector2Int(i, j));
                }
            }
            foreach (var barrier in puzzleModel.Barriers)
            {
                CreateBarrier(barrier);
            }
        }

        public void SetPuzzlePreview(PuzzleModel puzzleModel, bool isUnlocked)
        {
            Clear();
            previewImage.enabled = true;
            previewImage.sprite = Sprite.Create(puzzleModel.Texture2D, 
                new Rect(0, 0, puzzleModel.Texture2D.width, puzzleModel.Texture2D.height), 
                new Vector2(0.5f, 0.5f), 100);
            previewImage.material = isUnlocked ? null! : blurMaterial;
        }
        public void OnToolClicked(int toolType) => ToolClicked?.Invoke(this, (ToolTypeEnum)toolType);
        public void Clear()
        {
            BarrierViews.Clear();
            var numberCount = numbers.transform.childCount;
            for (int i = 0; i < numberCount; i++)
            {
                Destroy(numbers.transform.GetChild(i).gameObject);
            }
            var pieceCount = pieces.transform.childCount;
            for (int i = 0; i < pieceCount; i++)
            {
                Destroy(pieces.transform.GetChild(i).gameObject);
            }
            var barrierCount = barriers.transform.childCount;
            for (int i = 0; i < barrierCount; i++)
            {
                Destroy(barriers.transform.GetChild(i).gameObject);
            }

            previewImage.enabled = false;
        }
        private PuzzlePieceView CreatePiece(in PieceModel piece, in Vector2Int coords, Sprite? sprite)
        {
            var pieceView = PuzzlePieceViews[coords.x, coords.y] = Instantiate(puzzlePieceViewPrefab, pieces.transform).GetComponent<PuzzlePieceView>();
            pieceView.Init(piece.Id, sprite, piece.Type, _pieceSize);
            pieceView.rectTransform.localPosition = GetPosition(coords.x, coords.y);
            pieceView.gameObject.SetActive(!piece.IsEmpty);
            return pieceView;
        }
        private TMP_Text CreateBackNumber(int number, in Vector2Int coords)
        {
            var numberText = Instantiate(backNumberPrefab, numbers.transform).GetComponent<TMP_Text>();
            numberText.text = number.ToString();
            numberText.rectTransform.sizeDelta = _pieceSize;
            numberText.rectTransform.localPosition = GetPosition(coords.x, coords.y);
            return numberText;
        }

        private BarrierView CreateBarrier(in Barrier barrier)
        {
            var barrierView = Instantiate(barrierPrefab, barriers.transform).GetComponent<BarrierView>();
            barrierView.Barrier = barrier;
            barrierView.rectTransform.sizeDelta = _pieceSize;
            barrierView.rectTransform.localPosition = GetBarrierPosition(barrier);
            var normalVec = barrier.NormalVec;
            barrierView.rectTransform.rotation = Quaternion.Euler(0, 0, normalVec.x * 90);
            BarrierViews[barrier] = barrierView;
            return barrierView;
        }
        public void RemoveBarrier(in Barrier barrier)
        {
            Destroy(BarrierViews[barrier].gameObject);
            BarrierViews.Remove(barrier);
        }
        public void SwapPiece(in Vector2Int from, in Vector2Int to)
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

        public void Wim(float animTime)
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
            foreach (var barrier in BarrierViews.Values)
            {
                barrier.PlayComplete(0.2f);
            }
            StartCoroutine(WimAnim(animTime));
        }
        
        private IEnumerator WimAnim(float animTime)
        {
            yield return new WaitForSeconds(animTime);
            WinComplete?.Invoke(this, EventArgs.Empty);
        }
        private Vector2 GetPosition(int i, int j)
        {
            return _offset + new Vector2(i, - j) * _pieceSize;
        }

        private Vector2 GetBarrierPosition(in Barrier barrier)
        {
            return (GetPosition(barrier.Coords1.x, barrier.Coords1.y) +
                    GetPosition(barrier.Coords2.x, barrier.Coords2.y)) / 2;
        }
    }
}