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
            // 自动切割大图 → 小Sprite列表
            var pieces = CutTextureToSprites(texture2D, puzzleSize);
            SetRandomFixedPieces(pieces, 3);
            ShufflePieces(pieces, shuffleSteps);
            // 初始化 Model
            _puzzleModel.SetPuzzle(pieces);
            // 初始化 View
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
            if (_puzzleModel[coords].IsEmpty || !_puzzleModel[coords].IsCanMove) return;
            for (int i = 0; i < _directions.Length; i++)
            {
                var checkCoords = coords + _directions[i];
                if (checkCoords.x < 0 || checkCoords.x >= _puzzleModel.Size.x ||
                    checkCoords.y < 0 || checkCoords.y >= _puzzleModel.Size.y ||
                    !_puzzleModel[checkCoords].IsEmpty) continue;
                _puzzleModel.Swap(coords, checkCoords);
                puzzleView.SwapPiece(coords, checkCoords);
                AudioManager.Instance.PlaySfx("move");
                break;
            }
            if (CheckWin())
            {
                Debug.Log("Win!");
                _puzzleModel.FillAll();
                puzzleView.Wim();
            }
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
        private PieceModel[,] CutTextureToSprites(
            Texture2D tex, 
            Vector2Int size)
        {
            int cols = size.x; // 横向数量
            int rows = size.y; // 纵向数量

            float pieceWidth = tex.width / (float)cols;   // 每块宽度
            float pieceHeight = tex.height / (float)rows; // 每块高度
            var pieces = new PieceModel[cols, rows];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int id = x + y * cols;

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
                    pieces[x, y] = new PieceModel
                    {
                        Id = id,
                        Sprite = sprite,
                        IsEmpty = id == 0,
                        IsCanMove = true
                    };
                }
            }
            return pieces;
        }
        
        /// <summary>
        /// 打乱拼图（保证可解，模拟随机移动空白块）
        /// </summary>
        private void ShufflePieces(
            PieceModel[,] pieces, 
            int shuffleSteps = 1)
        {
            int cols = pieces.GetLength(0);
            int rows = pieces.GetLength(1);
            
            Vector2Int emptyPos = FindEmptyPosition(pieces);
            
            for (int i = 0; i < shuffleSteps; i++)
            {
                // 随机一个方向
                var dir = _directions[Random.Range(0, _directions.Length)];
                int nx = emptyPos.x + dir.x;
                int ny = emptyPos.y + dir.y;

                // 判断是否在范围内
                if (nx < 0 || nx >= cols || ny < 0 || ny >= rows || !pieces[nx, ny].IsCanMove)
                    continue;
                
                // 交换空白块和相邻块
                (pieces[emptyPos.x, emptyPos.y], pieces[nx, ny]) = (pieces[nx, ny], pieces[emptyPos.x, emptyPos.y]);
                emptyPos = new Vector2Int(nx, ny);
            }
        }
        
        /// <summary>
        /// 找到空白块 Id = -1
        /// </summary>
        private Vector2Int FindEmptyPosition(PieceModel[,] pieces)
        {
            int cols = pieces.GetLength(0);
            int rows = pieces.GetLength(1);
            for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
                if (pieces[x, y].IsEmpty)
                    return new Vector2Int(x, y);

            return Vector2Int.zero;
        }
        
        /// <summary>
        /// 随机指定数量的拼图块设为固定（无法移动），直接在pieces数组上操作
        /// </summary>
        /// <param name="pieces">拼图数据数组</param>
        /// <param name="count">固定块数量</param>
        public void SetRandomFixedPieces(PieceModel[,] pieces, int count)
        {
            if (count <= 0 || pieces == null) return;
    
            int cols = pieces.GetLength(0);
            int rows = pieces.GetLength(1);
    
            // 收集所有可用的拼图块（非空白且当前可移动）
            var availableIndices = new List<(int x, int y)>();
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    var piece = pieces[x, y];
                    // 忽略空白块，只选可移动的正常块
                    if (!piece.IsEmpty && piece.IsCanMove)
                    {
                        availableIndices.Add((x, y));
                    }
                }
            }
    
            // 限制数量不超过可用块数
            count = Mathf.Min(count, availableIndices.Count);
            if (count == 0) return;
    
            // Fisher-Yates 洗牌，保证随机且不重复
            for (int i = availableIndices.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (availableIndices[i], availableIndices[j]) = (availableIndices[j], availableIndices[i]);
            }
    
            // 设置前count个为固定块
            for (int i = 0; i < count; i++)
            {
                var (x, y) = availableIndices[i];
                pieces[x, y].IsCanMove = false;
            }
        }
    }
}