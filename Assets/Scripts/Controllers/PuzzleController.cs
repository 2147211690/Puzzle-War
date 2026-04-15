using System.Collections.Generic;
using System.Linq;
using Models;
using Tools;
using UnityEngine;
using Views;

namespace Controllers
{
    public class PuzzleController : MonoBehaviour
    {
        public PuzzleView puzzleView = null!;
        public int shuffleSteps = 150;
        private PuzzleModel _puzzleModel = null!;

        
        public void Init(Vector2Int puzzleSize, Texture2D texture2D)
        {
            // 自动切割大图 → 小Sprite列表
            _puzzleModel = PuzzleTools.CutTextureToSprites(texture2D, puzzleSize)
                .RandomPiecesType(3,2,2)
                .RandomBarriers(5)
                .ShufflePieces(shuffleSteps);
            // 初始化 View
            puzzleView.Init(_puzzleModel);
            RegesterClickPiece();
            
            AudioManager.Instance.PlayBGM("bgm");
        }

        private void RegesterClickPiece()
        {
            foreach (var pieceView in puzzleView.PuzzlePieceViews)
            {
                pieceView.AddClickEvent(OnClickPiece);
            }
        }

        private void OnClickPiece(int id)
        {
            var coords = _puzzleModel[id];
            if (!TrySwapCoords(coords)) return;
            AudioManager.Instance.PlaySfx("move");
            if (CheckWin())
            {
                Debug.Log("Win!");
                _puzzleModel.FillAll();
                puzzleView.Wim();
            }
        }

        private bool TrySwapCoords(Vector2Int coords)
        {
            if (!coords.InSize(_puzzleModel.Size)) return false;
            var piece = _puzzleModel[coords];
            if (piece.IsEmpty || piece.Type == PieceTypeEnum.Fixed) return false;
            var dir = PuzzleTools.GetTypePieceMoveDirections(piece.Type);
            for (int i = 0; i < dir.Count; i++)
            {
                var checkCoords = coords + dir[i];
                if (!checkCoords.InSize(_puzzleModel.Size) ||
                    !_puzzleModel[checkCoords].IsEmpty ||
                    _puzzleModel.IsBarrier(coords, checkCoords)) continue;
                _puzzleModel.Swap(coords, checkCoords);
                puzzleView.SwapPiece(coords, checkCoords);
                _puzzleModel.StepCount++;
                return true;
            }
            return false;
        }

        private bool CheckWin()
        {
            for (int i = 0; i < _puzzleModel.Size.x; i++)
            {
                for (int j = 0; j < _puzzleModel.Size.y; j++)
                {
                    if (!_puzzleModel[i, j].IsEmpty && _puzzleModel[i, j].Id != i + j * _puzzleModel.Size.x) return false;
                }
            }
            return true;
        }
        
    }
}