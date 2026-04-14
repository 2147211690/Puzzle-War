using System.Collections.Generic;
using System.Linq;
using Models;
using UnityEngine;
using Views;

namespace Controllers
{
    public class PuzzleController : MonoBehaviour
    {
        public PuzzleView puzzleView = null!;
        public int shuffleSteps = 150;
        private PuzzleModel _puzzleModel = null!;
        private Vector2Int[] _directions = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };

        public void Init(Vector2Int puzzleSize, Texture2D texture2D)
        {
            _puzzleModel = new PuzzleModel(puzzleSize);
            _puzzleModel.StepCount = 0;

            // 自动切割大图 → 小Sprite列表
            var pieces = CutTextureToSprites(texture2D, puzzleSize);
            ShufflePieces(pieces, shuffleSteps);
            for (int i = 0; i < puzzleSize.x; i++)
            {
                for (int j = 0; j < puzzleSize.y; j++)
                {
                    _puzzleModel[i, j] = pieces[i, j].Id;
                }
            }
            // 传给 View
            puzzleView.Init(puzzleSize, pieces);
            RegesterClickPiece();
            AudioManager.Instance.PlayBGM("bgm");
        }

        private void RegesterClickPiece()
        {
            foreach (var pieceView in puzzleView.PuzzlePieceViews)
            {
                pieceView?.AddClickEvent(OnClickPiece);
            }
        }

        private void OnClickPiece(int id)
        {
            var coords = _puzzleModel[id];
            for (int i = 0; i < _directions.Length; i++)
            {
                var checkCoords = coords + _directions[i];
                if (checkCoords.x < 0 || checkCoords.x >= _puzzleModel.Size.x ||
                    checkCoords.y < 0 || checkCoords.y >= _puzzleModel.Size.y ||
                    _puzzleModel[checkCoords] != -1) continue;
                _puzzleModel[checkCoords] = id;
                _puzzleModel.StepCount++;
                puzzleView.MovePiece(coords, checkCoords);
                AudioManager.Instance.PlaySfx("move");
                break;
            }
            if (CheckWin())
            {
                Debug.Log("Win!");
                puzzleView.Wim();
            }
        }

        private bool CheckWin()
        {
            for (int i = 0; i < _puzzleModel.Size.x; i++)
            {
                for (int j = 0; j < _puzzleModel.Size.y; j++)
                {
                    if (_puzzleModel[i, j] != -1 && _puzzleModel[i, j] != i + j * _puzzleModel.Size.x) return false;
                }
            }
            return true;
        }
        private (int Id, Sprite? Sprite)[,] CutTextureToSprites(
            Texture2D tex, 
            Vector2Int size)
        {
            int cols = size.x; // 横向数量
            int rows = size.y; // 纵向数量

            float pieceWidth = tex.width / (float)cols;   // 每块宽度
            float pieceHeight = tex.height / (float)rows; // 每块高度
            var pieces = new (int Id, Sprite? Sprite)[cols, rows];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int index = x + y * cols;

                    // 计算小图区域
                    Rect rect = new Rect(
                        x * pieceWidth,
                        (rows - 1 - y) * pieceHeight, // 翻转Y轴，保证图片方向正确
                        pieceWidth,
                        pieceHeight
                    );
                    rect.x = Mathf.Clamp(rect.x, 0, tex.width);
                    rect.y = Mathf.Clamp(rect.y, 0, tex.height);
                    rect.width = Mathf.Min(rect.width, tex.width - rect.x);
                    rect.height = Mathf.Min(rect.height, tex.height - rect.y);
                    // 生成Sprite
                    Sprite sprite = Sprite.Create(
                        tex,
                        rect,
                        new Vector2(0.5f, 0.5f),
                        1f
                    );
                    if (index == 0) pieces[x, y] = (-1, null);
                    else pieces[x, y] = (index, sprite);
                }
            }
            return pieces;
        }
        
        /// <summary>
        /// 打乱拼图（保证可解，模拟随机移动空白块）
        /// </summary>
        private void ShufflePieces(
            (int Id, Sprite? Sprite)[,] pieces, 
            int shuffleSteps = 1)
        {
            int cols = pieces.GetLength(0);
            int rows = pieces.GetLength(1);

            // 找到空白块位置（Id = -1）
            Vector2Int emptyPos = FindEmptyPosition(pieces);
            
            for (int i = 0; i < shuffleSteps; i++)
            {
                // 随机一个方向
                var dir = _directions[Random.Range(0, _directions.Length)];
                int nx = emptyPos.x + dir.x;
                int ny = emptyPos.y + dir.y;

                // 判断是否在范围内
                if (nx < 0 || nx >= cols || ny < 0 || ny >= rows)
                    continue;

                // 交换空白块和相邻块
                (pieces[emptyPos.x, emptyPos.y], pieces[nx, ny]) = (pieces[nx, ny], pieces[emptyPos.x, emptyPos.y]);
                
                emptyPos = new Vector2Int(nx, ny);
            }
        }
        
        /// <summary>
        /// 找到空白块 Id = -1
        /// </summary>
        private Vector2Int FindEmptyPosition((int Id, Sprite? Sprite)[,] pieces)
        {
            int cols = pieces.GetLength(0);
            int rows = pieces.GetLength(1);
            for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
                if (pieces[x, y].Id == -1)
                    return new Vector2Int(x, y);

            return Vector2Int.zero;
        }
    }
}